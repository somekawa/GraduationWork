using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCreate : MonoBehaviour
{
    [SerializeField]
    private GameObject miniGameObj;

    [SerializeField]
    private Canvas uniHouseCanvas;

    //[SerializeField]
    //private RectTransform miniGameMng;    // �~�j�Q�[���\���p

    // ���[�h��\�����邽�߂̐e�̈ʒu
    private RectTransform magicCreateParent;

    //���[�h�̍ő���擾�p
    private int[] selectKindMaxCnt_ = new int[(int)InitPopList.WORD.INFO];

    // �I�𒆂̃��[�h���
    private int kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
    // ��ʉ����A�����֘A
    private string[] topicString_ = new string[(int)Bag_Word.WORD_MNG.MAX] {
    "Head","Element","Tail","Sub1","Sub2","Sub3"};
    private TMPro.TextMeshProUGUI topicText_;    // �ǂ̃��[�h�̎�ނ�\�����Ă��邩

    // �ǂ��I�����Ă��邩��\��
    private TMPro.TextMeshProUGUI infoText_;
    //  private string[] seleEngWord_;
    private string[] selectWord_;
    private int targetWordNum_;// �K�����[�h�̔ԍ���ۑ�
    private int materiaNum_;//��̃}�e���A�̔ԍ���ۑ�
    private Image materiaImage_;// ��̃}�e���A�摜�`��
    private TMPro.TextMeshProUGUI materiaCntText_;// ��̃}�e���A�̏�������`��

    //   0.�쐬�J�n�{�^���@1.���@�����I���{�^��
    private bool createFlag_ = false;
    private Button createBtn_;
    private TMPro.TextMeshProUGUI createBtnText_;

    private Button cancelBtn_;
    private Bag_Magic bagMagic_;
    private Bag_Word bagWord_;
    private string allName_ = "";
    private int[] saveNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX]
        { -1,-1,-1,-1,-1,-1};// �I���������[�h�̔ԍ���ۑ�
    private int[] oldNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX] { -1, -1, -1, -1, -1, -1 };
    private int[] saveWordNum_ = new int[(int)Bag_Word.WORD_MNG.MAX];

    // ���{�^��
    private Button[] arrowBtn_ = new Button[2];

    ////�~�j�Q�[���X�^�[�g�p
    private CircleMng circleMng_;

    public struct MagicCreateData
    {
        public GameObject pleate;   // �C���X�^���X�����I�u�W�F�N�g��ۑ�
        public string name;         // ���[�h��
        public Button btn;
        public int power;
        public int MP;
        public bool getFlag;
    }
    public static Dictionary<Bag_Word.WORD_MNG, MagicCreateData[]> mCreateData = new Dictionary<Bag_Word.WORD_MNG, MagicCreateData[]>();
    private int[] mngMaxCnt = new int[(int)Bag_Word.WORD_MNG.MAX];

    // �З͊֘A
    private TMPro.TextMeshProUGUI powerText_;
    private int magicPower_ = 0;

    private TMPro.TextMeshProUGUI mpText_;
    private int mpPower_ = 0;
    private int maxMP = 56;


    private MagicCreateData[] InitCheck(Bag_Word.WORD_MNG kind)
    {
        int maxCnt = 0;
        int startNum = 0;
        int maxNum = 0;
        switch (kind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.HEAD];
                maxNum = (int)InitPopList.WORD.HEAD;
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_ASSIST] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_ATTACK] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ELEMENT_HEAL];
                startNum = (int)InitPopList.WORD.ELEMENT_HEAL;
                maxNum = (int)InitPopList.WORD.TAIL;
                break;

            case Bag_Word.WORD_MNG.TAIL:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.TAIL];
                maxNum = (int)InitPopList.WORD.TAIL;
                break;

            case Bag_Word.WORD_MNG.SUB1:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB1] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.SUB1_AND_SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB1;
                break;

            case Bag_Word.WORD_MNG.SUB2:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.SUB1_AND_SUB2] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB2;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                maxCnt = selectKindMaxCnt_[(int)InitPopList.WORD.SUB3] +
                         selectKindMaxCnt_[(int)InitPopList.WORD.ALL_SUB];
                startNum = (int)InitPopList.WORD.SUB3;
                maxNum = (int)InitPopList.WORD.INFO;
                break;

            default:
                break;
        }
        mngMaxCnt[(int)kind] = maxCnt;
        //  Debug.Log(kind + "            "+maxCntCheck_[(int)kind]);
        var state = new MagicCreateData[maxCnt];
        int count = 0;
        if (kind == Bag_Word.WORD_MNG.SUB1
            || kind == Bag_Word.WORD_MNG.SUB2)
        {
            // Sub1��Sub2�̎�
            for (int k = startNum; k < (int)InitPopList.WORD.MAX; k++)
            {
                if (k == startNum
                || k == (int)InitPopList.WORD.SUB1_AND_SUB2
                || k == (int)InitPopList.WORD.ALL_SUB)
                {
                    for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                    {
                        CommonInitCheck(state, count, (InitPopList.WORD)k, i);
                        // Debug.Log(state[count].name);
                        count++;
                    }
                }
            }
        }
        else
        {
            if (startNum == 0)
            {
                // HEAD,TAIL�p
                for (int i = 0; i < maxCnt; i++)
                {
                    CommonInitCheck(state, count, (InitPopList.WORD)maxNum, i);
                    count++;
                }
            }
            else
            {
                for (int k = startNum; k < maxNum; k++)
                {
                    // ELEMENT,SUB3�p
                    for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                    {
                        CommonInitCheck(state, count, (InitPopList.WORD)k, i);
                        count++;
                    }
                }
            }
        }


        return state;
    }

    private void CommonInitCheck(MagicCreateData[] data, int count, InitPopList.WORD kind, int dataNum)
    {
        data[count].name = Bag_Word.wordState[kind][dataNum].name;
        //   data[count].englishName = Bag_Word.wordState[kind][dataNum].english;
        data[count].pleate = Bag_Word.wordState[kind][dataNum].pleate;
        data[count].btn = Bag_Word.wordState[kind][dataNum].btn;
        data[count].btn.enabled = true;
        data[count].power = Bag_Word.wordState[kind][dataNum].power;
        data[count].MP = Bag_Word.wordState[kind][dataNum].MP;
        bool flag = false;
        if (Bag_Word.wordState[kind][dataNum].getFlag == 1)
        {
            flag = true;
        }
        data[count].getFlag = flag;
    }

    public void Init()
    {
        magicCreateParent = transform.Find("ScrollView/Viewport/WordParent").GetComponent<RectTransform>();
        // ���[�h�̍ő�����擾
        for (int i = 0; i < (int)InitPopList.WORD.INFO; i++)
        {
            selectKindMaxCnt_[i] = InitPopList.maxWordKindsCnt[i];
        }

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            mCreateData[(Bag_Word.WORD_MNG)i] = InitCheck((Bag_Word.WORD_MNG)i);
        }


        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();
        bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();

        // �쐬�J�n�{�^��
        createBtn_ = transform.Find("InfoMng/CreateBtn").GetComponent<Button>();
        createBtn_.interactable = false;
        createBtnText_ = createBtn_.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
        createBtnText_.text = "�m�F";
        // ���@�����I���{�^������
        cancelBtn_ = transform.Find("InfoMng/CancelBtn").GetComponent<Button>();

        //// �~�j�Q�[������
        circleMng_ = miniGameObj.transform.Find("ElementCircle/JudgeCircle").GetComponent<CircleMng>();
        miniGameObj.gameObject.SetActive(false);
        miniGameObj.transform.localPosition = new Vector3(-0.25f, 0.0f, 0.0f);

         // ScrollView/TopicBtn�܂ł̊K�w
         RectTransform viewParent_ = transform.Find("ScrollView/TopicBtn").GetComponent<RectTransform>();
        RectTransform infoParent_ = transform.Find("InfoMng/InfoBack").GetComponent<RectTransform>();

        // �E���̖��
        arrowBtn_[0] = viewParent_.transform.Find("LeftBtn").GetComponent<Button>();
        arrowBtn_[1] = viewParent_.transform.Find("RightBtn").GetComponent<Button>();

        // �ǂ����ނ̃��[�h����\������
        topicText_ = viewParent_.transform.Find("TopicText").GetComponent<TMPro.TextMeshProUGUI>();
        topicText_.text = topicString_[(int)Bag_Word.WORD_MNG.HEAD];

        // �I�񂾃��[�h��\��
        infoText_ = infoParent_.transform.Find("SelectWord").GetComponent<TMPro.TextMeshProUGUI>();
        selectWord_ = new string[(int)Bag_Word.WORD_MNG.MAX];
        targetWordNum_ = Bag_Word.targetWordNum;
        materiaNum_ = Bag_Materia.emptyMateriaNum;

        materiaImage_ = infoParent_.transform.Find("MateriaArea/MateriaImage").GetComponent<Image>();
        materiaCntText_ = infoParent_.transform.Find("MateriaArea/CountBack/Count").GetComponent<TMPro.TextMeshProUGUI>();
        if (0 < Bag_Materia.materiaState[materiaNum_].haveCnt)
        {
            // ��̃}�e���A��1�ł������Ă���Ε\��
            materiaImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum_];
            materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();
        }

        // �З͂�\��
        //   powerText_ = infoParent_.transform.Find("PowerArea/PowerText").GetComponent<TMPro.TextMeshProUGUI>();
        powerText_ = transform.Find("InfoMng/CheckBack/PowerArea/PowerText").GetComponent<TMPro.TextMeshProUGUI>();
        powerText_.text = "";
        mpText_ = transform.Find("InfoMng/CheckBack/MPArea/MPText").GetComponent<TMPro.TextMeshProUGUI>();
        mpText_.text = "";

        ResetCommon();

        // ���[�h�����𖂖@�����p�̐e�̎q�Ɉړ�������
        if (Bag_Word.wordState[InitPopList.WORD.HEAD][0].pleate.transform.parent != magicCreateParent.transform)
        {
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < selectKindMaxCnt_[k]; i++)
                {
                    Bag_Word.wordState[(InitPopList.WORD)k][i].pleate.transform.SetParent(magicCreateParent.transform);
                }
            }
            // Debug.Log("�e�̈ʒu�����炵�܂����B");
        }
        // ���[�h�̃A�N�e�B�u��Ԃ��`�F�b�N
        SelectWordKindCheck((Bag_Word.WORD_MNG)kindNum_);
    }

    public void OnClickRightArrow()
    {
        // �l�����Z
        kindNum_++;
        if (kindNum_ == (int)Bag_Word.WORD_MNG.SUB2)
        {
            if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "��")
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
                {
                    kindNum_ = (int)Bag_Word.WORD_MNG.SUB2;
                }
            }
            else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "�⏕")
            {
                if ((int)Bag_Word.WORD_MNG.SUB3 == kindNum_)
                {
                    kindNum_ = (int)Bag_Word.WORD_MNG.SUB3;
                }
            }
            else
            {
                // �U���nElement��I�����ɕK���������Ă��Ȃ�������Sub2�ŃX�g�b�v
                if (Bag_Word.wordState[InitPopList.WORD.SUB3][targetWordNum_].getFlag == 0)
                {
                    if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
                    {
                        kindNum_ = (int)Bag_Word.WORD_MNG.SUB2;
                    }
                }
                else
                {
                    if ((int)Bag_Word.WORD_MNG.SUB3 == kindNum_)
                    {
                        kindNum_ = (int)Bag_Word.WORD_MNG.SUB3;
                    }
                }
            }
        }
        // Debug.Log(selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
        ActiveKindsCheck((Bag_Word.WORD_MNG)kindNum_, false);
        //  Debug.Log("�E�����N���b�N" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        // ���[�h��ʂ�1�߂�Ƃ��ɂ��̎�ނ̃��[�h��I�����Ă����
        // �{�^���������ł���悤�ɂ��ĐF�����ɖ߂�
        if (saveNumber_[kindNum_] != -1)
        {
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.white;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = true;
            magicPower_ -= mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].power;
           // powerText_.text = magicPower_.ToString();
        }
        selectWord_[kindNum_] = null;
        oldNumber_[kindNum_] = -1;

        kindNum_--;

        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        ActiveKindsCheck((Bag_Word.WORD_MNG)kindNum_, true);
        Debug.Log("�������N���b�N" + kindNum_);
    }

    public void ActiveKindsCheck(Bag_Word.WORD_MNG kind, bool leftFlag)
    {
        topicText_.text = topicString_[(int)kind];
        createBtnText_.text = "�m�F";
        powerText_.text = "�v�Z��";
        mpText_.text = "�v�Z��";

        if (leftFlag == true)
        {
            // ����󂪓�����ő�ʒu
            if (kindNum_ <= (int)Bag_Word.WORD_MNG.HEAD)
            {
                kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
                // Head�̎��͉����ł��Ȃ��悤�ɂ���
                arrowBtn_[0].interactable = false;
            }
            arrowBtn_[1].interactable = true;
        }
        else
        {
            // �E��󂪓�����ő�ʒu
            if ((int)Bag_Word.WORD_MNG.SUB3 <= kindNum_)
            {
                kindNum_ = (int)Bag_Word.WORD_MNG.SUB3;
            }
            arrowBtn_[0].interactable = true;
            arrowBtn_[1].interactable = false;// �E������������K��false
        }
        Debug.Log((Bag_Word.WORD_MNG)kindNum_);
        // ���[�h�̃A�N�e�B�u��Ԃ��`�F�b�N
        SelectWordKindCheck((Bag_Word.WORD_MNG)kindNum_);

        // ���łɑI����Ԃ̃{�^���͉����ł��Ȃ��悤�ɂ���
        if (selectWord_[kindNum_] != null)
        {
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.green;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = false;
        }
    }

    private void SelectWordKindCheck(Bag_Word.WORD_MNG wordKind)
    {
        for (int k = 0; k < (int)Bag_Word.WORD_MNG.MAX; k++)
        {
            for (int i = 0; i < mngMaxCnt[k]; i++)
            {
                // ���ׂĔ�\���ɂ��Ă���
                mCreateData[(Bag_Word.WORD_MNG)k][i].pleate.SetActive(false);
            }
        }

        switch (wordKind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.HEAD]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue;// �����Ă��Ȃ����[�h�͕\�����Ȃ�
                    }
                    mCreateData[Bag_Word.WORD_MNG.HEAD][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.HEAD][i].pleate.SetActive(true);
                }
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.ELEMENT]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue; // �����Ă��Ȃ����[�h�͕\�����Ȃ�
                    }
                    mCreateData[Bag_Word.WORD_MNG.ELEMENT][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.ELEMENT][i].pleate.SetActive(true);
                }
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.TAIL:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.TAIL]; i++)
                {
                    if (mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].getFlag == false)
                    {
                        continue; // �����Ă��Ȃ����[�h�͕\�����Ȃ�
                    }
                    mCreateData[Bag_Word.WORD_MNG.TAIL][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.TAIL][i].pleate.SetActive(true);
                }
                createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null ? true : false;
                break;

            case Bag_Word.WORD_MNG.SUB1:
                ElementCheck(Bag_Word.WORD_MNG.SUB1, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.SUB2:
                ElementCheck(Bag_Word.WORD_MNG.SUB2, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                ElementCheck(Bag_Word.WORD_MNG.SUB3, selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
                createFlag_ = false;
                break;

            default:
                break;
        }
        createBtn_.interactable = createFlag_;
    }

    public void ElementCheck(Bag_Word.WORD_MNG kind, string selectWord)
    {
        // �G�������g�̑����`�F�b�N
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "��")
        {
            SetElementHeal(kind);
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "�⏕")
        {
            SetElementAssist(kind);
        }
        else
        {
            SetElementAttack(kind);
        }
    }

    private void SetElementHeal(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i++)
                {
                    CommonButtonCheck(true, "����", Bag_Word.WORD_MNG.SUB1, i);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB2]; i++)
                {
                    if (mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "HP"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "���"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "�È�"
                    || mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "��")
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = true;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SetElementAssist(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i++)
                {
                    // �G�Ɩ����̃��[�h���������\
                    mCreateData[Bag_Word.WORD_MNG.SUB1][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.SUB1][i].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB2]; i++)
                {
                    CommonButtonCheck(false, "HP", Bag_Word.WORD_MNG.SUB2, i);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = true;
                    mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(true);
                    // �G��I�����Ă����ꍇ
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == "�G")
                    {
                        CommonButtonCheck(true, "�ቺ", Bag_Word.WORD_MNG.SUB3, i);
                    }
                    else
                    {
                        //// ������I�����Ă����ꍇ
                        if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].name == "�K��"
                            || mCreateData[Bag_Word.WORD_MNG.SUB3][i].name == "�ቺ")
                        {
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = false;
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(false);
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    private void SetElementAttack(Bag_Word.WORD_MNG kind)
    {
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = selectKindMaxCnt_[(int)InitPopList.WORD.SUB1]; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB1]; i++)
                {
                    CommonButtonCheck(false, "�K��", Bag_Word.WORD_MNG.SUB1, i);
                    // �z���͔�\��
                    if (mCreateData[Bag_Word.WORD_MNG.SUB1][i].name == "�z��")
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB1][i].btn.interactable = false;
                        mCreateData[Bag_Word.WORD_MNG.SUB1][i].pleate.SetActive(false);
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = selectKindMaxCnt_[(int)InitPopList.WORD.SUB2]; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB2]; i++)
                {
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] != mCreateData[Bag_Word.WORD_MNG.SUB2][i].name)
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = true;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                    }
                    else
                    {
                        // Sub1�őI���������[�h�͉����ł��Ȃ��悤��
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(true);
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = false;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.image.color = Color.green;
                    }

                    // �z���͔�\��
                    if (mCreateData[Bag_Word.WORD_MNG.SUB2][i].name == "�z��")
                    {
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].btn.interactable = false;
                        mCreateData[Bag_Word.WORD_MNG.SUB2][i].pleate.SetActive(false);
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    Debug.Log(mCreateData[Bag_Word.WORD_MNG.SUB3][i].name);
                    if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].name == "�K��")
                    {
                        // �K���������Ă�����
                        if (mCreateData[Bag_Word.WORD_MNG.SUB3][i].getFlag == true)
                        {
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = true;
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(true);
                        }
                    }
                    else
                    {
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].btn.interactable = false;
                            mCreateData[Bag_Word.WORD_MNG.SUB3][i].pleate.SetActive(false);
                    }

                }
                break;

            default:
                break;
        }
    }

    private void CommonButtonCheck(bool flag, string name, Bag_Word.WORD_MNG kinds, int num)
    {
        // true�Ȃ灁���@false�Ȃ�I��
        if (flag == true)
        {
            mCreateData[kinds][num].btn.interactable = mCreateData[kinds][num].name == name ? true : false;
            mCreateData[kinds][num].pleate.SetActive(mCreateData[kinds][num].btn.interactable);
            // mCreateState[kinds][num].name��name�Ȃ�interactable��true�������
        }
        else
        {
            mCreateData[kinds][num].btn.interactable = mCreateData[kinds][num].name != name ? true : false;
            mCreateData[kinds][num].pleate.SetActive(mCreateData[kinds][num].btn.interactable);
            // mCreateState[kinds][num].name��name�ł͂Ȃ��{�^����interactable��true�������
        }
    }

    public void SetWord(string word)
    {
        // �[�[�[�[�[�����[�h�I������
        // ���[�h�̃{�^��������������Ăяo��
        for (int i = 0; i < mngMaxCnt[kindNum_]; i++)
        {
            // �ǂ̎�ނ̎��ɂǂ̃��[�h���������ꂽ��
            if (word == mCreateData[(Bag_Word.WORD_MNG)kindNum_][i].name)
            {
                // �������ꂽ���[�h�̔ԍ���������
                saveNumber_[kindNum_] = i;
                saveWordNum_[kindNum_] = i;
                break;
            }
        }

        // -1�ȊO�͓������[�h��ʓ��ŕ����̃{�^���������ꂽ�Ƃ�
        if (oldNumber_[kindNum_] != -1)
        {
            // 1�O�ɉ����ꂽ�{�^���͏�����Ԃɖ߂�
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][oldNumber_[kindNum_]].btn.image.color = Color.white;
            mCreateData[(Bag_Word.WORD_MNG)kindNum_][oldNumber_[kindNum_]].btn.interactable = true;
            // �I�����Ă������̃��[�h�̈З͂����炷
            magicPower_ -= mCreateData[(Bag_Word.WORD_MNG)kindNum_][oldNumber_[kindNum_]].power;
        }


        // �I���������[�h�̐F��΂ɂ��đI���ł��Ȃ��悤�ɂ���
        mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.image.color = Color.green;
        mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].btn.interactable = false;

        //// �g�s�b�N����ǂ̎�ނ̎��ɑI�΂ꂽ���[�h�Ȃ̂�������
        selectWord_[kindNum_] = mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].name;
        //  seleEngWord_[kindNum_] = mCreateData[(Bag_Word.WORD_MNG)kindNum_][saveNumber_[kindNum_]].englishName;
        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        // ���s�Ȃ��ō��ꂽ���O��ۑ����Ă���
        allName_ = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                   selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                   selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                   selectWord_[(int)Bag_Word.WORD_MNG.SUB3];


        // �[�[�[�[�[�����[�h�I�����������܂�

        // �[�[�[�[�[�Z�{�^���n��interactable�`�F�b�N

        // �ˑR�̎O�����Z�q���~�l���P���I�I�I�I�I�I�I�I
        // Sub3�̃��[�h��I��������E�����A�N�e�B�u�ɂ���
        arrowBtn_[1].interactable = (int)Bag_Word.WORD_MNG.SUB3 == kindNum_ ? false : true;
        // kindNum_��(int)Bag_Word.WORD_MNG.SUB3�Ȃ�arrowBtn_[1].interactable��false�������
        // �Ⴄ�Ȃ�arrowBtn_[1].interactable��true�������

        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "��")
        {
            // Element�Łu�񕜁v���[�h��I�����Ă�����
            if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
            {
                // Sub3���[�h��I���ł��Ȃ��悤�ɂ���
                arrowBtn_[1].interactable = false;

                // Element�ŉ񕜑I������Sub2��I�����邱�Ƃō쐬���ł���
                createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.SUB2] != null ? true : false;
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "�⏕")
        {
            createFlag_ = selectWord_[(int)Bag_Word.WORD_MNG.SUB3] != null ? true : false;
        }
        else
        {
            // �U���n��Sub�͂ǂ̃^�C�~���O�őI��ł��쐬���ł���
            if ((int)Bag_Word.WORD_MNG.TAIL <= kindNum_)
            {
                createFlag_ = selectWord_[kindNum_] != null ? true : false;
            }
            // �U���nElement�I�����Ɂu�K���v�������Ă��Ȃ���
            // Sab2�̎��_�Łu�K���v��I����
            if (Bag_Word.wordState[InitPopList.WORD.SUB3][targetWordNum_].getFlag == 0
             || selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == "�K��")
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= kindNum_)
                {
                    arrowBtn_[1].interactable = false;// �E������������K��false
                }
            }
        }

        if (kindNum_ == (int)Bag_Word.WORD_MNG.TAIL)
        {
            // Tail�I������Head�AElement�ATail��null�ł͂Ȃ�
            if (selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null)
            {
                createFlag_ = true;
            }
        }

        createBtn_.interactable = createFlag_;
        oldNumber_[kindNum_] = saveNumber_[kindNum_];
    }

    public void OnClickMagicCreate()
    {
        if (createBtnText_.text == "�쐬")
        {
            // ��̃}�e���A��1�ȏ㎝���Ă邩�`�F�b�N
            if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
            {
                return;
            }

            // ��̃}�e���A�̏����������炷
            Bag_Materia.materiaState[materiaNum_].haveCnt--;
            materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();

            miniGameObj.gameObject.SetActive(true);
            var judgeNum = (mpPower_ / maxMP * 100)/25;
            circleMng_.Init(saveWordNum_[(int)Bag_Word.WORD_MNG.ELEMENT], judgeNum);


            // �Q�[�����n�܂邽�߉����ł��Ȃ��悤�ɂ���
            createBtn_.interactable = false;
            cancelBtn_.interactable = false;
            arrowBtn_[0].interactable = false;
            arrowBtn_[1].interactable = false;
        }
        else
        {
            mpPower_ = 0;
            magicPower_ = 0;

            int maxWordMng_ = (int)Bag_Word.WORD_MNG.MAX;
            if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == null)
            {
                // Sub1���Z�b�g����ĂȂ�������h�����Tail�܂�
                maxWordMng_ = (int)Bag_Word.WORD_MNG.SUB1;
            }
            else if (selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == null)
            {
                // Sub2���Z�b�g����ĂȂ�������v�Z��SUB1�܂�
                maxWordMng_ = (int)Bag_Word.WORD_MNG.SUB2;
            }
            else if (selectWord_[(int)Bag_Word.WORD_MNG.SUB3] == null)
            {
                // SUB3���Z�b�g����ĂȂ�������v�Z��SUB2�܂�
                maxWordMng_ = (int)Bag_Word.WORD_MNG.SUB3;
            }
            else
            {
                //// SUB3���Z�b�g����ĂȂ�������v�Z��SUB3�܂�
                // �������Ȃ� maxWordMng_ = (int)Bag_Word.WORD_MNG.TAIL;
            }
          for (int i = (int)Bag_Word.WORD_MNG.HEAD; i < maxWordMng_; i++)
            {
                Debug.Log(mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].name + "      " + mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].power);
                mpPower_ += mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].MP;
                if (i == (int)Bag_Word.WORD_MNG.TAIL)
                {
                    continue;
                }
                // �З͂͑���������Tail�������邽��Tail�̎��͓���Ȃ��悤�ɂ���
                magicPower_ += mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].power;
            }
            float tailPower = mCreateData[Bag_Word.WORD_MNG.TAIL][saveNumber_[(int)Bag_Word.WORD_MNG.TAIL]].power;
            if (selectWord_[(int)Bag_Word.WORD_MNG.HEAD] == "������")
            {
                tailPower = Mathf.Abs(mCreateData[Bag_Word.WORD_MNG.TAIL][saveNumber_[(int)Bag_Word.WORD_MNG.TAIL]].power - 0.2f);
            }
            Debug.Log(magicPower_ + "*" + tailPower);

            magicPower_ = (int)(magicPower_ * tailPower);
            powerText_.text = magicPower_.ToString();
            mpText_.text = mpPower_.ToString();
            createBtnText_.text = "�쐬";
        }
    }

    public void ResultMagicCreate()
    {
        int rate = (int)circleMng_.GetMiniGameJudge();
        int power = magicPower_;
        int mp = mpPower_;
        // ���s�ȊO�̏ꍇ�͖��@��ۑ�����
        if (rate != (int)MovePoint.JUDGE.BAD)
        {
            // �听���̎��͈З͂��グ��MP����ʂ�����������
            if (rate == (int)CircleMng.JUDGE.GOOD)
            {
                power = (int)(magicPower_ * 1.3);// �З͂��グ��
                mp -= (int)(magicPower_ * 0.3);// ����MP��������
            }

            // ���[�h���ǂ��܂ŃZ�b�g����Ă��邩�m�F����
            if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == null)
            {
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB1] = -1;
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB2] = -1;
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB3] = -1;
            }
            else if (selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == null)
            {
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB2] = -1;
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB3] = -1;
            }
            else if (selectWord_[(int)Bag_Word.WORD_MNG.SUB3] == null)
            {
                saveWordNum_[(int)Bag_Word.WORD_MNG.SUB3] = -1;
            }
            else
            {
                // �������Ȃ�
            }

            // �o���オ�������@��ۑ�
            bagMagic_.MagicCreateCheck(allName_,
                        power, mp, rate,
                        saveWordNum_[(int)Bag_Word.WORD_MNG.HEAD],
                        saveWordNum_[(int)Bag_Word.WORD_MNG.ELEMENT],
                        saveWordNum_[(int)Bag_Word.WORD_MNG.TAIL],
                        saveWordNum_[(int)Bag_Word.WORD_MNG.SUB1],
                        saveWordNum_[(int)Bag_Word.WORD_MNG.SUB2],
                        saveWordNum_[(int)Bag_Word.WORD_MNG.SUB3]);
        }

      //  miniGameObj.gameObject.SetActive(false);
        cancelBtn_.interactable = true;
        if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
        {
            Debug.Log("��̃}�e���A���Ȃ��Ȃ�܂����B���[�h�������I�����܂�");
            OnClickCancelBtn();
            return;
        }

        ResetCommon();
    }

    public void OnClickCancelBtn()
    {
        // ���@�̍�������߂�
        ResetCommon();
        bagWord_.SetGetFlagCheck(false);
        GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().Init();
        this.gameObject.SetActive(false);
        uniHouseCanvas.gameObject.SetActive(true);
    }

    private void ResetCommon()
    {
        // Init()�ƃ��[�h������ƃ��[�h�����I�����ɌĂ�
        //    miniGameMng.localPosition = new Vector3(0.0f, -180.0f, 0.0f);

        infoText_.text = "";
        allName_ = "";
        magicPower_ = 0;
        mpPower_ = 0;
        powerText_.text = "";
        mpText_.text = "";
        kindNum_ = (int)Bag_Word.WORD_MNG.HEAD;
        topicText_.text = topicString_[kindNum_];

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            // ��x�ł�������ꂽ���Ƃ�����Ȃ珉��������
            if (saveNumber_[i] != -1)
            {
                mCreateData[(Bag_Word.WORD_MNG)i][saveNumber_[i]].btn.image.color = Color.white;
                selectWord_[i] = null;
                //  seleEngWord_[i] = null;
                // �N�ɂ��Y�����Ȃ��ԍ�������
                saveNumber_[i] = -1;
                oldNumber_[i] = saveNumber_[i];
            }
        }

        for (int k = 0; k < (int)Bag_Word.WORD_MNG.MAX; k++)
        {
            // ���ׂĔ�\���ɂ��Ă���
            for (int i = 0; i < mngMaxCnt[k]; i++)
            {
                //Debug.Log((Bag_Word.WORD_MNG)k + "       " + i + "      " + mCreateData[(Bag_Word.WORD_MNG)k][i].name);
                mCreateData[(Bag_Word.WORD_MNG)k][i].btn.interactable = false;
                mCreateData[(Bag_Word.WORD_MNG)k][i].pleate.gameObject.SetActive(false);
            }
        }
        // �w�b�h�̃��[�h�͕\�������Ă���
        for (int i = 0; i < mngMaxCnt[(int)InitPopList.WORD.HEAD]; i++)
        {
            if (Bag_Word.wordState[InitPopList.WORD.HEAD][i].getFlag == 1)
            {
                mCreateData[(int)Bag_Word.WORD_MNG.HEAD][i].btn.interactable = true;
                mCreateData[(int)Bag_Word.WORD_MNG.HEAD][i].pleate.SetActive(true);
            }
        }
        arrowBtn_[0].interactable = false;
        arrowBtn_[1].interactable = false;
        createFlag_ = false;
        createBtn_.interactable = createFlag_;
        createBtnText_.text = "�m�F";
    }
}