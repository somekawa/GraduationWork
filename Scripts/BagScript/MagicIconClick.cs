using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private RectTransform rectItemBagMng_;
    private MenuActive menuActive_;
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private Text info_; // �N���b�N�������@��������闓
    private Image magicIcon_;

    public void OnClickBagMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // �{�^�������琔���݂̂����o��
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        Debug.Log("�{�^���̔ԍ�"+number);
        info_.text = Bag_Magic.data[number].name+"  "+ Bag_Magic.data[number].power;

        //wordCheck_.SetWord(clickbtn_.name);// �{�^���̖��O����
    }

    public void OnClickStatusMagicIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
            rectItemBagMng_ = menuActive_.GetItemBagMng();
        }
        // �ǂ̖��@�{�^���ɉ摜���Z�b�g���邩
        int getNum = rectItemBagMng_.GetComponent<ItemBagMng>().GetClickButtonNum();
        magicIcon_ = GameObject.Find("StatusMng/MagicSet" + getNum+"/Icon").GetComponent<Image>();

        // ����CS�����Ă���{�^���̖��O
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // �{�^�������琔���݂̂����o��
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        magicIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][Bag_Magic.elementNum_[number]];
        magicIcon_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //itemBagMng_.SetMagicCheck(number,true);
        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(number);
    }
}
