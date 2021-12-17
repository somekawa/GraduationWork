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
    private const string saveDataFilePath_ = @"Assets/Resources/Save/wordData.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;


    private InitPopList popWordList_;
    private int[] maxCnt_ = new int[(int)InitPopList.WORD.INFO];
    [SerializeField]
    private RectTransform wordParent;    // 素材を拾ったときに生成されるプレハブ
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
        public string english;
        public int power;
        public int maxCnt;
    }
    public static Dictionary<InitPopList.WORD, WordData[]> wordState = new Dictionary<InitPopList.WORD, WordData[]>();
    public static WordData[] data;// = new  WordData[29];
    private int[] kindWordCnt_ = new int[(int)WORD_MNG.SUB1];

    // 他Scriptで指定するワードは番号を取得しておく
    public static int targetWordNum;            // 必中用の番号
    public static bool getFlagCheck = false;    // 中身を魔法作成の時に渡したら使われる

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
                wordState[(InitPopList.WORD)k] = InitWordState((InitPopList.WORD)k, maxCnt_[k]);
                for (int i = 0; i < maxCnt_[k]; i++)
                {
                    data[dataNum] = wordState[(InitPopList.WORD)k][i];
                    //Debug.Log(dataNum + "番目のワードは" + data[dataNum].name);
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
                    if (wordState[(InitPopList.WORD)k][i].name == "必中")
                    {
                        targetWordNum = i;
                        //Debug.Log((InitPopList.WORD)k + "の中で必中を保存している番号："+targetWordNum);
                    }

                    //----デバッグ用
                    //WordGetCheck((InitPopList.WORD)k, i);
                    //wordState[(InitPopList.WORD)k][i].pleate.SetActive(false);
                    //----ここまで
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

        // WordGetCheck(InitPopList.WORD.HEAD, 1);

        ActiveCheck(WORD_MNG.HEAD);
    }


    private WordData[] InitWordState(InitPopList.WORD kind, int maxCnt)
    {
        var word = new WordData[maxCnt];
        for (int i = 0; i < maxCnt; i++)
        {
            // ワード名を取得
            word[i].name = InitPopList.name[(int)kind, i];
            word[i].english = InitPopList.englishName[(int)kind, i];

            // オブジェクト情報を取得
            word[i].pleate = InitPopList.pleate[(int)kind, i];
            word[i].pleate.name = word[i].name;// +i;

            // オブジェクトの子のtextを取得
            word[i].nameText = word[i].pleate.transform.Find("Word").GetComponent<TMPro.TextMeshProUGUI>();
            word[i].nameText.text = word[i].name;

            // オブジェクトのコンポーネントにあるbuttonを取得
            word[i].btn = word[i].pleate.GetComponent<Button>();

            // 指定のワードを取得しているか
            word[i].getFlag = int.Parse(csvDatas_[i + 1][2]);
            word[i].kinds = InitPopList.kinds[(int)kind, i];
            word[i].power = InitPopList.power[(int)kind, i];
            word[i].maxCnt = maxCnt;
            //Debug.Log("BagWord内で " + kind + "の" + i + "番目は" + word[i].name);
        }
        return word;
    }

    public void WordGetCheck(InitPopList.WORD kinds, int wordNum, int dataNum)
    {
        Debug.Log(kinds + "の" + wordNum + "番目のワード：" + wordState[kinds][wordNum].name);
        Debug.Log(data[dataNum].name + "は" + dataNum + "番目です");
        wordState[kinds][wordNum].getFlag = 1;// 取得をしたら1を入れる;

        data[dataNum].getFlag = wordState[kinds][wordNum].getFlag;

    }

    public void OnClickWordKindsBtn()
    {
        // Debug.Log(eventSystem_.currentSelectedGameObject + "をクリック");
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        kindsBtn_ = clickbtn_.GetComponent<Button>();
        kindsBtn_.interactable = false;
        // どの種類のボタンを押したか
        for (int i = 0; i < (int)WORD_MNG.SUB2; i++)
        {
            if (clickbtn_.name == btnName[i])
            {
                ActiveCheck((WORD_MNG)i);
            }
            else
            {
                kindsBtn_.interactable = true;
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