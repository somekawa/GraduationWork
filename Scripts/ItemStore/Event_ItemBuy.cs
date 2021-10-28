using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Event_ItemBuy : MonoBehaviour
{
    private ItemStoreMng materiaCheck_;    // どのボタンをクリックしたか代入する変数
    //private Trade_Buy buy_;    // どのボタンをクリックしたか代入する変数
    //private Trade_Sell sell_;    // どのボタンをクリックしたか代入する変数
                                           // private CreateMng createMng_;

    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数


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

        //// 自分が選択中の状態で押されたら解除する
        //if (btn_.interactable == false)
        //{
        //    btn_.interactable = true;
        //    return;
        //}

        ////// 初めての時は入らないようにする
        //Debug.Log(clickbtn_.name + "をクリックしました");

        //for (int p = 0; p < materiaCheck_.FieldNumber(); p++)
        //{
        //    for (int i = 0; i < materiaCheck_.GetMateriaList().param.Count; i++)
        //    {
        //      //  クリックしたボタンを選択状態にしている
        //      if(clickbtn_.name == materiaCheck_.GetMateriaList().param[i].MateriaName)
        //        {

        //        }
        //   btn_.interactable = false;
        //        createMng_.SetButtonName(clickbtn_.name);
        //    }
        //}
    }
}
