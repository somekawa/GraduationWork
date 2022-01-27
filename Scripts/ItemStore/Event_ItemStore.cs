using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Event_ItemStore : MonoBehaviour
{
    private ItemStoreMng itemStoreMng_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^����

    public void OnClickSelectItemBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            itemStoreMng_ = GameObject.Find("ItemStoreMng").GetComponent<ItemStoreMng>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // ���������A�C�e���̔ԍ����擾
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        // ��������菜�������O���擾
        string name= clickbtn_.name.Replace( number.ToString(), "");
        itemStoreMng_.SetSelectItemName(number, name);
        Debug.Log("�ԍ��F"+number+"    ���O�F"+name);
    }
}
