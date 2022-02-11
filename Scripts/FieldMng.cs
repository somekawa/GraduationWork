using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public AudioClip BGM_search;
    public AudioClip BGM_normalButtle;
    public AudioClip BGM_bossButtle;

    private AudioSource audios;
    private GameObject jackObj_;

    // 画面状態一覧
    public enum MODE
    {
        NON,
        SEARCH,     // 探索中
        BUTTLE,     // 戦闘中
        MENU,       // メニュー画面中
        FORCEDBUTTLE,   // 強制戦闘中(壁との衝突時に切り替わる)
        MAX
    }

    public static MODE nowMode = MODE.SEARCH;      // 現在のモード
    public static MODE oldMode = MODE.NON;         // 前のモード

    public static bool stopEncountTimeFlg = false; // アイテムの効果で一時的にエンカウントを止める
    private bool stopEncountTimeFlgOld_ = false;   // 1フレーム前と比較する
    private float toButtleTime_ = 6.0f;            // 6秒経過でバトルへ遷移する
    private float time_ = 0.0f;                    // 現在の経過時間
    private float keepTime_ = 0.0f;                // 現在のエンカウントまでの時間を一時保存しておく

    private UnitychanController player_;           // プレイヤー情報格納用
    private CameraMng cameraMng_;

    private TMPro.TextMeshProUGUI titleInfo_;      // 宝箱か壁かで表示内容を変更する
    private TMPro.TextMeshProUGUI getChestsInfo_;  // 宝箱から獲得したアイテム内容の表示先

    // string->オブジェクト名,bool->クリア済み判定(falseは未クリア)
    public static List<(string, bool)> treasureList         = new List<(string, bool)>();
    public static List<(string, bool)> forcedButtleWallList = new List<(string, bool)>();

    // Excel情報
    private GameObject DataPopPrefab_;
    private ChestList  popChestList_;

    void Start()
    {
        // 現在のシーンをFIELDとする
        SceneMng.SetNowScene((SceneMng.SCENE)UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().Init();

        audios = GetComponent<AudioSource>();

        // イベントが発生するか確認する
        if (EventMng.GetChapterNum() == 8 && SceneMng.nowScene == SceneMng.SCENE.FIELD0)
        {
            // 進行度8 かつ 豊作の森
            EventMng.SetChapterNum(8, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 13 && SceneMng.nowScene == SceneMng.SCENE.FIELD1)
        {
            // 進行度13 かつ ヴェステ砂漠
            EventMng.SetChapterNum(13, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 16 && SceneMng.nowScene == SceneMng.SCENE.FIELD2)
        {
            // 進行度16 かつ Field3
            EventMng.SetChapterNum(16, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if(EventMng.GetChapterNum() == 19 && SceneMng.nowScene == SceneMng.SCENE.FIELD3)
        {
            // 進行度19 かつ Field4
            EventMng.SetChapterNum(19, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else if (EventMng.GetChapterNum() == 22 && SceneMng.nowScene == SceneMng.SCENE.FIELD4)
        {
            // 進行度22 かつ Field5
            EventMng.SetChapterNum(22, SceneMng.SCENE.CONVERSATION);
            nowMode = MODE.NON;
        }
        else
        {
            nowMode = MODE.SEARCH;
            audios.clip = BGM_search;
            audios.Play();
        }

        // DesertFieldかつ「オアシスを甦らせて」のクエストを達成後なら
        // クエスト達成後の会話で進行度は15となる
        if ((SceneMng.nowScene == SceneMng.SCENE.FIELD1) && EventMng.GetChapterNum() >= 15)
        {
            // Oasisオブジェクトを含めた全てのFieldMapオブジェクトをtrueにする
            var tmp = GameObject.Find("FieldMap").transform;
            for (int i = 0; i < tmp.childCount; i++)
            {
                tmp.GetChild(i).gameObject.SetActive(true);
            }
        }

        // Field3かつ「ゴーレム大量発生」のクエスト中(未達成)なら
        var quest = QuestClearCheck.GetOrderQuestsList();
        for(int i = 0; i < quest.Count; i++)
        {
            if(int.Parse(quest[i].Item1.name) == 5 && !quest[i].Item2)
            {
                if ((SceneMng.nowScene == SceneMng.SCENE.FIELD2))
                {
                    // エンカウントの速度を2倍にする(数字的には1/2にする)
                    toButtleTime_ /= 2.0f;
                    break;  // 処理を抜ける
                }
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

        cameraMng_.MainCameraPosInit();
        cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ

        // WarpField.csの初期化関数を先に呼ぶ
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // イベント戦発生用の宝箱/壁情報を取得する
        CheckWallAndChestActive("ButtleWall", forcedButtleWallList);
        CheckWallAndChestActive("Chests", treasureList);

        // Chest.xlsから宝箱内容の取得を行う
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        // StreamingAssetsからAssetBundleをロードする
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle開く");
        // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // 不要になったAssetBundleのメタ情報をアンロードする
        assetBundle.Unload(false);
        Debug.Log("破棄");

        popChestList_ = DataPopPrefab_.GetComponent<PopList>().GetData<ChestList>(PopList.ListData.CHEST);

        // 内容のタイトル
        titleInfo_ = fieldUICanvasPopUp_.transform.Find("TitleInfo").GetComponent<TMPro.TextMeshProUGUI>();
        // 宝箱の文字描画先
        getChestsInfo_ = fieldUICanvasPopUp_.transform.Find("GetChestsInfo").GetComponent<TMPro.TextMeshProUGUI>();

        // ステータスアップを消すか判定する
        if(!SceneMng.GetFinStatusUpTime().Item2)
        {
            for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].DeleteStatusUpByCook();
            }
        }

        GameObject.Find("FieldMap/MateriaPoints").GetComponent<DropFieldMateria>().Init();

        jackObj_ = GameObject.Find("Jack").gameObject;
        jackObj_.SetActive(false);
    }

    // クエストの受注状況に合わせて、壁や宝箱のアクティブ状態を判別する
    private void CheckWallAndChestActive(string parentName,List<(string,bool)> list)
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

        // 宝箱/壁情報の登録
        for (int i = 0; i < objChild.Length; i++)
        {
            bool addFlag = true;
            // そのFieldを初めて訪れたか確認する
            foreach (var tmpList in list)
            {
                // リスト内にある名前と、登録しようとしているオブジェクト名が1つでも一致していたら
                if (tmpList.Item1 == objChild[i].name)
                {
                    addFlag = false;
                    break;
                }
            }

            // 登録フラグがfalseになっていたらfor文からも抜ける
            if (!addFlag)
            {
                break;
            }

            // 初回登録
            list.Add((objChild[i].name, false));
        }

        // 宝箱/壁の個数でfor文を回す
        for (int i = 0; i < objChild.Length; i++)
        {
            // 最初はfalseにする
            objChild[i].SetActive(false);

            // 名前が[クエスト番号 - 配置番号]となっているから、ここで分割してあげる
            string[] arr = objChild[i].name.Split('-');

            // 受注中クエスト個数でfor文を回す
            for (int k = 0; k < orderList.Count; k++)
            {
                if (arr[0] != orderList[k].Item1.name)
                {
                    continue;
                }

                for (int a = 0; a < list.Count; a++)
                {
                    if (objChild[i].name == list[a].Item1)
                    {
                        // 名前一致時の処理
                        // クリア済みクエストなら宝箱/壁を非アクティブへ、未クリアなら宝箱/壁をアクティブへ
                        objChild[i].SetActive(!list[a].Item2);
                        break;
                    }
                }
            }

            if (arr[0] == "100" || arr[0] == "200") // クエストの要素に含まれない野良宝箱と壁の場合
            {
                for (int a = 0; a < list.Count; a++)
                {
                    // クリア済みクエストなら宝箱/壁を非アクティブへ、未クリアなら宝箱/壁をアクティブへ
                    objChild[i].SetActive(!list[a].Item2);
                    continue;
                }
            }
        }
    }

    void Update()
    {
        // アイテムの効果でエンカウント率停止
        if(stopEncountTimeFlg && stopEncountTimeFlg != stopEncountTimeFlgOld_)
        {
            // 値を保存しておく
            keepTime_ = toButtleTime_;
            stopEncountTimeFlgOld_ = stopEncountTimeFlg;
            toButtleTime_ = 20.0f;
            Debug.Log("一時的にエンカウントが停止しました");
        }

        if (nowMode == MODE.SEARCH)
        {
            if(SceneMng.nowScene == SceneMng.SCENE.FIELD4)
            {
                return;
            }

            // 探索中の時間加算処理
            if (player_.GetMoveFlag() && time_ < toButtleTime_)
            {
                time_ += Time.deltaTime;
            }
            else if (time_ >= toButtleTime_)
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;

                stopEncountTimeFlg = false;
                if(!stopEncountTimeFlg && stopEncountTimeFlgOld_)
                {
                    // 保存しておいた値を入れなおす
                    stopEncountTimeFlgOld_ = stopEncountTimeFlg;
                    toButtleTime_ = keepTime_;
                    Debug.Log("アイテムの効果がきれて、エンカウント再開しました");
                }

                // まだポップアップ中であったら、非アクティブにする
                if (fieldUICanvasPopUp_.activeSelf)
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
                if(SceneMng.nowScene == SceneMng.SCENE.FIELD4)
                {
                    jackObj_.SetActive(false);
                    cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ
                    SceneMng.MenuSetActive(true,true);
                    audios.Stop();
                    audios.clip = BGM_search;//流すクリップを切り替える
                    audios.Play();
                    nowMode = MODE.SEARCH;
                }
                break;

            case MODE.SEARCH:
                jackObj_.SetActive(false);
                cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ
                SceneMng.MenuSetActive(true, true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_search;//流すクリップを切り替える
                    audios.Play();
                }
                break;

            case MODE.BUTTLE:
                jackObj_.SetActive(true);
                cameraMng_.SetChangeCamera(true);    // サブカメラアクティブ
                SceneMng.MenuSetActive(false,true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_normalButtle;//流すクリップを切り替える
                    audios.Play();
                }
                break;

            case MODE.MENU:
                break;

            case MODE.FORCEDBUTTLE:
                jackObj_.SetActive(true);
                cameraMng_.SetChangeCamera(true);    // サブカメラアクティブ
                SceneMng.MenuSetActive(false,true);

                if (oldMode != MODE.MENU)
                {
                    audios.Stop();
                    audios.clip = BGM_bossButtle;//流すクリップを切り替える
                    audios.Play();
                }
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

    public bool GetStopEncountTimeFlg()
    {
        return stopEncountTimeFlg;
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
            getChestsInfo_.text = "逃げられない戦いだ…\n先に進みますか？";

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
