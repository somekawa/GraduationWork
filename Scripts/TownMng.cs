using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 現在のシーンをTOWNとする
        SceneMng.SetNowScene(SceneMng.SCENE.TOWN);
    }
}
