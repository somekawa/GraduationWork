using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    // �f�[�^�n
    private SaveCSV saveCsvSc_;// SceneMng���ɂ���Z�[�u�֘A�X�N���v�g

    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private int btnNumber_ = 0;

    [SerializeField]
    private Sprite[] CharaImage;

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
    "�A�C�e��","������","��[��","�܂ق�"};

    // �X�e�[�^�X�\���ɂłĂ���g�s�b�N��
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // �䎆�T�C�Y�ύX�p
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "���j","�W���b�N"};
    private RectTransform statusMagicCheck_;// �X�e�[�^�X��ʂŖ��@��\�����邽�߂̐e
    private Image[] equipMagic_ = new Image[4];
    private int[,] setMagicNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];
    private int setNullNum_ = 100;// ���@����������ĂȂ��Ƃ��̔ԍ�

    // ItemBox�I����
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    private MenuActive menuActive_;     // ���݂̃y�[�W��Ԃ��ǂ������擾����(���Ɏg�p����)
    // Chara.cs���L�������Ƀ��X�g������
    private List<Chara> charasList_ = new List<Chara>();

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
            topicText_ = transform.Find("Topics/TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)TOPIC.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }
        ActiveRectTransform();
    }

    public void StatusInit()
    {
        if (menuActive_ == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = topicParent_.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
            statusMagicCheck_ = topicParent_.Find("MagicCheck").GetComponent<RectTransform>();

            charasList_ = SceneMng.charasList_;
            var data = SceneMng.GetCharasSettings(charaStringNum_);

            for (int i = 0; i < 4; i++)
            {
                // setMagicNum_[charaStringNum_, i] = data.Magic[i];
                setMagicNum_[charaStringNum_, i] = data.Magic[i].element;
                 //   (int)charasList_[charaStringNum_].GetMagicNum(btnNumber_);

                equipMagic_[i] = topicParent_.Find("MagicSetMng/MagicSet" + i + "/Icon").GetComponent<Image>();
                Debug.Log(setMagicNum_[charaStringNum_, i]);
                if (setMagicNum_[charaStringNum_, i] != setNullNum_)
                {
                    // ���@���Z�b�g���Ă����炻�̉摜���X�e�[�^�X��ʂɏo��
                    equipMagic_[i].sprite =Bag_Magic.magicSpite[i];
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = topicParent_.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = topicParent_.transform.Find("CharaImage").GetComponent<Image>();
        }

        //�T�C�Y���ύX���ĉ摜��؂�ւ���
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
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
            Debug.Log("�E�����N���b�N" + stringNum_);
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
            Debug.Log("�E�����N���b�N" + charaStringNum_);
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
            Debug.Log("�������N���b�N" + stringNum_);
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
            Debug.Log("�������N���b�N" + charaStringNum_);
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

        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��
        for (int i = 0; i < 4; i++)
        {
            if (setMagicNum_[charaStringNum_, i] != setNullNum_)
            {
                equipMagic_[i].sprite = Bag_Magic.magicSpite[i];
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public void ActiveRectTransform()
    {
        Debug.Log(mngs_[stringNum_].gameObject.name + "��\�����܂�               " + stringNum_);

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // �I���̂��̂�\��
                mngs_[i].gameObject.SetActive(true);
                Debug.Log(mngs_[i].gameObject.name + "��\�����Ă��܂�");
            }
            else
            {
                // �I�𒆂̂��̈ȊO�͔�\����
                mngs_[i].gameObject.SetActive(false);
            }
        }

        if (stringNum_ == (int)TOPIC.WORD)
        {
            // ���@��������o�b�O�̃��[�h���J���Ɛe�̏ꏊ������Ă��܂�����
            GameObject.Find("Managers").GetComponent<Bag_Word>().Init();
        }
        if (stringNum_ == (int)TOPIC.MATERIA)
        {
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
        }
    }

    public void OnClickSetMagicButton()
    {
        // �X�e�[�^�X���J���Ė��@���Z�b�g����{�^������������
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // �����Ă��閂�@�ꗗ��\��
    }

    public void OnClickCloseMagicButton()
    {
        //data[num].battleSet0 = flag;
        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��
    }

    public void SetMagicCheck(int num)
    {
        // �L�����̃X�e�[�^�X�l��\����������
        charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);
        setMagicNum_[charaStringNum_, btnNumber_] = num;

        saveCsvSc_.SaveStart();
        // �L�����N�^�[������for������
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            saveCsvSc_.SaveData(SceneMng.GetCharasSettings(i));
        }
        saveCsvSc_.SaveEnd();
    }

    public int GetClickButtonNum()
    {
        return btnNumber_;
    }

    public Sprite GetImageTest(int num)
    {
        return equipMagic_[num].sprite;
    }
}