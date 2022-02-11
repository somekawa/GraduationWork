using System.Collections;
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

    [SerializeField]
    private Image warningInfo;// ���@���A�C�e�������Ȃ��Ƃ��̌x���摜
    private Text warningText_;

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
        warningInfo.gameObject.SetActive(false);
        warningText_ = warningInfo.transform.Find("Text").GetComponent<Text>();
        warningText_.text = null;

        // �X�e�[�^�X�A�b�v�����������肷��
        if (!SceneMng.GetFinStatusUpTime().Item2)
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

        SceneMng.MenuSetActive(true,true);
    }

    public void ClickSleepButton()
    {
        if(EventMng.GetChapterNum() < 7)    // �i�s�x��0�`6�̂Ƃ�
        {
            Debug.Log("���݂̐i�s�x��7�����̂��߁A�x�߂܂���");
            return; // �x�ރ{�^���������Ă��������Ȃ��悤�ɂ���
        }

        SceneMng.SetSE(0);

        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING,true);   // ���Ԍo��
        Debug.Log("�x�ރ{�^������������܂��� �������ʂ�����đ̗͉񕜂��܂�");

        // �����I�ɗ����̌��ʂ������āA�̗͂��񕜂���
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            SceneMng.charasList_[i].DeleteStatusUpByCook();
            SceneMng.charasList_[i].SetHP(SceneMng.charasList_[i].MaxHP());
            SceneMng.charasList_[i].SetMP(SceneMng.charasList_[i].MaxMP());
        }
    }

    public void ClickAlchemyButton()
    {
        SceneMng.SetSE(0);

        if (BookStoreMng.bookState_[5].readFlag == 0)
        {
            warningText_.text = "�{���Ń��V�s���w�����܂��傤";
            StartCoroutine(Warning());
            return;
        }

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
        SceneMng.SetSE(0);

        if (EventMng.GetChapterNum() < 8)
        {
            // �C�x���g8�ȉ��̓��[�h���擾���Ă��Ȃ����߉����Ȃ��悤�ɂ���
            warningText_.text = "���[�h������܂���";
            StartCoroutine(Warning());
            return;
        }
        else
        {
            // ��̃}�e���A��1�ȏ㎝���Ă����烏�[�h�������ł���
            if (Bag_Materia.materiaState[Bag_Materia.emptyMateriaNum].haveCnt <= 0)
            {
                warningText_.text = "��̃}�e���A������܂���";
                StartCoroutine(Warning());
                return;
            }
        }
        uniHouseCanvas.gameObject.SetActive(false);
        magicCreateMng.gameObject.SetActive(true);
        magicCreateMng.GetComponent<MagicCreate>().Init();
        Debug.Log("���@�쐬�{�^������������܂���");
    }

    private IEnumerator Warning()
    {
        warningInfo.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        warningInfo.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        warningText_.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        warningInfo.gameObject.SetActive(true);
        float alpha = 1.0f;
        while (true)
        {
            yield return null;
            alpha -= 0.01f;
            warningInfo.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            warningText_.color = new Color(1.0f, 0.0f, 0.0f, alpha);
            if (alpha <= 0.0f)
            {
                warningInfo.gameObject.SetActive(false);
                Debug.Log("�x���\���������܂�");
                yield break;
            }
        }
    }
}