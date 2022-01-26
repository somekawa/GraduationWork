using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookStoreMng : MonoBehaviour
{
    // データ系
    private SaveCSV_Book saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/Save/bookData.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;

    private PopListInTown popBookList_;
    private int maxCnt_ = 0;            // すべての素材数
    private int singleCnt_ = 0;         // 1つのシートに記載されてる最大個数

    [SerializeField]
    private GameObject bookStoreUI;    // 本屋用のUI
    private GameObject bookStoreCanvas_;      // 本屋に入るか、外に出るかのキャンバス
    [SerializeField]
    private RectTransform bookStoreParent;  // 表示位置の親

    private RectTransform bookInfoData_;
    private TMPro.TextMeshProUGUI bookNameText_;// 選択した際に表示する名前


    public struct BookData
    {
        public string name;     // 本の名前
        public int readFlag;    // 0がfalseで読んでない。1がtrueで読んだ
        // 生成しておいたオブジェクト全体
        public GameObject obj;  // 生成しておいたプレハブを代入
        // ボタンとボタン内で表示する関連
        public Button btn;      // オブジェクトの子のボタン
        public TMPro.TextMeshProUGUI nameText;   // 表示する名前
        public TMPro.TextMeshProUGUI priceText;  // 表示する料金
        // チェックボックス関連
        public Image checkBox;
        public Image checkMark;
        public int wordDataNum;// どの種類のワードか
        public string statusMng;
        public int number;// ステータスの上昇値、ワードの指定種類に対する番号
        public string kinds;// どの種類のステータスか
        public string info;// 選択した本の中身（ワードとかステータスとか
        public int price;       // 本の値段を保存
        public int imageNum;// 画像番号
    }
    public static BookData[] bookState_;

    private Image soldOutImage_;// すべて買われた場合の画像
    private Button buyBtn_;// 購入するためのボタン
    private TMPro.TextMeshProUGUI infoText_;
    private Image infoImage_;// 選択したアイテムの画像

    // 合計料金
    private TMPro.TextMeshProUGUI totalPriceText_;// 合計料金の表示
    private int totalPrice_ = 0;// 選択している本の合計金額

    // 所持金
    private TMPro.TextMeshProUGUI haveMoneyText_;
    private int haveMoney_ = 10001;// デバッグ用

    private Color badCheck_ = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color chengeCheck_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private TMPro.TextMeshProUGUI statusOrWordText_;
    private TMPro.TextMeshProUGUI getStatusOrWordText_;

    private Bag_Word bagWord_;

    private int selectCnt_ = 0;// 何冊の本を選んでいるか
    // ステータス順に並べた文字列
    private string[] statusUpStr_ = {
        "Attack", "MagicAttack", "Defence", "Speed", "Luck", "HP", "MP", "Exp" };
    // アップステータスの名前とアップ値をペアにした変数 どれを選択したかを保存する
    private (string, int)[] statusUp_ = new (string, int)[8];

    private int[] boolActiveTiming_ = new int[5] { 5, 10, 16,22,28 };
    void Start()
    {
        // 初期化
        for (int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }


        bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();
        saveCsvSc_ = GameObject.Find("TownMng").GetComponent<SaveCSV_Book>();

        bookStoreCanvas_ = GameObject.Find("BookStoreCanvas");
        bookInfoData_ = bookStoreUI.transform.Find("CheckArea/BookData").GetComponent<RectTransform>();
        buyBtn_ = bookInfoData_.Find("BuyButton").GetComponent<Button>();
        infoImage_ = bookInfoData_.Find("InfoArea/Back/Image").GetComponent<Image>();
        bookNameText_ = bookInfoData_.Find("InfoArea/Back/NameText").GetComponent<TMPro.TextMeshProUGUI>();
        infoText_ = bookInfoData_.Find("InfoArea/Text").GetComponent<TMPro.TextMeshProUGUI>();
        infoImage_.sprite = null;// ItemImageMng.spriteMap[ItemImageMng.IMAGE.BOOK][0];
        bookNameText_.text = "";
        infoText_.text = "本を選択してください";

        statusOrWordText_ = bookInfoData_.Find("StatusOrWordText").GetComponent<TMPro.TextMeshProUGUI>();
        getStatusOrWordText_ = statusOrWordText_.transform.Find("GetStatusOrWordText").GetComponent<TMPro.TextMeshProUGUI>();
        statusOrWordText_.text = "";
        getStatusOrWordText_.text = "";

        // 合計料金
        totalPriceText_ = bookInfoData_.Find("TotalPriceBack/TotalPrice").GetComponent<TMPro.TextMeshProUGUI>();
        totalPriceText_.text = "0";

        soldOutImage_ = bookStoreUI.transform.Find("ScrollView/Viewport/SoldOut").GetComponent<Image>();
        soldOutImage_.gameObject.SetActive(false);

       // haveMoney_ = SceneMng.GetHaveMoney();
        haveMoneyText_ = bookStoreUI.transform.Find("CheckArea/Money/Count").GetComponent<TMPro.TextMeshProUGUI>();
        haveMoneyText_.text = haveMoney_.ToString();

        popBookList_ = GameObject.Find("TownMng").GetComponent<PopListInTown>();

        singleCnt_ = popBookList_.SetSingleBookCount();
       // maxCnt_ = popBookList_.SetMaxBookCount();

     //   bookState_ = new BookData[maxCnt_];
        DataLoad();

        if (19 < EventMng.GetChapterNum())
        {
            maxCnt_ = boolActiveTiming_[4];
        }
        else if (16 < EventMng.GetChapterNum())
        {
            maxCnt_ = boolActiveTiming_[3];
        }
        else if (13 < EventMng.GetChapterNum())
        {
            maxCnt_ = boolActiveTiming_[2];
        }
        else if (8 < EventMng.GetChapterNum())
        {
            maxCnt_ = boolActiveTiming_[1];
        }
        else if (0 < EventMng.GetChapterNum())
        {
            maxCnt_ = boolActiveTiming_[0];
        }
        bookState_ = new BookData[maxCnt_];


        Debug.Log("イベント進行度チェック："+EventMng.GetChapterNum());
        for (int i = 0; i < maxCnt_; i++)
        {
            Debug.Log(i + "番目");
            bookState_[i] = new BookData
            {
                name = csvDatas_[i + 1][0],
            obj = PopListInTown.bookObj[i],
                wordDataNum = PopListInTown.bookWordNum[i],
                statusMng = PopListInTown.statusUp[i],
                price = PopListInTown.bookPrice[i],
                info = PopListInTown.bookInfo[i],
                imageNum = PopListInTown.bookImageNum[i],
            };
            if (EventMng.GetChapterNum()<1)
            {
                bookState_[i].readFlag = 0;
            }
            else
            {
                bookState_[i].readFlag = int.Parse(csvDatas_[i + 1][1]);
            }

            var underbarSplit = bookState_[i].statusMng.Split('_');
            //アンダーバーで区切ったものを、別々のリストへ入れる
            bookState_[i].kinds = underbarSplit[0];
            ////  Debug.Log("上昇ステータス" + bookState_[i].statusKinds);
            //bookState_[i].kindsNum = int.Parse(underbarSplit[0]);
            bookState_[i].number = int.Parse(underbarSplit[1]);

            // 事前に生成しておいたオブジェクトを代入する
            bookState_[i].btn = bookState_[i].obj.transform.Find("BookButton").GetComponent<Button>();
            bookState_[i].btn.name = bookState_[i].name + i;// ボタンの名前を変えないとクリック時に本を探せない
            bookState_[i].obj.name = bookState_[i].name + i;
            // 表示する名前
            bookState_[i].nameText = bookState_[i].btn.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>();
            bookState_[i].nameText.text = bookState_[i].name;
            // 表示する料金
            bookState_[i].priceText = bookState_[i].btn.transform.Find("Price").GetComponent<TMPro.TextMeshProUGUI>();
            bookState_[i].priceText.text = bookState_[i].price.ToString();

            // チェックボックス
            bookState_[i].checkBox = bookState_[i].obj.transform.Find("CheckBox").GetComponent<Image>();
            bookState_[i].checkMark = bookState_[i].checkBox.transform.Find("CheckMark").GetComponent<Image>();
            bookState_[i].checkMark.gameObject.SetActive(false);

            // 読んだことがない本だけを表示
            bool flag = bookState_[i].readFlag == 0 ? true : false;
            bookState_[i].obj.gameObject.SetActive(flag);
            bookStoreUI.SetActive(false);
        }

        //MoneyCheck();
        //// 説明エリアを非表示にする
        //bookInfoData_.gameObject.SetActive(false);
    }


    public void OnClickBookCheckButton()
    {
        Debug.Log("本を見るボタンを押下しました");
        bookStoreCanvas_.SetActive(false);
        bookStoreUI.SetActive(true);
        for(int i=0;i<maxCnt_;i++)
        {
            // 親が非アクティブの時に親を変えるとスケールが0になるときがあるため
            // アクティブにした後に親を変える
            bookState_[i].obj.transform.SetParent(bookStoreParent.transform);
        }
    }

    public void OnClickBackButton()
    {
        Debug.Log("戻るボタンを押下しました");
        bookStoreCanvas_.SetActive(true);
        bookStoreUI.SetActive(false);
        // 初期化
        for (int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }
    }

    public void SelectBookCheck(int num)
    {
        if (bookInfoData_.gameObject.activeSelf == false)
        {
            bookInfoData_.gameObject.SetActive(true);
        }

        //if (haveMoney_ < totalPrice_ + bookState_[num].price)
        //{
        //    Debug.Log("所持金が足りません");
        //    return;
        //}



        // 選択してないなら本を選択したらactiveをtrue 選択済みならactiveをfalse
        bool flag = bookState_[num].checkMark.gameObject.activeSelf == false ? true : false;
        bookState_[num].checkMark.gameObject.SetActive(flag);
        // selectBook_ = num;


        if (bookState_[num].checkMark.gameObject.activeSelf == true)
        {
            // Toggleがtrueになった時だけ加算
            totalPrice_ += bookState_[num].price;
            StatusUpCheck(num, true);
        }
        else
        {
            // 今回選択した本のToggleがfalseになったら全体料金から引く
            totalPrice_ -= bookState_[num].price;
            StatusUpCheck(num, false);
        }
        totalPriceText_.text = totalPrice_.ToString();
        MoneyCheck();

        // 本の画像
        infoImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BOOK][bookState_[num].imageNum];
        bookNameText_.text = bookState_[num].name;
        // 本の説明一覧
        infoText_.text = bookState_[num].info;
        // ワードが取得できるときはGetWord　ステータスアップの場合はStatusUp
        statusOrWordText_.text = bookState_[num].wordDataNum != -1 ? "GetWord：" : "StatusUp：";
        if (statusOrWordText_.text == "StatusUp：")
        {
            getStatusOrWordText_.text = bookState_[num].kinds + "+" + bookState_[num].number;
        }
        else
        {
            int wordKinds = int.Parse(bookState_[num].kinds);
            getStatusOrWordText_.text = "サブワード『" +
                Bag_Word.wordState[(InitPopList.WORD)wordKinds][bookState_[num].number].name + "』";
            Debug.Log(Bag_Word.wordState[(InitPopList.WORD)wordKinds][bookState_[num].number].name+"を選択してます");
        }

        buyBtn_.interactable = true;

        selectCnt_++;
    }

    private void MoneyCheck()
    {
        int maxSelectPrice = haveMoney_ - totalPrice_;// SceneMng.GetHaveMoney() - totalPrice_;
        for (int i = 0; i < maxCnt_; i++)
        {
            //Debug.Log(bookState_[i].name);
            if (bookState_[i].checkMark.gameObject.activeSelf == true ||
                bookState_[i].obj.activeSelf == false)
            {
                // Toggleがtrue状態ならボタンの状態を変えない
                // 非表示のオブジェクトはそのままでよい
                continue;
            }
            bookState_[i].btn.interactable = maxSelectPrice < bookState_[i].price ? false : true;
            bookState_[i].checkBox.color = maxSelectPrice < bookState_[i].price ? badCheck_ : chengeCheck_;
        }
    }

    private void StatusUpCheck(int bookNum, bool flag)
    {
        if (bookState_[bookNum].wordDataNum != -1)
        {
            // ワード取得系の本を選択していたら処理をやめる
            return;
        }

        // ステータスアップチェック
        for (int i = 0; i < statusUpStr_.Length; i++)
        {
            // 取得しておいたステータスの種類と一致しているか確認する
            if (statusUpStr_[i] == bookState_[bookNum].kinds)
            {
                Debug.Log(i + "番目に" + bookState_[bookNum].kinds + "_" + bookState_[bookNum].number);
                if (flag)
                {
                    //statusUp_[i].Item1 = bookState_[num].statusKinds;
                    statusUp_[i].Item2 += bookState_[bookNum].number;
                }
                else
                {
                    statusUp_[i].Item2 -= bookState_[bookNum].number;
                }
                break;
            }
        }
    }

    public void OnClickBuyBtn()
    {
        // お金の減少処理
        SceneMng.SetHaveMoney(haveMoney_ - totalPrice_);// SceneMng.GetHaveMoney() - totalPrice_);
        //haveMoney_ -= totalPrice_;
        haveMoneyText_.text = SceneMng.GetHaveMoney().ToString();

        int cnt = 0;
        bool statusUpFlag = false;
        for (int i = 0; i < maxCnt_; i++)
        {
            if (bookState_[i].checkMark.gameObject.activeSelf == true &&
                bookState_[i].btn.gameObject.activeSelf == true)
            {
                // 購入した本を非表示にする
                bookState_[i].obj.gameObject.SetActive(false);
                // 読んだ状態
                bookState_[i].readFlag = 1;
                if (bookState_[i].readFlag == 1)
                {
                    if (bookState_[i].wordDataNum != -1)
                    {
                        int wordKinds = int.Parse(bookState_[i].kinds);
                        // ワードを取得する
                        bagWord_.WordGetCheck((InitPopList.WORD)wordKinds,
                                                bookState_[i].number,
                                                bookState_[i].wordDataNum);

                    }
                    else
                    {
                        // ステータス上昇関数を呼び出せるようにしておく
                        statusUpFlag = true;
                    }
                    cnt++;
                }
            }

            if (cnt == selectCnt_)
            {
                // 買った本と同じ個数になったらfor文をぬける
                break;
            }
        }

        if (statusUpFlag == true)
        {
            int[] tmp = { statusUp_[0].Item2, statusUp_[1].Item2, statusUp_[2].Item2, statusUp_[3].Item2,
                        statusUp_[4].Item2, statusUp_[5].Item2, statusUp_[6].Item2, statusUp_[7].Item2 };
            SceneMng.charasList_[(int)SceneMng.CHARACTERNUM.UNI].SetStatusUpByCook(tmp, false);
            // 初期化
            for (int i = 0; i < statusUp_.Length; i++)
            {
                statusUp_[i] = (statusUpStr_[i], 0);
            }
        }

        totalPrice_ = 0;
        selectCnt_ = 0;
        bookInfoData_.gameObject.SetActive(false);
        // DataSave();
    }

    public void DataLoad()
    {
        // Debug.Log("ロードします");

        csvDatas_.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas_.Add(texts[i].Split(','));
        }
    }

    private void DataSave()
    {
        Debug.Log("本を読みました");
        saveCsvSc_.SaveStart();
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveBookData(bookState_[i]);
        }
        saveCsvSc_.SaveEnd();
    }
}