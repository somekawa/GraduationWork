using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private int btnNumber_ = 0;

    [SerializeField]
    private Sprite[] CharaImage;
    [SerializeField]
    private RectTransform statusMngObj;

    private RectTransform itemBagChild_;// itemBag_�̎q

    public enum TOPIC
    {
        NON = -1,
        ITEM,// �A�C�e��
        MATERIA,// �f��
        WORD,// ���[�h
        MAGIC,
        MAX,
    }
    // �A�C�e���I�����ɕ\������g�s�b�N��
    private Text topicText_;
    private int stringNum_ = (int)TOPIC.ITEM;
    private string[] topicString_ = new string[(int)TOPIC.MAX] {
    "�A�C�e��","�}�e���A","���[�h","�}�W�b�N"};
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button[] throwAwayBtn_ = new Button[(int)TOPIC.MAGIC];

    // �X�e�[�^�X�\���ɂłĂ���g�s�b�N��
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // �䎆�T�C�Y�ύX�p
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "���j","�W���b�N"};

    private Bag_Magic bagMagic_;
    // �X�e�[�^�X�֘A ���@
    private int magicCnt_ = -1;// �������Ă��閂�@�̐�
    private StatusMagicMng magicMng_;
    private RectTransform statusMagicCheck_;// �X�e�[�^�X��ʂŖ��@��\�����邽�߂̐e
    // ���@�Z�b�g��
    private Button[] equipBtn_ = new Button[4];// �Z�b�g��̃{�^��
    private Image[] equipMagic_ = new Image[4];// �Z�b�g����Ă��閂�@�̉摜
    private Image[] equipExMagic_ = new Image[4];// �Z�b�g����Ă��閂�@�̉摜
    private Color equipBtnSelColor_ = new Color(0.1f, 0.5f, 0.7f, 1.0f);// �Z�b�g��̑I�𒆂̐F
    private Color equipNormalColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);// �����I��łȂ���Ԃ̐F
    private Color equipResetColor_ = new Color(1.0f, 1.0f, 1.0f, 0.0f);// �I���ł��Ȃ���Ԃ̐F
    // �������@�ꗗ
    private int[,] setImageNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];// �ۑ�����閂�@�̉摜
    private Button seleMagicBtn_;
    private Button removeEquipBtn_;// ���@���͂����{�^��

    private int setNullNum_ = 0;// ���@����������ĂȂ��Ƃ��̔ԍ�
    // �ǂ̖��@��ۑ����Ă��邩�ǂݍ���
    private int[,] dataCheck_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];

    // ItemBox�I����
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    // ���݂̃y�[�W��Ԃ��ǂ������擾����(���Ɏg�p����)
    private MenuActive menuActive_;

    // Chara.cs���L�������Ƀ��X�g������
    private List<Chara> charasList_ = new List<Chara>();

    private CharaUseMagic useMagic_;

    public void Init()
    {
        // 1�x�����擾������Q�Ƃ��Ȃ��悤�ɂ���
        if (itemBagChild_ == null)
        {
            itemBagChild_ = transform.GetComponent<RectTransform>();
        }

        if (mngs_[(int)TOPIC.ITEM] == null)
        {
            mngs_[(int)TOPIC.ITEM] = transform.Find("ItemMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.MATERIA] = transform.Find("MateriaMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.WORD] = transform.Find("WordMng").GetComponent<RectTransform>();
            mngs_[(int)TOPIC.MAGIC] = transform.Find("MagicMng").GetComponent<RectTransform>();
        }

        if (topicText_ == null)
        {
            var itemBagMng = GameObject.Find("ItemBagMng").GetComponent<RectTransform>();
            info_ = itemBagMng.Find("InfoBack/InfoText").GetComponent<Text>();
            var infoBack = itemBagMng.Find("InfoBack").GetComponent<RectTransform>();
            throwAwayBtn_[0] = infoBack.Find("ItemDelete").GetComponent<Button>();
            throwAwayBtn_[1] = infoBack.Find("MateriaDelete").GetComponent<Button>();
            throwAwayBtn_[2] = infoBack.Find("MagicDelete").GetComponent<Button>();
            topicText_ = transform.Find("Topics/TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)TOPIC.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }
        info_.text = "";
        ActiveRectTransform();
    }

    public void StatusInit()
    {
        charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = statusMngObj.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = statusMngObj.transform.Find("CharaImage").GetComponent<Image>();
            statusMagicCheck_ = statusMngObj.transform.Find("MagicCheckMask/MagicCheck").GetComponent<RectTransform>();
            removeEquipBtn_ = statusMagicCheck_.transform.Find("Viewport/StatusMagicParent/RemoveMagicBox").GetComponent<Button>();
            info_ = statusMngObj.transform.Find("MagicSetBack/Info/MagicText").GetComponent<Text>();
        }
        MagicInit();

        if (magicMng_ == null)
        {
            magicMng_ = statusMngObj.GetComponent<StatusMagicMng>();
        }
        magicMng_.Init();

        //�T�C�Y���ύX���ĉ摜��؂�ւ���
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        //  RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = statusMngObj.transform.Find("Topics/TopicName").GetComponent<Text>();
        }
        charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];

        info_.text = (magicCnt_ < 2) ? "�����ł��閂�@������܂���" : info_.text = "���@��I�����Ă�������";

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
        useMagic_ = new CharaUseMagic();
    }

    public void MagicInit()
    {

        charasList_ = SceneMng.charasList_;

        if (magicMng_ == null)
        {
            //�f�o�b�O�p
            //  GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
            bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();
            magicMng_ = statusMngObj.GetComponent<StatusMagicMng>();
        }
        magicMng_.Init();
        magicCnt_ = bagMagic_.MagicNumber();

        for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
        {
            var data = SceneMng.GetCharasSettings(c);
            for (int i = 0; i < 4; i++)
            {
                dataCheck_[c, i] = data.Magic[i];

                bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)c, dataCheck_[c, i], true);

                setImageNum_[c, i] = Bag_Magic.data[dataCheck_[c, i]].element;
                //Debug.Log((SceneMng.CHARACTERNUM)c + "��" + i + "�Ԗ�:" + data.Magic[i] + "�Ԃ̖��@/���O�F" +
                //    Bag_Magic.data[data.Magic[i]].name +
                //    "/�G�������g�ԍ�" + Bag_Magic.data[data.Magic[i]].element);

                if (statusMngObj.gameObject.activeSelf == true)
                {
                    // StatusMng���A�N�e�B�u��ԂȂ疂�@�Z�b�g���T��
                    equipBtn_[i] = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + i).GetComponent<Button>();
                    equipMagic_[i] = equipBtn_[i].transform.Find("Icon").GetComponent<Image>();
                    equipExMagic_[i] = equipBtn_[i].transform.Find("ExIcon").GetComponent<Image>();
                    equipBtn_[i].interactable = false;
                }
            }
        }

        if (statusMngObj.gameObject.activeSelf == true)
        {
            Debug.Log("���@�������F" + magicCnt_);
            if (magicCnt_ < 2)
            {
                // 0�ԁi1�ځj�̖��@���G���[�̂���2�ȏ゠�邩���ׂ�
                equipBtn_[0].interactable = false;
                equipBtn_[1].interactable = false;
                equipBtn_[2].interactable = false;
                equipBtn_[3].interactable = false;
                return;
            }

            // ���@�Z�b�g���I�����Ă��Ȃ����ߊO���{�^���͉����Ȃ��悤�ɂ���
            removeEquipBtn_.interactable = false;

            Debug.Log("�I�𒆂̃L�����N�^�[�F" + charaStringNum_);
            MagicBtnCheck();

            // ���@���Z�b�g����Ă�֌W�Ȃ���ԍ��͉����\
            equipBtn_[0].interactable = true;
        }
    }

    public void OnClickRightArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if ((int)TOPIC.WORD < stringNum_++)
            {
                stringNum_ = (int)TOPIC.ITEM;
            }
            topicText_.text = topicString_[stringNum_];
            //  Debug.Log("�E�����N���b�N" + stringNum_);
        }
        else if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if (EventMng.GetChapterNum() < 8)
            {
                // �W���b�N�̃X�e�[�^�X�͕\�����Ȃ�(�܂����ԂɂȂ��ĂȂ�����)
                return;
            }

            if (++charaStringNum_ >= (int)SceneMng.CHARACTERNUM.MAX)
            {
                charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
            }
            ArrowCommon();
            //    Debug.Log("�E�����N���b�N" + charaStringNum_);
        }
        else
        {
            // �����������s��Ȃ�
        }

        if (gameObject.activeSelf == true)
        {
            ActiveRectTransform();
        }
    }

    public void OnClickLeftArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if (--stringNum_ < (int)TOPIC.ITEM)
            {
                stringNum_ = (int)TOPIC.MAGIC;
            }
            topicText_.text = topicString_[stringNum_];
            //  Debug.Log("�������N���b�N" + stringNum_);
        }
        else if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if (EventMng.GetChapterNum() < 8)
            {
                // �W���b�N�̃X�e�[�^�X�͕\�����Ȃ�(�܂����ԂɂȂ��ĂȂ�����)
                return;
            }

            if (--charaStringNum_ < (int)SceneMng.CHARACTERNUM.UNI)
            {
                charaStringNum_ = (int)SceneMng.CHARACTERNUM.JACK;
            }
            ArrowCommon();
            // Debug.Log("�������N���b�N" + charaStringNum_);
        }
        else
        {
            // �����������s��Ȃ�
        }

        if (gameObject.activeSelf == true)
        {
            ActiveRectTransform();
        }
    }

    // ���̋��ʏ�������
    private void ArrowCommon()
    {
        charaNameTopicText_.text = charaTopicString_[charaStringNum_];
        menuActive_.ViewStatus(charaStringNum_);

        //�T�C�Y���ύX���ĉ摜��؂�ւ���
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        if (magicCnt_ < 2)
        {
            // 0�ԁi1�ځj�̖��@���G���[�̂���2�ȏ゠�邩���ׂ�
            equipBtn_[0].interactable = false;
            equipBtn_[1].interactable = false;
            equipBtn_[2].interactable = false;
            equipBtn_[3].interactable = false;
            return;
        }


        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��

        Debug.Log("�I�𒆂̃Z�b�g��" + btnNumber_);

        MagicBtnCheck();


        // ���@���Z�b�g����Ă�֌W�Ȃ���ԍ��͉����\
        equipBtn_[0].interactable = true;

        // ���@�ꗗ�̕\��������
        magicMng_.SetCloseFlag();

        btnNumber_ = -1;// ���@�Z�b�g��̔ԍ������Z�b�g
    }

    private void MagicBtnCheck()
    {
        int addCnt_ = -1;
        for (int i = 0; i < 4; i++)
        {
            equipBtn_[i].image.color = equipNormalColor_;
            // �Z�b�g���Ă��閂�@�����邩�`�F�b�N
            if (dataCheck_[charaStringNum_, i] == setNullNum_)
            {
                equipMagic_[i].sprite = null;
                equipMagic_[i].color = equipResetColor_;
                equipExMagic_[i].color = equipResetColor_;
                // �Z�b�g�悪�I�����ꂽ��ԂŃL�����ւ����Ă�\�������邽�ߒʏ��Ԃɖ߂�
                if (addCnt_ != i)
                {
                    // addCnt_�ƈႤ�l�Ȃ��A�N�e�B�u�ŗǂ�
                    equipBtn_[i].interactable = false;
                }
            }
            else
            {
                // ���@���Z�b�g���Ă����炻�̉摜���X�e�[�^�X��ʂɏo��
                equipMagic_[i].color = equipNormalColor_;
                    equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];

                if (Bag_Magic.data[i].rate == 2)
                {
                    equipExMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                }
                equipBtn_[i].interactable = true;
                Debug.Log(i + "�Ԗڂɖ��@���Z�b�g����Ă��܂�");
                if (i < 3)
                {
                    addCnt_ = i + 1;
                    // �I���ς݂̎��̖��@�ɃZ�b�g�ł���悤��
                    equipBtn_[addCnt_].interactable = true;
                }
            }
          //  Debug.Log((SceneMng.CHARACTERNUM)charaStringNum_ + "��" + i + "�ԖڂɃZ�b�g���̖��@�̉摜�ԍ��F" + setImageNum_[charaStringNum_, i]);
        }
    }

    public void ActiveRectTransform()
    {
        //   Debug.Log(mngs_[stringNum_].gameObject.name + "��\�����܂�               " + stringNum_);
        info_.text = "";// �������̕\�������Z�b�g

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // �I���̂��̂�\��
                mngs_[i].gameObject.SetActive(true);
                // Debug.Log(mngs_[i].gameObject.name + "��\�����Ă��܂�");
            }
            else
            {
                // �I�𒆂̂��̈ȊO�͔�\����
                mngs_[i].gameObject.SetActive(false);
            }
        }

        // ���[�h�Ɏ̂Ă�{�^���͂Ȃ�����for���ŉ񂳂����ŏ���
        if (stringNum_ == (int)TOPIC.WORD)
        {
            // ���@��������o�b�O�̃��[�h���J���Ɛe�̏ꏊ������Ă��܂�����
            GameObject.Find("Managers").GetComponent<Bag_Word>().Init();
        }
        else if (stringNum_ == (int)TOPIC.MATERIA)
        {
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
            throwAwayBtn_[0].gameObject.SetActive(false);
            throwAwayBtn_[2].gameObject.SetActive(false);
        }
        else if (stringNum_ == (int)TOPIC.ITEM)
        {
            GameObject.Find("Managers").GetComponent<Bag_Item>().Init();
            throwAwayBtn_[1].gameObject.SetActive(false);
            throwAwayBtn_[2].gameObject.SetActive(false);
        }
        else if (stringNum_ == (int)TOPIC.MAGIC)
        {
            throwAwayBtn_[0].gameObject.SetActive(false);
            throwAwayBtn_[1].gameObject.SetActive(false);
        }
        else
        {
            // �������Ȃ�
        }
    }

    public void OnClickSetMagicButton()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        // �X�e�[�^�X���J���Ė��@���Z�b�g����{�^������������
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        //// 0�Ԗڂ̈ʒu��
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // �����Ă��閂�@�ꗗ��\��

        // �I�𒆂̐F
        equipBtn_[btnNumber_].image.color = equipBtnSelColor_;
        for (int i = 0; i < 4; i++)
        {
            if (i != btnNumber_)
            {
                // �ق��̃{�^���������ꂽ���ߑI����Ԃ���������
                equipBtn_[i].image.color = equipNormalColor_;
            }
        }

        // ���@�Z�b�g��ɖ��@���Z�b�g����Ă�����͂����{�^����������悤�ɂ���
        removeEquipBtn_.interactable = dataCheck_[charaStringNum_, btnNumber_] != setNullNum_ ? true : false;
        magicMng_.SetCoroutineFlag();
    }

    public void InfoCheck(Button btn, int number)
    {
        if (seleMagicBtn_ == null)
        {
            // �ŏ���������
            seleMagicBtn_ = btn;
            Debug.Log(seleMagicBtn_ + "       " + btn);
        }

        info_.text = useMagic_.MagicInfoMake(Bag_Magic.data[number]);

        // �ȍ~��seleMagicBtn_��1�O�̃{�^����񂪂��邽�߂�������
        if (seleMagicBtn_ != btn)
        {
            Debug.Log(seleMagicBtn_ + "       " + btn);
            // ���ɑI�����Ă�����̂��������ꍇ�F�����ɖ߂�
            seleMagicBtn_.image.color = equipNormalColor_;

            // �V�����I���������@�������đI����Ԃ̐F�ɂ���
            seleMagicBtn_ = btn;
            seleMagicBtn_.image.color = equipBtnSelColor_;

        }
        else
        {
            Debug.Log("�I�𒆂̃{�^��������܂�");

            if (seleMagicBtn_.image.color != equipBtnSelColor_)
            {
                // �ǂ̃{�^����I�����������
                seleMagicBtn_ = btn;
                // �I�������{�^���̐F��ύX
                seleMagicBtn_.image.color = equipBtnSelColor_;
                return;
            }
            // �������́�2��ڂ̃N���b�N�������疂�@���Z�b�g
            SetMagicCheck(number, true);
            // interactable=false���̐F�����������Ȃ邽�ߌ��ɖ߂�
            seleMagicBtn_.image.color = equipNormalColor_;
            // ���g����ɂ���
            seleMagicBtn_ = null;
        }
    }


    public void SetMagicCheck(int num, bool flag)
    {
        Debug.Log("�Z�b�g��̖��@�̔ԍ�" + btnNumber_ + "     �N���b�N�����{�^���̔ԍ��F" + num);

        if (btnNumber_ == -1)
        {
            info_.text = "�������I�����Ă�������";
            // �Z�b�g���I�������ɖ��@���������ꍇ
            return;
        }

        // flag�Ffalse�̎����O���Btrue�̎������@�Z�b�g
        if (flag == false)
        {
            int setMaxNum_ = -1;
            Debug.Log(btnNumber_ + "�Ԗڂ̖��@���O���܂���");

            for (int i = 0; i < 4; i++)
            {
                // �ǂ��܂ŃZ�b�g����Ă��邩���m�F����
                if (dataCheck_[charaStringNum_, i] != setNullNum_)
                {
                    setMaxNum_ = i;
                }
            }
            Debug.Log(setMaxNum_ + "�̖��@���Z�b�g���Ă��܂�");
            // �ő�܂ŃZ�b�g���ꂽ�ꍇ
            equipMagic_[setMaxNum_].sprite = null;
            equipMagic_[setMaxNum_].color = equipResetColor_;
            equipExMagic_[setMaxNum_].color = equipResetColor_;
            if (setMaxNum_ < 3)
            {
                equipBtn_[setMaxNum_ + 1].interactable = false;
            }

            // �O���{�^���������ꂽ��@�w��̖��@���܂������ł����Ԃɂ���
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, dataCheck_[charaStringNum_, btnNumber_], false);

            // 1��̃Z�b�g�ꏊ���A�Z�b�g���ĂȂ���Ԃɂ���
            for (int i = btnNumber_; i < setMaxNum_; i++)
            {
                // �������@�Z�b�g��Ԃŉ��ʂ̖��@���O���ꍇ,�I�𒆂̃Z�b�g�ꏊ+1�̖��@������
                dataCheck_[charaStringNum_, i] = dataCheck_[charaStringNum_, i + 1];
                equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i + 1]];
                if (Bag_Magic.data[i].rate == 2)
                {
                    equipExMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
                }
                else
                {
                    equipExMagic_[i].color = equipResetColor_;
                }
                setImageNum_[charaStringNum_, i] = Bag_Magic.data[dataCheck_[charaStringNum_, i + 1]].element;
                charasList_[charaStringNum_].SetMagicNum(i, dataCheck_[charaStringNum_, i + 1]);
                Debug.Log("�Z�b�g���̖��@�̉摜�ԍ��F" + setImageNum_[charaStringNum_, i]);
            }
            dataCheck_[charaStringNum_, setMaxNum_] = setNullNum_;

            // ���@���O�����ۂɖ��@�����ʂ̃Z�b�g��ɂ��ꂽ�ꍇ�͊O���{�^����������悤�ɂ���
            removeEquipBtn_.interactable = dataCheck_[charaStringNum_, btnNumber_] != setNullNum_ ? true : false;

        }
        else
        {
            info_.text = useMagic_.MagicInfoMake(Bag_Magic.data[num]);
            for (int i = 0; i < 4; i++)
            {
                // ������Z�b�g���������@�����łɃZ�b�g����Ă��邩
                if (dataCheck_[charaStringNum_, i] == num)
                {
                    Debug.Log("�ʂł��łɃZ�b�g����Ă��閂�@�ł�");
                    btnNumber_ = -1;// ���@�Z�b�g��̔ԍ������Z�b�g
                    return;
                }
            }
            bagMagic_.MagicNumber();
            charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);

            // �I������Ă����{�^������������
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, dataCheck_[charaStringNum_, btnNumber_], false);
            // �I�𒆂̃{�^�����X�V����
            dataCheck_[charaStringNum_, btnNumber_] = num;

            // SetMagic3�ȊO�̖��@���Z�b�g��
            if (btnNumber_ != 3)
            {
                if (dataCheck_[charaStringNum_, btnNumber_] != setNullNum_)
                {
                    // 1���̖��@���Z�b�g�ł���悤�ɂ���
                    equipBtn_[btnNumber_ + 1].interactable = true;
                }
            }

            // �摜�ԍ������ւ���
            setImageNum_[charaStringNum_, btnNumber_] = Bag_Magic.data[num].element;
            // Debug.Log((SceneMng.CHARACTERNUM)charaStringNum_ + "�̖��@�̉摜�ԍ��F" + setImageNum_[charaStringNum_, i]);
            equipMagic_[btnNumber_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, btnNumber_]];
            equipMagic_[btnNumber_].color = equipNormalColor_;
            if (Bag_Magic.data[num].rate == 2)
            {
                // �听���̖��@�̏ꍇ�͐��}�[�N��\��
                equipExMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 0.4f);
            }

            // �I�����ꂽ���@�������ł��Ȃ��悤�ɂ���
            bagMagic_.SetStatusMagicCheck((SceneMng.CHARACTERNUM)charaStringNum_, num, true);
        }

        equipBtn_[btnNumber_].image.color = equipNormalColor_;
        btnNumber_ = -1;// ���@�Z�b�g��̔ԍ������Z�b�g
    }

    public void OnClickCloseMagicButton()
    {
        magicMng_.SetCloseFlag();
        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��

        if (btnNumber_ != -1)
        {
            equipBtn_[btnNumber_].image.color = equipNormalColor_;
        }
    }
}