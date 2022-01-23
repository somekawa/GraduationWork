using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharaBase;
using static SceneMng;

// 探索中/戦闘中問わず、キャラクターに関連するものを管理する
public class CharacterMng : MonoBehaviour
{
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;

    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public GameObject buttleWarpPointPack;  // 戦闘時にフィールド上の戦闘ポイントにキャラをワープさせる

    //　通常攻撃弾のプレハブ
    [SerializeField]
    private GameObject uniAttackPrefab_;

    CHARACTERNUM oldTurnChar_ = CHARACTERNUM.UNI;     // 前に行動順が回ってきていたキャラクター
    CHARACTERNUM nowTurnChar_ = CHARACTERNUM.MAX;     // 現在行動順が回ってきているキャラクター
    private bool selectFlg_ = false;                  // 敵を選択中かのフラグ
    private bool lastEnemytoAttackFlg_ = false;       // キャラの攻撃対象が最後の敵であるか     

    private const int buttleCharMax_ = 2;             // バトル参加可能キャラ数の最大値(最終的には3にする)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // 戦闘時の配置位置を保存しておく変数
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // 戦闘時の回転角度を保存しておく変数(クォータニオン)

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    private Dictionary<CHARACTERNUM, GameObject> charMap_;
    // Chara.csをキャラ毎にリスト化する
    private List<Chara> charasList_ = new List<Chara>();
    // 各キャラのバステアイコン
    private GameObject[] charaBstIconImage_ = new GameObject[(int)CHARACTERNUM.MAX];

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // バトル中の案内
    private readonly string[] announceText_ = new string[2] { " 左シフトキー：\n 戦闘から逃げる", " Tキー：\n コマンドへ戻る" };

    private ImageRotate magicButtleCommandRotate_;                // 魔法用のImageRotate
    private ImageRotate buttleCommandRotate_;                     // バトル中のコマンドUIを取得して、保存しておく変数
    private GameObject buttleCommandFrame_;                       // 大枠のframe部分の画像
    private GameObject[] buttleCommandImage_ = new GameObject[4]; // バトルコマンドの画像4種類
    private EnemySelect buttleEnemySelect_;                       // バトル中の選択アイコン情報
    private GameObject buttleItemBackButtonObj_;                  // バトル中のアイテム画面から戻るアイコン
    private GameObject buttleDamageIconsObj_;                     // バトル中のダメージアイコンの親オブジェクト
    private Vector3[] buttleDamgeIconPopUpPos_ = new Vector3[2];  // ダメージアイコンの表示位置
    private GameObject buttleMagicInfoFrame_;                     // バトル中に魔法コマンドを選択したときの魔法説明欄
    private TMPro.TextMeshProUGUI magicInfoText_;                 // 魔法の説明テキスト
    private ButtleMng buttleMng_;                                 // ButtleMng.csの取得
    private BadStatusMng badStatusMng_;

    private GameObject setMagicObj_;                    // 魔法コマンド選択時に表示させるオブジェクト
    private int magicCommandNumOld_;                    // 魔法コマンドの1フレーム前の数字  
    private Image[] magicImage_ = new Image[4];         // 魔法画像の貼り付け先(最大4つ表示になる)

    private int enemyNum_ = 0;                                    // バトル時の敵の数
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // 敵のインスタンス位置の全情報

    private Vector3 charaPos_;                         // キャラクター座標
    private Vector3 enePos_;                           // 目標の敵座標

    private CharaUseMagic useMagic_;
    private int mpDecrease_ = 0;                       // 魔法発動時に減少させるMPの量

    private IEnumerator rest_;
    private bool myTurnOnceFlg_;                       // 自分のターンになった最初に1回だけ呼ばれるようにするフラグ

    private readonly int deathBstNoEffectItemNum_ = 17; // 即死肩代わりアイテムの番号

    public struct EachCharaData
    {
        // 各キャラのHP情報
        public (HPMPBar, HPMPBar) charaHPMPMap;
        // 各キャラの選択矢印
        public GameObject charaArrowImage;
        // 各キャラのバッドステータス回復までのターン数
        public Dictionary<CONDITION, int> charaBstTurn;
        // 各キャラのバフ画像
        public GameObject buffIconParent;
        // 各キャラの吸収or反射バフ
        public GameObject specialBuff;
        // 各キャラの行動順番の数字
        public TMPro.TextMeshProUGUI turnNum;
    }

    private EachCharaData[] eachCharaData_ = new EachCharaData[(int)CHARACTERNUM.MAX];

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
        buttleCommandFrame_ = buttleUICanvas.transform.Find("Command/Frame").gameObject;
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();
        buttleItemBackButtonObj_ = buttleUICanvas.transform.Find("ItemBackButton").gameObject;
        buttleItemBackButtonObj_.SetActive(false);  // 最初は非表示
        buttleDamageIconsObj_ = buttleUICanvas.transform.Find("DamageIcons").gameObject;
        buttleDamgeIconPopUpPos_[0] = new Vector3(-260,   0, 0);
        buttleDamgeIconPopUpPos_[1] = new Vector3(-260, 100, 0);
        buttleMagicInfoFrame_ = buttleUICanvas.transform.Find("MagicInfoFrame").gameObject;
        magicInfoText_ = buttleMagicInfoFrame_.transform.Find("MagicInfoText").GetComponent<TMPro.TextMeshProUGUI>();
        buttleMagicInfoFrame_.SetActive(false);     // 最初は非表示

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();

