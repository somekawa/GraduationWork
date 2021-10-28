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

    private Button createBtn_;// 作成開始ボタン

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
            // レシピ選択処理
            for (int p = (int)RecipeCheck.RECIPE.PAGE0; p < (int)RecipeCheck.RECIPE.MAX; p++)
            {
                for (int i = 0; i < recipeCheck_.GetRecipeList().param.Count; i++)
                {
                    // 選択中以外のものが押されたら
                    // 新しく選択されたものをfalse(ボタンクリック時に処理済み)
                    // それ以外をtrue
                    if (saveBtnName_ != RecipeCheck.recipeBtn_[p, i].name)
                    {
                        RecipeCheck.recipeBtn_[p, i].interactable = true;
                        saveBtn_ = RecipeCheck.recipeBtn_[p, i];
                        // Debug.Log(RecipeCheck.recipeBtn_[p, i].name + "がfalseになっています");
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
            // 何らかの判定をされた場合
            createBtn_.interactable = true;
            saveBtn_.interactable = true;
            saveBtnName_ = "";
            // 判定をリセット
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