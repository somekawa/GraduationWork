using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// �C���X�^���X�����{�^��1��1�ɃA�^�b�`����Ă�Script
public class Event_RecipeClick : MonoBehaviour
{
    private ItemCreateMng itemCreateMng_;

    private EventSystem eventSystem;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�


    public void OnClickItemRecipeBtn()
    {
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            itemCreateMng_ = GameObject.Find("AlchemyMng").GetComponent<ItemCreateMng>();
        }
        clickbtn_ = eventSystem.currentSelectedGameObject;
        Button btn_ = clickbtn_.GetComponent<Button>();

        // �N���b�N�����{�^����I����Ԃɂ��Ă���
        btn_.interactable = false;
        itemCreateMng_.SetButtonName(clickbtn_.name);
    }
}