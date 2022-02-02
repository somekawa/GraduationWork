using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopListInTown : MonoBehaviour
{
    private InitPopList popLists_;

    private enum STORE_MNG
    {
        NON = -1,
        ITEM,
        BOOK,
        MAX,
    }
    private STORE_MNG storeMng_ = STORE_MNG.NON;

    public enum PRICE_KINDS
    {
        NON = -1,
        BUY,       // 買う選択中
        SELL,      // 売る選択中
        MAX
    }

    // アイテム売買
    // private MateriaList materiaList_;   // 素材のExcelData読み込み
    private int maxCnt_ = 0;         //配列の最大値を保存
    //private int singleMateriaCnt_ = 0;         // シートに記載されてる個数

    [SerializeField]
    private GameObject itemStorePleate;         // 素材を拾ったときに生成されるプレハブ
    public static GameObject[] materiaPleate;  // 生成したプレハブを保存
    public static GameObject[] itemPleate;  // 生成したプレハブを保存
    public static Text[] activeText_;         // 表示する素材の名前
    public static Text[] activePrice_;        // 表示する値段

    //--------------

    // 本関連
    private enum BOOK
    {
        NON = -1,
        BOOK_0,
        BOOK_1,
        BOOK_2,
        BOOK_3,
        BOOK_4,
        MAX
    }

    private BookList[] bookList_=new BookList[(int)BOOK.MAX];   // 本のExcelData読み込み
    private int maxBookCnt_ = 0;         //配列の最大値を保存
    private int singleBookCnt_ = 0;         // シートに記載されてる個数

    // 本を買う
    [SerializeField]
    private GameObject bookPlatePrefab;         // 素材を拾ったときに生成されるプレハブ
    public static GameObject[] bookObj;  // 生成したプレハブを保存
    public static int[] bookImageNum;
    public static int[] bookWordNum;
    public static int[] bookPrice;   // 値段
    public static string[] statusUp;
    public static string[] bookInfo;

    //---------------

    private GameObject DataPopPrefab_;

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

        for (int b = 0; b < (int)BOOK.MAX; b++)
        {
            bookList_[b] = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, b);
            maxBookCnt_ += bookList_[b].param.Count;
        }

        Debug.Log("本の個数" + maxBookCnt_);
        // 本関連
        int number = 0;
        bookObj = new GameObject[maxBookCnt_];
        bookPrice = new int[maxBookCnt_];
        bookImageNum = new int[maxBookCnt_];
        bookWordNum = new int[maxBookCnt_];
        bookInfo = new string[maxBookCnt_];
        statusUp = new string[maxBookCnt_];

        for (int b = 0; b < (int)BOOK.MAX; b++)
        {
            singleBookCnt_ = bookList_[b].param.Count;
            for (int m = 0; m < singleBookCnt_; m++)
            {
                //if(maxBookCnt_<=number)
                //{
                //    break;
                //}
                bookObj[number] = Instantiate(bookPlatePrefab,
                    new Vector2(0, 0), Quaternion.identity, transform);
                bookObj[number].name = bookList_[b].param[m].BookName;
                bookWordNum[number] = bookList_[b].param[m].WordNumber;
                bookImageNum[number] = bookList_[b].param[m].ImageNumber;
                bookPrice[number] = bookList_[b].param[m].Price;
                statusUp[number] = bookList_[b].param[m].GetCheck;
                bookInfo[number] = bookList_[b].param[m].Info;
               //Debug.Log(number+"           1ページの本の個数" + bookObj[number]);
                number++;
            }
        }

        // 魔道具店関連
        popLists_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        maxCnt_ = popLists_.SetMaxMateriaCount();
        materiaPleate = new GameObject[maxCnt_];
        activePrice_ = new Text[maxCnt_];
        activeText_ = new Text[maxCnt_];
        int count = 0;
        for (int i = 0; i < maxCnt_; i++)
        {
            materiaPleate[i] = Instantiate(itemStorePleate,
                new Vector2(0, 0), Quaternion.identity, transform);
            materiaPleate[i].name = InitPopList.materiaData[i].name;
            count++;
        }
        maxCnt_ = popLists_.SetMaxItemCount();
        itemPleate = new GameObject[maxCnt_ * 2];
        for (int i = 0; i < maxCnt_ * 2; i++)
        {
            itemPleate[i] = Instantiate(itemStorePleate,
                    new Vector2(0, 0), Quaternion.identity, transform);
            // itemPleate[i].name = InitPopList.itemData[i].name;
        }
        Debug.Log("タウンでのデータを読み込み終わりました");
    }

    public int SetMaxBookCount()
    {
        return maxBookCnt_;
    }

    public int SetSingleBookCount()
    {
        return singleBookCnt_;
    }
}