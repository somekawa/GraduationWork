using System.Collections.Generic;
using System.IO;
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
        public string info;     // アイテムの説明
        public int sellPrice;   // 売るときの値段
        public int buyPrice;    // 買う時の値段
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
    public static int[,] dropNum = new int[5,3];//

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
    public static GameObject[,] pleate;   // インスタンスしたオブジェクトを保存
    public static string[,] name;         // ワード名
    public static int[,] activeNum;       // 表示してよいストーリーの番号
    public static WORD[,] kinds;          // ワードの種類
    public static int[,] power;
    public static int[,] MP;

    public static int[] maxWordKindsCnt = new int[(int)WORD.INFO];
    public static string[] materiaInfo_;
    public IEnumerator<string> information_;


    void Awake()
    {
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        // StreamingAssetsからAssetBundleをロードする
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle開く");
        // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // 不要になったAssetBundleのメタ情報をアンロードする
        assetBundle.Unload(false);
        Debug.Log("破棄");

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
              //  Debug.Log("InitPopListでのアイテムの名前：" + itemData[i].name);
                itemData[i].info = itemList_.param[i].Information;
                itemData[i].box.name = itemData[i].name;
                itemData[i].sellPrice = itemList_.param[i].Price_Sell;
                itemData[i].buyPrice = itemList_.param[i].Price_Buy;
                itemData[i].materia1 = itemList_.param[i].WantMateria1;
                itemData[i].materia2 = itemList_.param[i].WantMateria2;
                itemData[i].materia3 = itemList_.param[i].WantMateria3;
            }

            // Excelから素材のデータを読み込む
            materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, fieldNumber_);
            //singleMateriaCnt_ = materiaList_.param.Count;
            maxMateriaCnt_ = 35;// (int)FIELD_NUM.MAX * singleMateriaCnt_;
            materiaData = new MateriaData[maxMateriaCnt_];
            materiaInfo_ = new string[maxMateriaCnt_];

            // 1つのフィールドにつきドロップするのは3種類のため
            dropNum = new int[9, 3];// singleMateriaCnt_,3];

            int number = 0;
            int test=0;
            for (int f = 0; f < (int)FIELD_NUM.MAX; f++)
            {
                materiaList_ = DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, f);
                singleMateriaCnt_= materiaList_.param.Count;
                test += materiaList_.param.Count;
               //     Debug.Log(test + "      ");
                for (int i = 0; i < singleMateriaCnt_; i++)
                {
                    materiaData[number].box = Instantiate(materiaUIBox,
                        new Vector2(0, 0), Quaternion.identity, transform);
                    materiaData[number].name = materiaList_.param[i].MateriaName;
                    materiaData[number].box.name = materiaData[number].name;
                    materiaData[number].buyPrice = materiaList_.param[i].Price_Buy;
                    materiaData[number].sellPrice = materiaList_.param[i].Price_Sell;
                    materiaData[number].info = materiaList_.param[i].Info;

                    if (i < 3)
                    {
                        dropNum[f, i] = number;
                       // Debug.Log(i + "      " + dropNum[f, i]);
                        // drop_Field0_[i] = number;
                    }

                    number++;
                }
            }

            // Excelからワードのデータを読み込む
            wordList_ = DataPopPrefab_.GetComponent<PopList>().GetData<WordList>(PopList.ListData.WORD);
            maxWordCnt_ = wordList_.param.Count;
            pleate = new GameObject[(int)WORD.INFO, maxWordCnt_];   // インスタンスしたオブジェクトを保存
            name = new string[(int)WORD.INFO, maxWordCnt_];         // ワード名
            activeNum = new int[(int)WORD.INFO, maxWordCnt_];       // 表示してよいストーリーの番号
            kinds = new WORD[(int)WORD.INFO, maxWordCnt_];          // ワードの種類
            power = new int[(int)WORD.INFO, maxWordCnt_];
            MP = new int[(int)WORD.INFO, maxWordCnt_];
            for (int k = 0; k < (int)WORD.INFO; k++)
            {
                for (int i = 0; i < maxWordCnt_; i++)
                {
                    if (k == wordList_.param[i].KindsNumber)
                    {
                        maxWordKindsCnt[k]++;   // 各ワード種類の最大個数を確認
                    }
                }
            }
            int count = 0;// 各種類に応じた最大数
            for (int k = 0; k < (int)WORD.INFO; k++)
            {
                for (int i = 0; i < maxWordCnt_; i++)
                {
                    if (k == wordList_.param[i].KindsNumber)
                    {
                        name[k, count] = wordList_.param[i].Word;
                        pleate[k, count] = Instantiate(wordUIPleate,
                               new Vector2(0, 0), Quaternion.identity, this.transform);

                        pleate[k, count].name = name[k, count];
                        //Debug.Log(num+ "            " + data[num].name);
                        activeNum[k, count] = wordList_.param[i].ListNumber;
                        kinds[k, count] = (WORD)wordList_.param[i].KindsNumber;
                        power[k, count] = wordList_.param[i].Power;
                        MP[k, count] = wordList_.param[i].MP;

                        //  Debug.Log((WORD)k +"の"+ count + "番"+ name[k, count]);
                        count++;
                    }
                }
                count = 0;// カウントをリセット
            }
            onceFlag_ = true;
        }
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
