using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private RectTransform itembagMng_;

    private Bag_Materia bagMateria_;
    private Bag_Word bagWord_;
    private bool chapterCheck_ = false;

    
    
    void Awake()
    {
       // itembagMng_ = GameObject.Find("ItemBagMng").GetComponent<RectTransform>();
        bagMateria_ = gameObject.transform.GetComponent<Bag_Materia>();
        bagWord_ = gameObject.transform.GetComponent<Bag_Word>();
        //gameObject.GetComponent<Bag_Materia>().Init();
        //itembagMng_.gameObject.SetActive(false);
    }

    //void Update()
    //{
    //    if (chapterCheck_ == false)
    //    {
    //        chapterCheck_ = true;
    //        bagMateria_.MateriaGetCheck(5, "空のマテリア", 5);
    //        bagMateria_.MateriaGetCheck(0, "酢の橘", 5);
    //        bagMateria_.MateriaGetCheck(6, "清らかな水	", 5);


    //        //bagWord_.WordGetCheck(0, "単体", PopMateriaList.WORD.HEAD);
    //        //bagWord_.WordGetCheck(1, "炎", PopMateriaList.WORD.ELEMENT);
    //        //bagWord_.WordGetCheck(2, "小", PopMateriaList.WORD.TAIL);
    //        //bagWord_.WordGetCheck(3, "HP", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(4, "味方", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(19, "敵", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(15, "攻撃力", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(17, "上昇", PopMateriaList.WORD.SUB);
    //        //  Debug.Log("指定のものを取得しました");
    //    }
    //}
}