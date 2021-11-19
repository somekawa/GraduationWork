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

    private Bag_Item bagItem_;// 所持しているアイテムを確認

    private string saveBtnName_ = "";   // どのアイテムを押したかを保存
    private Button saveBtn_;            // どのボタンを押したかを保存
    private int saveItemNum_ = 0;

    public struct itemRecipe
    {
        public GameObject pleate;   // アイテムを表示するオブジェクト
        public Button btn;          // アイテムクリック
        public Text nameText;       // アイテムの名前を表示するText
        public string name;         // アイテムの名前
        public string materia1;     // 合成に必要な素材1
        public string materia2;     // 合成に必要な素材2
        public string materia3;     // 合成に必要な素材3
    }
    public static itemRecipe[] itemRecipeState;

    private RectTransform itemRecipeParent_;
    private Bag_Materia bagMateria_;// 所持素材を確認

    // ミニゲーム関連
    private MovePoint movePoint_;
    private MovePoint.JUDGE judge_ = MovePoint.JUDGE.NON;
    private Image judgeBack_;
    private Text judgeText_;

    private Button createBtn_;  // 作成開始ボタン
    private Button cancelBtn_;  // 合成終了開始ボタン

    // 選択したアイテムの説明欄
    private Text createName_;   // 合成したいアイテムの名前
    private Text wantMateria_;  // 合成したいアイテムの必要素材

    void Start()
    {
        itemRecipeParent_ = transform.Find("ScrollView/Viewport/ItemRecipeParent").GetComponent<RectTransform>();
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

        popItemRecipeList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        maxCnt_ = popItemRecipeList_.SetMaxItemCount();
        itemRecipeState = new itemRecipe[maxCnt_];

        for (int i = 0; i < maxCnt_; i++)
        {
            itemRecipeState[i].pleate = Instantiate(itemRecipePleate,
            new Vector2(0, 0), Quaternion.identity, itemRecipeParent_.transform);
            itemRecipeState[i].name = InitPopList.itemData[i].name;
            itemRecipeState[i].pleate.name = itemRecipeState[i].name;
            itemRecipeState[i].btn = itemRecipeState[i].pleate.GetComponent<Button>();

            // 表示するアイテムの名前
            itemRecipeState[i].nameText = itemRecipeState[i].pleate.transform.Find("Text").GetComponent<Text>();
            itemRecipeState[i].nameText.text = itemRecipeState[i].name;
            //  Debug.Log(wantMap_[WANT.MATERIA_0][number] );
            itemRecipeState[i].materia1 = InitPopList.itemData[i].materia1;
            itemRecipeState[i].materia2 = InitPopList.itemData[i].materia2;
            itemRecipeState[i].materia3 = InitPopList.itemData[i].materia3;
        }

        //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
        bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

        // ミニゲーム
        movePoint_ = miniGameMng.transform.GetComponent<MovePoint>();
        miniGameMng.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        judgeBack_ = miniGameMng.transform.Find("JudgeBack").GetComponent<Image>();
        judgeText_ = judgeBack_.transform.Find("Text").GetComponent<Text>();

        cancelBtn_ = transform.Find("CancelBtn").GetComponent<Button>();
        createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();

        createName_ = transform.Find("InfoBack/CreateItemName").GetComponent<Text>();
        wantMateria_ = transform.Find("InfoBack/WantMateriaNames").GetComponent<Text>();
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
                    judgeText_.text = "成功";
                }
                else
                {
                    judgeText_.text = "大成功";
                }
                judgeBack_.gameObject.SetActive(true);
                bagItem_.ItemGetCheck(saveItemNum_, saveBtnName_, 1);
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
        bool flag = false;
        int recipeNum = 0;
        int[] number = new int[3];
        //int number = 0;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (itemRecipeState[i].btn.interactable == false)
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

            number[0] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia1);

            number[2] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia3);

            number[1] = HaveMateriaCheck(recipeNum, itemRecipeState[recipeNum].materia2);

            Debug.Log("number[0]:" + number[0] + "   number[1]:" + number[1] + "   number[2]:" + number[2]);
            if (itemRecipeState[recipeNum].materia3 == "non")
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
        Debug.Log("必要素材" + wantMateria);

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
                    Debug.Log(Bag_Materia.materiaState[i].name + "を持ってます");
                    haveMateriaNum = i;
                    if (Bag_Materia.materiaState[i].haveCnt < 1)
                    {
                        Debug.Log("2つ目の素材の所持数が足りません");
                        return 0;
                    }
                    else
                    {
                        if (wantMateria == itemRecipeState[recipeNum].materia2)
                        {
                            createBtn_.interactable = false;
                            miniGameMng.gameObject.SetActive(true);
                            StartCoroutine(movePoint_.CountDown());
                            StartCoroutine(AlchemyRecipeSelect());
                            cancelBtn_.interactable = false;
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