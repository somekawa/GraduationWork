using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject recipeUIPrefab;    // 素材を拾ったときに生成されるプレハブ

    public enum RECIPE
    {
        NON = -1,
        PAGE0,
        PAGE1,
        PAGE2,
        PAGE3,
        MAX
    }
    private Bag_Materia bagMateria_;
    private RecipeList recipeList_;
    private MovePoint movePoint_;

    private RectTransform recipeParent_;
    public static GameObject[,] recipeBack_;
    public static Button[,] recipeBtn_;
    public static Text[,] recipeText_;
    private int btnCnt_ = 0;

    private Canvas uniHouseCanvas_;
    private Canvas miniGameCanvas_;
    private Button gouseiBtn_;
    private Button createStartBtn_;// 作成開始ボタン

    void Start()
    {
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();
        movePoint_ = transform.GetComponent<MovePoint>();

        //// 配列に最大値を入れるためにここで一度取得
        //recipeList_ = new RecipeList[(int)RECIPE.MAX];
        //for (int p = (int)RECIPE.PAGE0; p < (int)RECIPE.MAX; p++)
        //{
        //    recipeList_[p] = Resources.Load("RecipeList/Recipe" + p) as RecipeList;
        //}
        recipeList_ = Resources.Load("RecipeList/Recipe" + (int)RECIPE.PAGE0) as RecipeList;
        recipeBack_ = new GameObject[(int)RECIPE.MAX, recipeList_.param.Count];
        recipeText_ = new Text[(int)RECIPE.MAX, recipeList_.param.Count];
        recipeBtn_ = new Button[(int)RECIPE.MAX, recipeList_.param.Count];



        recipeParent_ = GameObject.Find("MiniGameCanvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
       
        uniHouseCanvas_ = GameObject.Find("HouseInterior/UniHouse/UniHouseCanvas").GetComponent<Canvas>();
        miniGameCanvas_ = GameObject.Find("MiniGameCanvas").GetComponent<Canvas>();
        miniGameCanvas_.gameObject.SetActive(false);
        gouseiBtn_ = uniHouseCanvas_.transform.Find("CreateBtn").GetComponent<Button>(); 
        createStartBtn_ = GameObject.Find("MiniGameCanvas/CreateBtn").GetComponent<Button>();
        for (int p = (int)RECIPE.PAGE0; p < (int)RECIPE.MAX; p++)
        {
            // 0番目から3番目のページを見たいためfor文で回す
            recipeList_ = Resources.Load("RecipeList/Recipe" + p) as RecipeList;

            for (int i = 0; i < recipeList_.param.Count; i++)
            {
                Debug.Log(recipeList_.param[i].WantMateria1 + "  " + recipeList_.param[i].WantMateria2);
                recipeBack_[p, i] = Instantiate(recipeUIPrefab,
                     new Vector2(0, 0), Quaternion.identity, recipeParent_.transform);

                recipeBack_[p, i].name = recipeList_.param[i].ItemName;
                recipeBtn_[p, i] = recipeBack_[p, i].GetComponent<Button>();

                recipeText_[p, i] = recipeBack_[p, i].transform.Find("Text").GetComponent<Text>();
                recipeText_[p, i].text = recipeList_.param[i].ItemName;
                //  Debug.Log(i + "番目の本の名前" + recipeList_.param[i].ItemName);
            }
        }
        btnCnt_ = recipeParent_.childCount;
    }

    public void OnClickCreate()
    {
        bool flag = false;
        int number = 0;
        for (int p = (int)RECIPE.PAGE0; p < (int)RECIPE.MAX; p++)
        {
            for (int i = 0; i < recipeList_.param.Count; i++)
            {
                if (recipeBtn_[p, i].interactable == false)
                {
                    recipeList_ = Resources.Load("RecipeList/Recipe" + p) as RecipeList;
                    number = i;
                    Debug.Log("必要な素材1つ目：" + recipeList_.param[number].WantMateria1);
                    Debug.Log("必要な素材2つ目：" + recipeList_.param[number].WantMateria2);
                    flag = true;
                    break;
                }
            }
        }
        if (flag == true)
        {
            Debug.Log("必要な素材を持っているかチェックします      " + Bag_Materia.materiaNum_);
            for (int i = 0; i <= Bag_Materia.materiaNum_; i++)
            {
                // 所持している素材と必要素材があっているかチェック
                if (Bag_Materia.saveMateriaName_[i] == recipeList_.param[number].WantMateria1)
                {
                    Debug.Log(Bag_Materia.saveMateriaName_[i] + "を持っていました。2つ目を探します");
                    break;
                }
            }
            for (int t = 0; t <= Bag_Materia.materiaNum_; t++)
            {
                if (Bag_Materia.saveMateriaName_[t] == recipeList_.param[number].WantMateria2)
                {
                    Debug.Log(Bag_Materia.saveMateriaName_[t] + "を持っていました。ゲームを始めます");
                    createStartBtn_.interactable = false;
                    StartCoroutine(movePoint_.CountDown());
                    return;
                }
            }
        }
    }

    public void OnClickGousei()
    {
        uniHouseCanvas_.gameObject.SetActive(false);
        miniGameCanvas_.gameObject.SetActive(true);
    }

    public int GetButtonCount()
    {
        return btnCnt_;
    }

    public RecipeList GetRecipeList()
    {
        return recipeList_;
    }
}