using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayorHouse : MonoBehaviour
{
    public bool CheckEventMayorHouse()
    {
        // �C�x���g����
        if (EventMng.GetChapterNum() == 0)
        {
            EventMng.SetChapterNum(1, (int)SceneMng.SCENE.CONVERSATION);
            return true;
        }
        return false;
    }
}
