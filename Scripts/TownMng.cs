using System.Collections.Generic;
using UnityEngine;

public class TownMng : MonoBehaviour
{
    //[SerializeField]
    //private Canvas inHouseCanvas;

    [SerializeField]
    private GameObject nightLights; // �X��

    private readonly string[] buildNameEng_ = { "MayorHouse", "BookStore", "ItemStore", "Guild", "Restaurant" };  // ������(�q�G�����L�[�Ɠ����p��)
    private readonly Vector3[] buildPos_ = { new Vector3(0.0f, 0.0f, 110.0f),   
                                             new Vector3(-12.0f,0.0f, 56.0f),
                                             new Vector3(31.0f, 0.0f, 96.0f),
                                             new Vector3(23.0f, 0.0f, 96.0f),
                                             new Vector3(17.0f, 0.0f, 52.0f) };
    private Dictionary<string, Vector3> uniPosMap_ = new Dictionary<string, Vector3>();    // �L�[:�p�ꌚ����,�l:���j�����\�����W
   
    private GameObject loadPrefab_;// �^�C�g���V�[������̑J�ڂ��ǂ���
    private OnceLoad onceLoad_;// LoadPrefab�ɃA�^�b�`����Ă�Script

    void Start()
    {
        // ���݂̃V�[����TOWN�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.TOWN);

        // ���C���J�����̏������֐����ĂԂ��߂ɁACameraMng.cs���o�R���Ă���
        GameObject.Find("CameraController").GetComponent<CameraMng>().MainCameraPosInit();

        // ������Ȃ�΃��C�g�_���A����ȊO�Ȃ烉�C�g����
        if (SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.NIGHT)
        {
            nightLights.SetActive(true);
        }
        else
        {
            nightLights.SetActive(false);
        }

        // �����ƍ��W����v������
        for (int i = 0; i < buildNameEng_.Length; i++)
        {
            uniPosMap_.Add(buildNameEng_[i], buildPos_[i]);
        }

        // SceneMng�����΂����������󂯂Ƃ�(��΂��Ȃ��Ă����Ƃ��͏������Ȃ��悤�ɒ���)
        string str = SceneMng.GetHouseName();

        // �I�u�W�F�N�g�����遁�^�C�g���V�[������J�ڂ��Ă���
        loadPrefab_ = GameObject.Find("LoadPrefab");
        if (loadPrefab_ != null)
        {
            onceLoad_ = GameObject.Find("LoadPrefab").GetComponent<OnceLoad>();
            onceLoad_.SetLoadFlag(true);
            // �Q�[���J�n�������Ăяo��
            GameObject.Find("SceneMng").GetComponent<MenuActive>().DataLoad(false);
        }

        // WarpTown.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();
        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();

        if (str != "Mob")
        {
            // �L�����ɍ��W��������
            // WarpTown.cs��Start�֐����ォ��Ă΂�ăL�����̍��W�����������Ă��܂����琳�����ݒ�ł��Ȃ�
            // WarpTown.cs��Start�֐����ɌĂԂ悤�ɂ��悤!
            GameObject.Find("Uni").gameObject.transform.position = uniPosMap_[str];

            var temp = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
            temp.SetHouseVisible(str);

            // �����ŃT�u�J�����ɐ؂�ւ������̂ɂ��܂������Ȃ�
            // ���ォ��CameraMng.cs��Start�֐��ŃT�u�J������false�ɂ���Ă��邹��������
            // CameraMng.cs��Start�֐����Ȃ����āA�J�����̐ؑ֏����͂��ׂ�SetChangeCamera�֐��őΉ�����
            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
            cameraMng_.SetChangeCamera(true);

            // �����L�����o�X�̕\��(��\���̏�Ԃ���擾���K�v�ɂȂ�ׁASerializeField���g���ĊO���A�^�b�`���Ă���)
            //inHouseCanvas.gameObject.SetActive(true);
            //temp.ChangeObjectActive(inHouseCanvas.gameObject.transform.childCount, inHouseCanvas.transform, str);

            temp.SetWarpCanvasAndCharaController(false);

            // ���݂̌�������ۑ�(�J�����ʒu�����ɕK�v)
            temp.SetInHouseName(str);

            SceneMng.SetHouseName("Mob");
        }
        else
        {
            // ���C���J�������A�N�e�B�u�ɂ���
            cameraMng_.SetChangeCamera(false);
        }

        // �X�e�[�^�X�A�b�v�����������肷��
        if (!SceneMng.GetFinStatusUpTime())
        {
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].DeleteStatusUpByCook();
            }
        }
    }
}