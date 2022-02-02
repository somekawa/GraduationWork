using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bag_Word : MonoBehaviour
{
    // �f�[�^�n
    private Word_SaveCSV saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private string saveDataFilePath_;
    List<string[]> csvDatas_ = new List<string[]>(); // CSV�̒��g�����郊�X�g;


    private InitPopList popWordList_;
    private int[] maxCnt_ = new int[(int)InitPopList.WORD.INFO];
    [SerializeField]
    private RectTransform wordParent;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private Button headBtn;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    public enum WORD_MNG
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT,        // 1 
        TAIL,           // 2
        SUB1,           // 3 
        SUB2,           // 4 
        SUB3,           // 5 
        MAX
    }

    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private Button kindsBtn_;
    private string[] btnName = new string[(int)WORD_MNG.SUB2] {
    "Head","Element","Tail","Sub"
    };

    public struct WordData
    {
        public int number;// �Z�[�u����ۂ̔ԍ�
        public string name;// ���[�h��
        public int getFlag;// 0��false�Ŏ擾���ĂȂ��B1��true�Ŏ擾���Ă���
        public GameObject pleate;
        public TMPro.TextMeshProUGUI nameText;
        public Button btn;
        public InitPopList.WORD kinds;
        public int power;
        public int MP;
        public int maxCnt;
    }
    public static Dictionary<InitPopList.WORD, WordData[]> wordState = new Dictionary<InitPopList.WORD, WordData[]>();
    public static WordData[] data;// = new  WordData[29];
    private int[] kindWordCnt_ = new int[(int)WORD_MNG.SUB1];

    // ��Script�Ŏw�肷�郏�[�h�͔ԍ����擾���Ă���
    public static int targetWordNum;            // �K���p�̔ԍ�
    public static bool getFlagCheck = false;    // ���g�𖂖@�쐬�̎��ɓn������g����

    public void NewGameInit()
    {
        // �^�C�g����NewGame�{�^�����������Ƃ��ɌĂ΂��Init
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<Word_SaveCSV>();
        data = new WordData[29];
        int dataNum = 0;
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            maxCnt_[k] = InitPopList.maxWordKindsCnt[k];
            //wordState[(InitPopList.WORD)k] = InitWordState((InitPopList.WORD)k, maxCnt_[k]);
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                data[dataNum].number = dataNum;
                data[dataNum].name = InitPopList.name[k, i];
                data[dataNum].getFlag = 0;
                dataNum++;
            }
        }
        DataSave();
        //DataLoad();
    }

    public void Init()
    {
        eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<Word_SaveCSV>();

        if (getFlagCheck == false)
        {
            int dataNum = 0;
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                maxCnt_[k] = InitPopList.maxWordKindsCnt[k];
                wordState[(InitPopList.WORD)k] = InitWordState((InitPopList.WORD)k, maxCnt_[k], dataNum);
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    data[dataNum] = wordState[(InitPopList.WORD)k][i];
                    //Debug.Log(dataNum + "�Ԗڂ̃��[�h��" + data[dataNum].name);
                    data[dataNum].number = dataNum;
                    dataNum++;
                }
            }
            getFlagCheck = true;
        }
        // ��ԏ��߂ɏo�Ă��郏�[�h�̕\���ʒu���������A�\���ʒu���o�b�O�ɂ���
        if (wordState[InitPopList.WORD.HEAD][0].pleate.transform.parent != wordParent.transform)
        {
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    // �o�b�O���̐e�ɕύX��
                    wordState[(InitPopList.WORD)k][i].pleate.transform.SetParent(wordParent.transform);
                    // �N���b�N�ł��Ȃ���Ԃɂ���
                    wordState[(InitPopList.WORD)k][i].btn.enabled = false ;
                    if (wordState[(InitPopList.WORD)k][i].name == "�K��")
                    {
                        targetWordNum = i;
                        //Debug.Log((InitPopList.WORD)k + "�̒��ŕK����ۑ����Ă���ԍ��F"+targetWordNum);
                    }
                }
            }
        }

        if (wordState[InitPopList.WORD.HEAD][0].btn == null)
        {
            int dataNum = 0;
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    wordState[(InitPopList.WORD)k][i] = data[dataNum];
                    dataNum++;
                }
            }
        }
        ActiveCheck(WORD_MNG.HEAD);
        if(kindsBtn_ != null)
        {
            // ����Head�ȊO���I������Ă����ꍇ���̃{�^���������\�ɂ���
            kindsBtn_.interactable = true;
        }
        //WordGetCheck(InitPopList.WORD.HEAD, 2, 2);// �S��
        //WordGetCheck(InitPopList.WORD.HEAD, 1, 1);// ������
        //WordGetCheck(InitPopList.WORD.ELEMENT_ASSIST, 0, 4);// �⏕
        //WordGetCheck(InitPopList.WORD.SUB1, 0, 13);// ����
        //WordGetCheck(InitPopList.WORD.SUB1_AND_SUB2, 0, 20);// ��
        //WordGetCheck(InitPopList.WORD.SUB1_AND_SUB2, 3, 23);// ����
        //WordGetCheck(InitPopList.WORD.SUB2, 1, 16);// �����U����
        //WordGetCheck(InitPopList.WORD.SUB2, 2, 17);// ���@�U����
                                                   //WordGetCheck(InitPopList.WORD.SUB2, 4, 19);// ����/���
                                                   //WordGetCheck(InitPopList.WORD.SUB2, 3, 18);// �h���
                                                   // WordGetCheck(InitPopList.WORD.ALL_SUB, 1, 28);// �K��
        kindsBtn_ = headBtn;
        kindsBtn_.interactable = false;
    }

    private WordData[] InitWordState(InitPopList.WORD kind, int maxCnt,int dataNum)
    {
        int csvNum = dataNum;
        var word = new WordData[maxCnt];
        for (int i = 0; i < maxCnt; i++)
        {
            // ���[�h�����擾
            word[i].name = InitPopList.name[(int)kind, i];

            // �I�u�W�F�N�g�����擾
            word[i].pleate = InitPopList.pleate[(int)kind, i];
            word[i].pleate.name = word[i].name;// +i;

            // �I�u�W�F�N�g�̎q��text���擾
            word[i].nameText = word[i].pleate.transform.Find("Word").GetComponent<TMPro.TextMeshProUGUI>();
            word[i].nameText.text = word[i].name;

            // �I�u�W�F�N�g�̃R���|�[�l���g�ɂ���button���擾
            word[i].btn = word[i].pleate.GetComponent<Button>();

            // �w��̃��[�h���擾���Ă��邩
            Debug.Log(csvNum + i + "�Ԗڂ�" + csvDatas_[csvNum + 1 + i][1]);
            data[dataNum].number = csvNum + i;
            word[i].getFlag = int.Parse(csvDatas_[csvNum + 1 + i][2]);
            word[i].kinds = InitPopList.kinds[(int)kind, i];
            word[i].power = InitPopList.power[(int)kind, i];
            word[i].MP = InitPopList.MP[(int)kind, i];
            word[i].maxCnt = maxCnt;
            Debug.Log("BagWord���� " + kind + "��" + i + "�Ԗڂ�" + word[i].name);
        }
        return word;
    }

    public void WordGetCheck(InitPopList.WORD kinds, int wordNum, int dataNum)
    {
        Debug.Log(kinds + "��" + wordNum + " " + dataNum);
        Debug.Log(kinds + "��" + wordNum + "�Ԗڂ̃��[�h�F" + wordState[kinds][wordNum].name);
        Debug.Log(data[dataNum].name + "��" + dataNum + "�Ԗڂł�");
        wordState[kinds][wordNum].getFlag = 1;// �擾��������1������;

        data[dataNum].getFlag = wordState[kinds][wordNum].getFlag;
        Debug.Log(data[dataNum].name +""+data[dataNum].getFlag);
    }

    public void OnClickWordKindsBtn()
    {
        // Debug.Log(eventSystem_.currentSelectedGameObject + "���N���b�N");
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        kindsBtn_.interactable = true;// 1�O�ɑI�����Ă����{�^���͉����\��
        // ���񉟂����̂͂ǂ̃{�^����
        kindsBtn_ = clickbtn_.GetComponent<Button>();
        kindsBtn_.interactable = false;
        // �ǂ̎�ނ̃{�^������������
        for (int i = 0; i < (int)WORD_MNG.SUB2; i++)
        {
            if (clickbtn_.name == btnName[i])
            {
                ActiveCheck((WORD_MNG)i);
            }
        }
    }

    public void SetGetFlagCheck(bool flag)
    {
        getFlagCheck = flag;
    }

    private void ActiveCheck(WORD_MNG kinds)
    {
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                // ���ׂĔ�\���ɂ��Ă���
                wordState[(InitPopList.WORD)k][i].pleate.SetActive(false);
            }
        }

        switch (kinds)
        {
            case WORD_MNG.HEAD:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.HEAD]; i++)
                {
                    if (data[i].getFlag == 0)
                    {
                        continue;
                    }
                    wordState[(int)InitPopList.WORD.HEAD][i].pleate.SetActive(true);
                }
                break;

            case WORD_MNG.ELEMENT:
                for (int k = (int)InitPopList.WORD.ELEMENT_HEAL; k < (int)InitPopList.WORD.TAIL; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        if (wordState[(InitPopList.WORD)k][i].getFlag == 0)
                        {
                            continue;
                        }
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            case WORD_MNG.TAIL:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.TAIL]; i++)
                {
                    if (wordState[InitPopList.WORD.TAIL][i].getFlag == 0)
                    {
                        continue;
                    }
                    wordState[InitPopList.WORD.TAIL][i].pleate.SetActive(true);
                }
                break;

            case WORD_MNG.SUB1:
                for (int k = (int)InitPopList.WORD.SUB1; k < (int)InitPopList.WORD.INFO; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        if (wordState[(InitPopList.WORD)k][i].getFlag == 0)
                        {
                            continue;
                        }
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            default:
                break;
        }
    }

    public void DataLoad()
    {
        // Debug.Log("���[�h���܂�");
        saveDataFilePath_ = Application.streamingAssetsPath + "/Save/wordData.csv";

        csvDatas_.Clear();

        // �s���������ɂƂǂ߂�
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas_.Add(texts[i].Split(','));
        }
        data = new WordData[csvDatas_.Count];
        Init();
    }

    public void DataSave()
    {
        saveCsvSc_.SaveStart();
        int dataNum = 0;
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                saveCsvSc_.SaveWordData(data[dataNum]);
                dataNum++;
            }
        }
        saveCsvSc_.SaveEnd();
    }
}