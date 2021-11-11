using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCreate : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas_;

    [SerializeField]
    private RectTransform miniGameParent_;    // ミニゲーム表示用

    [SerializeField]
    private RectTransform magicCreateParent_;    // 素材を拾ったときに生成されるプレハブ

    // ワードの最大個数取得用
    private PopMateriaList popMateriaList_;
    private int maxCnt_ = 0;

    // バッグの中身（ワード）
    private int stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
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

    //  0.作成開始ボタン　1.魔法合成終了ボタン
    private Button createBtn_;
    private Button cancelBtn_;
    private Bag_Magic bagMagic_;
    private string allName_ = "";
    private int[] saveNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];// 選択したワードの番号を保存
    private int[] oldNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];

    // 矢印ボタン
    private Button[] arrowBtn_ = new Button[2];

    // ミニゲームスタート用
    private MovePoint movePoint_;
    private Image judeBack_;
    private Text judgText_;

    public void Init()
    {
        // ワードの最大個数を取得
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<PopMateriaList>();
        maxCnt_ = popMateriaList_.SetMaxWordCount();
        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();


        // 作成開始ボタン
        createBtn_ = transform.Find("InfoMng/CreateBtn").GetComponent<Button>();
        createBtn_.interactable = false;

        // 魔法合成終了ボタンを代入
        cancelBtn_ = transform.Find("InfoMng/CancelBtn").GetComponent<Button>();

        // ミニゲーム処理
        movePoint_ = miniGameParent_.transform.GetComponent<MovePoint>();
        judeBack_ = transform.Find("JudgBack").GetComponent<Image>();
        judgText_ = judeBack_.transform.Find("Text").GetComponent<Text>();
        judeBack_.gameObject.SetActive(false);
        miniGameParent_.gameObject.SetActive(false);
        judgText_.text = "";

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
            materiaImage_.sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][materiaNum_];
            materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();
        }
        ResetCommon();

        // ワードたちを魔法合成用の親の子に移動させる
        if (Bag_Word.wordState_[0].pleate.transform.parent != magicCreateParent_.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                Bag_Word.wordState_[i].pleate.transform.SetParent(magicCreateParent_.transform);
            }
            //  Debug.Log("親の位置をずらしました。");
        }
    }

    public void OnClickRightArrow()
    {
        // 値を加算
        stringNum_++;
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "回復"
         && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "補助")
        {
            // 攻撃系Elementを選択時に必中をもっていなかったらSub2でストップ
            if (Bag_Word.wordState_[targetWordNum_].getFlag == false)
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
                {
                    stringNum_ = (int)Bag_Word.WORD_MNG.SUB2;
                }
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "回復")
        {
            if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.SUB2;
            }
        }
        else
        {
            // 何もしない
        }
        // Debug.Log(selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
        ActiveKindsCheck((Bag_Word.WORD_MNG)stringNum_, false);
        //  Debug.Log("右矢印をクリック" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        // ワード種別を1つ戻るときにその種類のワードを選択していれば
        //  Debug.Log("1つ前の種類に戻ります。");
        // ボタンを押下できるようにして色を元に戻す
        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.image.color = Color.white;
        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.interactable = true;
        selectWord_[stringNum_] = null;
        oldNumber_[stringNum_] = -1;

        stringNum_--;

        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.image.color = Color.green;
        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.interactable = false;

        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        ActiveKindsCheck((Bag_Word.WORD_MNG)stringNum_, true);
        Debug.Log("左矢印をクリック" + stringNum_);
    }

    public void ActiveKindsCheck(Bag_Word.WORD_MNG kind, bool leftFlag)
    {
        topicText_.text = topicString_[(int)kind];

        if (leftFlag == true)
        {
            // 左矢印が動ける最大位置
            if (stringNum_ <= (int)Bag_Word.WORD_MNG.HEAD)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
                // Headの時は押下できないようにする
                arrowBtn_[0].interactable = false;
            }
            arrowBtn_[1].interactable = true;
        }
        else
        {
            // 右矢印が動ける最大位置
            if ((int)Bag_Word.WORD_MNG.SUB3 <= stringNum_)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.SUB3;
            }
            arrowBtn_[0].interactable = true;
            arrowBtn_[1].interactable = false;// 右矢印を押したら必ずfalse
        }

        Debug.Log(topicText_.text);
        for (int i = 0; i < maxCnt_; i++)
        {
            // 全て非表示にしておく
            Bag_Word.wordState_[i].pleate.SetActive(false);
            // 取得しているか
            if (Bag_Word.wordState_[i].getFlag == true)
            {
                // ワードの種類と一致している番号だけ表示
                SelectWordKindCheck(i, (Bag_Word.WORD_MNG)stringNum_);
            }
        }

        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == null)
        {
            createBtn_.interactable = true;
        }
        else
        {
            createBtn_.interactable = false;
        }

        // Elementに何らかのワードが入っていたらサブで表示するワードをチェックする
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != null)
        {
            ElementCheck(kind);
        }

        // すでに選択状態のボタンは押下できないようにする
        if (selectWord_[stringNum_] != null)
        {
            Bag_Word.wordState_[saveNumber_[stringNum_]].btn.interactable = false;
        }
    }

    private void SelectWordKindCheck(int wordNum, Bag_Word.WORD_MNG wordKind)
    {
        switch (wordKind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.HEAD)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ELEMENT_ASSIST
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ELEMENT_HEAL
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ELEMENT_ATTACK)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.TAIL:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.TAIL)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB1:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.SUB1
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.SUB1_AND_SUB2
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.SUB2
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.SUB1_AND_SUB2
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                if (Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.SUB3
                 || Bag_Word.wordState_[wordNum].kinds == PopMateriaList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState_[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState_[wordNum].pleate.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

    public void ElementCheck(Bag_Word.WORD_MNG kind)
    {
        // Debug.Log("選択したElementのワード："+selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
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
                Debug.Log("interactableをtrueにしておく");
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "味方", true, PopMateriaList.WORD.SUB1);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "HP", true, PopMateriaList.WORD.SUB2);
                    SetInteractableCheck(i, "即死", false, PopMateriaList.WORD.SUB1_AND_SUB2);
                }
                // 作成開始ボタンを押下可能状態にする
                createBtn_.interactable = true;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                // Elementで回復選択時にSub3で選択できるものはない
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
                for (int i = 0; i < maxCnt_; i++)
                {
                    // 敵と味方のワードだけ押下可能
                    if (Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.SUB1)
                    {
                        Bag_Word.wordState_[i].btn.interactable = true;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "HP", false, PopMateriaList.WORD.SUB2);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == "敵")
                    {
                        SetInteractableCheck(i, "低下", true, PopMateriaList.WORD.SUB3);
                    }
                    else
                    {
                        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == "防御力")
                        {
                            SetInteractableCheck(i, "上昇", true, PopMateriaList.WORD.SUB3);
                        }
                        else
                        {
                            if (Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.SUB3
                             || Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.ALL_SUB)
                            {
                                // 低下以外を押下可能
                                if (Bag_Word.wordState_[i].name == "上昇"
                                    || Bag_Word.wordState_[i].name == "反射"
                                    || Bag_Word.wordState_[i].name == "吸収")
                                {
                                    Bag_Word.wordState_[i].btn.interactable = true;
                                }
                            }
                        }
                    }
                }
                createBtn_.interactable = true;
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
                // InteractableWordPleate(null, PopMateriaList.WORD.SUB1, PopMateriaList.WORD.SUB1_AND_SUB2);
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.SUB1_AND_SUB2
                        || Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.ALL_SUB)
                    {
                        Bag_Word.wordState_[i].btn.interactable = true;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                // HPだけ押下できない状態に
                // InteractableWordPleate("HP", PopMateriaList.WORD.SUB2, PopMateriaList.WORD.SUB1_AND_SUB2);
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.SUB1_AND_SUB2
                    || Bag_Word.wordState_[i].kinds == PopMateriaList.WORD.ALL_SUB)
                    {
                        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] != Bag_Word.wordState_[i].name)
                        {
                            // Sub1で選択したワード以外を押下可能に
                            Bag_Word.wordState_[i].btn.interactable = true;
                        }
                    }
                }
                // 作成ボタン押下可能にする
                createBtn_.interactable = true;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                // 必中を持っていたら
                if (Bag_Word.wordState_[targetWordNum_].getFlag == true)
                {
                    // 必中を表示
                    Bag_Word.wordState_[targetWordNum_].btn.interactable = true;
                }
                // 作成開始ボタンを押下可能状態にする
                createBtn_.interactable = true;
                break;

            default:
                break;
        }
    }
    private void SetInteractableCheck(int number, string num, bool flag, PopMateriaList.WORD word)
    {
        // nameを==で判断する場合はflagがtrue
        if (flag == true)
        {
            if (Bag_Word.wordState_[number].kinds == word)
            {
                // 上昇のみ押下できる
                if (Bag_Word.wordState_[number].name == num)
                {
                    Bag_Word.wordState_[number].btn.interactable = true;
                }
            }
        }
        else
        {
            if (Bag_Word.wordState_[number].kinds == word)
            {
                if (Bag_Word.wordState_[number].name != num)
                {
                    Bag_Word.wordState_[number].btn.interactable = true;
                }
            }
        }
    }

    public void SetWord(string word)
    {
        if ((int)Bag_Word.WORD_MNG.SUB3 == stringNum_)
        {
            // サブ3を表示中は右矢印を非アクティブにする
            arrowBtn_[1].interactable = false;
        }
        else
        {
            arrowBtn_[1].interactable = true;
        }

        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "回復"
         && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "補助")
        {
            if (Bag_Word.wordState_[targetWordNum_].getFlag == false)
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
                {
                    arrowBtn_[1].interactable = false;// 右矢印を押したら必ずfalse
                }
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "回復")
        {
            if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
            {
                arrowBtn_[1].interactable = false;// 右矢印を押したら必ずfalse
            }
        }
        else
        {
            // 何もしない
        }

        // ワードのボタンを押下したら呼び出す
        //int number = 0;
        for (int i = 0; i < maxCnt_; i++)
        {
            // どの種類の時にどのワードが押下されたか
            if (word == Bag_Word.wordState_[i].name)
            {
                // 押下されたワードの番号を代入する
                saveNumber_[stringNum_] = i;
                break;
            }
        }

        // -1以外は同じワード種別内で複数のボタンが押されたとき
        if (oldNumber_[stringNum_] != -1)
        {
            // 1つ前に押されたボタンは初期状態に戻す
            Bag_Word.wordState_[oldNumber_[stringNum_]].btn.image.color = Color.white;
            Bag_Word.wordState_[oldNumber_[stringNum_]].btn.interactable = true;
        }

        // Sub2を選択中
        if (stringNum_ == (int)Bag_Word.WORD_MNG.SUB2)
        {
            // 1つ前のSubで選択していたものは押下状態のままにする
            Bag_Word.wordState_[saveNumber_[stringNum_ - 1]].btn.image.color = Color.green;
            Bag_Word.wordState_[saveNumber_[stringNum_ - 1]].btn.interactable = false;
        }
        //Debug.Log(Bag_Word.wordState_[saveNumber_[stringNum_]].btn.name);

        // 選択したワードの色を緑にして選択できないようにする
        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.image.color = Color.green;
        Bag_Word.wordState_[saveNumber_[stringNum_]].btn.interactable = false;

        // トピックからどの種類の時に選ばれたワードなのかを見る
        selectWord_[stringNum_] = Bag_Word.wordState_[saveNumber_[stringNum_]].name;
        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        allName_ = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB3];// 作られた名前を保存しておく
        oldNumber_[stringNum_] = saveNumber_[stringNum_];
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
            if (allName_ == Bag_Magic.data_[0].name)
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
        miniGameParent_.gameObject.SetActive(true);

        // ゲームが始まるため押下できないようにする
        createBtn_.interactable = false;
        cancelBtn_.interactable = false;
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
                    judgText_.text = "成功";
                }
                else
                {
                    judgText_.text = "大成功";
                }
                judeBack_.gameObject.SetActive(true);

                movePoint_.SetMiniGameJudge(MovePoint.JUDGE.NON);// 初期化しておく
                bagMagic_.MagicCreateCheck(allName_, 100, 5, 5);// 出来上がった魔法を保存

                yield return new WaitForSeconds(2.0f);
                cancelBtn_.interactable = true;
                judeBack_.gameObject.SetActive(false);
                miniGameParent_.gameObject.SetActive(false);
                judgText_.text = "";

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
        uniHouseCanvas_.gameObject.SetActive(true);
    }

    private void ResetCommon()
    {
        // Init()とワード合成後とワード合成終了時に呼ぶ
        miniGameParent_.localPosition = new Vector3(0.0f, -180.0f, 0.0f);

        arrowBtn_[0].interactable = false;
        arrowBtn_[1].interactable = false;
        infoText_.text = "";
        allName_ = "";
        stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
        topicText_.text = topicString_[stringNum_];

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            // 一度でも代入されたことがあるなら初期化する
            if (saveNumber_[i] != -1)
            {
                Bag_Word.wordState_[saveNumber_[i]].btn.image.color = Color.white;
                selectWord_[i] = null;
                // 誰にも該当しない番号を入れる
                saveNumber_[i] = -1;
                oldNumber_[i] = saveNumber_[i];
            }
        }

        for (int i = 0; i < maxCnt_; i++)
        {
            Bag_Word.wordState_[i].pleate.gameObject.SetActive(false);//#
            Bag_Word.wordState_[i].btn.interactable = false;//#

            if (Bag_Word.wordState_[i].getFlag == true)
            {
                // ワードの種類と一致しているか
                if (PopMateriaList.WORD.HEAD == Bag_Word.wordState_[i].kinds)
                {
                    Bag_Word.wordState_[i].pleate.gameObject.SetActive(true);
                    Bag_Word.wordState_[i].btn.interactable = true;//#
                    //Bag_Word.wordState_[i].btn.image.color = Color.clear;
                }
            }
        }
    }

}