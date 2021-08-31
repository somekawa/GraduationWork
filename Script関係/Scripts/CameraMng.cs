using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : MonoBehaviour
{
    private GameObject mainCamera;      //メインカメラ格納用
    private GameObject buttleCamera;       //サブカメラ格納用 

    void Start()
    {
        //メインカメラとサブカメラをそれぞれ取得
        mainCamera = GameObject.Find("MainCamera");
        buttleCamera = GameObject.Find("ButtleCamera");

        //サブカメラを非アクティブにする
        buttleCamera.SetActive(false);
    }

    void Update()
    {
        //スペースキーが押されている間、サブカメラをアクティブにする
        if (Input.GetKey(KeyCode.A))
        {
            //サブカメラをアクティブに設定
            mainCamera.SetActive(false);
            buttleCamera.SetActive(true);
        }
        else
        {
            //メインカメラをアクティブに設定
            buttleCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
}
