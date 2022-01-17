using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Item : MonoBehaviour
{
    [SerializeField]
    private RectTransform itemParent;    // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private GameObject itemUIBox;     // Exアイテム生成系

    // データ系
    private SaveCSV_HaveItem saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/HaveItemList.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;
                                          
    // すべてのアイテム数
    private InitPopList popItemList_;
    private int maxCnt_ = 0;// xlsから読み込むアイテムの数

    public struct ItemData
    {
        public int number;
        public GameObject box;  // 生成しておいたオブジェクトを代入
        public Image image;     // アイテム画像
        public Image EX;// 大成功表示画像
        public Text cntText;    // 所持数を表示
        public string name;     // アイテムの名前
        public string info;
        public bool getFlag;    // 所持しているかどうか
        public int haveCnt;     // 指定アイテムの所持数
    }
    public static ItemData[] itemState;// xlsデータから読み込んだものを保存
    public static ItemData[] data;// csvデータから読み込んだものを保存
    public static bool itemUseFlg = false;  // アイテムを使用したらtrue

    private int clickItemNum_ = -1;
    private Text info_; // クリックしたアイテムを説明する欄
    private Button throwAwayBtn_;
    private Button useBtn_;
    private UseItem useItem_;
    private GameObject infoBack_;
    private GameObject charasText_;

    private bool checkFlag_ = false;

    public void Init()
    // void Start()
    {
        if (checkFlag_ == false)
        {
            popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

            maxCnt_ = popItemList_.SetMaxItemCount();
            itemState = new ItemData[maxCnt_];
            data = new ItemData[maxCnt_];
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_HaveItem>();
            for (int i = 0; i < maxCnt_; i++)
            {
                itemState[i] = new ItemData
                {
                    box = InitPopList.itemData[i].box,
                    info = InitPopList.itemData[i].info,
                };

                itemState[i].number = int.Parse(csvDatas_[i + 1][0]);
                itemState[i].name = csvDatas_[i + 1][1];
                //Debug.Log("アイテムの名前："+itemState[i].name);
                // アイテムの所持数を確認
                itemState[i].haveCnt = int.Parse(csvDatas_[i + 1][2]);
                // 親の位置を変更
                itemState[i].box.transform.SetParent(itemParent.transform);

                // 探しやすいように番号をつけておく
                itemState[i].box.name = itemState[i].name + i;

                // 生成したプレハブの子になっているImageを見つける
                itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
                int num = maxCnt_ / 2 < i ? i - maxCnt_ / 2 : i;
                itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][num];

                // Exアイテムの目印を表示するかしないか
                itemState[i].EX = itemState[i].box.transform.Find("SymbolImage").GetComponent<Image>();
                bool flag = maxCnt_ / 2 < i ? true : false;
                itemState[i].EX.gameObject.SetActive(flag);

                // 生成したプレハブの子になっているTextを見つける
                itemState[i].cntText = itemState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
                itemState[i].cntText.text = itemState[i].haveCnt.ToString();

                itemState[i].getFlag = 0 < itemState[i].haveCnt ? true : false;
                itemState[i].box.SetActive(itemState[i].getFlag);
            }
            checkFlag_ = true;
        }
        else
        {
            // 店かアイテム使用でアイテム個数が変更されたとき
            for (int i = 0; i < maxCnt_; i++)
            {
                itemState[i].cntText.text = itemState[i].haveCnt.ToString();
                itemState[i].getFlag = 0 < itemState[i].haveCnt ? true : false;
                itemState[i].box.SetActive(itemState[i].getFlag);
            }
        }


        //// デバッグ用 全部の素材を5個取得した状態で始まる
        //for (int i = 0; i < exItemNum_; i++)
        //{
        //    ItemGetCheck(i, itemState[i].name, 10, MovePoint.JUDGE.NORMAL);
        //   // Debug.Log(i + "番目の素材" + itemState[i].name);
        //}
        //ItemGetCheck(0, itemState[0].name, 10, MovePoint.JUDGE.GOOD);

    }

    public void ItemGetCheck(MovePoint.JUDGE judge, int itemNum,int createCnt)
    {
        if(judge == MovePoint.JUDGE.GOOD)
        {
            itemNum = itemNum*2;
        }
        Debug.Log("加算されるアイテム"+itemState[itemNum].name);
        itemState[itemNum].haveCnt = itemState[itemNum].haveCnt + createCnt;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
    }

    public void SetItemCntText(int itemNum)
    {
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();
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
        Init();
    }

    public void DataSave()
    {
        saveCsvSc_.SaveStart();

        // アイテムの個数分回す
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveItemData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

    public void SetItemNumber(int num)
    {
        // 捨てるボタンを表示
        if (throwAwayBtn_ == null)
        {
            infoBack_ = GameObject.Find("ItemBagMng/InfoBack").gameObject;
            throwAwayBtn_ = infoBack_.transform.Find("ItemDelete").GetComponent<Button>();
            useBtn_ = infoBack_.transform.Find("ItemUse").GetComponent<Button>();
            info_ = infoBack_.transform.Find("InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);
        useBtn_.gameObject.SetActive(true);

        // 選択されたアイテムの番号を保存
        clickItemNum_ = num;

        StartCoroutine(MoveObj(false));
    }

    public void OnClickThrowAwayButton()
    {
        if(itemParent.gameObject.activeSelf==false)
        {
            //throwAwayBtn_.gameObject.SetActive(false);
            return;
        }

        // 捨てるボタンを押された場合
        itemState[clickItemNum_].haveCnt--;// 所持数を1減らす

        // 表示中の所持数を更新
        itemState[clickItemNum_].cntText.text = itemState[clickItemNum_].haveCnt.ToString();
        if (itemState[clickItemNum_].haveCnt<1)
        {
            // 所持数が0になったら非表示にする
            itemState[clickItemNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            useBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
    }

    public void OnClickUseButton()
    {
        if (useItem_ == null)
        {
            useItem_ = GameObject.Find("ItemBagMng").GetComponent<UseItem>();
        }

        if (infoBack_ == null)
        {
            infoBack_ = GameObject.Find("ItemBagMng/InfoBack").gameObject;
        }

        Text[] text = new Text[2];
        for (int i = 0; i < charasText_.transform.childCount; i++)
        {
            text[i] = charasText_.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }

        // ここで返り値を一時変数にいれないと、Use関数を2回通ることになってバグが生じる
        var tmp = useItem_.Use(clickItemNum_);

        // 今選択中のアイテムを使える
        if (tmp.Item1)
        {
            if (tmp.Item2)
            {
                // 対象の選択が必要
                useItem_.TextInit(text);
                StartCoroutine(MoveObj(true));
            }
            else
            {
                // 対象の選択が必要ない
                // trueで返ってきたときには、使用したアイテムを-1する
                itemState[clickItemNum_].haveCnt--;
                itemUseFlg = true;
            }
        }

        // 表示中の所持数を更新
        itemState[clickItemNum_].cntText.text = itemState[clickItemNum_].haveCnt.ToString();
        if (itemState[clickItemNum_].haveCnt < 1)
        {
            // 所持数が0になったら非表示にする
            itemState[clickItemNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            useBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
    }


    // オブジェクト移動のコルーチン
    private System.Collections.IEnumerator MoveObj(bool outFlg)
    {
        if (charasText_ == null)
        {
            charasText_ = GameObject.Find("ItemBagMng/CharasText").gameObject;
        }

        (int, bool)[] tmpPair = new (int, bool)[2];
        if (outFlg)
        {
            tmpPair[0] = (300, false);
            tmpPair[1] = (-170, false);
        }
        else
        {
            tmpPair[0] = (105, false);
            tmpPair[1] = (0, false);
        }

        bool tmpFlg = false;
        while (!tmpFlg)
        {
            tmpFlg = true;

            if (outFlg)
            {
                // テキストは左に、アイテム説明は右にずらす
                if (!tmpPair[0].Item2)
                {
                    infoBack_.transform.position = new Vector3(infoBack_.transform.position.x + 3, infoBack_.transform.position.y, infoBack_.transform.position.z);
                    if (infoBack_.transform.localPosition.x >= tmpPair[0].Item1)
                    {
                        tmpPair[0].Item2 = true;
                    }
                }
                if (!tmpPair[1].Item2)
                {
                    charasText_.transform.position = new Vector3(charasText_.transform.position.x - 3, charasText_.transform.position.y, charasText_.transform.position.z);
                    if (charasText_.transform.localPosition.x <= tmpPair[1].Item1)
                    {
                        tmpPair[1].Item2 = true;
                    }
                }
            }
            else
            {
                // テキストは右に、アイテム説明は左に戻す
                if (!tmpPair[0].Item2)
                {
                    infoBack_.transform.position = new Vector3(infoBack_.transform.position.x - 3, infoBack_.transform.position.y, infoBack_.transform.position.z);
                    if (infoBack_.transform.localPosition.x <= tmpPair[0].Item1)
                    {
                        tmpPair[0].Item2 = true;
                    }
                }
                if (!tmpPair[1].Item2)
                {
                    charasText_.transform.position = new Vector3(charasText_.transform.position.x + 3, charasText_.transform.position.y, charasText_.transform.position.z);
                    if (charasText_.transform.localPosition.x >= tmpPair[1].Item1)
                    {
                        tmpPair[1].Item2 = true;
                    }
                }
            }

            for (int i = 0; i < tmpPair.Length; i++)
            {
                tmpFlg &= tmpPair[i].Item2;
            }

            yield return null;
        }
    }

}