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
    public static ItemData[] itemState;// xls�f�[�^����ǂݍ��񂾂��̂�ۑ�
    public static ItemData[] data;// csv�f�[�^����ǂݍ��񂾂��̂�ۑ�
    public static bool itemUseFlg = false;  // �A�C�e�����g�p������true

    private int clickItemNum_ = -1;
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button throwAwayBtn_;
    private Button useBtn_;
    private UseItem useItem_;
    private GameObject infoBack_;
    private GameObject charasText_;

    private bool checkFlag_ = false;

    public void Init()
    // void Start()
    {
        if (checkFlag_ == false)
        {
            popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

            maxCnt_ = popItemList_.SetMaxItemCount();
            itemState = new ItemData[maxCnt_];
            data = new ItemData[maxCnt_];
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_HaveItem>();
            for (int i = 0; i < maxCnt_; i++)
            {
                itemState[i] = new ItemData
                {
                    box = InitPopList.itemData[i].box,
                    info = InitPopList.itemData[i].info,
                };

                itemState[i].number = int.Parse(csvDatas_[i + 1][0]);
                itemState[i].name = csvDatas_[i + 1][1];
                //Debug.Log("�A�C�e���̖��O�F"+itemState[i].name);
                // �A�C�e���̏��������m�F
                itemState[i].haveCnt = int.Parse(csvDatas_[i + 1][2]);
                // �e�̈ʒu��ύX
                itemState[i].box.transform.SetParent(itemParent.transform);

                // �T���₷���悤�ɔԍ������Ă���
                itemState[i].box.name = itemState[i].name + i;

                // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
                itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
                int num = maxCnt_ / 2 < i ? i - maxCnt_ / 2 : i;
                itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][num];

                // Ex�A�C�e���̖ڈ��\�����邩���Ȃ���
                itemState[i].EX = itemState[i].box.transform.Find("SymbolImage").GetComponent<Image>();
                bool flag = maxCnt_ / 2 < i ? true : false;
                itemState[i].EX.gameObject.SetActive(flag);

                // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
                itemState[i].cntText = itemState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
                itemState[i].cntText.text = itemState[i].haveCnt.ToString();

                itemState[i].getFlag = 0 < itemState[i].haveCnt ? true : false;
                itemState[i].box.SetActive(itemState[i].getFlag);
            }
            checkFlag_ = true;
        }
        else
        {
            // �X���A�C�e���g�p�ŃA�C�e�������ύX���ꂽ�Ƃ�
            for (int i = 0; i < maxCnt_; i++)
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

    public void ItemGetCheck(MovePoint.JUDGE judge, int itemNum,int createCnt)
    {
        if(judge == MovePoint.JUDGE.GOOD)
        {
            itemNum = itemNum*2;
        }
        Debug.Log("���Z�����A�C�e��"+itemState[itemNum].name);
        itemState[itemNum].haveCnt = itemState[itemNum].haveCnt + createCnt;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
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
        Init();
    }

    public void DataSave()
    {
        saveCsvSc_.SaveStart();

        // �A�C�e���̌�����
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
            infoBack_ = GameObject.Find("ItemBagMng/InfoBack").gameObject;
            throwAwayBtn_ = infoBack_.transform.Find("ItemDelete").GetComponent<Button>();
            useBtn_ = infoBack_.transform.Find("ItemUse").GetComponent<Button>();
            info_ = infoBack_.transform.Find("InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);
        useBtn_.gameObject.SetActive(true);

        // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
        clickItemNum_ = num;

        StartCoroutine(MoveObj(false));
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
            useBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
    }

    public void OnClickUseButton()
    {
        if (useItem_ == null)
        {
            useItem_ = GameObject.Find("ItemBagMng").GetComponent<UseItem>();
        }

        if (infoBack_ == null)
        {
            infoBack_ = GameObject.Find("ItemBagMng/InfoBack").gameObject;
        }

        Text[] text = new Text[2];
        for (int i = 0; i < charasText_.transform.childCount; i++)
        {
            text[i] = charasText_.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }

        // �����ŕԂ�l���ꎞ�ϐ��ɂ���Ȃ��ƁAUse�֐���2��ʂ邱�ƂɂȂ��ăo�O��������
        var tmp = useItem_.Use(clickItemNum_);

        // ���I�𒆂̃A�C�e�����g����
        if (tmp.Item1)
        {
            if (tmp.Item2)
            {
                // �Ώۂ̑I�����K�v
                useItem_.TextInit(text);
                StartCoroutine(MoveObj(true));
            }
            else
            {
                // �Ώۂ̑I�����K�v�Ȃ�
                // true�ŕԂ��Ă����Ƃ��ɂ́A�g�p�����A�C�e����-1����
                itemState[clickItemNum_].haveCnt--;
                itemUseFlg = true;
            }
        }

        // �\�����̏��������X�V
        itemState[clickItemNum_].cntText.text = itemState[clickItemNum_].haveCnt.ToString();
        if (itemState[clickItemNum_].haveCnt < 1)
        {
            // ��������0�ɂȂ������\���ɂ���
            itemState[clickItemNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            useBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
    }


    // �I�u�W�F�N�g�ړ��̃R���[�`��
    private System.Collections.IEnumerator MoveObj(bool outFlg)
    {
        if (charasText_ == null)
        {
            charasText_ = GameObject.Find("ItemBagMng/CharasText").gameObject;
        }

        (int, bool)[] tmpPair = new (int, bool)[2];
        if (outFlg)
        {
            tmpPair[0] = (300, false);
            tmpPair[1] = (-170, false);
        }
        else
        {
            tmpPair[0] = (105, false);
            tmpPair[1] = (0, false);
        }

        bool tmpFlg = false;
        while (!tmpFlg)
        {
            tmpFlg = true;

            if (outFlg)
            {
                // �e�L�X�g�͍��ɁA�A�C�e�������͉E�ɂ��炷
                if (!tmpPair[0].Item2)
                {
                    infoBack_.transform.position = new Vector3(infoBack_.transform.position.x + 3, infoBack_.transform.position.y, infoBack_.transform.position.z);
                    if (infoBack_.transform.localPosition.x >= tmpPair[0].Item1)
                    {
                        tmpPair[0].Item2 = true;
                    }
                }
                if (!tmpPair[1].Item2)
                {
                    charasText_.transform.position = new Vector3(charasText_.transform.position.x - 3, charasText_.transform.position.y, charasText_.transform.position.z);
                    if (charasText_.transform.localPosition.x <= tmpPair[1].Item1)
                    {
                        tmpPair[1].Item2 = true;
                    }
                }
            }
            else
            {
                // �e�L�X�g�͉E�ɁA�A�C�e�������͍��ɖ߂�
                if (!tmpPair[0].Item2)
                {
                    infoBack_.transform.position = new Vector3(infoBack_.transform.position.x - 3, infoBack_.transform.position.y, infoBack_.transform.position.z);
                    if (infoBack_.transform.localPosition.x <= tmpPair[0].Item1)
                    {
                        tmpPair[0].Item2 = true;
                    }
                }
                if (!tmpPair[1].Item2)
                {
                    charasText_.transform.position = new Vector3(charasText_.transform.position.x + 3, charasText_.transform.position.y, charasText_.transform.position.z);
                    if (charasText_.transform.localPosition.x >= tmpPair[1].Item1)
                    {
                        tmpPair[1].Item2 = true;
                    }
                }
            }

            for (int i = 0; i < tmpPair.Length; i++)
            {
                tmpFlg &= tmpPair[i].Item2;
            }

            yield return null;
        }
    }

}