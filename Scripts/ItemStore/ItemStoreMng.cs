using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    [SerializeField]
    private GameObject itemStoreUI;    // 魔道具店用のUI

    public enum KIND
    {
        NON = -1,
        MATERIA,
        ITEM,
        MAX,
    }
    private Button[] kindBtn_=new Button[(int)KIND.MAX];// 0　素材　1アイテム
    private KIND selectKind_ = KIND.NON;

    public enum STORE
    {
        NON = -1,
        BUY,       // 買う選択中
        SELL,      // 売る選択中
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// アイテム売り買い用のキャンバス

    private int saveItemsNum_;  // 選択されたアイテムの番号を保存
   // private string saveName_;   // 選択されたアイテムの名前を保存
    private int savePrice_;     // 選択されたアイテムの料金を保存
    private int kindNum_ = 0;

    private RectTransform processParent_;// 売買したいものを表示するものの親
    private RectTransform cntParent_;
    private TMPro.TextMeshProUGUI itemInfo_;// アイテム説明欄
    private Image selectItemIcon_;// 選択したアイテムの画像
    private Image exSymbol_;        // Exアイテム用シンボル

    // 買う数、売る数関連
    private RectTransform shoppingParent_;// 売買情報
    private TMPro.TextMeshProUGUI shoppingText_;// 買うか売るか
    // ----アイテム個数関連
    private TMPro.TextMeshProUGUI itemCountText_;// 売買したい個数を表示
    private TMPro.TextMeshProUGUI maxCountText_;// 売買できる最大個数を表示
    private TMPro.TextMeshProUGUI minCountText_;// 売買できる最小個数を表示
    private int itemCnt_ = 1;
    private int minBuyCnt_ = 1;// 買う時の最小値
    private int minSellCnt_ = 0;// 売るときの最小値
    private int maxCnt_ = 99;// 最大個数


    private Button[] cntBtn_ = new Button[2];// 0左(countDown)　1右(countUp)
    private Slider slider_;// 売買するアイテムの個数変更用スライダー
    private bool pressFlag_ = false;

    // ----料金関連
    private TMPro.TextMeshProUGUI priceText_;
    private int totalPrice_ = 0;

    // 所持金
    private TMPro.TextMeshProUGUI haveMoneyText_;
    private int haveMoney_ = 1001;// デバッグ用

    // 所持数
    private TMPro.TextMeshProUGUI haveCntText_;
    private int haveCnt_ = 0;

    // 0買うボタン　1売るボタン　2外に出るボタン
  //  private Button[] itemStoreBtn_ = new Button[3];
    private Canvas itemStoreCanvas_;

    private Bag_Materia bagMateria_;// 素材選択時使用
    private Bag_Item bagItem_;// アイテム選択時使用

    private InitPopList popItemList_;
    private int itemMaxCnt_ = 0;
    void Start()
    {
        itemStoreCanvas_ = GameObject.Find("ItemStoreCanvas").GetComponent<Canvas>();
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        itemMaxCnt_ = popItemList_.SetMaxItemCount();

        var gameObject = DontDestroyMng.Instance;
        bagMateria_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();
        bagItem_ = gameObject.transform.Find("Managers").GetComponent<Bag_Item>();
        // 売買用の親
        tradeMng_[(int)STORE.BUY] = itemStoreUI.transform.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = itemStoreUI.transform.Find("SellMng").GetComponent<RectTransform>();
        // 素材とアイテムどちらを選択しているか
        kindBtn_[(int)KIND.MATERIA] = itemStoreUI.transform.Find("MateriaButton").GetComponent<Button>();
        kindBtn_[(int)KIND.ITEM] = itemStoreUI.transform.Find("ItemButton").GetComponent<Button>();
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;

        // 選択中のものを表示するエリア
        processParent_ = itemStoreUI.transform.Find("CheckArea").GetComponent<RectTransform>();
        // アイテムの説明欄
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<TMPro.TextMeshProUGUI>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();
        exSymbol_ = processParent_.transform.Find("InfoArea/ExSymbol").GetComponent<Image>();
        exSymbol_.gameObject.SetActive(false);
        // 所持金
        var myselfPaent = processParent_.Find("MyselfData").GetComponent<RectTransform>();
        haveMoney_ = SceneMng.GetHaveMoney();
        haveMoneyText_ = myselfPaent.Find("Money/Count").GetComponent<TMPro.TextMeshProUGUI>();
        haveMoneyText_.text = haveMoney_.ToString() ;
        // 選択しているアイテムの所持数
        haveCntText_ = myselfPaent.transform.Find("HaveItem/Count").GetComponent<TMPro.TextMeshProUGUI>();
        haveCntText_.text = haveCnt_.ToString()+"コ";

        shoppingParent_ = processParent_.Find("ShoppingData").GetComponent<RectTransform>();
        shoppingText_= shoppingParent_.Find("ShoppingButton/Text").GetComponent<TMPro.TextMeshProUGUI>();
        itemCountText_ = shoppingParent_.Find("TotalCount").GetComponent<TMPro.TextMeshProUGUI>();
        priceText_ = shoppingParent_.Find("PricePleate/TotalPrice").GetComponent<TMPro.TextMeshProUGUI>();
        itemCountText_.text = itemCnt_.ToString() + "コ";
        priceText_.text = totalPrice_.ToString();

        cntParent_ = shoppingParent_.Find("CountMng").GetComponent<RectTransform>();
        maxCountText_ = cntParent_.Find("Max/Count").GetComponent<TMPro.TextMeshProUGUI>();
        minCountText_ = cntParent_.Find("Min/Count").GetComponent<TMPro.TextMeshProUGUI>(); 
        // アイテム増減関連
        cntBtn_[0] = cntParent_.Find("CountDown").GetComponent<Button>();
        cntBtn_[1] = cntParent_.Find("CountUp").GetComponent<Button>();
        cntBtn_[0].interactable = false;
        cntBtn_[1].interactable = true;
        slider_ = cntParent_.Find("Slider").GetComponent<Slider>();
        slider_.maxValue = maxCnt_;// スライダーの最大値を設定


        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        itemStoreUI.transform.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        SceneMng.SetSE(0);
        CommonClickActive(STORE.BUY);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        shoppingText_.text = "購入";
        transform.GetComponent<Trade_Buy>().Init((int)KIND.MATERIA, (int)KIND.ITEM);
    }

    public void OnClickSellBtn()
    {
        SceneMng.SetSE(0);
        CommonClickActive(STORE.SELL);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        shoppingText_.text = "売却";
        transform.GetComponent<Trade_Sell>().Init((int)KIND.MATERIA, (int)KIND.ITEM);
    }

    private void CommonClickActive(STORE store)
    {
        storeActive_ = store;
        selectKind_ = KIND.MATERIA;
        itemStoreUI.transform.gameObject.SetActive(true);
        kindBtn_[(int)KIND.MATERIA].interactable = false;// 素材を表示
        kindBtn_[(int)KIND.ITEM].interactable = true;// アイテムを非表示
        itemStoreCanvas_.gameObject.SetActive(false);
    }

    public void SetSelectItemName(int num)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            // ボタンやスライダーを表示する
            processParent_.gameObject.SetActive(true);
        }
        // 呼び出し物が違うため1つ1つif文でチェックする
        // 素材とアイテムどちらを選択しているか（0:素材、1:アイテム
        itemInfo_.text = selectKind_ == KIND.ITEM ?  InitPopList.itemData[num].info: InitPopList.materiaData[num].info;
        // 指定アイテムの所持数
        haveCnt_ = selectKind_ == KIND.ITEM ? Bag_Item.itemState[num].haveCnt : Bag_Materia.materiaState[num].haveCnt;
        haveCntText_.text = haveCnt_.ToString() + "コ";
        // 指定アイテムの画像
        var imageKinds = selectKind_ == KIND.ITEM ? ItemImageMng.IMAGE.ITEM: ItemImageMng.IMAGE.MATERIA;
        int imageNum = num;
        if (exSymbol_.gameObject.activeSelf == true)
        {
            // 基本的にfalse
            exSymbol_.gameObject.SetActive(false);
        }
        if (imageKinds == ItemImageMng.IMAGE.ITEM)
        {
            // Exアイテムが押されたらtrueにする
            if(itemMaxCnt_ /2<= num)
            {
                imageNum -= itemMaxCnt_/2;
                exSymbol_.gameObject.SetActive(true);
            }
        }
        Debug.Log("番号：" + num + "    説明：" + itemInfo_.text);
        selectItemIcon_.sprite = ItemImageMng.spriteMap[imageKinds][imageNum];

        saveItemsNum_ = num;    // 番号を保存
       // saveName_ = name;       // アイテムを保存

        // 表示させておく
        shoppingParent_.gameObject.SetActive(true);

        if (storeActive_ == STORE.BUY)
        {
            savePrice_ = selectKind_ == KIND.ITEM ? InitPopList.itemData[num].buyPrice:InitPopList.materiaData[num].buyPrice ;
            Debug.Log("選択したアイテムの値段" + savePrice_);
            if (SceneMng.GetHaveMoney() < savePrice_ || maxCnt_ <= haveCnt_)
            {               

                // 最大まで所持してたり所持金が足りなかったら購入できない
                shoppingParent_.gameObject.SetActive(false);
                return;
            }

            int maxDiviCnt_ = SceneMng.GetHaveMoney() / savePrice_;// 所持金から見た最大値
            int maxSubCnt_ = maxCnt_ - haveCnt_;// 所持数から見た最大値
                                                // 小さい値を優先してmaxValueに代入する
            slider_.maxValue = maxSubCnt_ < maxDiviCnt_ ? maxSubCnt_ : maxDiviCnt_;
            slider_.minValue = minBuyCnt_;// 最小1
        }
        else if (storeActive_ == STORE.SELL)
        {
            if ( haveCnt_<=minSellCnt_)
            {
                // 所持していなかったら表示しない
                shoppingParent_.gameObject.SetActive(false);
                return;
            }
            savePrice_ = selectKind_ == KIND.ITEM ?  InitPopList.itemData[num].sellPrice: InitPopList.materiaData[num].sellPrice;
            slider_.maxValue = haveCnt_;
            slider_.minValue = minSellCnt_;
        }
        else
        {
            // 何もしない
        }
        Debug.Log("番号"+ saveItemsNum_ + "     totalPrice_" + totalPrice_ + "        savePrice_   " + savePrice_);
        totalPrice_ = savePrice_;
        maxCountText_.text = slider_.maxValue.ToString();
        minCountText_.text = slider_.minValue.ToString();
        if (slider_.minValue == slider_.maxValue)
        {
            // 最少と最大が同じ値なら個数変更系を非表示に
            cntParent_.gameObject.SetActive(false);
        }
        else
        {
            cntParent_.gameObject.SetActive(true);
            cntBtn_[1].interactable = true;
            slider_.value = slider_.minValue;
        }
        cntBtn_[0].interactable = false;// 最小の値だから押下できないようにする
        // 選択個数は最小値に
        itemCountText_.text = slider_.minValue.ToString() + "コ";
        // 指定アイテムの料金
        priceText_.text = totalPrice_.ToString() ;
    }

    public void OnClickCountDown()
    {
        itemCnt_--;
        if (itemCnt_ <= slider_.minValue)
        {
            cntBtn_[0].interactable = false;
        }
        Debug.Log("選択数" + itemCnt_ + "        ：最小値" + slider_.minValue);
        // 右矢印が押下できないなら押下可能状態に
        cntBtn_[1].interactable =  itemCnt_<slider_.maxValue  ? true : false;
        CommonUpDown();
        slider_.value = itemCnt_;
    }

    public void OnClickCountUp()
    {
        itemCnt_++;
        if (storeActive_ == STORE.BUY)                // 買う場合
        {
            // アイテムを買うとき
            // トータルの料金が所持金+選択中のアイテムの1つの料金
            if (slider_.maxValue <= itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }
        else
        {
            // アイテムを売るとき
            if (haveCnt_ <= itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }

        // 左矢印が押下できないなら押下可能状態に
        cntBtn_[0].interactable = slider_.minValue < itemCnt_ ? true : false;

        CommonUpDown();
        slider_.value = itemCnt_;
    }

    private void CommonUpDown()
    {
        totalPrice_ = savePrice_ * itemCnt_;

        itemCountText_.text = itemCnt_.ToString() + "コ";
        // 売買するアイテムの料金
        priceText_.text = totalPrice_.ToString();
        pressFlag_ = true;
    }

    public void OnNowValueCheck()
    {
        if (pressFlag_ == true)
        {
            // ボタン押下でのスライダー移動の場合処理に入らないようにする
            pressFlag_ = false;
            return;
        }

        // 売買する値を表示する
     //   Debug.Log("選択中の個数" + itemCnt_.ToString());
        itemCnt_ = (int)slider_.value;// スライダーの位置がアイテムの個数
        totalPrice_ = savePrice_ * itemCnt_;
        itemCountText_.text = itemCnt_.ToString() + "コ";
        // 売買するアイテムの料金
        priceText_.text = totalPrice_.ToString();

        // スライダーで増減時にボタンの状態を変える
        if (slider_.minValue < slider_.value && slider_.value < slider_.maxValue)
        {
            cntBtn_[0].interactable = true;
            cntBtn_[1].interactable = true;
        }
        else if (slider_.maxValue <= itemCnt_)
        {
            cntBtn_[1].interactable = false;
            cntBtn_[0].interactable = true;
        }
        else if (itemCnt_ <= slider_.minValue)
        {
            cntBtn_[0].interactable = false;
            cntBtn_[1].interactable = true;
        }
        else
        {
            // 何もしない
        }
    }


    public void OnClickShopping()
    {
        SceneMng.SetSE(0);

        // 購入ボタン押下
       // Debug.Log(saveName_ + "を購入しました");

        // 所持数を加算する

        if (storeActive_ == STORE.BUY)
        {
            if (selectKind_ == KIND.MATERIA)
            {
                bagMateria_.MateriaGetCheck(saveItemsNum_,  itemCnt_);
            }
            else
            {
                bagItem_.ItemGetCheck(0, saveItemsNum_, itemCnt_);
            }
            // お金の減少処理
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - totalPrice_);
           // haveMoney_ -= totalPrice_;  // 所持金を減算する
            haveCnt_ += itemCnt_;     // 所持数を増やす
        }
        else if (storeActive_ == STORE.SELL)
        {
            if (selectKind_ == KIND.MATERIA)
            {
                bagMateria_.MateriaGetCheck(saveItemsNum_,  -itemCnt_);
            }
            else
            {
                bagItem_.ItemGetCheck(0, saveItemsNum_, -itemCnt_);
            }
            // お金の加算処理
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() + totalPrice_);
            //haveMoney_ += totalPrice_;  // 所持金を加算する
            haveCnt_ -= itemCnt_;     // 所持数を減らす
            Debug.Log("アイテム所持数" + haveCnt_);
            if (haveCnt_ < 1)
            {
                transform.GetComponent<Trade_Sell>().SetHaveCntCheck(kindNum_,saveItemsNum_);
                processParent_.gameObject.SetActive(false);
            }
        }
        else
        {
            // 何もしない
        }

        // 表示する所持金の値を変える
        haveMoneyText_.text = SceneMng.GetHaveMoney().ToString();
        haveCntText_.text = haveCnt_.ToString() + "コ";// 表示する所持数を変更
        slider_.value = 0;
        // 初期化を兼ねて呼び出す
        SetSelectItemName(saveItemsNum_);
        //SetSelectItemName(saveItemsNum_, saveName_);
    }

    public void OnClickCancelBtn()
    {
        // 買い物を終わらせたいとき
        storeActive_ = STORE.NON;
        selectKind_ = KIND.NON;
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;
        itemStoreUI.transform.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        //itemStoreBtn_[0].gameObject.SetActive(true);
        //itemStoreBtn_[1].gameObject.SetActive(true);
        //itemStoreBtn_[2].gameObject.SetActive(true);
        itemStoreCanvas_.gameObject.SetActive(true);
    }




    public void OnClickItemButton()
    {
        selectKind_ = KIND.ITEM;
        if (storeActive_ == STORE.BUY)
        {
            transform.GetComponent<Trade_Buy>().Init((int)selectKind_, (int)KIND.MATERIA);
        }
        else
        {
            transform.GetComponent<Trade_Sell>().Init((int)selectKind_, (int)KIND.MATERIA);
        }
        processParent_.gameObject.SetActive(false);
        kindBtn_[(int)KIND.MATERIA].interactable = true;
        kindBtn_[(int)KIND.ITEM].interactable = false;
    }

    public void OnClickMateriaButton()
    {
        selectKind_ = KIND.MATERIA;
        if (storeActive_ == STORE.BUY)
        {
            transform.GetComponent<Trade_Buy>().Init((int)selectKind_, (int)KIND.ITEM);
        }
        else
        {
             transform.GetComponent<Trade_Sell>().Init((int)selectKind_, (int)KIND.ITEM);
        }
        processParent_.gameObject.SetActive(false);
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;
    }

    public void OnClickBackButton()
    {
        Debug.Log("戻るボタンを押下しました");
        itemStoreCanvas_.gameObject.SetActive(true);
        itemStoreUI.SetActive(false);

    }
}