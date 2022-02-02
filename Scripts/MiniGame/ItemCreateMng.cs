using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreateMng : MonoBehaviour
{
    [SerializeField]
    private Canvas uniHouseCanvas;

    [SerializeField]
    private RectTransform miniGameMng;    // ミニゲーム表示用

    [SerializeField]
    private GameObject itemRecipePleate;    // 素材を拾ったときに生成されるプレハブ

    private InitPopList popItemRecipeList_;
    private int maxCnt_ = 0;
    private int startCnt_ = 0;
    private int maxMateriaCnt_ = 0;

    private Bag_Item bagItem_;// 所持しているアイテムを確認

    private string saveBtnName_ = "";   // どのアイテムを押したかを保存
    private Button saveBtn_;            // どのボタンを押したかを保存
    private int saveItemNum_ = 0;
    // private string saveName_ = "";

    public struct itemRecipe
    {
        public GameObject pleate;   // アイテムを表示するオブジェクト
        public Button btn;          // アイテムクリック
        public TMPro.TextMeshProUGUI nameText;       // アイテムの名前を表示するText
        public string name;         // アイテムの名前
        public string materia1;     // 合成に必要な素材1
        public string materia2;     // 合成に必要な素材2
        public string materia3;     // 合成に必要な素材3
    }
    public static itemRecipe[] itemRecipeState;
    private int[] haveCnt;// 所持数
    private int nonMateriaNum_ = -1;
    private int[] materiaNumber1_;
    private int[] materiaNumber2_;
    private int[] materiaNumber3_;

    private RectTransform itemRecipeParent_;
    private Bag_Materia bagMateria_;// 所持素材を確認

    // ミニゲーム関連
    private MovePoint movePoint_;
    private MovePoint.JUDGE judge_ = MovePoint.JUDGE.NON;
    private Image judgeBack_;
    private TMPro.TextMeshProUGUI judgeText_;

    private Button createBtn_;  // 作成開始ボタン
    private Button cancelBtn_;  // 合成終了開始ボタン

    // 選択したアイテムの説明欄
    private TMPro.TextMeshProUGUI createName_;   // 合成したいアイテムの名前
    private TMPro.TextMeshProUGUI wantMateria_;  // 合成したいアイテムの必要素材

    private int[] maxRecipActiveCnt_ = new int[5] { 0, 5, 10, 15, 19 };

    void Start()
    {
        itemRecipeParent_ = transform.Find("ScrollView/Viewport/ItemRecipeParent").GetComponent<RectTransform>();
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

        popItemRecipeList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        maxCnt_ = popItemRecipeList_.SetMaxItemCount() / 2;
        Debug.Log("最大値" + maxCnt_);
        itemRecipeState = new itemRecipe[maxCnt_];
        maxMateriaCnt_ = popItemRecipeList_.SetMaxMateriaCount();

        //デバッグ用
        GameObject.Find("Managers").GetComponent<Bag_Materia>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
        haveCnt = new int[maxMateriaCnt_];
        for (int i = 0; i < maxMateriaCnt_; i++)
        {
            haveCnt[i] = Bag_Materia.materiaState[i].haveCnt;
        }

        Debug.Log(BookStoreMng.bookState_[25].readFlag);

        if (BookStoreMng.bookState_[5].readFlag == 1)
        {
            maxCnt_ = maxRecipActiveCnt_[4];
        }
        else if (BookStoreMng.bookState_[18].readFlag == 1)
        {
            maxCnt_ = maxRecipActiveCnt_[3];
        }
        else if (BookStoreMng.bookState_[11].readFlag == 1)
        {
            // レシピ初級を読んでいたら
            maxCnt_ = maxRecipActiveCnt_[2];
        }
        else if (BookStoreMng.bookState_[5].readFlag == 1)
        {
            // レシピ初級を読んでいたら
            startCnt_ = maxRecipActiveCnt_[0];
            maxCnt_ = maxRecipActiveCnt_[1];
        }
        Debug.Log("合成可能アイテム数" + maxCnt_);



        materiaNumber1_ = new int[maxCnt_];
        materiaNumber2_ = new int[maxCnt_];
        materiaNumber3_ = new int[maxCnt_];
        for (int i = startCnt_; i < maxCnt_; i++)
        {
            itemRecipeState[i].pleate = Instantiate(itemRecipePleate,
            new Vector2(0, 0), Quaternion.identity, itemRecipeParent_.transform);
            itemRecipeState[i].name = InitPopList.itemData[i].name;
            itemRecipeState[i].pleate.name = itemRecipeState[i].name;
            itemRecipeState[i].btn = itemRecipeState[i].pleate.GetComponent<Button>();

            // 表示するアイテムの名前
            itemRecipeState[i].nameText = itemRecipeState[i].pleate.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
            itemRecipeState[i].nameText.text = itemRecipeState[i].name;
            //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );

            var allNameCheck1 = InitPopList.itemData[i].materia1.Split('_');
            var allNameCheck2 = InitPopList.itemData[i].materia2.Split('_');
            var allNameCheck3 = InitPopList.itemData[i].materia3.Split('_');
            itemRecipeState[i].materia1 = allNameCheck1[0];
            itemRecipeState[i].materia2 = allNameCheck2[0];
            itemRecipeState[i].materia3 = allNameCheck3[0];

            materiaNumber1_[i] = int.Parse(allNameCheck1[1]);
            materiaNumber2_[i] = int.Parse(allNameCheck2[1]);
            materiaNumber3_[i] = int.Parse(allNameCheck3[1]);
            /////////////// haveCnt[i] = Bag_Materia.data[i].haveCnt;
            Debug.Log(haveCnt[i] + "個");
        }

        //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
        bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

        // ミニゲーム
        movePoint_ = miniGameMng.transform.GetComponent<MovePoint>();
        miniGameMng.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        judgeBack_ = miniGameMng.transform.Find("JudgeBack").GetComponent<Image>();
        judgeText_ = judgeBack_.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();

        cancelBtn_ = transform.Find("CancelBtn").GetComponent<Button>();
        createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();

        createName_ = transform.Find("InfoBack/CreateItemName").GetComponent<TMPro.TextMeshProUGUI>();
        wantMateria_ = transform.Find("InfoBack/WantMateriaNames").GetComponent<TMPro.TextMeshProUGUI>();
        ResetCommon();
    }

    public IEnumerator AlchemyRecipeSelect()
    {
        while (true)
        {
            if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NON)
            {
                yield return null;
            }
            else
            {
                if (movePoint_.GetMiniGameJudge() == MovePoint.JUDGE.NORMAL)
                {
                    judge_ = MovePoint.JUDGE.NORMAL;
                    judgeText_.text = "成功";
                }
                else
                {
                    judge_ = MovePoint.JUDGE.GOOD;
                    judgeText_.text = "大成功";
                }
                judgeBack_.gameObject.SetActive(true);
                bagItem_.ItemGetCheck(judge_, saveItemNum_, 1);
                saveBtn_.interactable = true;
                saveBtnName_ = "";

                yield return new WaitForSeconds(2.0f);
                ResetCommon();
                yield break;
            }
        }
    }

    public void SetActiveRecipe(int recipeNum, string name)
    {
        createName_.text = itemRecipeState[recipeNum].btn.name;
        if (itemRecipeState[recipeNum].materia3 == "non")
        {
            wantMateria_.text = "必要素材\n" +
                 itemRecipeState[recipeNum].materia1 +
           " " + itemRecipeState[recipeNum].materia2;
        }
        else
        {
            wantMateria_.text = "必要素材\n" +
                 itemRecipeState[recipeNum].materia1 +
           " " + itemRecipeState[recipeNum].materia2 +
           " " + itemRecipeState[recipeNum].materia3;
        }
    }

    public void OnClickCreate()
    {
        int recipeNum = -1;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (itemRecipeState[i].btn.interactable == false)
            {
                recipeNum = i;
                // Debug.Log("必要な素材1つ目：" + recipeList_.param[number].WantMateria1);
                // Debug.Log("必要な素材2つ目：" + recipeList_.param[number].WantMateria2);
                break;
            }
        }

        if (recipeNum != -1)
        {
            //Debug.Log("必要な素材を持っているかチェックします      " + bagMateria_.GetMaxHaveMateriaCnt());
            bool createFlag = true;
            if (materiaNumber3_[recipeNum] == nonMateriaNum_)
            {
                Debug.Log("3つ目の素材は必要ありません");
                if (haveCnt[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1つ目の素材が足りません");
                }
                else if (haveCnt[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia2 + "2つ目の素材が足りません");
                }
            }
            else
            {
                if (haveCnt[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1つ目の素材が足りません");
                }
                else if (haveCnt[materiaNumber2_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("3つ目の素材が足りません");
                }
                else if (haveCnt[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("2つ目の素材が足りません");
                }
            }

            if (createFlag == true)
            {
                createBtn_.interactable = false;
                cancelBtn_.interactable = false;
                miniGameMng.gameObject.SetActive(true);
                StartCoroutine(movePoint_.CountDown());
                StartCoroutine(AlchemyRecipeSelect());

                //Debug.Log("number[0]:" + number[0] + "   number[1]:" + number[1] + "   number[2]:" + number[2]);
                if (itemRecipeState[recipeNum].materia3 == "non")
                {
                    Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt--;
                }
                else
                {
                    Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt--;
                    Bag_Materia.materiaState[materiaNumber3_[recipeNum]].haveCnt--;
                }
            }
            else
            {
                Debug.Log("素材がないため合成することができません");
            }
        }
    }

    public void OnClickCancelCtn()
    {
        StopCoroutine(AlchemyRecipeSelect());
        gameObject.SetActive(false);
        uniHouseCanvas.gameObject.SetActive(true);
    }

    public void SetButtonName(string name)
    {
        saveBtnName_ = name;

        // レシピ選択処理
        for (int i = 0; i < maxCnt_; i++)
        {
            if (saveBtnName_ != itemRecipeState[i].btn.name)
            {
                // 選択してないボタンは押下できる状態にする
                itemRecipeState[i].btn.interactable = true;
            }
            else
            {
                // 選択したレシピの内容を表示
                SetActiveRecipe(i, saveBtnName_);
                saveBtn_ = itemRecipeState[i].btn;
                saveItemNum_ = i;// 番号を保存
                //saveName_ = Bag_Item.data[i].name;
                //Debug.Log("生成されたアイテム" + saveName_);
                if (miniGameMng.gameObject.activeSelf == false)
                {
                    createBtn_.interactable = true;
                }
            }
        }
    }

    private void ResetCommon()
    {
        // 何らかの判定をされた場合
        cancelBtn_.interactable = true;
        createBtn_.interactable = false;
        // 判定をリセット
        createName_.text = "";
        wantMateria_.text = "";
        miniGameMng.gameObject.SetActive(false);
        judge_ = MovePoint.JUDGE.NON;
        judgeText_.text = "";
        movePoint_.SetMiniGameJudge(judge_);
        judgeBack_.gameObject.SetActive(false);
    }
}