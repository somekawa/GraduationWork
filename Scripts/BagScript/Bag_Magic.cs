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

    public enum HEAD_KIND
    {
        NON = -1,
        SINGLE, // 0 単体
        MULTIPLE,// 1 複数回
        OVERALL,// 2 全体
        MAX
    }
    public static string[] headString = new string[(int)HEAD_KIND.MAX] {
    "単体","複数回","全体"};

    public enum ELEMENT_KIND
    {
        NON=-1,
        FIRE, // 0 炎
        HEAL,   // 1 回復
        WATER, // 2 水
        ASSIST,  // 3 補助
        EARTH, // 4 土
        WIND, // 5 風
        MAX
    }
    public static string[] elementString = new string[(int)ELEMENT_KIND.MAX] {
    "炎","回復","水","補助","土","風"};

    public enum TAIL_KIND
    {
        NON = -1,
        SMALL, // 0 小
        MEDIUM,// 1 中
        LARGE,// 2 大
        MAXMUM,// 3 極大
        MAX
    }
    public static string[] tailString = new string[(int)TAIL_KIND.MAX] {
    "小","中","大","極大"};
    public enum SUB1_KIND
    {
        NON = -1,
        FELLOW,//味方
        ENEMY,//敵
        POISON,//毒
        DARK,//暗闇
        PARALYSIS,// 麻痺
        DEATH,//即死
        SUCTION,//吸収
        TARGET,//必中
        MAX
    }
    public static string[] sub1String_ = new string[(int)SUB1_KIND.MAX] {
    "味方","敵","毒","暗闇","麻痺","即死","吸収","必中"};

    public enum SUB2_KIND
    {
        NON = -1,
        HP,//HP
        MAGIC_ATTACK,// 魔法攻撃
        PHYSICS_ATTACK,//物理攻撃
        DEFENCE,//防御力
        SPEED,//命中/回避
        POISON,//毒
        DARK,//暗闇
        PARALYSIS,// 麻痺
        DEATH,//即死
        SUCTION,//吸収
        TARGET,//必中
        MAX
    }
    public static string[] sub2String_ = new string[(int)SUB2_KIND.MAX] {
    "HP","魔法攻撃","物理攻撃","防御力","命中/回避","毒","暗闇","麻痺","即死","吸収","必中"};

    public enum SUB3_KIND
    {
        NON = -1,
        UP,//上昇、
        DOWN,//低下、
        REFLECTION,//反射
        SUCTION,//吸収
        TARGET,//必中
        MAX
    }
    public static string[] sub3String_ = new string[(int)SUB3_KIND.MAX] {
    "上昇","低下","反射","吸収","必中"};



    public struct MagicData
    {
        public string name;
        public int power;
        public int rate;
        public int head;
        public int element;
        public int tail;
        public int sub1;
        public int sub2;
        public int sub3;
      //  public Sprite sprite;
       // public int imageNum;
    }
    public static MagicData[] data = new MagicData[50];
    public static Sprite[] magicSpite = new Sprite[50];

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
    //private int minElementNum_ = 34;
  //  public static int[] elementNum_ = new int[50];

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
                    data[i].rate = int.Parse(csvDatas[i + 1][2]);
                    data[i].head = int.Parse(csvDatas[i + 1][3]);
                    data[i].element = int.Parse(csvDatas[i + 1][4]);
                    data[i].tail = int.Parse(csvDatas[i + 1][5]);
                    data[i].sub1 = int.Parse(csvDatas[i + 1][6]);
                    data[i].sub2 = int.Parse(csvDatas[i + 1][7]);
                    data[i].sub3 = int.Parse(csvDatas[i + 1][8]);

                    magicSpite[i]= ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                    Debug.Log(data[i].element + "            残り" + data[i].power);
                    // data[i].battleSet0 = bool.Parse(csvDatas[i + 1][5]);

                    // elementNum_[i] = minElementNum_ + data[i].element;
                    // バッグ用
                    magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, bagMagicParent.transform);
                    magicObject[i].name = "Magic_" + i;
                    magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    magicImage_[i].sprite = magicSpite[i];
                   
                    // ステータス用の座標に変更
                    statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                    Quaternion.identity, statusMagicParent.transform);
                    statusMagicObject[i].name = "Magic_" + i;
                    statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                    statusMagicImage_[i].sprite = magicSpite[i];
                }
            }
        }
    }

    public void MagicCreateCheck(string magicName, int pow, int rateNum, 
                                  int head, int element, int tail, int sub1, int sub2,int sub3)
    {
        data[number_].name = magicName;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].head = head;
        data[number_].element = element;
        data[number_].tail = tail;
        data[number_].sub1 = sub1;
        data[number_].sub2 = sub2;
        data[number_].sub3 = sub3;
        //data[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];
        DataSave();
        
        // バッグ用
        magicObject[number_] = Instantiate(bagMagicUI,new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
        magicObject[number_].name = "Magic_" + number_;
        magicImage_[number_] = magicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[number_].sprite = magicSpite[data[number_].element];
        //Debug.Log(data[number_].name + "  " + data[number_].power + "  " + data[number_].rate + "  " + data[number_].ability);
        
        // ステータス用の座標に変更
        statusMagicObject[number_] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                        Quaternion.identity, statusMagicParent.transform);
        statusMagicObject[number_].name = "Magic_" + number_;
        statusMagicImage_[number_] = statusMagicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[number_].sprite = magicSpite[data[number_].element];
       
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
        Debug.Log("データ数" + csvDatas.Count);
        number_ = csvDatas.Count- 1;

        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            MagicData set = new MagicData
            {
                name = csvDatas[i + 1][0],
                power = int.Parse(csvDatas[i + 1][1]),
                rate = int.Parse(csvDatas[i + 1][2]),
                head = int.Parse(csvDatas[i + 1][3]),
                element = int.Parse(csvDatas[i + 1][4]),
                tail = int.Parse(csvDatas[i + 1][5]),
                sub1 = int.Parse(csvDatas[i + 1][6]),
                sub2 = int.Parse(csvDatas[i + 1][7]),
                sub3 = int.Parse(csvDatas[i + 1][8]),
               // sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][int.Parse(csvDatas[i + 1][9])],
           // imageNum = int.Parse(csvDatas[i + 1][9]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i]);
         //   Debug.Log(data[i].name);
        }
        saveCsvSc_.SaveEnd();
    }
}