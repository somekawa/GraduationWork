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
    //        bagMateria_.MateriaGetCheck(5, "��̃}�e���A", 5);
    //        bagMateria_.MateriaGetCheck(0, "�|�̋k", 5);
    //        bagMateria_.MateriaGetCheck(6, "���炩�Ȑ�	", 5);


    //        //bagWord_.WordGetCheck(0, "�P��", PopMateriaList.WORD.HEAD);
    //        //bagWord_.WordGetCheck(1, "��", PopMateriaList.WORD.ELEMENT);
    //        //bagWord_.WordGetCheck(2, "��", PopMateriaList.WORD.TAIL);
    //        //bagWord_.WordGetCheck(3, "HP", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(4, "����", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(19, "�G", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(15, "�U����", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(17, "�㏸", PopMateriaList.WORD.SUB);
    //        //  Debug.Log("�w��̂��̂��擾���܂���");
    //    }
    //}
}