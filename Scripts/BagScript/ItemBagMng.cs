using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    // データ系
    private SaveCSV saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト

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
    "アイテム","そざい","わーど","まほう"};
    private Text info_; // クリックしたアイテムを説明する欄
    private Button[] throwAwayBtn_=new Button[(int)TOPIC.MAGIC];

    // ステータス表示にでてくるトピック名
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // 台紙サイズ変更用
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "ユニ","ジャック"};


    private Bag_Magic bagMagic_;
    private RectTransform statusMagicCheck_;// ステータス画面で魔法を表示するための親
    private Button[] equipBtn_ = new Button[4];// 保存先の魔法のボタン
    private Image[] equipMagic_ = new Image[4];// 保存される魔法の画像
    private int[,] setImageNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];
    private Button seleMagicBtn_;
    private Image infoBack_;
    private int saveClickNum_ = -1;
    //private Text magicName_;

    private int setNullNum_ = 0;// 魔法が装備されてないときの番号
    // どの魔法を保存しているか読み込む
    private int[,] dataCheck_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];

    // ItemBox選択時
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    private MenuActive menuActive_;     // 現在のページ状態がどこかを取得する(矢印に使用する)
    // Chara.csをキャラ毎にリスト化する
    private List<Chara> charasList_ = new List<Chara>();

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
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
            throwAwayBtn_[0] = GameObject.Find("ItemBagMng/InfoBack/ItemDelete").GetComponent<Button>();
            throwAwayBtn_[1] = GameObject.Find("ItemBagMng/InfoBack/MateriaDelete").GetComponent<Button>();
            throwAwayBtn_[2] = GameObject.Find("ItemBagMng/InfoBack/MagicDelete").GetComponent<Button>();
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
        if (menuActive_ == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

      //  RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = statusMngObj.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
            statusMagicCheck_ = statusMngObj.Find("MagicCheck").GetComponent<RectTransform>();
            infoBack_ = statusMagicCheck_.Find("Info").GetComponent<Image>();
            info_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = statusMngObj.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = statusMngObj.transform.Find("CharaImage").GetComponent<Image>();
            MagicInit();
        }
        //サイズが変更して画像を切り替える
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
    }

    public void MagicInit()
    {
        charasList_ = SceneMng.charasList_;
        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();

        for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
        {
            var data = SceneMng.GetCharasSettings(c);
            for (int i = 0; i < 4; i++)
            {
                dataCheck_[c, i] = data.Magic[i];

                bagMagic_.SetStatusMagicCheck(dataCheck_[c, i], true);

                setImageNum_[c, i] = Bag_Magic.data[data.Magic[i]].element;
                Debug.Log((SceneMng.CHARACTERNUM)c + "の" + i + "番目:" + data.Magic[i] + "番の魔法/名前：" +
                    Bag_Magic.data[data.Magic[i]].name +
                    "/エレメント番号" + Bag_Magic.data[data.Magic[i]].element);

                if (statusMngObj.gameObject.activeSelf == true)
                {
                    // StatusMngがアクティブ状態なら魔法セット先を探す
                    equipBtn_[i] = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + i).GetComponent<Button>();
                    equipMagic_[i] = equipBtn_[i].transform.Find("Icon").GetComponent<Image>();
                    equipBtn_[i].interactable = false;
                }
            }
        }

        if (statusMngObj.gameObject.activeSelf == true)
        {
            for (int i = 0; i < 4; i++)
            {
                // セットしている魔法があるかチェック
                if (dataCheck_[charaStringNum_, i] == setNullNum_)
                {
                    equipMagic_[i].sprite = null;
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else
                {
                    // 魔法をセットしていたらその画像をステータス画面に出す
                    equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    equipBtn_[i].interactable = true;
                    Debug.Log(i+"番目にに魔法がセットされています");
                    if (i < 3)
                    {
                        // 選択済みの次の魔法にセットできるように
                        equipBtn_[i + 1].interactable = true;
                    }
                }
            }
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

        statusMagicCheck_.gameObject.SetActive(false);     // 持っている魔法一覧を表示
       
        for (int i = 0; i < 4; i++)
        {
            if (dataCheck_[charaStringNum_, i] != setNullNum_)
            {
                equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                equipBtn_[i].interactable = false;
            }
            else
            {
                equipMagic_[i].sprite = null;
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                equipBtn_[i].interactable = true;

                if (i < 3)
                {
                    equipBtn_[i + 1].interactable = true;
                }
            }
        }
    }

    public void ActiveRectTransform()
    {
       // Debug.Log(mngs_[stringNum_].gameObject.name + "を表示します               " + stringNum_);

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
        if(eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        // ステータスを開いて魔法をセットするボタンを押した際
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        //// 0番目の位置に
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // 持っている魔法一覧を表示
    }

    public void OnClickCloseMagicButton()
    {
        //data[num].battleSet0 = flag;
        statusMagicCheck_.gameObject.SetActive(false);     // 持っている魔法一覧を表示
    }

    public void InfoCheck(Button btn,Vector2 pos,int number)
    {
        if (seleMagicBtn_==null)
        {
            // 最初だけ入る
            seleMagicBtn_ = btn;
            infoBack_.gameObject.SetActive(true);
        }

        // 以降はseleMagicBtn_に1つ前のボタン情報があるためここから
        if (seleMagicBtn_ != btn)
        {
            // 既に選択しているものがあった場合色を元に戻す
            seleMagicBtn_.image.color = Color.white;
            // 説明欄の座標をずらす
            infoBack_.transform.localPosition = pos;

            // サブの文字があるかで表示が違う
            info_.text = Bag_Magic.data[number].sub == "non" ? info_.text = Bag_Magic.data[number].main : Bag_Magic.data[number].main + "\n" + Bag_Magic.data[number].sub;
        }
        else
        {
            if (seleMagicBtn_.image.color != Color.green)
            {
                // どのボタンを選択したか代入
                seleMagicBtn_ = btn;
                // 選択したボタンの色を変更
                seleMagicBtn_.image.color = Color.green;
                // サブの文字があるかで表示が違う
                info_.text = Bag_Magic.data[number].sub == "non" ? info_.text = Bag_Magic.data[number].main : Bag_Magic.data[number].main + "\n" + Bag_Magic.data[number].sub;
                return;
            }
            // 同じもの＝2回目のクリックだったら魔法をセット
            SetMagicCheck(number, true);
            // 説明欄リセットして非表示に
            info_.text = "";
            infoBack_.gameObject.SetActive(false);
            // interactable=false時の色がおおかしくなるため元に戻す
            seleMagicBtn_.image.color = Color.white;
            // 魔法一覧を閉じる
            statusMagicCheck_.gameObject.SetActive(false);
            // 中身を空にする
            seleMagicBtn_ = null;
        }
    }

    public void SetMagicCheck(int num,bool flag)
    {
        // 魔法をセットしたらセーブする
        Debug.Log("セット先の魔法の番号"+btnNumber_+"     クリックしたボタンの番号：" + num);
        infoBack_.gameObject.SetActive(false) ;

        if (flag == false)
        {
            // 外すボタンが押されたら
            equipMagic_[btnNumber_].sprite = null;
            equipMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            // 魔法を外したため次のセット先を選択できないようにする
            equipBtn_[btnNumber_ + 1].interactable = false;
            bagMagic_.SetStatusMagicCheck(dataCheck_[charaStringNum_, btnNumber_], false);

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                // 今からセットしたい魔法がすでにセットされているか
                if (dataCheck_[charaStringNum_, i] == num)
                {
                    Debug.Log("別ですでにセットされている魔法です");
                    return;
                }
            }

            charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);

            // 選択されていたボタンを解除する
            bagMagic_.SetStatusMagicCheck(dataCheck_[charaStringNum_, btnNumber_], false);
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
            equipMagic_[btnNumber_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, btnNumber_]];
            equipMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            // 選択された魔法を押下できないようにする
            bagMagic_.SetStatusMagicCheck(num,true);
        }

        saveCsvSc_.SaveStart();
        // キャラクター数分のfor文を回す
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            saveCsvSc_.SaveData(SceneMng.GetCharasSettings(i));
        }
        saveCsvSc_.SaveEnd();
    }

    public int GetClickButtonNum()
    {
        return btnNumber_;
    }

    public Sprite GetImageTest(int num)
    {
        return equipMagic_[num].sprite;
    }


}