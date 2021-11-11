using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreateMng : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas_;

    [SerializeField]
    private RectTransform miniGameParent_;    // ミニゲーム表示用

    [SerializeField]
    private GameObject itemRecipePleate_;    // 素材を拾ったときに生成されるプレハブ


    private GameObject DataPopPrefab_;
    private RecipeList popItemRecipeList_;
    private int maxCnt_=0;
    private int singleCnt_ = 0;

    private Bag_Item bagItem_;// 所持しているアイテムを確認

    private string saveBtnName_ = "";
    private Button saveBtn_;
    private int saveItemNum_ = 0;

    // レシピ本の数
    public enum RECIPE
    {
        NON = -1,
        PAGE0,
        PAGE1,
        PAGE2,
        PAGE3,
        MAX
    }


    public struct itemRecipe
    {
        public GameObject pleate;
        public Button btn;
        public Text nameText;
        public string name;
        public string materia0;
        public string materia1;
        public string materia2;
    }
    public static itemRecipe[] itemRecipeState_;
    private RectTransform recipeParent_;
    private Bag_Materia bagMateria_;// 所持素材を確認


    // ミニゲーム関連
    private MovePoint movePoint_;
    private MovePoint.JUDGE judge_ = MovePoint.JUDGE.NON;
    private Image judeBack_;
    private Text judgText_;

    private Button createBtn_;// 作成開始ボタン


    private Text createName_;// 合成したいアイテムの名前
    private Text wantMateria_;// 合成したいアイテムの必要素材

    void Start()
    {
        recipeParent_ = this.transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popItemRecipeList_ = (RecipeList)DataPopPrefab_.GetComponent<PopList>().GetData<RecipeList>(PopList.ListData.ITEM_RECIPE, 0);
        singleCnt_ = popItemRecipeList_.param.Count;
        maxCnt_ = (int)RECIPE.MAX* singleCnt_;
        itemRecipeState_ = new itemRecipe[maxCnt_];

        int number = 0;
        for (int p = (int)RECIPE.PAGE0; p < (int)RECIPE.MAX; p++)
        {
            // 0番目から3番目のページを見たいためfor文で回す
            popItemRecipeList_ = (RecipeList)DataPopPrefab_.GetComponent<PopList>().GetData<RecipeList>(PopList.ListData.ITEM_RECIPE, p);

            for (int i = 0; i < singleCnt_; i++)
            {
                number = p * singleCnt_ + i;

                itemRecipeState_[number].pleate = Instantiate(itemRecipePleate_,
                new Vector2(0, 0), Quaternion.identity, recipeParent_.transform);
                itemRecipeState_[number].name = popItemRecipeList_.param[i].ItemName;
                itemRecipeState_[number].pleate.name = itemRecipeState_[number].name;
                itemRecipeState_[number].btn = itemRecipeState_[number].pleate.GetComponent<Button>();

                // 表示するアイテムの名前
                itemRecipeState_[number].nameText = itemRecipeState_[number].pleate.transform.Find("Text").GetComponent<Text>();
                itemRecipeState_[number].nameText.text = itemRecipeState_[number].name;
                //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );
                itemRecipeState_[number].materia0 = popItemRecipeList_.param[i].WantMateria1;
                itemRecipeState_[number].materia1 = popItemRecipeList_.param[i].WantMateria2;
                itemRecipeState_[number].materia2 = popItemRecipeList_.param[i].WantMateria3;
            }
        }

                //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
                bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();
        
        // ミニゲーム
        movePoint_ = miniGameParent_.transform.GetComponent<MovePoint>();
        miniGameParent_.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        // miniGameMng_= GameObject.Find("MiniGameMng").GetComponent<RectTransform>();
        judeBack_ = miniGameParent_.transform.Find("JudgBack").GetComponent<Image>();
        judgText_ = judeBack_.transform.Find("Text").GetComponent<Text>();
        judgText_.text = "";
        judeBack_.gameObject.SetActive(false);
        miniGameParent_.gameObject.SetActive(false);


        createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();
        createBtn_.interactable = false;

        createName_ = transform.Find("InfoBack/CreateItemName").GetComponent<Text>();
        wantMateria_ = transform.Find("InfoBack/WantMateriaNames").GetComponent<Text>();
        createName_.text = "";
        wantMateria_.text = "";
        StartCoroutine(AlchemyRecipeSelect());
    }

    public IEnumerator AlchemyRecipeSelect()
    {
        while (true)
        {
                yield return null;

            if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NON)
            {
                // レシピ選択処理
                for (int i = 0; i < maxCnt_; i++)
                {
                    if (saveBtnName_ != itemRecipeState_[i].btn.name)
                    {
                        // 選択してないボタンは押下できる状態にする
                        itemRecipeState_[i].btn.interactable = true;
                    }
                    else
                    {
                        // 選択したレシピの内容を表示
                        SetActiveRecipe(i, saveBtnName_);
                        saveBtn_ = itemRecipeState_[i].btn;
                        saveItemNum_ = i;// 番号を保存
                        createBtn_.interactable = true;
                    }
                }
            }
            else
            {
                if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NORMAL)
                {
                    judgText_.text = "成功";
                }
                else
                {
                    judgText_.text = "大成功";
                }
                judeBack_.gameObject.SetActive(true);
                bagItem_.ItemGetCheck(saveItemNum_, saveBtnName_, 1);
                // 何らかの判定をされた場合
                createBtn_.interactable = true;
                saveBtn_.interactable = true;
                saveBtnName_ = "";
                // 判定をリセット
                judge_ = MovePoint.JUDGE.NON;
                movePoint_.SetMiniGameJudge(judge_);

                yield return new WaitForSeconds(2.0f);
                judeBack_.gameObject.SetActive(false);
                miniGameParent_.gameObject.SetActive(false);
                judgText_.text = "";

            }
        }
    }
    public void SetActiveRecipe(int recipeNum, string name)
    {
        createName_.text = itemRecipeState_[recipeNum].btn.name;
        if (itemRecipeState_[recipeNum].materia2 == "non")
        {
            wantMateria_.text = "必要素材\n" +
                 itemRecipeState_[recipeNum].materia0 +
                " " + itemRecipeState_[recipeNum].materia1;
        }
        else
        {
            wantMateria_.text = "必要素材\n" +
                 itemRecipeState_[recipeNum].materia0 +
                " " + itemRecipeState_[recipeNum].materia1 +
                " " + itemRecipeState_[recipeNum].materia2;
        }
    }

    public void OnClickCreate()
    {
        bool flag = false;
        int recipeNum = 0;
        int[] number = new int[3];
        //int number = 0;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (itemRecipeState_[i].btn.interactable == false)
            {
                recipeNum = i;
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

            number[0] = HaveMateriaCheck(recipeNum, itemRecipeState_[recipeNum].materia0);

            number[2] = HaveMateriaCheck(recipeNum, itemRecipeState_[recipeNum].materia2);

            number[1] = HaveMateriaCheck(recipeNum, itemRecipeState_[recipeNum].materia1);

            Debug.Log("number[0]:" + number[0] + "   number[1]:" + number[1] + "   number[2]:" + number[2]);
            if (itemRecipeState_[number[2]].materia2 == "non")
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

    private int HaveMateriaCheck(int recipeNum, string wantMateria)
    {
        int haveMateriaNum = 0;
        Debug.Log("必要素材"+wantMateria);

        for (int i = 0; i <= bagMateria_.GetMaxHaveMateriaCnt(); i++)
        {
            if (wantMateria == "non")
            {
                // nonが入っているのは3つ目の素材だけ
                Debug.Log("3つ目の素材は必要ありません");
                break;
            }
            else
            {
                if (Bag_Materia.materiaState[i].name == wantMateria)
                {
                    Debug.Log(Bag_Materia.materiaState[i].name+"を持ってます");
                    haveMateriaNum = i;
                    if (Bag_Materia.materiaState[i].haveCnt < 1)
                    {
                        Debug.Log("2つ目の素材の所持数が足りません");
                        return 0;
                    }
                    else
                    {
                        if (wantMateria == itemRecipeState_[recipeNum].materia1)
                        {
                            createBtn_.interactable = false;
                            StartCoroutine(movePoint_.CountDown());
                            miniGameParent_.gameObject.SetActive(true);

                            Debug.Log(Bag_Materia.materiaState[i].name + "を持っていました。ゲームを始めます");
                        }
                    }
                    break;
                }
            }
        }
        return haveMateriaNum;
    }

    public void OnClickCancelCtn()
    {
        StopCoroutine(AlchemyRecipeSelect());
        this.gameObject.SetActive(false);
        uniHouseCanvas_.gameObject.SetActive(true);
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