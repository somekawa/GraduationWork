using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bag_Word : MonoBehaviour
{
    // データ系
    private Word_SaveCSV saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private string saveDataFilePath_;
    List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;


    private InitPopList popWordList_;
    private int[] maxCnt_ = new int[(int)InitPopList.WORD.INFO];
    [SerializeField]
    private RectTransform wordParent;    // 素材を拾ったときに生成されるプレハブ
    [SerializeField]
    private Button headBtn;    // 素材を拾ったときに生成されるプレハブ

    public enum WORD_MNG
    {
        NON = -1,
        HEAD,           // 0
        ELEMENT,        // 1 
        TAIL,           // 2
        SUB1,           // 3 
        SUB2,           // 4 
        SUB3,           // 5 
        MAX
    }

    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private Button kindsBtn_;
    private string[] btnName = new string[(int)WORD_MNG.SUB2] {
    "Head","Element","Tail","Sub"
    };

    public struct WordData
    {
        public int number;// セーブする際の番号
        public string name;// ワード名
        public int getFlag;// 0がfalseで取得してない。1がtrueで取得している
        public GameObject pleate;
        public TMPro.TextMeshProUGUI nameText;
        public Button btn;
        public InitPopList.WORD kinds;
        public int power;
        public int MP;
        public int maxCnt;
    }
    public static Dictionary<InitPopList.WORD, WordData[]> wordState = new Dictionary<InitPopList.WORD, WordData[]>();
    public static WordData[] data;// = new  WordData[29];
    private int[] kindWordCnt_ = new int[(int)WORD_MNG.SUB1];

    // 他Scriptで指定するワードは番号を取得しておく
    public static int targetWordNum;            // 必中用の番号
    public static bool getFlagCheck = false;    // 中身を魔法作成の時に渡したら使われる

    public void NewGameInit()
    {
        // タイトルでNewGameボタンを押したときに呼ばれるInit
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<Word_SaveCSV>();
        data = new WordData[29];
        int dataNum = 0;
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            maxCnt_[k] = InitPopList.maxWordKindsCnt[k];
            //wordState[(InitPopList.WORD)k] = InitWordState((InitPopList.WORD)k, maxCnt_[k]);
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                data[dataNum].number = dataNum;
                data[dataNum].name = InitPopList.name[k, i];
                data[dataNum].getFlag = 0;
                dataNum++;
            }
        }
        DataSave();
        //DataLoad();
    }

    public void Init()
    {
        eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<Word_SaveCSV>();

        if (getFlagCheck == false)
        {
            int dataNum = 0;
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                maxCnt_[k] = InitPopList.maxWordKindsCnt[k];
                wordState[(InitPopList.WORD)k] = InitWordState((InitPopList.WORD)k, maxCnt_[k], dataNum);
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    data[dataNum] = wordState[(InitPopList.WORD)k][i];
                    //Debug.Log(dataNum + "番目のワードは" + data[dataNum].name);
                    data[dataNum].number = dataNum;
                    dataNum++;
                }
            }
            getFlagCheck = true;
        }
        // 一番初めに出てくるワードの表示位置が違ったら、表示位置をバッグにする
        if (wordState[InitPopList.WORD.HEAD][0].pleate.transform.parent != wordParent.transform)
        {
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    // バッグ内の親に変更数
                    wordState[(InitPopList.WORD)k][i].pleate.transform.SetParent(wordParent.transform);
                    // クリックできない状態にする
                    wordState[(InitPopList.WORD)k][i].btn.enabled = false ;
                    if (wordState[(InitPopList.WORD)k][i].name == "必中")
                    {
                        targetWordNum = i;
                        //Debug.Log((InitPopList.WORD)k + "の中で必中を保存している番号："+targetWordNum);
                    }
                }
            }
        }

        if (wordState[InitPopList.WORD.HEAD][0].btn == null)
        {
            int dataNum = 0;
            for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
            {
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    wordState[(InitPopList.WORD)k][i] = data[dataNum];
                    dataNum++;
                }
            }
        }
        ActiveCheck(WORD_MNG.HEAD);
        if(kindsBtn_ != null)
        {
            // もしHead以外が選択されていた場合そのボタンを押下可能にする
            kindsBtn_.interactable = true;
        }
        //WordGetCheck(InitPopList.WORD.HEAD, 2, 2);// 全体
        //WordGetCheck(InitPopList.WORD.HEAD, 1, 1);// 複数回
        //WordGetCheck(InitPopList.WORD.ELEMENT_ASSIST, 0, 4);// 補助
        //WordGetCheck(InitPopList.WORD.SUB1, 0, 13);// 味方
        //WordGetCheck(InitPopList.WORD.SUB1_AND_SUB2, 0, 20);// 毒
        //WordGetCheck(InitPopList.WORD.SUB1_AND_SUB2, 3, 23);// 即死
        //WordGetCheck(InitPopList.WORD.SUB2, 1, 16);// 物理攻撃力
        //WordGetCheck(InitPopList.WORD.SUB2, 2, 17);// 魔法攻撃力
                                                   //WordGetCheck(InitPopList.WORD.SUB2, 4, 19);// 命中/回避
                                                   //WordGetCheck(InitPopList.WORD.SUB2, 3, 18);// 防御力
                                                   // WordGetCheck(InitPopList.WORD.ALL_SUB, 1, 28);// 必中
        kindsBtn_ = headBtn;
        kindsBtn_.interactable = false;
    }

    private WordData[] InitWordState(InitPopList.WORD kind, int maxCnt,int dataNum)
    {
        int csvNum = dataNum;
        var word = new WordData[maxCnt];
        for (int i = 0; i < maxCnt; i++)
        {
            // ワード名を取得
            word[i].name = InitPopList.name[(int)kind, i];

            // オブジェクト情報を取得
            word[i].pleate = InitPopList.pleate[(int)kind, i];
            word[i].pleate.name = word[i].name;// +i;

            // オブジェクトの子のtextを取得
            word[i].nameText = word[i].pleate.transform.Find("Word").GetComponent<TMPro.TextMeshProUGUI>();
            word[i].nameText.text = word[i].name;

            // オブジェクトのコンポーネントにあるbuttonを取得
            word[i].btn = word[i].pleate.GetComponent<Button>();

            // 指定のワードを取得しているか
            Debug.Log(csvNum + i + "番目は" + csvDatas_[csvNum + 1 + i][1]);
            data[dataNum].number = csvNum + i;
            word[i].getFlag = int.Parse(csvDatas_[csvNum + 1 + i][2]);
            word[i].kinds = InitPopList.kinds[(int)kind, i];
            word[i].power = InitPopList.power[(int)kind, i];
            word[i].MP = InitPopList.MP[(int)kind, i];
            word[i].maxCnt = maxCnt;
            Debug.Log("BagWord内で " + kind + "の" + i + "番目は" + word[i].name);
        }
        return word;
    }

    public void WordGetCheck(InitPopList.WORD kinds, int wordNum, int dataNum)
    {
        Debug.Log(kinds + "の" + wordNum + " " + dataNum);
        Debug.Log(kinds + "の" + wordNum + "番目のワード：" + wordState[kinds][wordNum].name);
        Debug.Log(data[dataNum].name + "は" + dataNum + "番目です");
        wordState[kinds][wordNum].getFlag = 1;// 取得をしたら1を入れる;

        data[dataNum].getFlag = wordState[kinds][wordNum].getFlag;
        Debug.Log(data[dataNum].name +""+data[dataNum].getFlag);
    }

    public void OnClickWordKindsBtn()
    {
        // Debug.Log(eventSystem_.currentSelectedGameObject + "をクリック");
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        kindsBtn_.interactable = true;// 1つ前に選択していたボタンは押下可能に
        // 今回押したのはどのボタンか
        kindsBtn_ = clickbtn_.GetComponent<Button>();
        kindsBtn_.interactable = false;
        // どの種類のボタンを押したか
        for (int i = 0; i < (int)WORD_MNG.SUB2; i++)
        {
            if (clickbtn_.name == btnName[i])
            {
                ActiveCheck((WORD_MNG)i);
            }
        }
    }

    public void SetGetFlagCheck(bool flag)
    {
        getFlagCheck = flag;
    }

    private void ActiveCheck(WORD_MNG kinds)
    {
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                // すべて非表示にしておく
                wordState[(InitPopList.WORD)k][i].pleate.SetActive(false);
            }
        }

        switch (kinds)
        {
            case WORD_MNG.HEAD:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.HEAD]; i++)
                {
                    if (data[i].getFlag == 0)
                    {
                        continue;
                    }
                    wordState[(int)InitPopList.WORD.HEAD][i].pleate.SetActive(true);
                }
                break;

            case WORD_MNG.ELEMENT:
                for (int k = (int)InitPopList.WORD.ELEMENT_HEAL; k < (int)InitPopList.WORD.TAIL; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        if (wordState[(InitPopList.WORD)k][i].getFlag == 0)
                        {
                            continue;
                        }
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            case WORD_MNG.TAIL:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.TAIL]; i++)
                {
                    if (wordState[InitPopList.WORD.TAIL][i].getFlag == 0)
                    {
                        continue;
                    }
                    wordState[InitPopList.WORD.TAIL][i].pleate.SetActive(true);
                }
                break;

            case WORD_MNG.SUB1:
                for (int k = (int)InitPopList.WORD.SUB1; k < (int)InitPopList.WORD.INFO; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        if (wordState[(InitPopList.WORD)k][i].getFlag == 0)
                        {
                            continue;
                        }
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            default:
                break;
        }
    }

    public void DataLoad()
    {
        // Debug.Log("ロードします");
        saveDataFilePath_ = Application.streamingAssetsPath + "/Save/wordData.csv";

        csvDatas_.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas_.Add(texts[i].Split(','));
        }
        data = new WordData[csvDatas_.Count];
        Init();
    }

    public void DataSave()
    {
        saveCsvSc_.SaveStart();
        int dataNum = 0;
        for (int k = 0; k < (int)InitPopList.WORD.INFO; k++)
        {
            for (int i = 0; i < maxCnt_[k]; i++)
            {
                saveCsvSc_.SaveWordData(data[dataNum]);
                dataNum++;
            }
        }
        saveCsvSc_.SaveEnd();
    }
}