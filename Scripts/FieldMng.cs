using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̊O�ł̉�ʊǗ�������B(�����ł�nowMode��NON�ɂ���)
// �T���E�퓬�E���j���[��ʂ̐؂�ւ�莞��enum�ŕύX���s��
// ����Mng�Ƃ����A���̃X�N���v�g�ɂ���nowMode_���Q�Ƃ��ĕύX���s��

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

    private float toButtleTime_ = 30.0f;            // 30�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                     // ���݂̌o�ߎ���

    private UnitychanController player_;            // �v���C���[���i�[�p

    void Start()
    {
        //unitychan�̏����擾
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
    }

    void Update()
    {
        //Debug.Log("���݂�MODE" + nowMode_);

        //Debug.Log(time_);

        switch (nowMode)
        {
            case MODE.SEARCH :
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
