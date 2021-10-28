using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopBookList : MonoBehaviour
{
    private BookList bookList_;
    private RectTransform bookParent_;
    private Button[] bookBack_;
    private Text[] bookText_;
    private Toggle[] bookToggle_;

    private Image soldOutImage_;
    private Button buyBtn_;
    private int buyCnt_ = 0;
    void Start()
    {
        bookList_ = Resources.Load("BookList/Story0") as BookList;
       // bookParent_ = GameObject.Find("BookStoreCanvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        bookParent_ = GameObject.Find("BookStoreCanvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        buyBtn_ = GameObject.Find("BookStoreCanvas/カゴマーク/Button").GetComponent<Button>();

        soldOutImage_ = GameObject.Find("BookStoreCanvas/ScrollView/Viewport/SoldOut").GetComponent<Image>();
        soldOutImage_.gameObject.SetActive(false);

        bookBack_ = new Button[bookList_.param.Count];
        bookText_ = new Text[bookList_.param.Count];
        bookToggle_ = new Toggle[bookList_.param.Count];

        Debug.Log("本の個数" + bookList_.param.Count);
        for (int i = 0; i < bookList_.param.Count; i++)
        {
            bookBack_[i] = bookParent_.transform.GetChild(i).GetComponent<Button>();
            bookBack_[i].name = bookList_.param[i].BookName;
            bookToggle_[i] = bookBack_[i].transform.Find("Toggle").GetComponent<Toggle>();
            bookText_[i] = bookBack_[i].transform.Find("Text").GetComponent<Text>();
            bookText_[i].text = bookList_.param[i].BookName;
            Debug.Log(i + "番目の本の名前" + bookList_.param[i].BookName);
            if (bookText_[i].text == "")
            {
                bookBack_[i].gameObject.SetActive(false);
                buyCnt_++;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < bookList_.param.Count; i++)
        {
            if (bookToggle_[i].isOn == true)
            {
                bookBack_[i].interactable = false;
            }
            else
            {
                bookBack_[i].interactable = true;
            }
        }

        if(GetBookList().param.Count<= buyCnt_)
        {
            buyBtn_.interactable = false;
            soldOutImage_.gameObject.SetActive(true);
        }
    }

    public BookList GetBookList()
    {
        return bookList_;
    }

    public void OnClickBuyBtn()
    {
        for (int i = 0; i < bookList_.param.Count; i++)
        {
            if (bookBack_[i].gameObject.activeSelf == true)
            {
                if (bookToggle_[i].isOn == true)
                {
                    bookBack_[i].gameObject.SetActive(false);
                    buyCnt_++;
                }
            }
        }
    }    
}