using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFieldMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneMng.SetNowScene(SceneMng.SCENE.FIELD1);
        // WarpField.csの初期化関数を先に呼ぶ
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
