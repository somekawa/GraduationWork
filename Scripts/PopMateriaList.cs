using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopMateriaList : MonoBehaviour
{
    private static bool onceFlag_ = false;// ��x�����Ă΂��t���O

    // �o�b�O�̒��g�Ɛ}��
    [SerializeField]
    private GameObject materiaUIPrefab;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    private MateriaList materiaList_;   // �f�ނ�ExcelData�ǂݍ���
    private int maxMateriaCnt_ = 0;         //�z��̍ő�l��ۑ�
    private int singleMateriaCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��
    public static GameObject[] materiaBox_; // ���������v���n�u��ۑ�


    // �o�b�O�̒��g(�A�C�e��)
    [SerializeField]
    private GameObject itemUIPrefab;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    private ItemList itemList_;   // �f�ނ�ExcelData�ǂݍ���
    private int maxItemCnt_ = 0;// �A�C�e���̍ő��
    public static GameObject[] itemBox_; // ���������v���n�u��ۑ�
    public static int[] itemSellPrice_;  // ����Ƃ��̒l�i


    // �o�b�O�̒��g�i���[�h�j
    public enum WORD
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT_HEAL,   // 1 ��
        ELEMENT_ASSIST, // 2 �⏕
        ELEMENT_ATTACK, // 3 �U���n
        TAIL,           // 4
        SUB1,           // 5 �����@�G
        SUB2,           // 6 HP ���@�U���@�����U���@�h���
        SUB1_AND_SUB2,  // 7 �Ł@�ÈŁ@��Ⴡ@����
        SUB3,           // 8 �㏸�A�ቺ�A���ˁA�K��
        ALL_SUB,         // 9�@�z��
        INFO,           // 10
        MAX
    }
    public int sub2sCnt_ = 0;

    [SerializeField]
    private GameObject wordUIPrefab;     // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    private WordList wordList_;   // �f�ނ�ExcelData�ǂݍ���
    private int maxWordCnt_ = 0;
    public static GameObject[] wordPleate_; // ���������v���n�u��ۑ�
    public static int[] activeNum_;
    public static WORD[] wordKinds_;


    public enum FIELD_NUM
    {
        NON = -1,
        FOREST,
        FIELD1,
        FIELD2,
        FIELD3,
        FIELD4,
        MATERIAS,
        MAX
    }
    private ItemGet materiaGet_;                    // �t�B�[���h��ŌĂяo�����Script
    private int fieldNumber_ = (int)FIELD_NUM.FOREST;  // ���݂���t�B�[���h�̔ԍ�

    void Awake()
    {
        for (int i = 0; i < (int)FIELD_NUM.MAX; i++)
        {
            // SceneMng�ɂ���enum��FIELD_NUM�̒l�����킹�邽�߂�nowScene-3
            if (((int)SceneMng.nowScene) - 3 == i)
            {
                fieldNumber_ = i;

                materiaGet_ = GameObject.Find("ItemPoints").GetComponent<ItemGet>();

                materiaGet_.SetMaterialKinds(fieldNumber_, materiaList_);

                break;
            }
        }
        materiaList_ = Resources.Load("MateriaList/M_Field" + fieldNumber_) as MateriaList;

        if (onceFlag_ == false)
        {
            // materiaList_.param.Count�͂ǂ̃V�[�g�ł���������
            singleMateriaCnt_ = materiaList_.param.Count;
            maxMateriaCnt_ = (int)FIELD_NUM.MAX * singleMateriaCnt_;

            materiaBox_ = new GameObject[maxMateriaCnt_];
            int number = 0;
            for (int f = 0; f < (int)FIELD_NUM.MAX; f++)
            {
                materiaList_ = Resources.Load("MateriaList/M_Field" + f) as MateriaList;
                for (int m = 0; m < singleMateriaCnt_; m++)
                {
                    number = f * singleMateriaCnt_ + m;
                  //  Debug.Log("�ǂݍ��񂾌�" + materiaList_.param[m].MateriaName);

                    materiaBox_[number] = Instantiate(materiaUIPrefab,
                            new Vector2(0, 0), Quaternion.identity, this.transform);
                    materiaBox_[number].name = materiaList_.param[m].MateriaName;
                }
            }


            itemList_ = Resources.Load("ItemList/ItemSheet1") as ItemList;
            maxItemCnt_ = itemList_.param.Count;
            itemBox_ = new GameObject[maxItemCnt_];
            itemSellPrice_ = new int[maxItemCnt_];
            // Debug.Log("�����Ăނ̂���邢" + maxItemCnt_);
            for (int i = 0; i < maxItemCnt_; i++)
            {
                itemBox_[i] = Instantiate(itemUIPrefab,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                itemBox_[i].name = itemList_.param[i].ItemName;
                itemSellPrice_[i] = itemList_.param[i].Price_Sell;
            }



            // ���[�h���X�g�̓ǂݍ���
            wordList_ = Resources.Load("WordList/WordList0") as WordList;
            maxWordCnt_ = wordList_.param.Count;

            wordPleate_ = new GameObject[maxWordCnt_];
            activeNum_ = new int[maxWordCnt_];
            wordKinds_ = new WORD[maxWordCnt_];
            for (int w = 0; w < maxWordCnt_; w++)
            {
                wordPleate_[w] = Instantiate(wordUIPrefab,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                wordPleate_[w].name = wordList_.param[w].Word;
                activeNum_[w] = wordList_.param[w].ListNumber;
                wordKinds_[w] = (WORD)wordList_.param[w].KindsNumber;

                if (wordKinds_[w] == WORD.SUB2
                    || wordKinds_[w] == WORD.SUB1_AND_SUB2)
                {
                    sub2sCnt_++;
                        }
                //  Debug.Log(w + "�ԖځF" + wordPleate_[w].name);
            }
            //GameObject.Find("Managers").GetComponent<Bag_Word>().Init();

            onceFlag_ = true;
        }
        //Debug.Log("PopMateriaList���Ă΂�܂���");
    }

    public int SetMaxMateriaCount()
    {
        return maxMateriaCnt_;
    }

    public int SetSingleMateriaCount()
    {
        return singleMateriaCnt_;
    }

    public int SetMaxItemCount()
    {
        return maxItemCnt_;
    }

    public int SetMaxWordCount()
    {
        return maxWordCnt_;
    }

    public int SetMaxSub2sWordCount()
    {
        return sub2sCnt_;
    }
}