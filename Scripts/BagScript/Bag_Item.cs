using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Item : MonoBehaviour
{
    //[SerializeField]
    //// クエストを受注したときに生成されるプレハブ
    //private GameObject itemUIPrefab;


    private Image itemBox_;// 

    // private MenuActive menuActive_;
    private Image[] instanceImages_ = new Image[30];
    private Text[] instanceTexts_ = new Text[30];
    private int instanceNum_ = 0;

    void Start()
    {
        //menuActive_ = menuActive;

        //  itemBox_ = transform.Find("ItemBox").GetComponent<Image>();
        gameObject.SetActive(true);

       // StartCoroutine(ActiveItem(menuActive));

    }

    public IEnumerator ActiveItem(MenuActive menuActive)
    {
        gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            if (menuActive.GetStringNumber() != (int)MenuActive.topic.ITEM)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (menuActive_.StringNumber() == (int)MenuActive.topic.ITEM)
        //{
        //    gameObject.SetActive(true);
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}

    }
}
