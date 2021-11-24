using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 町の外での画面管理をする。(町中ではnowModeをNONにする)
// 探索・戦闘・メニュー画面の切り替わり時にenumで変更を行う
// 他のMngとかも、このスクリプトにあるnowMode_を参照して変更を行う

// このスクリプトとは別に、SceneMng的なのを用意して、シーンのロード/アンロードとnowModeを切り替えてくれるものをつくったほうがいいと思う。

public class FieldMng : MonoBehaviour
{
    // 様々なクラスからMODEの状態は見られることになるから、nowMode_はstatic変数にしたほうがいい
    // このクラスは画面状態の遷移を管理するだけで、それ以外の画面処理は他のScriptで行う

    [SerializeField]
    private GameObject fieldUICanvasPopUp_; // FieldUICanvasの中にあるPopUpという空のオブジェクトを外部アタッチする

    //[SerializeField]
    //private GameObject menu_;               // DontDestroyCanvasの中のMenuオブジェクトを外部アタッチする

    // 画面状態一覧
    public enum MODE
    {
        NON,
        SEARCH,     // 探索中
        BUTTLE,     // 戦闘中
        MENU,       // メニュー画面中
        MAX
    }

    public static MODE nowMode = MODE.SEARCH;      // 現在のモード
    public static MODE oldMode = MODE.NON;         // 前のモード

    private float toButtleTime_ = 1.0f;            // 30秒経過でバトルへ遷移する
    private float time_ = 0.0f;                    // 現在の経過時間

    private UnitychanController player_;           // プレイヤー情報格納用
    private CameraMng cameraMng_;

    private TMPro.TextMeshProUGUI titleInfo_;      // 宝箱か壁かで表示内容を変更する
    private TMPro.TextMeshProUGUI getChestsInfo_;  // 宝箱から獲得したアイテム内容の表示先

    private GameObject DataPopPrefab_;
    private ChestList  popChestList_;

    void Start()
    {
        // 現在のシーンをFIELDとする
        SceneMng.SetNowScene((SceneMng.SCENE)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        // イベントが発生するか確認する
        if (EventMng.GetChapterNum() == 8)
        {
            EventMng.SetChapterNum(8, SceneMng.SCENE.CONVERSATION);
        }
        else if(EventMng.GetChapterNum() == 13)
        {
            EventMng.SetChapterNum(13, SceneMng.SCENE.CONVERSATION);
        }

        // DesertFieldかつ「オアシスを甦らせて」のクエストを達成後なら
        // クエスト達成後の会話で進行度は15となる
        if((SceneMng.nowScene == SceneMng.SCENE.FIELD1) && EventMng.GetChapterNum() >= 15)
        {
            // Oasisオブジェクトを含めた全てのFieldMapオブジェクトをtrueにする
            var tmp = GameObject.Find("FieldMap").transform;
            for (int i = 0; i < tmp.childCount; i++)
            {
                tmp.GetChild(i).gameObject.SetActive(true);
            }
        }

        //unitychanの情報を取得
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (player_ == null)
        {
            Debug.Log("FieldMng.csで取得しているPlayer情報がnullです");
        }

        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("FieldMng.csで取得しているCameraMngがnullです");
        }

        cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ

        // WarpField.csの初期化関数を先に呼ぶ
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // イベント戦発生用の宝箱/壁情報を取得する
        CheckWallAndChestActive("ButtleWall");
        CheckWallAndChestActive("Chests");

        // Chest.xlsから宝箱内容の取得を行う
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popChestList_  = DataPopPrefab_.GetComponent<PopList>().GetData<ChestList>(PopList.ListData.CHEST);

        // 内容のタイトル
        titleInfo_ = fieldUICanvasPopUp_.transform.Find("TitleInfo").GetComponent<TMPro.TextMeshProUGUI>();
        // 宝箱の文字描画先
        getChestsInfo_ = fieldUICanvasPopUp_.transform.Find("GetChestsInfo").GetComponent<TMPro.TextMeshProUGUI>();
    }

    // クエストの受注状況に合わせて、壁や宝箱のアクティブ状態を判別する
    private void CheckWallAndChestActive(string parentName)
    {
        // 現在受注中のクエスト情報を見る
        var orderList = QuestClearCheck.GetOrderQuestsList();

        // イベント用の宝箱情報を取得する
        var objParent = GameObject.Find(parentName);
        var objChild = new GameObject[objParent.transform.childCount];
        for (int i = 0; i < objParent.transform.childCount; i++)
        {
            objChild[i] = objParent.transform.GetChild(i).gameObject;
        }

        // 1つもクエストを受注していない際は、全ての宝箱/壁を非アクティブにする
        if (orderList.Count <= 0)
        {
            for (int i = 0; i < objChild.Length; i++)
            {
                objChild[i].SetActive(false);    
            }
        }

        // 宝箱/壁の個数でfor文を回す
        for (int i = 0; i < objChild.Length; i++)
        {
            // 受注中クエスト個数でfor文を回す
            for (int k = 0; k < orderList.Count; k++)
            {
                // 名前が[クエスト番号 - 配置番号]となっているから、ここで分割してあげる
                string[] arr = objChild[i].name.Split('-');

                if (arr[0] == orderList[k].Item1.name)
                {
                    // 名前一致時の処理
                    if (orderList[k].Item2)
                    {
                        // クリア済みクエストなら宝箱/壁を非アクティブへ
                        objChild[i].SetActive(false);
                    }
                    else
                    {
                        // 未クリアなら宝箱/壁をアクティブへ
                        objChild[i].SetActive(true);
                    }
                    break;
                }
                else
                {
                    // 名前不一致時の処理
                    objChild[i].SetActive(false);
                }
            }
        }
    }

