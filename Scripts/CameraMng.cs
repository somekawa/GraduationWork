using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 街の中の定点カメラや戦闘時のバトルカメラはサブカメラとして管理している
// メインカメラは、街でもフィールドでもキャラに後ろから追従するようにしている

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // メインカメラ格納用
    public GameObject subCamera;       // フィールドならバトルカメラ格納,街なら定点カメラ格納

    void Start()
    {
        //サブカメラを非アクティブにする
        subCamera.SetActive(false);
    }

    // 街のサブカメラ位置を変更するときに呼ばれる
    public void SetSubCameraPos(Vector3 pos)
    {
        subCamera.transform.position = pos;
    }

    // 外部からカメラ状態の切替を行えるようにする
    public void SetChangeCamera(bool flag)
    {
        mainCamera.SetActive(!flag);
        subCamera.SetActive(flag);
    }
}
