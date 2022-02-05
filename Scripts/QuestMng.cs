using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

// クエスト管理スクリプト

// メインクエストとサブクエストの達成回数を保存する変数が必要(メインは1回クリアしたら終了,サブは何回でも可能)

public class QuestMng : MonoBehaviour
{
    // 現在の画面状態
    enum NOWPAGE
    {
        NON,            // 入室時画面
        LOOK_QUEST,     // クエストを見るの画面
        REPORT_QUEST,   // クエスト報告画面
        CANCEL_QUEST    // クエスト破棄画面
    }

    // クエストのタイプ
    enum QUESTTYPE
    {
        DELIVERY,       // 納品
        SUBJUGATION,    // 討伐
        OTHER,          // その他(だいたいのメインクエスト系)
    }

    [SerializeField]
    //　クエスト画面UI
    private GameObject questUI;

    [SerializeField]
    //　一つ一つのクエスト情報を表示するプレハブ
    private Transform questPrefab;

    [SerializeField]
    // クエストを受注したときに生成されるプレハブ
    private GameObject completePrefab;

	private List<Transform> questUIInstanceList_;    // クエスト用UIのインスタンスを入れる
    private Transform questContent_;                 // クエストを表示するUIのコンテンツ
    private Text questInfoText_;                     // クエスト情報の題名テキスト
    private Text questInfoText2_;                    // クエスト情報の詳細テキスト
    private Text questInfoText3_;                    // クエスト情報の報酬テキスト
    private Image stampImg_;                         // ハンコの画像
    private GameObject questOrderButton_;            // クエスト受注ボタン
    private GameObject questReportButton_;           // クエスト報告ボタン
    private GameObject questCancelButton_;           // クエスト破棄ボタン
    private GameObject backButton_;                  // 前の画面に戻るボタン
    private GameObject questCanvas_;                 // クエストキャンバス

    private GameObject popUpReward_;                 // クリア報酬のポップアップ
    private TMPro.TextMeshProUGUI rewardText_;       // クリア報酬のテキスト

    private GameObject deliveryBack_;                // 納品クエストの背景パネル
    private GameObject[] deliveryItemBox_ = new GameObject[2];  // 納品予定のアイテムを表示するアイテムボックス     

    private QuestButton[] questButton_;

    private int totalQuestNum_;                      // 全クエストの数
    private int questNum_;                           // 選択中のクエスト番号を保存する変数
    public static Dictionary<int, int> questClearCnt = new Dictionary<int, int>();      // キー:クエスト番号,値:クエスト達成回数
    public static Dictionary<int, float> rewardGradeUp = new Dictionary<int, float>();  // キー:クエスト番号,値:納品する大成功アイテム数に応じて数値が変化する

    private Guild guild_ = null;                     // ギルドスクリプトのインスタンス

    private NOWPAGE nowPage_ = NOWPAGE.NON;          // 現在の画面状態

    private int[] nowDeliveryNum_ = new int[2];      // 現在の納品予定数
    private IEnumerator[] rest_ = new IEnumerator[2];         // 納品処理のコルーチンを保存する

    private static bool onceFlg_ = false;

    // Excelからのデータ読み込み
    private GameObject DataPopPrefab_;
    private QuestInfo popQuestInfo_;

    private void OnEnable()
    {
        if(guild_ == null)
        {
            guild_ = new Guild();
        }
        guild_.ChangeNPCFace(5);    // 表情変更->笑顔
    }

