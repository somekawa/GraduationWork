using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    private Bag_Materia materia_;

    public enum STORE
    {
        NON = -1,
        BUY,       // 買う選択中
        SELL,      // 売る選択中
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private Canvas tradeCanvas_;// アイテム売り買い用のキャンバス
    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// アイテム売り買い用のキャンバス
    private Canvas selectBtnParent_;

    private int saveItemsNum_;  // 選択されたアイテムの番号を保存
    private string saveName_;   // 選択されたアイテムの名前を保存
    private int savePrice_;     // 選択されたアイテムの料金を保存

    private RectTransform processParent_;// 売買したいものを表示するものの親
    private Text itemInfo_;// アイテム説明欄
    private Image selectItemIcon_;// 選択したアイテムの画像

    // 買う数、売る数関連
    private RectTransform countParent_;
    // ----アイテム個数関連
    private Text countText_;
    private int itemCount_ = 1;
    private int maxCnt_ = 99;

    // ----料金関連
    private Text priceText_;
    private int totalPrice_ = 0;

    // 所持金
    private Text haveMoneyText_;
    private int haveMoney_ = 9990;// デバッグ用

    // 持っているアイテムの数
    private Text haveMateriaText_;
    private int haveCnt_ = 0;
    void Start()
    {
        var gameObject = DontDestroyMng.Instance;
        materia_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();

        // 売買用の親
        tradeCanvas_ = GameObject.Find("ItemShoppingMng").GetComponent<Canvas>();
        tradeMng_[(int)STORE.BUY] = tradeCanvas_.transform.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = tradeCanvas_.transform.Find("SellMng").GetComponent<RectTransform>();

        // 選択中のものを表示するエリア
        processParent_ = tradeCanvas_.transform.Find("CheckArea").GetComponent<RectTransform>();

        // 所持金
        haveMoneyText_ = processParent_.transform.Find("Money/Count").GetComponent<Text>();
        haveMoneyText_.text = haveMoney_.ToString() + "ビット";

        // 選択しているアイテムの所持数
        haveMateriaText_ = processParent_.transform.Find("HaveItem/Count").GetComponent<Text>();
        haveMateriaText_.text = haveCnt_.ToString();

        // アイテムの説明欄
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<Text>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();

        selectBtnParent_ = GameObject.Find("InHouseCanvas").GetComponent<Canvas>();

        countParent_ = processParent_.transform.Find("CountMng").GetComponent<RectTransform>();
        countText_ = countParent_.transform.Find("BuyCount").GetComponent<Text>();
        countText_.text = itemCount_.ToString();
        priceText_ = countParent_.transform.Find("TotalPrice").GetComponent<Text>();
        priceText_.text = totalPrice_.ToString() + "ビット";

        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        tradeCanvas_.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        storeActive_ = STORE.BUY;
        Debug.Log(tradeCanvas_.name + "           " + selectBtnParent_.name);
        tradeCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
        transform.GetComponent<Trade_Buy>().SetActiveBuy();
    }

    public void OnClickSellBtn()
    {
        storeActive_ = STORE.SELL;
        tradeCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
        transform.GetComponent<Trade_Sell>().SetActiveSell();
    }

    public void SetSelectItemName(string name)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            processParent_.gameObject.SetActive(true);
        }

        // 個数を初期化
        itemCount_ = 1;
        countText_.text = itemCount_.ToString();

        for (int i = 0; i < materia_.GetMaxHaveMateriaCnt(); i++)
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
                }
                else if (storeActive_ == STORE.SELL)
                {
                    savePrice_ = InitPopList.materiaData[i].sellPrice;
                }
                else
                {
                    // 何もしない
                }

                // 指定アイテムの料金
                priceText_.text = savePrice_.ToString() + "ビット";
                totalPrice_ = savePrice_;

                return;
            }
        }
    }

    public void OnClickCountDown()
    {
        // 買いたい個数、売りたい個数を減らす
        if (itemCount_ <= 1)
        {
            // 1以下にならないようにする
            itemCount_ -= 0;
            totalPrice_ -= 0;
        }
        else
        {
            itemCount_--;
            totalPrice_ -= savePrice_;
        }

        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "ビット";
    }

    public void OnClickCountUp()
    {
        // 買いたい個数、売りたい個数を増やす
        if (storeActive_ == STORE.BUY)
        {
            if (haveMoney_-savePrice_ < totalPrice_)
            {
                itemCount_ += 0;
                totalPrice_ += 0;
            }
            else
            {
                CommonCountUp(maxCnt_);
            }
        }
        else if (storeActive_ == STORE.SELL)
        {
            // 指定の素材の所持数より多く売ろうとしていたら
            CommonCountUp(Bag_Materia.materiaState[saveItemsNum_].haveCnt);
        }
        else
        {
            // 何もしない
        }

        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "ビット";
    }

    private void CommonCountUp(int max)
    {
        if (max <= itemCount_)
        {
            // 所持数で加算を止める
            itemCount_ = max;
        }
        else
        {
            itemCount_++;
            totalPrice_ += savePrice_;
        }
    }

    public void OnClickShopping()
    {
        // 購入ボタン押下
        if (itemCount_ < 1)
        {
            return;
        }
        Debug.Log(saveName_ + "を購入しました");

        // 所持数を加算する

        if (storeActive_ == STORE.BUY)
        {
            materia_.MateriaGetCheck(saveItemsNum_, saveName_, itemCount_);
            haveMoney_ -= totalPrice_;  // 所持金を減算する
            haveCnt_ += itemCount_;     // 所持数を増やす
        }
        else if (storeActive_ == STORE.SELL)
        {
            materia_.MateriaGetCheck(saveItemsNum_, saveName_, -itemCount_);
            haveMoney_ += totalPrice_;  // 所持金を加算する
            haveCnt_ -= itemCount_;     // 所持数を減らす
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
        itemCount_ = 0;
        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "ビット";
    }

    public void OnClickCancelBtn()
    {
        // 買い物を終わらせたいとき
        storeActive_ = STORE.NON;
        tradeCanvas_.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        selectBtnParent_.gameObject.SetActive(true);
    }
}