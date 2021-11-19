using System.Collections.Generic;
using UnityEngine;

public class InitPopList : MonoBehaviour
{
    private static bool onceFlag_ = false;// 一度だけ呼ばれるフラグ
    private GameObject DataPopPrefab_;  // 各リストを呼び出す
    private ItemList itemList_;         // アイテムのExcelData読み込み
    private MateriaList materiaList_;   // 素材のExcelData読み込み
    private WordList wordList_;         // ワードのExcelData読み込み

    // アイテム関連
    [SerializeField]
    private GameObject itemUIBox;     // 素材を拾ったときに生成されるプレハブ
    private int maxItemCnt_ = 0;// アイテムの最大個数
    public struct ItemData
    {
        public GameObject box;  // インスタンスしたオブジェクトを保存
        public string name;     // アイテム名
        public int sellPrice;   // 売るときの値段
        public string materia1; // アイテム合成で必要な素材1
        public string materia2; // アイテム合成で必要な素材2
        public string materia3; // アイテム合成で必要な素材3
    }
    public static ItemData[] itemData;

    // 素材関連

    public enum FIELD_NUM
    {
        NON = -1,
        FOREST,
        FIELD1,
        FIELD2,
        FIELD3,
        FIELD4,
        MAX
    }
    private int fieldNumber_ = (int)FIELD_NUM.FOREST;  // 現在いるフィールドの番号

    [SerializeField]
    private GameObject materiaUIBox;     // 素材を拾ったときに生成されるプレハブ
    private int maxMateriaCnt_ = 0;         //配列の最大値を保存
    private int singleMateriaCnt_ = 0;         // シートに記載されてる個数
    public static GameObject[] materiaBox_; // 生成したプレハブを保存

    public struct MateriaData
    {
        public GameObject box;  // インスタンスしたオブジェクトを保存
        public string name;     // 素材の名前
        public int sellPrice;   // 売るときの値段
        public int buyPrice;    // 買う時の値段
        public string info;     // 素材説明
    }
    public static MateriaData[] materiaData;

    // ワード関連
    public enum WORD
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT_HEAL,   // 1 回復
        ELEMENT_ASSIST, // 2 補助
        ELEMENT_ATTACK, // 3 攻撃系
        TAIL,           // 4
        SUB1,           // 5 味方　敵
        SUB2,           // 6 HP 魔法攻撃　物理攻撃　防御力 命中/回避
        SUB1_AND_SUB2,  // 7 毒　暗闇　麻痺　即死
        SUB3,           // 8 上昇、低下、反射
        ALL_SUB,         // 9　吸収、必中
        INFO,           // 10
        MAX
    }

    [SerializeField]
    private GameObject wordUIPleate;     // 生成したいオブジェクトのプレハブ
    private int maxWordCnt_ = 0;
    public struct WordData
    {
        public GameObject pleate;   // インスタンスしたオブジェクトを保存
        public string name;         // ワード名
        public int activeNum;       // 表示してよいストーリーの番号
        public WORD kinds;          // ワードの種類
    }
    public static Dictionary<WORD, WordData[]> wordData = new Dictionary<WORD, WordData[]>();

    public static int[] maxWordCnt = new int[(int)WORD.INFO] ;


    void Awake()
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        for (int i = 0; i < (int)FIELD_NUM.MAX; i++)
        {
            // SceneMngにあるenumとFIELD_NUMの値を合わせるためにnowScene-3
            if (((int)SceneMng.nowScene) - 3 == i)
            {
                fieldNumber_ = i;
                break;
            }
            else
            {
                fieldNumber_ = 0;
            }
        }

        if (onceFlag_ == false)
        {
            // Excelからアイテムのデータを読み込む
            itemList_ = DataPopPrefab_.GetComponent<PopList>().GetData<ItemList>(PopList.ListData.ITEM);
            maxItemCnt_ = itemList_.param.Count;
            itemData = new ItemData[maxItemCnt_];
           // Debug.Log("あいてむのしゅるい" + maxItemCnt_);
            for (int i = 0; i < maxItemCnt_; i++)
            {
                itemData[i].box = Instantiate(itemUIBox,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                itemData[i].name = itemList_.param[i].ItemName;
                itemData[i].box.name = itemData[i].name;
                itemData[i].sellPrice = itemList_.param[i].Price_Sell;
                itemData[i].materia1 = itemList_.param[i].WantMateria1;
                itemData[i].materia2 = itemList_.param[i].WantMateria2;
                itemData[i].materia3 = itemList_.param[i].WantMateria3;
            }

            // Excelから素材のデータを読み込む
            materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, fieldNumber_);
            singleMateriaCnt_ = materiaList_.param.Count;
            maxMateriaCnt_ = (int)FIELD_NUM.MAX * singleMateriaCnt_;
            materiaData = new MateriaData[maxMateriaCnt_];

            int number = 0;
            for (int f = 0; f < (int)FIELD_NUM.MAX; f++)
            {
                materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, f);
                for (int i = 0; i < singleMateriaCnt_; i++)
                {
                    number = f * singleMateriaCnt_ + i;
                    materiaData[number].box = Instantiate(materiaUIBox,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                    materiaData[number].name = materiaList_.param[i].MateriaName;
                    materiaData[number].box.name = materiaData[number].name;
                    materiaData[number].buyPrice = materiaList_.param[i].Price_Buy;
                    materiaData[number].sellPrice = materiaList_.param[i].Price_Sell;
                }
            }

            // Excelからワードのデータを読み込む
            wordList_ = DataPopPrefab_.GetComponent<PopList>().GetData<WordList>(PopList.ListData.WORD);
            maxWordCnt_ = wordList_.param.Count;

            for (int k = 0; k < (int)WORD.INFO; k++)
            {
                for (int i = 0; i < maxWordCnt_; i++)
                {
                    if (k == wordList_.param[i].KindsNumber)
                    {
                        // 各ワード種類の最大個数を確認
                        maxWordCnt[k]++;                
                    }
                }
                //Debug.Log((InitPopList.WORD)k + "   InitPopList " + maxWordCnt[k]);
                wordData[(WORD)k] = test((WORD)k, maxWordCnt[k]);
            }

            onceFlag_ = true;
        }
    }

    public WordData[] test(WORD kind,int maxCnt)
    {
        // numはwordList用の番号
        var data = new WordData[maxCnt];
        int count = 0;
        for (int i = 0; i < maxWordCnt_; i++)
        {
            if ((int)kind == wordList_.param[i].KindsNumber)
            {
                data[count].name = wordList_.param[i].Word;
                data[count].pleate = Instantiate(wordUIPleate,
                       new Vector2(0, 0), Quaternion.identity, this.transform);
                data[count].pleate.name = data[count].name;
                //Debug.Log(num+ "            " + data[num].name);
                data[count].activeNum = wordList_.param[i].ListNumber;
                data[count].kinds= (WORD)wordList_.param[i].KindsNumber;
                count++;
            }
        }
        // Debug.Log(listNum+"数"+num+" "+kind);
        return data;
    }

    public int SetMaxMateriaCount()
    {
        return maxMateriaCnt_;
    }

    public int SetMaxItemCount()
    {
        return maxItemCnt_;
    }

    public int SetMaxWordCount()
    {
        return maxWordCnt_;
    }

    public int SetNowFieldMateriaList()
    {
        return fieldNumber_;
    }
}