    void Start()
	{
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        // StreamingAssetsからAssetBundleをロードする
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle開く");
        // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // 不要になったAssetBundleのメタ情報をアンロードする
        assetBundle.Unload(false);
        Debug.Log("破棄");

        popQuestInfo_ = DataPopPrefab_.GetComponent<PopList>().GetData<QuestInfo>(PopList.ListData.QUESTINFO);

        // Excelの行数からクエストの合計数を設定する
        totalQuestNum_ = popQuestInfo_.param.Count;
        // 現在の進行度番号とクエストの進行度番号を比較して、現在の進行度番号以下のクエストの個数を数える
        int tmpQuestNum = 0;
        for(int i = 0; i < totalQuestNum_; i++)
        {
            if(EventMng.GetChapterNum() >= popQuestInfo_.param[i].eventNum)
            {
                tmpQuestNum++;
            }
        }

        // ギルドスクリプトのインスタンス
        if (guild_ == null)
        {
            guild_ = new Guild();
        }

        // クエスト情報の表示先テキスト
        questInfoText_ = questUI.transform.Find("InfomationPanel/TitleText").GetComponent<Text>();
        questInfoText2_ = questUI.transform.Find("InfomationPanel/DetailText").GetComponent<Text>();
        questInfoText3_ = questUI.transform.Find("InfomationPanel/RewardText").GetComponent<Text>();
        // ハンコの画像
        stampImg_ = questUI.transform.Find("InfomationPanel/Stamp").GetComponent<Image>();

        questOrderButton_ = questUI.transform.Find("OrderButton").gameObject;
        questReportButton_ = questUI.transform.Find("ReportButton").gameObject;
        questCancelButton_ = questUI.transform.Find("CancelButton").gameObject;
        backButton_ = questUI.transform.Find("BackButton").gameObject;

		questUIInstanceList_ = new List<Transform>();

		//　クエスト表示用UIのコンテンツ
		questContent_ = transform.Find("QuestUI/Background/Scroll View/Viewport/Content");

        questButton_ = new QuestButton[tmpQuestNum];

        popUpReward_ = transform.Find("QuestUI/PopUp").gameObject;
        rewardText_ = transform.Find("QuestUI/PopUp/GetReward").GetComponent<TMPro.TextMeshProUGUI>();

        deliveryBack_ = transform.Find("QuestUI/DeliveryPanel").gameObject;
        deliveryItemBox_[0] = deliveryBack_.transform.Find("ItemBox").gameObject;
        deliveryItemBox_[1] = deliveryBack_.transform.Find("ItemBox (1)").gameObject;

        for (var i = 0; i < tmpQuestNum; i++)
		{
			//　クエスト用のUIプレハブを生成する
			var questUIInstance = Instantiate(questPrefab, questContent_);

            // クエスト番号を設定する
            questButton_[i] = questUIInstance.GetComponent<QuestButton>();
            questButton_[i].SetQuestNum(i);

            questUIInstanceList_.Add(questUIInstance);

            // 5文字以上のタイトル情報のとき
            if(popQuestInfo_.param[i].info.Length >= 5)
            {
                //　セットするコンポーネントを取得して情報を反映させる
                questUIInstanceList_[i].Find("TitlePanel/Button/Text").GetComponent<TMPro.TextMeshProUGUI>().text =
                    popQuestInfo_.param[i].info.Substring(0, 5) + "・・";
            }
            else
            {
                questUIInstanceList_[i].Find("TitlePanel/Button/Text").GetComponent<TMPro.TextMeshProUGUI>().text =
                    popQuestInfo_.param[i].info;
            }
        }

        // 初回のみ登録する
        if(!onceFlg_)
        {
            onceFlg_ = true;
            for (int i = 0; i < totalQuestNum_; i++)    // 全クエスト分で回す
            {
                if(!questClearCnt.ContainsKey(i))
                {
                    questClearCnt.Add(i, 0);   // クリア回数を0回で設定
                }
            }
        }

        questCanvas_ = GameObject.Find("QuestCanvas");
    }

    // クエスト掲示板を見るとき
    public void ClickLookQuest()
	{
        SceneMng.SetSE(0);
        nowPage_ = NOWPAGE.LOOK_QUEST;
        questCanvas_.gameObject.SetActive(false);
        questUI.SetActive(!questUI.activeSelf);
        questOrderButton_.SetActive(false);     // 受注ボタンは非表示
        questReportButton_.SetActive(false);    // 報告ボタンは非表示
        questCancelButton_.SetActive(false);    // 破棄ボタンは非表示
        backButton_.SetActive(true);

        // 受注中のクエストと報告待ちクエストを合体させたリストをつくる
        List<GameObject> mixList = new List<GameObject>();
        foreach(var list in QuestClearCheck.GetOrderQuestsList())
        {
            mixList.Add(list.Item1);
        }

        foreach (var list in QuestClearCheck.GetClearedQuestsList())
        {
            mixList.Add(list);
        }

        // QuestUIと、その子を全て表示状態にする
        questUI.SetActive(true);

        for (int k = 0; k < questContent_.childCount; k++)  // 全体のクエスト分回す
        {
            questContent_.GetChild(k).gameObject.SetActive(true);   // 表示状態から調べる

            // 合体させたリストに載っていたものは非表示にする
            for (int i = 0; i < mixList.Count; i++)
            {
                if (int.Parse(mixList[i].name) == questButton_[k].GetQuestNum())
                {
                    questContent_.GetChild(k).gameObject.SetActive(false);
                }
            }

            // 1度クリアしたメインクエストも非表示にする
            if (popQuestInfo_.param[k].type == "main" && questClearCnt[k] >= 1)
            {
                questContent_.GetChild(k).gameObject.SetActive(false);
            }
        }

        // ハンコを非アクティブにする
        stampImg_.gameObject.SetActive(false);
    }

