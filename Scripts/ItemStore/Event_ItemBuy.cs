using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Event_ItemBuy : MonoBehaviour
{
    private ItemStoreMng materiaCheck_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    //private Trade_Buy buy_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    //private Trade_Sell sell_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
                                           // private CreateMng createMng_;

    private EventSystem eventSystem;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�


    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        materiaCheck_ = GameObject.Find("HouseInterior/ItemStore/ItemStoreMng").GetComponent<ItemStoreMng>();

        //if (materiaCheck_.GetSelectActive() == ItemStoreMng.STORE.BUY)
        //{
        //    buy_ = GameObject.Find("HouseInterior/ItemStore/ItemStoreMng").GetComponent<Trade_Buy>();
        //}
        //else if (materiaCheck_.GetSelectActive() == ItemStoreMng.STORE.SELL)
        //{
        //    sell_ = GameObject.Find("HouseInterior/ItemStore/ItemStoreMng").GetComponent<Trade_Sell>();
        //}
        //recipeCheck_ = GameObject.Find("Mng").GetComponent<RecipeCheck>();
        //createMng_ = GameObject.Find("Mng").GetComponent<CreateMng>();
    }

    public void OnClickSelectItemBtn()
    {
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        }
        clickbtn_ = eventSystem.currentSelectedGameObject;
       // Button btn_ = clickbtn_.GetComponent<Button>();
        materiaCheck_.SetSelectItemName(clickbtn_.name);

        //// �������I�𒆂̏�Ԃŉ����ꂽ���������
        //if (btn_.interactable == false)
        //{
        //    btn_.interactable = true;
        //    return;
        //}

        ////// ���߂Ă̎��͓���Ȃ��悤�ɂ���
        //Debug.Log(clickbtn_.name + "���N���b�N���܂���");

        //for (int p = 0; p < materiaCheck_.FieldNumber(); p++)
        //{
        //    for (int i = 0; i < materiaCheck_.GetMateriaList().param.Count; i++)
        //    {
        //      //  �N���b�N�����{�^����I����Ԃɂ��Ă���
        //      if(clickbtn_.name == materiaCheck_.GetMateriaList().param[i].MateriaName)
        //        {

        //        }
        //   btn_.interactable = false;
        //        createMng_.SetButtonName(clickbtn_.name);
        //    }
        //}
    }
}
