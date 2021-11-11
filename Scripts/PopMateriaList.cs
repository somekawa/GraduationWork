using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopMateriaList : MonoBehaviour
{
    private static bool onceFlag_ = false;// 一度だけ呼ばれるフラグ

    // バッグの中身と図鑑
    [SerializeField]
    private GameObject materiaUIPrefab;     // 素材を拾ったときに生成されるプレハブ
    private MateriaList materiaList_;   // 素材のExcelData読み込み
    private int maxMateriaCnt_ = 0;         //配列の最大値を保存
    private int singleMateriaCnt_ = 0;         // シートに記載されてる個数
    public static GameObject[] materiaBox_; // 生成したプレハブを保存


    // バッグの中身(アイテム)
    [SerializeField]
    private GameObject itemUIPrefab;     // 素材を拾ったときに生成されるプレハブ
    private ItemList itemList_;   // 素材のExcelData読み込み
    private int maxItemCnt_ = 0;// アイテムの最大個数
    public static GameObject[] itemBox_; // 生成したプレハブを保存
    public static int[] itemSellPrice_;  // 売るときの値段


    // バッグの中身（ワード）
    public enum WORD
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT_HEAL,   // 1 回復
        ELEMENT_ASSIST, // 2 補助
        ELEMENT_ATTACK, // 3 攻撃系
        TAIL,           // 4
        SUB1,           // 5 味方　敵
        SUB2,           // 6 HP 魔法攻撃　物理攻撃　防御力
        SUB1_AND_SUB2,  // 7 毒　暗闇　麻痺　即死
        SUB3,           // 8 上昇、低下、反射、必中
        ALL_SUB,         // 9　吸収
        INFO,           // 10
        MAX
    }
    public int sub2sCnt_ = 0;

    [SerializeField]
    private GameObject wordUIPrefab;     // 素材を拾ったときに生成されるプレハブ
    private WordList wordList_;   // 素材のExcelData読み込み
    private int maxWordCnt_ = 0;
    public static GameObject[] wordPleate_; // 生成したプレハブを保存
    public static int[] activeNum_;
    public static WORD[] wordKinds_;


    public enum FIELD_NUM
    {
        NON = -1,
        FOREST,
        FIELD1,
        FIELD2,
        FIELD3,
        FIELD4,
        MATERIAS,
        MAX
    }
    private ItemGet materiaGet_;                    // フィールド上で呼び出されるScript
    private int fieldNumber_ = (int)FIELD_NUM.FOREST;  // 現在いるフィールドの番号

    void Awake()
    {
        for (int i = 0; i < (int)FIELD_NUM.MAX; i++)
        {
            // SceneMngにあるenumとFIELD_NUMの値を合わせるためにnowScene-3
            if (((int)SceneMng.nowScene) - 3 == i)
            {
                fieldNumber_ = i;

                materiaGet_ = GameObject.Find("ItemPoints").GetComponent<ItemGet>();

                materiaGet_.SetMaterialKinds(fieldNumber_, materiaList_);

                break;
            }
        }
        materiaList_ = Resources.Load("MateriaList/M_Field" + fieldNumber_) as MateriaList;

        if (onceFlag_ == false)
        {
            // materiaList_.param.Countはどのシートでも数が同じ
            singleMateriaCnt_ = materiaList_.param.Count;
            maxMateriaCnt_ = (int)FIELD_NUM.MAX * singleMateriaCnt_;

            materiaBox_ = new GameObject[maxMateriaCnt_];
            int number = 0;
            for (int f = 0; f < (int)FIELD_NUM.MAX; f++)
            {
                materiaList_ = Resources.Load("MateriaList/M_Field" + f) as MateriaList;
                for (int m = 0; m < singleMateriaCnt_; m++)
                {
                    number = f * singleMateriaCnt_ + m;
                  //  Debug.Log("読み込んだ個数" + materiaList_.param[m].MateriaName);

                    materiaBox_[number] = Instantiate(materiaUIPrefab,
                            new Vector2(0, 0), Quaternion.identity, this.transform);
                    materiaBox_[number].name = materiaList_.param[m].MateriaName;
                }
            }


            itemList_ = Resources.Load("ItemList/ItemSheet1") as ItemList;
            maxItemCnt_ = itemList_.param.Count;
            itemBox_ = new GameObject[maxItemCnt_];
            itemSellPrice_ = new int[maxItemCnt_];
            // Debug.Log("あいてむのしゅるい" + maxItemCnt_);
            for (int i = 0; i < maxItemCnt_; i++)
            {
                itemBox_[i] = Instantiate(itemUIPrefab,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                itemBox_[i].name = itemList_.param[i].ItemName;
                itemSellPrice_[i] = itemList_.param[i].Price_Sell;
            }



            // ワードリストの読み込み
            wordList_ = Resources.Load("WordList/WordList0") as WordList;
            maxWordCnt_ = wordList_.param.Count;

            wordPleate_ = new GameObject[maxWordCnt_];
            activeNum_ = new int[maxWordCnt_];
            wordKinds_ = new WORD[maxWordCnt_];
            for (int w = 0; w < maxWordCnt_; w++)
            {
                wordPleate_[w] = Instantiate(wordUIPrefab,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                wordPleate_[w].name = wordList_.param[w].Word;
                activeNum_[w] = wordList_.param[w].ListNumber;
                wordKinds_[w] = (WORD)wordList_.param[w].KindsNumber;

                if (wordKinds_[w] == WORD.SUB2
                    || wordKinds_[w] == WORD.SUB1_AND_SUB2)
                {
                    sub2sCnt_++;
                        }
                //  Debug.Log(w + "番目：" + wordPleate_[w].name);
            }
            //GameObject.Find("Managers").GetComponent<Bag_Word>().Init();

            onceFlag_ = true;
        }
        //Debug.Log("PopMateriaListが呼ばれました");
    }

    public int SetMaxMateriaCount()
    {
        return maxMateriaCnt_;
    }

    public int SetSingleMateriaCount()
    {
        return singleMateriaCnt_;
    }

    public int SetMaxItemCount()
    {
        return maxItemCnt_;
    }

    public int SetMaxWordCount()
    {
        return maxWordCnt_;
    }

    public int SetMaxSub2sWordCount()
    {
        return sub2sCnt_;
    }
}