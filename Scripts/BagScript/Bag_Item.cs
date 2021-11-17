using System.Collections;
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
        public bool getFlag;    // 所持しているかどうか
    }
    public static ItemData[] itemState;
    public static ItemData[] data = new ItemData[50];


    public void Init()
    // void Start()
    {
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
       
        maxCnt_ = popItemList_.SetMaxItemCount();

        itemState = new ItemData[maxCnt_];
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_HaveItem>();
        for (int i = 0; i < maxCnt_; i++)
        {
            itemState[i] = new ItemData
            {
                box = InitPopList.itemData[i].box,
                name = InitPopList.itemData[i].name,
                haveCnt = 0,
            };
            //Debug.Log(itemState[i].name);
            //itemBox_[i] = PopMateriaList.itemBox_[i];
            itemState[i].box.transform.SetParent(itemParent.transform);
            itemState[i].box.name = itemState[i].name;

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
            ItemGetCheck(i, itemState[i].name, 1);
           // Debug.Log(i + "番目の素材" + itemState[i].name);
        }

    }

    public void ItemGetCheck(int itemNum,string itemName,int createCnt)
    {
        itemState[itemNum].haveCnt += createCnt;
        if(0< itemState[itemNum].haveCnt)
        {
            itemState[itemNum].getFlag = true;
            itemState[itemNum].box.SetActive(true);
           // Debug.Log(itemName + "を取得しました");
        }
        else
        {
            itemState[itemNum].getFlag = false;
            itemState[itemNum].box.SetActive(false);
        }
        itemState[itemNum].cntText.text = itemState[itemNum].haveCnt.ToString();


        data[itemNum].name = itemName;
        data[itemNum].haveCnt = itemState[itemNum].haveCnt;
        DataSave();
    }

    public void DataLoad()
    {
        Debug.Log("ロードします");

        csvDatas.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
        }
        Debug.Log("データ数" + csvDatas.Count);


        // アイテム個数分回す
        for (int i = 1; i <= maxCnt_; i++)
        {
            ItemData set = new ItemData
            {
                name = csvDatas[i + 1][0],// name自体はこのScriptをアタッチ指定オブジェクト名が入ってる
                haveCnt = int.Parse(csvDatas[i + 1][1]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveItemData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

}