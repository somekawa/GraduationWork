using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreCount : MonoBehaviour
{
    private RectTransform countParent_;

    // アイテム個数関連
    private Text countText_;
    private int itemCount_ = 0;

    // 料金関連
    private Text priceText_;
    private int totalPrice_ = 0;

    // Start is called before the first frame update
    void Start()
    {
        countParent_ = GameObject.Find("TradeCanvas/CheckArea/CountMng").GetComponent<RectTransform>();

        countText_ = countParent_.transform.Find("BuyCount").GetComponent<Text>();
        countText_.text = itemCount_.ToString();

        priceText_ = countParent_.transform.Find("TotalPrice").GetComponent<Text>();
        priceText_.text = totalPrice_.ToString();
    }

    public void OnClickCountUp()
    {
        StartCoroutine(ActiveCount(true));
    }

    public void OnClickCountDown()
    {
        StartCoroutine(ActiveCount(true));
    }

    public void OnPointerUp()
    {
        //itemCount_+=0;
        //totalPrice_+=0;
        StopCoroutine(ActiveCount(false));
        //countText_.text = itemCount_.ToString();
        //priceText_.text = totalPrice_.ToString();
        Debug.Log("カウントを止めます" + itemCount_);
    }

    //  private IEnumerator ActiveCount(bool activeFlag, bool upFlag)
    private IEnumerator ActiveCount(bool activeFlag)
    {
        while (activeFlag)
        {
            yield return null;
            //   Debug.Log("購入個数"+ itemCount_);
            countText_.text = itemCount_.ToString();
            priceText_.text = totalPrice_.ToString() + "ビット";
            //if (upFlag == true)
            //{
            //if (99 <= itemCount_)
            //{
            //    itemCount_ = 99;
            //    //  OnPointerUp();
            //    break;
            //}
            //else
            //{
                itemCount_++;
                totalPrice_++;
                //    }
                //}
                //else
                //{
                //    if (itemCount_ <=1)
                //    {
                //        itemCount_ = 1;
                //       // OnPointerUp();
                //        break;
                //    }
                //    else
                //    {
                //        itemCount_--;
                //        totalPrice_--;
                //    }
                //}
            }
        }

    
}
