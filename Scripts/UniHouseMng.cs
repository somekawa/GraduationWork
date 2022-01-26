using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    [SerializeField]
    private GameObject nightLights; // �X��

    [SerializeField]
    private RectTransform alchemyMng;

    [SerializeField]
    private RectTransform magicCreateMng;

    [SerializeField]
    private Canvas uniHouseCanvas;

    private GameObject loadPrefab_;// �^�C�g���V�[������̑J�ڂ��ǂ���
    private OnceLoad onceLoad_;// LoadPrefab�ɃA�^�b�`����Ă�Script
    void Start()
    {
        // ���݂̃V�[����UNIHOUSE�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);

        // ������Ȃ�΃��C�g�_���A����ȊO�Ȃ烉�C�g����
        if (SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.NIGHT)
        {
            nightLights.SetActive(true);
        }
        else
        {
            nightLights.SetActive(false);
        }

        // WarpTown.cs�̏������֐����Ă�
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // �I�u�W�F�N�g�����遁�^�C�g���V�[������J�ڂ��Ă���
        loadPrefab_ = GameObject.Find("LoadPrefab");
        if (loadPrefab_ != null)
        {
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>().NewGameInit();
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>().NewGameInit();
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().NewGameInit();
            onceLoad_ = GameObject.Find("LoadPrefab").GetComponent<OnceLoad>();
            onceLoad_.SetNewGameFlag(true);
        }
        // ���C���J�������ŏ��ɃA�N�e�B�u�ɂ���
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);

        // �������̃~�j�Q�[���pMng
        alchemyMng.gameObject.SetActive(false);
        magicCreateMng.gameObject.SetActive(false);

        // �X�e�[�^�X�A�b�v�����������肷��
        if (!SceneMng.GetFinStatusUpTime())
        {
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].DeleteStatusUpByCook();
            }
        }
    }

    public void ClickSleepButton()
    {
        if(EventMng.GetChapterNum() < 7)    // �i�s�x��0�`6�̂Ƃ�
        {
            Debug.Log("���݂̐i�s�x��7�����̂��߁A�x�߂܂���");
            return; // �x�ރ{�^���������Ă��������Ȃ��悤�ɂ���
        }

        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING);   // ���Ԍo��
        Debug.Log("�x�ރ{�^������������܂���");
    }

    public void ClickAlchemyButton()
    {
        alchemyMng.gameObject.SetActive(true);
        uniHouseCanvas.gameObject.SetActive(false);
        Debug.Log("�����{�^������������܂���");
    }

    public void ClickMagicCreateButton()
    {    
        // ��̃}�e���A��1�ȏ㎝���Ă����烏�[�h�������ł���
        //if (0 < Bag_Materia.materiaState[Bag_Materia.emptyMateriaNum].haveCnt)
        //{
            uniHouseCanvas.gameObject.SetActive(false);
            magicCreateMng.gameObject.SetActive(true);
            magicCreateMng.GetComponent<MagicCreate>().Init();
      //  }
        Debug.Log("���@�쐬�{�^������������܂���");
    }
}
