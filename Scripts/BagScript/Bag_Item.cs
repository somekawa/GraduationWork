using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Item : MonoBehaviour
{
    [SerializeField]
    private RectTransform itemParent;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    [SerializeField]
    private GameObject itemUIBox;     // Ex�A�C�e�������n

    // �f�[�^�n
    private SaveCSV_HaveItem saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private const string saveDataFilePath_ = @"Assets/Resources/HaveItemList.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSV�̒��g�����郊�X�g;
                                          
    // ���ׂẴA�C�e����
    private InitPopList popItemList_;
    private int maxCnt_ = 0;// xls����ǂݍ��ރA�C�e���̐�

    public struct ItemData
    {
        public int number;
        public GameObject box;  // �������Ă������I�u�W�F�N�g����
        public Image image;     // �A�C�e���摜
        public Image EX;// �听���\���摜
        public Text cntText;    // ��������\��
        public string name;     // �A�C�e���̖��O
        public string info;
        public bool getFlag;    // �������Ă��邩�ǂ���
        public int haveCnt;     // �w��A�C�e���̏�����
    }
    public static ItemData[] itemState;
    public static ItemData[] data;
    private int exItemNum_ ;// �听�����܂߂��A�C�e���̐�

    //public struct LoadItemData
    //{
    //    public string name;
    //    public int cnt;
    //}

    private int clickItemNum_ = -1;
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button throwAwayBtn_;
    private bool checkFlag_ = false;

    public void Init()
    // void Start()
    {
        if (checkFlag_ == false)
        {
            popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

            maxCnt_ = popItemList_.SetMaxItemCount();
            Debug.Log("csv�A�C�e����" + exItemNum_ + "         xls�A�C�e����" + maxCnt_);
            //exItemNum_ =maxCnt_;
            itemState = new ItemData[exItemNum_];
            data = new ItemData[exItemNum_];
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_HaveItem>();
            for (int i = 0; i < exItemNum_; i++)
            {
                if (maxCnt_ - 1 < i)
                {
                    // �听���A�C�e���̐���
                    itemState[i].box = Instantiate(itemUIBox,
                             new Vector2(0, 0), Quaternion.identity, transform);
                    //Debug.Log(i+":�听���A�C�e���̐���");
                }
                else
                {
                    itemState[i] = new ItemData
                    {
                        box = InitPopList.itemData[i].box,
                        info = InitPopList.itemData[i].info,
                    };
                }

                itemState[i].number = int.Parse(csvDatas_[i + 1][0]);
                itemState[i].name = csvDatas_[i + 1][1];
                itemState[i].haveCnt = int.Parse(csvDatas_[i + 1][2]);
                // �e�̈ʒu��ύX
                itemState[i].box.transform.SetParent(itemParent.transform);

                // �T���₷���悤�ɔԍ������Ă���
                itemState[i].box.name = itemState[i].name + i;

                // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
                itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
                int num = maxCnt_ - 1 < i ? i - maxCnt_ : i;
                itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][num];

                // Ex�A�C�e���̖ڈ��\�����邩���Ȃ���
                itemState[i].EX = itemState[i].box.transform.Find("SymbolImage").GetComponent<Image>();
                bool flag = maxCnt_ - 1 < i ? true : false;
                itemState[i].EX.gameObject.SetActive(flag);

                // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
                itemState[i].cntText = itemState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
                itemState[i].cntText.text = itemState[i].haveCnt.ToString();
                //itemCnt_[i] = 0;// ���ׂẴA�C�e���̏�������0�ɂ���

                itemState[i].getFlag = 0 < itemState[i].haveCnt ? true : false;
                itemState[i].box.SetActive(itemState[i].getFlag);
            }
        }
        else
        {
            // �X���A�C�e���g�p�ŃA�C�e�������ύX���ꂽ�Ƃ�
            for (int i = 0; i < exItemNum_; i++)
            {
                itemState[i].cntText.text = itemState[i].haveCnt.ToString();
                itemState[i].getFlag = 0 < itemState[i].haveCnt ? true : false;
                itemState[i].box.SetActive(itemState[i].getFlag);
            }
        }


            //// �f�o�b�O�p �S���̑f�ނ�5�擾������ԂŎn�܂�
            //for (int i = 0; i < exItemNum_; i++)
            //{
            //    ItemGetCheck(i, itemState[i].name, 10, MovePoint.JUDGE.NORMAL);
            //   // Debug.Log(i + "�Ԗڂ̑f��" + itemState[i].name);
            //}
            //ItemGetCheck(0, itemState[0].name, 10, MovePoint.JUDGE.GOOD);

        }

    public void ItemGetCheck(int itemNum,int createCnt)
    {
        itemState[itemNum].haveCnt += createCnt;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
    }

    public void SetItemCntText(int itemNum)
    {
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();
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
        exItemNum_ = csvDatas_.Count - 1;
        Init();
    }

    private void DataSave()
    {
      //  Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < exItemNum_; i++)
        {
            saveCsvSc_.SaveItemData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

    public void SetItemNumber(int num)
    {
        // �̂Ă�{�^����\��
        if (throwAwayBtn_ == null)
        {
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/ItemDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);

        // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
        clickItemNum_ = num;
    }

    public void OnClickThrowAwayButton()
    {
        if(itemParent.gameObject.activeSelf==false)
        {
            //throwAwayBtn_.gameObject.SetActive(false);
            return;
        }

        // �̂Ă�{�^���������ꂽ�ꍇ
        itemState[clickItemNum_].haveCnt--;// ��������1���炷

        // �\�����̏��������X�V
        itemState[clickItemNum_].cntText.text = itemState[clickItemNum_].haveCnt.ToString();
        if (itemState[clickItemNum_].haveCnt<1)
        {
            // ��������0�ɂȂ������\���ɂ���
            itemState[clickItemNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
    }

}