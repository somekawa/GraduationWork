using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    private Bag_Materia bagMateria_;

    public enum STORE
    {
        NON = -1,
        BUY,       // 買う選択中
        SELL,      // 売る選択中
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private RectTransform tradeParentCanvas_;// アイテム売り買い用のキャンバス
    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// アイテム売り買い用のキャンバス
                                                                          // private Canvas selectBtnParent_;

    private int saveItemsNum_;  // 選択されたアイテムの番号を保存
    private string saveName_;   // 選択されたアイテムの名前を保存
    private int savePrice_;     // 選択されたアイテムの料金を保存

    private RectTransform processParent_;// 売買したいものを表示するものの親
    private Text itemInfo_;// アイテム説明欄
    private Image selectItemIcon_;// 選択したアイテムの画像

    // 買う数、売る数関連
    private RectTransform countParent_;
    // ----アイテム個数関連
    private Text itemCountText_;
    private Text maxCountText_;// 売買できる最大個数
    private int itemCnt_ = 1;
    private int oldItemCnt_ = 1;
    private int minCnt_ = 1;
    private int maxCnt_ = 99;


    private Button[] cntBtn_ = new Button[2];// 0左(countDown)　1右(countUp)
    private Slider slider_;// 売買するアイテムの個数変更用スライダー
    private bool pressFlag_ = false;

    // ----料金関連
    private Text priceText_;
    private int totalPrice_ = 0;

    // 所持金
    private Text haveMoneyText_;
    private int haveMoney_ = 1001;// デバッグ用

    // 持っているアイテムの数
    private Text haveMateriaText_;
    private int haveCnt_ = 0;

    // ItemShoppingMng以外の表示物
    private Image storeName_;// 魔道具店と表示するTextの背景
    private Vector3 startStoreNamePos_;// 入店時の座標
    private Vector3 newStoreNamePos_ = new Vector3(0.0f, 320.0f, 0.0f);// 売買選択時の移動先
    private Button[] itemStoreBtn_ = new Button[3];

    void Start()
    {


        storeName_ = GameObject.Find("HouseInfo").GetComponent<Image>();
        startStoreNamePos_ = storeName_.gameObject.transform.localPosition;
        itemStoreBtn_[0] = GameObject.Find("BuyButton").GetComponent<Button>();
        itemStoreBtn_[1] = GameObject.Find("SellButton").GetComponent<Button>();
        itemStoreBtn_[2] = GameObject.Find("ExitButton").GetComponent<Button>();

        var gameObject = DontDestroyMng.Instance;
        bagMateria_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();

        // 売買用の親
        tradeParentCanvas_ = GameObject.Find("ItemShoppingMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.BUY] = tradeParentCanvas_.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = tradeParentCanvas_.Find("SellMng").GetComponent<RectTransform>();

        // 選択中のものを表示するエリア
        processParent_ = tradeParentCanvas_.Find("CheckArea").GetComponent<RectTransform>();
        RectTransform countParent = processParent_.Find("CountMng").GetComponent<RectTransform>();

        itemCountText_ = countParent.Find("TotalCount").GetComponent<Text>();
        itemCountText_.text = itemCnt_.ToString();
        oldItemCnt_ = itemCnt_;
        maxCountText_ = countParent.Find("Max/Count").GetComponent<Text>();
        // アイテム増減関連
        cntBtn_[0] = countParent.Find("CountDown").GetComponent<Button>();
        cntBtn_[1] = countParent.Find("CountUp").GetComponent<Button>();
        slider_ = countParent.Find("Slider").GetComponent<Slider>();
        slider_.maxValue = maxCnt_;

        priceText_ = countParent.Find("TotalPrice").GetComponent<Text>();
        priceText_.text = totalPrice_.ToString() + "ビット";

        // 所持金
        haveMoneyText_ = processParent_.Find("Money/Count").GetComponent<Text>();
        haveMoneyText_.text = haveMoney_.ToString() + "ビット";

        // 選択しているアイテムの所持数
        haveMateriaText_ = processParent_.transform.Find("HaveItem/Count").GetComponent<Text>();
        haveMateriaText_.text = haveCnt_.ToString();

        // アイテムの説明欄
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<Text>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();


        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        tradeParentCanvas_.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        storeActive_ = STORE.BUY;
        // Debug.Log(tradeParentCanvas_.name + "           " + selectBtnParent_.name);
        tradeParentCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        itemStoreBtn_[0].gameObject.SetActive(false);
        itemStoreBtn_[1].gameObject.SetActive(false);
        itemStoreBtn_[2].gameObject.SetActive(false);
        //  selectBtnParent_.gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = newStoreNamePos_;
        transform.GetComponent<Trade_Buy>().SetActiveBuy();
    }

    public void OnClickSellBtn()
    {
        storeActive_ = STORE.SELL;
        tradeParentCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        //   selectBtnParent_.gameObject.SetActive(false);
        itemStoreBtn_[0].gameObject.SetActive(false);
        itemStoreBtn_[1].gameObject.SetActive(false);
        itemStoreBtn_[2].gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = newStoreNamePos_;
        transform.GetComponent<Trade_Sell>().SetActiveSell();
    }

    public void SetSelectItemName(string name)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            processParent_.gameObject.SetActive(true);
        }
        cntBtn_[0].interactable = false;// 最小の値だから押下できないようにする

        for (int i = 0; i < bagMateria_.GetMaxHaveMateriaCnt(); i++)
        {
            // nameがm番目のMateriaNameと同じなら
            if (name == InitPopList.materiaData[i].name)
            {
                // 説明欄
                itemInfo_.text = InitPopList.materiaData[i].info;

                // 画像
                selectItemIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];
                saveItemsNum_ = i;
                saveName_ = name;
                Debug.Log(saveName_ + "を選びました");

                // 指定アイテムの所持数
                haveCnt_ = Bag_Materia.materiaState[saveItemsNum_].haveCnt;
                haveMateriaText_.text = haveCnt_.ToString();

                if (storeActive_ == STORE.BUY)
                {
                    savePrice_ = InitPopList.materiaData[i].buyPrice;
                    int maxDiviCnt_ = haveMoney_ / savePrice_;// 所持金から見た最大値
                    int maxSubCnt_ = maxCnt_ - Bag_Item.itemState[i].haveCnt;// 所持数から見た最大値
                    // 小さい値を優先してmaxValueに代入する
                    slider_.maxValue = maxSubCnt_ < maxDiviCnt_ ? maxSubCnt_ : maxDiviCnt_;
                }
                else if (storeActive_ == STORE.SELL)
                {
                    savePrice_ = InitPopList.materiaData[i].sellPrice;
                    slider_.maxValue = Bag_Item.itemState[i].haveCnt;
                }
                else
                {
                    // 何もしない
                }

                maxCountText_.text = slider_.maxValue.ToString();
                
                // 指定アイテムの料金
                priceText_.text = savePrice_.ToString() + "ビット";
                totalPrice_ = savePrice_;

                return;
            }
        }
    }

    public void OnClickCountDown()
    {
        if (storeActive_ == STORE.BUY)
        {
            //// 買う場合
            if (itemCnt_ <= minCnt_)
            {
                //cntBtn_[0].interactable = false;
                itemCnt_ = minCnt_;
            }
            else
            {
                itemCnt_--;
            }
        }
        else
        {
            if (Bag_Item.itemState[saveItemsNum_].haveCnt <= minCnt_)
            {
                cntBtn_[0].interactable = false;
                itemCnt_ = minCnt_;
            }
            else
            {
                itemCnt_--;
            }
        }
        if (cntBtn_[1].interactable == false)
        {
            // 右矢印が押下できないなら押下可能状態に
            cntBtn_[1].interactable = true;
        }
        CommonUpDown();
        slider_.value = itemCnt_;
        // totalPrice_ -= savePrice_;
        // OnNowValueCheck(-1);
    }

    public void OnClickCountUp()
    {
        itemCnt_++;
        if (storeActive_ == STORE.BUY)                // 買う場合
        {
            // アイテムを買うとき
            // トータルの料金が所持金+選択中のアイテムの1つの料金
            if (totalPrice_ <  haveMoney_ -  savePrice_)
              //  || itemCnt_ <= maxCnt_ - Bag_Item.itemState[saveItemsNum_].haveCnt)
            {
                cntBtn_[1].interactable = false;
            }
        }
        else
        {
            // アイテムを売るとき
            if (Bag_Item.itemState[saveItemsNum_].haveCnt < itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }
        if (cntBtn_[0].interactable == false)
        {
            // 左矢印が押下できないなら押下可能状態に
            cntBtn_[0].interactable = true;
        }
        CommonUpDown();
        slider_.value = itemCnt_;
        //totalPrice_ += savePrice_;
        // totalPrice_ += savePrice_;
        // OnNowValueCheck(1);
    }

    public void OnNowValueCheck()
    {
        if (pressFlag_ == true)
        {
            pressFlag_ = false;
            return;
        }

        // ボタンでスライダーも増減するためここが共通項になる
        // 売買する値を表示する
        Debug.Log("選択中の個数"+itemCnt_.ToString());
        totalPrice_ = savePrice_ * itemCnt_;
        itemCountText_.text = itemCnt_.ToString();
        // 売買するアイテムの料金
        priceText_.text = totalPrice_ + "ビット";

        // スライダーで増減時にボタンの状態を変える
        if (itemCnt_== slider_.maxValue)
        {
            cntBtn_[1].interactable = false;
            cntBtn_[0].interactable = true;
        }
        else if(itemCnt_==minCnt_)
        {
            cntBtn_[0].interactable = false;
            cntBtn_[1].interactable = true;
        }
        else
        {
            // 何もしない
        }

        oldItemCnt_ = itemCnt_;
        if (itemCnt_ < 1)
        {
            cntBtn_[0].interactable = false;
            itemCnt_ = 1;
        }
        if (storeActive_ == STORE.BUY)                // 買う場合
        {           
            // トータルの料金が所持金+選択中のアイテムの1つの料金
            if (haveMoney_ - savePrice_< totalPrice_)
            //  || itemCnt_ <= maxCnt_ - Bag_Item.itemState[saveItemsNum_].haveCnt)
            {
                cntBtn_[1].interactable = false;
                // if文に入る前に加算した分を消す
                itemCnt_--;
            }
        }
        //number = 0;
    }

    private void CommonUpDown()
    {
        totalPrice_ = savePrice_ * itemCnt_;

        itemCountText_.text = itemCnt_.ToString();
        // 売買するアイテムの料金
        priceText_.text = totalPrice_ + "ビット";
        pressFlag_ = true;

    }



    public void OnClickShopping()
    {
        // 購入ボタン押下
        //if (itemCount_ < 1)
        //{
        //    return;
        //}
        Debug.Log(saveName_ + "を購入しました");

        // 所持数を加算する

        if (storeActive_ == STORE.BUY)
        {
            //bagMateria_.MateriaGetCheck(saveItemsNum_, saveName_, itemCount_);
            //haveMoney_ -= totalPrice_;  // 所持金を減算する
            //haveCnt_ += itemCount_;     // 所持数を増やす
        }
        else if (storeActive_ == STORE.SELL)
        {
            //bagMateria_.MateriaGetCheck(saveItemsNum_, saveName_, -itemCount_);
            //haveMoney_ += totalPrice_;  // 所持金を加算する
            //haveCnt_ -= itemCount_;     // 所持数を減らす
            if (haveCnt_ < 1)
            {
                transform.GetComponent<Trade_Sell>().SetHaveCntCheck(saveItemsNum_);
            }
        }

        // 表示する所持金の値を変える
        haveMoneyText_.text = haveMoney_.ToString() + "ビット";
        haveMateriaText_.text = haveCnt_.ToString();// 表示する所持数を変更

        // 値を初期化
        totalPrice_ = 0;
        itemCnt_ = 1;
        itemCountText_.text = itemCnt_.ToString();
        priceText_.text = totalPrice_ + "ビット";
    }

    public void OnClickCancelBtn()
    {
        // 買い物を終わらせたいとき
        storeActive_ = STORE.NON;
        tradeParentCanvas_.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = startStoreNamePos_;
        itemStoreBtn_[0].gameObject.SetActive(true);
        itemStoreBtn_[1].gameObject.SetActive(true);
        itemStoreBtn_[2].gameObject.SetActive(true);
        // selectBtnParent_.gameObject.SetActive(true);
    }
}