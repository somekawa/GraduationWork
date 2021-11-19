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
    private int maxMateriaCnt_ = 0;         //配列の最大値を保存
    //private int singleMateriaCnt_ = 0;         // シートに記載されてる個数

    [SerializeField]
    private GameObject itemStorePleate;         // 素材を拾ったときに生成されるプレハブ
    public static GameObject[] materiaPleate;  // 生成したプレハブを保存
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

    private BookList bookList_;   // 本のExcelData読み込み
    private int maxBookCnt_ = 0;         //配列の最大値を保存
    private int singleBookCnt_ = 0;         // シートに記載されてる個数

    // 本を買う
    [SerializeField]
    private GameObject bookPlatePrefab;         // 素材を拾ったときに生成されるプレハブ
    public static GameObject[] bookObj_;  // 生成したプレハブを保存
    public static int[] bookPrice_;   // 値段
    public static string[] bookInfo_;
                                      //---------------

    private GameObject DataPopPrefab_;

    void Awake()
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popLists_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        bookList_ = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, 0);

        // 本関連
        int number = 0;
        singleBookCnt_ = bookList_.param.Count;
        maxBookCnt_ = (int)BOOK.MAX * singleBookCnt_;

        bookObj_ = new GameObject[maxBookCnt_];
        bookPrice_ = new int[maxBookCnt_];
        bookInfo_ = new string[maxBookCnt_];
        for (int b = 0; b < (int)BOOK.MAX; b++)
        {
            bookList_ = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, b);
            for (int m = 0; m < singleBookCnt_; m++)
            {
                number = b * singleBookCnt_ + m;
                bookObj_[number] = Instantiate(bookPlatePrefab,
                    new Vector2(0, 0), Quaternion.identity, this.transform);
                bookObj_[number].name = bookList_.param[m].BookName;
                bookPrice_[number] = bookList_.param[m].Price;
                bookInfo_[number] = bookList_.param[m].Info;
            }
        }

        // 魔道具店関連
        maxMateriaCnt_ = popLists_.SetMaxMateriaCount();
        materiaPleate = new GameObject[maxMateriaCnt_];
        activePrice_ = new Text[maxMateriaCnt_];
        activeText_ = new Text[maxMateriaCnt_];

        for (int i = 0; i < maxMateriaCnt_; i++)
        {
            materiaPleate[i] = Instantiate(itemStorePleate,
                new Vector2(0, 0), Quaternion.identity, this.transform);
            materiaPleate[i].name = InitPopList.materiaData[i].name;
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