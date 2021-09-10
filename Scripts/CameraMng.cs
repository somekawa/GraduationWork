using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �X�̒��ł���_�J�����𗘗p����̂ŁA���̃X�N���v�g�����p�ł���悤�ɂ���

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // ���C���J�����i�[�p
    public GameObject subCamera;       // �t�B�[���h�Ȃ�o�g���J�����i�[,�X�Ȃ��_�J�����i�[

    private bool changeFlg_ = false;   // �J������؂�ւ��邩���f����(false:MainCamera,true:SubCamera)

    void Start()
    {
        //�T�u�J�������A�N�e�B�u�ɂ���
        subCamera.SetActive(false);
    }

    void Update()
    {
        // ���̏���
        // FieldMng�ő����^�C�~���O�𒲐����Ă��邽�߁A������Q�Ƃ���
        //if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        //{
        //    //�T�u�J�������A�N�e�B�u�ɐݒ�
        //    mainCamera.SetActive(false);
        //    subCamera.SetActive(true);
        //}
        //else
        //{
        //    //���C���J�������A�N�e�B�u�ɐݒ�
        //    subCamera.SetActive(false);
        //    mainCamera.SetActive(true);
        //}

        if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        {
            //�T�u�J�������A�N�e�B�u�ɐݒ�
            mainCamera.SetActive(false);
            subCamera.SetActive(true);
        }
        else
        {
            if(changeFlg_)
            {
                //�T�u�J�������A�N�e�B�u�ɐݒ�
                mainCamera.SetActive(false);
                subCamera.SetActive(true);
            }
            else
            {
                //���C���J�������A�N�e�B�u�ɐݒ�
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
