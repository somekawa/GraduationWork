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

    //public enum HEAD_KIND
    //{
    //    NON = -1,
    //    SINGLE, // 0 �P��
    //    MULTIPLE,// 1 ������
    //    OVERALL,// 2 �S��
    //    MAX
    //}
    //public static string[] headString = new string[(int)HEAD_KIND.MAX] {
    //"�P��","������","�S��"};

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

    //public enum TAIL_KIND
    //{
    //    NON = -1,
    //    SMALL, // 0 ��
    //    MEDIUM,// 1 ��
    //    LARGE,// 2 ��
    //    MAXMUM,// 3 �ɑ�
    //    MAX
    //}
    //public static string[] tailString = new string[(int)TAIL_KIND.MAX] {
    //"��","��","��","�ɑ�"};
    //public enum SUB1_KIND
    //{
    //    NON = -1,
    //    FELLOW,//����
    //    ENEMY,//�G
    //    POISON,//��
    //    DARK,//�È�
    //    PARALYSIS,// ���
    //    DEATH,//����
    //    SUCTION,//�z��
    //    TARGET,//�K��
    //    MAX
    //}
    //public static string[] sub1String_ = new string[(int)SUB1_KIND.MAX] {
    //"����","�G","��","�È�","���","����","�z��","�K��"};

    //public enum SUB2_KIND
    //{
    //    NON = -1,
    //    HP,//HP
    //    MAGIC_ATTACK,// ���@�U��
    //    PHYSICS_ATTACK,//�����U��
    //    DEFENCE,//�h���
    //    SPEED,//����/���
    //    POISON,//��
    //    DARK,//�È�
    //    PARALYSIS,// ���
    //    DEATH,//����
    //    SUCTION,//�z��
    //    TARGET,//�K��
    //    MAX
    //}
    //public static string[] sub2String_ = new string[(int)SUB2_KIND.MAX] {
    //"HP","���@�U��","�����U��","�h���","����/���","��","�È�","���","����","�z��","�K��"};

    //public enum SUB3_KIND
    //{
    //    NON = -1,
    //    UP,//�㏸�A
    //    DOWN,//�ቺ�A
    //    REFLECTION,//����
    //    SUCTION,//�z��
    //    TARGET,//�K��
    //    MAX
    //}
    //public static string[] sub3String_ = new string[(int)SUB3_KIND.MAX] {
    //"�㏸","�ቺ","����","�z��","�K��"};



    public struct MagicData
    {
        public int number;
        public string name;
        public int power;
        public int rate;
        public int head;
        public int element;
        public int tail;
        public int sub1;
        public int sub2;
        public int sub3;
      //  public Sprite sprite;
       // public int imageNum;
    }
    public static MagicData[] data = new MagicData[50];
    public static Sprite[] magicSpite = new Sprite[50];

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
    public static int number_ = 1;
    //private int minElementNum_ = 34;
  //  public static int[] elementNum_ = new int[50];

    public void Init()
    {
        if (magicObject[0] == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_Magic>();
            if (0 < number_)
            {
               // Debug.Log(number_);
                for (int i = 1; i < number_; i++)
                {
                    data[i].number = int.Parse(csvDatas[i + 1][0]);
                    data[i].name = csvDatas[i + 1][1];
                    data[i].power = int.Parse(csvDatas[i + 1][2]);
                    data[i].rate = int.Parse(csvDatas[i + 1][3]);
                    data[i].head = int.Parse(csvDatas[i + 1][4]);
                    data[i].element = int.Parse(csvDatas[i + 1][5]);
                    data[i].tail = int.Parse(csvDatas[i + 1][6]);
                    data[i].sub1 = int.Parse(csvDatas[i + 1][7]);
                    data[i].sub2 = int.Parse(csvDatas[i + 1][8]);
                    data[i].sub3 = int.Parse(csvDatas[i + 1][9]);

                    //  magicSpite[i]= ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                    Debug.Log(i+"�ԖځF"+data[i].name + "           "+ data[i].element);
                    // data[i].battleSet0 = bool.Parse(csvDatas[i + 1][5]);

                    // elementNum_[i] = minElementNum_ + data[i].element;
                    // �o�b�O�p
                    magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, bagMagicParent.transform);
                    magicObject[i].name = "Magic_" + data[i].number;
                    magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    magicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];

                    // �X�e�[�^�X�p�̍��W�ɕύX
                    statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                    Quaternion.identity, statusMagicParent.transform);
                    statusMagicObject[i].name = "Magic_" + data[i].number;
                    statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    statusMagicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                    //Debug.Log(data[i].name + "           " + data[i].number);
                }
            }
        }
    }

    public void MagicCreateCheck(string magicName, int pow, int rateNum, 
                                  int head, int element, int tail, int sub1, int sub2,int sub3)
    {
        data[number_].number = number_;
        data[number_].name = magicName;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].head = head;
        data[number_].element = element;
        data[number_].tail = tail;
        data[number_].sub1 = sub1;
        data[number_].sub2 = sub2;
        data[number_].sub3 = sub3;
        //data[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];
        DataSave();
        
        // �o�b�O�p
        magicObject[number_] = Instantiate(bagMagicUI,new Vector2(0, 0),
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
        Debug.Log("�f�[�^��" + csvDatas.Count);
        number_ = csvDatas.Count-1;

        //// ���@�̌�����
        //for (int i = 1; i < number_; i++)
        //{
        //    MagicData set = new MagicData
        //    {
        //        number = int.Parse(csvDatas[i + 1][0]),
        //        name = csvDatas[i + 1][1],
        //        power = int.Parse(csvDatas[i + 1][2]),
        //        rate = int.Parse(csvDatas[i + 1][3]),
        //        head = int.Parse(csvDatas[i + 1][4]),
        //        element = int.Parse(csvDatas[i + 1][5]),
        //        tail = int.Parse(csvDatas[i + 1][6]),
        //        sub1 = int.Parse(csvDatas[i + 1][7]),
        //        sub2 = int.Parse(csvDatas[i + 1][8]),
        //        sub3 = int.Parse(csvDatas[i + 1][9]),
        //        // sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][int.Parse(csvDatas[i + 1][9])],
        //        // imageNum = int.Parse(csvDatas[i + 1][9]),
        //    };
       // }
        Init();
    }

    private void DataSave()
    {
        Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i+1]);
            Debug.Log(data[i].name+"��ۑ����܂�");
        }
        saveCsvSc_.SaveEnd();
    }
}