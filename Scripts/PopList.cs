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
            Debug.Log("PopMob.cs��chapterList���null�ł�");
        }

        return chapterList;
    }

    public T GetData<T>(NTest nTest, int num)
    {
        if (nTest == NTest.CHAPTER)
        {
            //object(�C�ӂ�T�ɃL���X�g�ł���)�ɃL���X�g���A��������ʂ̌^�ɃL���X�g����K�v������
            return (T)(object)GetChapterList(num);
        }
        return default(T);
    }
}
