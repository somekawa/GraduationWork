using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Event_BookBuy : MonoBehaviour
{
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Toggle bookToggle_;

    public void OnClickBookSelectBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        bookToggle_ = clickbtn_.transform.Find("Toggle").GetComponent<Toggle>();
        bookToggle_.isOn = true;
        Debug.Log(clickbtn_.name + "がクリックされました");
    }
}
