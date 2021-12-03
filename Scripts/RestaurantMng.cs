using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantMng : MonoBehaviour
{
    [SerializeField]
    // レストランのメニューを表示するUI
    private GameObject restaurantMenuUI;

    [SerializeField]
    //　一つ一つの料理情報を表示するプレハブ
    private Transform cookPrefab;

    private GameObject restaurantCanvas_;      // 料理を注文するか、外に出るかのキャンバス
    private Transform content_;                // UIのコンテンツ

    private Text cookInfoText_;                // 料理情報の題名テキスト
    private Text statusUpInfoText_;            // ステータスアップの題名テキスト
    private Text numStatusUpInfoText_;         // アップする数字のテキスト
    private Text needFoodText_;                // 必要な素材のテキスト
    private Text haveFoodText_;                // 必要な素材の現在持ち数と、必要個数の数字テキスト
    private Text moneyText_;                   // 必要なお金のテキスト

    private (string, int)[] statusUp_ = new (string, int)[5];    // アップステータスの名前とアップ値をペアにした変数
    private readonly string[] statusUpStr_ = { "Attack", "MagicAttack", "Defence", "Speed", "Luck" };   // ステータス順に並べた文字列

    private int num_ = -1;

    private List<Transform> cookUIInstanceList_;    // UIのインスタンスを入れる
    private QuestButton[] button_;                  // 左側のボタン生成

    // Excelからのデータ読み込み
    private GameObject DataPopPrefab_;
    private Cook0 popCookInfo_;

    void Start()
    {
        restaurantCanvas_ = GameObject.Find("RestaurantCanvas");

        //　料理表示用UIのコンテンツ
        content_ = restaurantMenuUI.transform.Find("Background/Scroll View/Viewport/Content");
        cookInfoText_ = restaurantMenuUI.transform.Find("CookInfoText").GetComponent<Text>();
        statusUpInfoText_ = restaurantMenuUI.transform.Find("StatusUpInfoText").GetComponent<Text>();
        numStatusUpInfoText_ = restaurantMenuUI.transform.Find("StatusUpInfoText/StatusUpInfoNumText").GetComponent<Text>();
        needFoodText_ = restaurantMenuUI.transform.Find("NeedFoodText").GetComponent<Text>();
        haveFoodText_ = restaurantMenuUI.transform.Find("NeedFoodText/NeedFoodNumText").GetComponent<Text>();
        moneyText_ = restaurantMenuUI.transform.Find("MoneyText").GetComponent<Text>();

        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popCookInfo_ = DataPopPrefab_.GetComponent<PopList>().GetData<Cook0>(PopList.ListData.RESTAURANT);

        // Excelの行数からクエストの合計数を設定する
        var totalNum_ = popCookInfo_.param.Count;
        // 現在の進行度番号とクエストの進行度番号を比較して、現在の進行度番号以下のクエストの個数を数える
        int tmpCookNum = 0;
        for (int i = 0; i < totalNum_; i++)
        {
            if (EventMng.GetChapterNum() >= popCookInfo_.param[i].eventNum)
            {
                tmpCookNum++;
            }
        }

        cookUIInstanceList_ = new List<Transform>();
        button_ = new QuestButton[tmpCookNum];

        for (var i = 0; i < tmpCookNum; i++)
        {
            // 料理用のUIプレハブを生成する
            var uIInstance = Instantiate(cookPrefab, content_);
            cookUIInstanceList_.Add(uIInstance);

            // 番号を設定する
            button_[i] = uIInstance.GetComponent<QuestButton>();
            button_[i].SetQuestNum(i);

            // 左側の見出しに料理名を出す
            cookUIInstanceList_[i].Find("TitlePanel/Button/Text").GetComponent<TMPro.TextMeshProUGUI>().text =
                popCookInfo_.param[i].name;
        }

        // 初期化
        for(int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }
    }

    // 「料理を注文する」のボタン処理
    public void OnClickOrderButton()
    {
        Debug.Log("料理を注文するボタンを押下しました");
        restaurantCanvas_.SetActive(false);
        restaurantMenuUI.SetActive(true);
    }

    // 各メニューを開いた時の右下の「注文する」のボタン処理
    public void OnClickMenuOrderButton()
    {
        Debug.Log("注文ボタンを押下しました");

        // フラグがtrueなら食べれないようにする
        if(SceneMng.GetFinStatusUpTime())
        {
            Debug.Log("お腹がいっぱいで食べられないヨ！！");
            return;
        }

        // 繰り返し利用するので、一時変数に保存して使う
        var tmppop = popCookInfo_.param[num_];

        // そもそもお金が足りるか確認する
        if(tmppop.needMoney > SceneMng.GetHaveMoney())
        {
            Debug.Log("注文するのにお金が足りません");
            return;
        }

        // 素材が要求される料理かどうかで処理を分ける
        if (tmppop.needFood <= -1)
        {
            // 必要素材がないとき

            // お金の減少処理
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - tmppop.needMoney);

            // キャラのステータスアップ処理
            int[] tmp = { statusUp_[0].Item2, statusUp_[1].Item2, statusUp_[2].Item2, statusUp_[3].Item2, statusUp_[4].Item2 };
            for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].SetStatusUpByCook(tmp);
            }
        }
        else
        {
            // 必要素材があるとき
            // 素材が足りているか確認する
            if(Bag_Materia.materiaState[tmppop.needFood].haveCnt >= tmppop.needNum)
            {
                // 素材が足りる
                Debug.Log("素材が足りる");

                // お金の減少処理
                SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - tmppop.needMoney);

                // キャラのステータスアップ処理
                int[] tmp = { statusUp_[0].Item2, statusUp_[1].Item2, statusUp_[2].Item2, statusUp_[3].Item2, statusUp_[4].Item2 };
                for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
                {
                    SceneMng.charasList_[i].SetStatusUpByCook(tmp);
                }

                // 素材を減らす
                Bag_Materia.materiaState[tmppop.needFood].haveCnt -= tmppop.needNum;
            }
            else
            {
                // 素材が足りない
                Debug.Log("素材が足りない");
                return;
            }
        }

        BackMenu();

        // 効果が切れる時刻を保存する
        SceneMng.SetFinStatusUpTime();

    }

    public void SetSelectOrder(int num)
    {
        Debug.Log("RestaurantMngで" + num + "を受け取りました");

        // 繰り返し利用するので、一時変数に保存して使う
        var tmppop = popCookInfo_.param[num];

        // 料理名と説明を出す
        cookInfoText_.text = tmppop.name + "\n\n" + tmppop.info;

        // カンマ区切り
        var tmp = tmppop.statusUp.Split(',');

        List<string> tmpList = new List<string>();
        List<int> tmpListNum = new List<int>();

        // カンマ区切りになったものをさらにアンダーバーで区切る
        for (int i = 0; i < tmp.Length; i++)
        {
            var underbarSplit = tmp[i].Split('_');
            // アンダーバーで区切ったものを、別々のリストへ入れる
            tmpList.Add(underbarSplit[0]);
            tmpListNum.Add(int.Parse(underbarSplit[1]));
        }

        // 文字の初期化
        statusUpInfoText_.text = "Status Up";
        numStatusUpInfoText_.text = "";
        needFoodText_.text = "Need Food";
        haveFoodText_.text = "Have";

        // ステータスアップ部分を出す
        for (int i = 0; i < tmpList.Count; i++)
        {
            statusUpInfoText_.text += "\n・" + tmpList[i];
            numStatusUpInfoText_.text += "\n+" + tmpListNum[i];

            // 該当する場所に数字を入れる
            for(int k = 0; k < statusUp_.Length; k++)
            {
                // 名前一致
                if(statusUp_[k].Item1 == tmpList[i])
                {
                    statusUp_[k].Item2 = tmpListNum[i];
                    break;  // 1度一致するところを見つけたのでbreakで抜ける
                }
            }
        }

        // 必要素材の名称を出す(数字から素材名を取る)
        if(tmppop.needFood <= -1)
        {
            // needFoodの値が-1のときは必要素材が無しだから、表示はなしにする
            needFoodText_.text += "\nなし";
            haveFoodText_.text += "\n0/0";
        }
        else
        {
            // 必要素材と数について表示する
            needFoodText_.text += "\n" + Bag_Materia.materiaState[tmppop.needFood].name;
            // 所持数 / 必要数
            haveFoodText_.text += "\n" + Bag_Materia.materiaState[tmppop.needFood].haveCnt + "/" + tmppop.needNum;
        }

        // 必要なお金を出す
        moneyText_.text = tmppop.needMoney + "ビット";

        // 選択中のメニューを保存する
        num_ = num;
    }

    public void OnClickBackButton()
    {
        Debug.Log("戻るボタンを押下しました");
        BackMenu();
    }

    // 画面を戻す際に必要な処理
    private void BackMenu()
    {
        restaurantCanvas_.SetActive(true);
        restaurantMenuUI.SetActive(false);

        // 値の初期化
        for (int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }
    }
}
