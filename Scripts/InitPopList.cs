using System.Collections.Generic;
using UnityEngine;

public class InitPopList : MonoBehaviour
{
    private static bool onceFlag_ = false;// ��x�����Ă΂��t���O
    private GameObject DataPopPrefab_;  // �e���X�g���Ăяo��
    private ItemList itemList_;         // �A�C�e����ExcelData�ǂݍ���
    private MateriaList materiaList_;   // �f�ނ�ExcelData�ǂݍ���
    private WordList wordList_;         // ���[�h��ExcelData�ǂݍ���

    // �A�C�e���֘A
    [SerializeField]
    private GameObject itemUIBox;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    private int maxItemCnt_ = 0;// �A�C�e���̍ő��
    public struct ItemData
    {
        public GameObject box;  // �C���X�^���X�����I�u�W�F�N�g��ۑ�
        public string name;     // �A�C�e����
        public int sellPrice;   // ����Ƃ��̒l�i
        public string materia1; // �A�C�e�������ŕK�v�ȑf��1
        public string materia2; // �A�C�e�������ŕK�v�ȑf��2
        public string materia3; // �A�C�e�������ŕK�v�ȑf��3
    }
    public static ItemData[] itemData;

    // �f�ފ֘A

    public enum FIELD_NUM
    {
        NON = -1,
        FOREST,
        FIELD1,
        FIELD2,
        FIELD3,
        FIELD4,
        MAX
    }
    private int fieldNumber_ = (int)FIELD_NUM.FOREST;  // ���݂���t�B�[���h�̔ԍ�

    [SerializeField]
    private GameObject materiaUIBox;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    private int maxMateriaCnt_ = 0;         //�z��̍ő�l��ۑ�
    private int singleMateriaCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��
    public static GameObject[] materiaBox_; // ���������v���n�u��ۑ�

    public struct MateriaData
    {
        public GameObject box;  // �C���X�^���X�����I�u�W�F�N�g��ۑ�
        public string name;     // �f�ނ̖��O
        public int sellPrice;   // ����Ƃ��̒l�i
        public int buyPrice;    // �������̒l�i
        public string info;     // �f�ސ���
    }
    public static MateriaData[] materiaData;

