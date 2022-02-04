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
    private Bag_Magic bagMagic_;

    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            bagMagic_ = GameObject.Find("Managers").GetComponent<Bag_Magic>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;

        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        bagMagic_.SetMagicNumber(number);
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
        }      
        // このCSがついているボタンの名前
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        Button btn = clickbtn_.GetComponent<Button>();
        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        rectItemBagMng_.GetComponent<ItemBagMng>().InfoCheck(btn, number);
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
        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(-1, false);
    }
}