using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bag_Word : MonoBehaviour
{
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

    public struct word
    {
        public GameObject pleate;
        public Text nameText;
        public Button btn;
        public InitPopList.WORD kinds;
        public string name;
        public string english;
        public int power;
        public bool getFlag;
        public int maxCnt;
    }
    public static Dictionary<InitPopList.WORD, word[]> wordState = new Dictionary<InitPopList.WORD, word[]>();
    // 他Scriptで指定するワードは番号を取得しておく
    public static int targetWordNum;            // 必中用の番号
    public static bool getFlagCheck = false;    // 中身を魔法作成の時に渡したら使われる

    //   void Start()
    public void Init()
    {
        eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

        if (getFlagCheck == false)
        {
            for (int i = 0; i < (int)InitPopList.WORD.INFO; i++)
            {
                maxCnt_[i] = InitPopList.maxWordKindsCnt[i];
                // 事前に生成していた情報を取得
                wordState[(InitPopList.WORD)i] = InitWordState((InitPopList.WORD)i, maxCnt_[i]);
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
                    wordState[(InitPopList.WORD)k][i].pleate.transform.SetParent(wordParent.transform);
                    //// デバッグ用
                    WordGetCheck((InitPopList.WORD)k, i);
                    wordState[(InitPopList.WORD)k][i].pleate.SetActive(false);
                    if (wordState[(InitPopList.WORD)k][i].name == "必中")
                    {
                        targetWordNum = i;
                        //Debug.Log((InitPopList.WORD)k + "の中で必中を保存している番号："+targetWordNum);
                    }
                }
            }
        }
        ActiveCheck(WORD_MNG.HEAD);
    }


    private word[] InitWordState(InitPopList.WORD kind, int maxCnt)
    {
        var word = new word[maxCnt];
        for (int i = 0; i < maxCnt; i++)
        {
            // ワード名を取得
            word[i].name = InitPopList.name[(int)kind, i];
            word[i].english = InitPopList.englishName[(int)kind, i];

            // オブジェクト情報を取得
            word[i].pleate = InitPopList.pleate[(int)kind, i];
            word[i].pleate.name = word[i].name;

            // オブジェクトの子のtextを取得
            word[i].nameText = word[i].pleate.transform.Find("Word").GetComponent<Text>();
            word[i].nameText.text = word[i].name;

            // オブジェクトのコンポーネントにあるbuttonを取得
            word[i].btn = word[i].pleate.GetComponent<Button>();

            // 指定のワードを取得しているか
            word[i].getFlag = false;
            word[i].kinds = InitPopList.kinds[(int)kind, i];
            word[i].power = InitPopList.power[(int)kind, i];
            word[i].maxCnt = maxCnt;
            // Debug.Log(kind + "      "+ i);
        }
        return word;
    }

    public void WordGetCheck(InitPopList.WORD kinds, int wordNum)
    {
        //Debug.Log(wordState[kinds][wordNum].name+"は"+wordNum+"番目です");
        wordState[kinds][wordNum].getFlag = true;
    }

    public void OnClickWordKindsBtn()
    {
        Debug.Log(eventSystem_.currentSelectedGameObject + "をクリック");
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
            case Bag_Word.WORD_MNG.HEAD:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.HEAD]; i++)
                {
                    wordState[(int)InitPopList.WORD.HEAD][i].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.ELEMENT:
                for (int k = (int)InitPopList.WORD.ELEMENT_HEAL; k < (int)InitPopList.WORD.TAIL; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            case Bag_Word.WORD_MNG.TAIL:
                for (int i = 0; i < maxCnt_[(int)InitPopList.WORD.TAIL]; i++)
                {
                    wordState[InitPopList.WORD.TAIL][i].pleate.SetActive(true);
                }
                break;

            case Bag_Word.WORD_MNG.SUB1:
                for (int k = (int)InitPopList.WORD.SUB1; k < (int)InitPopList.WORD.INFO; k++)
                {
                    for (int i = 0; i < maxCnt_[k]; i++)
                    {
                        wordState[(InitPopList.WORD)k][i].pleate.SetActive(true);
                    }
                }
                break;

            default:
                break;
        }
    }
}