using System.Collections;
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

    [SerializeField]
    private GameObject fieldUICanvasPopUp_; // FieldUICanvas�̒��ɂ���PopUp�Ƃ�����̃I�u�W�F�N�g���O���A�^�b�`����

    //[SerializeField]
    //private GameObject menu_;               // DontDestroyCanvas�̒���Menu�I�u�W�F�N�g���O���A�^�b�`����

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

    private TMPro.TextMeshProUGUI titleInfo_;      // �󔠂��ǂ��ŕ\�����e��ύX����
    private TMPro.TextMeshProUGUI getChestsInfo_;  // �󔠂���l�������A�C�e�����e�̕\����

    private GameObject DataPopPrefab_;
    private ChestList  popChestList_;

    void Start()
    {
        // ���݂̃V�[����FIELD�Ƃ���
        SceneMng.SetNowScene((SceneMng.SCENE)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        // �C�x���g���������邩�m�F����
        if (EventMng.GetChapterNum() == 8)
        {
            EventMng.SetChapterNum(8, SceneMng.SCENE.CONVERSATION);
        }
        else if(EventMng.GetChapterNum() == 13)
        {
            EventMng.SetChapterNum(13, SceneMng.SCENE.CONVERSATION);
        }

        // DesertField���u�I�A�V�X���S�点�āv�̃N�G�X�g��B����Ȃ�
        // �N�G�X�g�B����̉�b�Ői�s�x��15�ƂȂ�
        if((SceneMng.nowScene == SceneMng.SCENE.FIELD1) && EventMng.GetChapterNum() >= 15)
        {
            // Oasis�I�u�W�F�N�g���܂߂��S�Ă�FieldMap�I�u�W�F�N�g��true�ɂ���
            var tmp = GameObject.Find("FieldMap").transform;
            for (int i = 0; i < tmp.childCount; i++)
            {
                tmp.GetChild(i).gameObject.SetActive(true);
            }
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

        cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // �C�x���g�픭���p�̕�/�Ǐ����擾����
        CheckWallAndChestActive("ButtleWall");
        CheckWallAndChestActive("Chests");

        // Chest.xls����󔠓��e�̎擾���s��
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        popChestList_  = DataPopPrefab_.GetComponent<PopList>().GetData<ChestList>(PopList.ListData.CHEST);

        // ���e�̃^�C�g��
        titleInfo_ = fieldUICanvasPopUp_.transform.Find("TitleInfo").GetComponent<TMPro.TextMeshProUGUI>();
        // �󔠂̕����`���
        getChestsInfo_ = fieldUICanvasPopUp_.transform.Find("GetChestsInfo").GetComponent<TMPro.TextMeshProUGUI>();
    }

    // �N�G�X�g�̎󒍏󋵂ɍ��킹�āA�ǂ�󔠂̃A�N�e�B�u��Ԃ𔻕ʂ���
    private void CheckWallAndChestActive(string parentName)
    {
        // ���ݎ󒍒��̃N�G�X�g��������
        var orderList = QuestClearCheck.GetOrderQuestsList();

        // �C�x���g�p�̕󔠏����擾����
        var objParent = GameObject.Find(parentName);
        var objChild = new GameObject[objParent.transform.childCount];
        for (int i = 0; i < objParent.transform.childCount; i++)
        {
            objChild[i] = objParent.transform.GetChild(i).gameObject;
        }

        // 1���N�G�X�g���󒍂��Ă��Ȃ��ۂ́A�S�Ă̕�/�ǂ��A�N�e�B�u�ɂ���
        if (orderList.Count <= 0)
        {
            for (int i = 0; i < objChild.Length; i++)
            {
                objChild[i].SetActive(false);    
            }
        }

        // ��/�ǂ̌���for������
        for (int i = 0; i < objChild.Length; i++)
        {
            // �󒍒��N�G�X�g����for������
            for (int k = 0; k < orderList.Count; k++)
            {
                // ���O��[�N�G�X�g�ԍ� - �z�u�ԍ�]�ƂȂ��Ă��邩��A�����ŕ������Ă�����
                string[] arr = objChild[i].name.Split('-');

                if (arr[0] == orderList[k].Item1.name)
                {
                    // ���O��v���̏���
                    if (orderList[k].Item2)
                    {
                        // �N���A�ς݃N�G�X�g�Ȃ��/�ǂ��A�N�e�B�u��
                        objChild[i].SetActive(false);
                    }
                    else
                    {
                        // ���N���A�Ȃ��/�ǂ��A�N�e�B�u��
                        objChild[i].SetActive(true);
                    }
                    break;
                }
                else
                {
                    // ���O�s��v���̏���
                    objChild[i].SetActive(false);
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
                time_ += Time.deltaTime;
            }
            else if (time_ >= toButtleTime_)
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;

                // �܂��|�b�v�A�b�v���ł�������A��A�N�e�B�u�ɂ���
                if(fieldUICanvasPopUp_.activeSelf)
                {
                    fieldUICanvasPopUp_.SetActive(false);
                }
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
                //menu_.SetActive(true);
                break;

            case MODE.BUTTLE:
                cameraMng_.SetChangeCamera(true);    // �T�u�J�����A�N�e�B�u
                //menu_.SetActive(false);
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

    // flag�́Atrue->��,false->��(�����퓬)�̏����Ƃ������Ɏg��������
    public void ChangeFieldUICanvasPopUpActive(int num1,int num2,bool flag)
    {
        if(flag)
        {
            // �󔠊֘A����
            // �\�����镶�������肷��
            for (int i = 0; i < popChestList_.param.Count; i++)
            {
                // �ǂ���̐�������v���Ă�����
                if (popChestList_.param[i].num1 == num1 &&
                   popChestList_.param[i].num2 == num2)
                {
                    // Excel��getItem���e���������݁A�^�C�g����GetItem�ɂ���
                    titleInfo_.text = "GetItem";
                    getChestsInfo_.text = popChestList_.param[i].getItem;
                }
            }

            fieldUICanvasPopUp_.SetActive(true);
            // �I�����͔�\���ɂ���
            // �����񐔓I��Find��K���s���Ă����Ȃ������Ȃ̂ŁA���̏������ɂ��Ă��܂��B
            fieldUICanvasPopUp_.transform.Find("Select").gameObject.SetActive(false);

            StartCoroutine(PopUpMessage());
        }
        else
        {
            // ��(�����퓬)�֘A����
            titleInfo_.text = "Danger!!";
            getChestsInfo_.text = "�����X�^�[�̋C�z������c\n��ɐi�݂܂����H";

            // ���łɃA�N�e�B�u��ԂȂ�΁A��A�N�e�B�u�ɂȂ邵�A
            // ��A�N�e�B�u�Ȃ�΁A�A�N�e�B�u�ɂ���(�������t�ɂ���ƑI�����o�Ă��Ȃ����璍��)
            // �����񐔓I��Find��K���s���Ă����Ȃ������Ȃ̂ŁA���̏������ɂ��Ă��܂��B
            fieldUICanvasPopUp_.transform.Find("Select").gameObject.SetActive(!fieldUICanvasPopUp_.activeSelf);
            fieldUICanvasPopUp_.SetActive(!fieldUICanvasPopUp_.activeSelf);
        }
    }

    private IEnumerator PopUpMessage()
    {
        float time = 0.0f;
        while (fieldUICanvasPopUp_.activeSelf)
        {
            yield return null;

            if(time >= 3.0f)
            {
                fieldUICanvasPopUp_.SetActive(false);
            }
            else
            {
                time += Time.deltaTime;
            }
        }
    }

    public void MoveArrowIcon(bool flag)
    {
        // �����񐔓I��Find��K���s���Ă����Ȃ������Ȃ̂ŁA���̏������ɂ��Ă��܂��B
        if(flag)
        {
            // �͂�
            fieldUICanvasPopUp_.transform.Find("Select/Icon").transform.localPosition = new Vector3(-160.0f, -60.0f, 0.0f);
        }
        else
        {
            // ������
            fieldUICanvasPopUp_.transform.Find("Select/Icon").transform.localPosition = new Vector3(40.0f, -60.0f, 0.0f);
        }
    }

}
