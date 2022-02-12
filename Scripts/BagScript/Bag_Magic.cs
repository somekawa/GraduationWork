using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Bag_Magic : MonoBehaviour
{
    // データ系
    private SaveCSV_Magic saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private string saveDataFilePath_;
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;
    string[] texts;

    public struct MagicData
    {
        public int number;
        public string name;
        public int rate;
        public int power;
        public int mp;
        public int head;
        public int element;
        public int tail;
        public int sub1;
        public int sub2;
        public int sub3;
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
    private Image[] magicExImage_ = new Image[20];
    private CharaUseMagic useMagic_;

    // ステータス表示用
    [SerializeField]
    private GameObject statusMagicUI;     // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private RectTransform statusMagicParent;// ステータス画面を開いた時の親

    public static GameObject[] statusMagicObject = new GameObject[20];
    private Image[] statusMagicImage_ = new Image[20];
    private Image[] statusMagicImageEx_ = new Image[20];
    private Button[] statusMagicBtn_ = new Button[20];
    private Text[] statusMagicSetText_ = new Text[20];
    // 共通
    public static int number_ = 1;

    // 魔法を捨てる関連
    private int clickMagicNum_ = -1;
    private Button throwAwayBtn_;   // 捨てるボタン
    private Text info_;             // クリックしたアイテムを説明する欄

    private List<Chara> charasList_ = new List<Chara>();

    public void NewGameInit()
    {
        // タイトルでNewGameボタンを押したときに呼ばれるInit
        DataLoad();     // エラーナンバーの魔法は残しておきたいためロード
        int indexToRemove = 0;
        data = data.Where((source, index) => index == indexToRemove).ToArray();
        Debug.Log(data);
        number_ = 1;
        DataSave();     // エラーナンバー以外を消した状態でセーブ
        DataLoad();     // エラーナンバー以外を消した状態でロード
    }

    public void Init()
    {
        if (magicObject[1] == null)
        {
            useMagic_ = new CharaUseMagic();
            saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Magic>();
            charasList_ = SceneMng.charasList_;
            //// データの個数が最低値より大きかったらデータを呼ばない
            //if (number_ < minNumber_)
            //{
            //    return;
            //}
            // Debug.Log(number_);
            for (int i = 0; i < number_; i++)
            {
                data[i].number = int.Parse(csvDatas[i + 1][0]);
                data[i].name = csvDatas[i + 1][1];
                data[i].rate = int.Parse(csvDatas[i + 1][2]);
                data[i].power = int.Parse(csvDatas[i + 1][3]);
                data[i].mp = int.Parse(csvDatas[i + 1][4]);
                data[i].head = int.Parse(csvDatas[i + 1][5]);
                data[i].element = int.Parse(csvDatas[i + 1][6]);
                data[i].tail = int.Parse(csvDatas[i + 1][7]);
                data[i].sub1 = int.Parse(csvDatas[i + 1][8]);
                data[i].sub2 = int.Parse(csvDatas[i + 1][9]);
                data[i].sub3 = int.Parse(csvDatas[i + 1][10]);

                //  Debug.Log(i + "番目：" + data[i].name + "           " + data[i].element);
                if (i == 0)
                {
                    continue;
                }
                CommonActive(true,i, i);

                // Debug.Log(statusMagicBtn_[i].name);
                //Debug.Log(data[i].name + "           " + data[i].number);

                bool exFlag = data[i].rate == 2 ? true : false;
                statusMagicImageEx_[i].gameObject.SetActive(exFlag);
                magicExImage_[i].gameObject.SetActive(exFlag);
            }
        }
    }

    public void MagicCreateCheck(string magicName, int pow, int mp, int rateNum,
                                  int head, int element, int tail, int sub1, int sub2, int sub3)
    {
        data[number_].number = number_;
        data[number_].name = magicName;
        data[number_].mp = mp;
        data[number_].power = pow;
        data[number_].rate = rateNum;
        data[number_].head = head;
        data[number_].element = element;
        data[number_].tail = tail;
        data[number_].sub1 = sub1;
        data[number_].sub2 = sub2;
        data[number_].sub3 = sub3;
        //Debug.Log("保存した魔法" + data[number_].main + data[number_].sub);

        CommonActive(true, number_, number_);
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

    public void SetStatusMagicCheck(SceneMng.CHARACTERNUM chara, int num, bool flag)
    {
        if (num < 1 || number_ < num)
        {
            // 範囲外の数値が入ってた場合はreturnする
            return;
        }
        //Debug.Log(flag + "        既に選択されている魔法の番号：" + num);
        //Debug.Log(statusMagicBtn_[num].name + "      " + statusMagicBtn_[num].interactable);

        // flagがtrueならinteractableをfalseにする
        statusMagicBtn_[num].interactable = flag == true ? false : true;

        if (flag == false)
        {
            statusMagicSetText_[num].text = "";
            Debug.Log("魔法を外したので名前を消します");
        }
        else
        {
            // どのキャラに魔法がセットされているか。セットされているキャラの名前を表示する
            statusMagicSetText_[num].text = chara == SceneMng.CHARACTERNUM.UNI ? "Uni" : "Jack";
        }
    }

    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
        csvDatas.Clear();
        saveDataFilePath_ = Application.streamingAssetsPath + "/Save/magicData.csv";

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

    public void DataSave()
    {
        Debug.Log("魔法が生成されました。セーブします");
        saveDataFilePath_ = Application.streamingAssetsPath + "/Save/magicData.csv";

        saveCsvSc_.SaveStart();
        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data[i]);
        }
        if (clickMagicNum_ != -1)
        {
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
                // Debug.Log("セットされてる魔法番号" + charasList_[i].GetMagicNum(m).number);
                if (charasList_[i].GetMagicNum(m).number == num)
                {
                    throwAwayBtn_.interactable = false;
                }
            }
        }

        // 選択されたアイテムの番号を保存
        clickMagicNum_ = num;
        info_.text = useMagic_.MagicInfoMake(data[clickMagicNum_]) + "\n" +
            "威力:" + data[clickMagicNum_].power + "消費MP:" + data[clickMagicNum_].mp;
    }

    public void OnClickThrowAwayButton()
    {
        if (bagMagicParent.gameObject.activeSelf == false
         || statusMagicParent.gameObject.activeSelf == false)
        {
            //throwAwayBtn_.gameObject.SetActive(false);
            return;
        }
        Debug.Log("指定する魔法番号" + clickMagicNum_);
        // 指定の魔法を削除する
        magicObject[clickMagicNum_].SetActive(false);
        statusMagicObject[clickMagicNum_].SetActive(false);
        throwAwayBtn_.gameObject.SetActive(false);
        info_.text = "";

        int dataNumber = 0;
        for (int i = 1; i < number_; i++)
        {
            if (i == clickMagicNum_)
            {
                Debug.Log("削除する魔法の名前：" + data[i].name);
                // 選択した魔法は読み込まない
                continue;
            }
            dataNumber++;// 保存時の番号をずらす
            Debug.Log(data[dataNumber].name + "       " + data[i].name);
            data[dataNumber] = data[i];
            data[dataNumber].number = dataNumber;
            CommonActive(false,i, dataNumber);
        }
        number_ = dataNumber;
    }

    private void CommonActive(bool instanceFlag,int oldNum, int newNum)
    {
        if(instanceFlag == true)
        {
            // バッグ用
            magicObject[newNum] = Instantiate(bagMagicUI, new Vector2(0, 0),
                                                Quaternion.identity, bagMagicParent.transform);
            // ステータス用の座標に変更
            statusMagicObject[newNum] = Instantiate(statusMagicUI, new Vector2(0, 0),
                                            Quaternion.identity, statusMagicParent.transform);
        }
        else
        {
            // バッグ用
            magicObject[newNum] = magicObject[oldNum];
            // ステータス用
            statusMagicObject[newNum] = statusMagicObject[oldNum];
        }

        // バッグ用
        magicObject[newNum].name = "Magic" + data[newNum].number;
        magicImage_[newNum] = magicObject[newNum].transform.Find("MagicIcon").GetComponent<Image>();
        magicImage_[newNum].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[newNum].element];
        magicExImage_[newNum] = magicObject[newNum].transform.Find("ExImage").GetComponent<Image>();

        // ステータス用
        statusMagicObject[newNum].name = "Magic" + data[newNum].number;
        statusMagicBtn_[newNum] = statusMagicObject[newNum].GetComponent<Button>();
        statusMagicImage_[newNum] = statusMagicObject[newNum].transform.Find("MagicIcon").GetComponent<Image>();
        statusMagicImage_[newNum].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][data[newNum].element];
        statusMagicSetText_[newNum] = statusMagicBtn_[newNum].transform.Find("SetCharaName").GetComponent<Text>();
        statusMagicSetText_[newNum].text = "";
        statusMagicImageEx_[newNum] = statusMagicObject[newNum].transform.Find("ExImage").GetComponent<Image>();

        // 大成功、成功チェック
        bool exFlag = data[newNum].rate == 2 ? true : false;
        statusMagicImageEx_[newNum].gameObject.SetActive(exFlag);
        magicExImage_[newNum].gameObject.SetActive(exFlag);
    }
}