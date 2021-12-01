using System.Text.RegularExpressions;
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
    public static ItemData[] itemState;
    public static ItemData[] data;
    private int exItemNum_ ;// 大成功を含めたアイテムの数

    //public struct LoadItemData
    //{
    //    public string name;
    //    public int cnt;
    //}

    private int clickItemNum_ = -1;
    private Text info_; // クリックしたアイテムを説明する欄
    private Button throwAwayBtn_;

    public void Init()
    // void Start()
    {
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
       
        maxCnt_ = popItemList_.SetMaxItemCount();
        Debug.Log("csvアイテム数" + exItemNum_+"         xlsアイテム数"+maxCnt_);
        //exItemNum_ =maxCnt_;
        itemState = new ItemData[exItemNum_];
        data = new ItemData[exItemNum_];
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_HaveItem>();
        for (int i = 0; i < exItemNum_; i++)
        {
            if (maxCnt_-1<i)
            {
                // 大成功アイテムの生成
                itemState[i].box = Instantiate(itemUIBox,
                         new Vector2(0, 0), Quaternion.identity, transform);
                //Debug.Log(i+":大成功アイテムの生成");
            }
            else
            {
                itemState[i] = new ItemData
                {
                    box = InitPopList.itemData[i].box,
                    info = InitPopList.itemData[i].info,
                };
            }

            itemState[i].number = int.Parse(csvDatas_[i + 1][0]);
            itemState[i].name = csvDatas_[i + 1][1];
            itemState[i].haveCnt = int.Parse(csvDatas_[i + 1][2]);
             // 親の位置を変更
            itemState[i].box.transform.SetParent(itemParent.transform);
          
            // 探しやすいように番号をつけておく
            itemState[i].box.name = itemState[i].name + i;

            // 生成したプレハブの子になっているImageを見つける
            itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
            int num= maxCnt_ - 1 < i ? i-maxCnt_ : i;
            itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][num];

            // Exアイテムの目印を表示するかしないか
            itemState[i].EX = itemState[i].box.transform.Find("SymbolImage").GetComponent<Image>();
            bool flag = maxCnt_-1 < i ? true : false;
            itemState[i].EX.gameObject.SetActive(flag);

            // 生成したプレハブの子になっているTextを見つける
            itemState[i].cntText = itemState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
            itemState[i].cntText.text = itemState[i].haveCnt.ToString();
            //itemCnt_[i] = 0;// すべてのアイテムの所持数を0にする

            itemState[i].getFlag = 0 < itemState[i].haveCnt ?   true: false;
            itemState[i].box.SetActive(itemState[i].getFlag);
        }



        //// デバッグ用 全部の素材を5個取得した状態で始まる
        //for (int i = 0; i < exItemNum_; i++)
        //{
        //    ItemGetCheck(i, itemState[i].name, 10, MovePoint.JUDGE.NORMAL);
        //   // Debug.Log(i + "番目の素材" + itemState[i].name);
        //}
        //ItemGetCheck(0, itemState[0].name, 10, MovePoint.JUDGE.GOOD);

    }

    public void ItemGetCheck(int itemNum,string itemName,int createCnt, MovePoint.JUDGE judge)
    {
        //if(judge==MovePoint.JUDGE.GOOD)
        //{
        //    Debug.Log("生成したアイテム番号"+exItemNum_);
        //    itemNum = exItemNum_;
        //  //  itemName = itemName + "Ex";
        //    //itemState[exItemNum_].box = Instantiate(itemUIBox,
        //    //            new Vector2(0, 0), Quaternion.identity, transform);
        //    //// 親の位置を変更
        //    //itemState[exItemNum_].box.transform.SetParent(itemParent.transform);

        //    //// 探しやすいように番号をつけておく
        //    //itemState[exItemNum_].box.name = itemName + exItemNum_;

        //    //// 生成したプレハブの子になっているImageを見つける
        //    //itemState[exItemNum_].image = itemState[exItemNum_].box.transform.Find("ItemIcon").GetComponent<Image>();
        //    //itemState[exItemNum_].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][exItemNum_ - maxCnt_];
        //    //itemState[exItemNum_].EX = itemState[exItemNum_].box.transform.Find("SymbolImage").GetComponent<Image>();
        //    //itemState[exItemNum_].EX.gameObject.SetActive(true);

        //    //// 生成したプレハブの子になっているTextを見つける
        //    //itemState[exItemNum_].cntText = itemState[exItemNum_].box.transform.Find("NumBack/Num").GetComponent<Text>();
        //    //itemState[exItemNum_].cntText.text = itemState[exItemNum_].haveCnt.ToString();
        //    //exItemNum_++;
        //}

        itemState[itemNum].haveCnt += createCnt;

        itemState[itemNum].getFlag = 0 < itemState[itemNum].haveCnt ? true : false;
        itemState[itemNum].box.SetActive(itemState[itemNum].getFlag);
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();

        //data[itemNum].name = itemName;
        //data[itemNum].number = itemNum;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
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
        exItemNum_ = csvDatas_.Count - 1;
        Init();
    }

    private void DataSave()
    {
      //  Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < exItemNum_; i++)
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
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/ItemDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);

        // 選択されたアイテムの番号を保存
        clickItemNum_ = num;
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
            info_.text = "";
        }
    }

}