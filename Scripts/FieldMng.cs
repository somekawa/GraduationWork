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

    [SerializeField]
    private GameObject fieldUICanvasPopUp_; // FieldUICanvas�̒��ɂ���PopUp�Ƃ�����̃I�u�W�F�N�g���O���A�^�b�`����

    public AudioClip BGM_search;
    public AudioClip BGM_normalButtle;
    public AudioClip BGM_bossButtle;

    private AudioSource audios;
    private GameObject jackObj_;

    // ��ʏ�Ԉꗗ
    public enum MODE
    {
        NON,
        SEARCH,     // �T����
        BUTTLE,     // �퓬��
        MENU,       // ���j���[��ʒ�
        FORCEDBUTTLE,   // �����퓬��(�ǂƂ̏Փˎ��ɐ؂�ւ��)
        MAX
    }

    public static MODE nowMode = MODE.SEARCH;      // ���݂̃��[�h
    public static MODE oldMode = MODE.NON;         // �O�̃��[�h

    public static bool stopEncountTimeFlg = false; // �A�C�e���̌��ʂňꎞ�I�ɃG���J�E���g���~�߂�
    private bool stopEncountTimeFlgOld_ = false;   // 1�t���[���O�Ɣ�r����
    private float toButtleTime_ = 6.0f;            // 6�b�o�߂Ńo�g���֑J�ڂ���
    private float time_ = 0.0f;                    // ���݂̌o�ߎ���
    private float keepTime_ = 0.0f;                // ���݂̃G���J�E���g�܂ł̎��Ԃ��ꎞ�ۑ����Ă���

    private UnitychanController player_;           // �v���C���[���i�[�p
    private CameraMng cameraMng_;

    private TMPro.TextMeshProUGUI titleInfo_;      // �󔠂��ǂ��ŕ\�����e��ύX����
    private TMPro.TextMeshProUGUI getChestsInfo_;  // �󔠂���l�������A�C�e�����e�̕\����

    // string->�I�u�W�F�N�g��,bool->�N���A�ςݔ���(false�͖��N���A)
    public static List<(string, bool)> treasureList         = new List<(string, bool)>();
    public static List<(string, bool)> forcedButtleWallList = new List<(string, bool)>();

    // Excel���
    private GameObject DataPopPrefab_;
    private ChestList  popChestList_;

    void Start()
    {
        // ���݂̃V�[����FIELD�Ƃ���
        SceneMng.SetNowScene((SceneMng.SCENE)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().Init();

        audios = GetComponent<AudioSource>();

        // �C�x���g���������邩�m�F����
        if (EventMng.GetChapterNum() == 8 && SceneMng.nowScene == SceneMng.SCENE.FIELD0)
        {
            // �i�s�x8 ���� �L��̐X
            EventMng.SetChapterNum(8, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 13 && SceneMng.nowScene == SceneMng.SCENE.FIELD1)
        {
            // �i�s�x13 ���� ���F�X�e����
            EventMng.SetChapterNum(13, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 16 && SceneMng.nowScene == SceneMng.SCENE.FIELD2)
        {
            // �i�s�x16 ���� Field3
            EventMng.SetChapterNum(16, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 19 && SceneMng.nowScene == SceneMng.SCENE.FIELD3)
        {
            // �i�s�x19 ���� Field4
            EventMng.SetChapterNum(19, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if (EventMng.GetChapterNum() == 22 && SceneMng.nowScene == SceneMng.SCENE.FIELD4)
        {
            // �i�s�x22 ���� Field5
            EventMng.SetChapterNum(22, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else
        {
            nowMode = MODE.SEARCH;
            audios.clip = BGM_search;
            audios.Play();
        }

        // DesertField���u�I�A�V�X���S�点�āv�̃N�G�X�g��B����Ȃ�
        // �N�G�X�g�B����̉�b�Ői�s�x��15�ƂȂ�
        if ((SceneMng.nowScene == SceneMng.SCENE.FIELD1) && EventMng.GetChapterNum() >= 15)
        {
            // Oasis�I�u�W�F�N�g���܂߂��S�Ă�FieldMap�I�u�W�F�N�g��true�ɂ���
            var tmp = GameObject.Find("FieldMap").transform;
            for (int i = 0; i < tmp.childCount; i++)
            {
                tmp.GetChild(i).gameObject.SetActive(true);
            }
        }

        // Field3���u�S�[������ʔ����v�̃N�G�X�g��(���B��)�Ȃ�
        var quest = QuestClearCheck.GetOrderQuestsList();
        for(int i = 0; i < quest.Count; i++)
        {
            if(int.Parse(quest[i].Item1.name) == 5 && !quest[i].Item2)
            {
                if ((SceneMng.nowScene == SceneMng.SCENE.FIELD2))
                {
                    // �G���J�E���g�̑��x��2�{�ɂ���(�����I�ɂ�1/2�ɂ���)
                    toButtleTime_ /= 2.0f;
                    break;  // �����𔲂���
                }
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

        cameraMng_.MainCameraPosInit();
        cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // �C�x���g�픭���p�̕�/�Ǐ����擾����
        CheckWallAndChestActive("ButtleWall", forcedButtleWallList);
        CheckWallAndChestActive("Chests", treasureList);

        // Chest.xls����󔠓��e�̎擾���s��
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        // StreamingAssets����AssetBundle�����[�h����
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle�J��");
        // AssetBundle���̃A�Z�b�g�ɂ̓r���h���̃A�Z�b�g�̃p�X�A�܂��̓t�@�C�����A�t�@�C�����{�g���q�ŃA�N�Z�X�ł���
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // �s�v�ɂȂ���AssetBundle�̃��^�����A�����[�h����
        assetBundle.Unload(false);
        Debug.Log("�j��");

        popChestList_ = DataPopPrefab_.GetComponent<PopList>().GetData<ChestList>(PopList.ListData.CHEST);

        // ���e�̃^�C�g��
        titleInfo_ = fieldUICanvasPopUp_.transform.Find("TitleInfo").GetComponent<TMPro.TextMeshProUGUI>();
        // �󔠂̕����`���
        getChestsInfo_ = fieldUICanvasPopUp_.transform.Find("GetChestsInfo").GetComponent<TMPro.TextMeshProUGUI>();

        // �X�e�[�^�X�A�b�v�����������肷��
        if(!SceneMng.GetFinStatusUpTime().Item2)
        {
            for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].DeleteStatusUpByCook();
            }
        }

        GameObject.Find("FieldMap/MateriaPoints").GetComponent<DropFieldMateria>().Init();

        jackObj_ = GameObject.Find("Jack").gameObject;
        jackObj_.SetActive(false);
    }

    // �N�G�X�g�̎󒍏󋵂ɍ��킹�āA�ǂ�󔠂̃A�N�e�B�u��Ԃ𔻕ʂ���
    private void CheckWallAndChestActive(string parentName,List<(string,bool)> list)
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

        // ��/�Ǐ��̓o�^
        for (int i = 0; i < objChild.Length; i++)
        {
            bool addFlag = true;
            // ����Field�����߂ĖK�ꂽ���m�F����
            foreach (var tmpList in list)
            {
                // ���X�g���ɂ��閼�O�ƁA�o�^���悤�Ƃ��Ă���I�u�W�F�N�g����1�ł���v���Ă�����
                if (tmpList.Item1 == objChild[i].name)
                {
                    addFlag = false;
                    break;
                }
            }

            // �o�^�t���O��false�ɂȂ��Ă�����for�������������
            if (!addFlag)
            {
                break;
            }

            // ����o�^
            list.Add((objChild[i].name, false));
        }

        // ��/�ǂ̌���for������
        for (int i = 0; i < objChild.Length; i++)
        {
            // �ŏ���false�ɂ���
            objChild[i].SetActive(false);

            // ���O��[�N�G�X�g�ԍ� - �z�u�ԍ�]�ƂȂ��Ă��邩��A�����ŕ������Ă�����
            string[] arr = objChild[i].name.Split('-');

            // �󒍒��N�G�X�g����for������
            for (int k = 0; k < orderList.Count; k++)
            {
                if (arr[0] != orderList[k].Item1.name)
                {
                    continue;
                }

                for (int a = 0; a < list.Count; a++)
                {
                    if (objChild[i].name == list[a].Item1)
                    {
                        // ���O��v���̏���
                        // �N���A�ς݃N�G�X�g�Ȃ��/�ǂ��A�N�e�B�u�ցA���N���A�Ȃ��/�ǂ��A�N�e�B�u��
                        objChild[i].SetActive(!list[a].Item2);
                        break;
                    }
                }
            }

            if (arr[0] == "100" || arr[0] == "200") // �N�G�X�g�̗v�f�Ɋ܂܂�Ȃ���Ǖ󔠂ƕǂ̏ꍇ
            {
                for (int a = 0; a < list.Count; a++)
                {
                    // �N���A�ς݃N�G�X�g�Ȃ��/�ǂ��A�N�e�B�u�ցA���N���A�Ȃ��/�ǂ��A�N�e�B�u��
                    objChild[i].SetActive(!list[a].Item2);
                    continue;
                }
            }
        }
    }

    void Update()
    {
        // �A�C�e���̌��ʂŃG���J�E���g����~
        if(stopEncountTimeFlg && stopEncountTimeFlg != stopEncountTimeFlgOld_)
        {
            // �l��ۑ����Ă���
            keepTime_ = toButtleTime_;
            stopEncountTimeFlgOld_ = stopEncountTimeFlg;
            toButtleTime_ = 20.0f;
            Debug.Log("�ꎞ�I�ɃG���J�E���g����~���܂���");
        }

        if (nowMode == MODE.SEARCH)
        {
            if(SceneMng.nowScene == SceneMng.SCENE.FIELD4)
            {
                return;
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

                stopEncountTimeFlg = false;
                if(!stopEncountTimeFlg && stopEncountTimeFlgOld_)
                {
                    // �ۑ����Ă������l�����Ȃ���
                    stopEncountTimeFlgOld_ = stopEncountTimeFlg;
                    toButtleTime_ = keepTime_;
                    Debug.Log("�A�C�e���̌��ʂ�����āA�G���J�E���g�ĊJ���܂���");
                }

                // �܂��|�b�v�A�b�v���ł�������A��A�N�e�B�u�ɂ���
                if (fieldUICanvasPopUp_.activeSelf)
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
                if(SceneMng.nowScene == SceneMng.SCENE.FIELD4)
                {
                    jackObj_.SetActive(false);
                    cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u
                    SceneMng.MenuSetActive(true,true);
                    audios.Stop();
                    audios.clip = BGM_search;//�����N���b�v��؂�ւ���
                    audios.Play();
                    nowMode = MODE.SEARCH;
                }
                break;

            case MODE.SEARCH:
                jackObj_.SetActive(false);
                cameraMng_.SetChangeCamera(false);   // ���C���J�����A�N�e�B�u
                SceneMng.MenuSetActive(true, true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_search;//�����N���b�v��؂�ւ���
                    audios.Play();
                }
                break;

            case MODE.BUTTLE:
                jackObj_.SetActive(true);
                cameraMng_.SetChangeCamera(true);    // �T�u�J�����A�N�e�B�u
                SceneMng.MenuSetActive(false,true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_normalButtle;//�����N���b�v��؂�ւ���
                    audios.Play();
                }
                break;

            case MODE.MENU:
                break;

            case MODE.FORCEDBUTTLE:
                jackObj_.SetActive(true);
                cameraMng_.SetChangeCamera(true);    // �T�u�J�����A�N�e�B�u
                SceneMng.MenuSetActive(false,true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_bossButtle;//�����N���b�v��؂�ւ���
                    audios.Play();
                }
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

    public bool GetStopEncountTimeFlg()
    {
        return stopEncountTimeFlg;
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
            getChestsInfo_.text = "�������Ȃ��킢���c\n��ɐi�݂܂����H";

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
