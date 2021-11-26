using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharaBase;
using static SceneMng;

// 探索中/戦闘中問わず、キャラクターに関連するものを管理する

// Chara.csをインスタンスするときに外部データのキャラデータをその前に読み込んでおいて、newの引数に入れて渡すようにする
// そうしたら、各キャラにそれぞれのステータス値を渡せる。はず。たぶん。。。

public class CharacterMng : MonoBehaviour
{
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;

    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public GameObject buttleWarpPointPack;  // 戦闘時にフィールド上の戦闘ポイントにキャラをワープさせる

    //　通常攻撃弾のプレハブ
    [SerializeField]
    private GameObject uniAttackPrefab_;

    public ItemBagMng itemBagMng_;

    CHARACTERNUM oldTurnChar_ = CHARACTERNUM.UNI;     // 前に行動順が回ってきていたキャラクター
    CHARACTERNUM nowTurnChar_ = CHARACTERNUM.MAX;     // 現在行動順が回ってきているキャラクター
    private bool selectFlg_ = false;                  // 敵を選択中かのフラグ
    private bool lastEnemytoAttackFlg_ = false;       // キャラの攻撃対象が最後の敵であるか     

    private Vector3 keepFieldPos_;                    // 戦闘に入る直前のキャラの座標を保存しておく 
    private const int buttleCharMax_ = 2;             // バトル参加可能キャラ数の最大値(最終的には3にする)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // 戦闘時の配置位置を保存しておく変数
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // 戦闘時の回転角度を保存しておく変数(クォータニオン)

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    private Dictionary<CHARACTERNUM, GameObject> charMap_;
    // Chara.csをキャラ毎にリスト化する
    private List<Chara> charasList_ = new List<Chara>();
    // 各キャラのHP情報
    private Dictionary<CHARACTERNUM, (HPMPBar, HPMPBar)> charaHPMPMap_ = new Dictionary<CHARACTERNUM, (HPMPBar, HPMPBar)>();

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // バトル中の案内
    private readonly string[] announceText_ = new string[2] { " 左シフトキー：\n 戦闘から逃げる", " Tキー：\n コマンドへ戻る" };

    private ImageRotate buttleCommandRotate_;                     // バトル中のコマンドUIを取得して、保存しておく変数
    private GameObject[] buttleCommandImage_ = new GameObject[4]; // バトルコマンドの画像4種類
    private EnemySelect buttleEnemySelect_;                       // バトル中の選択アイコン情報
    private ButtleMng buttleMng_;                                 // ButtleMng.csの取得

    private GameObject setMagicObj_;                    // 魔法コマンド選択時に表示させるオブジェクト
    private Image[] magicImage_ = new Image[4];         // 魔法画像の貼り付け先(最大4つ表示になる)

    private int enemyNum_ = 0;                                    // バトル時の敵の数
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // 敵のインスタンス位置の全情報

    private Vector3 charaPos_;                         // キャラクター座標
    private Vector3 enePos_;                           // 目標の敵座標

    private CharaUseMagic useMagic_;

