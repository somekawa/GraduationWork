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
    private SaveLoadCSV saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト

    //--------------
    private EventSystem eventSystem_;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;   // どのボタンをクリックしたか代入する変数

    private Image backPanel_;       // 背景を暗くするためのパネル
    private CANVAS nowCanvas_;      // 現在開かれているメニューはどこであるかを保存する

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

    private RectTransform[] parentRectTrans_ = new RectTransform[(int)CANVAS.MAX];
    private string[] btnName_ = new string[(int)CANVAS.MAX] {
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

    // バッグ使用中かどうか
    private bool activeFlag_ = false;

    // ワープ中かどうか
    private WarpField warpField_;

    private HouseInteriorMng interiorMng_;
    private Bag_Magic bagMagic_;

    void Awake()
    {
        saveCsvSc_ = GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>();
        eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        parentCanvas_ = GameObject.Find("DontDestroyCanvas").GetComponent<Canvas>();
        backPanel_ = parentCanvas_.transform.Find("BackPanel").GetComponent<Image>();
        parentRectTrans_[(int)CANVAS.MENU] = parentCanvas_.transform.Find("Menu").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.BAG] = parentCanvas_.transform.Find("ItemBagMng").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.STATUS] = parentCanvas_.transform.Find("StatusMng").GetComponent<RectTransform>();
        parentRectTrans_[(int)CANVAS.OTHER] = parentCanvas_.transform.Find("OtherUI").GetComponent<RectTransform>();

        bagMagic_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Magic>();

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

        backPanel_.gameObject.SetActive(false);
        // メニューの子であるBagは常時表示しておきたいためBagスタート
        for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(warpField_ == null)
        {
            warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();
        }

        if (SceneMng.nowScene == SceneMng.SCENE.CONVERSATION ||
            warpField_.GetWarpNowFlag() == true ||
            FieldMng.nowMode == FieldMng.MODE.BUTTLE ||
            FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE)
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
                // parentRectTrans_[(int)CANVAS.MENU].gameObject.SetActive(true);
                StartCoroutine(MoveMenuButtons(1));
            }
        }
    }

    private IEnumerator MoveMenuButtons(int puramai)
    {
        parentMenuBtn_.gameObject.SetActive(true);

        // 街とユニハウスではセーブ可能にし、それ以外では不可能にする
        if (SceneMng.nowScene != SceneMng.SCENE.TOWN && SceneMng.nowScene != SceneMng.SCENE.UNIHOUSE)
        {
            //設定した色をstage_buttonを押した時の色へ設定
            ButtonStateColorChange("Save",false);
        }
        else
        {
            //設定した色をstage_buttonを押した時の色へ設定
            ButtonStateColorChange("Save",true);
        }

        // そもそもロードするデータがないときは押せないようにする
        //TextAsset saveFile = Resources.Load("data") as TextAsset;

        //if (saveFile == null)
        //{
        //    ButtonStateColorChange("Load", false);
        //}
        //else
        //{
        //    ButtonStateColorChange("Load", true);
        //}


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


    public void ViewStatus(int charaNum)
    {
        Debug.Log("ステータス確認ボタンが押された");
        //  buttons_.SetActive(false);

        // キャラのステータス値を表示させたい
        var data = SceneMng.GetCharasSettings(charaNum);

        // 表示する文字の作成
        string str = "名前  :" + data.name + "\n" +
                     "レベル:" + data.Level.ToString() + "\n" +
                     "HP    :" + data.HP.ToString() + "/" + data.maxHP.ToString() + "\n" +
                     "MP    :" + data.MP.ToString() + "/" + data.maxMP.ToString() + "\n" +
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

        saveCsvSc_.SaveStart(SaveLoadCSV.SAVEDATA.CHARACTER);
        // キャラクター数分のfor文を回す
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            saveCsvSc_.SaveData(SceneMng.GetCharasSettings(i));
        }
        saveCsvSc_.SaveEnd();

        // その他データのセーブ
        saveCsvSc_.SaveStart(SaveLoadCSV.SAVEDATA.OTHER);
        saveCsvSc_.OtherSaveData();
        saveCsvSc_.SaveEnd();
    }

    public void DataLoad(bool isWarp = true)
    {
        MenuClose();

        // 呼んでいいのはMenuActive.csのロードだけ。Mng系で呼ぶべきじゃない
        Debug.Log("ロードキー押下");

        bagMagic_.DataLoad();
        saveCsvSc_.LoadData(SaveLoadCSV.SAVEDATA.CHARACTER);
        saveCsvSc_.LoadData(SaveLoadCSV.SAVEDATA.OTHER);
        saveCsvSc_.LoadData(SaveLoadCSV.SAVEDATA.BOOK);

        GameObject.Find("Managers").GetComponent<Bag_Word>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Materia>().DataLoad();

        parentRectTrans_[(int)CANVAS.BAG].GetComponent<ItemBagMng>().MagicInit();

        // ワープでの強制移動かどうか
        if(isWarp)
        {
            // 強制的にゲームスタート先をTOWNにする
            SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);
        }
    }

    public bool GetActiveFlag()
    {
        return activeFlag_;
    }

    public void OnClickMenuBtn()
    {
        Debug.Log(eventSystem_.currentSelectedGameObject + "をクリック");
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        }

        clickbtn_ = eventSystem_.currentSelectedGameObject;
        if (clickbtn_.name == btnName_[(int)CANVAS.CANCEL])
        {
            MenuClose();
            return;
        }

        // キャンセル以外だった場合
        backPanel_.gameObject.SetActive(true);
        if (clickbtn_.name == btnName_[(int)CANVAS.SAVE])
        {
            DataSave();
            return;
        }
        else if (clickbtn_.name == btnName_[(int)CANVAS.LOAD])
        {
            DataLoad();
        }
        else
        {
            for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
            {
                if (clickbtn_.name == btnName_[i])
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
        // 現在のページを保存
        nowCanvas_ = canvas;

        parentRectTrans_[(int)canvas].gameObject.SetActive(true);
        switch (canvas)
        {
            case CANVAS.BAG:
                // アイテムボタンが押されるとき
                parentRectTrans_[(int)CANVAS.BAG].GetComponent<ItemBagMng>().Init();
                // itemBagMngCS_.Init();
                break;

            case CANVAS.STATUS:
                parentRectTrans_[(int)CANVAS.BAG].GetComponent<ItemBagMng>().StatusInit();
                break;

            case CANVAS.OTHER:
                // PictureAndQuestMng.csの初期化関数を呼ぶ
               // GameObject.Find("DontDestroyCanvas/OtherUI").GetComponent<PictureAndQuestMng>().Init();
                break;

            default:
                break;
        }
    }

    public CANVAS GetNowMenuCanvas()
    {
        return nowCanvas_;
    }


    public RectTransform GetItemBagMng()
    {
        return parentRectTrans_[(int)CANVAS.BAG];
    }

    public void IsOpenItemMng(bool flag)
    {
        parentRectTrans_[(int)CANVAS.BAG].gameObject.SetActive(flag);

        // 1度全てfalseにする
        for (int i = 0; i < parentRectTrans_[(int)CANVAS.BAG].childCount; i++)
        {
            parentRectTrans_[(int)CANVAS.BAG].GetChild(i).gameObject.SetActive(false);
        }

        if (flag)
        {
            // 必要なものだけtrueにする
            parentRectTrans_[(int)CANVAS.BAG].Find("ItemMng").gameObject.SetActive(true);
            parentRectTrans_[(int)CANVAS.BAG].Find("CharasText").gameObject.SetActive(true);
            parentRectTrans_[(int)CANVAS.BAG].Find("InfoBack").gameObject.SetActive(true);
        }
    }

    // セーブボタンの状態切替
    private void ButtonStateColorChange(string str,bool flag)
    {
        var tmp = parentMenuBtn_.Find(str).GetComponent<Button>();
        ColorBlock colorblock = tmp.colors;
        tmp.interactable = flag;
        if (flag)
        {
            colorblock.normalColor = new Color(1.0f,1.0f,1.0f);
        }
        else
        {
            colorblock.normalColor = new Color(0.5f, 0.5f, 0.5f);
        }
        tmp.colors = colorblock;
    }

    // キャンセルボタン等、メニュー画面を閉じる処理
    private void MenuClose()
    {
        backPanel_.gameObject.SetActive(false);
        StartCoroutine(MoveMenuButtons(-1));
        bagImage_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        FieldMng.nowMode = FieldMng.MODE.SEARCH;
        for (int i = (int)CANVAS.BAG; i < (int)CANVAS.CANCEL; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
    }
}