using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InHouseCheck : MonoBehaviour
{
    private CameraMng cameraMng_;
    private HouseInteriorMng interiorMng_;

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("InHouseCheck.csで取得しているCameraMngがnullです");
        }

        interiorMng_ = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
        if (interiorMng_ == null)
        {
            Debug.Log("InHouseCheck.csで取得しているHouseInteriorMngがnullです");
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        // Uniと家の当たり判定
        if(collision.transform.tag == "Player")
        {
            // 家のアクティブ/非アクティブの切替
            interiorMng_.SetHouseVisible(this.gameObject.name);

            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.0f, 0.0f));
            cameraMng_.SetChangeCamera(true);
            Debug.Log(this.gameObject.name + "とPlayerが接触");
        }
    }
}
