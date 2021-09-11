using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �X�̒��̒�_�J������퓬���̃o�g���J�����̓T�u�J�����Ƃ��ĊǗ����Ă���
// ���C���J�����́A�X�ł��t�B�[���h�ł��L�����Ɍ�납��Ǐ]����悤�ɂ��Ă���

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // ���C���J�����i�[�p
    public GameObject subCamera;       // �t�B�[���h�Ȃ�o�g���J�����i�[,�X�Ȃ��_�J�����i�[

    void Start()
    {
        //�T�u�J�������A�N�e�B�u�ɂ���
        subCamera.SetActive(false);
    }

    // �X�̃T�u�J�����ʒu��ύX����Ƃ��ɌĂ΂��
    public void SetSubCameraPos(Vector3 pos)
    {
        subCamera.transform.position = pos;
    }

    // �O������J������Ԃ̐ؑւ��s����悤�ɂ���
    public void SetChangeCamera(bool flag)
    {
        mainCamera.SetActive(!flag);
        subCamera.SetActive(flag);
    }
}