    void Start()
    {
        // SceneMngからキャラの情報をもらう(charMap_とcharasList_)
        charMap_ = SceneMng.charMap_;
        charasList_ = SceneMng.charasList_;

        nowTurnChar_ = CHARACTERNUM.UNI;

        // ワープポイントの数ぶん、for文を回す
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // 個別にワープポイントを変数へ保存していく
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleAnounceText_ = buttleUICanvas.transform.Find("AnnounceText").GetComponent<TMPro.TextMeshProUGUI>();

        var commandImage = buttleUICanvas.transform.Find("Command/Image");
        buttleCommandRotate_ = commandImage.GetComponent<ImageRotate>();
        for(int i = 0; i < commandImage.childCount; i++)
        {
            // コマンド画像4種類を取得
            buttleCommandImage_[i] = commandImage.GetChild(i).gameObject;
        }

        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();

        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();

        setMagicObj_ = buttleUICanvas.transform.Find("SetMagicObj").gameObject;
        // 魔法画像の表示先を設定する
        for (int i = 0; i < magicImage_.Length; i++)
        {
            magicImage_[i] = setMagicObj_.transform.GetChild(i).GetComponent<Image>();
        }
        setMagicObj_.SetActive(false);  // 魔法コマンドを選択するまで非表示にする

        // キャラ名+CharaDataのステータス表から、HP/MP情報を取得する
        charaHPMPMap_[CHARACTERNUM.UNI] = 
            (buttleUICanvas.transform.Find("UniCharaData/HPSlider").GetComponent<HPMPBar>(), buttleUICanvas.transform.Find("UniCharaData/MPSlider").GetComponent<HPMPBar>());
        charaHPMPMap_[CHARACTERNUM.JACK] =
            (buttleUICanvas.transform.Find("JackCharaData/HPSlider").GetComponent<HPMPBar>(), buttleUICanvas.transform.Find("JackCharaData/MPSlider").GetComponent<HPMPBar>());
        // 初期HPを代入
        charaHPMPMap_[CHARACTERNUM.UNI].Item1.SetHPMPBar(charasList_[(int)CHARACTERNUM.UNI].HP(), charasList_[(int)CHARACTERNUM.UNI].MaxHP());
        charaHPMPMap_[CHARACTERNUM.JACK].Item1.SetHPMPBar(charasList_[(int)CHARACTERNUM.JACK].HP(), charasList_[(int)CHARACTERNUM.JACK].MaxHP());
        // 初期MPを代入
        charaHPMPMap_[CHARACTERNUM.UNI].Item2.SetHPMPBar(charasList_[(int)CHARACTERNUM.UNI].MP(), charasList_[(int)CHARACTERNUM.UNI].MaxMP());
        charaHPMPMap_[CHARACTERNUM.JACK].Item2.SetHPMPBar(charasList_[(int)CHARACTERNUM.JACK].MP(), charasList_[(int)CHARACTERNUM.JACK].MaxMP());

        useMagic_ = new CharaUseMagic();
    }

    // ButtleMng.csから敵の数を受け取る
    public void SetEnemyNum(int enemyNum)
    {
        enemyNum_ = enemyNum;

        // 矢印アイコンが表示できるように座標を渡す
        // 一時変数に発生位置をコピーしてそれを代入することで、敵の発生位置の高さが書き換えるのを防ぐ
        List<Vector3> tmpInsPos = new List<Vector3>(enemyInstancePos_[enemyNum_]);
        buttleEnemySelect_.SetPosList(tmpInsPos);

        // NGな書き方
        // この書き方では、元の敵の発生位置座標を書き換える形で矢印アイコンが生成されて、2回目以降敵の発生位置が矢印アイコンの高さになってしまう
        //buttleEnemySelect_.SetPosList(enemyInstancePos_[enemyNum_]);
    }

    // 戦闘開始時に設定される項目(ButtleMng.csで参照)
    public void ButtleSetCallOnce()
    {
        if (buttleUICanvas.gameObject.activeSelf)
        {
            buttleCommandRotate_.ResetRotate();   // UIの回転を一番最初に戻す
        }

        anim_ = ANIMATION.IDLE;
        oldAnim_ = ANIMATION.IDLE;

        buttleAnounceText_.text = announceText_[0];

        // 最初の行動キャラを指定する
        nowTurnChar_ = CHARACTERNUM.UNI;

        // 最初の行動キャラのHPバーを表示する
        //charaHPBar.SetHPBar(charasList_[(int)nowTurnChar_].HP(), charasList_[(int)nowTurnChar_].MaxHP());

        // フラグの初期化を行う
        lastEnemytoAttackFlg_ = false;

        // 戦闘前の座標を保存しておく
        keepFieldPos_ = charMap_[CHARACTERNUM.UNI].gameObject.transform.position;

        // 戦闘用座標と回転角度を代入する
        // キャラの角度を変更は、ButtleWarpPointの箱の角度を回転させると可能。(1体1体向きを変えることもできる)
        foreach (KeyValuePair<CHARACTERNUM, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // ここで座標を保存しておくことで、メニュー画面での並び替えでも反映できるだろうし、
            // 攻撃エフェクトの発生位置の目安になる
            //charSetting[(int)character.Key].buttlePos  = character.Value.gameObject.transform.position;
            charasList_[(int)character.Key].SetButtlePos(character.Value.gameObject.transform.position);

            // 行動順に関連する値を初期化する
            charasList_[(int)character.Key].SetTurnInit();
        }

        useMagic_.Init();
    }

