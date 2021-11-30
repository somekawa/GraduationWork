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
    "�A�C�e��","������","��[��","�܂ق�"};
    private Text info_; // �N���b�N�����A�C�e����������闓
    private Button[] throwAwayBtn_=new Button[(int)TOPIC.MAGIC];

    // �X�e�[�^�X�\���ɂłĂ���g�s�b�N��
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // �䎆�T�C�Y�ύX�p
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "���j","�W���b�N"};


    private Bag_Magic bagMagic_;
    private RectTransform statusMagicCheck_;// �X�e�[�^�X��ʂŖ��@��\�����邽�߂̐e
    private Button[] equipBtn_ = new Button[4];// �ۑ���̖��@�̃{�^��
    private Image[] equipMagic_ = new Image[4];// �ۑ�����閂�@�̉摜
    private int[,] setImageNum_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];
    private Button seleMagicBtn_;
    private Image infoBack_;
    private int saveClickNum_ = -1;
    //private Text magicName_;

    private int setNullNum_ = 0;// ���@����������ĂȂ��Ƃ��̔ԍ�
    // �ǂ̖��@��ۑ����Ă��邩�ǂݍ���
    private int[,] dataCheck_ = new int[(int)SceneMng.CHARACTERNUM.MAX, 4];

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
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
            throwAwayBtn_[0] = GameObject.Find("ItemBagMng/InfoBack/ItemDelete").GetComponent<Button>();
            throwAwayBtn_[1] = GameObject.Find("ItemBagMng/InfoBack/MateriaDelete").GetComponent<Button>();
            throwAwayBtn_[2] = GameObject.Find("ItemBagMng/InfoBack/MagicDelete").GetComponent<Button>();
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
        if (menuActive_ == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

      //  RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = statusMngObj.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
            statusMagicCheck_ = statusMngObj.Find("MagicCheck").GetComponent<RectTransform>();
            infoBack_ = statusMagicCheck_.Find("Info").GetComponent<Image>();
            info_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = statusMngObj.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = statusMngObj.transform.Find("CharaImage").GetComponent<Image>();
            MagicInit();
        }
        //�T�C�Y���ύX���ĉ摜��؂�ւ���
        charaImgRect_.sizeDelta = new Vector2(CharaImage[charaStringNum_].rect.width, CharaImage[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
        statusMagicCheck_.gameObject.SetActive(false);
    }

    public void MagicInit()
    {
        charasList_ = SceneMng.charasList_;
        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();

        for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
        {
            var data = SceneMng.GetCharasSettings(c);
            for (int i = 0; i < 4; i++)
            {
                dataCheck_[c, i] = data.Magic[i];

                bagMagic_.SetStatusMagicCheck(dataCheck_[c, i], true);

                setImageNum_[c, i] = Bag_Magic.data[data.Magic[i]].element;
                Debug.Log((SceneMng.CHARACTERNUM)c + "��" + i + "�Ԗ�:" + data.Magic[i] + "�Ԃ̖��@/���O�F" +
                    Bag_Magic.data[data.Magic[i]].name +
                    "/�G�������g�ԍ�" + Bag_Magic.data[data.Magic[i]].element);

                if (statusMngObj.gameObject.activeSelf == true)
                {
                    // StatusMng���A�N�e�B�u��ԂȂ疂�@�Z�b�g���T��
                    equipBtn_[i] = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + i).GetComponent<Button>();
                    equipMagic_[i] = equipBtn_[i].transform.Find("Icon").GetComponent<Image>();
                    equipBtn_[i].interactable = false;
                }
            }
        }

        if (statusMngObj.gameObject.activeSelf == true)
        {
            for (int i = 0; i < 4; i++)
            {
                // �Z�b�g���Ă��閂�@�����邩�`�F�b�N
                if (dataCheck_[charaStringNum_, i] == setNullNum_)
                {
                    equipMagic_[i].sprite = null;
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }
                else
                {
                    // ���@���Z�b�g���Ă����炻�̉摜���X�e�[�^�X��ʂɏo��
                    equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];
                    equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    equipBtn_[i].interactable = true;
                    Debug.Log(i+"�Ԗڂɂɖ��@���Z�b�g����Ă��܂�");
                    if (i < 3)
                    {
                        // �I���ς݂̎��̖��@�ɃZ�b�g�ł���悤��
                        equipBtn_[i + 1].interactable = true;
                    }
                }
            }
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

        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��
       
        for (int i = 0; i < 4; i++)
        {
            if (dataCheck_[charaStringNum_, i] != setNullNum_)
            {
                equipMagic_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, i]];
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                equipBtn_[i].interactable = false;
            }
            else
            {
                equipMagic_[i].sprite = null;
                equipMagic_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                equipBtn_[i].interactable = true;

                if (i < 3)
                {
                    equipBtn_[i + 1].interactable = true;
                }
            }
        }
    }

    public void ActiveRectTransform()
    {
       // Debug.Log(mngs_[stringNum_].gameObject.name + "��\�����܂�               " + stringNum_);

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
        if(eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        // �X�e�[�^�X���J���Ė��@���Z�b�g����{�^������������
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        //// 0�Ԗڂ̈ʒu��
        btnNumber_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        statusMagicCheck_.gameObject.SetActive(true);     // �����Ă��閂�@�ꗗ��\��
    }

    public void OnClickCloseMagicButton()
    {
        //data[num].battleSet0 = flag;
        statusMagicCheck_.gameObject.SetActive(false);     // �����Ă��閂�@�ꗗ��\��
    }

    public void InfoCheck(Button btn,Vector2 pos,int number)
    {
        if (seleMagicBtn_==null)
        {
            // �ŏ���������
            seleMagicBtn_ = btn;
            infoBack_.gameObject.SetActive(true);
        }

        // �ȍ~��seleMagicBtn_��1�O�̃{�^����񂪂��邽�߂�������
        if (seleMagicBtn_ != btn)
        {
            // ���ɑI�����Ă�����̂��������ꍇ�F�����ɖ߂�
            seleMagicBtn_.image.color = Color.white;
            // �������̍��W�����炷
            infoBack_.transform.localPosition = pos;

            // �T�u�̕��������邩�ŕ\�����Ⴄ
            info_.text = Bag_Magic.data[number].sub == "non" ? info_.text = Bag_Magic.data[number].main : Bag_Magic.data[number].main + "\n" + Bag_Magic.data[number].sub;
        }
        else
        {
            if (seleMagicBtn_.image.color != Color.green)
            {
                // �ǂ̃{�^����I�����������
                seleMagicBtn_ = btn;
                // �I�������{�^���̐F��ύX
                seleMagicBtn_.image.color = Color.green;
                // �T�u�̕��������邩�ŕ\�����Ⴄ
                info_.text = Bag_Magic.data[number].sub == "non" ? info_.text = Bag_Magic.data[number].main : Bag_Magic.data[number].main + "\n" + Bag_Magic.data[number].sub;
                return;
            }
            // �������́�2��ڂ̃N���b�N�������疂�@���Z�b�g
            SetMagicCheck(number, true);
            // ���������Z�b�g���Ĕ�\����
            info_.text = "";
            infoBack_.gameObject.SetActive(false);
            // interactable=false���̐F�������������Ȃ邽�ߌ��ɖ߂�
            seleMagicBtn_.image.color = Color.white;
            // ���@�ꗗ�����
            statusMagicCheck_.gameObject.SetActive(false);
            // ���g����ɂ���
            seleMagicBtn_ = null;
        }
    }

    public void SetMagicCheck(int num,bool flag)
    {
        // ���@���Z�b�g������Z�[�u����
        Debug.Log("�Z�b�g��̖��@�̔ԍ�"+btnNumber_+"     �N���b�N�����{�^���̔ԍ��F" + num);
        infoBack_.gameObject.SetActive(false) ;

        if (flag == false)
        {
            // �O���{�^���������ꂽ��
            equipMagic_[btnNumber_].sprite = null;
            equipMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            // ���@���O�������ߎ��̃Z�b�g���I���ł��Ȃ��悤�ɂ���
            equipBtn_[btnNumber_ + 1].interactable = false;
            bagMagic_.SetStatusMagicCheck(dataCheck_[charaStringNum_, btnNumber_], false);

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                // ������Z�b�g���������@�����łɃZ�b�g����Ă��邩
                if (dataCheck_[charaStringNum_, i] == num)
                {
                    Debug.Log("�ʂł��łɃZ�b�g����Ă��閂�@�ł�");
                    return;
                }
            }

            charasList_[charaStringNum_].SetMagicNum(btnNumber_, num);

            // �I������Ă����{�^������������
            bagMagic_.SetStatusMagicCheck(dataCheck_[charaStringNum_, btnNumber_], false);
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
            equipMagic_[btnNumber_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][setImageNum_[charaStringNum_, btnNumber_]];
            equipMagic_[btnNumber_].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            // �I�����ꂽ���@�������ł��Ȃ��悤�ɂ���
            bagMagic_.SetStatusMagicCheck(num,true);
        }

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