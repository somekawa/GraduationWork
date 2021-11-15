using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    [SerializeField]
    private Sprite[] CharaImage_;
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

    // ItemBox選択時
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    private MenuActive menuActive_;     // 現在のページ状態がどこかを取得する(矢印に使用する)

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
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = topicParent_.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = topicParent_.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = topicParent_.transform.Find("CharaImage").GetComponent<Image>();
        }

        //サイズが変更して画像を切り替える
        charaImgRect_.sizeDelta = new Vector2(CharaImage_[charaStringNum_].rect.width, CharaImage_[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage_[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
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
                stringNum_ = (int)TOPIC.WORD;
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
        charaImgRect_.sizeDelta = new Vector2(CharaImage_[charaStringNum_].rect.width, CharaImage_[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage_[charaStringNum_];
    }

    public void ActiveRectTransform()
    {
        Debug.Log(mngs_[stringNum_].gameObject.name + "を表示します               "+ stringNum_);

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // 選択のものを表示
                mngs_[i].gameObject.SetActive(true);
                Debug.Log(mngs_[i].gameObject.name+"を表示しています");
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
            // 魔法合成からバッグのワードを開くと親の場所がずれてしまうため
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
        }
    }
}
