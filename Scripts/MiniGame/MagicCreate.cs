using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCreate : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas;

    [SerializeField]
    private RectTransform miniGameMng;    // �~�j�Q�[���\���p

 //   [SerializeField]
    private RectTransform magicCreateParent;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    // ���[�h�̍ő���擾�p
    private InitPopList popWordList_;
    private int maxCnt_ = 0;

    // �o�b�O�̒��g�i���[�h�j
    private int stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
    // ��ʉ����A�����֘A
    private string[] topicString_ = new string[(int)Bag_Word.WORD_MNG.MAX] {
    "Head","Element","Tail","Sub1","Sub2","Sub3"};
    private Text topicText_;    // �ǂ̃��[�h�̎�ނ�\�����Ă��邩

    // �ǂ��I�����Ă��邩��\��
    private Text infoText_;
    private string[] selectWord_;
    private int targetWordNum_;// �K�����[�h�̔ԍ���ۑ�
    private int materiaNum_;//��̃}�e���A�̔ԍ���ۑ�
    private Image materiaImage_;// ��̃}�e���A�摜�`��
    private Text materiaCntText_;// ��̃}�e���A�̏�������`��

    //  0.�쐬�J�n�{�^���@1.���@�����I���{�^��
    private bool createFlag_ = false;
    private Button createBtn_;
    private Button cancelBtn_;
    private Bag_Magic bagMagic_;
    private string allName_ = "";
    private int[] saveNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];// �I���������[�h�̔ԍ���ۑ�
    private int[] oldNumber_ = new int[(int)Bag_Word.WORD_MNG.MAX];

    // ���{�^��
    private Button[] arrowBtn_ = new Button[2];

    // �~�j�Q�[���X�^�[�g�p
    private MovePoint movePoint_;
    private Image judgeBack_;
    private Text judgeText_;

    public void Init()
    {
        magicCreateParent = transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        // ���[�h�̍ő�����擾
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        maxCnt_ = popWordList_.SetMaxWordCount();
        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();


        // �쐬�J�n�{�^��
        createBtn_ = transform.Find("InfoMng/CreateBtn").GetComponent<Button>();
        createBtn_.interactable = false;

        // ���@�����I���{�^������
        cancelBtn_ = transform.Find("InfoMng/CancelBtn").GetComponent<Button>();

        // �~�j�Q�[������
        movePoint_ = miniGameMng.transform.GetComponent<MovePoint>();
        judgeBack_ = movePoint_.transform.Find("JudgeBack").GetComponent<Image>();
        judgeText_ = judgeBack_.transform.Find("Text").GetComponent<Text>();
        judgeBack_.gameObject.SetActive(false);
        miniGameMng.gameObject.SetActive(false);
        judgeText_.text = "";

        // ScrollView/TopicBtn�܂ł̊K�w
        RectTransform viewParent_ = transform.Find("ScrollView/TopicBtn").GetComponent<RectTransform>();
        RectTransform infoParent_ = transform.Find("InfoMng/InfoBack").GetComponent<RectTransform>();

        // �E���̖��
        arrowBtn_[0] = viewParent_.transform.Find("LeftBtn").GetComponent<Button>();
        arrowBtn_[1] = viewParent_.transform.Find("RightBtn").GetComponent<Button>();

        // �ǂ����ނ̃��[�h����\������
        topicText_ = viewParent_.transform.Find("TopicText").GetComponent<Text>();
        topicText_.text = topicString_[(int)Bag_Word.WORD_MNG.HEAD];

        // �I�񂾃��[�h��\��
        infoText_ = infoParent_.transform.Find("SelectWord").GetComponent<Text>();
        selectWord_ = new string[(int)Bag_Word.WORD_MNG.MAX];
        targetWordNum_ = Bag_Word.targetWordNum;
        materiaNum_ = Bag_Materia.emptyMateriaNum;

        materiaImage_ = infoParent_.transform.Find("MateriaArea/MateriaImage").GetComponent<Image>();
        materiaCntText_ = infoParent_.transform.Find("MateriaArea/CountBack/Count").GetComponent<Text>();
        if (0 < Bag_Materia.materiaState[materiaNum_].haveCnt)
        {
            // ��̃}�e���A��1�ł������Ă���Ε\��
            materiaImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum_];
            materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();
        }
        ResetCommon();

        // ���[�h�����𖂖@�����p�̐e�̎q�Ɉړ�������
        if (Bag_Word.wordState[0].pleate.transform.parent != magicCreateParent.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                Bag_Word.wordState[i].pleate.transform.SetParent(magicCreateParent.transform);
            }
            //  Debug.Log("�e�̈ʒu�����炵�܂����B");
        }
    }

    public void OnClickRightArrow()
    {
        // �l�����Z
        stringNum_++;
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "��"
         && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "�⏕")
        {
            // �U���nElement��I�����ɕK���������Ă��Ȃ�������Sub2�ŃX�g�b�v
            if (Bag_Word.wordState[targetWordNum_].getFlag == false)
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
                {
                    stringNum_ = (int)Bag_Word.WORD_MNG.SUB2;
                }
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "��")
        {
            if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.SUB2;
            }
        }
        else
        {
            // �������Ȃ�
        }
        // Debug.Log(selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
        ActiveKindsCheck((Bag_Word.WORD_MNG)stringNum_, false);
        //  Debug.Log("�E�����N���b�N" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        // ���[�h��ʂ�1�߂�Ƃ��ɂ��̎�ނ̃��[�h��I�����Ă����
        //  Debug.Log("1�O�̎�ނɖ߂�܂��B");
        // �{�^���������ł���悤�ɂ��ĐF�����ɖ߂�
        if (saveNumber_[stringNum_] != -1)
        {
            Bag_Word.wordState[saveNumber_[stringNum_]].btn.image.color = Color.white;
            Bag_Word.wordState[saveNumber_[stringNum_]].btn.interactable = true;
        }
        selectWord_[stringNum_] = null;
        oldNumber_[stringNum_] = -1;

        stringNum_--;

        Bag_Word.wordState[saveNumber_[stringNum_]].btn.image.color = Color.green;
        Bag_Word.wordState[saveNumber_[stringNum_]].btn.interactable = false;

        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
          "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        ActiveKindsCheck((Bag_Word.WORD_MNG)stringNum_, true);


        Debug.Log("�������N���b�N" + stringNum_);
    }

    public void ActiveKindsCheck(Bag_Word.WORD_MNG kind, bool leftFlag)
    {
        topicText_.text = topicString_[(int)kind];

        if (leftFlag == true)
        {
            // ����󂪓�����ő�ʒu
            if (stringNum_ <= (int)Bag_Word.WORD_MNG.HEAD)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
                // Head�̎��͉����ł��Ȃ��悤�ɂ���
                arrowBtn_[0].interactable = false;
            }
            arrowBtn_[1].interactable = true;
        }
        else
        {
            // �E��󂪓�����ő�ʒu
            if ((int)Bag_Word.WORD_MNG.SUB3 <= stringNum_)
            {
                stringNum_ = (int)Bag_Word.WORD_MNG.SUB3;
            }
            arrowBtn_[0].interactable = true;
            arrowBtn_[1].interactable = false;// �E������������K��false
        }

        Debug.Log(topicText_.text);
        for (int i = 0; i < maxCnt_; i++)
        {
            // �S�Ĕ�\���ɂ��Ă���
            Bag_Word.wordState[i].pleate.SetActive(false);
            // �擾���Ă��邩
            if (Bag_Word.wordState[i].getFlag == true)
            {
                // ���[�h�̎�ނƈ�v���Ă���ԍ������\��
                SelectWordKindCheck(i, (Bag_Word.WORD_MNG)stringNum_);
            }
        }

        // Element�ɉ��炩�̃��[�h�������Ă�����T�u�ŕ\�����郏�[�h���`�F�b�N����
        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != null)
        {
            ElementCheck(kind);
        }

        // ���łɑI����Ԃ̃{�^���͉����ł��Ȃ��悤�ɂ���
        if (selectWord_[stringNum_] != null)
        {
            Bag_Word.wordState[saveNumber_[stringNum_]].btn.interactable = false;
        }
    }

    private void SelectWordKindCheck(int wordNum, Bag_Word.WORD_MNG wordKind)
    {
        switch (wordKind)
        {
            case Bag_Word.WORD_MNG.HEAD:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.HEAD)
                {
                    createBtn_.interactable = false;
                    Bag_Word.wordState[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ELEMENT_ASSIST
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ELEMENT_HEAL
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ELEMENT_ATTACK)
                {
                    createBtn_.interactable = false;
                    Bag_Word.wordState[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.TAIL:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.TAIL)
                {
                    Bag_Word.wordState[wordNum].btn.interactable = true;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                    if (selectWord_[(int)Bag_Word.WORD_MNG.HEAD] != null
                    && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != null
                    && selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null)
                    {
                        createBtn_.interactable = true;
                    }
                    else
                    {
                        createBtn_.interactable = false;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB1:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.SUB1
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.SUB1_AND_SUB2
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.SUB2
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.SUB1_AND_SUB2
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                if (Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.SUB3
                 || Bag_Word.wordState[wordNum].kinds == InitPopList.WORD.ALL_SUB)
                {
                    Bag_Word.wordState[wordNum].btn.interactable = false;//#
                    Bag_Word.wordState[wordNum].pleate.SetActive(true);
                }
                break;

            default:
                break;
        }
    }

    public void ElementCheck(Bag_Word.WORD_MNG kind)
    {
        // Debug.Log("�I������Element�̃��[�h�F"+selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT]);
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
                Debug.Log("interactable��true�ɂ��Ă���");
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "����", true, InitPopList.WORD.SUB1);
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "HP", true, InitPopList.WORD.SUB2);
                    SetInteractableCheck(i, "����", false, InitPopList.WORD.SUB1_AND_SUB2);
                }
                // �쐬�J�n�{�^���������\��Ԃɂ���
                createFlag_ = true;
                //createBtn_.interactable = true;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                // Element�ŉ񕜑I������Sub3�őI���ł�����̂͂Ȃ�
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
                for (int i = 0; i < maxCnt_; i++)
                {
                    // �G�Ɩ����̃��[�h���������\
                    if (Bag_Word.wordState[i].kinds == InitPopList.WORD.SUB1)
                    {
                        Bag_Word.wordState[i].btn.interactable = true;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < maxCnt_; i++)
                {
                    SetInteractableCheck(i, "HP", false, InitPopList.WORD.SUB2);
                }
                break;

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] == "�G")
                    {
                        SetInteractableCheck(i, "�ቺ", true, InitPopList.WORD.SUB3);
                    }
                    else
                    {
                        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB2] == "�h���")
                        {
                            SetInteractableCheck(i, "�㏸", true, InitPopList.WORD.SUB3);
                        }
                        else
                        {
                            if (Bag_Word.wordState[i].kinds == InitPopList.WORD.SUB3
                             || Bag_Word.wordState[i].kinds == InitPopList.WORD.ALL_SUB)
                            {
                                // �ቺ�ȊO�������\
                                if (Bag_Word.wordState[i].name == "�㏸"
                                    || Bag_Word.wordState[i].name == "����"
                                    || Bag_Word.wordState[i].name == "�z��")
                                {
                                    Bag_Word.wordState[i].btn.interactable = true;
                                }
                            }
                        }
                    }
                }
                createFlag_ = true;
                //createBtn_.interactable = true;
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
                // InteractableWordPleate(null, PopMateriaList.WORD.SUB1, PopMateriaList.WORD.SUB1_AND_SUB2);
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (Bag_Word.wordState[i].kinds == InitPopList.WORD.SUB1_AND_SUB2
                        || Bag_Word.wordState[i].kinds == InitPopList.WORD.ALL_SUB)
                    {
                        Bag_Word.wordState[i].btn.interactable = true;
                    }
                }
                break;

            case Bag_Word.WORD_MNG.SUB2:
                // HP���������ł��Ȃ���Ԃ�
                // InteractableWordPleate("HP", PopMateriaList.WORD.SUB2, PopMateriaList.WORD.SUB1_AND_SUB2);
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (Bag_Word.wordState[i].kinds == InitPopList.WORD.SUB1_AND_SUB2
                    || Bag_Word.wordState[i].kinds == InitPopList.WORD.ALL_SUB)
                    {
                        if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] != Bag_Word.wordState[i].name)
                        {
                            // Sub1�őI���������[�h�ȊO�������\��
                            Bag_Word.wordState[i].btn.interactable = true;
                        }
                    }
                }
                // �쐬�{�^�������\�ɂ���
                createFlag_ = true;
                //createBtn_.interactable = true;
                break;

            case Bag_Word.WORD_MNG.SUB3:
                // �K���������Ă�����
                if (Bag_Word.wordState[targetWordNum_].getFlag == true)
                {
                    // �K����\��
                    Bag_Word.wordState[targetWordNum_].btn.interactable = true;
                }
                // �쐬�J�n�{�^���������\��Ԃɂ���
                createFlag_ = true;
                //createBtn_.interactable = true;
                break;

            default:
                break;
        }
    }
    private void SetInteractableCheck(int number, string num, bool flag, InitPopList.WORD word)
    {
        // name��==�Ŕ��f����ꍇ��flag��true
        if (flag == true)
        {
            if (Bag_Word.wordState[number].kinds == word)
            {
                // �㏸�̂݉����ł���
                if (Bag_Word.wordState[number].name == num)
                {
                    Bag_Word.wordState[number].btn.interactable = true;
                }
            }
        }
        else
        {
            if (Bag_Word.wordState[number].kinds == word)
            {
                if (Bag_Word.wordState[number].name != num)
                {
                    Bag_Word.wordState[number].btn.interactable = true;
                }
            }
        }
    }

    public void SetWord(string word)
    {
        if ((int)Bag_Word.WORD_MNG.SUB3 == stringNum_)
        {
            // �T�u3��\�����͉E�����A�N�e�B�u�ɂ���
            arrowBtn_[1].interactable = false;
        }
        else
        {
            arrowBtn_[1].interactable = true;
        }

        if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "��"
         && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != "�⏕")
        {
            if (Bag_Word.wordState[targetWordNum_].getFlag == false)
            {
                if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
                {
                    arrowBtn_[1].interactable = false;// �E������������K��false
                }
            }
        }
        else if (selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] == "��")
        {
            if ((int)Bag_Word.WORD_MNG.SUB2 <= stringNum_)
            {
                arrowBtn_[1].interactable = false;// �E������������K��false
            }
        }
        else
        {
            // �������Ȃ�
        }


        // ���[�h�̃{�^��������������Ăяo��
        for (int i = 0; i < maxCnt_; i++)
        {
            // �ǂ̎�ނ̎��ɂǂ̃��[�h���������ꂽ��
            if (word == Bag_Word.wordState[i].name)
            {
                // �������ꂽ���[�h�̔ԍ���������
                saveNumber_[stringNum_] = i;
                break;
            }
        }

        // -1�ȊO�͓������[�h��ʓ��ŕ����̃{�^���������ꂽ�Ƃ�
        if (oldNumber_[stringNum_] != -1)
        {
            // 1�O�ɉ����ꂽ�{�^���͏�����Ԃɖ߂�
            Bag_Word.wordState[oldNumber_[stringNum_]].btn.image.color = Color.white;
            Bag_Word.wordState[oldNumber_[stringNum_]].btn.interactable = true;
        }

        // Sub2��I��
        if (stringNum_ == (int)Bag_Word.WORD_MNG.SUB2)
        {
            // 1�O��Sub�őI�����Ă������͉̂�����Ԃ̂܂܂ɂ���
            Bag_Word.wordState[saveNumber_[stringNum_ - 1]].btn.image.color = Color.green;
            Bag_Word.wordState[saveNumber_[stringNum_ - 1]].btn.interactable = false;
        }
        //Debug.Log(Bag_Word.wordState_[saveNumber_[stringNum_]].btn.name);

        // �I���������[�h�̐F��΂ɂ��đI���ł��Ȃ��悤�ɂ���
        Bag_Word.wordState[saveNumber_[stringNum_]].btn.image.color = Color.green;
        Bag_Word.wordState[saveNumber_[stringNum_]].btn.interactable = false;

        // �g�s�b�N����ǂ̎�ނ̎��ɑI�΂ꂽ���[�h�Ȃ̂�������
        selectWord_[stringNum_] = Bag_Word.wordState[saveNumber_[stringNum_]].name;
        infoText_.text = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  "\n" + selectWord_[(int)Bag_Word.WORD_MNG.SUB3];

        allName_ = selectWord_[(int)Bag_Word.WORD_MNG.HEAD] +
                  selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] +
                  selectWord_[(int)Bag_Word.WORD_MNG.TAIL] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB1] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB2] +
                  selectWord_[(int)Bag_Word.WORD_MNG.SUB3];// ���ꂽ���O��ۑ����Ă���

        if (stringNum_ == (int)Bag_Word.WORD_MNG.TAIL)
        {
            if (selectWord_[(int)Bag_Word.WORD_MNG.HEAD] != null
                && selectWord_[(int)Bag_Word.WORD_MNG.ELEMENT] != null
                && selectWord_[(int)Bag_Word.WORD_MNG.TAIL] != null)
            {
                createFlag_ = true;
            }
        }
        if (stringNum_ == (int)Bag_Word.WORD_MNG.SUB1)
        {
            if (selectWord_[(int)Bag_Word.WORD_MNG.SUB1] != null)
            {
                createFlag_ = false;
            }
        }

        if (createFlag_ == true)
        {
            Debug.Log("�쐬�J�n�{�^���������܂�");
            createBtn_.interactable = true;
        }
        else
        {
            Debug.Log("�쐬�J�n�{�^���������Ȃ��ł�");
            createBtn_.interactable = false;
        }



        oldNumber_[stringNum_] = saveNumber_[stringNum_];
    }

    public void OnClickMagicCreate()
    {
        // ��̃}�e���A��1�ȏ㎝���Ă邩�`�F�b�N
        if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
        {
            return;
        }

        for (int i = 0; i < bagMagic_.MagicNumber(); i++)
        {
            if (allName_ == Bag_Magic.data[0].name)
            {
                Debug.Log(allName_ + "�͂��łɐ�������Ă��܂�");
                return;
            }
        }

        // ��̃}�e���A�̏����������炷
        Bag_Materia.materiaState[materiaNum_].haveCnt--;
        materiaCntText_.text = Bag_Materia.materiaState[materiaNum_].haveCnt.ToString();

        StartCoroutine(movePoint_.CountDown());
        StartCoroutine(ResultMagicCreate());
        miniGameMng.gameObject.SetActive(true);

        // �Q�[�����n�܂邽�߉����ł��Ȃ��悤�ɂ���
        createBtn_.interactable = false;
        cancelBtn_.interactable = false;
    }

    public IEnumerator ResultMagicCreate()
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

                movePoint_.SetMiniGameJudge(MovePoint.JUDGE.NON);// ���������Ă���
                bagMagic_.MagicCreateCheck(allName_, 100, 5, 5);// �o���オ�������@��ۑ�

                yield return new WaitForSeconds(2.0f);
                cancelBtn_.interactable = true;
                judgeBack_.gameObject.SetActive(false);
                miniGameMng.gameObject.SetActive(false);
                judgeText_.text = "";

                if (Bag_Materia.materiaState[materiaNum_].haveCnt < 1)
                {
                    Debug.Log("��̃}�e���A���Ȃ��Ȃ�܂����B���[�h�������I�����܂�");
                    OnClickCancelCtn();
                    yield break;
                }

                ResetCommon();
                yield break;
            }
        }
    }

    public void OnClickCancelCtn()
    {
        // ���@�̍�������߂�
        ResetCommon();
        this.gameObject.SetActive(false);
        uniHouseCanvas.gameObject.SetActive(true);
    }

    private void ResetCommon()
    {
        // Init()�ƃ��[�h������ƃ��[�h�����I�����ɌĂ�
        miniGameMng.localPosition = new Vector3(0.0f, -180.0f, 0.0f);

        createFlag_ = false;

        arrowBtn_[0].interactable = false;
        arrowBtn_[1].interactable = false;
        infoText_.text = "";
        allName_ = "";
        stringNum_ = (int)Bag_Word.WORD_MNG.HEAD;
        topicText_.text = topicString_[stringNum_];

        for (int i = 0; i < (int)Bag_Word.WORD_MNG.MAX; i++)
        {
            // ��x�ł�������ꂽ���Ƃ�����Ȃ珉��������
            if (saveNumber_[i] != -1)
            {
                Bag_Word.wordState[saveNumber_[i]].btn.image.color = Color.white;
                selectWord_[i] = null;
                // �N�ɂ��Y�����Ȃ��ԍ�������
                saveNumber_[i] = -1;
                oldNumber_[i] = saveNumber_[i];
            }
        }

        for (int i = 0; i < maxCnt_; i++)
        {
            Bag_Word.wordState[i].pleate.gameObject.SetActive(false);//#
            Bag_Word.wordState[i].btn.interactable = false;//#

            if (Bag_Word.wordState[i].getFlag == true)
            {
                // ���[�h�̎�ނƈ�v���Ă��邩
                if (InitPopList.WORD.HEAD == Bag_Word.wordState[i].kinds)
                {
                    Bag_Word.wordState[i].pleate.gameObject.SetActive(true);
                    Bag_Word.wordState[i].btn.interactable = true;//#
                    //Bag_Word.wordState_[i].btn.image.color = Color.clear;
                }
            }
        }
    }

}