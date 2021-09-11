using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�������ʉ߂��������m�F���āACameraMng.cs�ɃJ�����؂�ւ��̎w�����o��

public class GoThroughCheck : MonoBehaviour
{
    private CameraMng cameraMng_;
    private GameObject player_;

    private Vector3 EnterPos_;  // �����蔻����ɓ������u�Ԃ̍��W
    private Vector3 ExitPos_;   // �����蔻������o���u�Ԃ̍��W

    void Start()
    {
        //unitychan�̏����擾
        player_ = GameObject.Find("Uni");
        if(player_ == null)
        {
            Debug.Log("GoThroughCheck.cs�Ŏ擾���Ă���Player���null�ł�");
        }

        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if(cameraMng_ == null)
        {
            Debug.Log("GoThroughCheck.cs�Ŏ擾���Ă���CameraMng��null�ł�");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player")) //col.tag == "Player"�Ə������A����������
        {
            Debug.Log("�J�����ؑ�");

            // ����L�����N�^�[���A��ʂɉf��Ȃ���ԂŃJ�����؂�ւ�����������̂�h��
            if(player_.transform.position.z <= 94.0f)
            {
                player_.transform.position = new Vector3(player_.transform.position.x, player_.transform.position.y, 95.0f);
            }

            EnterPos_ = player_.transform.position;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player")) //col.tag == "Player"�Ə������A����������
        {
            ExitPos_ = player_.transform.position;

            if (this.gameObject.name == "GoThroughRight")
            {
                // �E�ʘH�̎�(�M���h�Ɩ��������)
                if ((ExitPos_ - EnterPos_).normalized.x >= 0.0f)
                {
                    // 1.0�̎��͉E�ւ̒ʉ߂̈�true(�T�u�J�����A�N�e�B�u)
                    cameraMng_.SetChangeCamera(true);
                    // �J�����ʒu����
                    cameraMng_.SetSubCameraPos(new Vector3(24.0f, 3.0f, 89.0f));
                }
                else
                {
                    // -1.0�̎��͍��ւ̒ʉ߂̈�false(���C���J�����A�N�e�B�u)
                    cameraMng_.SetChangeCamera(false);
                }
            }
            else
            {
                // ���ʘH�̎�(�Z��X)
                if ((ExitPos_ - EnterPos_).normalized.x >= 0.0f)
                {
                    // 1.0�̎��͍��ւ̒ʉ߂̈�false(���C���J�����A�N�e�B�u)
                    cameraMng_.SetChangeCamera(false);
                }
                else
                {
                    // -1.0�̎��͉E�ւ̒ʉ߂̈�true(�T�u�J�����A�N�e�B�u)
                    cameraMng_.SetChangeCamera(true);
                    // �J�����ʒu����
                    cameraMng_.SetSubCameraPos(new Vector3(-24.0f, 3.0f, 89.0f));
                }
            }
        }
    }
}
