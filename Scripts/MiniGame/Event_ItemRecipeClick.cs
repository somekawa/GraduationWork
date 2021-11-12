using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// インスタンスされるボタン1つ1つにアタッチされてるScript
public class Event_ItemRecipeClick : MonoBehaviour
{
    private ItemCreateMng itemCreateMng_;

    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数


    public void OnClickItemRecipeBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            itemCreateMng_ = GameObject.Find("AlchemyMng").GetComponent<ItemCreateMng>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        Button btn_ = clickbtn_.GetComponent<Button>();

        // クリックしたボタンを選択状態にしている
        btn_.interactable = false;
        itemCreateMng_.SetButtonName(clickbtn_.name);
    }
}