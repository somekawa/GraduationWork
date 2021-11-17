using UnityEngine;
using UnityEngine.EventSystems;

public class Event_WordClick : MonoBehaviour
{
    private MagicCreate wordCheck_;    // どのボタンをクリックしたか代入する変数
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    public void OnClickWordBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            wordCheck_ = GameObject.Find("MagicCreateMng").GetComponent<MagicCreate>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        wordCheck_.SetWord(clickbtn_.name);// ボタンの名前を代入
    } 
}