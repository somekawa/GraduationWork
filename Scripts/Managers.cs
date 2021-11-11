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
    //        bagMateria_.MateriaGetCheck(5, "‹ó‚Ìƒ}ƒeƒŠƒA", 5);
    //        bagMateria_.MateriaGetCheck(0, "|‚Ì‹k", 5);
    //        bagMateria_.MateriaGetCheck(6, "´‚ç‚©‚È…	", 5);


    //        //bagWord_.WordGetCheck(0, "’P‘Ì", PopMateriaList.WORD.HEAD);
    //        //bagWord_.WordGetCheck(1, "‰Š", PopMateriaList.WORD.ELEMENT);
    //        //bagWord_.WordGetCheck(2, "¬", PopMateriaList.WORD.TAIL);
    //        //bagWord_.WordGetCheck(3, "HP", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(4, "–¡•û", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(19, "“G", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(15, "UŒ‚—Í", PopMateriaList.WORD.SUB);
    //        //bagWord_.WordGetCheck(17, "ã¸", PopMateriaList.WORD.SUB);
    //        //  Debug.Log("w’è‚Ì‚à‚Ì‚ğæ“¾‚µ‚Ü‚µ‚½");
    //    }
    //}
}