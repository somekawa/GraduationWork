using UnityEngine;
using UnityEngine.UI;

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
    public static bool onceflag_ = true;

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
            var tmp = GameObject.Find("DontDestroyCanvas/Managers");
            tmp.GetComponent<Bag_Item>().NewGameInit();
            tmp.GetComponent<Bag_Materia>().NewGameInit();
            tmp.GetComponent<Bag_Word>().NewGameInit();
            tmp.GetComponent<Bag_Magic>().NewGameInit();

            GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>().NewGameInit();

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

        // �{�^���̏�Ԃ��킩��₷���悤�ɂ���
        var sleepBtn_ = uniHouseCanvas.transform.Find("SleepButton").GetComponent<Button>();
        if (EventMng.GetChapterNum() < 7)    // �i�s�x��0�`6�̂Ƃ�
        {
            sleepBtn_.interactable = false;
        }
        else
        {
            sleepBtn_.interactable = true;
        }

        SceneMng.MenuSetActive(true);

        if (onceflag_ == true)
        {
            onceflag_ = false;
            GameObject.Find("Managers").GetComponent<Bag_Word>().DataLoad();
            GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
            GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
            GameObject.Find("Managers").GetComponent<Bag_Materia>().DataLoad();
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
        Debug.Log("�x�ރ{�^������������܂��� �������ʂ�����đ̗͉񕜂��܂�");

        // �����I�ɗ����̌��ʂ�����
        GameObject.Find("DontDestroyCanvas/TimeGear/CookPowerIcon").GetComponent<Image>().color =
            new Color(1.0f, 1.0f, 1.0f, 0.0f);
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            SceneMng.charasList_[i].DeleteStatusUpByCook();
            SceneMng.charasList_[i].SetHP(SceneMng.charasList_[i].MaxHP());
            SceneMng.charasList_[i].SetMP(SceneMng.charasList_[i].MaxMP());
        }
    }

    public void ClickAlchemyButton()
    {
        alchemyMng.gameObject.SetActive(true);
        uniHouseCanvas.gameObject.SetActive(false);
        alchemyMng.GetComponent<ItemCreateMng>().Init();
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
