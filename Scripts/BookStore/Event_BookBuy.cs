using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Event_BookBuy : MonoBehaviour
{
    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Toggle bookToggle_;
    private PopBookList popBookList_;

    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        popBookList_ = GameObject.Find("Mng").GetComponent<PopBookList>();
    }

    public void OnClickBookSelectBtn()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        Debug.Log(clickbtn_.name + "がクリックされました");
        bookToggle_ = clickbtn_.transform.Find("Toggle").GetComponent<Toggle>();
        for (int i = 0; i < popBookList_.GetBookList().param.Count - 1; i++)
        {
            if (clickbtn_.name == popBookList_.GetBookList().param[i].BookName)
            {
                if (bookToggle_.isOn == true)
                {
                    bookToggle_.isOn = false;
                }
                else
                {
                    bookToggle_.isOn = true;
                }
            }
        }
    }

}
