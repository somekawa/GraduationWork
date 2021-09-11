using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̊O�ł̉�ʊǗ�������B(�����ł�nowMode��NON�ɂ���)
// �T���E�퓬�E���j���[��ʂ̐؂�ւ�莞��enum�ŕύX���s��
// ����Mng�Ƃ����A���̃X�N���v�g�ɂ���nowMode_���Q�Ƃ��ĕύX���s��

// ���̃X�N���v�g�Ƃ͕ʂɁASceneMng�I�Ȃ̂�p�ӂ��āA�V�[���̃��[�h/�A�����[�h��nowMode��؂�ւ��Ă������̂��������ق��������Ǝv���B

public class FieldMng : MonoBehaviour
{
    // �l�X�ȃN���X����MODE�̏�Ԃ͌����邱�ƂɂȂ邩��AnowMode_��static�ϐ��ɂ����ق�������
    // ���̃N���X�͉�ʏ�Ԃ̑J�ڂ��Ǘ����邾���ŁA����ȊO�̉�ʏ����͑���Script�ōs��

    // ��ʏ�Ԉꗗ
    public enum MODE
    {
        NON,
        SEARCH,     // �T����
        BUTTLE,     // �퓬��
        MENU,       // ���j���[��ʒ�
        MAX
    }

    public static MODE nowMode = MODE.NON;          // ���݂̃��[�h
    public static float charaRunSpeed = 0.0f;

    private float toButtleTime_ = 30.0f;            // 30�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                     // ���݂̌o�ߎ���

    private UnitychanController player_;            // �v���C���[���i�[�p
    private CameraMng cameraMng_;

    void Start()
    {
        //unitychan�̏����擾
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (player_ == null)
        {
            Debug.Log("FieldMng.cs�Ŏ擾���Ă���Player���null�ł�");
        }

        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("FieldMng.cs�Ŏ擾���Ă���CameraMng��null�ł�");
        }
    }

    void Update()
    {
        //Debug.Log("���݂�MODE" + nowMode_);

        //Debug.Log(time_);

        switch (nowMode)
        {
            case MODE.NON:
                charaRunSpeed = 8.0f;
                break;

            case MODE.SEARCH :
                cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u
                charaRunSpeed = 4.0f;

                if (player_.GetMoveFlag() && time_ < toButtleTime_)
                {
                    time_ += Time.deltaTime;
                }
                else if(time_ >= toButtleTime_)
                {
                    nowMode = MODE.BUTTLE;
                    time_ = 0.0f;
                }
                else
                {
                    // �����������s��Ȃ�
                }
            break;

            case MODE.BUTTLE:
                cameraMng_.SetChangeCamera(true);   // �T�u�J�����A�N�e�B�u
                charaRunSpeed = 0.0f;

                if (time_ < toButtleTime_)
                {
                    time_ += Time.deltaTime;
                }
                else
                {
                    nowMode = MODE.SEARCH;
                    time_ = 0.0f;
                }
            break;

            default:
                Debug.Log("��ʏ�Ԉꗗ�ŃG���[�ł�");
            break;
        }
    }

    // ���ݒl / �G���J�E���g�����l
    public float GetNowEncountTime()
    {
        return time_ / toButtleTime_;
    }

}
