using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float toButtleTime_ = 1.0f;            // 30�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                    // ���݂̌o�ߎ���

    private UnitychanController player_;           // �v���C���[���i�[�p
    private CameraMng cameraMng_;
    private Image bagImage_;// �����̃o�b�O�̉摜

    void Start()
    {
        // ���݂̃V�[����FIELD�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.FIELD0);

        // �C�x���g���������邩�m�F����
        if (EventMng.GetChapterNum() == 8)
        {
            EventMng.SetChapterNum(8, SceneMng.SCENE.CONVERSATION);
        }
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

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // �o�b�O�̉摜��DontDestroyOnLoad�����I�u�W�F�N�g�̂��̂��߂��̐e����T��
        //var gameObject = DontDestroyMng.Instance;
        bagImage_ = GameObject.Find("DontDestroyCanvas/Menu/BagImage").GetComponent<Image>();

        // �C�x���g�픭���p�̕Ǐ����擾����
        var ParentObject = GameObject.Find("ButtleWall");
        var ChildObject = new GameObject[ParentObject.transform.childCount];
        for (int i = 0; i < ParentObject.transform.childCount; i++)
        {
            ChildObject[i] = ParentObject.transform.GetChild(i).gameObject;
        }

        // ���ݎ󒍒��̃N�G�X�g��������
        var orderList = QuestClearCheck.GetOrderQuestsList();

        // 1���N�G�X�g���󒍂��Ă��Ȃ��ۂ́A�S�Ă̕ǂ��A�N�e�B�u�ɂ���
        if(orderList.Count <= 0)
        {
            for (int i = 0; i < ChildObject.Length; i++)
            {
                ChildObject[i].SetActive(false);
            }
        }

        // �ǂ̌���for������
        for (int i = 0; i < ChildObject.Length; i++)
        {
            // �󒍒��N�G�X�g����for������
            for(int k = 0; k < orderList.Count; k++)
            {
                if(ChildObject[i].name == orderList[k].Item1.name)
                {
                    // ���O��v���̏���
                    if (orderList[k].Item2)
                    {
                        // �N���A�ς݃N�G�X�g�Ȃ�ǂ��A�N�e�B�u��
                        ChildObject[i].SetActive(false);
                    }
                    else
                    {
                        // ���N���A�Ȃ�ǂ��A�N�e�B�u��
                        ChildObject[i].SetActive(true);
                    }
                    break;
                }
                else
                {
                    // ���O�s��v���̏���
                    ChildObject[i].SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        //Debug.Log("���݂�MODE" + nowMode);
        //Debug.Log(time_);

        if (nowMode == MODE.SEARCH)
        {
            // �T�����̎��ԉ��Z����
            if (player_.GetMoveFlag() && time_ < toButtleTime_)
            {
                //time_ += Time.deltaTime;
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
                bagImage_.gameObject.SetActive(true);
                break;

            case MODE.BUTTLE:
                cameraMng_.SetChangeCamera(true);   // �T�u�J�����A�N�e�B�u
                bagImage_.gameObject.SetActive(false);// �o�b�O�摜���A�N�e�B�u��
                break;

            case MODE.MENU:
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
