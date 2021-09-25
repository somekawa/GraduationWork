using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInteriorMng : MonoBehaviour
{
    private CameraMng cameraMng_;
    private UnitychanController playerController_;

    private GameObject inHouseInfoCanvas_;
    private bool inHouseFlg_ = true;        // �������邩(true:���� , false:����Ȃ�)

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("HouseInteriorMng.cs�Ŏ擾���Ă���CameraMng��null�ł�");
        }

        playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (playerController_ == null)
        {
            Debug.Log("HouseInteriorMng.cs�Ŏ擾���Ă���playerController_��null�ł�");
        }

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
    }

    public bool SetHouseVisible(string name)
    {
        if (!inHouseFlg_)
        {
            // �����������ɕK�����̊֐��͌Ă΂�邪�A
            // inHouseFlg_��false�Ȃ�K�v�ȏ���������return����
            SetActiveCanvas(false, "");

            // �L����������ĊJ����
            playerController_.enabled = true;
            return false;
        }

        // ��v�����I�u�W�F�N�g�ȊO���A�N�e�B�u
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if (child.name != name)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }

        return true;
    }

    // �����m�F�p�L�����o�X�̕\��/��\����؂�ւ���
    public void SetActiveCanvas(bool flag, string name)
    {
        inHouseInfoCanvas_.SetActive(flag);

        if (name == "")
        {
            return;
        }

        // �R���[�`���X�^�[�g  
        StartCoroutine(SelectInHouse(flag,name));
    }

    // �R���[�`��  
    private IEnumerator SelectInHouse(bool flag,string name)
    {
        // �L�����A�j���[�V�������~�߂�
        playerController_.StopUniRunAnim();

        // �L����������~�߂�
        playerController_.enabled = false;

        // �L�����o�X���\�����̊Ԃ̓R���[�`���������s��
        while (flag)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                inHouseFlg_ = false;
                Debug.Log("�I�����u�������v");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                inHouseFlg_ = true;
                Debug.Log("�I�����u�͂��v");
            }
            else
            {
                // �����������s��Ȃ�
            }

            // �X�y�[�X�L�[�őI���������肵�Aflag��false�ɂ��邱�Ƃ�while�����甲����悤�ɂ���
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("�I�����̌���");
                flag = false;
            }
        }

        Debug.Log("�R���[�`���̏I��");

        // �Ƃ̃A�N�e�B�u/��A�N�e�B�u�̐ؑ�
        if (SetHouseVisible(name))
        {
            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
            cameraMng_.SetChangeCamera(true);
        }
    }
}
