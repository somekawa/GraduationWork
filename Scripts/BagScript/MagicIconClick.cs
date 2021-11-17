using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private Bag_Magic bagMagic_;
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Text info_; // クリックした魔法を説明する欄
    private Image magicIcon_;

    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        Debug.Log("ボタンの番号"+number);
        info_.text = Bag_Magic.data[number].name+"  "+ Bag_Magic.data[number].power;

        //wordCheck_.SetWord(clickbtn_.name);// ボタンの名前を代入
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            magicIcon_ = GameObject.Find("StatusMng/MagicSet0").GetComponent<Image>();
            bagMagic_ = GameObject.Find("Managers").GetComponent<Bag_Magic>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ボタン名から数字のみを取り出す
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        magicIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][Bag_Magic.elementNum_[number]];
        bagMagic_.SetMagicCheck(number,true);
    }

}
