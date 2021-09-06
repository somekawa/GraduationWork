using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopList : MonoBehaviour
{
    //[SerializeField] int chapterNum;

    //void Start()
    //{
    //    MobList mobList = Resources.Load("Chapter" + chapterNum) as MobList;

    //    Debug.Log("総数 = " + mobList.param.Count);
    //    Debug.Log("0番目 name=" + mobList.param[0].name);
    //    Debug.Log("1番目 name=" + mobList.param[1].name);

    //}

    public ChapterList GetMobList(int chapterNum)
    {
        ChapterList chapterList = Resources.Load("Chapter" + chapterNum) as ChapterList;

        if(chapterList == null)
        {
            Debug.Log("PopMob.csのchapterList情報がnullです");
        }

        return chapterList;
    }


}