    // クエストを受注するとき
    public void ClickOrderButton()
    {
        if(!QuestClearCheck.CanOrderNewQuest(questNum_))
        {
            // 既に3つクエストを受けているためor受注中のクエストと同じものを受けようとしたらreturn
            return;
        }
        else
        {
            // まだ3つ以下だから受けられる
            SceneMng.SetSE(0);

            // 指定したキーが存在するかどうか
            if (!rewardGradeUp.ContainsKey(questNum_))
            {
                rewardGradeUp.Add(questNum_, 0.0f);
            }

            // 納品クエストかを確認する
            if (popQuestInfo_.param[questNum_].questType == (int)QUESTTYPE.DELIVERY)
            {
                deliveryBack_.SetActive(true);

                // カンマ区切りにする(前の数字が成功,後ろの数字が大成功時のアイテムの番号)
                var split = popQuestInfo_.param[questNum_].delivery.Split(',');
                int[] numSplit = new int[split.Length];
                for(int i = 0; i < split.Length; i++)
                {
                    numSplit[i] = int.Parse(split[i]);

                    // 画像を出す
                    deliveryItemBox_[i].transform.Find("ItemIcon").GetComponent<Image>().sprite = Bag_Item.itemState[numSplit[i]].image.sprite;

                    // 所持数を出す
                    deliveryItemBox_[i].transform.Find("ItemNum").GetComponent<Text>().text = Bag_Item.itemState[numSplit[i]].cntText.text;

                    // コルーチンを初期化
                    rest_[i] = DeliveryCoroutine(i);
                    StartCoroutine(rest_[i]); // コルーチンを呼び出す
                }
            }
            else
            {
                // まだ3つ以下だから受けられる
                OrderQuestCommon();

                // 受注したのが炎マテリアの合成クエスト(番号が3)のとき
                if(popQuestInfo_.param[questNum_].num == 3)
                {
                    // Magic番号の番号を見て、1なら「炎単体小」の魔法なのですでに魔法を取得しているからクリア状態にする
                    for(int i = 1; i < Bag_Magic.magicObject.Length; i++)
                    {
                        // 空白だったら抜ける
                        if(Bag_Magic.magicObject[i] == null)
                        {
                            break;
                        }

                        var tmp = Bag_Magic.data[int.Parse(Regex.Replace(Bag_Magic.magicObject[i].name, @"[^0-9]", ""))];

                        if(tmp.name == "炎単体小")
                        {
                            // クリア状態にする
                            QuestClearCheck.QuestClear(questNum_);
                            break;  // これ以上for文を回す必要がないから抜ける
                        }
                    }
                }
            }

            guild_.ChangeNPCFace(5);    // 表情変更->笑顔
        }
    }

