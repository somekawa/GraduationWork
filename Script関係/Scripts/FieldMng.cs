using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���̊O�ł̉�ʊǗ�������B
// �T���E�퓬�E���j���[��ʂ̐؂�ւ�莞��enum�ŕύX���s��
// ����Mng�Ƃ����A���̃X�N���v�g�ɂ���nowMode_���Q�Ƃ��ĕύX���s��

public class FieldMng : MonoBehaviour
{
    // �l�X�ȃN���X����MODE�̏�Ԃ͌����邱�ƂɂȂ邩��AnowMode_��static�ϐ��ɂ����ق�������

    // ��ʏ�Ԉꗗ
    public enum MODE
    {
        NON,
        SEARCH,     // �T����
        BUTTLE,     // �퓬��
        MENU,       // ���j���[��ʒ�
        MAX
    }

    public static MODE nowMode = MODE.SEARCH;       // ���݂̃��[�h

    private float toButtleTime_ = 5.0f;              // 5�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                      // ���݂̌o�ߎ���

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("���݂�MODE" + nowMode_);

        switch(nowMode)
        {
            case MODE.SEARCH :
            if (time_ < toButtleTime_)
            {
                time_ += Time.deltaTime;
            }
            else
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;
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
}
