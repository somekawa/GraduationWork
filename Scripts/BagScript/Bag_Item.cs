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

    // データ系
    private SaveCSV_HaveItem saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath = @"Assets/Resources/HaveItemList.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;
                                          
    // すべてのアイテム数
    private InitPopList popItemList_;
    private int maxCnt_ = 0;

    public struct ItemData
    {
        public GameObject box;  // 生成しておいたオブジェクトを代入
        public Image image;     // アイテム画像
        public Text cntText;    // 所持数を表示
        public int haveCnt;     // 指定アイテムの所持数
        public string name;     // アイテムの名前
        public string info;
        public bool getFlag;    // 所持しているかどうか
    }
    public static ItemData[] itemState;
    public static ItemData[] data = new ItemData[20];
    private int exItemNum_ ;

    private int clickItemNum_ = -1;
    private Text info_; // クリックしたアイテムを説明する欄
    private Button throwAwayBtn_;

    public void Init()
    // void Start()
    {
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
       
        maxCnt_ = popItemList_.SetMaxItemCount();
        maxCnt_ = popItemList_.SetMaxItemCount();
        exItemNum_ =maxCnt_+1;
        itemState = new ItemData[maxCnt_];
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_HaveItem>();
        for (int i = 0; i < maxCnt_; i++)
        {
            itemState[i] = new ItemData
            {
                box = InitPopList.itemData[i].box,
                name = InitPopList.itemData[i].name,
                info = InitPopList.itemData[i].info,
                haveCnt = 0,
            };
            // 親の位置を変更
            itemState[i].box.transform.SetParent(itemParent.transform);
          
            // 探しやすいように番号をつけておく
            itemState[i].box.name = itemState[i].name + i;

            // 生成したプレハブの子になっているImageを見つける
            itemState[i].image = itemState[i].box.transform.Find("ItemIcon").GetComponent<Image>();
            itemState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][i];
           
            // 生成したプレハブの子になっているTextを見つける
            itemState[i].cntText = itemState[i].box.transform.Find("ItemNum").GetComponent<Text>();
            itemState[i].cntText.text = itemState[i].haveCnt.ToString();
            //itemCnt_[i] = 0;// すべてのアイテムの所持数を0にする

            itemState[i].getFlag = 0 < itemState[i].haveCnt ? false : true;
            itemState[i].box.SetActive(itemState[i].getFlag);
        }

        // デバッグ用 全部の素材を5個取得した状態で始まる
        for (int i = 0; i < maxCnt_; i++)
        {
            ItemGetCheck(i, itemState[i].name, 1, MovePoint.JUDGE.NORMAL);
           // Debug.Log(i + "番目の素材" + itemState[i].name);
        }

    }

    public void ItemGetCheck(int itemNum,string itemName,int createCnt, MovePoint.JUDGE judge)
    {
        bool checkFlag_ = true;
        if(judge==MovePoint.JUDGE.GOOD)
        {
            // 親の位置を変更
            itemState[exItemNum_].box.transform.SetParent(itemParent.transform);

            // 探しやすいように番号をつけておく
            itemState[exItemNum_].box.name = itemState[exItemNum_].name+"Ex" + exItemNum_;

            // 生成したプレハブの子になっているImageを見つける
            itemState[exItemNum_].image = itemState[exItemNum_].box.transform.Find("ItemIcon").GetComponent<Image>();
            itemState[exItemNum_].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.ITEM][itemNum];

            // 生成したプレハブの子になっているTextを見つける
            itemState[exItemNum_].cntText = itemState[exItemNum_].box.transform.Find("ItemNum").GetComponent<Text>();
            itemState[exItemNum_].cntText.text = itemState[exItemNum_].haveCnt.ToString();
            exItemNum_++;

        }



        itemState[itemNum].haveCnt += createCnt;

        itemState[itemNum].getFlag = 0 < itemState[itemNum].haveCnt ? true : false;
        itemState[itemNum].box.SetActive(itemState[itemNum].getFlag);
        //if (0< itemState[itemNum].haveCnt)
        //{
        //    itemState[itemNum].getFlag = true;
        //   // Debug.Log(itemName + "を取得しました");
        //}
        //else
        //{
        //    itemState[itemNum].getFlag = false;
        //    itemState[itemNum].box.SetActive(false);
        //}
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();

        data[itemNum].name = itemName;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
    }

    public void DataLoad()
    {
       // Debug.Log("ロードします");

        csvDatas.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
        }
       // Debug.Log("データ数" + csvDatas.Count);
        Init();
    }

    private void DataSave()
    {
      //  Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
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