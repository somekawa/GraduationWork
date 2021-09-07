using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopList : MonoBehaviour
{
    public enum NTest
    {
        CHAPTER,
        CHARA,
        MAX
    }

    private ChapterList GetChapterList(int chapterNum)
    {
        ChapterList chapterList = Resources.Load("Chapter" + chapterNum) as ChapterList;

        if(chapterList == null)
        {
            Debug.Log("PopMob.csのchapterList情報がnullです");
        }

        return chapterList;
    }

    public T GetData<T>(NTest nTest, int num)
    {
        if (nTest == NTest.CHAPTER)
        {
            //object(任意のTにキャストできる)にキャストし、そこから別の型にキャストする必要がある
            return (T)(object)GetChapterList(num);
        }
        return default(T);
    }
}
