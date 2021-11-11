using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Event_BookBuy : MonoBehaviour
{
    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Button button_;
    private Toggle bookToggle_;
  
    // private PopBookList popBookList_;

    //void Start()
    //{
    //    eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    //    //popBookList_ = GameObject.Find("Mng").GetComponent<PopBookList>();
    //}

    public void OnClickBookSelectBtn()
    {
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem.currentSelectedGameObject;
        //button_=this.gameObject.transform.GetComponent<Button>();
        //button_.interactable = false;
        bookToggle_ = clickbtn_.transform.Find("Toggle").GetComponent<Toggle>();
        bookToggle_.isOn = true;
        Debug.Log(clickbtn_.name + "がクリックされました");
    }
}
