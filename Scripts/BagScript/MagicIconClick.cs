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
    private Bag_Magic bagMagic_;

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
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        info_.text = Bag_Magic.data[number].name + "  " + Bag_Magic.data[number].power;
        bagMagic_.SetMagicNumber(number);
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
        // ����CS�����Ă���{�^���̖��O
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        Button btn = clickbtn_.GetComponent<Button>();
        // �{�^�������琔���݂̂����o��
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        var pos = transform.localPosition;
        rectItemBagMng_.GetComponent<ItemBagMng>().InfoCheck(btn, pos, number);
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
        rectItemBagMng_.GetComponent<ItemBagMng>().SetMagicCheck(-1, false);
    }
}