    public (int, string) CharaTurnSpeed(int num)
    {
        return (charasList_[num].Speed(), charasList_[num].Name());
    }

    // キャラの戦闘中に関する処理(ButtleMng.csで参照)
    public void Buttle()
    {
        //Debug.Log(anim_);

        // 死亡していたら
        if (charasList_[(int)nowTurnChar_].GetDeathFlg())
        {
            if (charaHPMPMap_[nowTurnChar_].Item1.GetColFlg())
            {
                return;
            }
            else
            {
                oldAnim_ = anim_;
                Debug.Log("死亡中だから行動を飛ばす");
                anim_ = ANIMATION.IDLE;
                oldAnim_ = ANIMATION.NON;

                // 全滅したか確認する処理
                bool allDeathFlg = true;
                for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                {
                    if (!charasList_[i].GetDeathFlg())
                    {
                        // 1人でも生存状態だと分かればbreakして抜ける
                        allDeathFlg = false;
                        break;
                    }
                }
                // 全滅時は町長の家へ飛ばす
                if (allDeathFlg)
                {
                    EventMng.SetChapterNum(100, SCENE.CONVERSATION);
                }
            }
        }

        // テスト用(レベルアップ処理)
        if (Input.GetKeyDown(KeyCode.L))
        {
            charasList_[0].LevelUp();
            charasList_[1].LevelUp();
        }


        // 戦闘から逃げる
        if (!selectFlg_ && Input.GetKeyDown(KeyCode.LeftShift))
        {
            buttleMng_.CallDeleteEnemy();

            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            charMap_[CHARACTERNUM.UNI].gameObject.transform.position = keepFieldPos_;

            Debug.Log("Uniは逃げ出した");
        }

        // ATTACKで敵選択中に、特定のキー(今はTキー)を押下されたらコマンド選択に戻る
        if (selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            anim_ = ANIMATION.NON;
            selectFlg_ = false;
            buttleCommandRotate_.SetRotaFlg(!selectFlg_);   // コマンド回転を有効化
            buttleAnounceText_.text = announceText_[0];

            // 魔法コマンドが有効だった時
            if(setMagicObj_.activeSelf)
            {
                Debug.Log("魔法コマンドの選択を解除します");
                setMagicObj_.SetActive(false);
                setMagicObj_.GetComponent<ImageRotate>().enabled = false;
                // コマンド画像を表示にする
                for (int i = 0; i < buttleCommandImage_.Length; i++)
                {
                    buttleCommandImage_[i].SetActive(true);
                }
            }
        }

        // キャラ毎のモーションを呼ぶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(setMagicObj_.activeSelf)    // 魔法コマンド選択中のとき
            {
                // 魔法コマンド選択中のとき
                buttleAnounceText_.text = announceText_[1];
                Debug.Log("現在は魔法コマンドが有効コマンドです");

                var tmp = (int)setMagicObj_.GetComponent<ImageRotate>().GetNowCommand() - 1;
                Debug.Log(tmp + "番の魔法を使用しようとしています");

                // 範囲外かの確認をする
                if (charasList_[(int)nowTurnChar_].CheckMagicNum(tmp))
                {
                    // CharaUseMagic.csに情報を渡す
                    useMagic_.CheckUseMagic(charasList_[(int)nowTurnChar_].GetMagicNum(tmp));
                    setMagicObj_.SetActive(false);
                    buttleCommandRotate_.SetRotaFlg(false);
                }
                else
                {
                    Debug.Log("範囲外の魔法番号です。選択しなおしてください");
                }
            }
            else
            {
                // 選択されたコマンドに対する処理
                switch (buttleCommandRotate_.GetNowCommand())
                {
                    case ImageRotate.COMMAND.ATTACK:
                        if (!selectFlg_)
                        {
                            // 自分の行動の前の人が動作終わっているか調べる
                            if (!charasList_[(int)oldTurnChar_].GetIsMove())
                            {
                                if (anim_ == ANIMATION.IDLE || anim_ == ANIMATION.NON)
                                {
                                    anim_ = ANIMATION.BEFORE;
                                }
                            }
                            else
                            {
                                Debug.Log("前のキャラがアニメーション中");
                            }
                        }
                        else
                        {
                            if (anim_ == ANIMATION.BEFORE)
                            {
                                // 防御用の値を0に戻す
                                charasList_[(int)nowTurnChar_].SetBarrierNum();
                                BeforeAttack();    // 攻撃準備
                            }
                        }

                        break;
                    case ImageRotate.COMMAND.MAGIC:

                        if (!selectFlg_)
                        {
                            // 自分の行動の前の人が動作終わっているか調べる
                            if (!charasList_[(int)oldTurnChar_].GetIsMove())
                            {
                                anim_ = ANIMATION.NON;

                                // 防御用の値を0に戻す
                                charasList_[(int)nowTurnChar_].SetBarrierNum();

                                // コマンド画像を非表示にする
                                for(int i = 0; i < buttleCommandImage_.Length; i++)
                                {
                                    buttleCommandImage_[i].SetActive(false);
                                }

                                Debug.Log("魔法コマンドが有効コマンドです");
                                selectFlg_ = true;

                                setMagicObj_.SetActive(true);
                                setMagicObj_.GetComponent<ImageRotate>().enabled = true;
                                buttleCommandRotate_.SetRotaFlg(true);

                                //@ 行動中のキャラに設定された魔法画像を描画する
                                for (int i = 0; i < 4; i++)
                                {
                                    if (charasList_[(int)nowTurnChar_].GetImageTest(i) == null)
                                    {
                                        // nullのときは魔法を設定していないから透過する
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                                    }
                                    else
                                    {
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                                        magicImage_[i].sprite = charasList_[(int)nowTurnChar_].GetImageTest(i);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("前のキャラがアニメーション中");
                            }
                        }
                        else
                        {
                            MagicAttack();
                        }

                        break;
                    case ImageRotate.COMMAND.ITEM:
                        // 防御用の値を0に戻す
                        charasList_[(int)nowTurnChar_].SetBarrierNum();
                        Debug.Log("アイテムコマンドが有効コマンドです");
                        break;
                    case ImageRotate.COMMAND.BARRIER:
                        // 次の自分のターンまで防御力を1.5倍にする
                        charasList_[(int)nowTurnChar_].SetBarrierNum(charasList_[(int)nowTurnChar_].Defence(false) / 2);
                        // 次のキャラor敵に行動が回るようにanim_とoldAnim_を設定する
                        anim_ = ANIMATION.IDLE;
                        oldAnim_ = ANIMATION.NON;

                        Debug.Log("防御コマンドが有効コマンドです");
                        break;
                    default:
                        // 防御用の値を0に戻す
                        charasList_[(int)nowTurnChar_].SetBarrierNum();
                        Debug.Log("無効なコマンドです");
                        break;
                }
            }
        }

        if (oldAnim_ != anim_)
        {
            oldAnim_ = anim_;
            AnimationChange();
        }

        if (anim_ == ANIMATION.ATTACK && charasList_[(int)nowTurnChar_].ChangeNextChara())
        {
            anim_ = ANIMATION.AFTER;
        }
    }

    public void NotMyTurn()
    {
        // ダメージを受けたときのモーションを規定時間で終了させる
        for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            charasList_[i].DamageAnim();
        }
    }