    // 納品数のいろいろするコルーチン
    private IEnumerator DeliveryCoroutine(int num)
    {
        var slider = deliveryItemBox_[num].transform.Find("Slider").GetComponent<Slider>();
        // スライダーの位置を初期化する
        slider.value = 0.0f;

        var deliveryNum = deliveryItemBox_[num].transform.Find("DeliveryNum").GetComponent<Text>();
        var itemNumText = deliveryItemBox_[num].transform.Find("ItemNum").GetComponent<Text>();
        var charaHaveItemCnt = int.Parse(itemNumText.text);
        var buttonImage = deliveryBack_.transform.Find("Button").GetComponent<Image>();
        var button = deliveryBack_.transform.Find("Button").GetComponent<Button>();

        // クエスト情報から数字だけ抜き取る(= 納品数)
        string str = Regex.Replace(popQuestInfo_.param[questNum_].detail, @"[^0-9]", "");

        // 納品数 < 所持数ならば、納品数をスライダーの上限とする
        if(int.Parse(str) < int.Parse(itemNumText.text))
        {
            // スライダーのmax値を納品数にする
            slider.maxValue = float.Parse(str);
        }
        else
        {
            // スライダーのmax値を現在所持しているアイテム数にする
            slider.maxValue = float.Parse(itemNumText.text);
        }

        bool flag = false;
        while (!flag)
        {
            yield return null;

            // 納品予定の数をスライダーのスライド量で変更する
            deliveryNum.text = slider.value.ToString();

            // 画像右下の数字をスライダーが増えた分だけマイナスする
            itemNumText.text = (charaHaveItemCnt - int.Parse(deliveryNum.text)).ToString();

            // 納品予定数を保存する
            nowDeliveryNum_[num] = int.Parse(deliveryNum.text);
            Debug.Log("現在の納品予定数は、、" + nowDeliveryNum_[num]);

            // 成功と大成功の納品予定数を合わせる
            int tmpNum = 0; 
            for (int i = 0; i < 2; i++)
            {
                tmpNum += nowDeliveryNum_[i];
            }
            // 合計の納品予定数とmaxで比較する
            if (int.Parse(str) == tmpNum)
            {
                // 納品数がぴったりのとき
                button.interactable = true;
                buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                // 納品数が合わないとき(多すぎてもこっちに入る)
                button.interactable = false;
                buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
        }
    }

    // 納品の確定
    public void ClickDeliveryButton()
    {
        SceneMng.SetSE(0);
        Debug.Log("納品確定ボタン");

        // カンマ区切りにする(前の数字が成功,後ろの数字が大成功時のアイテムの番号)
        var split = popQuestInfo_.param[questNum_].delivery.Split(',');
        int[] numSplit = new int[split.Length];

        // 報酬のグレードアップ処理
        var persent = (int)((float)nowDeliveryNum_[1] / float.Parse(Regex.Replace(popQuestInfo_.param[questNum_].detail, @"[^0-9]", "")) * 100.0f);
        // 4段階ぐらい差をつける
        if(persent >= 80 && persent <= 100)
        {
            // 80 % ～ 100 % --報酬1.8倍
            rewardGradeUp[questNum_] = 1.8f;
        }
        else if (persent >= 50 && persent < 80)
        {
            // 50 % ～ 79 % --報酬1.5倍
            rewardGradeUp[questNum_] = 1.5f;
        }
        else if (persent >= 1 && persent < 50)
        {
            // 1 %  ～ 49 % --報酬1.2倍
            rewardGradeUp[questNum_] = 1.2f;
        }
        else
        {
            // 0 % --報酬0倍
        }

        for (int i = 0; i < split.Length; i++)
        {
            numSplit[i] = int.Parse(split[i]);
            Bag_Item.itemState[numSplit[i]].haveCnt -= (int)deliveryItemBox_[i].transform.Find("Slider").GetComponent<Slider>().value;
            StopCoroutine(rest_[i]); //一時停止
            rest_[i] = null;         //リセット
            nowDeliveryNum_[i] = 0; // 納品予定数の初期化
        }

        OrderQuestCommon();

        // パネルを非表示にする
        deliveryBack_.SetActive(false);

        // クリア状態にする
        QuestClearCheck.QuestClear(questNum_);
    }

    public void ClickCancelDeliveryButton()
    {
        for (int i = 0; i < 2; i++)
        {
            StopCoroutine(rest_[i]); //一時停止
            rest_[i] = null; //リセット
            nowDeliveryNum_[i] = 0; // 納品予定数の初期化
        }


        // パネルを非表示にする
        deliveryBack_.SetActive(false);
    }

    // 受注処理の共通部分
    private void OrderQuestCommon()
    {
        // プレハブのインスタンス
        var prefab = Instantiate(completePrefab);
        // クエスト番号の設定
        prefab.GetComponent<CompleteQuest>().SetMyNum(questNum_);

        // クエストクリアを確認するスクリプトのリストに登録する
        QuestClearCheck.SetOrderQuestsList(prefab);

        // クエストの受注でイベントが進行するか判断する
        guild_.GuildQuestEvent(questNum_, false, questClearCnt);

        Debug.Log("クエストを受注しました");

        // 受注ボタンを非表示にする
        questOrderButton_.SetActive(false);
        // 受注したものを左側のリストから非表示にする
        questButton_[questNum_].gameObject.SetActive(false);

        // 討伐クエストの場合、フィールドと敵番号と討伐数を設定する
        if(popQuestInfo_.param[questNum_].questType == (int)QUESTTYPE.SUBJUGATION)
        {
            string[] split = popQuestInfo_.param[questNum_].delivery.Split('_');
            // 引数にはdeliveriの欄に書いた、敵名前_討伐必要数を分割して入れる
            prefab.GetComponent<CompleteQuest>().SetEnemyNameAndNeedSubjugation(split[0], int.Parse(split[1]));
        }

    }

    // クリアクエストの報告画面を出す
    public void ClickReportQuest()
    {
        SceneMng.SetSE(0);
        nowPage_ = NOWPAGE.REPORT_QUEST;
        questCanvas_.gameObject.SetActive(false);

        QuestUpdate(false);

        questOrderButton_.SetActive(false);     // 受注ボタンは非表示
        questReportButton_.SetActive(false);    // 報告ボタンは非表示
        questCancelButton_.SetActive(false);    // 破棄ボタンは非表示
        backButton_.SetActive(true);

        // ハンコを非アクティブにする
        stampImg_.gameObject.SetActive(false);
    }

    // クリアクエストの報告をする
    public void ClickReportButton()
    {
        SceneMng.SetSE(0);
        string text1 = "";
        string text2 = "";
        int addMoney = (int)(popQuestInfo_.param[questNum_].money * rewardGradeUp[questNum_]) - popQuestInfo_.param[questNum_].money;
        // 報酬処理
        // お金
        SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() + popQuestInfo_.param[questNum_].money + addMoney);
        // 素材
        if (popQuestInfo_.param[questNum_].materia != "")
        {
            var materia = popQuestInfo_.param[questNum_].materia.Split('_');
            var add = (int)(int.Parse(materia[1]) * rewardGradeUp[questNum_]) - int.Parse(materia[1]);
            text1 = "・素材 +" + add + "\n";
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>().MateriaGetCheck(int.Parse(materia[0]),  int.Parse(materia[1]) + add);
        }
        // アイテム
        if (popQuestInfo_.param[questNum_].item != "")
        {
            var item = popQuestInfo_.param[questNum_].item.Split('_');
            var add = (int)(int.Parse(item[1]) * rewardGradeUp[questNum_]) - int.Parse(item[1]);
            text1 = "・アイテム +" + add;
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>().ItemGetCheck(0, int.Parse(item[0]), int.Parse(item[1]) + add);
        }

        // ハンコをアクティブにする
        stampImg_.gameObject.SetActive(true);

        if (rewardGradeUp[questNum_] > 0.0f)
        {
            // 報酬内容を記載する
            rewardText_.text = popQuestInfo_.param[questNum_].reward + "\n追加報酬\n・" + addMoney + "バイト\n" + text1 + text2;
        }
        else
        {
            // 報酬内容を記載する
            rewardText_.text = popQuestInfo_.param[questNum_].reward;
        }

        // 数字の初期化
        rewardGradeUp[questNum_] = 0.0f;

        // ポップアップをアクティブにする
        popUpReward_.SetActive(true);

        // そのクエストに対するクリア回数を加算する
        questClearCnt[questNum_]++;
        Debug.Log(questNum_ + "番のクリア回数が" + questClearCnt[questNum_] + "になりました");

        // ポップアップが時間経過で消えるようにコルーチンを呼び出す
        StartCoroutine(PopUpMessage());

        // CompleteQuestのクリア報告用リストから報告した番号のオブジェクトを削除する
        QuestClearCheck.SetClearedQuestsList(questNum_);

        QuestDestroy();

        questReportButton_.SetActive(false);    // 報告ボタン非表示

        QuestUpdate(false);

        guild_.ChangeNPCFace(5);                // 表情変更->笑顔

        Debug.Log("クエストを報告しました");
    }

