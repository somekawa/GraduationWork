using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuActive : MonoBehaviour
{
    //[SerializeField]
    //public GameObject canvasPrefab;    // 素材を拾ったときに生成されるプレハブ
    //private GameObject parentObjPrefab_;

    private GameObject itemBag_;

    public enum topic
    {
        NON = -1,
        ITEM,// 魔法
        MATERIA,// 素材
        WORD,// ワード
        MAX,
    }
    // アイテム選択時に表示するトピック名
    private Text topicText_;
    private int stringNum_ = 0;
    private string[] topicString_ = new string[(int)topic.MAX] {
    "アイテム","そざい","わーど"};


    private Canvas parentMenuCanvas_;
    private enum canvas
    {
        NON = -1,
        MENU,// メニュー関連のCanvas
        BAG,// メニューでアイテムを選んだ際のCanvas
            // STATUS,
        SAVE_LOAD,// セーブとロード用のキャンバス
        OTHER,// クエストと図鑑用のキャンバス
        MAX
    }
    private RectTransform[] parentRectTrans_ = new RectTransform[(int)canvas.MAX];

    // バッグ使用中はずっとアクティブにしたいCanvasのため別で取得
    // private Canvas menuCanvas_;         // メニューCanvas
    private RectTransform parentMenuBtn_;  // 表示するボタンたちの親
    private Vector2 startMenuPos_;      // メニューの初期座標
    private Vector2 maxMenuPos_;        // メニューのマックス座標
    private float menuSpeed_ = 50.0f;   // メニューが動くスピード
    private Image bagImage_;// バッグの画像
    private TMPro.TextMeshProUGUI statusInfo_;  // キャラのステータス描画先

    // ItemBox選択時
    private RectTransform topicParent_;// 何を表示しているか
    private RectTransform[] mngs_ = new RectTransform[(int)topic.MAX];
    private Bag_Materia bagMateria_;
    private Bag_Item bagItem_;
    private Bag_Word bagWord_;

    // バッグ使用中かどうか
    private bool activeFlag_ = false;

    // ワープ中かどうか
    private WarpField warpField_;

    void Start()
    {
        warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();
        parentMenuCanvas_ = GameObject.Find("MenuCanvas").GetComponent<Canvas>();
        
        parentRectTrans_[(int)canvas.MENU] = parentMenuCanvas_.transform.Find("Menu").GetComponent<RectTransform>();
        // バッグ（左下）の画像
        bagImage_ = parentRectTrans_[(int)canvas.MENU].transform.Find("BagImage").GetComponent<Image>();
        statusInfo_ = parentRectTrans_[(int)canvas.MENU].transform.Find("StatusInfo").GetComponent<TMPro.TextMeshProUGUI>();
        statusInfo_.gameObject.SetActive(false);
        parentMenuBtn_ = parentRectTrans_[(int)canvas.MENU].transform.Find("MenuImage").GetComponent<RectTransform>();
        parentMenuBtn_.gameObject.SetActive(false);
        startMenuPos_ = parentMenuBtn_.transform.localPosition;
        maxMenuPos_ = new Vector2(startMenuPos_.x, 300.0f);
        Debug.Log("Menuボタンの初期座標" + startMenuPos_ + "        移動最大位置" + maxMenuPos_);

        // アイテム、ワード、素材表示キャンバス
        //if(BagCanvasUI.singleton==null)
        //{
        //    parentObjPrefab_ = Instantiate(canvasPrefab);
        //    parentObjPrefab_.GetComponent<BagCanvasUI>().SetMyName("CommonItemBagCanvas");

        //}
        itemBag_ = BagCanvasUI.Instance;
        mngs_[(int)topic.ITEM] = itemBag_.transform.Find("ItemMng").GetComponent<RectTransform>();
        bagItem_ = itemBag_.transform.Find("ItemMng").GetComponent<Bag_Item>();
        mngs_[(int)topic.MATERIA] = itemBag_.transform.Find("MateriaMng").GetComponent<RectTransform>();
        bagMateria_ = itemBag_.transform.Find("MateriaMng").GetComponent<Bag_Materia>();
        mngs_[(int)topic.WORD] = itemBag_.transform.Find("WordMng").GetComponent<RectTransform>();
        bagWord_ = itemBag_.transform.Find("WordMng").GetComponent<Bag_Word>();
        itemBag_.gameObject.SetActive(false);

        parentRectTrans_[(int)canvas.BAG] = parentMenuCanvas_.transform.Find("ItemBag").GetComponent<RectTransform>();
        topicParent_ = parentRectTrans_[(int)canvas.BAG].transform.Find("Topics").GetComponent<RectTransform>();
        topicText_ = topicParent_.transform.Find("TopicName").GetComponent<Text>();
        topicText_.text = topicString_[(int)topic.ITEM];

        // セーブとロード用のキャンバス
        parentRectTrans_[(int)canvas.SAVE_LOAD] = parentMenuCanvas_.transform.Find("SaveOrLoad").GetComponent<RectTransform>();
        parentRectTrans_[(int)canvas.SAVE_LOAD].gameObject.SetActive(false);

        // クエスト確認と図鑑用のキャンバス
        parentRectTrans_[(int)canvas.OTHER] = parentMenuCanvas_.transform.Find("OtherUI").GetComponent<RectTransform>();
        parentRectTrans_[(int)canvas.OTHER].gameObject.SetActive(false);

        // メニューの子であるBagは常時表示しておきたいためBagスタート
        for (int i = (int)canvas.BAG; i < (int)canvas.MAX; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < (int)topic.MAX; i++)
        {
            mngs_[i].gameObject.SetActive(false);
        }

        //if (parentObjPrefab_ == null)
        //{
        //    parentObjPrefab_ = parentObjPrefab_.transform;
        //}
    }

    void Update()
    {
        if (warpField_.GetWarpNowFlag() == true)
        {
            return;
        }

        if (parentMenuBtn_.gameObject.activeSelf == false)
        {
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                activeFlag_ = true;
                bagImage_.color = new Color(0.5f, 1.0f, 0.5f, 1.0f);
                StartCoroutine(MoveMenuButtons(1));
            }
        }
    }

    public void OnClickItemButton()
    {
        // アイテムボタンが押されるとき
        CommonOnClick(canvas.BAG);

        itemBag_.gameObject.SetActive(true);
        //parentCanvas_[(int)canvas.SAVE_LOAD].gameObject.SetActive(false);
        //parentCanvas_[(int)canvas.OTHER].gameObject.SetActive(false);
        bagItem_.ActiveItem(this);
        //parentObjPrefab_.transform.Find("ItemMng").GetComponent<Bag_Item>().Init(this);
    }

    public void OnClickCancelButton()
    {
        // キャンセルボタンが押されるとき
        itemBag_.gameObject.SetActive(false);
        StartCoroutine(MoveMenuButtons(-1));
        bagImage_.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        stringNum_ = (int)topic.ITEM;
        for (int i = 0; i < (int)topic.MAX; i++)
        {
            mngs_[i].gameObject.SetActive(false);
        }
        for (int i = (int)canvas.BAG; i < (int)canvas.MAX; i++)
        {
            parentRectTrans_[i].gameObject.SetActive(false);
        }
        statusInfo_.gameObject.SetActive(false);
    }

    public void OnClickRightArrow()
    {
        // 値を加算
        stringNum_++;
        if ((int)topic.WORD < stringNum_)
        {
            stringNum_ = (int)topic.ITEM;
        }
        ActiveRectTransform();
        topicText_.text = topicString_[stringNum_];
        Debug.Log("右矢印をクリック" + stringNum_);
    }

    public void OnClickLeftArrow()
    {
        stringNum_--;
        if (stringNum_ < (int)topic.ITEM)
        {
            stringNum_ = (int)topic.WORD;
        }
        topicText_.text = topicString_[stringNum_];
        ActiveRectTransform();
        Debug.Log("左矢印をクリック" + stringNum_);
    }

    private void ActiveRectTransform()
    {
        for (int i = 0; i < (int)topic.MAX; i++)
        {
            if (stringNum_ == i)
            {
                // 選択のものを表示
                mngs_[i].gameObject.SetActive(true);
            }
            else
            {
                // 選択中のもの以外は非表示に
                mngs_[i].gameObject.SetActive(false);
            }
        }
        if (stringNum_ == (int)MenuActive.topic.MATERIA)
        {
            // WarpField.csの初期化関数を先に呼ぶ
            // parentCanvas_[(int)canvas.BAG].transform.Find("MateriaMng").GetComponent<Bag_Materia>().Init(this);
            bagMateria_.ActiveMateria(this);
        }
        else if (stringNum_ == (int)MenuActive.topic.WORD)
        {
            // WarpField.csの初期化関数を先に呼ぶ
            //parentObjPrefab_.transform.Find("WordMng").GetComponent<Bag_Word>().Init(this);
            bagWord_.ActiveWord(this);

        }
        else if (stringNum_ == (int)MenuActive.topic.ITEM)
        {
            // WarpField.csの初期化関数を先に呼ぶ
            //parentObjPrefab_.transform.Find("ItemMng").GetComponent<Bag_Item>().Init(this);
            bagItem_.ActiveItem(this);
        }
    }

    public int GetStringNumber()
    {
        return stringNum_;
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

    public void OnClickStatus()
    {
        Debug.Log("ステータス確認ボタンが押された");
        //  buttons_.SetActive(false);

        // キャラのステータス値を表示させたい
        var data = SceneMng.GetCharasSettings((int)SceneMng.CharcterNum.UNI);

        // 表示する文字の作成
        string str = "名前  :" + data.name + "\n" +
                     "レベル:" + data.Level.ToString() + "\n" +
                     "HP    :" + data.HP.ToString() + "\n" +
                     "MP    :" + data.MP.ToString() + "\n" +
                     "体    :" + data.Constitution.ToString() + "\n" +
                     "精神  :" + data.Power.ToString() + "\n" +
                     "攻撃力:" + data.Attack.ToString() + "\n" +
                     "防御力:" + data.Defence.ToString() + "\n" +
                     "素早さ:" + data.Speed.ToString() + "\n" +
                     "幸運  :" + data.Luck.ToString();

        // 作成した文字を入れる
        statusInfo_.text = str;
    }


    public void OnClickSaveOrLoad()
    {
        Debug.Log("セーブボタンが押された");
        CommonOnClick(canvas.SAVE_LOAD);
        //  parentCanvas_[(int)canvas.SAVE_LOAD].gameObject.SetActive(true);
    }

    public void OnClickOtherBtn()
    {
        Debug.Log("アザーボタンが押されました");
        CommonOnClick(canvas.OTHER);
        // parentCanvas_[(int)canvas.OTHER].gameObject.SetActive(true);
    }

    public void OnClickSave()
    {
        Debug.Log("セーブボタンが押された");

        var data = SceneMng.GetCharasSettings((int)SceneMng.CharcterNum.UNI);

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
                     data.Constitution.ToString() + "," +
                     data.Power.ToString() + "," +
                     data.Attack.ToString() + "," +
                     data.Defence.ToString() + "," +
                     data.Speed.ToString() + "," +
                     data.Luck.ToString();

        swLEyeLog.Write(str);   // 書き込み
        swLEyeLog.Flush();
        swLEyeLog.Close();
    }

    private void CommonOnClick(canvas num)
    {
        for (int i = (int)canvas.BAG; i < (int)canvas.MAX; i++)
        {
            if (i == (int)num)
            {
                parentRectTrans_[(int)num].gameObject.SetActive(true);
            }
            else
            {
                parentRectTrans_[i].gameObject.SetActive(false);
            }
        }
    }

    public bool GetActiveFlag()
    {
        return activeFlag_;
    }

    public Bag_Materia GetBagMateria()
    {
        return bagMateria_;
    }
}