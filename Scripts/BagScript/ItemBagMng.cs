using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    [SerializeField]
    private Sprite[] CharaImage_;

    private RectTransform itemBagMng_;// itemBag_の子

    public enum topic
    {
        NON = -1,
        ITEM,// 魔法
        MATERIA,// 素材
        WORD,// ワード
        MAX,
    }
    // アイテム選択時に表示するトピック名
    private Text topicText_;
    private int stringNum_ = (int)topic.ITEM;
    private string[] topicString_ = new string[(int)topic.MAX] {
    "アイテム","そざい","わーど"};

    // ステータス表示にでてくるトピック名
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // 台紙サイズ変更用
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "ユニ","ジャック"};

    // ItemBox選択時
    private RectTransform[] mngs_ = new RectTransform[(int)topic.MAX];
    private Bag_Materia bagMateria_;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    private MenuActive menuActive_;     // 現在のページ状態がどこかを取得する(矢印に使用する)
    private bool activeFlag_ = false;

    public void Init()
    {
        // 1度情報を取得したら参照しないようにする
        if(itemBagMng_ == null)
        {
            itemBagMng_ = transform.GetComponent<RectTransform>();
        }

        if(mngs_[(int)topic.ITEM] == null)
        {
            mngs_[(int)topic.ITEM] = itemBagMng_.transform.Find("ItemMng").GetComponent<RectTransform>();
        }
        if (mngs_[(int)topic.MATERIA] == null)
        {
            mngs_[(int)topic.MATERIA] = itemBagMng_.transform.Find("MateriaMng").GetComponent<RectTransform>();
        }
        if (mngs_[(int)topic.WORD] == null)
        {
            mngs_[(int)topic.WORD] = itemBagMng_.transform.Find("WordMng").GetComponent<RectTransform>();
        }

        if(bagItem_ == null)
        {
            bagItem_ = GameObject.Find("Managers").GetComponent<Bag_Item>();
        }
        if(bagMateria_ == null)
        {
            bagMateria_ = GameObject.Find("Managers").GetComponent<Bag_Materia>();
        }
        if(bagWord_ == null)
        {
            bagWord_ = GameObject.Find("Managers").GetComponent<Bag_Word>();
        }

        if(topicText_ == null)
        {
            RectTransform topicParent_ = itemBagMng_.transform.Find("Topics").GetComponent<RectTransform>();
            topicText_ = topicParent_.transform.Find("TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)topic.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        StartCoroutine(ActiveRectTransform((int)topic.ITEM, true));
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

        if(charaImg_ == null)
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
            if ((int)topic.WORD < stringNum_++)
            {
                stringNum_ = (int)topic.ITEM;
            }
            topicText_.text = topicString_[stringNum_];
            Debug.Log("右矢印をクリック" + stringNum_);
        }
        else if(menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if(EventMng.GetChapterNum() < 8)
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
    }

    public void OnClickLeftArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if (--stringNum_ < (int)topic.ITEM)
            {
                stringNum_ = (int)topic.WORD;
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

    public IEnumerator ActiveRectTransform(int startNum, bool flag)
    {
        activeFlag_ = flag;
        while (activeFlag_)
        {
            yield return null;
            for (int i = 0; i < (int)topic.MAX; i++)
            {
                if (stringNum_ == i)
                {
                    // 選択のものを表示
                    mngs_[i].gameObject.SetActive(true);
                }
                else
                {
                    // 選択中のもの以外は非表示に
                    mngs_[i].gameObject.SetActive(false);
                }
            }

            if (stringNum_ == (int)ItemBagMng.topic.MATERIA)
            {
                // parentCanvas_[(int)canvas.BAG].transform.Find("MateriaMng").GetComponent<Bag_Materia>().Init(this);
                bagMateria_.ActiveMateria(this);
            }
            else if (stringNum_ == (int)ItemBagMng.topic.WORD)
            {
                //parentObjPrefab_.transform.Find("WordMng").GetComponent<Bag_Word>().Init(this);
                bagWord_.ActiveWord(this);
            }
            else if (stringNum_ == (int)ItemBagMng.topic.ITEM)
            {
                //parentObjPrefab_.transform.Find("ItemMng").GetComponent<Bag_Item>().Init(this);
                bagItem_.ActiveItem(this);
                //bagItem_.ItemGetCheck(EventMng.GetChapterNum());
            }
        }
    }

    public int GetStringNumber()
    {
        return stringNum_;
    }

    public void SetActiveCanvas()
    {
        StopCoroutine(ActiveRectTransform((int)topic.ITEM, false));
    }
}
