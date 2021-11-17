using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCreate : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas;

    [SerializeField]
    private RectTransform miniGameMng;    // ミニゲーム表示用

    // ワードを表示するための親の位置
    private RectTransform magicCreateParent;

    //ワードの最大個数取得用
    private int[] selectKindMaxCnt_ = new int[(int)InitPopList.WORD.INFO];

    // 選択中のワード種類
    private int kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
    // 画面下部、説明関連
    private string[] topicString_ = new string[(int)Bag_Word.WORD_MNG.MAX] {
    "Head","Element","Tail","Sub1","Sub2","Sub3"};
    private Text topicText_;    // どのワードの種類を表示しているか

    // どれを選択しているかを表示
    private Text infoText_;
    private string[] selectWord_;
    private int targetWordNum_;// 必中ワードの番号を保存
    private int materiaNum_;//空のマテリアの番号を保存
    private Image materiaImage_;// 空のマテリア画像描画
    private Text materiaCntText_;// 空のマテリアの所持数を描画

    //   0.作成開始ボタン　1.魔法合成終了ボタン
    private bool createFlag_ = false;
    private Button createBtn_;
    private Button cancelBtn_;
    private Bag_Magic bagMagic_;
    private string allName_ = "";
    private int[] saveNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];// 選択したワードの番号を保存
    private int[] oldNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];
    private int saveElementKind_ = 0;

    // 矢印ボタン
    private Button[] arrowBtn_ = new Button[2];

    //ミニゲームスタート用
    private MovePoint movePoint_;
    private Image judgeBack_;
    private Text judgeText_;

    public struct MagicCreateData
    {
        public GameObject pleate;   // インスタンスしたオブジェクトを保存
        public string name;         // ワード名
        public Button btn;
        public bool getFlag;
    }
    public static Dictionary<Bag_Word.WORD_MNG, MagicCreateData[]> mCreateData = new Dictionary<Bag_Word.WORD_MNG, MagicCreateData[]>();
    private int[] mngMaxCnt = new int[(int)Bag_Word.WORD_MNG.MAX];

    private MagicCreateData[] InitCheck(Bag_Word.WORD_MNG kind)
    {
        int maxCnt = 0;
        int startNum = 0;
        int maxNum = 0;
        switch (kind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.HEAD];
                maxNum = (int)InitPopList.WORD.HEAD;
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_ASSIST] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_ATTACK] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_HEAL];
                startNum = (int)InitPopList.WORD.ELEMENT_HEAL;
                maxNum = (int)InitPopList.WORD.TAIL;
                break;

            case Bag_Word.WORD_MNG.TAIL:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.TAIL];
                maxNum = (int)InitPopList.WORD.TAIL;
                break;

            case Bag_Word.WORD_MNG.SUB1:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB1] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.SUB1_AND_SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB1;
                break;

            case Bag_Word.WORD_MNG.SUB2:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.SUB1_AND_SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB2;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB3] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB3;
                maxNum = (int)InitPopList.WORD.INFO;
                break;

            default:
                break;
        }
        mngMaxCnt[(int)kind] = maxCnt;
        //  Debug.Log(kind + "            "+maxCntCheck_[(int)kind]);
        var state = new MagicCreateData[maxCnt];
        int count = 0;
        if (kind == Bag_Word.WORD_MNG.SUB1
            || kind == Bag_Word.WORD_MNG.SUB2)
        {
            // Sub1かSub2の時
            for (int k = startNum; k < (int)InitPopList.WORD.MAX; k++)
            {
                if (k == startNum
                || k == (int)InitPopList.WORD.SUB1_AND_SUB2
                || k == (int)InitPopList.WORD.ALL_SUB)
                {
                    for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                    {
                        CommonInitCheck(state, count, (InitPopList.WORD)k, i);
                       // Debug.Log(state[count].name);
                        count++;
                    }
                }
            }
        }
        else
        {
            if (startNum == 0)
            {
                // HEAD,TAIL用
                for (int i = 0; i < maxCnt; i++)
                {
                    CommonInitCheck(state, count, (InitPopList.WORD)maxNum, i);
                    count++;
                }
            }
            else
            {
                for (int k = startNum; k < maxNum; k++)
                {
                    // ELEMENT,SUB3用
                    for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                    {
                        CommonInitCheck(state, count, (InitPopList.WORD)k, i);
                        count++;
                    }
                }
            }
        }
        return state;
    }

    private void CommonInitCheck(MagicCreateData[] data, int count, InitPopList.WORD kind, int dataNum)
    {
        data[count].name = Bag_Word.wordState[kind][dataNum].name;
        data[count].pleate = Bag_Word.wordState[kind][dataNum].pleate;
        data[count].btn = Bag_Word.wordState[kind][dataNum].btn;
        data[count].getFlag = Bag_Word.wordState[kind][dataNum].getFlag;
    }

    public void Init()
    {
        magicCreateParent = transform.Find("ScrollView/Viewport/WordParent").GetComponent<RectTransform>();
        // ワードの最大個数を取得
        for (int i = 0; i < (int)InitPopList.WORD.INFO; i++)
        {
            selectKindMaxCnt_[i] = InitPopList.maxWordCnt[i];
        }

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            mCreateData[(Bag_Word.WORD_MNG)i] = InitCheck((Bag_Word.WORD_MNG)i);
        }

        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();

        // 作成開始ボタン
        createBtn_ = transform.Find("InfoMng/CreateBtn").GetComponent<Button>();
        createBtn_.interactable = false;

        // 魔法合成終了ボタンを代入
        cancelBtn_ = transform.Find("InfoMng/CancelBtn").GetComponent<Button>();

        // ミニゲーム処理
        movePoint_ = miniGameMng.transform.GetComponent<MovePoint>();
        judgeBack_ = movePoint_.transform.Find("JudgeBack").GetComponent<Image>();
        judgeText_ = judgeBack_.transform.Find("Text").GetComponent<Text>();
        judgeBack_.gameObject.SetActive(false);
        miniGameMng.gameObject.SetActive(false);
        judgeText_.text = "";

        // ScrollView/TopicBtnまでの階層
        RectTransform viewParent_ = transform.Find("ScrollView/TopicBtn").GetComponent<RectTransform>();
        RectTransform infoParent_ = transform.Find("InfoMng/InfoBack").GetComponent<RectTransform>();

        // 右左の矢印
        arrowBtn_[0] = viewParent_.transform.Find("LeftBtn").GetComponent<Button>();
        arrowBtn_[1] = viewParent_.transform.Find("RightBtn").GetComponent<Button>();

        // どれを種類のワードかを表示する
        topicText_ = viewParent_.transform.Find("TopicText").GetComponent<Text>();
        topicText_.text = topicString_[(int)Bag_Word.WORD_MNG.HEAD];

        // 選んだワードを表示
        infoText_ = infoParent_.transform.Find("SelectWord").GetComponent<Text>();
        selectWord_ = new string[(int)Bag_Word.WORD_MNG.MAX];
        targetWordNum_ = Bag_Word.targetWordNum;
        materiaNum_ = Bag_Materia.emptyMateriaNum;

        materiaImage_ = infoParent_.transform.Find("MateriaArea/MateriaImage").GetComponent<Image>();
        materiaCntText_ = infoParent_.transform.Find("MateriaArea/CountBack/Count").GetComponent<Text>();
        if (0 < Bag_Materia.materiaState[materiaNum_].haveCnt)
        {
            // 空のマテリアを1つでも持っていれば表示
            materiaImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum_];
            materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();
        }
        ResetCommon();

        // ワードたちを魔法合成用の親の子に移動させる
        if (Bag_Word.wordState[InitPopList.WORD.HEAD][0].pleate.transform.parent != magicCreateParent.transform)
        {
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                {
                    Bag_Word.wordState[(InitPopList.WORD)k][i].pleate.transform.SetParent(magicCreateParent.transform);
                }
            }
           // Debug.Log("親の位置をずらしました。");
        }
    }

    public void OnClickRightArrow()
    {
        // 値を加算
        kindNum_++;
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "回復")
        {
            if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
            {
                kindNum_ = (int)Bag_Word.WORD_MNG.SUB2;
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "補助")
        {
            // 攻撃系Elementを選択時に必中をもっていなかったらSub2でストップ
            if (Bag_Word.wordState[InitPopList.WORD.SUB3][targetWordNum_].getFlag == false)
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
                {
                    kindNum_ = (int)Bag_Word.WORD_MNG.SUB2;
                }
            }
        }
        else
        {
            // 何もしない
        }
        // Debug.Log(selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
        ActiveKindsCheck((Bag_Word.WORD_MNG)kindNum_, false);
        //  Debug.Log("右矢印をクリック" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        // ワード種別を1つ戻るときにその種類のワードを選択していれば
        // ボタンを押下できるようにして色を元に戻す
        if (saveNumber_[kindNum_] != -1)
        {
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.white;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = true;
        }
        selectWord_[kindNum_] = null;
        oldNumber_[kindNum_] = -1;

        kindNum_--;

        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        ActiveKindsCheck((Bag_Word.WORD_MNG)kindNum_, true);
        Debug.Log("左矢印をクリック" + kindNum_);
    }

    public void ActiveKindsCheck(Bag_Word.WORD_MNG kind, bool leftFlag)
    {
        topicText_.text = topicString_[(int)kind];

        if (leftFlag == true)
        {
            // 左矢印が動ける最大位置
            if (kindNum_ <= (int)Bag_Word.WORD_MNG.HEAD)
            {
                kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
                // Headの時は押下できないようにする
                arrowBtn_[0].interactable = false;
            }
            arrowBtn_[1].interactable = true;
        }
        else
        {
            // 右矢印が動ける最大位置
            if ((int)Bag_Word.WORD_MNG.SUB3 <= kindNum_)
            {
                kindNum_ = (int)Bag_Word.WORD_MNG.SUB3;
            }
            arrowBtn_[0].interactable = true;
            arrowBtn_[1].interactable = false;// 右矢印を押したら必ずfalse
        }
        // ワードの種類と一致している番号だけ表示
        SelectWordKindCheck((Bag_Word.WORD_MNG)kindNum_);

        // すでに選択状態のボタンは押下できないようにする
        if (selectWord_[kindNum_] != null)
        {
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.green;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = false;
        }
    }

    private void SelectWordKindCheck(Bag_Word.WORD_MNG wordKind)
    {
        for (int k = 0; k < (int)Bag_Word.WORD_MNG.MAX; k++)
        {
            for (int i = 0; i < mngMaxCnt[k]; i++)
            {
                // すべて非表示にしておく
                mCreateData[(Bag_Word.WORD_MNG)k][i].pleate.SetActive(false);
            }
        }

        switch (wordKind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.HEAD]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue;// 持っていないワードは表示しない
                    }
                    mCreateData[Bag_Word.WORD_MNG.HEAD][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.HEAD][i].pleate.SetActive(true);
                }
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.ELEMENT]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue; // 持っていないワードは表示しない
                    }
                    mCreateData[Bag_Word.WORD_MNG.ELEMENT][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.ELEMENT][i].pleate.SetActive(true);
                }
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.TAIL:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.TAIL]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue; // 持っていないワードは表示しない
                    }
                    mCreateData[Bag_Word.WORD_MNG.TAIL][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.TAIL][i].pleate.SetActive(true);
                }
                createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null ? true : false;
                break;

            case Bag_Word.WORD_MNG.SUB1:
                ElementCheck(Bag_Word.WORD_MNG.SUB1, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.SUB2:
                ElementCheck(Bag_Word.WORD_MNG.SUB2, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                ElementCheck(Bag_Word.WORD_MNG.SUB3, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            default:
                break;
        }
        createBtn_.interactable = createFlag_;
    }

    public void ElementCheck(Bag_Word.WORD_MNG kind, string selectWord)
    {
        // エレメントの属性チェック
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "回復")
        {
            SetElementHeal(kind);
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "補助")
        {
            SetElementAssist(kind);
        }
        else
        {
            SetElementAttack(kind);
        }
    }

    private void SetElementHeal(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i++)
                {
                    CommonButtonCheck(true, "味方", Bag_Word.WORD_MNG.SUB1, i);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB2]; i++)
                {
                    if (mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "HP"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "麻痺"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "暗闇"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "毒")
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = true;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SetElementAssist(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i++)
                {
                    // 敵と味方のワードだけ押下可能
                    mCreateData[Bag_Word.WORD_MNG.SUB1][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.SUB1][i].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB2]; i++)
                {
                    CommonButtonCheck(false, "HP", Bag_Word.WORD_MNG.SUB2, i);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == "敵")
                    {
                        CommonButtonCheck(true, "低下", Bag_Word.WORD_MNG.SUB3, i);
                    }
                    else
                    {
                        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == "防御力")
                        {
                            CommonButtonCheck(true, "上昇", Bag_Word.WORD_MNG.SUB3, i);
                        }
                        else
                        {
                            CommonButtonCheck(false, "低下", Bag_Word.WORD_MNG.SUB3, i);
                            if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].name == "必中")
                            {
                                mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = false;
                                mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(false);
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SetElementAttack(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB1]; i++)
                {
                    CommonButtonCheck(false, "必中", Bag_Word.WORD_MNG.SUB1, i);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = selectKindMaxCnt_[(int)InitPopList.WORD.SUB2]; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB2]; i++)
                {
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] != mCreateData[Bag_Word.WORD_MNG.SUB2][i].name)
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = true;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                    }
                    else
                    {
                        // Sub1で選択したワードは押下できないように
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = false;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.image.color = Color.green;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].name == "必中")
                    {
                        // 必中を持っていたら
                        if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].getFlag == true)
                        {
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = true;
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(true);
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    private void CommonButtonCheck(bool flag, string name, Bag_Word.WORD_MNG kinds, int num)
    {
        // trueなら＝＝　falseなら！＝
        if (flag == true)
        {
            mCreateData[kinds][num].btn.interactable = mCreateData[kinds][num].name == name ? true : false;
            mCreateData[kinds][num].pleate.SetActive(mCreateData[kinds][num].btn.interactable);
            // mCreateState[kinds][num].nameがnameならinteractableにtrueをいれる
        }
        else
        {
            mCreateData[kinds][num].btn.interactable = mCreateData[kinds][num].name != name ? true : false;
            mCreateData[kinds][num].pleate.SetActive(mCreateData[kinds][num].btn.interactable);
            // mCreateState[kinds][num].nameがnameではないボタンのinteractableにtrueをいれる
        }
    }

    public void SetWord(string word)
    {
        // ーーーーー●ワード選択処理
        // ワードのボタンを押下したら呼び出す
        for (int i = 0; i < mngMaxCnt[kindNum_]; i++)
        {
            // どの種類の時にどのワードが押下されたか
            if (word == mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].name)
            {
                // 押下されたワードの番号を代入する
                saveNumber_[kindNum_] = i;
                break;
            }
        }
        //Debug.Log("選択したワード" + mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].name);

        // Elementがどの属性なのかをチェック
        if (kindNum_ == (int)Bag_Word.WORD_MNG.ELEMENT)
        {
            for (int i = 0; i < (int)Bag_Magic.ELEMENT_KIND.MAX; i++)
            {
                // Element選択中しか入らないからwordと比べる
                if (Bag_Magic.elementString[i] == word)
                {
                    saveElementKind_ = i;
                }
            }
        }

        // -1以外は同じワード種別内で複数のボタンが押されたとき
        if (oldNumber_[kindNum_] != -1)
        {
            // 1つ前に押されたボタンは初期状態に戻す
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][oldNumber_[kindNum_]].btn.image.color = Color.white;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][oldNumber_[kindNum_]].btn.interactable = true;
        }

        // 選択したワードの色を緑にして選択できないようにする
        mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.green;
        mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = false;

        //// トピックからどの種類の時に選ばれたワードなのかを見る
        selectWord_[kindNum_] = mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].name;
        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        // 改行なしで作られた名前を保存しておく
        allName_ = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                 selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB3];
        // ーーーーー●ワード選択処理ここまで

        // ーーーーー〇ボタン系のinteractableチェック

        // 突然の三項演算子がミネを襲う！！！！！！！！
        // Sub3のワードを選択したら右矢印を非アクティブにする
        arrowBtn_[1].interactable = (int)Bag_Word.WORD_MNG.SUB3 == kindNum_ ? false : true;
        // kindNum_が(int)Bag_Word.WORD_MNG.SUB3ならarrowBtn_[1].interactableにfalseをいれる
        // 違うならarrowBtn_[1].interactableにtrueをいれる

        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "回復")
        {
            // Elementで「回復」ワードを選択していたら
            if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
            {
                // Sub3ワードを選択できないようにする
                arrowBtn_[1].interactable = false;

                // Elementで回復選択時はSub2を選択することで作成ができる
                createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.SUB2] != null ? true : false;
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "補助")
        {
            // 攻撃系のSubはどのタイミングで選んでも作成ができる
            if ((int)Bag_Word.WORD_MNG.TAIL <= kindNum_)
            {
                createFlag_ = selectWord_[kindNum_] != null ? true : false;
            }
            // 攻撃系Element選択時に「必中」を持っていない時
            // Sab2の時点で「必中」を選択時
            if (Bag_Word.wordState[InitPopList.WORD.SUB3][targetWordNum_].getFlag == false
             || selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == "必中")
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
                {
                    arrowBtn_[1].interactable = false;// 右矢印を押したら必ずfalse
                }
            }
        }
        else
        {
            createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.SUB3] != null ? true : false;
        }

        if (kindNum_ == (int)Bag_Word.WORD_MNG.TAIL)
        {
            // Tail選択時＝Head、Element、Tailがnullではない
            if (selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null)
            {
                createFlag_ = true;
            }
        }


        createBtn_.interactable = createFlag_;
        oldNumber_[kindNum_] = saveNumber_[kindNum_];
    }

    public void OnClickMagicCreate()
    {
        // 空のマテリアが1つ以上持ってるかチェック
        if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
        {
            return;
        }

        for (int i = 0; i < bagMagic_.MagicNumber(); i++)
        {
            if (allName_ == Bag_Magic.data[0].name)
            {
                Debug.Log(allName_ + "はすでに生成されています");
                return;
            }
        }

        // 空のマテリアの所持数を減らす
        Bag_Materia.materiaState[materiaNum_].haveCnt--;
        materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();

        StartCoroutine(movePoint_.CountDown());
        StartCoroutine(ResultMagicCreate());
        miniGameMng.gameObject.SetActive(true);

        // ゲームが始まるため押下できないようにする
        createBtn_.interactable = false;
        cancelBtn_.interactable = false;
        arrowBtn_[0].interactable = false;
        arrowBtn_[1].interactable = false;
    }

    public IEnumerator ResultMagicCreate()
    {
        while (true)
        {
            if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NON)
            {
                yield return null;
            }
            else
            {
                if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NORMAL)
                {
                    judgeText_.text = "成功";
                }
                else
                {
                    judgeText_.text = "大成功";
                }
                judgeBack_.gameObject.SetActive(true);

                movePoint_.SetMiniGameJudge(MovePoint.JUDGE.NON);// 初期化しておく
                bagMagic_.MagicCreateCheck(allName_, 100, 5, 5, saveElementKind_);// 出来上がった魔法を保存

                yield return new WaitForSeconds(2.0f);
                cancelBtn_.interactable = true;
                judgeBack_.gameObject.SetActive(false);
                miniGameMng.gameObject.SetActive(false);
                judgeText_.text = "";

                if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
                {
                    Debug.Log("空のマテリアがなくなりました。ワード合成を終了します");
                    OnClickCancelCtn();
                    yield break;
                }

                ResetCommon();
                yield break;
            }
        }
    }

    public void OnClickCancelCtn()
    {
        // 魔法の合成をやめる
        ResetCommon();
        this.gameObject.SetActive(false);
        uniHouseCanvas.gameObject.SetActive(true);
    }

    private void ResetCommon()
    {
        // Init()とワード合成後とワード合成終了時に呼ぶ
        miniGameMng.localPosition = new Vector3(0.0f, -180.0f, 0.0f);

        infoText_.text = "";
        allName_ = "";
        kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
        topicText_.text = topicString_[kindNum_];

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            // 一度でも代入されたことがあるなら初期化する
            if (saveNumber_[i] != -1)
            {
                mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].btn.image.color = Color.white;
                selectWord_[i] = null;
                // 誰にも該当しない番号を入れる
                saveNumber_[i] = -1;
                oldNumber_[i] = saveNumber_[i];
            }
        }

        for (int k = 0; k < (int)Bag_Word.WORD_MNG.MAX; k++)
        {
            // すべて非表示にしておく
            for (int i = 0; i < mngMaxCnt[k]; i++)
            {
                //Debug.Log((Bag_Word.WORD_MNG)k + "       " + i + "      " + mCreateData[(Bag_Word.WORD_MNG)k][i].name);
                mCreateData[(Bag_Word.WORD_MNG)k][i].btn.interactable = false;
                mCreateData[(Bag_Word.WORD_MNG)k][i].pleate.gameObject.SetActive(false);
            }
        }
        // ヘッドのワードは表示させておく
        for (int i = 0; i < mngMaxCnt[(int)InitPopList.WORD.HEAD]; i++)
        {
            if (Bag_Word.wordState[InitPopList.WORD.HEAD][i].getFlag == true)
            {
                mCreateData[(int)Bag_Word.WORD_MNG.HEAD][i].btn.interactable = true;
                mCreateData[(int)Bag_Word.WORD_MNG.HEAD][i].pleate.SetActive(true);
            }
        }
        arrowBtn_[0].interactable = false;
        arrowBtn_[1].interactable = false;
        createFlag_ = false;
        createBtn_.interactable = createFlag_;
    }
}