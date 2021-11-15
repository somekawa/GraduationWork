using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    [SerializeField]
    private Sprite[] CharaImage_;
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

    // ItemBox�I����
    private RectTransform[] mngs_ = new RectTransform[(int)TOPIC.MAX];

    private MenuActive menuActive_;     // ���݂̃y�[�W��Ԃ��ǂ������擾����(���Ɏg�p����)

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
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        RectTransform topicParent_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
        if (charaNameTopicText_ == null)
        {
            charaNameTopicText_ = topicParent_.transform.Find("Topics/TopicName").GetComponent<Text>();
            charaNameTopicText_.text = charaTopicString_[(int)SceneMng.CHARACTERNUM.UNI];
        }

        if (charaImg_ == null)
        {
            charaImgRect_ = topicParent_.transform.Find("CharaImage").GetComponent<RectTransform>();
            charaImg_ = topicParent_.transform.Find("CharaImage").GetComponent<Image>();
        }

        //�T�C�Y���ύX���ĉ摜��؂�ւ���
        charaImgRect_.sizeDelta = new Vector2(CharaImage_[charaStringNum_].rect.width, CharaImage_[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage_[charaStringNum_];

        menuActive_.ViewStatus(charaStringNum_);
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
                stringNum_ = (int)TOPIC.WORD;
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
        charaImgRect_.sizeDelta = new Vector2(CharaImage_[charaStringNum_].rect.width, CharaImage_[charaStringNum_].rect.height);
        charaImg_.sprite = CharaImage_[charaStringNum_];
    }

    public void ActiveRectTransform()
    {
        Debug.Log(mngs_[stringNum_].gameObject.name + "��\�����܂�               "+ stringNum_);

        for (int i = 0; i < (int)TOPIC.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // �I���̂��̂�\��
                mngs_[i].gameObject.SetActive(true);
                Debug.Log(mngs_[i].gameObject.name+"��\�����Ă��܂�");
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
            // ���@��������o�b�O�̃��[�h���J���Ɛe�̏ꏊ������Ă��܂�����
            GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();
        }
    }
}
