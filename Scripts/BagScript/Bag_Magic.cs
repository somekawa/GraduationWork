using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Magic : MonoBehaviour
{
    // �f�[�^�n
    private SaveCSV_Magic saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private const string saveDataFilePath_ = @"Assets/Resources/Save/magicData.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSV�̒��g�����郊�X�g;
    string[] texts;

    public enum ELEMENT_KIND
    {
        NON = -1,
        HEAL,   // 0 ��
        ASSIST,  // 1 �⏕
        FIRE, // 2 ��
        WATER, // 3 ��
        EARTH, // 4 �y
        WIND, // 5 ��
        MAX
    }
    public static string[] elementString = new string[(int)ELEMENT_KIND.MAX] {
    "��","�⏕","��","��","�y","��"};

    public struct MagicData
    {
        public int number;
        public string name;
        public string main;
        public string sub;
        public int power;
        public int rate;
        public int head;
        public int element;
        public int tail;
        public int sub1;
        public int sub2;
        public int sub3;
    }
    public static MagicData[] data = new MagicData[20];
   // public static MagicData[] saveData = new MagicData[20];
    public static Sprite[] magicSpite = new Sprite[20];

    // �o�b�O�\���p
    [SerializeField]
    private GameObject bagMagicUI;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private RectTransform bagMagicParent;// �A�C�e���{�b�N�X����m�F����Ƃ��̐e

    public static GameObject[] magicObject = new GameObject[20];
    private Image[] magicImage_ = new Image[20];

    // �X�e�[�^�X�\���p
    [SerializeField]
    private GameObject statusMagicUI;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private RectTransform statusMagicParent;// �X�e�[�^�X��ʂ��J�������̐e

    public static GameObject[] statusMagicObject = new GameObject[20];
    private Image[] statusMagicImage_ = new Image[20];
    private Button[] statusMagicBtn_ = new Button[20];

    // ����
    public static int number_ = 1;
    private int minNumber_ = 2;

    // ���@���̂Ă�֘A
    private int clickMagicNum_ = -1;
    private Button throwAwayBtn_;
    private Text info_; // �N���b�N�����A�C�e����������闓

    private List<Chara> charasList_ = new List<Chara>();
    public void Init()
    {
        if (magicObject[1] == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Magic>();
            charasList_ = SceneMng.charasList_;
            // �f�[�^�̌����Œ�l���傫��������f�[�^���Ă΂Ȃ�
            if (number_ < minNumber_)
            {
                return;
            }
            // Debug.Log(number_);
            for (int i = 0; i < number_; i++)
            {
                data[i].number = int.Parse(csvDatas[i + 1][0]);
                data[i].name = csvDatas[i + 1][1];
                data[i].main = csvDatas[i + 1][2];
                data[i].sub = csvDatas[i + 1][3];
                data[i].power = int.Parse(csvDatas[i + 1][4]);
                data[i].rate = int.Parse(csvDatas[i + 1][5]);
                data[i].head = int.Parse(csvDatas[i + 1][6]);
                data[i].element = int.Parse(csvDatas[i + 1][7]);
                data[i].tail = int.Parse(csvDatas[i + 1][8]);
                data[i].sub1 = int.Parse(csvDatas[i + 1][9]);
                data[i].sub2 = int.Parse(csvDatas[i + 1][10]);
                data[i].sub3 = int.Parse(csvDatas[i + 1][11]);

              //  Debug.Log(i + "�ԖځF" + data[i].name + "           " + data[i].element);
                if (i == 0)
                {
                    continue;
                }
                //  magicSpite[i]= ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                // data[i].battleSet0 = bool.Parse(csvDatas[i + 1][5]);

                // elementNum_[i] = minElementNum_ + data[i].element;
                // �o�b�O�p
                magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
                magicObject[i].name = "Magic" + data[i].number;
                magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                magicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];

                // �X�e�[�^�X�p�̍��W�ɕύX
                statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, statusMagicParent.transform);
                statusMagicObject[i].name = "Magic" + data[i].number;
                statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                statusMagicBtn_[i] = statusMagicObject[i].GetComponent<Button>();
                // Debug.Log(statusMagicBtn_[i].name);
                statusMagicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                //Debug.Log(data[i].name + "           " + data[i].number);
            }
        }
    }

    public void MagicCreateCheck(string magicName, string mainWords, string subWords, int pow, int rateNum,
                                  int head, int element, int tail, int sub1, int sub2, int sub3)
    {
        data[number_].number = number_;
        data[number_].name = magicName;
        data[number_].main = mainWords;
        data[number_].sub = subWords;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].head = head;
        data[number_].element = element;
        data[number_].tail = tail;
        data[number_].sub1 = sub1;
        data[number_].sub2 = sub2;
        data[number_].sub3 = sub3;
        //Debug.Log("�ۑ��������@" + data[number_].main + data[number_].sub);
      //  DataSave();

        // �o�b�O�p
        magicObject[number_] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
        magicObject[number_].name = "Magic_" + data[number_].number;
        magicImage_[number_] = magicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];
        //Debug.Log(data[number_].name + "  " + data[number_].power + "  " + data[number_].rate + "  " + data[number_].ability);

        // �X�e�[�^�X�p�̍��W�ɕύX
        statusMagicObject[number_] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                        Quaternion.identity, statusMagicParent.transform);
        statusMagicObject[number_].name = "Magic_" + data[number_].number;
        statusMagicImage_[number_] = statusMagicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];

        number_++;


        // ���ݎ󒍒��̃N�G�X�g������
        var orderList = QuestClearCheck.GetOrderQuestsList();

        for (int k = 0; k < orderList.Count; k++)
        {
            // ���P�̏��̃N�G�X�g�N���A�m�F
            // �󒍂����̂����}�e���A�̍����N�G�X�g(�ԍ���3)�̂Ƃ�
            if (int.Parse(orderList[k].Item1.name) == 3)
            {
                // Magic_�ԍ��̔ԍ������āA1�Ȃ�u���P�̏��v�̖��@�Ȃ̂ł��łɖ��@���擾���Ă��邩��N���A��Ԃɂ���
                for (int i = 1; i < magicObject.Length; i++)
                {
                    if (int.Parse(magicObject[i].name.Split('_')[1]) == 1)
                    {
                        // �N���A��Ԃɂ���
                        QuestClearCheck.QuestClear(3);
                        break;  // ����ȏ�for�����񂷕K�v���Ȃ����甲����
                    }
                }
            }
        }

    }

    public void SetStatusMagicCheck(int num, bool flag)
    {
        if (num < 1 || number_ < num)
        {
            // �͈͊O�̐��l�������Ă��ꍇ��return����
            return;
        }
        //Debug.Log(flag + "        ���ɑI������Ă��閂�@�̔ԍ��F" + num);
        //Debug.Log(statusMagicBtn_[num].name + "      " + statusMagicBtn_[num].interactable);


        // flag��true�Ȃ�interactable��false�ɂ���
        statusMagicBtn_[num].interactable = flag == true ? false : true;
    }


    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
        csvDatas.Clear();

        // �s���������ɂƂǂ߂�
        texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas.Add(texts[i].Split(','));
           // csvMagicDatas.Add(texts[i].Split(','));
        }
        Debug.Log("�f�[�^��" + csvDatas.Count);
        number_ = csvDatas.Count - 1;
        Init();
    }

    public void DataSave()
    {
        Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();
        // ���@�̌�����
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i]);
        }
        if (clickMagicNum_ != -1)
        {
            clickMagicNum_ = -1;
        }

        saveCsvSc_.SaveEnd();
    }


    public void SetMagicNumber(int num)
    {
        // �̂Ă�{�^����\��
        if (throwAwayBtn_ == null)
        {
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/MagicDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);
        throwAwayBtn_.interactable = true;

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            for (int m = 0; m < 3; m++)
            {
               // Debug.Log("�Z�b�g����Ă閂�@�ԍ�" + charasList_[i].GetMagicNum(m).number);
                if (charasList_[i].GetMagicNum(m).number==num)
                {
                    throwAwayBtn_.interactable = false;
                }
            }
        }

        // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
        clickMagicNum_ = num;
    }

    public void OnClickThrowAwayButton()
    {
        if (bagMagicParent.gameObject.activeSelf == false
         || statusMagicParent.gameObject.activeSelf == false)
        {
            //throwAwayBtn_.gameObject.SetActive(false);
            return;
        }
        Debug.Log("�w�肷�閂�@�ԍ�"+clickMagicNum_);
        // �w��̖��@���폜����
        magicObject[clickMagicNum_].SetActive(false);
        statusMagicObject[clickMagicNum_].SetActive(false);
        throwAwayBtn_.gameObject.SetActive(false);
        info_.text = "";

        int dataNumber = 0;
        for (int i = 1; i < number_; i++)
        {
            
            if(i==clickMagicNum_)
            {
                Debug.Log("�폜���閂�@�̖��O�F" + data[i].name);
                // �I���������@�͓ǂݍ��܂Ȃ�
                continue;
            }
            dataNumber++;// �ۑ����̔ԍ������炷
            Debug.Log(data[dataNumber].name + "       " + data[i].name);
            data[dataNumber] = data[i];
            data[dataNumber].number = dataNumber;

            // �o�b�O�p
            magicObject[dataNumber] = magicObject[i];
            magicObject[dataNumber].name = "Magic" + data[dataNumber].number;
            magicImage_[dataNumber] = magicObject[dataNumber].transform.Find("MagicIcon").GetComponent<Image>();
            magicImage_[dataNumber].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[dataNumber].element];

            // �X�e�[�^�X�p
            statusMagicObject[dataNumber] = statusMagicObject[i];
            statusMagicObject[dataNumber].name = "Magic" + data[dataNumber].number;
            statusMagicImage_[dataNumber] = statusMagicObject[dataNumber].transform.Find("MagicIcon").GetComponent<Image>();
            statusMagicBtn_[dataNumber] = statusMagicObject[dataNumber].GetComponent<Button>();
            statusMagicImage_[dataNumber].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[dataNumber].element];

        }

        number_ = dataNumber;

    }
}