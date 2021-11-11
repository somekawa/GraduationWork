using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCheck : MonoBehaviour
{

    [SerializeField]
    private GameObject recipeUIPrefab;    // 素材を拾ったときに生成されるプレハブ

    //// 合成時に必要な素材
    //private enum WANT
    //{
    //    NON = -1,
    //    MATERIA_0,
    //    MATERIA_1,
    //    MATERIA_2,
    //    MAX
    //}
    //private static Dictionary<WANT, string[]> wantMap_ = new Dictionary<WANT, string[]>();

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
    public static GameObject[] recipeBack_;
    public static Button[] recipeBtn_;
    public static Text[] recipeText_;
    public static string[] wantMateria0_;
    public static string[] wantMateria1_;
    public static string[] wantMateria2_;
    private int maxRecipeCnt_ = 0;
    private int singleRecipeCnt_ = 0;         // 1つのシートに記載されてる最大個数
    private int btnCnt_ = 0;

    private Canvas miniGameCanvas_;
    //  private Button gouseiBtn_;
    private Button createStartBtn_;// 作成開始ボタン

    private int materiaHaveCnt_ = 0;

    void Start()
    {
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();
        movePoint_ = transform.GetComponent<MovePoint>();

        recipeList_ = Resources.Load("RecipeList/Recipe" + (int)RECIPE.PAGE0) as RecipeList;
        singleRecipeCnt_ = recipeList_.param.Count;
        maxRecipeCnt_ = (int)RECIPE.MAX * singleRecipeCnt_;
        recipeBack_ = new GameObject[maxRecipeCnt_];
        recipeText_ = new Text[maxRecipeCnt_];
        recipeBtn_ = new Button[maxRecipeCnt_];
        wantMateria0_ = new string[maxRecipeCnt_];
        wantMateria1_ = new string[maxRecipeCnt_];
        wantMateria2_ = new string[maxRecipeCnt_];


        recipeParent_ = this.transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();

        //miniGameCanvas_ = GameObject.Find("MiniGameCanvas").GetComponent<Canvas>();
        //  miniGameCanvas_.gameObject.SetActive(false);
        createStartBtn_ = this.transform.Find("CreateBtn").GetComponent<Button>();
        createStartBtn_.interactable = false;
        if (recipeBack_[0] == null)
        {
            int number = 0;
            for (int p = (int)RECIPE.PAGE0; p < (int)RECIPE.MAX; p++)
            {
                // 0番目から3番目のページを見たいためfor文で回す
                recipeList_ = Resources.Load("RecipeList/Recipe" + p) as RecipeList;

                for (int i = 0; i < singleRecipeCnt_; i++)
                {
                    Debug.Log(recipeList_.param[i].WantMateria1 + "  " + recipeList_.param[i].WantMateria2);

                    number = p * singleRecipeCnt_ + i;
                    recipeBack_[number] = Instantiate(recipeUIPrefab,
                         new Vector2(0, 0), Quaternion.identity, recipeParent_.transform);

                    recipeBack_[number].name = recipeList_.param[i].ItemName;
                    recipeBtn_[number] = recipeBack_[number].GetComponent<Button>();

                    // 表示するアイテムの名前
                    recipeText_[number] = recipeBack_[number].transform.Find("Text").GetComponent<Text>();
                    recipeText_[number].text = recipeList_.param[i].ItemName;
                    //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );
                    wantMateria0_[number] = recipeList_.param[i].WantMateria1;
                    wantMateria1_[number] = recipeList_.param[i].WantMateria2;
                    wantMateria2_[number] = recipeList_.param[i].WantMateria3;
                }
            }
        }
        btnCnt_ = recipeParent_.childCount;
    }

    public void OnClickCreate()
    {
        bool flag = false;
        int[] number = new int[3];
        //int number = 0;
        for (int i = 0; i < maxRecipeCnt_; i++)
        {
            if (recipeBtn_[i].interactable == false)
            {
                //// number = i;
                // Debug.Log("必要な素材1つ目：" + recipeList_.param[number].WantMateria1);
                // Debug.Log("必要な素材2つ目：" + recipeList_.param[number].WantMateria2);
                flag = true;
                break;
            }
        }

        if (flag == true)
        {
            Debug.Log("必要な素材を持っているかチェックします      " + bagMateria_.GetMaxHaveMateriaCnt());
            for (int i = 0; i <= bagMateria_.GetMaxHaveMateriaCnt(); i++)
            {
                // 所持している素材と必要素材があっているかチェック                
                if (Bag_Materia.materiaState[i].name == wantMateria0_[i])
                {
                    number[0] = i;
                    if (Bag_Materia.materiaState[number[i]].haveCnt < 1)
                    {
                        Debug.Log("1つ目の素材の所持数が足りません");
                    }
                    else
                    {
                        Debug.Log(Bag_Materia.materiaState[i].name + "を持っていました。2つ目を探します");
                    }
                    break;
                }
            }

            for (int f = 0; f <= bagMateria_.GetMaxHaveMateriaCnt(); f++)
            {
                if (Bag_Materia.materiaState[f].name == wantMateria2_[f])
                {
                    number[2] = f;
                    if (Bag_Materia.materiaState[number[f]].haveCnt < 1)
                    {
                        Debug.Log("3つ目の素材の所持数が足りません");
                    }
                    else
                    {
                        Debug.Log(Bag_Materia.materiaState[f].name + "を持っていました");
                    }
                    break;
                }
                else
                {
                    if (wantMateria2_[f] == "non")
                    {
                        number[2] = 0;
                        Debug.Log("3つ目の素材は必要ありません");
                        break;
                    }
                }
            }

            for (int t = 0; t <= bagMateria_.GetMaxHaveMateriaCnt(); t++)
            {
                if (Bag_Materia.materiaState[t].name == wantMateria1_[t])
                {
                    number[1] = t;
                    if (Bag_Materia.materiaState[number[t]].haveCnt < 1)
                    {
                        Debug.Log("2つ目の素材の所持数が足りません");
                    }
                    else
                    {
                        Debug.Log(Bag_Materia.materiaState[t].name + "を持っていました。ゲームを始めます");
                        createStartBtn_.interactable = false;
                        StartCoroutine(movePoint_.CountDown());
                    }
                    break;
                }
            }

            if (createStartBtn_.interactable == false)
            {
                if (wantMateria2_[number[2]] == "non")
                {
                    Bag_Materia.materiaState[number[0]].haveCnt--;
                    Bag_Materia.materiaState[number[1]].haveCnt--;
                }
                else
                {
                    Bag_Materia.materiaState[number[0]].haveCnt--;
                    Bag_Materia.materiaState[number[1]].haveCnt--;
                    Bag_Materia.materiaState[number[2]].haveCnt--;
                }
            }
        }
    }


    public int GetButtonCount()
    {
        return btnCnt_;
    }

    public RecipeList GetRecipeList()
    {
        return recipeList_;
    }

    public int SetMaxRecipeCnt()
    {
        return maxRecipeCnt_;
    }
}