using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopListInTown : MonoBehaviour
{
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
    private MateriaList materiaList_;   // 素材のExcelData読み込み
    private int maxMateriaCnt_ = 0;         //配列の最大値を保存
    private int singleMateriaCnt_ = 0;         // シートに記載されてる個数

    [SerializeField]
    private GameObject materiaPlatePrefab;         // 素材を拾ったときに生成されるプレハブ
    public static GameObject[] activeObj_;  // 生成したプレハブを保存
    public static int[] mateiraSellPrice_;  // 売るときの値段
    public static int[] mateiraBuyPrice_;   // 買う時の値段
    public static string[] materiaInfo_;    // アイテムの説明

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
                                      //---------------

    private GameObject DataPopPrefab_;
    private object[] popList_=new object[(int)STORE_MNG.MAX];

    void Start()
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
    
        // materiaList_ = Resources.Load("MateriaList/M_Field" + 0) as MateriaList;        
        // bookList_ = Resources.Load("BookList/BookList" + 0) as BookList;
        
        popList_[(int)STORE_MNG.ITEM] = (MateriaList)DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, 0);
        popList_[(int)STORE_MNG.BOOK] = (BookList)DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, 0);



        // 魔道具店関連
        singleMateriaCnt_ = ((MateriaList)popList_[(int)STORE_MNG.ITEM]).param.Count;

     //   singleMateriaCnt_ = materiaList_.param.Count;
        maxMateriaCnt_ = (int)PopMateriaList.FIELD_NUM.MAX * singleMateriaCnt_;

        activeObj_ = new GameObject[maxMateriaCnt_];
        mateiraSellPrice_ = new int[maxMateriaCnt_];
        mateiraBuyPrice_ = new int[maxMateriaCnt_];
        materiaInfo_ = new string[maxMateriaCnt_];

        int number = 0;
        for (int f = 0; f < (int)PopMateriaList.FIELD_NUM.MAX; f++)
        {
            popList_[(int)STORE_MNG.ITEM] = (MateriaList)DataPopPrefab_.GetComponent<PopList>().GetData<MateriaList>(PopList.ListData.MATERIA, f);
            for (int m = 0; m < singleMateriaCnt_; m++)
            {
                number = f * singleMateriaCnt_ + m;

                activeObj_[number] = Instantiate(materiaPlatePrefab,
                    new Vector2(0, 0), Quaternion.identity, this.transform);
                activeObj_[number].name = ((MateriaList)popList_[(int)STORE_MNG.ITEM]).param[m].MateriaName;

                mateiraSellPrice_[number] = ((MateriaList)popList_[(int)STORE_MNG.ITEM]).param[m].Price_Sell;
                mateiraBuyPrice_[number] = ((MateriaList)popList_[(int)STORE_MNG.ITEM]).param[m].Price_Buy;
                materiaInfo_[number] = ((MateriaList)popList_[(int)STORE_MNG.ITEM]).param[m].Explanation;
            }
        }

        // 本関連
        number = 0;
        singleBookCnt_ = ((BookList)popList_[(int)STORE_MNG.BOOK]).param.Count;

      //  singleBookCnt_ = bookList_.param.Count;
        maxBookCnt_ = (int)BOOK.MAX * singleBookCnt_;

        bookObj_ = new GameObject[maxBookCnt_];
        bookPrice_ = new int[maxBookCnt_];
        for (int b = 0; b < (int)BOOK.MAX; b++)
        {
            popList_[(int)STORE_MNG.BOOK] = (BookList)DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, b);
            for (int m = 0; m < singleBookCnt_; m++)
            {
                number = b * singleBookCnt_ + m;
                bookObj_[number] = Instantiate(bookPlatePrefab,
                    new Vector2(0, 0), Quaternion.identity, this.transform);
                bookObj_[number].name = ((BookList)popList_[(int)STORE_MNG.BOOK]).param[m].BookName;
                bookPrice_[number] = ((BookList)popList_[(int)STORE_MNG.BOOK]).param[m].Price;
            }
        }
    }

    public int SetMaxItemsCount()
    {
        return maxMateriaCnt_;
    }

    public int SetSingleItemsCount()
    {
        return singleMateriaCnt_;
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