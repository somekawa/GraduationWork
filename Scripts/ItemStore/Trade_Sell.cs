using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Sell : MonoBehaviour
{
    private InitPopList popItemsList_;

    [SerializeField]
    private RectTransform sellParent;  // 表示位置の親

    public struct StoreSell
    {
        public GameObject obj;
        public Button btn;
        public TMPro.TextMeshProUGUI priceText;        // 表示する値段
        public TMPro.TextMeshProUGUI nameText;         // 表示する素材の名前
        public int price;
        public int haveCnt;
        public string name;
    }
    public static Dictionary<ItemStoreMng.KIND, StoreSell[]> sellData = new Dictionary<ItemStoreMng.KIND, StoreSell[]>();
    private int[] maxCnt_;            // すべての素材数

    public void Init(int seleKind, int kind)
    // void Start()
    {

        if (popItemsList_ == null)
        {
            popItemsList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

            maxCnt_ = new int[(int)ItemStoreMng.KIND.MAX];
            maxCnt_[(int)ItemStoreMng.KIND.MATERIA] = popItemsList_.SetMaxMateriaCount();
            maxCnt_[(int)ItemStoreMng.KIND.ITEM] = popItemsList_.SetMaxItemCount();
            sellData[ItemStoreMng.KIND.ITEM] = InitSellData(ItemStoreMng.KIND.ITEM, maxCnt_[(int)ItemStoreMng.KIND.ITEM]);
            sellData[ItemStoreMng.KIND.MATERIA] = InitSellData(ItemStoreMng.KIND.MATERIA, maxCnt_[(int)ItemStoreMng.KIND.MATERIA]);

        }
        SetActiveSell(seleKind, maxCnt_[seleKind]);// 表示する
        InactiveSell(kind, maxCnt_[kind]);
    }

    private StoreSell[] InitSellData(ItemStoreMng.KIND kind, int maxCnt)
    {
        var data = new StoreSell[maxCnt];

        for (int i = 0; i < maxCnt; i++)
        {
            data[i].haveCnt = kind == ItemStoreMng.KIND.ITEM ? Bag_Item.itemState[i].haveCnt : Bag_Materia.materiaState[i].haveCnt;

            data[i].obj = kind == ItemStoreMng.KIND.ITEM ? PopListInTown.itemPleate[i] : PopListInTown.materiaPleate[i];
            data[i].name = kind == ItemStoreMng.KIND.ITEM ? Bag_Item.itemState[i].name : Bag_Materia.materiaState[i].name;
            data[i].obj.name = data[i].name + i;
            data[i].btn = data[i].obj.GetComponent<Button>();
            data[i].price = kind == ItemStoreMng.KIND.ITEM ? InitPopList.itemData[i].sellPrice : InitPopList.materiaData[i].sellPrice;

            if (kind == ItemStoreMng.KIND.ITEM && ((maxCnt - 1) / 2 < i))
            {
                data[i].name = "Ex\n" + data[i - (maxCnt / 2)].name;
            }

            // 表示する名前を変更する
            data[i].nameText = data[i].obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            data[i].nameText.text = data[i].name+i;

            // 料金を表示するText
            data[i].priceText = data[i].obj.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>();
            data[i].priceText.text = data[i].price.ToString();

            data[i].obj.transform.SetParent(sellParent.transform);

            data[i].obj.SetActive(true);
        }
        return data;
    }


    public void SetActiveSell(int kindNum, int maxCnt)
    {
        for (int i = 0; i < maxCnt; i++)
        {
            if (sellData[(ItemStoreMng.KIND)kindNum][i].obj.transform.parent != sellParent.transform)
            {
                // 親位置を変える
                sellData[(ItemStoreMng.KIND)kindNum][i].obj.transform.SetParent(sellParent.transform);
            }
            sellData[(ItemStoreMng.KIND)kindNum][i].obj.name = sellData[(ItemStoreMng.KIND)kindNum][i].name + i;
            sellData[(ItemStoreMng.KIND)kindNum][i].nameText.text = sellData[(ItemStoreMng.KIND)kindNum][i].name;

            // 表示する料金を買い値に変更
            sellData[(ItemStoreMng.KIND)kindNum][i].priceText.text = sellData[(ItemStoreMng.KIND)kindNum][i].price.ToString();

            // すべて表示させておいて
            sellData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(true);

            if (kindNum == (int)ItemStoreMng.KIND.MATERIA)
            {
                if (Bag_Materia.materiaState[i].haveCnt < 1)
                {
                    // 持っていないアイテム、素材は非表示にする
                    sellData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(false);
                }
            }
            else
            {
                if (Bag_Item.itemState[i].haveCnt < 1)
                {
                    // 持っていないアイテム、素材は非表示にする
                    sellData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(false);
                }
            }
        }
    }

    public void InactiveSell(int kindNum, int maxCnt)
    {
        // 非アクティブにしたいとき
        for (int i = 0; i < maxCnt; i++)
        {
            sellData[(ItemStoreMng.KIND)kindNum][i].obj.SetActive(false);
        }
    }

    public void SetHaveCntCheck(int kindNum, int materiaNum)
    {
        // 指定素材の所持数が0個になったら呼び出される
        sellData[(ItemStoreMng.KIND)kindNum][materiaNum].obj.SetActive(false);
    }
}