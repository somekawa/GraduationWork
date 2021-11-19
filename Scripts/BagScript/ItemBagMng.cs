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

    // ステータス表示にでてくるトピック名
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // 台紙サイズ変更用
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "ユニ","ジャック"};
    private RectTransform statusMagicCheck_;// ステータス画面で魔法を表示するための親
    private Image[] equipMagic_ = new Image[4];
    private int[,] setMagicNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];
    private int setNullNum_ = 100;// 魔法が装備されてないときの番号

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
            topicText_ = transform.Find("Topics/TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)TOPIC.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }
        ActiveRectTransform();
    }

    public void StatusInit()
    {
        if (menuActive_ == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = topicParent_.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
            statusMagicCheck_ = topicParent_.Find("MagicCheck").GetComponent<RectTransform>();

            charasList_ = SceneMng.charasList_;
            var data = SceneMng.GetCharasSettings(charaStringNum_);

            for (int i = 0; i < 4; i++)
            {
                // setMagicNum_[charaStringNum_, i] = data.Magic[i];
                setMagicNum_[charaStringNum_, i] = data.Magic[i].element;
                 //   (int)charasList_[charaStringNum_].GetMagicNum(btnNumber_);

                equipMagic_[i] = topicParent_.Find("MagicSetMng/MagicSet" + i + "/Icon").GetComponent<Image>();
                Debug.Log(setMagicNum_[charaStringNum_, i]);
                if (setMagicNum_[charaStringNum_, i] != setNullNum_)
                {
                    // 魔法をセットしていたらその画像をステータス画面に出す
                    equipMagic_[i].sprite =Bag_Magic.magicSpite[i];
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = topicParent_.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = topicParent_.transform.Find("CharaImage").GetComponent<Image>();
        }

        //サイズが変更して画像を切り替える
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
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
            Debug.Log("右矢印をクリック" + stringNum_);
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
            Debug.Log("右矢印をクリック" + charaStringNum_);
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
            Debug.Log("左矢印をクリック" + stringNum_);
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
            Debug.Log("左矢印をクリック" + charaStringNum_);
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
            if (setMagicNum_[charaStringNum_, i] != setNullNum_)
            {
                equipMagic_[i].sprite = Bag_Magic.magicSpite[i];
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void ActiveRectTransform()
    {
        Debug.Log(mngs_[stringNum_].gameObject.name + "を表示します               " + stringNum_);

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // 選択のものを表示
                mngs_[i].gameObject.SetActive(true);
                Debug.Log(mngs_[i].gameObject.name + "を表示しています");
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
        if (stringNum_ == (int)TOPIC.MATERIA)
        {
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
        }
    }

    public void OnClickSetMagicButton()
    {
        // ステータスを開いて魔法をセットするボタンを押した際
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // 持っている魔法一覧を表示
    }

    public void OnClickCloseMagicButton()
    {
        //data[num].battleSet0 = flag;
        statusMagicCheck_.gameObject.SetActive(false);     // 持っている魔法一覧を表示
    }

    public void SetMagicCheck(int num)
    {
        // キャラのステータス値を表示させたい
        charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);
        setMagicNum_[charaStringNum_, btnNumber_] = num;

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