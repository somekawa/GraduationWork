using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Bag_Word : MonoBehaviour
{
    private InitPopList popWordList_;
    private int maxCnt_ = 0;

    [SerializeField]
    private RectTransform wordParent_;    // 素材を拾ったときに生成されるプレハブ
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

    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
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
        public bool getFlag;
    }
    public static word[] wordState_;
    // 他Scriptで指定するワードは番号を取得しておく
    public static int targetWordNum;// 必中

    //   void Start()
    public void Init()
    {

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        popWordList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();


        if (maxCnt_ == 0)
        {
            maxCnt_ = popWordList_.SetMaxWordCount();
            wordState_ = new word[maxCnt_];

            for (int i = 0; i < maxCnt_; i++)
            {
                wordState_[i] = new word
                {
                    pleate = InitPopList.wordData[i].pleate,// 生成しておいたオブジェクトを代入
                    kinds = InitPopList.wordData[i].kinds,// 取得しておいたワードの種類を代入
                    getFlag = false,// すべて持ってない状態にしておく
                };
                wordState_[i].nameText = wordState_[i].pleate.transform.Find("Word").GetComponent<Text>();
                wordState_[i].btn = wordState_[i].pleate.GetComponent<Button>();

                wordState_[i].name = wordState_[i].pleate.name;// ワード（名前）を代入
                wordState_[i].pleate.name = wordState_[i].name;
                wordState_[i].nameText.text = wordState_[i].name;
                //Debug.Log(i + "番目のワードの名前：" + wordState_[i].name);

                if(wordState_[i].name=="必中")
                {
                   targetWordNum = i;
                }

                wordState_[i].pleate.SetActive(false);// すべて非表示に
            }
        }
        if (wordState_[0].pleate.transform.parent != wordParent_.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                wordState_[i].pleate.transform.SetParent(wordParent_.transform);
            }
        }

        // デバッグ用　すべてのワードを取得した状態で始まる
        for (int i = 0; i < maxCnt_; i++)
        {

            WordGetCheck(i, wordState_[i].pleate.name, wordState_[i].kinds);

        }

        ActiveCheck(WORD_MNG.HEAD);
    }

    public void WordGetCheck(int wordNum, string word, InitPopList.WORD kinds)
    {
        wordState_[wordNum].getFlag = true;
        //wordState_[wordNum].wordPleate.SetActive(true);
    }

    public void OnClickWordKindsBtn()
    {
        Debug.Log(eventSystem.currentSelectedGameObject + "をクリック");
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }
        clickbtn_ = eventSystem.currentSelectedGameObject;
        kindsBtn_ = clickbtn_.GetComponent<Button>();
        kindsBtn_.interactable = false;
        // どの種類のボタンを押したか
        for (int i = 0; i < (int)WORD_MNG.SUB2; i++)
        {
            if (clickbtn_.name == btnName[i])
            {
                // nowWord_ = (PopMateriaList.WORD)i;

                ActiveCheck((WORD_MNG)i);
            }
            else
            {
                kindsBtn_.interactable = true;
            }
        }
    }

    private void ActiveCheck(WORD_MNG kinds)
    {
        for (int w = 0; w < (int)InitPopList.WORD.MAX; w++)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                // すべて非表示にしておく
                wordState_[i].pleate.SetActive(false);

                // 取得しているか
                if (wordState_[i].getFlag == true)
                {
                    if (kinds == WORD_MNG.HEAD)
                    {
                        if (wordState_[i].kinds == InitPopList.WORD.HEAD)
                        {
                            wordState_[i].pleate.SetActive(true);
                        }
                    }
                    else if (kinds == WORD_MNG.ELEMENT)
                    {
                        if (wordState_[i].kinds == InitPopList.WORD.ELEMENT_ASSIST
                            || wordState_[i].kinds == InitPopList.WORD.ELEMENT_ATTACK
                            || wordState_[i].kinds == InitPopList.WORD.ELEMENT_HEAL)
                        {
                            wordState_[i].pleate.SetActive(true);
                        }
                    }
                    else if (kinds == WORD_MNG.TAIL)
                    {
                        if (wordState_[i].kinds == InitPopList.WORD.TAIL)
                        {
                            wordState_[i].pleate.SetActive(true);
                        }
                    }
                    else if (kinds == WORD_MNG.SUB1)
                    {
                        if (wordState_[i].kinds == InitPopList.WORD.SUB1
                         || wordState_[i].kinds == InitPopList.WORD.SUB2
                         || wordState_[i].kinds == InitPopList.WORD.SUB1_AND_SUB2
                        || wordState_[i].kinds == InitPopList.WORD.SUB3
                        || wordState_[i].kinds == InitPopList.WORD.ALL_SUB)
                        {
                            wordState_[i].pleate.SetActive(true);
                        }
                    }
                    else
                    {
                        // 何もしない
                    }
                }
            }
        }
    }
}