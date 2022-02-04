using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private int btnNumber_ = 0;

    [SerializeField]
    private Sprite[] CharaImage;
    [SerializeField]
    private RectTransform statusMngObj;

    private RectTransform itemBagChild_;// itemBag_の子

    public enum TOPIC
    {
        NON = -1,
        ITEM,// アイテム
        MATERIA,// 素材
        WORD,// ワード
        MAGIC,
        MAX,
    }
    // アイテム選択時に表示するトピック名
    private Text topicText_;
    private int stringNum_ = (int)TOPIC.ITEM;
    private string[] topicString_ = new string[(int)TOPIC.MAX] {
    "アイテム","マテリア","ワード","マジック"};
    private Text info_; // クリックしたアイテムを説明する欄
    private Button[] throwAwayBtn_ = new Button[(int)TOPIC.MAGIC];

    // ステータス表示にでてくるトピック名
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // 台紙サイズ変更用
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "ユニ","ジャック"};

    private Bag_Magic bagMagic_;
    // ステータス関連 魔法
    private int magicCnt_ = -1;// 所持している魔法の数
    private StatusMagicMng magicMng_;
    private RectTransform statusMagicCheck_;// ステータス画面で魔法を表示するための親
    // 魔法セット先
    private Button[] equipBtn_ = new Button[4];// セット先のボタン
    private Image[] equipMagic_ = new Image[4];// セットされている魔法の画像
    private Image[] equipExMagic_ = new Image[4];// セットされている魔法の画像
    private Color equipBtnSelColor_ = new Color(0.1f, 0.5f, 0.7f, 1.0f);// セット先の選択中の色
    private Color equipNormalColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);// 何も選んでない状態の色
    private Color equipResetColor_ = new Color(1.0f, 1.0f, 1.0f, 0.0f);// 選択できない状態の色
    // 所持魔法一覧
    private int[,] setImageNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];// 保存される魔法の画像
    private Button seleMagicBtn_;
    private Button removeEquipBtn_;// 魔法をはずすボタン

    private int setNullNum_ = 0;// 魔法が装備されてないときの番号
    // どの魔法を保存しているか読み込む
    private int[,] dataCheck_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];

    // ItemBox選択時
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    // 現在のページ状態がどこかを取得する(矢印に使用する)
    private MenuActive menuActive_;

    // Chara.csをキャラ毎にリスト化する
    private List<Chara> charasList_ = new List<Chara>();

    private CharaUseMagic useMagic_;

    public void Init()
    {
        // 1度情報を取得したら参照しないようにする
        if (itemBagChild_ == null)
        {
            itemBagChild_ = transform.GetComponent<RectTransform>();
        }

        if (mngs_[(int)TOPIC.ITEM] == null)
        {
            mngs_[(int)TOPIC.ITEM] = transform.Find("ItemMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.MATERIA] = transform.Find("MateriaMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.WORD] = transform.Find("WordMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.MAGIC] = transform.Find("MagicMng").GetComponent<RectTransform>();
        }

        if (topicText_ == null)
        {
            var itemBagMng = GameObject.Find("ItemBagMng").GetComponent<RectTransform>();
            info_ = itemBagMng.Find("InfoBack/InfoText").GetComponent<Text>();
            var infoBack = itemBagMng.Find("InfoBack").GetComponent<RectTransform>();
            throwAwayBtn_[0] = infoBack.Find("ItemDelete").GetComponent<Button>();
            throwAwayBtn_[1] = infoBack.Find("MateriaDelete").GetComponent<Button>();
            throwAwayBtn_[2] = infoBack.Find("MagicDelete").GetComponent<Button>();
            topicText_ = transform.Find("Topics/TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)TOPIC.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }
        info_.text = "";
        ActiveRectTransform();
    }

    public void StatusInit()
    {
        charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = statusMngObj.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = statusMngObj.transform.Find("CharaImage").GetComponent<Image>();
            statusMagicCheck_ = statusMngObj.transform.Find("MagicCheckMask/MagicCheck").GetComponent<RectTransform>();
            removeEquipBtn_ = statusMagicCheck_.transform.Find("Viewport/StatusMagicParent/RemoveMagicBox").GetComponent<Button>();
            info_ = statusMngObj.transform.Find("MagicSetBack/Info/MagicText").GetComponent<Text>();
        }
        MagicInit();

        if (magicMng_ == null)
        {
            magicMng_ = statusMngObj.GetComponent<StatusMagicMng>();
        }
        magicMng_.Init();

        //サイズが変更して画像を切り替える
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        //  RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = statusMngObj.transform.Find("Topics/TopicName").GetComponent<Text>();
        }
        charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];

        info_.text = (magicCnt_ < 2) ? "装備できる魔法がありません" : info_.text = "魔法を選択してください";

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
        useMagic_ = new CharaUseMagic();
    }

    public void MagicInit()
    {

        charasList_ = SceneMng.charasList_;

        if (magicMng_ == null)
        {
            //デバッグ用
            //  GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
            bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();
            magicMng_ = statusMngObj.GetComponent<StatusMagicMng>();
        }
        magicMng_.Init();
        magicCnt_ = bagMagic_.MagicNumber();

        for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
        {
            var data = SceneMng.GetCharasSettings(c);
            for (int i = 0; i < 4; i++)
            {
                dataCheck_[c, i] = data.Magic[i];

                bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)c, dataCheck_[c, i], true);

                setImageNum_[c, i] = Bag_Magic.data[dataCheck_[c, i]].element;
                //Debug.Log((SceneMng.CHARACTERNUM)c + "の" + i + "番目:" + data.Magic[i] + "番の魔法/名前：" +
                //    Bag_Magic.data[data.Magic[i]].name +
                //    "/エレメント番号" + Bag_Magic.data[data.Magic[i]].element);

                if (statusMngObj.gameObject.activeSelf == true)
                {
                    // StatusMngがアクティブ状態なら魔法セット先を探す
                    equipBtn_[i] = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + i).GetComponent<Button>();
                    equipMagic_[i] = equipBtn_[i].transform.Find("Icon").GetComponent<Image>();
                    equipExMagic_[i] = equipBtn_[i].transform.Find("ExIcon").GetComponent<Image>();
                    equipBtn_[i].interactable = false;
                }
            }
        }

        if (statusMngObj.gameObject.activeSelf == true)
        {
            Debug.Log("魔法所持数：" + magicCnt_);
            if (magicCnt_ < 2)
            {
                // 0番（1つ目）の魔法がエラーのため2つ以上あるかを比べる
                equipBtn_[0].interactable = false;
                equipBtn_[1].interactable = false;
                equipBtn_[2].interactable = false;
                equipBtn_[3].interactable = false;
                return;
            }

            // 魔法セット先を選択していないため外すボタンは押せないようにする
            removeEquipBtn_.interactable = false;

            Debug.Log("選択中のキャラクター：" + charaStringNum_);
            MagicBtnCheck();

            // 魔法がセットされてる関係なく一番左は押下可能
            equipBtn_[0].interactable = true;
        }
    }

    public void OnClickRightArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if ((int)TOPIC.WORD < stringNum_++)
            {
                stringNum_ = (int)TOPIC.ITEM;
            }
            topicText_.text = topicString_[stringNum_];
            //  Debug.Log("右矢印をクリック" + stringNum_);
        }
        else if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if (EventMng.GetChapterNum() < 8)
            {
                // ジャックのステータスは表示しない(まだ仲間になってないから)
                return;
            }

            if (++charaStringNum_ >= (int)SceneMng.CHARACTERNUM.MAX)
            {
                charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
            }
            ArrowCommon();
            //    Debug.Log("右矢印をクリック" + charaStringNum_);
        }
        else
        {
            // 何も処理を行わない
        }

        if (gameObject.activeSelf == true)
        {
            ActiveRectTransform();
        }
    }

    public void OnClickLeftArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if (--stringNum_ < (int)TOPIC.ITEM)
            {
                stringNum_ = (int)TOPIC.MAGIC;
            }
            topicText_.text = topicString_[stringNum_];
            //  Debug.Log("左矢印をクリック" + stringNum_);
        }
        else if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if (EventMng.GetChapterNum() < 8)
            {
                // ジャックのステータスは表示しない(まだ仲間になってないから)
                return;
            }

            if (--charaStringNum_ < (int)SceneMng.CHARACTERNUM.UNI)
            {
                charaStringNum_ = (int)SceneMng.CHARACTERNUM.JACK;
            }
            ArrowCommon();
            // Debug.Log("左矢印をクリック" + charaStringNum_);
        }
        else
        {
            // 何も処理を行わない
        }

        if (gameObject.activeSelf == true)
        {
            ActiveRectTransform();
        }
    }

    // 矢印の共通処理部分
    private void ArrowCommon()
    {
        charaNameTopicText_.text = charaTopicString_[charaStringNum_];
        menuActive_.ViewStatus(charaStringNum_);

        //サイズが変更して画像を切り替える
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        if (magicCnt_ < 2)
        {
            // 0番（1つ目）の魔法がエラーのため2つ以上あるかを比べる
            equipBtn_[0].interactable = false;
            equipBtn_[1].interactable = false;
            equipBtn_[2].interactable = false;
            equipBtn_[3].interactable = false;
            return;
        }


        statusMagicCheck_.gameObject.SetActive(false);     // 持っている魔法一覧を表示

        Debug.Log("選択中のセット先" + btnNumber_);

        MagicBtnCheck();


        // 魔法がセットされてる関係なく一番左は押下可能
        equipBtn_[0].interactable = true;

        // 魔法一覧の表示を消す
        magicMng_.SetCloseFlag();

        btnNumber_ = -1;// 魔法セット先の番号をリセット
    }

    private void MagicBtnCheck()
    {
        int addCnt_ = -1;
        for (int i = 0; i < 4; i++)
        {
            equipBtn_[i].image.color = equipNormalColor_;
            // セットしている魔法があるかチェック
            if (dataCheck_[charaStringNum_, i] == setNullNum_)
            {
                equipMagic_[i].sprite = null;
                equipMagic_[i].color = equipResetColor_;
                equipExMagic_[i].color = equipResetColor_;
                // セット先が選択された状態でキャラ替えしてる可能性があるため通常状態に戻す
                if (addCnt_ != i)
                {
                    // addCnt_と違う値なら非アクティブで良い
                    equipBtn_[i].interactable = false;
                }
            }
            else
            {
                // 魔法をセットしていたらその画像をステータス画面に出す
                equipMagic_[i].color = equipNormalColor_;
                    equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];

                if (Bag_Magic.data[i].rate == 2)
                {
                    equipExMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                }
                equipBtn_[i].interactable = true;
                Debug.Log(i + "番目に魔法がセットされています");
                if (i < 3)
                {
                    addCnt_ = i + 1;
                    // 選択済みの次の魔法にセットできるように
                    equipBtn_[addCnt_].interactable = true;
                }
            }
          //  Debug.Log((SceneMng.CHARACTERNUM)charaStringNum_ + "の" + i + "番目にセット中の魔法の画像番号：" + setImageNum_[charaStringNum_, i]);
        }
    }

    public void ActiveRectTransform()
    {
        //   Debug.Log(mngs_[stringNum_].gameObject.name + "を表示します               " + stringNum_);
        info_.text = "";// 説明欄の表示をリセット

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // 選択のものを表示
                mngs_[i].gameObject.SetActive(true);
                // Debug.Log(mngs_[i].gameObject.name + "を表示しています");
            }
            else
            {
                // 選択中のもの以外は非表示に
                mngs_[i].gameObject.SetActive(false);
            }
        }

        // ワードに捨てるボタンはないためfor文で回さず直で書く
        if (stringNum_ == (int)TOPIC.WORD)
        {
            // 魔法合成からバッグのワードを開くと親の場所がずれてしまうため
            GameObject.Find("Managers").GetComponent<Bag_Word>().Init();
        }
        else if (stringNum_ == (int)TOPIC.MATERIA)
        {
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
            throwAwayBtn_[0].gameObject.SetActive(false);
            throwAwayBtn_[2].gameObject.SetActive(false);
        }
        else if (stringNum_ == (int)TOPIC.ITEM)
        {
            GameObject.Find("Managers").GetComponent<Bag_Item>().Init();
            throwAwayBtn_[1].gameObject.SetActive(false);
            throwAwayBtn_[2].gameObject.SetActive(false);
        }
        else if (stringNum_ == (int)TOPIC.MAGIC)
        {
            throwAwayBtn_[0].gameObject.SetActive(false);
            throwAwayBtn_[1].gameObject.SetActive(false);
        }
        else
        {
            // 何もしない
        }
    }

    public void OnClickSetMagicButton()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        // ステータスを開いて魔法をセットするボタンを押した際
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        //// 0番目の位置に
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // 持っている魔法一覧を表示

        // 選択中の色
        equipBtn_[btnNumber_].image.color = equipBtnSelColor_;
        for (int i = 0; i < 4; i++)
        {
            if (i != btnNumber_)
            {
                // ほかのボタンが押されたため選択状態を解除する
                equipBtn_[i].image.color = equipNormalColor_;
            }
        }

        // 魔法セット先に魔法がセットされていたらはずすボタンを押せるようにする
        removeEquipBtn_.interactable = dataCheck_[charaStringNum_, btnNumber_] != setNullNum_ ? true : false;
        magicMng_.SetCoroutineFlag();
    }

    public void InfoCheck(Button btn, int number)
    {
        if (seleMagicBtn_ == null)
        {
            // 最初だけ入る
            seleMagicBtn_ = btn;
            Debug.Log(seleMagicBtn_ + "       " + btn);
        }

        info_.text = useMagic_.MagicInfoMake(Bag_Magic.data[number]);

        // 以降はseleMagicBtn_に1つ前のボタン情報があるためここから
        if (seleMagicBtn_ != btn)
        {
            Debug.Log(seleMagicBtn_ + "       " + btn);
            // 既に選択しているものがあった場合色を元に戻す
            seleMagicBtn_.image.color = equipNormalColor_;

            // 新しく選択した魔法を代入して選択状態の色にする
            seleMagicBtn_ = btn;
            seleMagicBtn_.image.color = equipBtnSelColor_;

        }
        else
        {
            Debug.Log("選択中のボタンがあります");

            if (seleMagicBtn_.image.color != equipBtnSelColor_)
            {
                // どのボタンを選択したか代入
                seleMagicBtn_ = btn;
                // 選択したボタンの色を変更
                seleMagicBtn_.image.color = equipBtnSelColor_;
                return;
            }
            // 同じもの＝2回目のクリックだったら魔法をセット
            SetMagicCheck(number, true);
            // interactable=false時の色がおかしくなるため元に戻す
            seleMagicBtn_.image.color = equipNormalColor_;
            // 中身を空にする
            seleMagicBtn_ = null;
        }
    }


    public void SetMagicCheck(int num, bool flag)
    {
        Debug.Log("セット先の魔法の番号" + btnNumber_ + "     クリックしたボタンの番号：" + num);

        if (btnNumber_ == -1)
        {
            info_.text = "装備先を選択してください";
            // セット先を選択せずに魔法を押した場合
            return;
        }

        // flag：falseの時が外す。trueの時が魔法セット
        if (flag == false)
        {
            int setMaxNum_ = -1;
            Debug.Log(btnNumber_ + "番目の魔法を外しました");

            for (int i = 0; i < 4; i++)
            {
                // どこまでセットされているかを確認する
                if (dataCheck_[charaStringNum_, i] != setNullNum_)
                {
                    setMaxNum_ = i;
                }
            }
            Debug.Log(setMaxNum_ + "個の魔法をセットしています");
            // 最大までセットされた場合
            equipMagic_[setMaxNum_].sprite = null;
            equipMagic_[setMaxNum_].color = equipResetColor_;
            equipExMagic_[setMaxNum_].color = equipResetColor_;
            if (setMaxNum_ < 3)
            {
                equipBtn_[setMaxNum_ + 1].interactable = false;
            }

            // 外すボタンが押されたら　指定の魔法をまた押下できる状態にする
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, dataCheck_[charaStringNum_, btnNumber_], false);

            // 1つ先のセット場所を、セットしてない状態にする
            for (int i = btnNumber_; i < setMaxNum_; i++)
            {
                // 複数魔法セット状態で下位の魔法を外す場合,選択中のセット場所+1の魔法を入れる
                dataCheck_[charaStringNum_, i] = dataCheck_[charaStringNum_, i + 1];
                equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i + 1]];
                if (Bag_Magic.data[i].rate == 2)
                {
                    equipExMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                }
                else
                {
                    equipExMagic_[i].color = equipResetColor_;
                }
                setImageNum_[charaStringNum_, i] = Bag_Magic.data[dataCheck_[charaStringNum_, i + 1]].element;
                charasList_[charaStringNum_].SetMagicNum(i, dataCheck_[charaStringNum_, i + 1]);
                Debug.Log("セット中の魔法の画像番号：" + setImageNum_[charaStringNum_, i]);
            }
            dataCheck_[charaStringNum_, setMaxNum_] = setNullNum_;

            // 魔法を外した際に魔法が下位のセット先にずれた場合は外すボタンを押せるようにする
            removeEquipBtn_.interactable = dataCheck_[charaStringNum_, btnNumber_] != setNullNum_ ? true : false;

        }
        else
        {
            info_.text = useMagic_.MagicInfoMake(Bag_Magic.data[num]);
            for (int i = 0; i < 4; i++)
            {
                // 今からセットしたい魔法がすでにセットされているか
                if (dataCheck_[charaStringNum_, i] == num)
                {
                    Debug.Log("別ですでにセットされている魔法です");
                    btnNumber_ = -1;// 魔法セット先の番号をリセット
                    return;
                }
            }
            bagMagic_.MagicNumber();
            charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);

            // 選択されていたボタンを解除する
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, dataCheck_[charaStringNum_, btnNumber_], false);
            // 選択中のボタンを更新する
            dataCheck_[charaStringNum_, btnNumber_] = num;

            // SetMagic3以外の魔法をセット時
            if (btnNumber_ != 3)
            {
                if (dataCheck_[charaStringNum_, btnNumber_] != setNullNum_)
                {
                    // 1つ後ろの魔法をセットできるようにする
                    equipBtn_[btnNumber_ + 1].interactable = true;
                }
            }

            // 画像番号を入れ替える
            setImageNum_[charaStringNum_, btnNumber_] = Bag_Magic.data[num].element;
            // Debug.Log((SceneMng.CHARACTERNUM)charaStringNum_ + "の魔法の画像番号：" + setImageNum_[charaStringNum_, i]);
            equipMagic_[btnNumber_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, btnNumber_]];
            equipMagic_[btnNumber_].color = equipNormalColor_;
            if (Bag_Magic.data[num].rate == 2)
            {
                // 大成功の魔法の場合は星マークを表示
                equipExMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
            }

            // 選択された魔法を押下できないようにする
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, num, true);
        }

        equipBtn_[btnNumber_].image.color = equipNormalColor_;
        btnNumber_ = -1;// 魔法セット先の番号をリセット
    }

    public void OnClickCloseMagicButton()
    {
        magicMng_.SetCloseFlag();
        statusMagicCheck_.gameObject.SetActive(false);     // 持っている魔法一覧を表示

        if (btnNumber_ != -1)
        {
            equipBtn_[btnNumber_].image.color = equipNormalColor_;
        }
    }
}