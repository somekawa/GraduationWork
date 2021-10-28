using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMng : MonoBehaviour
{
    private Bag_Item bagItem_;

    private CreateContents createContents_;
    private RecipeCheck recipeCheck_;
    private string saveBtnName_ = "";
    private Button saveBtn_;

    private MovePoint.JUDGE judge_ = MovePoint.JUDGE.NON;

    private Button createBtn_;// �쐬�J�n�{�^��

    void Start()
    {
        GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
        bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

        recipeCheck_ = transform.GetComponent<RecipeCheck>();
        createContents_ = transform.GetComponent<CreateContents>();
        createBtn_ = GameObject.Find("MiniGameCanvas/CreateBtn").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (judge_ == MovePoint.JUDGE.NON)

        {
            // ���V�s�I������
            for (int p = (int)RecipeCheck.RECIPE.PAGE0; p < (int)RecipeCheck.RECIPE.MAX; p++)
            {
                for (int i = 0; i < recipeCheck_.GetRecipeList().param.Count; i++)
                {
                    // �I�𒆈ȊO�̂��̂������ꂽ��
                    // �V�����I�����ꂽ���̂�false(�{�^���N���b�N���ɏ����ς�)
                    // ����ȊO��true
                    if (saveBtnName_ != RecipeCheck.recipeBtn_[p, i].name)
                    {
                        RecipeCheck.recipeBtn_[p, i].interactable = true;
                        saveBtn_ = RecipeCheck.recipeBtn_[p, i];
                        // Debug.Log(RecipeCheck.recipeBtn_[p, i].name + "��false�ɂȂ��Ă��܂�");
                    }
                    else
                    {
                        createContents_.SetActiveRecipe(p, i, saveBtnName_);
                    }
                }
            }
        }
        else
        {
            bagItem_.ItemGetCheck(saveBtnName_);
            // ���炩�̔�������ꂽ�ꍇ
            createBtn_.interactable = true;
            saveBtn_.interactable = true;
            saveBtnName_ = "";
            // ��������Z�b�g
            judge_ = MovePoint.JUDGE.NON;
        }
    }

    public void SetButtonName(string name)
    {
        saveBtnName_ = name;
    }

    public void GetJudgeCheck(MovePoint.JUDGE num)
    {
        judge_ = num;
    }
}