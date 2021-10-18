using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuActive : MonoBehaviour
{
    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    private Image backPanel_;
    //[SerializeField]
    //public GameObject canvasPrefab;    // 素材を拾ったときに生成されるプレハブ
    //private GameObject parentObjPrefab_;

    //private GameObject itemBagCanvas_;// シーン遷移後も使いたいオブジェクトたちの親
    private RectTransform itemBagMng_;// itemBag_の子
    private ItemBagMng itemBagMngCS_;
    private RectTransform pictureMng_;// itemBag_の子
    private RectTransform[] otherMng_ = new RectTransform[2];


    private Canvas parentCanvas_;

    public enum CANVAS
    {
        NON = -1,
        MENU,   // メニュー関連のCanvas
        BAG,    // メニューでアイテムを選んだ際のCanvas
        STATUS, // ステータス表示ボタン
        OTHER,  // クエストと図鑑用のキャンバス
        // ここまで表示用のMng持ち
        CANCEL,
        LOAD,   // ロード用のボタン
        SAVE,   // セーブ用のボタン
        MAX
    }
    public static RectTransform[] parentRectTrans_ = new RectTransform[(int)CANVAS.MAX];
    private string[] btnName = new string[(int)CANVAS.MAX] {
    "Menu","ItemBox","Status","Other","Cancel","Load","Save"
    };
    // バッグ使用中はずっとアクティブにしたいCanvasのため別で取得
    // private Canvas menuCanvas_;         // メニューCanvas
    private RectTransform parentMenuBtn_;  // 表示するボタンたちの親
    private Vector2 startMenuPos_;      // メニューの初期座標
    private Vector2 maxMenuPos_;        // メニューのマックス座標
    private float menuSpeed_ = 50.0f;   // メニューが動くスピード
    private Image bagImage_;// バッグの画像
    private TMPro.TextMeshProUGUI statusInfo_;  // キャラのステータス描画先

    //// ItemBox選択時
    //private RectTransform topicParent_;// 何を表示しているか
    //private RectTransform[] mngs_ = new RectTransform[(int)topic.MAX];
    private Bag_Materia bagMateria_;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    // バッグ使用中かどうか
    private bool activeFlag_ = false;

    // ワープ中かどうか
    private WarpField warpField_;

    void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();

        parentCanvas_ = GameObject.Find("DontDestroyCanvas").GetComponent<Canvas>();
        backPanel_ = parentCanvas_.transform.Find("BackPanel").GetComponent<Image>();
        parentRectTrans_[(int)CANVAS.MENU] = parentCanvas_.transform.Find("Menu").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.BAG] = parentCanvas_.transform.Find("ItemBagMng").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.STATUS] = parentCanvas_.transform.Find("StatusMng").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.OTHER] = parentCanvas_.transform.Find("OtherUI").GetComponent<RectTransform>();


        // バッグ（左下）の画像
        bagImage_ = parentRectTrans_[(int)CANVAS.MENU].Find("BagImage").GetComponent<Image>();
     
        // ステータス表示
        statusInfo_ = parentRectTrans_[(int)CANVAS.STATUS].Find("StatusInfo").GetComponent<TMPro.TextMeshProUGUI>();

        // メニューで表示するボタンたち
        parentMenuBtn_ = parentRectTrans_[(int)CANVAS.MENU].Find("MenuImage").GetComponent<RectTransform>();
        parentMenuBtn_.gameObject.SetActive(false);
        startMenuPos_ = parentMenuBtn_.transform.localPosition;
        maxMenuPos_ = new Vector2(startMenuPos_.x, 300.0f);
        Debug.Log("Menuボタンの初期座標" + startMenuPos_ + "        移動最大位置" + maxMenuPos_);

        // アイテム、ワード、素材表示キャンバス
        //itemBagCanvas_ = BagCanvasUI.Instance;
        itemBagMngCS_ = parentCanvas_.transform.Find("ItemBagMng").GetComponent<ItemBagMng>();
        // parentRectTrans_[(int)CANVAS.BAG] = itemBagMng_;
        bagItem_ = parentRectTrans_[(int)CANVAS.BAG].transform.Find("ItemMng").GetComponent<Bag_Item>();
        bagMateria_ = parentRectTrans_[(int)CANVAS.BAG].transform.Find("MateriaMng").GetComponent<Bag_Materia>();
        bagWord_ = parentRectTrans_[(int)CANVAS.BAG].transform.Find("WordMng").GetComponent<Bag_Word>();
        parentRectTrans_[(int)CANVAS.BAG].gameObject.SetActive(false);
        //itemBagCanvas_.gameObject.SetActive(false);

        //topicParent_ = parentRectTrans_[(int)canvas.BAG].transform.Find("Topics").GetComponent<RectTransform>();
        //topicText_ = topicParent_.transform.Find("TopicName").GetComponent<Text>();
        //topicText_.text = topicString_[(int)topic.ITEM];

        // セーブとロード用のキャンバス
        // parentRectTrans_[(int)CANVAS.SAVE_LOAD] = parentCanvas_.transform.Find("SaveOrLoad").GetComponent<RectTransform>();
        //parentRectTrans_[(int)CANVAS.SAVE_LOAD].gameObject.SetActive(false);

        // クエスト確認と図鑑用のキャンバス
        //otherMng_[0] = itemBagMng_.transform.Find("PictureImage").GetComponent<RectTransform>();
        //otherMng_[1] = itemBagMng_.transform.Find("QuestImage").GetComponent<RectTransform>();
        //parentRectTrans_[(int)CANVAS.OTHER] = pictureMng_;
        //parentRectTrans_[(int)canvas.OTHER] = parentMenuCanvas_.transform.Find("OtherUI").GetComponent<RectTransform>();
        // parentRectTrans_[(int)CANVAS.OTHER].gameObject.SetActive(false);

        backPanel_.gameObject.SetActive(false);
        // メニューの子であるBagは常時表示しておきたいためBagスタート
        for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
        //for (int i = 0; i < (int)topic.MAX; i++)
        //{
        //    mngs_[i].gameObject.SetActive(false);
        //}
    }

    void Update()
    {
        if ((int)SceneMng.nowScene == (int)SceneMng.SCENE.CONVERSATION)
        {
            return;
        }

        if (warpField_.GetWarpNowFlag() == true)
        {
            return;
        }

        if (parentMenuBtn_.gameObject.activeSelf == false)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                Debug.Log("メニュー画面を表示します");
                FieldMng.nowMode = FieldMng.MODE.MENU;  // ユニが歩行できないようにモードを切り替える  activeFlag_ = true;
                bagImage_.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);
                parentRectTrans_[(int)CANVAS.MENU].gameObject.SetActive(true);
                StartCoroutine(MoveMenuButtons(1));
            }
        }
    }


    //public int GetStringNumber()
    //{
    //    return stringNum_;
    //}

    private IEnumerator MoveMenuButtons(int puramai)
    {
        parentMenuBtn_.gameObject.SetActive(true);
        while (true)
        {
            yield return null;

            menuSpeed_ += 50.0f * Time.deltaTime;
            // puramai 表示したい時は正、非表示にしたいときは負
            parentMenuBtn_.transform.localPosition = new Vector2(
                    parentMenuBtn_.transform.localPosition.x,
                    parentMenuBtn_.transform.localPosition.y + menuSpeed_ * puramai);
            if (puramai == 1)
            {
                if (maxMenuPos_.y < parentMenuBtn_.transform.localPosition.y)
                {
                    parentMenuBtn_.transform.localPosition = maxMenuPos_;
                    menuSpeed_ = 50.0f;
                    yield break;
                }
            }
            else
            {
                if (parentMenuBtn_.transform.localPosition.y < startMenuPos_.y)
                {
                    menuSpeed_ = 50.0f;
                    parentMenuBtn_.transform.localPosition = startMenuPos_;
                    parentMenuBtn_.gameObject.SetActive(false);
                    activeFlag_ = false;
                    yield break;
                }
            }
        }
    }

    private void SctiveStatus()
    {
        Debug.Log("ステータス確認ボタンが押された");
        //  buttons_.SetActive(false);

        // キャラのステータス値を表示させたい
        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // 表示する文字の作成
        string str = "名前  :" + data.name + "\n" +
                     "レベル:" + data.Level.ToString() + "\n" +
                     "HP    :" + data.HP.ToString() + "\n" +
                     "MP    :" + data.MP.ToString() + "\n" +
                     "攻撃力:" + data.Attack.ToString() + "\n" +
                     "防御力:" + data.Defence.ToString() + "\n" +
                     "素早さ:" + data.Speed.ToString() + "\n" +
                     "幸運  :" + data.Luck.ToString();

        // 作成した文字を入れる
        statusInfo_.text = str;
    }


    private void DateSave()
    {
        Debug.Log("セーブボタンが押された");

        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // データ書き出しテスト
        StreamWriter swLEyeLog;
        FileInfo fiLEyeLog;

        // 保存位置
        fiLEyeLog = new FileInfo(Application.dataPath + "/saveData.csv");

        swLEyeLog = fiLEyeLog.AppendText();

        // 書き込み内容の作成
        string str = data.name + "," +
                     data.Level.ToString() + "," +
                     data.HP.ToString() + "," +
                     data.MP.ToString() + "," +
                     data.Attack.ToString() + "," +
                     data.Defence.ToString() + "," +
                     data.Speed.ToString() + "," +
                     data.Luck.ToString();

        swLEyeLog.Write(str);   // 書き込み
        swLEyeLog.Flush();
        swLEyeLog.Close();
    }

    public bool GetActiveFlag()
    {
        return activeFlag_;
    }

    public Bag_Materia GetBagMateria()
    {
        return bagMateria_;
    }

    public void OnClickMenuBtn()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        if (clickbtn_.name == btnName[(int)CANVAS.CANCEL])
        {
            //itemBagFlag_ = false;
            backPanel_.gameObject.SetActive(false) ;
            StartCoroutine(MoveMenuButtons(-1));
            bagImage_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
            {
                parentRectTrans_[i].gameObject.SetActive(false);
            }
          //  statusInfo_.gameObject.SetActive(false);
            return;
        }

        // キャンセル意外だった場合
        backPanel_.gameObject.SetActive(true);
        if (clickbtn_.name == btnName[(int)CANVAS.SAVE])
        {
            DateSave();
            return;
        }
        else
        {
            //statusInfo_.gameObject.SetActive(false);
            for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
            {
            if (clickbtn_.name == btnName[i])
                {
                //Debug.Log(clickbtn_.name);
                // クリックしたボタンの名前と一致していたら
                SelectCanvasActive((CANVAS)i);
                }
                else
                {
                parentRectTrans_[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void SelectCanvasActive(CANVAS canvas)
    {
        Debug.Log(canvas + "を表示しています");

        parentRectTrans_[(int)canvas].gameObject.SetActive(true);
        switch (canvas)
        {
            case CANVAS.BAG:
                // アイテムボタンが押されるとき
                itemBagMngCS_.ActiveRectTransform();
                break;

            case CANVAS.STATUS:
                SctiveStatus();
                //Debug.Log("移動速度変更" + charaRunSpeed);
                break;

            case CANVAS.OTHER:
                // PictureAndQuestMng.csの初期化関数を呼ぶ
                GameObject.Find("DontDestroyCanvas/OtherUI").GetComponent<PictureAndQuestMng>().Init();
                //Debug.Log("移動速度変更" + charaRunSpeed);
                break;

            default:
                break;
        }
    }
}