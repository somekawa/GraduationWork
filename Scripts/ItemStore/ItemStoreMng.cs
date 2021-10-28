using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    public enum STORE
    { NON=-1,
    BUY,
    SELL,
    MAX
    }
    private STORE storeActive_ = STORE.NON;

    private Canvas tradeCanvas_;// アイテム売り買い用のキャンバス
    private Canvas selectBtnParent_;

    // プレハブからの生成関連
    //[SerializeField]
    //private GameObject merchandisePrefab;    // 素材を拾ったときに生成されるプレハブ
    //private static GameObject[,] merchandise_ = new GameObject[5, 30];
    //private static Text[,] merchandiseName_ = new Text[5, 30];

    //private Text buyCnt_;
    private MateriaList materiaList_;
    private int nowFieldNum_ = 1;// どのフィールドまで進んでいるか。

    //private string merchandiseString = "";

    //private Trade_Buy buy_;
    //private Trade_Sell sell_;

    private RectTransform processParent_;
    private Text itemExplanation_;
    private Image selectItemIcon_;
    private string itemName_ = "";
    void Start()
    {
        // 買う処理なのか売る処理なのか
        //buy_ = this.transform.GetComponent<Trade_Buy>();
        //sell_ =this.transform.GetComponent<Trade_Sell>();
        //buy_.enabled = false;
        //sell_.enabled = false;


        tradeCanvas_ = GameObject.Find("TradeCanvas").GetComponent<Canvas>();
        processParent_ = tradeCanvas_.transform.Find("CheckArea").GetComponent<RectTransform>();


        itemExplanation_ = processParent_.transform.Find("ExplanationArea/ExplanationText").GetComponent<Text>();
        itemExplanation_.text = "";
        selectItemIcon_ = processParent_.transform.Find("ExplanationArea/ItemImage").GetComponent<Image>();
        selectItemIcon_.sprite = ItemImageMng.materialIcon_[1, 0];
        // processParent_.gameObject.SetActive(false);

        selectBtnParent_ = GameObject.Find("HouseInterior/InHouseCanvas").GetComponent<Canvas>();
        //for (int f=0; f <= nowFieldNum_; f++)
        //{
        //    materiaList_ = Resources.Load("MaterialList/Field" + f) as MaterialList;
        //    for (int m = 0; m < materiaList_.param.Count; m++)
        //    {
        //        merchandise_[f, m] = Instantiate(merchandisePrefab,
        //        new Vector2(0, 0), Quaternion.identity,
        //        tradeCanvas_.transform.Find("BuyMng/Viewport/Content").transform);
        //        merchandiseName_[f, m] = merchandise_[f, m].transform.Find("Name").GetComponent<Text>();
        //        merchandiseName_[f, m].text = materiaList_.param[m].MateriaName;
        //        merchandise_[f, m].name = materiaList_.param[m].MateriaName;
        //    }
        //}
        tradeCanvas_.gameObject.SetActive(false);
    }

    //public void SelectItemEx()
    //{
    //    for (int f = 0; f <= nowFieldNum_; f++)
    //    {
    //        for (int m = 0; m < materiaList_.param.Count; m++)
    //        {
    //            if (itemName_ == Trade_Buy.merchandiseName_[f, m].name)
    //            {
    //                selectItemIcon_.sprite = ItemImageMng.materialIcon_[f, m];
    //            }

    //        }
    //    }
    //}

            // Update is called once per frame
            void Update()
    {

        //// 選択された行動のScriptを有効にする
        //if()

        //{


        //}

    }

    //public void SetActiveWantItem(int page, int recipeNum, string name)
    //{
    //    // 表示したいアイテムがあるシート番号（ページ）
    //    materiaList_ = Resources.Load("MaterialList/Field" + page) as MaterialList;

    //   // createName_.text = materiaList_.param[recipeNum].ItemName;
    //    //wantMateria_.text = "必要素材\n" +
    //    //    recipeList_.param[recipeNum].WantMateria1 +
    //    //    " " + materiaList_.param[recipeNum].WantMateria2 +
    //    //    " " + recipeList_.param[recipeNum].WantMateria3;
    //}

    public void OnClickBuyBtn()
    {
        storeActive_ = STORE.BUY;
        Debug.Log(tradeCanvas_.name+"           "+ selectBtnParent_.name);
        tradeCanvas_.gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
       // transform.GetComponent<Trade_Buy>().Init();
    }

    public void OnClickSellBtn()
    {
        storeActive_ = STORE.SELL;
        tradeCanvas_.gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
       // transform.GetComponent<Trade_Sell>().Init();
    }

    public void SetSelectItemName(string name)
    {
        for (int f = 0; f <= nowFieldNum_; f++)
        {
            materiaList_ = Resources.Load("MaterialList/Field" + f) as MateriaList;
            for (int m = 0; m < materiaList_.param.Count; m++)
            {
                if (name == materiaList_.param[m].MateriaName)
                {
                    Debug.Log(f+"       "+m);
                    selectItemIcon_.sprite = ItemImageMng.materialIcon_[f,m];
                }
            }
        }
    }

    //public string GetSelectItemName()
    //{
    //    return itemName_;
    //}

    public MateriaList GetMateriaList()
    {
        return materiaList_;
    }

    public int FieldNumber()
    {
        return nowFieldNum_;
    }

}