    void Update()
    {
        //Debug.Log("現在のMODE" + nowMode);
        //Debug.Log(time_);

        if (nowMode == MODE.SEARCH)
        {
            // 探索中の時間加算処理
            if (player_.GetMoveFlag() && time_ < toButtleTime_)
            {
                time_ += Time.deltaTime;
            }
            else if (time_ >= toButtleTime_)
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;

                // まだポップアップ中であったら、非アクティブにする
                if(fieldUICanvasPopUp_.activeSelf)
                {
                    fieldUICanvasPopUp_.SetActive(false);
                }
            }
            else
            {
                // 何も処理を行わない
            }
        }

        // 前回のModeと一致しないとき
        if (nowMode != oldMode)
        {
            ChangeMode(nowMode);
        }
    }

    // モードが切り替わったタイミングのみで呼び出す関数
    void ChangeMode(MODE mode)
    {
        switch (mode)
        {
            case MODE.NON:
                break;

            case MODE.SEARCH:
                cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ
                //menu_.SetActive(true);
                break;

            case MODE.BUTTLE:
                cameraMng_.SetChangeCamera(true);    // サブカメラアクティブ
                //menu_.SetActive(false);
                break;

            case MODE.MENU:
                break;

            default:
                Debug.Log("画面状態一覧でエラーです");
                break;
        }

        // oldModeの更新をする
        oldMode = nowMode;
    }

    // 現在値 / エンカウント発生値
    public float GetNowEncountTime()
    {
        return time_ / toButtleTime_;
    }

    // flagは、true->宝箱,false->壁(強制戦闘)の処理という風に使い分ける
    public void ChangeFieldUICanvasPopUpActive(int num1,int num2,bool flag)
    {
        if(flag)
        {
            // 宝箱関連処理
            // 表示する文字を決定する
            for (int i = 0; i < popChestList_.param.Count; i++)
            {
                // どちらの数字も一致していたら
                if (popChestList_.param[i].num1 == num1 &&
                   popChestList_.param[i].num2 == num2)
                {
                    // ExcelのgetItem内容を書き込み、タイトルもGetItemにする
                    titleInfo_.text = "GetItem";
                    getChestsInfo_.text = popChestList_.param[i].getItem;
                }
            }

            fieldUICanvasPopUp_.SetActive(true);
            // 選択肢は非表示にする
            // 処理回数的にFindを適時行っても問題なさそうなので、この書き方にしています。
            fieldUICanvasPopUp_.transform.Find("Select").gameObject.SetActive(false);

            StartCoroutine(PopUpMessage());
        }
        else
        {
            // 壁(強制戦闘)関連処理
            titleInfo_.text = "Danger!!";
            getChestsInfo_.text = "モンスターの気配がする…\n先に進みますか？";

            // すでにアクティブ状態ならば、非アクティブになるし、
            // 非アクティブならば、アクティブにする(処理順逆にすると選択肢出てこないから注意)
            // 処理回数的にFindを適時行っても問題なさそうなので、この書き方にしています。
            fieldUICanvasPopUp_.transform.Find("Select").gameObject.SetActive(!fieldUICanvasPopUp_.activeSelf);
            fieldUICanvasPopUp_.SetActive(!fieldUICanvasPopUp_.activeSelf);
        }
    }

    private IEnumerator PopUpMessage()
    {
        float time = 0.0f;
        while (fieldUICanvasPopUp_.activeSelf)
        {
            yield return null;

            if(time >= 3.0f)
            {
                fieldUICanvasPopUp_.SetActive(false);
            }
            else
            {
                time += Time.deltaTime;
            }
        }
    }

    public void MoveArrowIcon(bool flag)
    {
        // 処理回数的にFindを適時行っても問題なさそうなので、この書き方にしています。
        if(flag)
        {
            // はい
            fieldUICanvasPopUp_.transform.Find("Select/Icon").transform.localPosition = new Vector3(-160.0f, -60.0f, 0.0f);
        }
        else
        {
            // いいえ
            fieldUICanvasPopUp_.transform.Find("Select/Icon").transform.localPosition = new Vector3(40.0f, -60.0f, 0.0f);
        }
    }

}
