using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    // �f�[�^�n
    private SaveCSV_Materia saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private const string saveDataFilePath_ = @"Assets/Resources/materiaData.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSV�̒��g�����郊�X�g;

    private InitPopList popMateriaList_;

    [SerializeField]
    private RectTransform materiaParent;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    public struct MateriaData
    {
        public int number;
        public GameObject box;  // �f�ނ̃I�u�W�F�N�g
        public Image image;     // �f�ނ̉摜
        public Text cntText;    // �����Ă���f�ނ̌���\��
        public int haveCnt;     // �w��f�ނ̎����Ă��
        public string name;     // �f�ނ̖��O
        public string info;
        public bool getFlag;    // 1�ȏ㎝���Ă��邩
    }
    public static MateriaData[] materiaState;
    public static MateriaData[] data;

    // ��Script�Ŏw�肷�郏�[�h�͔ԍ����擾���Ă���
    public static int emptyMateriaNum;// ��̃}�e���A�̔ԍ�

    // �v���n�u���琶�����ꂽ�I�u�W�F�N�g����
    private int maxCnt_ = 0;            // ���ׂĂ̑f�ސ�

    private int maxHaveCnt_ = 99;// �w��f�ނ̏��������

    private int clickMateriaNum_ = -1;
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button throwAwayBtn_;

    private static bool newGameFlag_ = false;
    public void NewGameInit()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Materia>();
        // �^�C�g����NewGame�{�^�����������Ƃ��ɌĂ΂��Init
        newGameFlag_ = true;
        maxCnt_ = popMateriaList_.SetMaxMateriaCount();
        data = new MateriaData[maxCnt_];
        for (int i = 0; i < maxCnt_; i++)
        {
            data[i].number = i;
            data[i].name = InitPopList.materiaData[i].name;
            data[i].haveCnt = 0;
        }
        DataSave();
        DataLoad();
    }

    public void Init()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Materia>();        

        if (maxCnt_ == 0|| newGameFlag_ ==true)
        {
            maxCnt_ = popMateriaList_.SetMaxMateriaCount();
            materiaState = new MateriaData[maxCnt_];
            data = new MateriaData[maxCnt_];
            newGameFlag_ = false;
            Debug.Log(maxCnt_+"      "+materiaParent.transform);
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i] = new MateriaData
                {
                    box = InitPopList.materiaData[i].box,
                    name = InitPopList.materiaData[i].name,
                    info = InitPopList.materiaData[i].info,
                    getFlag = false,
                    number = int.Parse(csvDatas_[i + 1][0]),
                    haveCnt = int.Parse(csvDatas_[i + 1][2]),
                };
             //   Debug.Log(i + "�Ԗڂ̃A�C�e��" + materiaState[i].name + "�̏�����" + materiaState[i].haveCnt);

                materiaState[i].box.transform.SetParent(materiaParent.transform);
                // ���O�ɔԍ������đf�ރN���b�N���ɏ����擾���₷������
                materiaState[i].box.name = materiaState[i].name + i;

                // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
                materiaState[i].image = materiaState[i].box.transform.Find("MateriaIcon").GetComponent<Image>();
                materiaState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];

                // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
                materiaState[i].cntText = materiaState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
                materiaState[i].cntText.text = materiaState[i].haveCnt.ToString();

                if (materiaState[i].name == "��̃}�e���A")
                {
                    emptyMateriaNum = i;
                }

                materiaState[i].getFlag = 0 < materiaState[i].haveCnt ? true : false;
                materiaState[i].box.SetActive(materiaState[i].getFlag);
              //  Debug.Log(i + "       " + materiaState[i].getFlag);
            }
        }
        else
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].cntText.text = materiaState[i].haveCnt.ToString();
                materiaState[i].getFlag = 0 < materiaState[i].haveCnt ? true : false;
                materiaState[i].box.SetActive(materiaState[i].getFlag);
            }
        }

        if (materiaState[0].box.transform.parent != materiaParent.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].box.transform.SetParent(materiaParent.transform);
            }
        }
    }

    public void MateriaGetCheck(int materiaNum, int getCnt)
    {
        materiaState[materiaNum].getFlag = true;
        if (getCnt < 0)
        {
            // ��������ꍇ�͌����D�悷��
            materiaState[materiaNum].haveCnt += getCnt;
            // ��������0�ȉ��ɂȂ�����0�����Ă����B�Ⴄ�Ȃ炻�̂܂�
            materiaState[materiaNum].haveCnt = materiaState[materiaNum].haveCnt <= 0 ? 0 : materiaState[materiaNum].haveCnt;
        }
        else
        {
            // �ő及������99��                // �Ă΂ꂽ�f�ޔԍ��̑f�ނ̏����������Z
            materiaState[materiaNum].haveCnt = maxHaveCnt_ <= materiaState[materiaNum].haveCnt ? maxHaveCnt_ : materiaState[materiaNum].haveCnt + getCnt;
        }
       // Debug.Log(materiaState[materiaNum].haveCnt + "     �������F" + getCnt);

        if (materiaState[materiaNum].haveCnt < 1)
        {
            // ��������1�ȉ��Ȃ��\����
            materiaState[materiaNum].box.SetActive(false);
        }
        else
        {
            // 1�ł������Ă�����\������
            materiaState[materiaNum].box.SetActive(true);
        }
        materiaState[materiaNum].cntText.text = materiaState[materiaNum].haveCnt.ToString();

        data[materiaNum].haveCnt = materiaState[materiaNum].haveCnt;
    }

    public int GetMaxHaveMateriaCnt()
    {
        return materiaParent.transform.childCount;
    }

    public void SetMateriaNumber(int num)
    {
        // �̂Ă�{�^����\��
        if (throwAwayBtn_ == null)
        {
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/MateriaDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);

        // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
        clickMateriaNum_ = num;
    }

    public void OnClickThrowAwayButton()
    {
        if (materiaParent.gameObject.activeSelf == false)
        {
            //materiaState[clickMateriaNum_].box.SetActive(false);
            return;
        }

        // �̂Ă�{�^���������ꂽ�ꍇ
        materiaState[clickMateriaNum_].haveCnt--;// ��������1���炷
                                                 // �\�����̏��������X�V
        materiaState[clickMateriaNum_].cntText.text = materiaState[clickMateriaNum_].haveCnt.ToString();
        if (materiaState[clickMateriaNum_].haveCnt < 1)
        {
            // ��������0�ɂȂ������\���ɂ���
            materiaState[clickMateriaNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
        data[clickMateriaNum_].haveCnt = materiaState[clickMateriaNum_].haveCnt;
    }

    public void DataLoad()
    {
        // Debug.Log("���[�h���܂�");

        csvDatas_.Clear();

        // �s���������ɂƂǂ߂�
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas_.Add(texts[i].Split(','));
        }
        Init();
    }

    public void DataSave()
    {
        //  Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveMateriaData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

}