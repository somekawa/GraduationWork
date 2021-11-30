using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private RectTransform rectItemBagMng_;
    private MenuActive menuActive_;
    private Text info_; // クリックした魔法を説明する欄
    private Image magicIcon_;
    private Bag_Magic bagMagic_;

    private RectTransform magicSetMng_;
    private int number_ = -1;
    private Image infoBack_;
    private Text magicName_;

    private int getSetBtnNumber_=0;
    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
            bagMagic_ = GameObject.Find("Managers").GetComponent<Bag_Magic>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        number_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        info_.text = Bag_Magic.data[number_].name + "  " + Bag_Magic.data[number_].power;
        bagMagic_.SetMagicNumber(number_);
        //wordCheck_.SetWord(clickbtn_.name);// ボタンの名前を代入
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
            magicSetMng_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
            infoBack_ = GameObject.Find("StatusMng/MagicCheck/Info").GetComponent<Image>();
            magicName_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
        }
        // このCSがついているボタンの名前
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        number_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        if (infoBack_.gameObject.activeSelf == true)
        {
            //// 魔法をセットするとき　どの魔法ボタンに画像をセットするか
            // getSetBtnNumber_ = rectItemBagMng_.GetComponent<ItemBagMng>().GetClickButtonNum();
            //magicIcon_ = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + getSetBtnNumber_ + "/Icon").GetComponent<Image>();

            // どの魔法を選んだか
            rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(number_,true);
            // 魔法の詳細を消す
            infoBack_.gameObject.SetActive(false);
            magicName_.text = "";
        }
        else
        {
            var pos = transform.localPosition;
            infoBack_.transform.localPosition = pos;
            Debug.Log("説明欄の座標" + pos.x + "  " + pos.y + "   " + pos.z);
            // 接触した魔法の詳細を出す
            infoBack_.gameObject.SetActive(true);


            if (Bag_Magic.data[number_].sub != "non")
            {
                magicName_.text = Bag_Magic.data[number_].main + "\n" + Bag_Magic.data[number_].sub;
            }
            else
            {
                magicName_.text = Bag_Magic.data[number_].main;
            }
        }
    }

    public void OnClickRemoveMagic()
    {
        // 外すボタンが押された場合
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
        }

        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(-1,false);
    }


    //public void OnMouseOver()
    //{
    //    if (magicSetMng_ == null)
    //    {
    //        magicSetMng_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
    //    }

    //    if (EventSystem.current.IsPointerOverGameObject()) return;
    //    Debug.Log(this.transform.gameObject.name + "にマウスオーバーしました");
    //    if (magicSetMng_.gameObject.activeSelf == true)
    //    {

    //        infoBack_ = GameObject.Find("StatusMng/MagicCheck/Info").GetComponent<Image>();
    //        magicName_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
    //        var pos = transform.localPosition;
    //        infoBack_.transform.localPosition = pos;
    //        Debug.Log("説明欄の座標"+pos.x+"  "+pos.y+"   "+pos.z);
    //        // 接触した魔法の詳細を出す
    //        infoBack_.gameObject.SetActive(true);


    //        if (Bag_Magic.data[number_].sub != "non")
    //        {
    //            magicName_.text = Bag_Magic.data[number_].main + "\n" + Bag_Magic.data[number_].sub;
    //        }
    //        else
    //        {
    //            magicName_.text = Bag_Magic.data[number_].main;
    //        }
    //    }
    //}

    //public void OnMouseExit()
    //{
    //    if (magicSetMng_.gameObject.activeSelf == true)
    //    {
    //        // 魔法の詳細を消す
    //        infoBack_.gameObject.SetActive(false);
    //        magicName_.text = "";
    //    }
    //}

}
