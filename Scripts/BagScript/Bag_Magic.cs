using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Magic : MonoBehaviour
{
    // �f�[�^�n
    private SaveCSV_Magic saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private const string saveDataFilePath_ = @"Assets/Resources/magicData.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSV�̒��g�����郊�X�g;

    public enum ELEMENT_KIND
    {
        NON=-1,
        FIRE, // 0 ��
        HEAL,   // 1 ��
        WATER, // 2 ��
        ASSIST,  // 3 �⏕
        EARTH, // 4 �y
        WIND, // 5 ��
        DRAGON, // 6 ��
        MAX
    }
    public static string[] elementString = new string[(int)ELEMENT_KIND.MAX] {
    "��","��","��","�⏕","�y","��","��"};

    public struct MagicData
    {
        public string name;
        public int power;
        public int rate;
        public int ability;
        public int element;
        public bool setNumber;
    }
    public static MagicData[] data = new MagicData[50];

    // �o�b�O�\���p
    [SerializeField]
    private GameObject bagMagicUI;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private RectTransform bagMagicParent;// �A�C�e���{�b�N�X����m�F����Ƃ��̐e

    public static GameObject[] magicObject = new GameObject[50];
    private Image[] magicImage_ = new Image[50];
    
    // �X�e�[�^�X�\���p
    [SerializeField]
    private GameObject statusMagicUI;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private RectTransform statusMagicParent;// �X�e�[�^�X��ʂ��J�������̐e
   
    public static GameObject[] statusMagicObject = new GameObject[50];
    private Image[] statusMagicImage_ = new Image[50];

    // ����
    public static int number_ = 0;
    private int minElementNum_ = 34;
    public static int[] elementNum_ = new int[50];

    public void Init()
    {
        if (magicObject[0] == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_Magic>();
            if (0 < number_)
            {
               // Debug.Log(number_);
                for (int i = 0; i < number_; i++)
                {
                    data[i].name = csvDatas[i + 1][0];
                    data[i].power = int.Parse(csvDatas[i + 1][1]);
                    data[i].ability = int.Parse(csvDatas[i + 1][2]);
                    data[i].rate = int.Parse(csvDatas[i + 1][3]);
                    data[i].element = int.Parse(csvDatas[i + 1][4]);
                    data[i].setNumber = bool.Parse(csvDatas[i + 1][5]);

                    elementNum_[i] = minElementNum_ + data[i].element;
                    Debug.Log(elementNum_[i] + "            �c��" + data[i].power);
                    // �o�b�O�p
                    magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, bagMagicParent.transform);
                    magicObject[i].name = "Magic_" + i;
                    magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    magicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[i]];
                   
                    // �X�e�[�^�X�p�̍��W�ɕύX
                    statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                    Quaternion.identity, statusMagicParent.transform);
                    statusMagicObject[i].name = "Magic_" + i;
                    statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    statusMagicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[i]];
                }
            }
        }
    }

    public void MagicCreateCheck(string magic, int pow, int rateNum, int abilityNum,int element)
    {
        data[number_].name = magic;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].ability = abilityNum;
        data[number_].element = element;
        data[number_].setNumber = false;// ���ꂽ�����̂���
        DataSave();

        elementNum_[number_] = minElementNum_ + data[number_].element;
        
        // �o�b�O�p
        magicObject[number_] = Instantiate(bagMagicUI,new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
        magicObject[number_].name = "Magic_" + number_;
        magicImage_[number_] = magicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[number_]];
        //Debug.Log(data[number_].name + "  " + data[number_].power + "  " + data[number_].rate + "  " + data[number_].ability);
        
        // �X�e�[�^�X�p�̍��W�ɕύX
        statusMagicObject[number_] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                        Quaternion.identity, statusMagicParent.transform);
        statusMagicObject[number_].name = "Magic_" + number_;
        statusMagicImage_[number_] = statusMagicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[number_]];
       
        number_++;
    }

    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
     //   Debug.Log("���[�h���܂�");

        csvDatas.Clear();

        // �s���������ɂƂǂ߂�
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas.Add(texts[i].Split(','));
        }
       // Debug.Log("�f�[�^��" + csvDatas.Count);
        number_ = csvDatas.Count - 1;

        // ���@�̌�����
        for (int i = 1; i < number_; i++)
        {
            MagicData set = new MagicData
            {
                name = csvDatas[i + 1][0],
                power = int.Parse(csvDatas[i + 1][1]),
                ability = int.Parse(csvDatas[i + 1][2]),
                rate = int.Parse(csvDatas[i + 1][3]),
                element = int.Parse(csvDatas[i + 1][4]),
                setNumber = bool.Parse(csvDatas[i + 1][5]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        //  Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i]);
         //   Debug.Log(data[i].name);
        }
        saveCsvSc_.SaveEnd();
    }

    public void SetMagicCheck(int num,bool flag)
    {
        data[num].setNumber = flag;
        DataSave();
    }

}