        var buttleMng = GameObject.Find("ButtleMng");
        buttleMng_ = buttleMng.GetComponent<ButtleMng>();
        badStatusMng_ = buttleMng.GetComponent<BadStatusMng>();

        setMagicObj_ = buttleUICanvas.transform.Find("SetMagicObj").gameObject;
        // 魔法画像の表示先を設定する
        for (int i = 0; i < magicImage_.Length; i++)
        {
            magicImage_[i] = setMagicObj_.transform.GetChild(i).GetComponent<Image>();
        }
        magicButtleCommandRotate_ = setMagicObj_.GetComponent<ImageRotate>();
        magicButtleCommandRotate_.SetEnableAndActive(false);

        // ユニとジャックの分で回す
        for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            // キャラ名+CharaDataのステータス表から、HP/MP情報を取得する
            eachCharaData_[i].charaHPMPMap =
                (buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/HPSlider").GetComponent<HPMPBar>(), buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/MPSlider").GetComponent<HPMPBar>());
            // 初期HPを代入
            eachCharaData_[i].charaHPMPMap.Item1.SetHPMPBar(charasList_[i].HP(), charasList_[i].MaxHP());
            // 初期MPを代入
            eachCharaData_[i].charaHPMPMap.Item2.SetHPMPBar(charasList_[i].MP(), charasList_[i].MaxMP());

            // 各キャラについている選択矢印のオブジェクトを取得する
            eachCharaData_[i].charaArrowImage = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/ArrowImage").gameObject;
            eachCharaData_[i].charaArrowImage.SetActive(false);

            // バッドステータス持続管理用
            eachCharaData_[i].charaBstTurn = new Dictionary<CONDITION, int>();

            charaBstIconImage_[i] = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/BadStateImages").gameObject;

            // バフ画像用
            eachCharaData_[i].buffIconParent = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/BuffImages").gameObject;
            eachCharaData_[i].specialBuff = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/SpecialBuff").gameObject;

            // 行動順番の数字
            eachCharaData_[i].turnNum = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/MoveSpeed").GetComponent<TMPro.TextMeshProUGUI>();
        }

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
            magicButtleCommandRotate_.ResetRotate();
        }

        for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            eachCharaData_[i].charaHPMPMap.Item1.SetHPMPBar(charasList_[i].HP(), charasList_[i].MaxHP());
            eachCharaData_[i].charaHPMPMap.Item2.SetHPMPBar(charasList_[i].MP(), charasList_[i].MaxMP());

            // バステやバフのアイコンを消す
            badStatusMng_.SetBstIconImage(i, -1, charaBstIconImage_, charasList_[i].GetBS(), true);
            // 効果が切れた(=ターンが0以下)
            var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild(i);
            if (child.GetComponent<Image>().sprite != null)
            {
                // アイコンをnullにして、上昇矢印も非表示にする
                child.GetComponent<Image>().sprite = null;
                for (int m = 0; m < child.childCount; m++)
                {
                    child.GetChild(m).gameObject.SetActive(false);
                }
            }
        }

        anim_ = ANIMATION.IDLE;
        oldAnim_ = ANIMATION.IDLE;

        buttleAnounceText_.text = announceText_[0];
        magicCommandNumOld_ = -1;

        // 最初の行動キャラを指定する
        //@ キャラ同士で速度を見て、早い人を入れないとだめ
        nowTurnChar_ = CHARACTERNUM.UNI;

        // フラグの初期化を行う
        lastEnemytoAttackFlg_ = false;

        // 戦闘前の座標を保存しておく
        buttleMng_.SetFieldPos(charMap_[CHARACTERNUM.UNI].gameObject.transform.position);

        // 戦闘用座標と回転角度を代入する
        // キャラの角度を変更は、ButtleWarpPointの箱の角度を回転させると可能。(1体1体向きを変えることもできる)
        foreach (KeyValuePair<CHARACTERNUM, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // ここで座標を保存しておくことで、メニュー画面での並び替えでも反映できるだろうし、
            // 攻撃エフェクトの発生位置の目安になる
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
            if (eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1.GetColFlg())
            {
                return;
            }
            else
            {
                // バステとバフの削除
                charasList_[(int)nowTurnChar_].ButtleInit(false);
                // バステやバフのアイコンを消す
                badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                // 効果が切れた(=ターンが0以下)
                var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild((int)nowTurnChar_);
                if (child.GetComponent<Image>().sprite != null)
                {
                    // アイコンをnullにして、上昇矢印も非表示にする
                    child.GetComponent<Image>().sprite = null;
                    for (int m = 0; m < child.childCount; m++)
                    {
                        child.GetChild(m).gameObject.SetActive(false);
                    }
                }

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
                    // 強制戦闘用の壁の名前を保存している所を初期化
                    ButtleMng.forcedButtleWallName = "";
                    EventMng.SetChapterNum(100, SCENE.CONVERSATION);
                }
            }
        }

        // キャラによるアイテムの使用状態を確認
        if (Bag_Item.itemUseFlg)
        {
            Bag_Item.itemUseFlg = false;
            // アイテム画面を閉じる
            GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
            buttleItemBackButtonObj_.SetActive(false);  

            for (int k = 0; k < (int)CHARACTERNUM.MAX; k++)
            {
                // HPMPバーの更新
                eachCharaData_[k].charaHPMPMap.Item1.SetHPMPBar(charasList_[k].HP(), charasList_[k].MaxHP());
                eachCharaData_[k].charaHPMPMap.Item2.SetHPMPBar(charasList_[k].MP(), charasList_[k].MaxMP());

                // バステの更新
                var tmpbs = charasList_[k].GetBS();
                var tmpflg = false;
                // 治した状態異常を探して、状態異常アイコンを外す
                for (int i = 0; i < (int)CONDITION.DEATH; i++)
                {
                    // NONが最初からtrueならbreakで飛ばす
                    if (tmpbs[0].Item2)
                    {
                        break;
                    }

                    if (!tmpbs[i].Item2)
                    {
                        badStatusMng_.SetBstIconImage(k, -1, charaBstIconImage_, charasList_[k].GetBS(), true);
                    }
                    tmpflg |= tmpbs[i].Item2;
                }

                // GetBSで全てfalseになっていたら、アイテムで全ての状態異常が治ったという証拠
                if (!tmpflg)
                {
                    // CONDITIONをNONに戻す
                    charasList_[k].ConditionReset(true);
                    badStatusMng_.SetBstIconImage(k, -1, charaBstIconImage_, charasList_[k].GetBS(), true);
                    Debug.Log("キャラ状態異常が全て治った");
                }
            }

            // 次のキャラor敵に行動が回るようにanim_とoldAnim_を設定する
            anim_ = ANIMATION.IDLE;
            oldAnim_ = ANIMATION.NON;
            Debug.Log("アイテムを使用したので、行動終了");
        }

        // 行動前に発動するバッドステータスの処理
        if (!myTurnOnceFlg_)
        {
            myTurnOnceFlg_ = true;
            var bst = badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

            if (bst == (CONDITION.PARALYSIS, true))
            {
                // 麻痺で動けない
                Debug.Log("麻痺だから行動を飛ばす");
                anim_ = ANIMATION.IDLE;
                oldAnim_ = ANIMATION.NON;
                oldAnim_ = anim_;
                AnimationChange();
                return;
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
            if(FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
            {
                buttleMng_.CallDeleteEnemy();

                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                charMap_[CHARACTERNUM.UNI].gameObject.transform.position = buttleMng_.GetFieldPos();

                Debug.Log("Uniは逃げ出した");
            }
            else
            {
                Debug.Log("強制戦闘だ！逃げられない！");
            }
        }

        // ATTACKで敵選択中に、特定のキー(今はTキー)を押下されたらコマンド選択に戻る
        if (selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            anim_ = ANIMATION.NON;
            selectFlg_ = false;

            buttleCommandRotate_.SetEnableAndActive(true);
            buttleCommandFrame_.SetActive(true);

            buttleAnounceText_.text = announceText_[0];

            if(rest_ != null)
            {
                StopCoroutine(rest_);
                rest_ = null;
            }

            for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                eachCharaData_[i].charaArrowImage.SetActive(false);
            }

            // 魔法コマンドが有効だった時
            if (setMagicObj_.activeSelf || buttleEnemySelect_.gameObject.activeSelf)
            {
                Debug.Log("魔法コマンドの選択を解除します");
                buttleMagicInfoFrame_.SetActive(false);
                magicButtleCommandRotate_.SetEnableAndActive(false);

                // コマンド画像を表示にする
                for (int i = 0; i < buttleCommandImage_.Length; i++)
                {
                    buttleCommandImage_[i].SetActive(true);
                }
                // UIの回転を一番最初に戻す
                magicButtleCommandRotate_.ResetRotate();
            }
        }

        // キャラ毎のモーションを呼ぶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(setMagicObj_.activeSelf)    // 魔法コマンド選択中のとき
            {
                // 魔法コマンド選択中のとき
                buttleAnounceText_.text = announceText_[1];

                var tmp = (int)magicButtleCommandRotate_.GetNowCommand() - 1;
                mpDecrease_ = useMagic_.MPdecrease(charasList_[(int)nowTurnChar_].GetMagicNum(tmp));

                Debug.Log(tmp + "番の魔法を使用しようとしています");

                // 回転中は魔法情報を取得しないようにする。かつ、範囲外かの確認をする
                if (charasList_[(int)nowTurnChar_].CheckMagicNum(tmp) && charasList_[(int)nowTurnChar_].GetMagicNum(tmp).number > 0)
                {
                    // 現在のMP量と、発動に必要なMP量を比較する
                    if(charasList_[(int)nowTurnChar_].MP() < mpDecrease_)
                    {
                        Debug.Log("魔法を発動するためのMPが不足しています");
                        mpDecrease_ = 0;
                    }
                    else
                    {
                        // CharaUseMagic.csに情報を渡す
                        useMagic_.CheckUseMagic(charasList_[(int)nowTurnChar_].GetMagicNum(tmp), charasList_[(int)nowTurnChar_].MagicPower());

                        magicButtleCommandRotate_.SetEnableAndActive(false);
                        buttleCommandRotate_.SetEnableAndActive(false);
                        buttleCommandFrame_.SetActive(false);
                    }
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
                                    buttleCommandFrame_.SetActive(false);
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
                                // 攻撃属性を-1にする(弱点とかが無い状態にする)
                                buttleMng_.SetElement(-1);
                                // バステの付与を無しに戻す
                                buttleMng_.SetBadStatus(-1,-1);
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

                                buttleMagicInfoFrame_.SetActive(true);
                                magicButtleCommandRotate_.SetEnableAndActive(true);
                                buttleCommandRotate_.SetEnableAndActive(false);

                                //@ 行動中のキャラに設定された魔法画像を描画する
                                for (int i = 0; i < 4; i++)
                                {
                                    if (charasList_[(int)nowTurnChar_].GetMagicImage(i) == null)
                                    {
                                        // nullのときは魔法を設定していないから透過する
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                                    }
                                    else
                                    {
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                                        magicImage_[i].sprite = charasList_[(int)nowTurnChar_].GetMagicImage(i);
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

                        // アイテム画面を開く
                        GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(true);
                        buttleItemBackButtonObj_.SetActive(true);
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
        else
        {
            if (setMagicObj_.activeSelf)    // 魔法コマンド選択中のとき
            {
                var tmp = (int)magicButtleCommandRotate_.GetNowCommand() - 1;
                if(tmp >= 0 && magicCommandNumOld_ != tmp)
                {
                    if(charasList_[(int)nowTurnChar_].GetMagicNum(tmp).number > 0)
                    {
                        magicInfoText_.text = useMagic_.MagicInfoMake(charasList_[(int)nowTurnChar_].GetMagicNum(tmp));
                    }
                    else
                    {
                        magicInfoText_.text = "";       // 魔法をセットしていないところにコマンドが回転したときは、内容を未記入にする
                    }
                }
                magicCommandNumOld_ = tmp;
                Debug.Log("魔法選択中-----現在は" + tmp + "番のコマンド上");
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

    // 行動ターン数字を代入する
    public bool SetMoveSpeedNum(int num,string name)
    {
        if(name == "Uni")
        {
            eachCharaData_[(int)CHARACTERNUM.UNI].turnNum.text = num.ToString();
            return true;
        }
        else if(name == "Jack")
        {
            eachCharaData_[(int)CHARACTERNUM.JACK].turnNum.text = num.ToString();
            return true;
        }
        else
        {
            // 何も処理を行わない
        }
        return false;
    }

    void AnimationChange()
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:

                // 毒の処理をいれる
                badStatusMng_.BadStateMoveAfter(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);
                // バステの持続ターン数を-1する
                for (int i = 0; i < (int)CONDITION.DEATH; i++)
                {
                    // キーが存在しなければとばす
                    if (!eachCharaData_[(int)nowTurnChar_].charaBstTurn.ContainsKey((CONDITION)i))
                    {
                        continue;
                    }
                    // -1ターンする
                    eachCharaData_[(int)nowTurnChar_].charaBstTurn[(CONDITION)i]--;
                    // ターン数が0以下になったら、マップから削除する
                    if(eachCharaData_[(int)nowTurnChar_].charaBstTurn[(CONDITION)i] <= 0)
                    {
                        charasList_[(int)nowTurnChar_].ConditionReset(false, i);    // 0以下になったものだけ回復
                        eachCharaData_[(int)nowTurnChar_].charaBstTurn.Remove((CONDITION)i);
                        badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                    }
                }
                // 全ての状態異常が治ったとき
                if(eachCharaData_[(int)nowTurnChar_].charaBstTurn.Count <= 0)
                {
                    // CONDITIONをNONに戻す
                    charasList_[(int)nowTurnChar_].ConditionReset(true);
                    badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                    Debug.Log("キャラ状態異常が全て治った");
                }

                // バフを1ターン減少させる
                if(!charasList_[(int)nowTurnChar_].CheckBuffTurn())
                {
                    // falseの状態(=何かのバフがきれたら)
                    var buffMap = charasList_[(int)nowTurnChar_].GetBuff();
                    for(int i = 0; i < buffMap.Count; i++)
                    {
                        if (buffMap[i + 1].Item2 > 0)   
                        {
                            continue;
                        }

                        // 効果が切れた(=ターンが0以下)
                        var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild(i);
                        if(child.GetComponent<Image>().sprite != null)
                        {
                            // アイコンをnullにして、上昇矢印も非表示にする
                            child.GetComponent<Image>().sprite = null;
                            for (int m = 0; m < child.childCount; m++)
                            {
                                child.GetChild(m).gameObject.SetActive(false);
                            }
                        }
                    }
                }

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
                buttleCommandRotate_.SetEnableAndActive(true);
                buttleCommandFrame_.SetActive(true);

                myTurnOnceFlg_ = false;

                // コマンド画像を表示にする
                for (int i = 0; i < buttleCommandImage_.Length; i++)
                {
                    buttleCommandImage_[i].SetActive(true);
                }

                for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                {
                    eachCharaData_[i].charaArrowImage.SetActive(false);
                }
                break;
            case ANIMATION.BEFORE:
                oldTurnChar_ = nowTurnChar_;

                Debug.Log("前のキャラが行動終了");
                selectFlg_ = true;
                buttleAnounceText_.text = announceText_[1];

                buttleCommandRotate_.gameObject.SetActive(false);
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
                    buttleCommandRotate_.gameObject.SetActive(true);
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
        // 行動前に発動するバッドステータスの処理
        badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

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
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(buttleEnemySelect_.GetSelectNum()[0] + 1,-1);
            }
        }

    }

    void MagicAttack()
    {
        // 行動前に発動するバッドステータスの処理
        badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

        // キャラの位置を取得する
        charaPos_ = charasList_[(int)nowTurnChar_].GetButtlePos();

        // 速度と幸運を渡す
        buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
        buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

        // MP減少処理
        StartCoroutine(eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item2.MoveSlideBar
        (charasList_[(int)nowTurnChar_].MP() - mpDecrease_, charasList_[(int)nowTurnChar_].MP()));
        // 内部数値の変更を行う
        charasList_[(int)nowTurnChar_].SetMP(charasList_[(int)nowTurnChar_].MP() - mpDecrease_);

        // 魔法での攻撃対象を決定したときに入る
        if (useMagic_.GetElementAndSub1Num().Item1 >= 2 ||  // 属性魔法攻撃もしくは、敵へのデバフ(補助魔法)
            useMagic_.GetElementAndSub1Num().Item1 == 1 && useMagic_.GetElementAndSub1Num().Item2 == 1) 
        {
            // 選択した敵の番号を渡す
            var tmp = buttleEnemySelect_.GetSelectNum();
            //int[] tmp = { 0, 0, 0, 0 }; // デバッグ用(好きな数値で攻撃対象を決められる)
            for (int i = 0; i < tmp.Length; i++)
            {
                // tmp内容が-1以外なら攻撃対象がいるので処理を行う
                if (tmp[i] >= 0)
                {
                    // 敵の位置を取得する
                    enePos_ = buttleEnemySelect_.GetSelectEnemyPos(tmp[i]);
                    enePos_.y = 0.0f;        // ここで0.0fにしないと斜め上方向に飛んでしまう
                                             // 情報を設定する
                    useMagic_.InstanceMagicInfo(charaPos_, enePos_, tmp[i], i);
                }
            }
            StartCoroutine(useMagic_.InstanceMagicCoroutine());
        }

        buttleCommandRotate_.gameObject.SetActive(true);
        buttleEnemySelect_.SetActive(false);
        mpDecrease_ = 0;

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

    public void SetCharaFieldPos()
    {
        charMap_[CHARACTERNUM.UNI].gameObject.transform.position = buttleMng_.GetFieldPos();
    }

    public void HPdecrease(int num,int fromNum)
    {
        int hitProbabilityOffset = 0;   // 命中率
        var damage = 0;                 // ダメージ値の算出
        bool criticallFlg = false;      // クリティカル判断用のフラグ

        // クリティカルの計算をする(基礎値と幸運値で上限を狭める)
        int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
        if(criticalRand <= 10 + buttleMng_.GetLuckNum())
        {
            // クリティカル発生(必中+ダメージ2倍)10はクリティカルの基礎値
            Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "なので、敵の攻撃がクリティカル！");
            // クリティカルダメージ
            damage = (buttleMng_.GetDamageNum() * 2) - charasList_[num].Defence(true);

            hitProbabilityOffset = 200; // 100以上の数字が必要になる(バステ付与時に幸運値+ランダムが100を越える可能性があるから)
            criticallFlg = true;
        }
        else
        {
            // クリティカルじゃないとき
            Debug.Log(criticalRand + ">" + (10 + charasList_[num].Luck()) + "なので、敵の攻撃はクリティカルではない");

            // 命中計算をする
            // ①攻撃する側のSpeed / 攻撃される側のSpeed * 100 = ％の出力
            var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)charasList_[num].Speed() * 100.0f);
            // ②キャラも敵も+10％の補正値を入れる。
            hitProbabilityOffset = hitProbability + 10;
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
                    DamageIcon(num, "ミス",false,false);
                    return;
                }
            }
            else
            {
                Debug.Log("命中率" + hitProbabilityOffset + "が100以上なので、自動命中");
            }

            int tmpLuck;

            // 命中時にはLuckで回避判定をする
            // 判定の範囲は、100 - 現在のLuckを最大値にして、より回避成功に近づける
            if (charasList_[num].Luck() <= 10)
            {
                tmpLuck = 10;
                Debug.Log("Luckが10以下なので、10を適用して回避判定をします");
            }
            else
            {
                tmpLuck = charasList_[num].Luck();
                Debug.Log("Luckが10以上なので、現在のステータスのLuckを使って回避判定をします");
            }

            int randLuck = Random.Range(0, 100 - tmpLuck);
            if (randLuck <= tmpLuck)
            {
                Debug.Log(randLuck + "<=" + tmpLuck + "以下なので、回避成功");
                DamageIcon(num, "ミス", false, false);
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

        // バステやダメージ処理よりも先に、反射か吸収のバフを張っているか確認する
        var spbuff = charasList_[num].GetReflectionOrAbsorption();
        if(spbuff == Chara.SPECIALBUFF.REF || spbuff == Chara.SPECIALBUFF.REF_M)         // 反射処理
        {
            bool tmpFlg = false;
            if(!buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.REF)
            {
                // 物理攻撃に物理反射
                tmpFlg = true;
            }
            else if(buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.REF_M)
            {
                // 魔法攻撃に魔法反射
                tmpFlg = true;
            }
            else
            {
                Debug.Log("敵の攻撃と反射の条件が違うので処理なし");
            }

            if(tmpFlg)
            {
                // 攻撃値分相手のHPを減らす
                buttleMng_.SetRefEnemyNum(fromNum);
                charasList_[num].SetReflectionOrAbsorption(0, 1);  // NONに戻す
                Debug.Log("攻撃反射");

                eachCharaData_[num].specialBuff.GetComponent<Image>().sprite = null;
                eachCharaData_[num].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                return;
            }
        }
        else if(spbuff == Chara.SPECIALBUFF.ABS || spbuff == Chara.SPECIALBUFF.ABS_M)    // 吸収処理
        {
            bool tmpFlg = false;
            if (!buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.ABS)
            {
                // 物理攻撃に物理吸収
                tmpFlg = true;
            }
            else if (buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.ABS_M)
            {
                // 魔法攻撃に魔法吸収
                tmpFlg = true;
            }
            else
            {
                Debug.Log("敵の攻撃と吸収の条件が違うので処理なし");
            }

            if (tmpFlg)
            {
                // 攻撃値の半分をHP回復する(防御力から引いた値にならないようにGet関数から直接値をとってくる)
                charasList_[num].SetHP(charasList_[num].HP() + (buttleMng_.GetDamageNum() / 2));
                charasList_[num].SetReflectionOrAbsorption(0, 1);  // NONに戻す
                Debug.Log("攻撃吸収");

                eachCharaData_[num].specialBuff.GetComponent<Image>().sprite = null;
                eachCharaData_[num].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                return;
            }
        }
        else
        {
            // 何も処理を行わない
        }

        DamageIcon(num,damage.ToString(), false,criticallFlg);

        // バッドステータスが付与されるか判定
        charasList_[num].SetBS(buttleMng_.GetBadStatus(), hitProbabilityOffset);

        var getBs = charasList_[num].GetBS();
        // バステの効果持続ターンを設定する
        for(int i = 0; i < (int)CONDITION.DEATH; i++)
        {
            // 健康状態以外のコンディションのフラグがtrueになっていたら
            if(getBs[i].Item2 && getBs[i].Item1 != CONDITION.NON)
            {
                if (!eachCharaData_[num].charaBstTurn.ContainsKey(getBs[i].Item1))
                {
                    eachCharaData_[num].charaBstTurn.Add(getBs[i].Item1, Random.Range(1, 5));// 1以上5未満
                    badStatusMng_.SetBstIconImage(num, (int)getBs[i].Item1, charaBstIconImage_, charasList_[num].GetBS());
                    break;
                }
            }
        }

        // 即死用に処理呼び出し
        var bst = badStatusMng_.BadStateMoveBefore(getBs, charasList_[num], eachCharaData_[num].charaHPMPMap.Item1, true);
        if (bst == (CONDITION.DEATH, true))   // 即死処理
        {
            // 身代わりアイテムを持っているか調べる
            if(Bag_Item.itemState[deathBstNoEffectItemNum_].haveCnt > 0)
            {
                Bag_Item.itemState[deathBstNoEffectItemNum_].haveCnt--;
                Debug.Log("アイテムが即死を肩代わりしてくれた");
                return;
            }

            StartCoroutine(eachCharaData_[num].charaHPMPMap.Item1.MoveSlideBar(charasList_[num].HP() - 999, charasList_[num].HP()));
            charasList_[num].SetHP(charasList_[num].HP() - 999);
        }
        else
        {
            // キャラのHPを削る(スライドバー変更)
            StartCoroutine(eachCharaData_[num].charaHPMPMap.Item1.MoveSlideBar(charasList_[num].HP() - damage, charasList_[num].HP()));
            // 内部数値の変更を行う
            charasList_[num].SetHP(charasList_[num].HP() - damage);
        }

        if (charasList_[num].HP() <= 0)
        {
            Debug.Log("キャラが死亡");
            charasList_[num].SetHP(0);
            // Chara.csに死亡情報を入れる
            charasList_[num].SetDeathFlg(true);
        }
    }

    //@ 不足処理あり(死亡時の回復をnoeffectにしたほうがいい)
    public void SetCharaArrowActive(bool allflag, bool randFlg,int whatHeal,int sub3Num)
    {
        // 誰か1人をtrueにする場合は、上から順なので0番の人をtrueにする
        if(!allflag)
        {
            eachCharaData_[0].charaArrowImage.SetActive(true);
        }
        else
        {
            // 全員表示状態にするならfor文で回す
            for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                eachCharaData_[i].charaArrowImage.SetActive(true);
            }
        }

        // 初期数値を-1にしておく
        int[] tmpArray = { -1, -1, -1, -1 };

        // 複数か全体で処理を分ける
        if (randFlg)
        {
            // 回復回数をランダムにする(2～4回とする)
            int randHealNum = Random.Range(2, 5); // 2以上5未満の値がでる
            Debug.Log("複数回HP回復の回数は" + randHealNum + "回に決定しました");

            // 死亡中のキャラも含めてランダムで決定する
            for (int i = 0; i < randHealNum; i++)
            {
                tmpArray[i] = Random.Range(0, (int)CHARACTERNUM.MAX);// 0以上MAX未満の値がでる
            }
        }
        else
        {
            // 全体
            // 死亡中のキャラも含めて全体回復とする
            for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                tmpArray[i] = i;
            }
        }

        if(useMagic_.GetElementAndSub1Num().Item1 == 0) 
        {
            // 回復処理へ
            rest_ = SelectToHealMagicChara(allflag, tmpArray, whatHeal);
        }
        else
        {
            // 補助処理へ(バフ)
            rest_ = SelectToBuffMagicChara(allflag, tmpArray, whatHeal,sub3Num);
        }

        StartCoroutine(rest_);
    }

    private IEnumerator SelectToHealMagicChara(bool allflag, int[] array,int whatHeal)
    {
        bool flag = false;
        var selectChara = CHARACTERNUM.UNI;

        while (!flag)
        {
            yield return null;

            if (!allflag)    // 単体回復魔法の発動時
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    // ユニより数値が小さくならないようにする
                    if (--selectChara < CHARACTERNUM.UNI)
                    {
                        selectChara = CHARACTERNUM.UNI;
                    }
                    // 1度全てfalseにする
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // 該当するキャラの矢印だけtrueにする
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    // ジャックより数値が大きくならないようにする
                    if (++selectChara > CHARACTERNUM.JACK)
                    {
                        selectChara = CHARACTERNUM.JACK;
                    }
                    // 1度全てfalseにする
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // 該当するキャラの矢印だけtrueにする
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else
                {
                    // 何も処理を行わない
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!allflag)    // 単体回復
                {
                    // enePosとtargetNumは入れなくてよい(charaPos_に発動相手の座標を入れるようにする)
                    useMagic_.InstanceMagicInfo(charasList_[(int)selectChara].GetButtlePos(), new Vector3(-1, -1, -1), -1, 0);

                    if(whatHeal == 0)   // HP回復
                    {
                        // キャラのHPを増やす(スライドバー変更)
                        StartCoroutine(eachCharaData_[(int)selectChara].charaHPMPMap.Item1.MoveSlideBar(charasList_[(int)selectChara].HP() + useMagic_.GetHealPower(), charasList_[(int)selectChara].HP()));
                        // 内部数値の変更を行う
                        charasList_[(int)selectChara].SetHP(charasList_[(int)selectChara].HP() + useMagic_.GetHealPower());
                    }
                    else
                    {
                        // 毒・暗闇・麻痺回復
                        charasList_[(int)selectChara].ConditionReset(false,whatHeal);
                        badStatusMng_.SetBstIconImage((int)selectChara, -1, charaBstIconImage_, charasList_[(int)selectChara].GetBS(), true);
                    }
                }
                else
                {
                    // 複数回or全体回復
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= -1)
                        {
                            break;
                        }

                        // enePosとtargetNumは入れなくてよい(charaPos_に発動相手の座標を入れるようにする)
                        useMagic_.InstanceMagicInfo(charasList_[array[i]].GetButtlePos(), new Vector3(-1, -1, -1), -1, i);

                        if (whatHeal == 0)
                        {
                            // キャラのHPを増やす(スライドバー変更)
                            StartCoroutine(eachCharaData_[array[i]].charaHPMPMap.Item1.MoveSlideBar(charasList_[array[i]].HP() + useMagic_.GetHealPower(), charasList_[array[i]].HP()));
                            // 内部数値の変更を行う
                            charasList_[array[i]].SetHP(charasList_[array[i]].HP() + useMagic_.GetHealPower());
                            Debug.Log((CHARACTERNUM)array[i] + "のHPを、" + useMagic_.GetHealPower() + "回復しました");
                        }
                        else
                        {
                            // 毒・暗闇・麻痺回復
                            charasList_[array[i]].ConditionReset(false, whatHeal);
                            badStatusMng_.SetBstIconImage(array[i], -1, charaBstIconImage_, charasList_[array[i]].GetBS(), true);
                            Debug.Log((CHARACTERNUM)array[i] + "の状態異常を回復しました");
                        }
                    }
                }

                StartCoroutine(useMagic_.InstanceMagicCoroutine());

                flag = true;
            }
        }
    }

    private IEnumerator SelectToBuffMagicChara(bool allflag, int[] array, int whatBuff,int sub3Num)
    {
        bool flag = false;
        var selectChara = CHARACTERNUM.UNI;

        // バフのアイコン処理
        System.Action<int,int> action = (int charaNum,int buffnum) => {
            var bufftra = eachCharaData_[charaNum].buffIconParent.transform;
            for (int i = 0; i < bufftra.childCount; i++)
            {
                if (bufftra.GetChild(i).GetComponent<Image>().sprite == null)
                {
                    // アイコンをいれる
                    bufftra.GetChild(i).GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff - 1];
                    // 矢印でアップ倍率をいれる
                    // ▲*1 = バフが1% ~30%,▲*2 = バフが31% ~70%,▲*3 = バフが71%~100%
                    for (int m = 0; m < bufftra.GetChild(i).childCount; m++)
                    {
                        if (m <= buffnum)    // buffnumの数字以下ならtrueにして良い
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(true);
                        }
                        else
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(false);
                        }
                    }

                    break;
                }
            }
        };

        while (!flag)
        {
            yield return null;

            if (!allflag)    // 単体回復魔法の発動時
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    // ユニより数値が小さくならないようにする
                    if (--selectChara < CHARACTERNUM.UNI)
                    {
                        selectChara = CHARACTERNUM.UNI;
                    }
                    // 1度全てfalseにする
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // 該当するキャラの矢印だけtrueにする
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    // ジャックより数値が大きくならないようにする
                    if (++selectChara > CHARACTERNUM.JACK)
                    {
                        selectChara = CHARACTERNUM.JACK;
                    }
                    // 1度全てfalseにする
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // 該当するキャラの矢印だけtrueにする
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else
                {
                    // 何も処理を行わない
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!allflag)    // 単体バフ
                {
                    // enePosとtargetNumは入れなくてよい(charaPos_に発動相手の座標を入れるようにする)
                    useMagic_.InstanceMagicInfo(charasList_[(int)selectChara].GetButtlePos(), new Vector3(-1, -1, -1), -1, 0);

                    if(sub3Num == 0)
                    {
                        // 上昇
                        var buffnum = charasList_[(int)selectChara].SetBuff(useMagic_.GetTailNum(), whatBuff);
                        if(!buffnum.Item2)
                        {
                            action((int)selectChara, buffnum.Item1);
                        }
                    }
                    else
                    {
                        // 反射か吸収
                        charasList_[(int)selectChara].SetReflectionOrAbsorption(whatBuff-1, sub3Num);
                        eachCharaData_[(int)selectChara].specialBuff.GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff + 3];
                        if(sub3Num == 2)
                        {
                            eachCharaData_[(int)selectChara].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "反";
                        }
                        else
                        {
                            eachCharaData_[(int)selectChara].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "吸";
                        }
                    }
                }
                else
                {
                    // 複数回or全体回復
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= -1)
                        {
                            break;
                        }

                        // enePosとtargetNumは入れなくてよい(charaPos_に発動相手の座標を入れるようにする)
                        useMagic_.InstanceMagicInfo(charasList_[array[i]].GetButtlePos(), new Vector3(-1, -1, -1), -1, i);

                        if (sub3Num == 0)
                        {
                            // 上昇
                            var buffnum = charasList_[array[i]].SetBuff(useMagic_.GetTailNum(), whatBuff);
                            if(!buffnum.Item2)
                            {
                                action(array[i], buffnum.Item1);
                            }
                        }
                        else
                        {
                            // 反射か吸収
                            charasList_[array[i]].SetReflectionOrAbsorption(whatBuff-1, sub3Num);
                            eachCharaData_[array[i]].specialBuff.GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff + 3];
                            if (sub3Num == 2)
                            {
                                eachCharaData_[array[i]].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "反";
                            }
                            else
                            {
                                eachCharaData_[array[i]].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "吸";
                            }
                        }
                    }
                }
                StartCoroutine(useMagic_.InstanceMagicCoroutine());
                flag = true;
            }
        }
    }

    // ダメージアイコンの表示処理
    private void DamageIcon(int num,string str,bool weakFlg,bool criticallFlg)
    {
        // ダメージアイコン
        for (int i = 0; i < buttleDamageIconsObj_.transform.childCount; i++)
        {
            var tmp = buttleDamageIconsObj_.transform.GetChild(i).gameObject;
            if (tmp.activeSelf)
            {
                continue;
            }

            // まだ非表示状態の使われていないアイコンを見つけたとき
            // 座標を被ダメージキャラの頭上にする
            buttleDamageIconsObj_.transform.GetChild(i).transform.localPosition = buttleDamgeIconPopUpPos_[num];
            // ダメージ数値を入れる
            tmp.transform.GetChild(0).GetComponent<Text>().text = str;
            // 表示状態にする
            tmp.SetActive(true);

            tmp.transform.GetChild(0).Find("Weak").gameObject.SetActive(weakFlg);
            tmp.transform.GetChild(0).Find("Critical").gameObject.SetActive(criticallFlg);

            break;
        }
    }
}
