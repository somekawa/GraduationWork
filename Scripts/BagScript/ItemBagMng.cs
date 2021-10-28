using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    // private GameObject itemBagCanvas_;// シーン遷移後も使いたいオブジェクトたちの親
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

    // ItemBox選択時
    private RectTransform topicParent_;// 何を表示しているか
    private RectTransform[] mngs_ = new RectTransform[(int)topic.MAX];
    private Bag_Materia bagMateria_;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    private bool activeFlag_ = false;

    public void Init()
    {
        // itemBag_ = BagCanvasUI.Instance;
        itemBagMng_ = transform.GetComponent<RectTransform>();
        mngs_[(int)topic.ITEM] = itemBagMng_.transform.Find("ItemMng").GetComponent<RectTransform>();
        mngs_[(int)topic.MATERIA] = itemBagMng_.transform.Find("MateriaMng").GetComponent<RectTransform>();
        mngs_[(int)topic.WORD] = itemBagMng_.transform.Find("WordMng").GetComponent<RectTransform>();

        bagItem_ = itemBagMng_.transform.Find("ItemMng").GetComponent<Bag_Item>();
        bagMateria_ = itemBagMng_.transform.Find("MateriaMng").GetComponent<Bag_Materia>();
        bagWord_ = itemBagMng_.transform.Find("WordMng").GetComponent<Bag_Word>();

        topicParent_ = itemBagMng_.transform.Find("Topics").GetComponent<RectTransform>();
        topicText_ = topicParent_.transform.Find("TopicName").GetComponent<Text>();
        topicText_.text = topicString_[(int)topic.ITEM];

        //for (int i = 0; i < (int)topic.MAX; i++)
        //{
        //    Debug.Log(stringNum_ + "      "+mngs_[i].gameObject.name);
        //  //  Debug.Log();
        //}
        StartCoroutine(ActiveRectTransform((int)topic.ITEM, true));
    }

    public void OnClickRightArrow()
    {
        // 値を加算
        stringNum_++;
        if ((int)topic.WORD < stringNum_)
        {
            stringNum_ = (int)topic.ITEM;
        }
        //ActiveRectTransform();
        topicText_.text = topicString_[stringNum_];
        Debug.Log("右矢印をクリック" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        stringNum_--;
        if (stringNum_ < (int)topic.ITEM)
        {
            stringNum_ = (int)topic.WORD;
        }
        topicText_.text = topicString_[stringNum_];
        //ActiveRectTransform();
        Debug.Log("左矢印をクリック" + stringNum_);
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