    // ���[�h�֘A
    public enum WORD
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT_HEAL,   // 1 ��
        ELEMENT_ASSIST, // 2 �⏕
        ELEMENT_ATTACK, // 3 �U���n
        TAIL,           // 4
        SUB1,           // 5 �����@�G
        SUB2,           // 6 HP ���@�U���@�����U���@�h��� ����/���
        SUB1_AND_SUB2,  // 7 �Ł@�ÈŁ@��Ⴡ@����
        SUB3,           // 8 �㏸�A�ቺ�A����
        ALL_SUB,         // 9�@�z���A�K��
        INFO,           // 10
        MAX
    }

    [SerializeField]
    private GameObject wordUIPleate;     // �����������I�u�W�F�N�g�̃v���n�u
    private int maxWordCnt_ = 0;
    public struct WordData
    {
        public GameObject pleate;   // �C���X�^���X�����I�u�W�F�N�g��ۑ�
        public string name;         // ���[�h��
        public int activeNum;       // �\�����Ă悢�X�g�[���[�̔ԍ�
        public WORD kinds;          // ���[�h�̎��
    }
    public static Dictionary<WORD, WordData[]> wordData = new Dictionary<WORD, WordData[]>();

    public static int[] maxWordCnt = new int[(int)WORD.INFO] ;


    void Awake()
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������

        for (int i = 0; i < (int)FIELD_NUM.MAX; i++)
        {
            // SceneMng�ɂ���enum��FIELD_NUM�̒l�����킹�邽�߂�nowScene-3
            if (((int)SceneMng.nowScene) - 3 == i)
            {
                fieldNumber_ = i;
                break;
            }
            else
            {
                fieldNumber_ = 0;
            }
        }

        if (onceFlag_ == false)
        {
            // Excel����A�C�e���̃f�[�^��ǂݍ���
            itemList_ = DataPopPrefab_.GetComponent<PopList>().GetData<ItemList>(PopList.ListData.ITEM);
            maxItemCnt_ = itemList_.param.Count;
            itemData = new ItemData[maxItemCnt_];
           // Debug.Log("�����Ăނ̂���邢" + maxItemCnt_);
            for (int i = 0; i < maxItemCnt_; i++)
            {
                itemData[i].box = Instantiate(itemUIBox,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                itemData[i].name = itemList_.param[i].ItemName;
                itemData[i].box.name = itemData[i].name;
                itemData[i].sellPrice = itemList_.param[i].Price_Sell;
                itemData[i].materia1 = itemList_.param[i].WantMateria1;
                itemData[i].materia2 = itemList_.param[i].WantMateria2;
                itemData[i].materia3 = itemList_.param[i].WantMateria3;
            }

            // Excel����f�ނ̃f�[�^��ǂݍ���
            materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, fieldNumber_);
            singleMateriaCnt_ = materiaList_.param.Count;
            maxMateriaCnt_ = (int)FIELD_NUM.MAX * singleMateriaCnt_;
            materiaData = new MateriaData[maxMateriaCnt_];

            int number = 0;
            for (int f = 0; f < (int)FIELD_NUM.MAX; f++)
            {
                materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, f);
                for (int i = 0; i < singleMateriaCnt_; i++)
                {
                    number = f * singleMateriaCnt_ + i;
                    materiaData[number].box = Instantiate(materiaUIBox,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                    materiaData[number].name = materiaList_.param[i].MateriaName;
                    materiaData[number].box.name = materiaData[number].name;
                    materiaData[number].buyPrice = materiaList_.param[i].Price_Buy;
                    materiaData[number].sellPrice = materiaList_.param[i].Price_Sell;
                }
            }

            // Excel���烏�[�h�̃f�[�^��ǂݍ���
            wordList_ = DataPopPrefab_.GetComponent<PopList>().GetData<WordList>(PopList.ListData.WORD);
            maxWordCnt_ = wordList_.param.Count;

            for (int k = 0; k < (int)WORD.INFO; k++)
            {
                for (int i = 0; i < maxWordCnt_; i++)
                {
                    if (k == wordList_.param[i].KindsNumber)
                    {
                        // �e���[�h��ނ̍ő�����m�F
                        maxWordCnt[k]++;                
                    }
                }
                //Debug.Log((InitPopList.WORD)k + "   InitPopList " + maxWordCnt[k]);
                wordData[(WORD)k] = test((WORD)k, maxWordCnt[k]);
            }

            onceFlag_ = true;
        }
    }

    public WordData[] test(WORD kind,int maxCnt)
    {
        // num��wordList�p�̔ԍ�
        var data = new WordData[maxCnt];
        int count = 0;
        for (int i = 0; i < maxWordCnt_; i++)
        {
            if ((int)kind == wordList_.param[i].KindsNumber)
            {
                data[count].name = wordList_.param[i].Word;
                data[count].pleate = Instantiate(wordUIPleate,
                       new Vector2(0, 0), Quaternion.identity, this.transform);
                data[count].pleate.name = data[count].name;
                //Debug.Log(num+ "            " + data[num].name);
                data[count].activeNum = wordList_.param[i].ListNumber;
                data[count].kinds= (WORD)wordList_.param[i].KindsNumber;
                count++;
            }
        }
        // Debug.Log(listNum+"��"+num+" "+kind);
        return data;
    }

    public int SetMaxMateriaCount()
    {
        return maxMateriaCnt_;
    }

    public int SetMaxItemCount()
    {
        return maxItemCnt_;
    }

    public int SetMaxWordCount()
    {
        return maxWordCnt_;
    }

    public int SetNowFieldMateriaList()
    {
        return fieldNumber_;
    }
}
