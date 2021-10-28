using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private RectTransform itembagMng_;

    private Bag_Materia bagMateria_;
    private bool chapterCheck_ = false;

    void Start()
    {
        itembagMng_ = GameObject.Find("ItemBagMng").GetComponent<RectTransform>();
        bagMateria_ = gameObject.transform.GetComponent<Bag_Materia>();
        gameObject.GetComponent<Bag_Materia>().Init();
       // itembagMng_.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (itembagMng_.gameObject.activeSelf == true)
        {
            if (5 < EventMng.GetChapterNum() && chapterCheck_ == false)
            {
                chapterCheck_ = true;
                bagMateria_.MateriaGetCheck(0, 5, "空のマテリア");
                Debug.Log("空のマテリアを取得しました");
            }
        }


    }

}
