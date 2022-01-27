using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Buy : MonoBehaviour
{
    private InitPopList popItemsList_;

    [SerializeField]
    private RectTransform buyParent;   // 表示位置の親

    public struct StoreBuy
    {
        public GameObject obj;
        public Button btn;
        public TMPro.TextMeshProUGUI priceText;        // 表示する値段
        public TMPro.TextMeshProUGUI nameText;         // 表示する素材の名前
        public int price;// = new int[2];
        public int haveCnt;// = new int[2];
        public string name;
    }
    public static Dictionary<ItemStoreMng.KIND, StoreBuy[]> buyData = new Dictionary<ItemStoreMng.KIND, StoreBuy[]>();

    private int[] maxCnt_;
    private int[] itemMaxCnt_ = new int[4] { 5, 10, 15, 19 };
    private int[] materiaMaxCnt_ = new int[5] { 7, 14, 23, 28, 34 };

    public void Init(int seleKind, int kind)
    // void Start()
    {
        maxCnt_ = new int[(int)ItemStoreMng.KIND.MAX];
        if (19 < EventMng.GetChapterNum())
        {
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = materiaMaxCnt_[4];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = itemMaxCnt_[3];
        }
        else if (16 < EventMng.GetChapterNum())
        {
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = materiaMaxCnt_[3];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = itemMaxCnt_[3];
        }
        else if (13 < EventMng.GetChapterNum())
        {
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = materiaMaxCnt_[2];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = itemMaxCnt_[2];
        }
        else if (8 < EventMng.GetChapterNum())
        {
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = materiaMaxCnt_[1];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = itemMaxCnt_[1];
        }
        else if (0 < EventMng.GetChapterNum())
        {
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = materiaMaxCnt_[0];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = itemMaxCnt_[0];
        }
        buyData[ItemStoreMng.KIND.ITEM] = InitBuyData(ItemStoreMng.KIND.ITEM, maxCnt_[(int)ItemStoreMng.KIND.ITEM]);
        buyData[ItemStoreMng.KIND.MATERIA] = InitBuyData(ItemStoreMng.KIND.MATERIA, maxCnt_[(int)ItemStoreMng.KIND.MATERIA]);

        //if (popItemsList_ == null)
        //{
        //    popItemsList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

        //    //maxCnt_ = new int[(int)ItemStoreMng.KIND.MAX];
        //    //maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = popItemsList_.SetMaxMateriaCount();
        //    //maxCnt_[(int)ItemStoreMng.KIND.ITEM] = popItemsList_.SetMaxItemCount();
        //    buyData[ItemStoreMng.KIND.ITEM] = InitBuyData(ItemStoreMng.KIND.ITEM, maxCnt_[(int)ItemStoreMng.KIND.ITEM]);
        //    buyData[ItemStoreMng.KIND.MATERIA] = InitBuyData(ItemStoreMng.KIND.MATERIA, maxCnt_[(int)ItemStoreMng.KIND.MATERIA]);
        //}
        SetActiveBuy(seleKind, maxCnt_[seleKind]);// 表示する
        InactiveBuy(kind, maxCnt_[kind]);// 非表示にする
    }

    private StoreBuy[] InitBuyData(ItemStoreMng.KIND kind, int maxCnt)
    {
        var data = new StoreBuy[maxCnt];
       // Debug.Log(maxCnt);
        for (int i = 0; i < maxCnt; i++)
        {
            data[i].haveCnt = kind == ItemStoreMng.KIND.ITEM ? Bag_Item.itemState[i].haveCnt : Bag_Materia.materiaState[i].haveCnt;
            data[i].price = kind == ItemStoreMng.KIND.ITEM ? InitPopList.itemData[i].buyPrice:InitPopList.materiaData[i].buyPrice ;
            data[i].name = kind == ItemStoreMng.KIND.ITEM ? Bag_Item.itemState[i].name: Bag_Materia.materiaState[i].name ;
            data[i].obj = kind == ItemStoreMng.KIND.ITEM ? PopListInTown.itemPleate[i] : PopListInTown.materiaPleate[i];
            data[i].obj.name = data[i].name + i;
            data[i].btn = data[i].obj.GetComponent<Button>();
            //if (kind == ItemStoreMng.KIND.ITEM)
            //{
            //    Debug.Log("番号：" + i + "名前" + data[i].name + "               " + data[i].price);
            //}
            // 表示する名前を変更する
            data[i].nameText = data[i].obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            data[i].nameText.text = data[i].name;

            // 料金を表示するText
            data[i].priceText = data[i].obj.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>();
            data[i].priceText.text = data[i].price.ToString();
            data[i].obj.transform.SetParent(buyParent.transform);
            data[i].obj.SetActive(true);
        }
        return data;
    }

    public void SetActiveBuy(int kindNum, int maxCnt)
    {
        for (int i = 0; i < maxCnt; i++)
        {
            if (buyData[(ItemStoreMng.KIND)kindNum][i].obj.transform.parent != buyParent.transform)
            {
                // 親位置を変える
                buyData[(ItemStoreMng.KIND)kindNum][i].obj.transform.SetParent(buyParent.transform);
            }
            buyData[(ItemStoreMng.KIND)kindNum][i].obj.name = buyData[(ItemStoreMng.KIND)kindNum][i].name + i;
            buyData[(ItemStoreMng.KIND)kindNum][i].nameText.text = buyData[(ItemStoreMng.KIND)kindNum][i].name;

            // 表示する料金を買い値に変更
            buyData[(ItemStoreMng.KIND)kindNum][i].priceText.text = buyData[(ItemStoreMng.KIND)kindNum][i].price.ToString();
            buyData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(true);
        }
    }

    public void InactiveBuy(int kindNum, int maxCnt)
    {
        // 非アクティブにしたいとき
        for (int i = 0; i < maxCnt; i++)
        {
            buyData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(false);
        }
    }
}