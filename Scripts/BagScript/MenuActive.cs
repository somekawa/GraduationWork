using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MenuActive : MonoBehaviour
{
    // データ系
    private SaveCSV saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/data.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    //--------------
    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    private Image backPanel_;// 背景を暗くするためのパネル

    //private RectTransform itemBagMng_;// itemBag_の子
    //private RectTransform pictureMng_;// itemBag_の子
    //private RectTransform[] otherMng_ = new RectTransform[2];


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
    private RectTransform parentMenuBtn_;  // 表示するボタンたちの親
    private Vector2 startMenuPos_;      // メニューの初期座標
    private Vector2 maxMenuPos_;        // メニューのマックス座標
    private float menuSpeed_ = 50.0f;   // メニューが動くスピード
    private Image bagImage_;// バッグの画像
    private TMPro.TextMeshProUGUI statusInfo_;  // キャラのステータス描画先

    //// ItemBox選択時
    private ItemBagMng itemBagMngCS_;
    private Bag_Materia bagMateria_;
  //  private bool chapterCheck_ = false;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    // バッグ使用中かどうか
    private bool activeFlag_ = false;

    // ワープ中かどうか
    private WarpField warpField_;

    void Awake()
    {
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveCSV>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        Debug.Log(eventSystem.name + "をクリック");
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
        //Debug.Log("Menuボタンの初期座標" + startMenuPos_ + "        移動最大位置" + maxMenuPos_);

        // アイテム、ワード、素材表示キャンバス
        itemBagMngCS_ = parentCanvas_.transform.Find("ItemBagMng").GetComponent<ItemBagMng>();
        bagItem_ = parentRectTrans_[(int)CANVAS.BAG].transform.Find("ItemMng").GetComponent<Bag_Item>();
        bagMateria_ = parentCanvas_.transform.Find("Managers").GetComponent<Bag_Materia>();
        bagWord_ = parentRectTrans_[(int)CANVAS.BAG].transform.Find("WordMng").GetComponent<Bag_Word>();

        backPanel_.gameObject.SetActive(false);
        // メニューの子であるBagは常時表示しておきたいためBagスタート
        for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
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
                     "物理攻撃力:" + data.Attack.ToString() + "\n" +
                     "魔法攻撃力:" + data.MagicAttack.ToString() + "\n" +
                     "防御力:" + data.Defence.ToString() + "\n" +
                     "素早さ:" + data.Speed.ToString() + "\n" +
                     "幸運  :" + data.Luck.ToString();

        // 作成した文字を入れる
        statusInfo_.text = str;
    }

    private void DataSave()
    {
        Debug.Log("セーブボタンが押された");
        // ユニのデータだけテスト中
        saveCsvSc_.SaveData(SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI));
    }

    private void DataLoad()
    {
        Debug.Log("ロードキー押下");

        csvDatas.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
        }

        Debug.Log("データ数" + csvDatas.Count);

        // csvDatas[行][列]を指定して値を自由に取り出す方法
        //Debug.Log(csvDatas[0][1]);

        // 列を見た後に次の行へ進むようにする(デバッグ用)
        for (int y = 0; y < csvDatas.Count; y++) // 全ての行文で回す
        {
            for (int x = 0; x < csvDatas[y].Length; x++) // 1行当たりに対する列の数で回す
            {
                // ゲームを閉じた後とかじゃないと反映されてない可能性あり
                // SaveCSV.csからデータを直接もらってきたほうがいい?
                Debug.Log(csvDatas[y][x]);
            }
        }

        // yのほうは、キャラenum+1でいけそう2重for文でいけそうやな
        CharaBase.CharacterSetting set = new CharaBase.CharacterSetting
        {
            name = csvDatas[1][0],
            Level = int.Parse(csvDatas[1][1])
        };
        SceneMng.SetCharasSettings((int)SceneMng.CHARACTERNUM.UNI, set);
    }

    public bool GetActiveFlag()
    {
        return activeFlag_;
    }

    public Bag_Materia GetBagMateria()
    {
        return bagMateria_;
    }

    public Bag_Item GetBagItem()
    {
        return bagItem_;
    }

    public void OnClickMenuBtn()
    {
        Debug.Log(eventSystem.currentSelectedGameObject + "をクリック");
        if (eventSystem == null)
        {
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        clickbtn_ = eventSystem.currentSelectedGameObject;
        if (clickbtn_.name == btnName[(int)CANVAS.CANCEL])
        {
            backPanel_.gameObject.SetActive(false);
            StartCoroutine(MoveMenuButtons(-1));
            bagImage_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
            {
                parentRectTrans_[i].gameObject.SetActive(false);
            }
            return;
        }

        // キャンセル以外だった場合
        backPanel_.gameObject.SetActive(true);
        if (clickbtn_.name == btnName[(int)CANVAS.SAVE])
        {
            DataSave();
            return;
        }
        else if (clickbtn_.name == btnName[(int)CANVAS.LOAD])
        {
            DataLoad();
        }
        else
        {
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
                parentRectTrans_[(int)CANVAS.BAG].GetComponent<ItemBagMng>().Init();
                // itemBagMngCS_.Init();
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

        if (clickbtn_.name != btnName[(int)CANVAS.BAG])
        {
            itemBagMngCS_.SetActiveCanvas();
        }
    }
}