using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// インスタンスされるボタン1つ1つにアタッチされてるScript
public class Event_RecipeClick : MonoBehaviour
{
    private RecipeCheck recipeCheck_;    // どのボタンをクリックしたか代入する変数
    private CreateMng createMng_;

    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数


    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        recipeCheck_ = GameObject.Find("Mng").GetComponent<RecipeCheck>();
        createMng_ = GameObject.Find("Mng").GetComponent<CreateMng>();
    }

    public void OnClickRecipeBtn()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        Button btn_ = clickbtn_.GetComponent<Button>();

        // 自分が選択中の状態で押されたら解除する
        if (btn_.interactable == false)
        {
            btn_.interactable = true;
            return;
        }

        //// 初めての時は入らないようにする
        Debug.Log(clickbtn_.name + "をクリックしました");

        for (int p = (int)RecipeCheck.RECIPE.PAGE0; p < (int)RecipeCheck.RECIPE.MAX; p++)
        {
            for (int i = 0; i < recipeCheck_.GetRecipeList().param.Count; i++)
            {
                // クリックしたボタンを選択状態にしている
                btn_.interactable = false;
                createMng_.SetButtonName(clickbtn_.name);
            }
        }
    }
}