    void AnimationChange()
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:

                // 前のキャラの行動が終わったからターンを移す
                buttleMng_.SetMoveTurn();

                buttleAnounceText_.text = announceText_[0];

                // 次のキャラが行動できるようにする
                // 最大まで加算されたら、初期値に戻す(前演算子重要)
                if (++nowTurnChar_ >= CHARACTERNUM.MAX)
                {
                    nowTurnChar_ = CHARACTERNUM.UNI;
                }

                if (charasList_[(int)nowTurnChar_].HP() <= 0)
                {
                    Debug.Log("キャラが死亡");
                    //anim_ = ANIMATION.DEATH;
                }
                else
                {
                    anim_ = ANIMATION.IDLE;
                }

                selectFlg_ = false;

                //// 矢印位置のリセットを行う(falseなら、敵を全て倒したということなのでフラグを切り替える)
                //lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();

                break;
            case ANIMATION.BEFORE:
                oldTurnChar_ = nowTurnChar_;

                Debug.Log("前のキャラが行動終了");
                selectFlg_ = true;
                buttleAnounceText_.text = announceText_[1];

                buttleCommandRotate_.SetRotaFlg(false);
                buttleEnemySelect_.SetActive(true);

                break;
            case ANIMATION.ATTACK:
                if (charasList_[(int)nowTurnChar_].Attack())
                {
                    // ここでキャラの攻撃力と速度と幸運をButtleMng.csに渡す
                    buttleMng_.SetDamageNum(charasList_[(int)nowTurnChar_].Damage());
                    buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
                    buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

                    AttackStart((int)nowTurnChar_);
                    buttleCommandRotate_.SetRotaFlg(true);
                    buttleEnemySelect_.SetActive(false);
                }
                break;
            case ANIMATION.AFTER:
                AfterAttack((int)nowTurnChar_);
                break;
            default:
                break;
        }
    }

    // 攻撃準備処理
    void BeforeAttack()
    {
        // キャラの位置を取得する
        charaPos_ = charasList_[(int)nowTurnChar_].GetButtlePos();
        // 敵の位置を取得する
        enePos_ = buttleEnemySelect_.GetSelectEnemyPos(buttleEnemySelect_.GetSelectNum()[0]);
        enePos_.y = 0.0f;        // ここで0.0fにしないと斜め上方向に飛んでしまう

        // 行動中のキャラが、攻撃対象の方向に体を向ける
        // charMap_の情報を直接変更する必要があるため、charMap_[nowTurnChar_]と記述している
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos_ - charaPos_);


        if ((int)nowTurnChar_ == (int)CHARACTERNUM.JACK)
        {
            // 敵に向かって走る処理
            StartCoroutine(MoveToEnemyPos());
        }
        else
        {
            anim_ = ANIMATION.ATTACK;    // ユニ
        }

    }

    // 攻撃への移動コルーチン  
    private IEnumerator MoveToEnemyPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTimeだけだと移動が速すぎるため、任意の値で割る

            var tmp = charasList_[(int)nowTurnChar_].RunMove(time, charMap_[nowTurnChar_].transform.localPosition, enePos_);
            flag = tmp.Item2;   // while文を抜けるかフラグを代入する
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // キャラ座標を代入する

            //Debug.Log("ジャック現在値" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.ATTACK;    // 攻撃モーション移行確認切替

    }

    void AfterAttack(int charNum)
    {
        // キャラの位置を取得する
        charaPos_ = charasList_[charNum].GetButtlePos();

        if (charNum == (int)CHARACTERNUM.JACK)
        {
            // 元いた位置に戻る処理
            StartCoroutine(MoveToInitPos());
        }
        else
        {
            anim_ = ANIMATION.IDLE;    // ユニ
        }
    }

    // 攻撃から戻ってくるコルーチン  
    private IEnumerator MoveToInitPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTimeだけだと移動が速すぎるため、任意の値で割る

            var tmp = charasList_[(int)nowTurnChar_].BackMove(time, charMap_[nowTurnChar_].transform.localPosition, buttleWarpPointsPos_[(int)CHARACTERNUM.JACK]);
            flag = tmp.Item2;   // while文を抜けるかフラグを代入する
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // キャラ座標を代入する

            //Debug.Log("ジャック現在値" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.IDLE;

    }


    void AttackStart(int charNum)
    {
        string str = "";

        if (charNum == (int)CHARACTERNUM.UNI)
        {
            // 通常攻撃弾の方向の計算
            var dir = (enePos_ - charaPos_).normalized;
            // エフェクトの発生位置高さ調整
            var adjustPos = new Vector3(charaPos_.x, charaPos_.y + 0.5f, charaPos_.z);

            // 通常攻撃弾プレハブをインスタンス化
            var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);
            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // 通常攻撃弾の飛んでいく方向を指定
            magicMove.SetDirection(dir);

            // 名前の設定
            str = "UniAttack(Clone)";
        }
        else if (charNum == (int)CHARACTERNUM.JACK)
        {
            // 名前の設定
            str = "Axe1h";
        }
        else
        {
            return;  // 何も処理を行わない
        }

        if (str == "")
        {
            Debug.Log("エラー：文字が入っていません");
            return; // 文字が入っていない場合はreturnする
        }

        // [Weapon]のタグがついているオブジェクトを全て検索する
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // 見つけたオブジェクトの名前を比較して、今回攻撃に扱う武器についているCheckAttackHit関数の設定を行う
            if (weaponTagObj[i].name == str)
            {
                // 武器コライダーの有効化
                if (str == "Axe1h")
                {
                    weaponTagObj[i].GetComponent<BoxCollider>().enabled = true;
                }

                // 選択した敵の番号を渡す
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(buttleEnemySelect_.GetSelectNum()[0] + 1);
            }
        }

    }

    void MagicAttack()
    {
        // キャラの位置を取得する
        charaPos_ = charasList_[(int)nowTurnChar_].GetButtlePos();

        // 速度と幸運を渡す
        buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
        buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

        // MP減少処理
        int MPdecrease = useMagic_.MPdecrease();
        StartCoroutine(charaHPMPMap_[nowTurnChar_].Item2.MoveSlideBar
        (charasList_[(int)nowTurnChar_].MP() - MPdecrease));
        // 内部数値の変更を行う
        charasList_[(int)nowTurnChar_].SetMP(charasList_[(int)nowTurnChar_].MP() - MPdecrease);

        // 魔法での攻撃対象を決定したときに入る
        // 選択した敵の番号を渡す
        var tmp = buttleEnemySelect_.GetSelectNum();
        //int[] tmp = { 0, 0, 0, 0 }; // デバッグ用(好きな数値で攻撃対象を決められる)
        for(int i = 0; i < tmp.Length; i++)
        {
            // tmp内容が-1以外なら攻撃対象がいるので処理を行う
            if(tmp[i] >= 0)
            {
                // 敵の位置を取得する
                enePos_ = buttleEnemySelect_.GetSelectEnemyPos(tmp[i]);
                enePos_.y = 0.0f;        // ここで0.0fにしないと斜め上方向に飛んでしまう
                // 情報を設定する
                useMagic_.InstanceMagicInfo(charaPos_, enePos_, tmp[i], i);
            }
        }

        StartCoroutine(useMagic_.InstanceMagicCoroutine());

        buttleCommandRotate_.SetRotaFlg(true);
        buttleEnemySelect_.SetActive(false);

        // IDLEに切り替わると敵の死亡処理の判定を行うので、間を開けるようにコルーチンで調節する
        StartCoroutine(ChangeIDLETiming());
    }

    private IEnumerator ChangeIDLETiming()
    {
        Debug.Log("スタート");
        yield return new WaitForSeconds(3.0f);
        Debug.Log("スタートから3秒後");

        anim_ = ANIMATION.IDLE;
    }

    // ButtleMng.csで参照
    public bool GetLastEnemyToAttackFlg()
    {
        // 矢印位置のリセットを行う(falseなら、敵を全て倒したということなのでフラグを切り替える)
        lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();
        return lastEnemytoAttackFlg_;
    }

    // ButtleMng.csで参照
    public bool GetSelectFlg()
    {
        return selectFlg_;
    }

    public void SetCharaFieldPos()
    {
        charMap_[CHARACTERNUM.UNI].gameObject.transform.position = keepFieldPos_;
    }

    public void HPdecrease(int num)
    {
        // ダメージ値の算出
        var damage = 0;

        // クリティカルの計算をする(基礎値と幸運値で上限を狭める)
        int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
        if(criticalRand <= 10 + buttleMng_.GetLuckNum())
        {
            // クリティカル発生(必中+ダメージ2倍)10はクリティカルの基礎値
            Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "なので、敵の攻撃がクリティカル！");
            // クリティカルダメージ
            damage = (buttleMng_.GetDamageNum() * 2) - charasList_[num].Defence(true);
        }
        else
        {
            // クリティカルじゃないとき
            Debug.Log(criticalRand + ">" + (10 + charasList_[(int)nowTurnChar_].Luck()) + "なので、敵の攻撃はクリティカルではない");

            // 命中計算をする
            // ①攻撃する側のSpeed / 攻撃される側のSpeed * 100 = ％の出力
            var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)charasList_[(int)nowTurnChar_].Speed() * 100.0f);
            // ②キャラも敵も+10％の補正値を入れる。
            var hitProbabilityOffset = hitProbability + 10;
            // ③hitProbabilityOffsetが100以上なら自動命中で、それ以下ならランダム値を取る。
            if (hitProbabilityOffset < 100)
            {
                int rand = Random.Range(0, 100);
                Debug.Log("命中率" + hitProbabilityOffset + "ランダム値" + rand);

                if (rand <= hitProbabilityOffset)
                {
                    // 命中
                    Debug.Log(rand + "<=" + hitProbabilityOffset + "なので、命中");
                }
                else
                {
                    // 回避
                    Debug.Log(rand + ">" + hitProbabilityOffset + "なので、回避");
                    return;
                }
            }
            else
            {
                Debug.Log("命中率" + hitProbabilityOffset + "が100以上ならので、自動命中");
            }

            int tmpLuck = 0;

            // 命中時にはLuckで回避判定をする
            // 判定の範囲は、100 - 現在のLuckを最大値にして、より回避成功に近づける
            if (charasList_[(int)nowTurnChar_].Luck() <= 10)
            {
                tmpLuck = 10;
                Debug.Log("Luckが10以下なので、10を適用して回避判定をします");
            }
            else
            {
                tmpLuck = charasList_[(int)nowTurnChar_].Luck();
                Debug.Log("Luckが10以上なので、現在のステータスのLuckを使って回避判定をします");
            }

            int randLuck = Random.Range(0, 100 - tmpLuck);
            if (randLuck <= tmpLuck)
            {
                Debug.Log(randLuck + "<=" + tmpLuck + "以下なので、回避成功");
                return;
            }
            else
            {
                Debug.Log(randLuck + ">" + tmpLuck + "以下なので、回避失敗");
                damage = buttleMng_.GetDamageNum() - charasList_[num].Defence(true);
            }
        }

        // 通常ダメージ
        if (damage <= 0)
        {
            Debug.Log("敵の攻撃力よりキャラの防御力が上回ったのでダメージが1になりました");
            damage = 1;
        }

        // キャラのHPを削る(スライドバー変更)
        StartCoroutine(charaHPMPMap_[(CHARACTERNUM)num].Item1.MoveSlideBar(charasList_[num].HP() - damage));

        // 内部数値の変更を行う
        charasList_[num].SetHP(charasList_[num].HP() - damage);

        if (charasList_[num].HP() <= 0)
        {
            Debug.Log("キャラが死亡");
            charasList_[num].SetHP(0);
            //anim_ = ANIMATION.DEATH;
            // Chara.csに死亡情報を入れる
            charasList_[num].SetDeathFlg(true);
        }
    }
}
