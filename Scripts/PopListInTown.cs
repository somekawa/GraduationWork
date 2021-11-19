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
        BUY,       // �����I��
        SELL,      // ����I��
        MAX
    }

    // �A�C�e������
    // private MateriaList materiaList_;   // �f�ނ�ExcelData�ǂݍ���
    private int maxMateriaCnt_ = 0;         //�z��̍ő�l��ۑ�
    //private int singleMateriaCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��

    [SerializeField]
    private GameObject itemStorePleate;         // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    public static GameObject[] materiaPleate;  // ���������v���n�u��ۑ�
    public static Text[] activeText_;         // �\������f�ނ̖��O
    public static Text[] activePrice_;        // �\������l�i

    //--------------

    // �{�֘A
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

    private BookList bookList_;   // �{��ExcelData�ǂݍ���
    private int maxBookCnt_ = 0;         //�z��̍ő�l��ۑ�
    private int singleBookCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��

    // �{�𔃂�
    [SerializeField]
    private GameObject bookPlatePrefab;         // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    public static GameObject[] bookObj_;  // ���������v���n�u��ۑ�
    public static int[] bookPrice_;   // �l�i
    public static string[] bookInfo_;
                                      //---------------

    private GameObject DataPopPrefab_;

    void Awake()
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        popLists_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        bookList_ = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, 0);

        // �{�֘A
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

        // ������X�֘A
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
        Debug.Log("�^�E���ł̃f�[�^��ǂݍ��ݏI���܂���");
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