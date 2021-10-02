using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopList : MonoBehaviour
{
    public enum ListData
    {
        CHAPTER,
        CHARACTER,
        MAX
    }

    private T GetList<T>(string dataName)
    {
        T characterList = (T)(object)Resources.Load(dataName);

        if (characterList == null)
        {
            Debug.Log("PopMob.csのcharacterList情報がnullです");
        }

        return characterList;
    }

    public T GetData<T>(ListData data, int num = 0, string str = "")
    {
        if (data == ListData.CHAPTER)
        {
            string tmpStr = "Chapter/Chapter" + num;
            //object(任意のTにキャストできる)にキャストし、そこから別の型にキャストする必要がある
            return (T)(object)GetList<ChapterList>(tmpStr);
        }
        else if(data == ListData.CHARACTER)
        {
            string tmpStr = "Chara" + str;
            return (T)(object)GetList<CharacterList>(tmpStr);
        }
        return default(T);
    }
}
