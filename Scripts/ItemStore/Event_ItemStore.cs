using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Event_ItemStore : MonoBehaviour
{
    private ItemStoreMng itemStoreMng_;    // どのボタンをクリックしたか代入する変数
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンか

    public void OnClickSelectItemBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            itemStoreMng_ = GameObject.Find("ItemStoreMng").GetComponent<ItemStoreMng>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        Debug.Log("    名前：" + clickbtn_.name);
        // 押下したアイテムの番号を取得
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        // 数字を取り除いた名前を取得
        string name = clickbtn_.name.Replace(number.ToString(), "");
        Debug.Log("番号：" + number + "    名前：" + name);
        itemStoreMng_.SetSelectItemName(number);
    }
}
