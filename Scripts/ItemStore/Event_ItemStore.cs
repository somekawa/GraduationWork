using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Event_ItemStore : MonoBehaviour
{
    private ItemStoreMng materiaCheck_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�

    public void OnClickSelectItemBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            materiaCheck_ = GameObject.Find("HouseInterior/ItemStore").GetComponent<ItemStoreMng>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        materiaCheck_.SetSelectItemName(clickbtn_.name);
    }
}
