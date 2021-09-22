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
            Debug.Log("InHouseCheck.cs�Ŏ擾���Ă���CameraMng��null�ł�");
        }

        interiorMng_ = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
        if (interiorMng_ == null)
        {
            Debug.Log("InHouseCheck.cs�Ŏ擾���Ă���HouseInteriorMng��null�ł�");
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        // Uni�ƉƂ̓����蔻��
        if(collision.transform.tag == "Player")
        {
            // �Ƃ̃A�N�e�B�u/��A�N�e�B�u�̐ؑ�
            interiorMng_.SetHouseVisible(this.gameObject.name);

            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.0f, 0.0f));
            cameraMng_.SetChangeCamera(true);
            Debug.Log(this.gameObject.name + "��Player���ڐG");
        }
    }
}
