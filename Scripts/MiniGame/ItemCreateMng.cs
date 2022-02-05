using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreateMng : MonoBehaviour
{
    [SerializeField]
    private GameObject miniGameObj;// �~�j�Q�[���p�̃I�u�W�F�N�g

    [SerializeField]
    private Canvas uniHouseCanvas;

    [SerializeField]
    private GameObject itemRecipePleate;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    private InitPopList popItemRecipeList_;
    private int maxCnt_ = 0;
    private int startCnt_ = 0;
    private int maxMateriaCnt_ = 0;

    private Bag_Item bagItem_;// �������Ă���A�C�e�����m�F

    private string saveBtnName_ = "";   // �ǂ̃A�C�e��������������ۑ�
    private Button saveBtn_;            // �ǂ̃{�^��������������ۑ�
    private int saveItemNum_ = 0;
    // private string saveName_ = "";

    public struct itemRecipe
    {
        public GameObject pleate;   // �A�C�e����\������I�u�W�F�N�g
        public Button btn;          // �A�C�e���N���b�N
        public TMPro.TextMeshProUGUI nameText;       // �A�C�e���̖��O��\������Text
        public string name;         // �A�C�e���̖��O
        public string materia1;     // �����ɕK�v�ȑf��1
        public string materia2;     // �����ɕK�v�ȑf��2
        public string materia3;     // �����ɕK�v�ȑf��3
    }
    public static itemRecipe[] itemRecipeState;
    private int[] haveCnt_;// ������
    private int nonMateriaNum_ = -1;
    private int[] materiaNumber1_;
    private int[] materiaNumber2_;
    private int[] materiaNumber3_;

    private RectTransform itemRecipeParent_;
    private Bag_Materia bagMateria_;// �����f�ނ��m�F

    //// �~�j�Q�[���֘A
    private CircleMng circleMng_;
    private int circleNum_ = 6;

    private Button createBtn_;  // �쐬�J�n�{�^��
    private Button cancelBtn_;  // �����I���J�n�{�^��

    // �I�������A�C�e���̐�����
    private Image arrowImage_;
    private Image itemIconBack_;// �A�C�e���̃A�C�R���̔w�i
    private Image itemIcon_;// �A�C�e���摜
    private Image itemNameFrame_;// �A�C�e�����O�ʒu�̔w�i
    private TMPro.TextMeshProUGUI createName_;   // �����������A�C�e���̖��O
    private TMPro.TextMeshProUGUI itemhaveCnt_;   // �����������A�C�e���̏�����
    private TMPro.TextMeshProUGUI haveCntTitle_;   // �����������A�C�e���̏�����

    private TMPro.TextMeshProUGUI needMateria_;  // �����������A�C�e���̕K�v�f��
    private TMPro.TextMeshProUGUI needCnt_;

    private int[] maxRecipActiveCnt_ = new int[5] { 0, 5, 10, 15, 19 };

    // �f�o�b�O�p
    private SaveLoadCSV saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g

    public void Init()
    {
        // �f�o�b�O�p
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>();
        saveCsvSc_.LoadData(SaveLoadCSV.SAVEDATA.BOOK);

        GameObject.Find("Managers").GetComponent<Bag_Word>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Materia>().DataLoad();


        if (circleMng_==null)
        {
            circleMng_ = miniGameObj.transform.Find("ElementCircle/JudgeCircle").GetComponent<CircleMng>();
            itemRecipeParent_ = transform.Find("ScrollView/Viewport/ItemRecipeParent").GetComponent<RectTransform>();
            bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

            //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
            bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

            cancelBtn_ = transform.Find("CancelBtn").GetComponent<Button>();
            createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();

            var infoBack= transform.Find("InfoBack").GetComponent<RectTransform>();
            arrowImage_ = infoBack.Find("ArrowImage").GetComponent<Image>();
            itemNameFrame_= infoBack.Find("ItemNameFrame").GetComponent<Image>();
            createName_ = itemNameFrame_.transform.Find("ItemName").GetComponent<TMPro.TextMeshProUGUI>();
            itemIconBack_ = createName_.transform.Find("ImageBack").GetComponent<Image>();
            itemIcon_ = itemIconBack_.transform.Find("ItemImage").GetComponent<Image>();
            itemhaveCnt_ = createName_.transform.Find("ItemCnt").GetComponent<TMPro.TextMeshProUGUI>();
            haveCntTitle_ = createName_.transform.Find("CntTitle").GetComponent<TMPro.TextMeshProUGUI>();
            haveCntTitle_.text = "������\nEx";

            needMateria_ = transform.Find("InfoBack/NeedMateria").GetComponent<TMPro.TextMeshProUGUI>();
            needCnt_= needMateria_.transform.Find("NeedCnt").GetComponent<TMPro.TextMeshProUGUI>();
        
            
            popItemRecipeList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
            maxCnt_ = popItemRecipeList_.SetMaxItemCount() / 2;
            Debug.Log("�ő�l" + maxCnt_);
            itemRecipeState = new itemRecipe[maxCnt_];
            maxMateriaCnt_ = popItemRecipeList_.SetMaxMateriaCount();

            haveCnt_ = new int[maxMateriaCnt_];
            for (int i = 0; i < maxMateriaCnt_; i++)
            {
                haveCnt_[i] = Bag_Materia.materiaState[i].haveCnt;
            }

            Debug.Log(BookStoreMng.bookState_[25].readFlag);

            if (BookStoreMng.bookState_[25].readFlag == 1)
            {
                // ���V�s�ɋ���ǂ�ł�����
                maxCnt_ = maxRecipActiveCnt_[4];
            }
            else if (BookStoreMng.bookState_[18].readFlag == 1)
            {
                // ���V�s������ǂ�ł�����
                maxCnt_ = maxRecipActiveCnt_[3];
            }
            else if (BookStoreMng.bookState_[11].readFlag == 1)
            {
                // ���V�s������ǂ�ł�����
                maxCnt_ = maxRecipActiveCnt_[2];
            }
            else if (BookStoreMng.bookState_[5].readFlag == 1)
            {
                // ���V�s������ǂ�ł�����
                startCnt_ = maxRecipActiveCnt_[0];
                maxCnt_ = maxRecipActiveCnt_[1];
            }
            else
            {
                maxCnt_ = 0;// ���V�s�{���������ĂȂ�
            }
            Debug.Log("�����\�A�C�e����" + maxCnt_);

            if (maxCnt_ == 0)
            {
                // ���V�s�{���������ĂȂ��ꍇ
                createName_.text = null;
                needMateria_.text = "�{���Ń��V�s���w�����܂��傤";
                createBtn_.interactable = false;
                return;
            }

            materiaNumber1_ = new int[maxCnt_];
            materiaNumber2_ = new int[maxCnt_];
            materiaNumber3_ = new int[maxCnt_];
            for (int i = startCnt_; i < maxCnt_; i++)
            {
                itemRecipeState[i].pleate = Instantiate(itemRecipePleate,
                new Vector2(0, 0), Quaternion.identity, itemRecipeParent_.transform);
                itemRecipeState[i].name = InitPopList.itemData[i].name;
                itemRecipeState[i].pleate.name = itemRecipeState[i].name;
                itemRecipeState[i].btn = itemRecipeState[i].pleate.GetComponent<Button>();

                // �\������A�C�e���̖��O
                itemRecipeState[i].nameText = itemRecipeState[i].pleate.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
                itemRecipeState[i].nameText.text = itemRecipeState[i].name;
                //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );

                var allNameCheck1 = InitPopList.itemData[i].materia1.Split('_');
                var allNameCheck2 = InitPopList.itemData[i].materia2.Split('_');
                var allNameCheck3 = InitPopList.itemData[i].materia3.Split('_');
                itemRecipeState[i].materia1 = allNameCheck1[0];
                itemRecipeState[i].materia2 = allNameCheck2[0];
                itemRecipeState[i].materia3 = allNameCheck3[0];

                materiaNumber1_[i] = int.Parse(allNameCheck1[1]);
                materiaNumber2_[i] = int.Parse(allNameCheck2[1]);
                materiaNumber3_[i] = int.Parse(allNameCheck3[1]);
                 haveCnt_[i] = Bag_Materia.materiaState[i].haveCnt;
              //  Debug.Log(haveCnt[i] + "��");
            }

        }
        miniGameObj.gameObject.SetActive(false);
        miniGameObj.transform.localPosition = new Vector3(-0.25f, 0.8f, 0.2f);


        ResetCommon();
    }

    public void AlchemyRecipeSelect()
    {
        int rate = (int)circleMng_.GetMiniGameJudge();
        bagItem_.ItemGetCheck(rate, saveItemNum_, 1);
        saveBtn_.interactable = true;
        saveBtnName_ = "";

        ResetCommon();
    }

    public void SetActiveRecipe(int recipeNum, string name)
    {
        // �摜�̃A�C�R��
        itemIconBack_.gameObject.SetActive(true);
        itemIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][recipeNum];
        // ���
        arrowImage_.gameObject.SetActive(true);
        // �I�������A�C�e���̖��O
        itemNameFrame_.gameObject.SetActive(true);
        createName_.text = itemRecipeState[recipeNum].btn.name;
        // �u�����v���ƁuEX�v������
        haveCntTitle_.gameObject.SetActive(true);
        // ��������EX������
        itemhaveCnt_.text = Bag_Item.itemState[recipeNum].haveCnt + "�R\n" 
                          + Bag_Item.itemState[recipeNum * 2].haveCnt + "�R\n";
        if (itemRecipeState[recipeNum].materia3 == "non")
        {
            needMateria_.text = itemRecipeState[recipeNum].materia1 +
                         "\n" + itemRecipeState[recipeNum].materia2;
            needCnt_.text = Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt + "/1\n"
                          + Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt + "/1";
        }
        else
        {
            needMateria_.text = itemRecipeState[recipeNum].materia1 +
                         "\n" + itemRecipeState[recipeNum].materia2 +
                         "\n" + itemRecipeState[recipeNum].materia3;
            needCnt_.text = Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt + "/1\n"
                          + Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt + "/1"
                          + Bag_Materia.materiaState[materiaNumber3_[recipeNum]].haveCnt + "/1";
        }
    }

    public void OnClickCreate()
    {
        int recipeNum = -1;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (itemRecipeState[i].btn.interactable == false)
            {
                recipeNum = i;
                // Debug.Log("�K�v�ȑf��1�ځF" + recipeList_.param[number].WantMateria1);
                // Debug.Log("�K�v�ȑf��2�ځF" + recipeList_.param[number].WantMateria2);
                break;
            }
        }

        if (recipeNum != -1)
        {
            //Debug.Log("�K�v�ȑf�ނ������Ă��邩�`�F�b�N���܂�      " + bagMateria_.GetMaxHaveMateriaCnt());
            bool createFlag = true;
            if (materiaNumber3_[recipeNum] == nonMateriaNum_)
            {
                Debug.Log("3�ڂ̑f�ނ͕K�v����܂���");
                if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1�ڂ̑f�ނ�����܂���");
                }
                else if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia2 + "2�ڂ̑f�ނ�����܂���");
                }
            }
            else
            {
                if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1�ڂ̑f�ނ�����܂���");
                }
                else if (haveCnt_[materiaNumber2_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("3�ڂ̑f�ނ�����܂���");
                }
                else if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("2�ڂ̑f�ނ�����܂���");
                }
            }

            if (createFlag == true)
            {
                createBtn_.interactable = false;
                cancelBtn_.interactable = false;
                miniGameObj.gameObject.SetActive(true);
                circleMng_.Init(circleNum_, recipeNum%4);

                //Debug.Log("number[0]:" + number[0] + "   number[1]:" + number[1] + "   number[2]:" + number[2]);
                if (itemRecipeState[recipeNum].materia3 == "non")
                {
                    Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt--;
                }
                else
                {
                    Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber3_[recipeNum]].haveCnt--;
                }
            }
            else
            {
                createBtn_.interactable = false;
                Debug.Log("�f�ނ��Ȃ����ߍ������邱�Ƃ��ł��܂���");
            }
        }
    }

    public void OnClickCancelBtn()
    {
        gameObject.SetActive(false);
        uniHouseCanvas.gameObject.SetActive(true);
    }

    public void SetButtonName(string name)
    {
        saveBtnName_ = name;

        // ���V�s�I������
        for (int i = 0; i < maxCnt_; i++)
        {
            if (saveBtnName_ != itemRecipeState[i].btn.name)
            {
                // �I�����ĂȂ��{�^���͉����ł����Ԃɂ���
                itemRecipeState[i].btn.interactable = true;
            }
            else
            {
                // �I���������V�s�̓��e��\��
                SetActiveRecipe(i, saveBtnName_);
                saveBtn_ = itemRecipeState[i].btn;
                saveItemNum_ = i;// �ԍ���ۑ�
                //saveName_ = Bag_Item.data[i].name;
                //Debug.Log("�������ꂽ�A�C�e��" + saveName_);
                if (miniGameObj.gameObject.activeSelf == false)
                {
                    createBtn_.interactable = true;
                }
            }
        }
    }

    private void ResetCommon()
    {
        // ���炩�̔�������ꂽ�ꍇ
        cancelBtn_.interactable = true;
        createBtn_.interactable = false;
        // ��������Z�b�g
        createName_.text = null;
        itemhaveCnt_.text = null;
        needMateria_.text = null;
        needCnt_.text = null;
        arrowImage_.gameObject.SetActive(false);
        haveCntTitle_.gameObject.SetActive(false);
        itemIconBack_.gameObject.SetActive(false);
        itemNameFrame_.gameObject.SetActive(false);
    }
}