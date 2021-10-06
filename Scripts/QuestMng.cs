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

    //　クエストリスト
    private List<Quest> questList;
    //　クエストが終了しているかどうか
    private List<bool> questIsDone;
    //　表示する個数
    private int num;
	//　クエスト用UIのインスタンスを入れる
	private List<Transform> questUIInstanceList;
	//　クエストを表示するUIのコンテンツ
	private Transform questContent;
    //　クエスト情報の表示先テキスト
    private Text questInfoText;
    //　クエスト受注ボタン
    private GameObject questOrderButton;

    // クエストを見るボタン
    private GameObject lookQuest_;

    // Excelからのデータ読み込み
    private GameObject DataPopPrefab_;
    private QuestInfo popQuestInfo_;

    private int questNum_;  // 選択中のクエスト番号を保存する変数

    void Start()
	{
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popQuestInfo_ = DataPopPrefab_.GetComponent<PopList>().GetData<QuestInfo>(PopList.ListData.QUESTINFO);


        //　クエスト情報の表示先テキスト
        questInfoText = questUI.transform.Find("InfomationPanel/Text").GetComponent<Text>();
        questOrderButton = questUI.transform.Find("OrderButton").gameObject;

        questList = new List<Quest>();
		questUIInstanceList = new List<Transform>();
		questIsDone = Enumerable.Repeat(false, totalQuestNum).ToList<bool>();

		//　クエスト表示用UIのコンテンツ
		questContent = transform.Find("QuestUI/Background/Scroll View/Viewport/Content");

        for (var i = 0; i < totalQuestNum; i++)
		{
			//　クエスト用のUIプレハブを生成する
			var questUIInstance = Instantiate(questPrefab, questContent);

            // クエスト番号を設定する
            questUIInstance.GetComponent<QuestButton>().SetQuestNum(i);

            questUIInstanceList.Add(questUIInstance);
			//　サンプルの説明文を設定しクエストインスタンスを大量生産
			questList.Add(new Quest("タイトル" + i, "テストの説明" + i));

			//　セットするコンポーネントを取得
			//var toggleCom = questUIInstanceList[i].Find("TitlePanel/Button").GetComponent<Button>();
			var toggleTextCom = questUIInstanceList[i].Find("TitlePanel/Button/Text").GetComponent<Text>();
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
			Debug.Log(questList[i].GetTitle() + ":" + questList[i].GetInformation());
		}

		lookQuest_ = GameObject.Find("QuestCanvas");
	}

	// クエスト掲示板を見るとき
	public void ClickLookQuest()
	{
		lookQuest_.gameObject.SetActive(false);

		//　クエストの状態をアップデート
		//UpdateQuestData();
		//　ShowQuestスクリプトのShow関数を呼び出し情報を書き換える
		questUI.SetActive(!questUI.activeSelf);

        // クエストを選択するまでは、受注ボタンは非表示にしておく
        questOrderButton.SetActive(false);
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
            var temp = Instantiate(completePrefab);
            // クエスト番号の設定
            temp.GetComponent<CompleteQuest>().SetMyNum(questNum_);

            // クエストクリアを確認するスクリプトのリストに登録する
            QuestClearCheck.SetList(temp);

            Debug.Log("クエストを受注しました");
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
		questIsDone[num] = true;
	}

	//　クエストが終了しているかどうか
	public bool IsQuestFlag(int num)
	{
		return questIsDone[num];
	}

	//　クエストを返す
	public Quest GetQuest(int num)
	{
		return questList[num];
	}

	//　トータルクエスト数を返す
	public int GetTotalQuestNum()
	{
		return totalQuestNum;
	}

    public void SetSelectQuest(int num)
    {
        questNum_ = num;
        questInfoText.text = popQuestInfo_.param[num].info;
        Debug.Log("QuestMngで" + num + "を受け取りました");

        // クエストを選択したため受注ボタン表示
        questOrderButton.SetActive(true);
    }
}