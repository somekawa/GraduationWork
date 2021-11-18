using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private RectTransform rectItemBagMng_;
    private MenuActive menuActive_;
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Text info_; // クリックした魔法を説明する欄
    private Image magicIcon_;

    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        Debug.Log("ボタンの番号"+number);
        info_.text = Bag_Magic.data[number].name+"  "+ Bag_Magic.data[number].power;

        //wordCheck_.SetWord(clickbtn_.name);// ボタンの名前を代入
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
        }
        // どの魔法ボタンに画像をセットするか
        int getNum = rectItemBagMng_.GetComponent<ItemBagMng>().GetClickButtonNum();
        magicIcon_ = GameObject.Find("StatusMng/MagicSet" + getNum+"/Icon").GetComponent<Image>();

        // このCSがついているボタンの名前
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        magicIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][Bag_Magic.elementNum_[number]];
        magicIcon_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //itemBagMng_.SetMagicCheck(number,true);
        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(number);
    }
}
