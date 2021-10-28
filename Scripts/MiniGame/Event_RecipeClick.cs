using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// �C���X�^���X�����{�^��1��1�ɃA�^�b�`����Ă�Script
public class Event_RecipeClick : MonoBehaviour
{
    private RecipeCheck recipeCheck_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private CreateMng createMng_;

    private EventSystem eventSystem;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�


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

        // �������I�𒆂̏�Ԃŉ����ꂽ���������
        if (btn_.interactable == false)
        {
            btn_.interactable = true;
            return;
        }

        //// ���߂Ă̎��͓���Ȃ��悤�ɂ���
        Debug.Log(clickbtn_.name + "���N���b�N���܂���");

        for (int p = (int)RecipeCheck.RECIPE.PAGE0; p < (int)RecipeCheck.RECIPE.MAX; p++)
        {
            for (int i = 0; i < recipeCheck_.GetRecipeList().param.Count; i++)
            {
                // �N���b�N�����{�^����I����Ԃɂ��Ă���
                btn_.interactable = false;
                createMng_.SetButtonName(clickbtn_.name);
            }
        }
    }
}