    private void QuestDestroy()
    {
        // staticで生成していたプレハブを削除する
        var tmp = GameObject.FindGameObjectsWithTag("Quest");
        for (int i = 0; i < tmp.Length; i++)
        {
            if (int.Parse(tmp[i].name) == questNum_)
            {
                Destroy(tmp[i]);
                tmp[i] = null;
            }
        }
    }

    // クエスト破棄の報告画面を出す
    public void ClickCancelQuest()
    {
        SceneMng.SetSE(0);
        // ハンコを非アクティブにする
        stampImg_.gameObject.SetActive(false);

        nowPage_ = NOWPAGE.CANCEL_QUEST;
        questCanvas_.gameObject.SetActive(false);

        QuestUpdate(true);
    }

    // クエスト破棄ボタンを押す
    public void ClickCancelButton()
    {
        if (popQuestInfo_.param[questNum_].type != "main")
        {
            SceneMng.SetSE(0);
            Debug.Log(questNum_ + "番のクエストを、破棄します");
            QuestClearCheck.CancelOrderQuest(questNum_);
            QuestDestroy();
            QuestUpdate(true); // 画面更新
        }
        else
        {
            Debug.Log(questNum_ + "番はメインクエストなので、破棄できません");
        }
    }


    private IEnumerator PopUpMessage()
    {
        float time = 0.0f;
        while (popUpReward_.activeSelf)
        {
            yield return null;

            if (time >= 1.5f)
            {
                popUpReward_.SetActive(false);
            }
            else
            {
                time += Time.deltaTime;
            }
        }

        // クエストの達成報告でイベントが進行するか判断する
        // ポップアップ後にイベントが発生してほしいからここで行う
        guild_.GuildQuestEvent(questNum_, true, questClearCnt);

    }

