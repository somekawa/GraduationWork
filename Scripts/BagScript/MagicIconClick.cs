using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private RectTransform rectItemBagMng_;
    private MenuActive menuActive_;
    private Text info_; // �N���b�N�������@��������闓
    private Image magicIcon_;
    private Bag_Magic bagMagic_;

    private RectTransform magicSetMng_;
    private int number_ = -1;
    private Image infoBack_;
    private Text magicName_;

    private int getSetBtnNumber_=0;
    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
            bagMagic_ = GameObject.Find("Managers").GetComponent<Bag_Magic>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // �{�^�������琔���݂̂����o��
        number_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        info_.text = Bag_Magic.data[number_].name + "  " + Bag_Magic.data[number_].power;
        bagMagic_.SetMagicNumber(number_);
        //wordCheck_.SetWord(clickbtn_.name);// �{�^���̖��O����
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
            magicSetMng_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
            infoBack_ = GameObject.Find("StatusMng/MagicCheck/Info").GetComponent<Image>();
            magicName_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
        }
        // ����CS�����Ă���{�^���̖��O
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // �{�^�������琔���݂̂����o��
        number_ = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        if (infoBack_.gameObject.activeSelf == true)
        {
            //// ���@���Z�b�g����Ƃ��@�ǂ̖��@�{�^���ɉ摜���Z�b�g���邩
            // getSetBtnNumber_ = rectItemBagMng_.GetComponent<ItemBagMng>().GetClickButtonNum();
            //magicIcon_ = GameObject.Find("StatusMng/MagicSetMng/MagicSet" + getSetBtnNumber_ + "/Icon").GetComponent<Image>();

            // �ǂ̖��@��I�񂾂�
            rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(number_,true);
            // ���@�̏ڍׂ�����
            infoBack_.gameObject.SetActive(false);
            magicName_.text = "";
        }
        else
        {
            var pos = transform.localPosition;
            infoBack_.transform.localPosition = pos;
            Debug.Log("�������̍��W" + pos.x + "  " + pos.y + "   " + pos.z);
            // �ڐG�������@�̏ڍׂ��o��
            infoBack_.gameObject.SetActive(true);


            if (Bag_Magic.data[number_].sub != "non")
            {
                magicName_.text = Bag_Magic.data[number_].main + "\n" + Bag_Magic.data[number_].sub;
            }
            else
            {
                magicName_.text = Bag_Magic.data[number_].main;
            }
        }
    }

    public void OnClickRemoveMagic()
    {
        // �O���{�^���������ꂽ�ꍇ
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
        }

        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(-1,false);
    }


    //public void OnMouseOver()
    //{
    //    if (magicSetMng_ == null)
    //    {
    //        magicSetMng_ = GameObject.Find("StatusMng").GetComponent<RectTransform>();
    //    }

    //    if (EventSystem.current.IsPointerOverGameObject()) return;
    //    Debug.Log(this.transform.gameObject.name + "�Ƀ}�E�X�I�[�o�[���܂���");
    //    if (magicSetMng_.gameObject.activeSelf == true)
    //    {

    //        infoBack_ = GameObject.Find("StatusMng/MagicCheck/Info").GetComponent<Image>();
    //        magicName_ = infoBack_.transform.Find("MagicText").GetComponent<Text>();
    //        var pos = transform.localPosition;
    //        infoBack_.transform.localPosition = pos;
    //        Debug.Log("�������̍��W"+pos.x+"  "+pos.y+"   "+pos.z);
    //        // �ڐG�������@�̏ڍׂ��o��
    //        infoBack_.gameObject.SetActive(true);


    //        if (Bag_Magic.data[number_].sub != "non")
    //        {
    //            magicName_.text = Bag_Magic.data[number_].main + "\n" + Bag_Magic.data[number_].sub;
    //        }
    //        else
    //        {
    //            magicName_.text = Bag_Magic.data[number_].main;
    //        }
    //    }
    //}

    //public void OnMouseExit()
    //{
    //    if (magicSetMng_.gameObject.activeSelf == true)
    //    {
    //        // ���@�̏ڍׂ�����
    //        infoBack_.gameObject.SetActive(false);
    //        magicName_.text = "";
    //    }
    //}

}
