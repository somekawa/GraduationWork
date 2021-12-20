using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Event_WordClick : MonoBehaviour
{
    private MagicCreate wordCheck_;    // どのボタンをクリックしたか代入する変数
    private Bag_Word bagWord_;
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Text info_; // クリックしたアイテムを説明する欄

    public void OnClickWordBtn()
    {
        if (SceneMng.nowScene != SceneMng.SCENE.UNIHOUSE)
        {
            if (eventSystem_ == null)
            {
                eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                bagWord_ = GameObject.Find("Managers").GetComponent<Bag_Word>(); 
                info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
                info_.text = "";
            }
            clickbtn_ = eventSystem_.currentSelectedGameObject;
            int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
            Debug.Log("説明欄に表示するワード："+Bag_Word.data[number].name);
            info_.text = Bag_Word.data[number].name; //  number.ToString();
            // bagWord_.wordState[,number].name + "\n" + Bag_Materia.materiaState[number].info;
            //bagWord_.SetMateriaNumber(number);// どのボタンを押したか保存する
        }
        else
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
}