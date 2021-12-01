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
    string[] texts;

    //public enum HEAD_KIND
    //{
    //    NON = -1,
    //    SINGLE, // 0 単体
    //    MULTIPLE,// 1 複数回
    //    OVERALL,// 2 全体
    //    MAX
    //}
    //public static string[] headString = new string[(int)HEAD_KIND.MAX] {
    //"単体","複数回","全体"};

    public enum ELEMENT_KIND
    {
        NON = -1,
        HEAL,   // 0 回復
        ASSIST,  // 1 補助
        FIRE, // 2 炎
        WATER, // 3 水
        EARTH, // 4 土
        WIND, // 5 風
        MAX
    }
    public static string[] elementString = new string[(int)ELEMENT_KIND.MAX] {
    "回復","補助","炎","水","土","風"};

    //public enum TAIL_KIND
    //{
    //    NON = -1,
    //    SMALL, // 0 小
    //    MEDIUM,// 1 中
    //    LARGE,// 2 大
    //    MAXMUM,// 3 極大
    //    MAX
    //}
    //public static string[] tailString = new string[(int)TAIL_KIND.MAX] {
    //"小","中","大","極大"};
    //public enum SUB1_KIND
    //{
    //    NON = -1,
    //    FELLOW,//味方
    //    ENEMY,//敵
    //    POISON,//毒
    //    DARK,//暗闇
    //    PARALYSIS,// 麻痺
    //    DEATH,//即死
    //    SUCTION,//吸収
    //    TARGET,//必中
    //    MAX
    //}
    //public static string[] sub1String_ = new string[(int)SUB1_KIND.MAX] {
    //"味方","敵","毒","暗闇","麻痺","即死","吸収","必中"};

    //public enum SUB2_KIND
    //{
    //    NON = -1,
    //    HP,//HP
    //    MAGIC_ATTACK,// 魔法攻撃
    //    PHYSICS_ATTACK,//物理攻撃
    //    DEFENCE,//防御力
    //    SPEED,//命中/回避
    //    POISON,//毒
    //    DARK,//暗闇
    //    PARALYSIS,// 麻痺
    //    DEATH,//即死
    //    SUCTION,//吸収
    //    TARGET,//必中
    //    MAX
    //}
    //public static string[] sub2String_ = new string[(int)SUB2_KIND.MAX] {
    //"HP","魔法攻撃","物理攻撃","防御力","命中/回避","毒","暗闇","麻痺","即死","吸収","必中"};

    //public enum SUB3_KIND
    //{
    //    NON = -1,
    //    UP,//上昇、
    //    DOWN,//低下、
    //    REFLECTION,//反射
    //    SUCTION,//吸収
    //    TARGET,//必中
    //    MAX
    //}
    //public static string[] sub3String_ = new string[(int)SUB3_KIND.MAX] {
    //"上昇","低下","反射","吸収","必中"};



    public struct MagicData
    {
        public int number;
        public string name;
        public string main;
        public string sub;
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
    public static MagicData[] data = new MagicData[20];
    public static Sprite[] magicSpite = new Sprite[20];

    // バッグ表示用
    [SerializeField]
    private GameObject bagMagicUI;     // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private RectTransform bagMagicParent;// アイテムボックスから確認するときの親

    public static GameObject[] magicObject = new GameObject[20];
    private Image[] magicImage_ = new Image[20];

    // ステータス表示用
    [SerializeField]
    private GameObject statusMagicUI;     // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private RectTransform statusMagicParent;// ステータス画面を開いた時の親

    public static GameObject[] statusMagicObject = new GameObject[20];
    private Image[] statusMagicImage_ = new Image[20];
    private Button[] statusMagicBtn_ = new Button[20];

    // 共通
    public static int number_ = 1;
    private int minNumber_ = 2;

    // 魔法を捨てる関連
    private int clickMagicNum_ = -1;
    private Button throwAwayBtn_;
    private Text info_; // クリックしたアイテムを説明する欄

    private List<Chara> charasList_ = new List<Chara>();
    public void Init()
    {
        if (magicObject[1] == null)
        {
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Magic>();
            charasList_ = SceneMng.charasList_;
            // データの個数が最低値より大きかったらデータを呼ばない
            if (number_ < minNumber_)
            {
                return;
            }
            // Debug.Log(number_);
            for (int i = 0; i < number_; i++)
            {
                data[i].number = int.Parse(csvDatas[i + 1][0]);
                data[i].name = csvDatas[i + 1][1];
                data[i].main = csvDatas[i + 1][2];
                data[i].sub = csvDatas[i + 1][3];
                data[i].power = int.Parse(csvDatas[i + 1][4]);
                data[i].rate = int.Parse(csvDatas[i + 1][5]);
                data[i].head = int.Parse(csvDatas[i + 1][6]);
                data[i].element = int.Parse(csvDatas[i + 1][7]);
                data[i].tail = int.Parse(csvDatas[i + 1][8]);
                data[i].sub1 = int.Parse(csvDatas[i + 1][9]);
                data[i].sub2 = int.Parse(csvDatas[i + 1][10]);
                data[i].sub3 = int.Parse(csvDatas[i + 1][11]);

                Debug.Log(i + "番目：" + data[i].name + "           " + data[i].element);
                if (i == 0)
                {
                    continue;
                }
                //  magicSpite[i]= ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                // data[i].battleSet0 = bool.Parse(csvDatas[i + 1][5]);

                // elementNum_[i] = minElementNum_ + data[i].element;
                // バッグ用
                magicObject[i] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
                magicObject[i].name = "Magic" + data[i].number;
                magicImage_[i] = magicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                magicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];

                // ステータス用の座標に変更
                statusMagicObject[i] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, statusMagicParent.transform);
                statusMagicObject[i].name = "Magic" + data[i].number;
                statusMagicImage_[i] = statusMagicObject[i].transform.Find("MagicIcon").GetComponent<Image>();
                statusMagicBtn_[i] = statusMagicObject[i].GetComponent<Button>();
                // Debug.Log(statusMagicBtn_[i].name);
                statusMagicImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[i].element];
                //Debug.Log(data[i].name + "           " + data[i].number);
            }
        }
    }

    public void MagicCreateCheck(string magicName, string mainWords, string subWords, int pow, int rateNum,
                                  int head, int element, int tail, int sub1, int sub2, int sub3)
    {
        data[number_].number = number_;
        data[number_].name = magicName;
        data[number_].main = mainWords;
        data[number_].sub = subWords;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].head = head;
        data[number_].element = element;
        data[number_].tail = tail;
        data[number_].sub1 = sub1;
        data[number_].sub2 = sub2;
        data[number_].sub3 = sub3;
        Debug.Log("保存した魔法" + data[number_].main + data[number_].sub);
        DataSave();

        // バッグ用
        magicObject[number_] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                            Quaternion.identity, bagMagicParent.transform);
        magicObject[number_].name = "Magic_" + data[number_].number;
        magicImage_[number_] = magicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];
        //Debug.Log(data[number_].name + "  " + data[number_].power + "  " + data[number_].rate + "  " + data[number_].ability);

        // ステータス用の座標に変更
        statusMagicObject[number_] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                        Quaternion.identity, statusMagicParent.transform);
        statusMagicObject[number_].name = "Magic_" + data[number_].number;
        statusMagicImage_[number_] = statusMagicObject[number_].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[number_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][element];

        number_++;


        // 現在受注中のクエストを見る
        var orderList = QuestClearCheck.GetOrderQuestsList();

        for (int k = 0; k < orderList.Count; k++)
        {
            // 炎単体小のクエストクリア確認
            // 受注したのが炎マテリアの合成クエスト(番号が3)のとき
            if (int.Parse(orderList[k].Item1.name) == 3)
            {
                // Magic_番号の番号を見て、1なら「炎単体小」の魔法なのですでに魔法を取得しているからクリア状態にする
                for (int i = 1; i < magicObject.Length; i++)
                {
                    if (int.Parse(magicObject[i].name.Split('_')[1]) == 1)
                    {
                        // クリア状態にする
                        QuestClearCheck.QuestClear(3);
                        break;  // これ以上for文を回す必要がないから抜ける
                    }
                }
            }
        }

    }

    public void SetStatusMagicCheck(int num, bool flag)
    {
        if (num < 1 || number_ < num)
        {
            // 範囲外の数値が入ってた場合はreturnする
            return;
        }
        Debug.Log(flag + "        既に選択されている魔法の番号：" + num);
        Debug.Log(statusMagicBtn_[num].name + "      " + statusMagicBtn_[num].interactable);


        // flagがtrueならinteractableをfalseにする
        statusMagicBtn_[num].interactable = flag == true ? false : true;
    }


    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
        //   Debug.Log("ロードします");

        csvDatas.Clear();
       // csvMagicDatas.Clear();

        // 行分けだけにとどめる
        texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
           // csvMagicDatas.Add(texts[i].Split(','));
        }
        Debug.Log("データ数" + csvDatas.Count);
        number_ = csvDatas.Count - 1;

        Init();
    }

    private void DataSave()
    {
        Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        for (int i = clickMagicNum_; i < number_; i++)
        {
            data[i].number -= 1;
        }
        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            if (i == (clickMagicNum_ ))
            {
                // 削除された行は読み込まない
                // Debug.Log(magicObject[clickMagicNum_].name+"を削除：行" +i);
                continue;
            }
            saveCsvSc_.SaveMagicData(data[i]);
            //  saveCsvSc_.SaveMagicData((MagicData)csvDatas[i][]);

            Debug.Log("行" + i);
        }
        if (clickMagicNum_ != -1)
        {
            number_ -= 1;
            clickMagicNum_ = -1;
        }

        saveCsvSc_.SaveEnd();
    }


    public void SetMagicNumber(int num)
    {
        // 捨てるボタンを表示
        if (throwAwayBtn_ == null)
        {
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/MagicDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);
        throwAwayBtn_.interactable = true;

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            for (int m = 0; m < 3; m++)
            {
                Debug.Log("セットされてる魔法番号" + charasList_[i].GetMagicNum(m).number);
                if (charasList_[i].GetMagicNum(m).number==num)
                {
                    throwAwayBtn_.interactable = false;
                }
            }
        }

        // 選択されたアイテムの番号を保存
        clickMagicNum_ = num;
    }

    public void OnClickThrowAwayButton()
    {
        if (bagMagicParent.gameObject.activeSelf == false
         || statusMagicParent.gameObject.activeSelf == false)
        {
            //throwAwayBtn_.gameObject.SetActive(false);
            return;
        }
        Debug.Log("指定する魔法番号"+clickMagicNum_);
        // 指定の魔法を削除する
        magicObject[clickMagicNum_].SetActive(false);
        statusMagicObject[clickMagicNum_].SetActive(false);
        throwAwayBtn_.gameObject.SetActive(false);
        info_.text = "";


        // 指定の魔法以外をロードする
        csvDatas.Clear();

        // 行分けだけにとどめる
        texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < texts.Length; i++)
        {
            //Debug.Log("行" + i);
            if (i == (clickMagicNum_ + 1))
            {
                // 削除された行は読み込まない
                // Debug.Log(magicObject[clickMagicNum_].name+"を削除：行" +i);
                continue;
            }
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
            Debug.Log("行" + i);
            // Debug.Log(magicObject[i].name + "：行" + i);
        }
        Debug.Log("データ数" + csvDatas.Count);

        // 指定行を読み込まない状態でセーブをする
       DataSave();
    }
}