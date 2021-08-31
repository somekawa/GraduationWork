using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMng : MonoBehaviour
{
    private GameObject mainCamera;      //���C���J�����i�[�p
    private GameObject buttleCamera;       //�T�u�J�����i�[�p 

    void Start()
    {
        //���C���J�����ƃT�u�J���������ꂼ��擾
        mainCamera = GameObject.Find("MainCamera");
        buttleCamera = GameObject.Find("ButtleCamera");

        //�T�u�J�������A�N�e�B�u�ɂ���
        buttleCamera.SetActive(false);
    }

    void Update()
    {
        //�X�y�[�X�L�[��������Ă���ԁA�T�u�J�������A�N�e�B�u�ɂ���
        if (Input.GetKey(KeyCode.A))
        {
            //�T�u�J�������A�N�e�B�u�ɐݒ�
            mainCamera.SetActive(false);
            buttleCamera.SetActive(true);
        }
        else
        {
            //���C���J�������A�N�e�B�u�ɐݒ�
            buttleCamera.SetActive(false);
            mainCamera.SetActive(true);
        }
    }
}
