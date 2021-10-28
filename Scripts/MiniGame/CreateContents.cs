using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateContents : MonoBehaviour
{
    //private RecipeCheck recipeCheck_;
    private RecipeList recipeList_;

    private Text createName_;
    private Text wantMateria_;

    // Start is called before the first frame update
    void Start()
    {

        createName_ = GameObject.Find("MiniGameCanvas/ExplanationImage/CreateItemName").GetComponent<Text>();
        wantMateria_ = GameObject.Find("MiniGameCanvas/ExplanationImage/WantMateriaNames").GetComponent<Text>();
        createName_.text = "";
        wantMateria_.text = "";
    }

    // Update is called once per frame
    public void SetActiveRecipe(int page, int recipeNum, string name)
    {
        // 表示したいアイテムがあるシート番号（ページ）
        recipeList_ = Resources.Load("RecipeList/Recipe" + page) as RecipeList;

        createName_.text = recipeList_.param[recipeNum].ItemName;
        wantMateria_.text = "必要素材\n" +
            recipeList_.param[recipeNum].WantMateria1 +
            " " + recipeList_.param[recipeNum].WantMateria2 +
            " " + recipeList_.param[recipeNum].WantMateria3;
    }
}