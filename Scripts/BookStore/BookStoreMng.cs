using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookStoreMng : MonoBehaviour
{
    private PopListInTown popBookList_;
    private int maxCnt_ = 0;            // すべての素材数
    private int singleCnt_ = 0;         // 1つのシートに記載されてる最大個数

    [SerializeField]
    private RectTransform bookParent_;  // 表示位置の親

    // 買い物用のキャンバス
    //  private Canvas bookStoreCanvas_;
    // 買うか出るかのキャンバス
    // private Canvas selectBtnParent_;

    //  private RectTransform bookParent_;
    public struct book
    {
        public GameObject obj;  // 生成しておいたプレハブを代入
        public Button btn;      // オブジェクトの子のボタン
        public Text nameText;   // 表示する名前
        public Text priceText;  // 表示する料金
        public Toggle toggle;   // 選択中にチェックが入るチェックボックス
        public bool selectFlag; // 選択中はtrueにする
        public string name;     // 本の名前
        public string info;// 選択した本の説明
        public int price;       // 本の値段を保存
    }
    public static book[] bookState_;

    private Image soldOutImage_;// すべて買われた場合の画像
    private Button buyBtn_;// 購入するためのボタン

    // 合計料金
    private Text totalPriceText_;// 合計料金の表示
    private int totalPrice_ = 0;// 選択している本の合計金額
    private int selectBook_ = 0;// どのボタンを選んでいるか

    private Text infoText_;

    void Start()
    {
        // selectBtnParent_ = GameObject.Find("InHouseCanvas").GetComponent<Canvas>();
        //bookStoreCanvas_ = GameObject.Find("BookShopCanvas").GetComponent<Canvas>();
        //bookStoreCanvas_.gameObject.SetActive(false);
        // bookParent_ = GameObject.Find("BookStoreCanvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        //bookParent_ = GameObject.Find("BookShopCanvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        buyBtn_ = this.transform.Find("CheckArea/BuyButton").GetComponent<Button>();
        totalPriceText_ = this.transform.Find("CheckArea/TotalPrice").GetComponent<Text>();
        totalPriceText_.text = "0ビット";

        soldOutImage_ = this.transform.Find("ScrollView/Viewport/SoldOut").GetComponent<Image>();
        soldOutImage_.gameObject.SetActive(false);


        infoText_ = this.transform.Find("CheckArea/InfoArea/InfoText").GetComponent<Text>();
        infoText_.text = "";

        popBookList_ = GameObject.Find("TownMng").GetComponent<PopListInTown>();

        singleCnt_ = popBookList_.SetSingleBookCount();
        maxCnt_ = popBookList_.SetMaxBookCount();

        bookState_ = new book[maxCnt_];

        //  Debug.Log("本の個数" + bookList_.param.Count);
        for (int i = 0; i < maxCnt_; i++)
        {
            bookState_[i] = new book
            {
                obj = PopListInTown.bookObj_[i],
                name = PopListInTown.bookObj_[i].name,
                price = PopListInTown.bookPrice_[i],
                info = PopListInTown.bookInfo_[i],
            };
            // Debug.Log(bookState_[i].obj.name);

            // 事前に生成しておいたオブジェクトを代入する
            bookState_[i].btn = bookState_[i].obj.transform.GetComponent<Button>();
            bookState_[i].obj.name = bookState_[i].name;

            // チェックボックス
            bookState_[i].toggle = bookState_[i].btn.transform.Find("Toggle").GetComponent<Toggle>();

            // 表示する名前
            bookState_[i].nameText = bookState_[i].btn.transform.Find("Name").GetComponent<Text>();
            bookState_[i].nameText.text = bookState_[i].name;

            // 表示する料金
            bookState_[i].priceText = bookState_[i].btn.transform.Find("Price").GetComponent<Text>();
            bookState_[i].priceText.text = bookState_[i].price.ToString() + "ビット";

            // 親の位置を変える
            bookState_[i].btn.transform.SetParent(bookParent_.transform);

            // すべて非表示
            bookState_[i].btn.gameObject.SetActive(false);
            bookState_[i].selectFlag = false;
        }
        StartCoroutine(BookBuyCheck());
    }

    private IEnumerator BookBuyCheck()
    //void Update()
    {
        while (true)
        {
            yield return null;

            for (int i = 0; i < maxCnt_; i++)
            {
                // non以外は表示する
                if (bookState_[i].btn.name == "non")
                {
                    bookState_[i].btn.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    bookState_[i].btn.gameObject.SetActive(true);
                }

                if (bookState_[i].toggle.isOn == true)
                {
                    bookState_[i].btn.interactable = false;
                    selectBook_ = i;
                    infoText_.text = bookState_[selectBook_].info;
                    buyBtn_.interactable = true;
                }
                else
                {
                    bookState_[i].btn.interactable = true;
                }
            }



            // 現在選択中で選択されたことがない場合は合計金額を加算する
            if ((bookState_[selectBook_].btn.interactable == false)
                && (bookState_[selectBook_].selectFlag == false))
            {
                totalPrice_ += bookState_[selectBook_].price;
                totalPriceText_.text = totalPrice_ + "ビット";
                bookState_[selectBook_].selectFlag = true;
            }
            if (bookParent_.transform.childCount <= 0)
            {
                buyBtn_.interactable = false;
                soldOutImage_.gameObject.SetActive(true);
            }

            //if(singleCnt_ <= buyCnt_)
            //{
            //    buyBtn_.interactable = false;
            //    soldOutImage_.gameObject.SetActive(true);
            //}
        }
    }

    //public BookList GetBookList()
    //{
    //    return bookList_;
    //}

    public void OnClickBuyBtn()
    {
        for (int i = 0; i < maxCnt_; i++)
        {
            if (bookState_[selectBook_].btn.gameObject.activeSelf == true)
            {
                if (bookState_[selectBook_].toggle.isOn == true)
                {
                    bookState_[selectBook_].btn.gameObject.SetActive(false);
                }
            }
        }
    }

    //public void OnClickShoppingBtn()
    //{
    //    //   Debug.Log(tradeCanvas_.name + "           " + selectBtnParent_.name);
    //    // bookStoreCanvas_.gameObject.SetActive(true);
    //    //  selectBtnParent_.gameObject.SetActive(false);
    //    // gameObject.SetActive()
    //    Debug.Log("OnClickShoppingBtn");
    //    StartCoroutine(BookBuyCheck());
    //}

    public void OnClickSelectCancelBtn()
    {
        //   Debug.Log(tradeCanvas_.name + "           " + selectBtnParent_.name);
        totalPrice_ = 0;
        //StopCoroutine(BookBuyCheck());
        //gameObject.SetActive(false);
        Debug.Log("OnClickShoppingCancelBtn");
        //  bookStoreCanvas_.gameObject.SetActive(false);
        //  selectBtnParent_.gameObject.SetActive(true);
        for (int i = 0; i < maxCnt_; i++)
        {
            if (bookState_[i].btn.gameObject.activeSelf == true)
            {
                if (bookState_[i].toggle.isOn == true)
                {
                    bookState_[i].toggle.isOn = false;
                    bookState_[i].btn.interactable = false;
                }
            }
        }
    }
}