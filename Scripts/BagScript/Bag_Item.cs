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

    // �f�[�^�n
    private SaveCSV_HaveItem saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g
    private const string saveDataFilePath = @"Assets/Resources/HaveItemList.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSV�̒��g�����郊�X�g;
                                          
    // ���ׂẴA�C�e����
    private InitPopList popItemList_;
    private int maxCnt_ = 0;

    public struct ItemData
    {
        public GameObject box;  // �������Ă������I�u�W�F�N�g����
        public Image image;     // �A�C�e���摜
        public Text cntText;    // ��������\��
        public int haveCnt;     // �w��A�C�e���̏�����
        public string name;     // �A�C�e���̖��O
        public string info;
        public bool getFlag;    // �������Ă��邩�ǂ���
    }
    public static ItemData[] itemState;
    public static ItemData[] data = new ItemData[20];
    private int exItemNum_ ;

    private int clickItemNum_ = -1;
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button throwAwayBtn_;

    public void Init()
    // void Start()
    {
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
       
        maxCnt_ = popItemList_.SetMaxItemCount();
        maxCnt_ = popItemList_.SetMaxItemCount();
        exItemNum_ =maxCnt_+1;
        itemState = new ItemData[maxCnt_];
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_HaveItem>();
        for (int i = 0; i < maxCnt_; i++)
        {
            itemState[i] = new ItemData
            {
                box = InitPopList.itemData[i].box,
                name = InitPopList.itemData[i].name,
                info = InitPopList.itemData[i].info,
                haveCnt = 0,
            };
            // �e�̈ʒu��ύX
            itemState[i].box.transform.SetParent(itemParent.transform);
          
            // �T���₷���悤�ɔԍ������Ă���
            itemState[i].box.name = itemState[i].name + i;

            // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
            itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
            itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][i];
           
            // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
            itemState[i].cntText = itemState[i].box.transform.Find("ItemNum").GetComponent<Text>();
            itemState[i].cntText.text = itemState[i].haveCnt.ToString();
            //itemCnt_[i] = 0;// ���ׂẴA�C�e���̏�������0�ɂ���

            itemState[i].getFlag = 0 < itemState[i].haveCnt ? false : true;
            itemState[i].box.SetActive(itemState[i].getFlag);
        }

        // �f�o�b�O�p �S���̑f�ނ�5�擾������ԂŎn�܂�
        for (int i = 0; i < maxCnt_; i++)
        {
            ItemGetCheck(i, itemState[i].name, 1, MovePoint.JUDGE.NORMAL);
           // Debug.Log(i + "�Ԗڂ̑f��" + itemState[i].name);
        }

    }

    public void ItemGetCheck(int itemNum,string itemName,int createCnt, MovePoint.JUDGE judge)
    {
        bool checkFlag_ = true;
        if(judge==MovePoint.JUDGE.GOOD)
        {
            // �e�̈ʒu��ύX
            itemState[exItemNum_].box.transform.SetParent(itemParent.transform);

            // �T���₷���悤�ɔԍ������Ă���
            itemState[exItemNum_].box.name = itemState[exItemNum_].name+"Ex" + exItemNum_;

            // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
            itemState[exItemNum_].image = itemState[exItemNum_].box.transform.Find("ItemIcon").GetComponent<Image>();
            itemState[exItemNum_].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][itemNum];

            // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
            itemState[exItemNum_].cntText = itemState[exItemNum_].box.transform.Find("ItemNum").GetComponent<Text>();
            itemState[exItemNum_].cntText.text = itemState[exItemNum_].haveCnt.ToString();
            exItemNum_++;

        }



        itemState[itemNum].haveCnt += createCnt;

        itemState[itemNum].getFlag = 0 < itemState[itemNum].haveCnt ? true : false;
        itemState[itemNum].box.SetActive(itemState[itemNum].getFlag);
        //if (0< itemState[itemNum].haveCnt)
        //{
        //    itemState[itemNum].getFlag = true;
        //   // Debug.Log(itemName + "���擾���܂���");
        //}
        //else
        //{
        //    itemState[itemNum].getFlag = false;
        //    itemState[itemNum].box.SetActive(false);
        //}
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();

        data[itemNum].name = itemName;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
    }

    public void DataLoad()
    {
       // Debug.Log("���[�h���܂�");

        csvDatas.Clear();

        // �s���������ɂƂǂ߂�
        string[] texts = File.ReadAllText(saveDataFilePath).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas.Add(texts[i].Split(','));
        }
       // Debug.Log("�f�[�^��" + csvDatas.Count);
        Init();
    }

    private void DataSave()
    {
      //  Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < maxCnt_; i++)
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