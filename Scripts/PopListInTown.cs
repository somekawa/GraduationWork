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
    private int maxCnt_ = 0;         //�z��̍ő�l��ۑ�
    //private int singleMateriaCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��

    [SerializeField]
    private GameObject itemStorePleate;         // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    public static GameObject[] materiaPleate;  // ���������v���n�u��ۑ�
    public static GameObject[] itemPleate;  // ���������v���n�u��ۑ�
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

    private BookList[] bookList_=new BookList[(int)BOOK.MAX];   // �{��ExcelData�ǂݍ���
    private int maxBookCnt_ = 0;         //�z��̍ő�l��ۑ�
    private int singleBookCnt_ = 0;         // �V�[�g�ɋL�ڂ���Ă��

    // �{�𔃂�
    [SerializeField]
    private GameObject bookPlatePrefab;         // �f�ނ��E�����Ƃ��ɐ��������v���n�u
    public static GameObject[] bookObj;  // ���������v���n�u��ۑ�
    public static int[] bookImageNum;
    public static int[] bookWordNum;
    public static int[] bookPrice;   // �l�i
    public static string[] statusUp;
    public static string[] bookInfo;

    //---------------

    private GameObject DataPopPrefab_;

    void Awake()
    {
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        // StreamingAssets����AssetBundle�����[�h����
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle�J��");
        // AssetBundle���̃A�Z�b�g�ɂ̓r���h���̃A�Z�b�g�̃p�X�A�܂��̓t�@�C�����A�t�@�C�����{�g���q�ŃA�N�Z�X�ł���
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // �s�v�ɂȂ���AssetBundle�̃��^�����A�����[�h����
        assetBundle.Unload(false);
        Debug.Log("�j��");

        for (int b = 0; b < (int)BOOK.MAX; b++)
        {
            bookList_[b] = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, b);
            maxBookCnt_ += bookList_[b].param.Count;
        }

        Debug.Log("�{�̌�" + maxBookCnt_);
        // �{�֘A
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
               //Debug.Log(number+"           1�y�[�W�̖{�̌�" + bookObj[number]);
                number++;
            }
        }

        // ������X�֘A
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