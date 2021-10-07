using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// クリックされたクエストを取得

public class QuestMng : MonoBehaviour
{
	[SerializeField]
    //　全クエストの数
    private int totalQuestNum = 50;

    [SerializeField]
    //　クエスト画面UI
    private GameObject questUI;

    [SerializeField]
    //　一つ一つのクエスト情報を表示するプレハブ
    private Transform questPrefab;

    [SerializeField]
    // クエストを受注したときに生成されるプレハブ
    private GameObject completePrefab;

    private List<Quest> questList_;    //　クエストリスト
    private List<bool> questIsDone_;   //　クエストが終了しているかどうか(未使用?)

	private List<Transform> questUIInstanceList_;    //　クエスト用UIのインスタンスを入れる
    private Transform questContent_;                 //　クエストを表示するUIのコンテンツ
    private Text questInfoText_;                     //　クエスト情報の表示先テキスト
    private GameObject questOrderButton_;            //　クエスト受注ボタン

    private GameObject lookQuest_;   // クエストを見るボタン
    private int questNum_;           // 選択中のクエスト番号を保存する変数

    private Guild guild_ = null;     // ギルドスクリプトのインスタンス

    // Excelからのデータ読み込み
    private GameObject DataPopPrefab_;
    private QuestInfo popQuestInfo_;

    void Start()
	{
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popQuestInfo_ = DataPopPrefab_.GetComponent<PopList>().GetData<QuestInfo>(PopList.ListData.QUESTINFO);

        // ギルドスクリプトのインスタンス
        guild_ = new Guild();

        //　クエスト情報の表示先テキスト
        questInfoText_ = questUI.transform.Find("InfomationPanel/Text").GetComponent<Text>();
        questOrderButton_ = questUI.transform.Find("OrderButton").gameObject;

        questList_ = new List<Quest>();
		questUIInstanceList_ = new List<Transform>();
		questIsDone_ = Enumerable.Repeat(false, totalQuestNum).ToList<bool>();

		//　クエスト表示用UIのコンテンツ
		questContent_ = transform.Find("QuestUI/Background/Scroll View/Viewport/Content");

        for (var i = 0; i < totalQuestNum; i++)
		{
			//　クエスト用のUIプレハブを生成する
			var questUIInstance = Instantiate(questPrefab, questContent_);

            // クエスト番号を設定する
            questUIInstance.GetComponent<QuestButton>().SetQuestNum(i);

            questUIInstanceList_.Add(questUIInstance);
			//　サンプルの説明文を設定しクエストインスタンスを大量生産
			questList_.Add(new Quest("タイトル" + i, "テストの説明" + i));

			//　セットするコンポーネントを取得
			//var toggleCom = questUIInstanceList[i].Find("TitlePanel/Button").GetComponent<Button>();
			var toggleTextCom = questUIInstanceList_[i].Find("TitlePanel/Button/Text").GetComponent<Text>();
			//var informationTextCom = questUIInstanceList[i].Find("InformationPanel/Information").GetComponent<Text>();

			//　クエストの情報を表示ページと表示数を使って計算し取得する
			//var check = IsQuestFlag(i);
			var title = GetQuest(i).GetTitle();
			//var info = GetQuest(i).GetInformation();

			//　取得した情報をUIに反映させる
			//toggleCom.isOn = check;
			toggleTextCom.text = title;
			//informationTextCom.text = info;
		}
		//　それぞれのクエスト情報を表示（確認の為）
		for (var i = 0; i < totalQuestNum; i++)
		{
			Debug.Log(questList_[i].GetTitle() + ":" + questList_[i].GetInformation());
		}

		lookQuest_ = GameObject.Find("QuestCanvas");
	}

	// クエスト掲示板を見るとき
	public void ClickLookQuest()
	{
		lookQuest_.gameObject.SetActive(false);

		//　クエストの状態をアップデート
		//UpdateQuestData();

		questUI.SetActive(!questUI.activeSelf);

        // クエストを選択するまでは、受注ボタンは非表示にしておく
        questOrderButton_.SetActive(false);
    }

    // クエストを受注するとき
    public void ClickOrderQuest()
    {
        if(!QuestClearCheck.CanOrderNewQuest(questNum_))
        {
            // 既に3つクエストを受けているためor受注中のクエストと同じものを受けようとしたらreturn
            return;
        }
        else
        {
            // まだ3つ以下だから受けられる
            // プレハブのインスタンス
            var prefab = Instantiate(completePrefab);
            // クエスト番号の設定
            prefab.GetComponent<CompleteQuest>().SetMyNum(questNum_);

            // クエストクリアを確認するスクリプトのリストに登録する
            QuestClearCheck.SetList(prefab);

            // クエストの受注でイベントが進行するか判断する
            guild_.GuildQuestEvent(questNum_);

            Debug.Log("クエストを受注しました");

            // 受注ボタンを非表示にして画面を選択前に戻す
            lookQuest_.gameObject.SetActive(true);
            questUI.SetActive(!questUI.activeSelf);
            questOrderButton_.SetActive(false);
        }
    }

	//　クエストの状態をアップデート(トグルじゃなくなったから使わないけど、update自体は使えるかもしれないからコメントアウト)
	//void UpdateQuestData()
	//{
	//	for (int i = 0; i < questList.Count; i++)
	//	{
	//		questUIInstanceList[i].GetComponentInChildren<Toggle>().isOn = IsQuestFlag(i);
	//	}
	//}

	//　クエスト終了をセット
	public void SetQuestFlag(int num)
	{
		questIsDone_[num] = true;
	}

	//　クエストが終了しているかどうか
	public bool IsQuestFlag(int num)
	{
		return questIsDone_[num];
	}

	//　クエストを返す
	public Quest GetQuest(int num)
	{
		return questList_[num];
	}

	//　トータルクエスト数を返す
	public int GetTotalQuestNum()
	{
		return totalQuestNum;
	}

    public void SetSelectQuest(int num)
    {
        questNum_ = num;
        // クエスト説明を描画する(画面右側の所)
        questInfoText_.text = popQuestInfo_.param[num].info;
        Debug.Log("QuestMngで" + num + "を受け取りました");

        // クエストを選択したため受注ボタン表示
        questOrderButton_.SetActive(true);
    }
}