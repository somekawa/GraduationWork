using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemCreateMng : MonoBehaviour
{
    [SerializeField]
    private GameObject miniGameObj;// ミニゲーム用のオブジェクト

    [SerializeField]
    private Canvas uniHouseCanvas;

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
    private int[] haveCnt_;// 所持数
    private int nonMateriaNum_ = -1;
    private int[] materiaNumber1_;
    private int[] materiaNumber2_;
    private int[] materiaNumber3_;

    private RectTransform itemRecipeParent_;
    private Bag_Materia bagMateria_;// 所持素材を確認

    //// ミニゲーム関連
    private CircleMng circleMng_;
    private int circleNum_ = 6;

    private Button createBtn_;  // 作成開始ボタン
    private Button cancelBtn_;  // 合成終了開始ボタン

    // 選択したアイテムの説明欄
    private Image arrowImage_;
    private Image itemIconBack_;// アイテムのアイコンの背景
    private Image itemIcon_;// アイテム画像
    private Image itemNameFrame_;// アイテム名前位置の背景
    private TMPro.TextMeshProUGUI createName_;   // 合成したいアイテムの名前
    private TMPro.TextMeshProUGUI itemhaveCnt_;   // 合成したいアイテムの所持数
    private TMPro.TextMeshProUGUI haveCntTitle_;   // 合成したいアイテムの所持数

    private TMPro.TextMeshProUGUI needMateria_;  // 合成したいアイテムの必要素材
    private TMPro.TextMeshProUGUI needCnt_;

    private int[] maxRecipActiveCnt_ = new int[5] { 0, 5, 10, 15, 19 };

    // デバッグ用
    private SaveLoadCSV saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト

    public void Init()
    {
        // デバッグ用
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>();
        saveCsvSc_.LoadData(SaveLoadCSV.SAVEDATA.BOOK);

        GameObject.Find("Managers").GetComponent<Bag_Word>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Materia>().DataLoad();


        if (circleMng_==null)
        {
            circleMng_ = miniGameObj.transform.Find("ElementCircle/JudgeCircle").GetComponent<CircleMng>();
            itemRecipeParent_ = transform.Find("ScrollView/Viewport/ItemRecipeParent").GetComponent<RectTransform>();
            bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();

            //GameObject.Find("DontDestroyCanvas/ItemBagMng").GetComponent<ItemBagMng>().Init();
            bagItem_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>();

            cancelBtn_ = transform.Find("CancelBtn").GetComponent<Button>();
            createBtn_ = transform.Find("CreateBtn").GetComponent<Button>();

            var infoBack= transform.Find("InfoBack").GetComponent<RectTransform>();
            arrowImage_ = infoBack.Find("ArrowImage").GetComponent<Image>();
            itemNameFrame_= infoBack.Find("ItemNameFrame").GetComponent<Image>();
            createName_ = itemNameFrame_.transform.Find("ItemName").GetComponent<TMPro.TextMeshProUGUI>();
            itemIconBack_ = createName_.transform.Find("ImageBack").GetComponent<Image>();
            itemIcon_ = itemIconBack_.transform.Find("ItemImage").GetComponent<Image>();
            itemhaveCnt_ = createName_.transform.Find("ItemCnt").GetComponent<TMPro.TextMeshProUGUI>();
            haveCntTitle_ = createName_.transform.Find("CntTitle").GetComponent<TMPro.TextMeshProUGUI>();
            haveCntTitle_.text = "所持数\nEx";

            needMateria_ = transform.Find("InfoBack/NeedMateria").GetComponent<TMPro.TextMeshProUGUI>();
            needCnt_= needMateria_.transform.Find("NeedCnt").GetComponent<TMPro.TextMeshProUGUI>();
        
            
            popItemRecipeList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
            maxCnt_ = popItemRecipeList_.SetMaxItemCount() / 2;
            Debug.Log("最大値" + maxCnt_);
            itemRecipeState = new itemRecipe[maxCnt_];
            maxMateriaCnt_ = popItemRecipeList_.SetMaxMateriaCount();

            haveCnt_ = new int[maxMateriaCnt_];
            for (int i = 0; i < maxMateriaCnt_; i++)
            {
                haveCnt_[i] = Bag_Materia.materiaState[i].haveCnt;
            }

            Debug.Log(BookStoreMng.bookState_[25].readFlag);

            if (BookStoreMng.bookState_[25].readFlag == 1)
            {
                // レシピ極級を読んでいたら
                maxCnt_ = maxRecipActiveCnt_[4];
            }
            else if (BookStoreMng.bookState_[18].readFlag == 1)
            {
                // レシピ高級を読んでいたら
                maxCnt_ = maxRecipActiveCnt_[3];
            }
            else if (BookStoreMng.bookState_[11].readFlag == 1)
            {
                // レシピ中級を読んでいたら
                maxCnt_ = maxRecipActiveCnt_[2];
            }
            else if (BookStoreMng.bookState_[5].readFlag == 1)
            {
                // レシピ初級を読んでいたら
                startCnt_ = maxRecipActiveCnt_[0];
                maxCnt_ = maxRecipActiveCnt_[1];
            }
            else
            {
                maxCnt_ = 0;// レシピ本を所持してない
            }
            Debug.Log("合成可能アイテム数" + maxCnt_);

            if (maxCnt_ == 0)
            {
                // レシピ本を所持してない場合
                createName_.text = null;
                needMateria_.text = "本屋でレシピを購入しましょう";
                createBtn_.interactable = false;
                return;
            }

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
                 haveCnt_[i] = Bag_Materia.materiaState[i].haveCnt;
              //  Debug.Log(haveCnt[i] + "個");
            }

        }
        miniGameObj.gameObject.SetActive(false);
        miniGameObj.transform.localPosition = new Vector3(-0.25f, 0.8f, 0.2f);


        ResetCommon();
    }

    public void AlchemyRecipeSelect()
    {
        int rate = (int)circleMng_.GetMiniGameJudge();
        bagItem_.ItemGetCheck(rate, saveItemNum_, 1);
        saveBtn_.interactable = true;
        saveBtnName_ = "";

        ResetCommon();
    }

    public void SetActiveRecipe(int recipeNum, string name)
    {
        // 画像のアイコン
        itemIconBack_.gameObject.SetActive(true);
        itemIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][recipeNum];
        // 矢印
        arrowImage_.gameObject.SetActive(true);
        // 選択したアイテムの名前
        itemNameFrame_.gameObject.SetActive(true);
        createName_.text = itemRecipeState[recipeNum].btn.name;
        // 「所持」数と「EX」所持数
        haveCntTitle_.gameObject.SetActive(true);
        // 所持数とEX所持数
        itemhaveCnt_.text = Bag_Item.itemState[recipeNum].haveCnt + "コ\n" 
                          + Bag_Item.itemState[recipeNum * 2].haveCnt + "コ\n";
        if (itemRecipeState[recipeNum].materia3 == "non")
        {
            needMateria_.text = itemRecipeState[recipeNum].materia1 +
                         "\n" + itemRecipeState[recipeNum].materia2;
            needCnt_.text = Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt + "/1\n"
                          + Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt + "/1";
        }
        else
        {
            needMateria_.text = itemRecipeState[recipeNum].materia1 +
                         "\n" + itemRecipeState[recipeNum].materia2 +
                         "\n" + itemRecipeState[recipeNum].materia3;
            needCnt_.text = Bag_Materia.materiaState[materiaNumber1_[recipeNum]].haveCnt + "/1\n"
                          + Bag_Materia.materiaState[materiaNumber2_[recipeNum]].haveCnt + "/1"
                          + Bag_Materia.materiaState[materiaNumber3_[recipeNum]].haveCnt + "/1";
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
                if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1つ目の素材が足りません");
                }
                else if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia2 + "2つ目の素材が足りません");
                }
            }
            else
            {
                if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log(itemRecipeState[recipeNum].materia1 + "1つ目の素材が足りません");
                }
                else if (haveCnt_[materiaNumber2_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("3つ目の素材が足りません");
                }
                else if (haveCnt_[materiaNumber1_[recipeNum]] < 1)
                {
                    createFlag = false;
                    Debug.Log("2つ目の素材が足りません");
                }
            }

            if (createFlag == true)
            {
                createBtn_.interactable = false;
                cancelBtn_.interactable = false;
                miniGameObj.gameObject.SetActive(true);
                circleMng_.Init(circleNum_, recipeNum%4);

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
                createBtn_.interactable = false;
                Debug.Log("素材がないため合成することができません");
            }
        }
    }

    public void OnClickCancelBtn()
    {
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
                if (miniGameObj.gameObject.activeSelf == false)
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
        createName_.text = null;
        itemhaveCnt_.text = null;
        needMateria_.text = null;
        needCnt_.text = null;
        arrowImage_.gameObject.SetActive(false);
        haveCntTitle_.gameObject.SetActive(false);
        itemIconBack_.gameObject.SetActive(false);
        itemNameFrame_.gameObject.SetActive(false);
    }
}