using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 街の中でも定点カメラを利用するので、このスクリプトを兼用できるようにする

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // メインカメラ格納用
    public GameObject subCamera;       // フィールドならバトルカメラ格納,街なら定点カメラ格納

    private bool changeFlg_ = false;   // カメラを切り替えるか判断する(false:MainCamera,true:SubCamera)

    void Start()
    {
        //サブカメラを非アクティブにする
        subCamera.SetActive(false);
    }

    void Update()
    {
        // 元の処理
        // FieldMngで遭遇タイミングを調整しているため、それを参照する
        //if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        //{
        //    //サブカメラをアクティブに設定
        //    mainCamera.SetActive(false);
        //    subCamera.SetActive(true);
        //}
        //else
        //{
        //    //メインカメラをアクティブに設定
        //    subCamera.SetActive(false);
        //    mainCamera.SetActive(true);
        //}

        if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        {
            //サブカメラをアクティブに設定
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        else
        {
            if(changeFlg_)
            {
                //サブカメラをアクティブに設定
                mainCamera.SetActive(false);
                subCamera.SetActive(true);
            }
            else
            {
                //メインカメラをアクティブに設定
                mainCamera.SetActive(true);
                subCamera.SetActive(false);
            }
        }
    }

    public void SetChangeFlg(bool flag)
    {
        changeFlg_ = flag;
    }

    public void SetSubCameraPos(Vector3 pos)
    {
        subCamera.transform.position = pos;
    }
}
