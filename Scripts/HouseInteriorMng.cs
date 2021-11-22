using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInteriorMng : MonoBehaviour
{
    public Canvas warpCanvas;       // ���[�v��������L�����o�X(�\��/��\����؂�ւ��邽�߂Ɏg�p)
    public WarpTown warpTown;       // ���[�v�����̐ؑ�(enable�Ŏg�p)

    private CameraMng cameraMng_;   // �J�����֌W
    private UnitychanController playerController_;  // �L��������̐ؑ�

    private GameObject inHouseInfoCanvas_;  // �����ɓ��邩�̈ē����o��
    private GameObject iconImage_;          // [�͂�][������]�̌��ݑI�𒆂̕��ɖ��A�C�R�����o��
    private TMPro.TextMeshProUGUI text_;    // ������������

    private GameObject inHouseCanvas_;      // �����̑I����

    private bool inHouseFlg_ = true;        // �������邩(true:���� , false:����Ȃ�)

    private string nowInHouseName = "";     // �����錚���̖��O���ꎞ�I�ɕۑ�����
    private readonly string[] buildNameEng_ = { "UniHouse","MayorHouse","BookStore","ItemStore", "Guild" , "Restaurant" };  // ������(�q�G�����L�[�Ɠ����p��)
    private readonly string[] buildNameJpn_ = { "���j�̉�", "�����̉�" , "���X"    , "�����", "�M���h", "���X�g����" };  // ������(�\���p���{��)
    private Dictionary<string, string> buildNameMap_ = new Dictionary<string, string>();    // �L�[:�p�ꌚ����,�l:���{�ꌚ����

    private Dictionary<string, Func<bool>> func_ = new Dictionary<string, Func<bool>>();    // �L�[:�p�ꌚ����,�l:�C�x���g�����m�F�p��override�֐�


    private RectTransform menuRect_;
    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        menuRect_ = GameObject.Find("DontDestroyCanvas/Menu").GetComponent<RectTransform>();
        if (playerController_ == null)
        {
            playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        }

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
        iconImage_ = inHouseInfoCanvas_.transform.Find("Icon").gameObject;
        text_ = inHouseInfoCanvas_.transform.Find("HouseInfo/Text").GetComponent<TMPro.TextMeshProUGUI>();

        inHouseCanvas_ = this.transform.Find("InHouseCanvas").gameObject;

        // �p�ꌚ�����Ɠ��{�ꌚ������g�ݍ��킹��
        for (int i = 0; i < buildNameEng_.Length; i++)
        {
            buildNameMap_.Add(buildNameEng_[i], buildNameJpn_[i]);
        }

        // �����ɑS���̌����̃C�x���g�����m�F�֐���o�^���Ă���
        func_.Add("MayorHouse", new MayorHouse().CheckEvent);
        func_.Add("Guild"     , new Guild().CheckEvent);
        func_.Add("BookStore" , new BookStore().CheckEvent);
        func_.Add("ItemStore" , new ItemStore().CheckEvent);
        func_.Add("Restaurant", new Restaurant().CheckEvent);
        func_.Add("UniHouse"  , new UniHouse().CheckEvent);
    }

    public bool SetHouseVisible(string name)
    {
        if (!inHouseFlg_)
        {
            // �I�������u�͂��v�ɖ߂�
            inHouseFlg_ = true;
            // �A�C�R���ʒu���u�͂��v�ɖ߂�
            iconImage_.transform.localPosition = new Vector3(-140.0f, -70.0f, 0.0f);

            SetWarpCanvasAndCharaController(true);

            // �����������ɕK�����̊֐��͌Ă΂�邪�A
            // inHouseFlg_��false�Ȃ�K�v�ȏ���������return����
            SetActiveCanvas(false, "");

            return false;
        }

        // �����I�u�W�F�N�g�̕\��/��\���؂�ւ�
        ChangeObjectActive(this.gameObject.transform.childCount, this.transform,name);
        menuRect_.gameObject.SetActive(false);// �o�b�O���\���ɂ���

        return true;
    }

    // �����m�F�p�L�����o�X�̕\��/��\����؂�ւ���
    public void SetActiveCanvas(bool flag, string name)
    {
        inHouseInfoCanvas_.SetActive(flag);

        if (name == "")
        {
            return;
        }

        // �R���[�`���X�^�[�g  
        StartCoroutine(SelectInHouse(flag,name));
    }

    // �R���[�`��  
    private IEnumerator SelectInHouse(bool flag,string name)
    {
        SetWarpCanvasAndCharaController(false);
        // �C�x���g��Ԃ̊m�F
        string  tmpstr = "";
        tmpstr = EventMng.CheckEventHouse(name);
        if (tmpstr == "")
        {
            text_.text = buildNameMap_[name] + "�ɓ���H";
        }
        else
        {
            text_.text = "���́A"+ buildNameMap_[tmpstr] + "�֌��������I";
        }

        // �L�����o�X���\�����̊Ԃ̓R���[�`���������s��
        while (flag)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                iconImage_.transform.localPosition = new Vector3(40.0f, -70.0f, 0.0f);
                inHouseFlg_ = false;
                Debug.Log("�I�����u�������v");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                iconImage_.transform.localPosition = new Vector3(-140.0f, -70.0f, 0.0f);
                inHouseFlg_ = true;
                Debug.Log("�I�����u�͂��v");
            }
            else
            {
                // �����������s��Ȃ�
            }

            // �X�y�[�X�L�[�őI���������肵�Aflag��false�ɂ��邱�Ƃ�while�����甲����悤�ɂ���
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (tmpstr != "")
                {
                    inHouseFlg_ = false;
                }

                Debug.Log("�I�����̌���");
                flag = false;
            }
        }

        Debug.Log("�R���[�`���̏I��");

        // �Ƃ̃A�N�e�B�u/��A�N�e�B�u�̐؂�ւ�
        if (SetHouseVisible(name))
        {
            // ���݂��錚������ۑ����Ă���
            nowInHouseName = name;

            // �t�@���N�V�����ɓo�^���Ă������C�x���g�����m�F�֐����Ăяo��
            if (!func_[name]())
            {
                cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
                cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
                cameraMng_.SetChangeCamera(true);

                // �����L�����o�X�̕\��
                inHouseCanvas_.SetActive(true);

                // �����p�L�����o�X�̕\��/��\���؂�ւ�
                ChangeObjectActive(inHouseCanvas_.gameObject.transform.childCount, inHouseCanvas_.transform, name);
            }
        }
    }

    // �I�u�W�F�N�g�̕\��/��\���̏���
    public void ChangeObjectActive(int childNum, Transform trans, string buildName)
    {
        // �q�̐���for�����񂵂āA�����̖��O��v�͕\���A���O�s��v�͔�\���ɂ���
        for (int i = 0; i < childNum; i++)
        {
            var child = trans.GetChild(i);
            if (child.name != buildName)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    // ���[�v�L�����o�X�ƃL�����R���g���[���[�̐ݒ肪�ł���悤�ɂ���
    public void SetWarpCanvasAndCharaController(bool allFlg)
    {
        // ���[�v��L�����o�X�̕\���ؑ�
        warpCanvas.gameObject.SetActive(allFlg);

        // ���[�v�������~�߂邩�ǂ���
        warpTown.enabled = allFlg;

        // TownMng.cs����Ăяo���ꂽ���ɁA�܂��v���C���[�����擾���Ă��Ȃ�������擾����悤�ɂ���
        if (playerController_ == null)
        {
            playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        }

        if (!allFlg)
        {
            // �L�����A�j���[�V�������~�߂�
            playerController_.StopUniRunAnim();
        }

        // �L����������~�߂邩�ǂ���
        playerController_.enabled = allFlg;

    }

    // �O�����猻�݂��錚������ݒ�ł���悤�ɂ���
    public void SetInHouseName(string name)
    {
        nowInHouseName = name;
    }

    public string GetInHouseName()
    {
        return nowInHouseName;
    }

    // ��������̏o�������e�X�g
    public void ExitButton()
    {
        Debug.Log("�O�֏o��{�^����������܂���");

        // �����I�u�W�F�N�g�̔�\��(���O������""�ɂ��邱�ƂŁA�S�Ĕ�\���ɂł���)
        ChangeObjectActive(this.gameObject.transform.childCount, this.transform, "");

        // �����p�L�����o�X�̔�\��
        inHouseCanvas_.SetActive(false);

        // ���[�v��L�����o�X�̕\��
        warpCanvas.gameObject.SetActive(true);

        // ���[�v�������\�ɂ��邽�߂�true�ɖ߂�
        warpTown.enabled = true;

        // �L����������ĊJ����
        playerController_.enabled = true;

        // �����̖��O�ŏ����𕪂���
        if(nowInHouseName == "Guild" || nowInHouseName == "ItemStore")
        {
            // �E�ʘH�ʒu�ɃJ������߂�
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
            cameraMng_.SetSubCameraPos(new Vector3(28.0f, 3.0f, 89.0f));
        }
        else
        {
            // ���C���J�����ɖ߂�
            cameraMng_.SetChangeCamera(false);
        }


        //@ �V�KScript�Ŗ��O�擾Set���Ăяo���ꏊ
        QuestClearCheck.SetBuildName(nowInHouseName);

        // ���݂̌�������������
        nowInHouseName = "";

        // �o�b�O��\��
        menuRect_.gameObject.SetActive(true);
    }
}
