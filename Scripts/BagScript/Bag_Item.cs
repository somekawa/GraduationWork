using System.Collections;
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
        public bool getFlag;    // �������Ă��邩�ǂ���
    }
    public static ItemData[] itemState;
    public static ItemData[] data = new ItemData[50];


    public void Init()
    // void Start()
    {
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
       
        maxCnt_ = popItemList_.SetMaxItemCount();

        itemState = new ItemData[maxCnt_];
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_HaveItem>();
        for (int i = 0; i < maxCnt_; i++)
        {
            itemState[i] = new ItemData
            {
                box = InitPopList.itemData[i].box,
                name = InitPopList.itemData[i].name,
                haveCnt = 0,
            };
            //Debug.Log(itemState[i].name);
            //itemBox_[i] = PopMateriaList.itemBox_[i];
            itemState[i].box.transform.SetParent(itemParent.transform);
            itemState[i].box.name = itemState[i].name;

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
            ItemGetCheck(i, itemState[i].name, 1);
           // Debug.Log(i + "�Ԗڂ̑f��" + itemState[i].name);
        }

    }

    public void ItemGetCheck(int itemNum,string itemName,int createCnt)
    {
        itemState[itemNum].haveCnt += createCnt;
        if(0< itemState[itemNum].haveCnt)
        {
            itemState[itemNum].getFlag = true;
            itemState[itemNum].box.SetActive(true);
           // Debug.Log(itemName + "���擾���܂���");
        }
        else
        {
            itemState[itemNum].getFlag = false;
            itemState[itemNum].box.SetActive(false);
        }
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();


        data[itemNum].name = itemName;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
    }

    public void DataLoad()
    {
        Debug.Log("���[�h���܂�");

        csvDatas.Clear();

        // �s���������ɂƂǂ߂�
        string[] texts = File.ReadAllText(saveDataFilePath).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
            csvDatas.Add(texts[i].Split(','));
        }
        Debug.Log("�f�[�^��" + csvDatas.Count);


        // �A�C�e��������
        for (int i = 1; i <= maxCnt_; i++)
        {
            ItemData set = new ItemData
            {
                name = csvDatas[i + 1][0],// name���̂͂���Script���A�^�b�`�w��I�u�W�F�N�g���������Ă�
                haveCnt = int.Parse(csvDatas[i + 1][1]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        Debug.Log("���@����������܂����B�Z�[�u���܂�");

        saveCsvSc_.SaveStart();

        // ���@�̌�����
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveItemData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

}