using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SceneMng;

// 探索中/戦闘中問わず、キャラクターに関連するものを管理する

// Chara.csをインスタンスするときに外部データのキャラデータをその前に読み込んでおいて、newの引数に入れて渡すようにする
// そうしたら、各キャラにそれぞれのステータス値を渡せる。はず。たぶん。。。

public class CharacterMng : MonoBehaviour
{
    enum ANIMATION
    {
        NON,
        IDLE,
        BEFORE,
        ATTACK,
        AFTER
    };

    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;

    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public HPBar charaHPBar;               // キャラクター用のHPバー
    public GameObject buttleWarpPointPack;  // 戦闘時にフィールド上の戦闘ポイントにキャラをワープさせる

    //　通常攻撃弾のプレハブ
    [SerializeField]
    private GameObject uniAttackPrefab_;

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

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // バトル中の案内
    private readonly string[] announceText_ = new string[2]{ " 左シフトキー：\n 戦闘から逃げる", " Tキー：\n コマンドへ戻る" };

    private ImageRotate buttleCommandRotate_;                     // バトル中のコマンドUIを取得して、保存しておく変数
    private EnemySelect buttleEnemySelect_;                       // バトル中の選択アイコン情報

    private int enemyNum_ = 0;                                    // バトル時の敵の数
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // 敵のインスタンス位置の全情報

    private Vector3 charaPos_;                         // キャラクター座標
    private Vector3 enePos_;                           // 目標の敵座標

    void Start()
    {
        // SceneMngからキャラの情報をもらう(charMap_とcharasList_)
        charMap_ = SceneMng.charMap_;
        charasList_ = SceneMng.charasList_;

        // テスト用
        //charasList_[0].sethp(charasList_[0].HP() - 10);
        //Debug.Log("HHHPPP"+charasList_[0].HP());
        //SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);

        nowTurnChar_ = CHARACTERNUM.UNI;

        // ワープポイントの数ぶん、for文を回す
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // 個別にワープポイントを変数へ保存していく
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleAnounceText_ = buttleUICanvas.transform.Find("AnnounceText").GetComponent<TMPro.TextMeshProUGUI>();

        buttleCommandRotate_ = buttleUICanvas.transform.Find("Command").transform.Find("Image").GetComponent<ImageRotate>();
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();


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
        charaHPBar.SetHPBar(charasList_[(int)nowTurnChar_].HP(), charasList_[(int)nowTurnChar_].MaxHP());

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
    }

    // キャラの戦闘中に関する処理(ButtleMng.csで参照)
    public void Buttle()
    {
        // テスト用(ダメージ処理)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 現在行動中のキャラのHPを削る(スライドバー変更)
            StartCoroutine(charaHPBar.MoveSlideBar(charasList_[(int)nowTurnChar_].HP() - 10));
            // 内部数値の変更を行う
            charasList_[(int)nowTurnChar_].sethp(charasList_[(int)nowTurnChar_].HP() - 10);
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
            // 敵オブジェクトを削除する(タグ検索)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(obj);
            }
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
        }

        // キャラ毎のモーションを呼ぶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 選択されたコマンドに対する処理
            switch(buttleCommandRotate_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:
                    if(!selectFlg_)
                    {
                        // 自分の行動の前の人が動作終わっているか調べる
                        if (!charasList_[(int)oldTurnChar_].GetIsMove())
                        {
                            if(anim_ == ANIMATION.IDLE || anim_ == ANIMATION.NON)
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
                            BeforeAttack((int)nowTurnChar_);    // 攻撃準備
                        }
                    }

                    break;
                case ImageRotate.COMMAND.MAGIC:
                    Debug.Log("魔法コマンドが有効コマンドです");
                    break;
                case ImageRotate.COMMAND.ITEM:
                    Debug.Log("アイテムコマンドが有効コマンドです");
                    break;
                case ImageRotate.COMMAND.BARRIER:
                    Debug.Log("防御コマンドが有効コマンドです");
                    break;
                default:
                    Debug.Log("無効なコマンドです");
                    break;
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

    void AnimationChange()
    {
        switch(anim_)
        {
            case ANIMATION.IDLE:
                buttleAnounceText_.text = announceText_[0];

                // 次のキャラが行動できるようにする
                // 最大まで加算されたら、初期値に戻す(前演算子重要)
                if (++nowTurnChar_ >= CHARACTERNUM.MAX)
                {
                    nowTurnChar_ = CHARACTERNUM.UNI;
                }

                // 次の行動キャラのHPバーを表示する
                charaHPBar.SetHPBar(charasList_[(int)nowTurnChar_].HP(), charasList_[(int)nowTurnChar_].MaxHP());

                selectFlg_ = false;

                // 矢印位置のリセットを行う(falseなら、敵を全て倒したということなのでフラグを切り替える)
                lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();

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
    void BeforeAttack(int charNum)
    {
        // キャラの位置を取得する
        charaPos_ = charasList_[charNum].GetButtlePos();
        // 敵の位置を取得する
        enePos_ = buttleEnemySelect_.GetSelectEnemyPos();
        enePos_.y = 0.0f;        // ここで0.0fにしないと斜め上方向に飛んでしまう

        // 行動中のキャラが、攻撃対象の方向に体を向ける
        // charMap_の情報を直接変更する必要があるため、charMap_[nowTurnChar_]と記述している
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos_ - charaPos_);


        if (charNum == (int)CHARACTERNUM.JACK)
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

            var tmp = charasList_[(int)nowTurnChar_].RunMove(time,charMap_[nowTurnChar_].transform.localPosition, enePos_);
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
        else if(charNum == (int)CHARACTERNUM.JACK)
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
            if(weaponTagObj[i].name == str)
            {
                // 武器コライダーの有効化
                if (str == "Axe1h")
                {
                    weaponTagObj[i].GetComponent<BoxCollider>().enabled = true;
                }

                // 選択した敵の番号を渡す
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(buttleEnemySelect_.GetSelectNum() + 1);
            }
        }

    }

    // ButtleMng.csで参照
    public bool GetLastEnemyToAttackFlg()
    {
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
}
