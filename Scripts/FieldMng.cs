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

    public static MODE nowMode = MODE.SEARCH;      // ���݂̃��[�h
    public static MODE oldMode = MODE.NON;         // �O�̃��[�h

    public Canvas menuUICanvas;                    // ���j���[���Canvas

    private float toButtleTime_ = 1.0f;            // 30�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                    // ���݂̌o�ߎ���

    private UnitychanController player_;           // �v���C���[���i�[�p
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

        menuUICanvas.gameObject.SetActive(false);

        // ���݂̃V�[����FIELD�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.FIELD);
    }

    void Update()
    {
        //Debug.Log("���݂�MODE" + nowMode);
        //Debug.Log(time_);

        if (nowMode == MODE.SEARCH)
        {
            // �T������TAB�L�[���������ꂽ��nowMode��MENU�ɐ؂�ւ���
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("���j���[��ʂ�\�����܂�");
                nowMode = MODE.MENU;
            }

            // �T�����̎��ԉ��Z����
            if (player_.GetMoveFlag() && time_ < toButtleTime_)
            {
                time_ += Time.deltaTime;
            }
            else if (time_ >= toButtleTime_)
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;
            }
            else
            {
                // �����������s��Ȃ�
            }
        }

        // �O���Mode�ƈ�v���Ȃ��Ƃ�
        if (nowMode != oldMode)
        {
            ChangeMode(nowMode);
        }

    }

    // ���[�h���؂�ւ�����^�C�~���O�݂̂ŌĂяo���֐�
    void ChangeMode(MODE mode)
    {
        switch (mode)
        {
            case MODE.NON:
                break;

            case MODE.SEARCH:
                cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u
                menuUICanvas.gameObject.SetActive(false);
                break;

            case MODE.BUTTLE:
                cameraMng_.SetSubCameraPos(new Vector3(4.0f,2.0f,11.5f));
                cameraMng_.SetChangeCamera(true);   // �T�u�J�����A�N�e�B�u
                break;

            case MODE.MENU:
                menuUICanvas.gameObject.SetActive(true);
                break;

            default:
                Debug.Log("��ʏ�Ԉꗗ�ŃG���[�ł�");
                break;
        }

        // oldMode�̍X�V������
        oldMode = nowMode;
    }

    // ���ݒl / �G���J�E���g�����l
    public float GetNowEncountTime()
    {
        return time_ / toButtleTime_;
    }

}
