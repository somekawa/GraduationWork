using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreateMng : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas;

    [SerializeField]
    private RectTransform miniGameMng;    // �~�j�Q�[���\���p

    [SerializeField]
    private GameObject itemRecipePleate;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    private InitPopList popItemRecipeList_;
    private int maxCnt_ = 0;

    private Bag_Item bagItem_;// �������Ă���A�C�e�����m�F

    private string saveBtnName_ = "";   // �ǂ̃A�C�e��������������ۑ�
    private Button saveBtn_;            // �ǂ̃{�^��������������ۑ�
    private int saveItemNum_ = 0;

    public struct itemRecipe
    {
        public GameObject pleate;   // �A�C�e����\������I�u�W�F�N�g
        public Button btn;          // �A�C�e���N���b�N
        public Text nameText;       // �A�C�e���̖��O��\������Text
        public string name;         // �A�C�e���̖��O
        public string materia1;     // �����ɕK�v�ȑf��1
        public string materia2;     // �����ɕK�v�ȑf��2
        public string materia3;     // �����ɕK�v�ȑf��3
    }
    public static itemRecipe[] itemRecipeState;

    private RectTransform itemRecipeParent_;
    private Bag_Materia bagMateria_;// �����f�ނ��m�F

    // �~�j�Q�[���֘A
    private MovePoint movePoint_;
    private MovePoint.JUDGE judge_ = MovePoint.JUDGE.NON;
    private Image judgeBack_;
    private Text judgeText_;

    private Button createBtn_;  // �쐬�J�n�{�^��
    private Button cancelBtn_;  // �����I���J�n�{�^��

    // �I�������A�C�e���̐�����
    private Text createName_;   // �����������A�C�e���̖��O
    private Text wantMateria_;  // �����������A�C�e���̕K�v�f��

    void Start()
    {
        itemRecipeParent_ = transform.Find("ScrollView/Viewport/ItemRecipeParent").GetComponent<RectTransform>();
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

        popItemRecipeList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        maxCnt_ = popItemRecipeList_.SetMaxItemCount();
        itemRecipeState = new itemRecipe[maxCnt_];

        for (int i = 0; i < maxCnt_; i++)
        {
            itemRecipeState[i].pleate = Instantiate(itemRecipePleate,
            new Vector2(0, 0), Quaternion.identity, itemRecipeParent_.transform);
            itemRecipeState[i].name = InitPopList.itemData[i].name;
            itemRecipeState[i].pleate.name = itemRecipeState[i].name;
            itemRecipeState[i].btn = itemRecipeState[i].pleate.GetComponent<Button>();

            // �\������A�C�e���̖��O
            itemRecipeState[i].nameText = itemRecipeState[i].pleate.transform.Find("Text").GetComponent<Text>();
            itemRecipeState[i].nameText.text = itemRecipeState[i].name;
            //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );
            itemRecipeState[i].materia1 = InitPopList.itemData[i].materia1;
            itemRecipeState[i].materia2 = InitPopList.itemData[i].materia2;
            itemRecipeState[i].materia3 = InitPopList.itemData[i].materia3;
        }

        //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
        bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

        // �~�j�Q�[��
        movePoint_ = miniGameMng.transform.GetComponent<MovePoint>();
        miniGameMng.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        judgeBack_ = miniGameMng.transform.Find("JudgeBack").GetComponent<Image>();
        judgeText_ = judgeBack_.transform.Find("Text").GetComponent<Text>();

        cancelBtn_ = transform.Find("CancelBtn").GetComponent<Button>();
        createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();

        createName_ = transform.Find("InfoBack/CreateItemName").GetComponent<Text>();
        wantMateria_ = transform.Find("InfoBack/WantMateriaNames").GetComponent<Text>();
        ResetCommon();
    }

    public IEnumerator AlchemyRecipeSelect()
    {
        while (true)
        {
            if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NON)
            {
                yield return null;
            }
            else
            {
                if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NORMAL)
                {
                    judgeText_.text = "����";
                }
                else
                {
                    judgeText_.text = "�听��";
                }
                judgeBack_.gameObject.SetActive(true);
                bagItem_.ItemGetCheck(saveItemNum_, saveBtnName_, 1);
                saveBtn_.interactable = true;
                saveBtnName_ = "";

                yield return new WaitForSeconds(2.0f);
                ResetCommon();
                yield break;
            }
        }
    }

    public void SetActiveRecipe(int recipeNum, string name)
    {
        createName_.text = itemRecipeState[recipeNum].btn.name;
        if (itemRecipeState[recipeNum].materia3 == "non")
        {
            wantMateria_.text = "�K�v�f��\n" +
                 itemRecipeState[recipeNum].materia1 +
           " " + itemRecipeState[recipeNum].materia2;
        }
        else
        {
            wantMateria_.text = "�K�v�f��\n" +
                 itemRecipeState[recipeNum].materia1 +
           " " + itemRecipeState[recipeNum].materia2 +
           " " + itemRecipeState[recipeNum].materia3;
        }
    }

    public void OnClickCreate()
    {
        bool flag = false;
        int recipeNum = 0;
        int[] number = new int[3];
        //int number = 0;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (itemRecipeState[i].btn.interactable == false)
            {
                recipeNum = i;
                //// number = i;
                // Debug.Log("�K�v�ȑf��1�ځF" + recipeList_.param[number].WantMateria1);
                // Debug.Log("�K�v�ȑf��2�ځF" + recipeList_.param[number].WantMateria2);
                flag = true;
                break;
            }
        }

        if (flag == true)
        {
            Debug.Log("�K�v�ȑf�ނ������Ă��邩�`�F�b�N���܂�      " + bagMateria_.GetMaxHaveMateriaCnt());

            number[0] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia1);

            number[2] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia3);

            number[1] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia2);

            Debug.Log("number[0]:" + number[0] + "   number[1]:" + number[1] + "   number[2]:" + number[2]);
            if (itemRecipeState[recipeNum].materia3 == "non")
            {
                Bag_Materia.materiaState[number[0]].haveCnt--;
                Bag_Materia.materiaState[number[1]].haveCnt--;
            }
            else
            {
                Bag_Materia.materiaState[number[0]].haveCnt--;
                Bag_Materia.materiaState[number[1]].haveCnt--;
                Bag_Materia.materiaState[number[2]].haveCnt--;
            }
        }
    }

    private int HaveMateriaCheck(int recipeNum, string wantMateria)
    {
        int haveMateriaNum = 0;
        Debug.Log("�K�v�f��" + wantMateria);

        for (int i = 0; i <= bagMateria_.GetMaxHaveMateriaCnt(); i++)
        {
            if (wantMateria == "non")
            {
                // non�������Ă���̂�3�ڂ̑f�ނ���
                Debug.Log("3�ڂ̑f�ނ͕K�v����܂���");
                break;
            }
            else
            {
                if (Bag_Materia.materiaState[i].name == wantMateria)
                {
                    Debug.Log(Bag_Materia.materiaState[i].name + "�������Ă܂�");
                    haveMateriaNum = i;
                    if (Bag_Materia.materiaState[i].haveCnt < 1)
                    {
                        Debug.Log("2�ڂ̑f�ނ̏�����������܂���");
                        return 0;
                    }
                    else
                    {
                        if (wantMateria == itemRecipeState[recipeNum].materia2)
                        {
                            createBtn_.interactable = false;
                            miniGameMng.gameObject.SetActive(true);
                            StartCoroutine(movePoint_.CountDown());
                            StartCoroutine(AlchemyRecipeSelect());
                            cancelBtn_.interactable = false;
                            Debug.Log(Bag_Materia.materiaState[i].name + "�������Ă��܂����B�Q�[�����n�߂܂�");
                        }
                    }
                    break;
                }
            }
        }
        return haveMateriaNum;
    }

    public void OnClickCancelCtn()
    {
        StopCoroutine(AlchemyRecipeSelect());
        this.gameObject.SetActive(false);
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

                if (miniGameMng.gameObject.activeSelf == false)
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
        createName_.text = "";
        wantMateria_.text = "";
        miniGameMng.gameObject.SetActive(false);
        judge_ = MovePoint.JUDGE.NON;
        judgeText_.text = "";
        movePoint_.SetMiniGameJudge(judge_);
        judgeBack_.gameObject.SetActive(false);
    }
}