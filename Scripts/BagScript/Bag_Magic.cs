using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Magic : MonoBehaviour
{
    // データ系
    private SaveCSV_Magic saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/magicData.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    public enum ELEMENT_KIND
    {
        NON=-1,
        FIRE, // 0 炎
        HEAL,   // 1 回復
        WATER, // 2 水
        ASSIST,  // 3 補助
        EARTH, // 4 土
        WIND, // 5 風
        DRAGON, // 6 龍
        MAX
    }
    public static string[] elementString = new string[(int)ELEMENT_KIND.MAX] {
    "炎","回復","水","補助","土","風","龍"};

    public struct MagicData
    {
        public string name;
        public int power;
        public int rate;
        public int ability;
        public int element;
        public bool setNumber;
    }
    public static MagicData[] data = new MagicData[50];

    // バッグ表示用
    [SerializeField]
    private GameObject bagMagicUI;     // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private RectTransform bagMagicParent;// アイテムボックスから確認するときの親

    public static GameObject[] magicObject = new GameObject[50];
    private Image[] magicImage_ = new Image[50];
    
    // ステータス表示用
    [SerializeField]
    private GameObject statusMagicUI;     // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private RectTransform statusMagicParent;// ステータス画面を開いた時の親
   
    public static GameObject[] statusMagicObject = new GameObject[50];
    private Image[] statusMagicImage_ = new Image[50];

    // 共通
    public static int number_ = 0;
    private int minElementNum_ = 34;
    public static int[] elementNum_ = new int[50];

    public void Init()
    {
        if (magicObject[0] == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV_Magic>();
            if (0 < number_)
            {
               // Debug.Log(number_);
                for (int i = 0; i < number_; i++)
                {
                    data[i].name = csvDatas[i + 1][0];
                    data[i].power = int.Parse(csvDatas[i + 1][1]);
                    data[i].ability = int.Parse(csvDatas[i + 1][2]);
                    data[i].rate = int.Parse(csvDatas[i + 1][3]);
                    data[i].element = int.Parse(csvDatas[i + 1][4]);
                    data[i].setNumber = bool.Parse(csvDatas[i + 1][5]);

                    elementNum_[i] = minElementNum_ + data[i].element;
                    Debug.Log(elementNum_[i] + "            残り" + data[i].power);
                    // バッグ用
                    magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, bagMagicParent.transform);
                    magicObject[i].name = "Magic_" + i;
                    magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    magicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[i]];
                   
                    // ステータス用の座標に変更
                    statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                    Quaternion.identity, statusMagicParent.transform);
                    statusMagicObject[i].name = "Magic_" + i;
                    statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    statusMagicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[i]];
                }
            }
        }
    }

    public void MagicCreateCheck(string magic, int pow, int rateNum, int abilityNum,int element)
    {
        data[number_].name = magic;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].ability = abilityNum;
        data[number_].element = element;
        data[number_].setNumber = false;// 作られただけのため
        DataSave();

        elementNum_[number_] = minElementNum_ + data[number_].element;
        
        // バッグ用
        magicObject[number_] = Instantiate(bagMagicUI,new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
        magicObject[number_].name = "Magic_" + number_;
        magicImage_[number_] = magicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[number_]];
        //Debug.Log(data[number_].name + "  " + data[number_].power + "  " + data[number_].rate + "  " + data[number_].ability);
        
        // ステータス用の座標に変更
        statusMagicObject[number_] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                        Quaternion.identity, statusMagicParent.transform);
        statusMagicObject[number_].name = "Magic_" + number_;
        statusMagicImage_[number_] = statusMagicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][elementNum_[number_]];
       
        number_++;
    }

    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
     //   Debug.Log("ロードします");

        csvDatas.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
        }
       // Debug.Log("データ数" + csvDatas.Count);
        number_ = csvDatas.Count - 1;

        // 魔法の個数分回す
        for (int i = 1; i < number_; i++)
        {
            MagicData set = new MagicData
            {
                name = csvDatas[i + 1][0],
                power = int.Parse(csvDatas[i + 1][1]),
                ability = int.Parse(csvDatas[i + 1][2]),
                rate = int.Parse(csvDatas[i + 1][3]),
                element = int.Parse(csvDatas[i + 1][4]),
                setNumber = bool.Parse(csvDatas[i + 1][5]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        //  Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i]);
         //   Debug.Log(data[i].name);
        }
        saveCsvSc_.SaveEnd();
    }

    public void SetMagicCheck(int num,bool flag)
    {
        data[num].setNumber = flag;
        DataSave();
    }

}