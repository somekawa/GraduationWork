using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MayorHouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // �C�x���g����
        if(EventMng.GetChapterNum() == 0)
        {
            EventMng.SetChapterNum(1,(int)SceneMng.SCENE.CONVERSATION);
        }
    }
}
