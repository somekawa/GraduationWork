using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Event_ItemStore : MonoBehaviour
{
    private ItemStoreMng materiaCheck_;    // どのボタンをクリックしたか代入する変数

    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    //void Start()
    //{
    //    eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    //    materiaCheck_ = GameObject.Find("HouseInterior/ItemStore").GetComponent<ItemStoreMng>();
    //}

    public void OnClickSelectItemBtn()
    {
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            materiaCheck_ = GameObject.Find("HouseInterior/ItemStore").GetComponent<ItemStoreMng>();
        }
        clickbtn_ = eventSystem.currentSelectedGameObject;
        materiaCheck_.SetSelectItemName(clickbtn_.name);
    }
}
