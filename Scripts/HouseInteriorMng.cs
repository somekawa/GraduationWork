using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private MayorHouse mayorHouse_;

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
        iconImage_ = inHouseInfoCanvas_.transform.Find("Icon").gameObject;
        text_ = inHouseInfoCanvas_.transform.Find("HouseInfo/Text").GetComponent<TMPro.TextMeshProUGUI>();

        inHouseCanvas_ = this.transform.Find("InHouseCanvas").gameObject;

        // �p�ꌚ�����Ɠ��{�ꌚ������g�ݍ��킹��
        for (int i = 0; i < buildNameEng_.Length; i++)
        {
            buildNameMap_.Add(buildNameEng_[i], buildNameJpn_[i]);
        }

        mayorHouse_ = this.transform.Find("MayorHouse").GetComponent<MayorHouse>();
    }

    public bool SetHouseVisible(string name)
    {
        if (!inHouseFlg_)
        {
            // �I�������u�͂��v�ɖ߂�
            inHouseFlg_ = true;
            // �A�C�R���ʒu���u�͂��v�ɖ߂�
            iconImage_.transform.localPosition = new Vector3(-140.0f, -70.0f, 0.0f);

            // ���[�v��L�����o�X�̕\��
            warpCanvas.gameObject.SetActive(true);

            // ���[�v�������\�ɂ��邽�߂�true�ɖ߂�
            warpTown.enabled = true;

            // �����������ɕK�����̊֐��͌Ă΂�邪�A
            // inHouseFlg_��false�Ȃ�K�v�ȏ���������return����
            SetActiveCanvas(false, "");

            // �L����������ĊJ����
            playerController_.enabled = true;
            return false;
        }

        // �����I�u�W�F�N�g�̕\��/��\���؂�ւ�
        ChangeObjectActive(this.gameObject.transform.childCount, this.transform,name);

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
        // ���[�v��L�����o�X�̔�\��
        warpCanvas.gameObject.SetActive(false);
        // ���[�v�����̃X�y�[�X�L�[���~�߂邽�߂�false
        warpTown.enabled = false;

        // �L�����A�j���[�V�������~�߂�
        playerController_.StopUniRunAnim();

        // �L����������~�߂�
        playerController_.enabled = false;

        text_.text = buildNameMap_[name] + "�ɓ���H";

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

            //@ �ǂ̃X�N���v�g�ł��Ή��ł���悤�ɁAmayorHouse_�̕����������I�ɂ�map�ɂ���
            if (!mayorHouse_.CheckEventMayorHouse())
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
    private void ChangeObjectActive(int childNum, Transform trans, string buildName)
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

    // ��������̏o�������e�X�g
    public void ExitButton()
    {
        Debug.Log("�O�֏o��{�^����������܂���");

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

        // ���݂̌�������������
        nowInHouseName = "";
    }
}
