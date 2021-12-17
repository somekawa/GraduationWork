using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Event_BookBuy : MonoBehaviour
{
    private BookStoreMng bookStoreMng;    // どのボタンをクリックしたか代入する変数
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    public void OnClickBookSelectBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            bookStoreMng = GameObject.Find("BookStoreMng").GetComponent<BookStoreMng>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // 本の名前の番号を取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        bookStoreMng.SelectBookCheck(number);

      //  Debug.Log(clickbtn_.name + "がクリックされました");
    }
}