    private void QuestUpdate(bool isOrderQuestFlag)
    {
        questUI.SetActive(true);

        // すべてのボタンをfalseにする
        for (int k = 0; k < questContent_.childCount; k++)  // 全体のクエスト分回す
        {
            questContent_.GetChild(k).gameObject.SetActive(false);
        }

        if(isOrderQuestFlag)
        {
            // 受注中クエストの更新(報告時にも非表示切り替えが発生するため)
            var tmp = QuestClearCheck.GetOrderQuestsList();
            // リストに載ってたら表示にする(questContent_の子供を見る)
            for (int k = 0; k < questContent_.childCount; k++)  // 全体のクエスト分回す
            {
                for (int i = 0; i < tmp.Count; i++)
                {
                    if (int.Parse(tmp[i].Item1.name) == questButton_[k].GetQuestNum())
                    {
                        questContent_.GetChild(k).gameObject.SetActive(true);
                    }
                }
            }
        }
        else
        {
            // クリアクエストの更新(報告時にも非表示切り替えが発生するため)
            var tmp = QuestClearCheck.GetClearedQuestsList();

            // リストに載ってたら表示にする(questContent_の子供を見る)
            for (int k = 0; k < questContent_.childCount; k++)  // 全体のクエスト分回す
            {
                for (int i = 0; i < tmp.Count; i++)
                {
                    if (int.Parse(tmp[i].name) == questButton_[k].GetQuestNum())
                    {
                        questContent_.GetChild(k).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void ClickBackButton()
    {
        // 前の画面に戻す
        nowPage_ = NOWPAGE.NON;
        questCanvas_.gameObject.SetActive(true);
        questUI.SetActive(!questUI.activeSelf);
        // パネルを非表示にする
        deliveryBack_.SetActive(false);
        Debug.Log("戻るボタンが押されました");
    }

	//　トータルクエスト数を返す
	public int GetTotalQuestNum()
	{
		return totalQuestNum_;
	}

    public void SetSelectQuest(int num)
    {
        questNum_ = num;
        // クエスト説明を描画する(画面右側の所)
        questInfoText_.text = popQuestInfo_.param[num].info;
        questInfoText2_.text = popQuestInfo_.param[num].detail;
        questInfoText3_.text = "報酬\n" + popQuestInfo_.param[num].reward;

        Debug.Log("QuestMngで" + num + "を受け取りました");

        switch(nowPage_)
        {
            case NOWPAGE.NON:
                break;
            case NOWPAGE.LOOK_QUEST:
                // クエストを選択したため受注ボタン表示
                questOrderButton_.SetActive(true);
                break;
            case NOWPAGE.REPORT_QUEST:
                // クエストを選択したため報告ボタン表示
                questReportButton_.SetActive(true);
                // ハンコを非アクティブにする
                stampImg_.gameObject.SetActive(false);
                break;
            case NOWPAGE.CANCEL_QUEST:
                // サブクエスト破棄を選択したため破棄ボタン表示
                questCancelButton_.SetActive(true);
                break;
            default:
                Debug.Log("該当しないnowPage_番号です");
                break;
        }
    }
}