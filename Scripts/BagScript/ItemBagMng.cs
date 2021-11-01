using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemBagMng : MonoBehaviour
{
    [SerializeField]
    private Sprite[] CharaImage_;

    private RectTransform itemBagMng_;// itemBag_�̎q

    public enum topic
    {
        NON = -1,
        ITEM,// ���@
        MATERIA,// �f��
        WORD,// ���[�h
        MAX,
    }
    // �A�C�e���I�����ɕ\������g�s�b�N��
    private Text topicText_;
    private int stringNum_ = (int)topic.ITEM;
    private string[] topicString_ = new string[(int)topic.MAX] {
    "�A�C�e��","������","��[��"};

    // �X�e�[�^�X�\���ɂłĂ���g�s�b�N��
    private Text charaNameTopicText_;
    private RectTransform charaImgRect_;    // �䎆�T�C�Y�ύX�p
    private Image charaImg_;
    private int charaStringNum_ = (int)SceneMng.CHARACTERNUM.UNI;
    private string[] charaTopicString_ = new string[(int)SceneMng.CHARACTERNUM.MAX] {
    "���j","�W���b�N"};

    // ItemBox�I����
    private RectTransform[] mngs_ = new RectTransform[(int)topic.MAX];
    private Bag_Materia bagMateria_;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    private MenuActive menuActive_;     // ���݂̃y�[�W��Ԃ��ǂ������擾����(���Ɏg�p����)
    private bool activeFlag_ = false;

    public void Init()
    {
        // 1�x�����擾������Q�Ƃ��Ȃ��悤�ɂ���
        if(itemBagMng_ == null)
        {
            itemBagMng_ = transform.GetComponent<RectTransform>();
        }

        if(mngs_[(int)topic.ITEM] == null)
        {
            mngs_[(int)topic.ITEM] = itemBagMng_.transform.Find("ItemMng").GetComponent<RectTransform>();
        }
        if (mngs_[(int)topic.MATERIA] == null)
        {
            mngs_[(int)topic.MATERIA] = itemBagMng_.transform.Find("MateriaMng").GetComponent<RectTransform>();
        }
        if (mngs_[(int)topic.WORD] == null)
        {
            mngs_[(int)topic.WORD] = itemBagMng_.transform.Find("WordMng").GetComponent<RectTransform>();
        }

        if(bagItem_ == null)
        {
            bagItem_ = GameObject.Find("Managers").GetComponent<Bag_Item>();
        }
        if(bagMateria_ == null)
        {
            bagMateria_ = GameObject.Find("Managers").GetComponent<Bag_Materia>();
        }
        if(bagWord_ == null)
        {
            bagWord_ = GameObject.Find("Managers").GetComponent<Bag_Word>();
        }

        if(topicText_ == null)
        {
            RectTransform topicParent_ = itemBagMng_.transform.Find("Topics").GetComponent<RectTransform>();
            topicText_ = topicParent_.transform.Find("TopicName").GetComponent<Text>();
            topicText_.text = topicString_[(int)topic.ITEM];
        }

        if (menuActive_ == null)
        {
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        }

        StartCoroutine(ActiveRectTransform((int)topic.ITEM, true));
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

        if(charaImg_ == null)
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
            if ((int)topic.WORD < stringNum_++)
            {
                stringNum_ = (int)topic.ITEM;
            }
            topicText_.text = topicString_[stringNum_];
            Debug.Log("�E�����N���b�N" + stringNum_);
        }
        else if(menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.STATUS)
        {
            if(EventMng.GetChapterNum() < 8)
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
    }

    public void OnClickLeftArrow()
    {
        if (menuActive_.GetNowMenuCanvas() == MenuActive.CANVAS.BAG)
        {
            if (--stringNum_ < (int)topic.ITEM)
            {
                stringNum_ = (int)topic.WORD;
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

    public IEnumerator ActiveRectTransform(int startNum, bool flag)
    {
        activeFlag_ = flag;
        while (activeFlag_)
        {
            yield return null;
            for (int i = 0; i < (int)topic.MAX; i++)
            {
                if (stringNum_ == i)
                {
                    // �I���̂��̂�\��
                    mngs_[i].gameObject.SetActive(true);
                }
                else
                {
                    // �I�𒆂̂��̈ȊO�͔�\����
                    mngs_[i].gameObject.SetActive(false);
                }
            }

            if (stringNum_ == (int)ItemBagMng.topic.MATERIA)
            {
                // parentCanvas_[(int)canvas.BAG].transform.Find("MateriaMng").GetComponent<Bag_Materia>().Init(this);
                bagMateria_.ActiveMateria(this);
            }
            else if (stringNum_ == (int)ItemBagMng.topic.WORD)
            {
                //parentObjPrefab_.transform.Find("WordMng").GetComponent<Bag_Word>().Init(this);
                bagWord_.ActiveWord(this);
            }
            else if (stringNum_ == (int)ItemBagMng.topic.ITEM)
            {
                //parentObjPrefab_.transform.Find("ItemMng").GetComponent<Bag_Item>().Init(this);
                bagItem_.ActiveItem(this);
                //bagItem_.ItemGetCheck(EventMng.GetChapterNum());
            }
        }
    }

    public int GetStringNumber()
    {
        return stringNum_;
    }

    public void SetActiveCanvas()
    {
        StopCoroutine(ActiveRectTransform((int)topic.ITEM, false));
    }
}
