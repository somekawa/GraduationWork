﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

// クエスト管理スクリプト

public class QuestMng : MonoBehaviour
{
    // 現在の画面状態
    enum NOWPAGE
    {
        NON,            // 入室時画面
        LOOK_QUEST,     // クエストを見るの画面
        REPORT_QUEST    // クエスト報告画面
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

    private List<Quest> questList_;                  // クエストリスト
	private List<Transform> questUIInstanceList_;    // クエスト用UIのインスタンスを入れる
    private Transform questContent_;                 // クエストを表示するUIのコンテンツ
    private Text questInfoText_;                     // クエスト情報の表示先テキスト
    private GameObject questOrderButton_;            // クエスト受注ボタン
    private GameObject questReportButton_;           // クエスト報告ボタン
    private GameObject backButton_;                  // 前の画面に戻るボタン
    private GameObject questCanvas_;                 // クエストキャンバス
    private QuestButton[] questButton_;

    private int totalQuestNum_;                      // 全クエストの数
    private int questNum_;                           // 選択中のクエスト番号を保存する変数

    private Guild guild_ = null;                     // ギルドスクリプトのインスタンス

    private NOWPAGE nowPage_ = NOWPAGE.NON;          // 現在の画面状態

    // Excelからのデータ読み込み
    private GameObject DataPopPrefab_;
    private QuestInfo popQuestInfo_;

    void Start()
	{
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
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
        guild_ = new Guild();

        //　クエスト情報の表示先テキスト
        questInfoText_ = questUI.transform.Find("InfomationPanel/Text").GetComponent<Text>();
        questOrderButton_ = questUI.transform.Find("OrderButton").gameObject;
        questReportButton_ = questUI.transform.Find("ReportButton").gameObject;
        backButton_ = questUI.transform.Find("BackButton").gameObject;

        questList_ = new List<Quest>();
		questUIInstanceList_ = new List<Transform>();

		//　クエスト表示用UIのコンテンツ
		questContent_ = transform.Find("QuestUI/Background/Scroll View/Viewport/Content");

        questButton_ = new QuestButton[tmpQuestNum];

        for (var i = 0; i < tmpQuestNum; i++)
		{
			//　クエスト用のUIプレハブを生成する
			var questUIInstance = Instantiate(questPrefab, questContent_);

            // クエスト番号を設定する
            questButton_[i] = questUIInstance.GetComponent<QuestButton>();
            questButton_[i].SetQuestNum(i);

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
		for (var i = 0; i < totalQuestNum_; i++)
		{
            Debug.Log("進行度"+popQuestInfo_.param[i].eventNum);
        }

        questCanvas_ = GameObject.Find("QuestCanvas");
    }

    // クエスト掲示板を見るとき
    public void ClickLookQuest()
	{
        nowPage_ = NOWPAGE.LOOK_QUEST;
        questCanvas_.gameObject.SetActive(false);
        questUI.SetActive(!questUI.activeSelf);
        questOrderButton_.SetActive(false);     // 受注ボタンは非表示
        questReportButton_.SetActive(false);    // 報告ボタンは非表示
        backButton_.SetActive(true);
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
            // プレハブのインスタンス
            var prefab = Instantiate(completePrefab);
            // クエスト番号の設定
            prefab.GetComponent<CompleteQuest>().SetMyNum(questNum_);

            // クエストクリアを確認するスクリプトのリストに登録する
            QuestClearCheck.SetOrderQuestsList(prefab);

            // クエストの受注でイベントが進行するか判断する
            guild_.GuildQuestEvent(questNum_);

            Debug.Log("クエストを受注しました");

            // 受注ボタンを非表示にする
            questOrderButton_.SetActive(false);
        }
    }

    // クリアクエストの報告画面を出す
    public void ClickReportQuest()
    {
        nowPage_ = NOWPAGE.REPORT_QUEST;
        questCanvas_.gameObject.SetActive(false);

        ClearQuestUpdate();

        questOrderButton_.SetActive(false);     // 受注ボタンは非表示
        questReportButton_.SetActive(false);    // 報告ボタンは非表示
        backButton_.SetActive(true);
    }

    // クリアクエストの報告をする
    public void ClickReportButton()
    {
        //@ 報酬処理
        //@ そのクエストに対するクリア回数を+する


        // CompleteQuestのクリア報告用リストから報告した番号のオブジェクトを削除する
        QuestClearCheck.SetClearedQuestsList(questNum_);
        // staticで生成していたプレハブを削除する
        var tmp = GameObject.FindGameObjectsWithTag("Quest");
        for(int i = 0; i < tmp.Length; i++)
        {
            if(int.Parse(tmp[i].name) == questNum_)
            {
                Destroy(tmp[i]);
                tmp[i] = null;
            }
        }

        questReportButton_.SetActive(false);    // 報告ボタン非表示

        ClearQuestUpdate();

        Debug.Log("クエストを報告しました");
    }

    // クリアクエストの更新(報告時にも非表示切り替えが発生するため)
    private void ClearQuestUpdate()
    {
        // クリアしたやつが上に来るようにヒエラルキー順を並び替える
        var tmp = QuestClearCheck.GetClearedQuestsList();
        for (int i = 0; i < tmp.Count; i++)      // クリアしたクエスト分回す
        {
            for (int k = 0; k < questContent_.childCount; k++)  // 全体のクエスト分回す
            {
                // 番号が一致してたら、ヒエラルキー順を先頭にもってくる
                if (int.Parse(tmp[i].name) == questButton_[k].GetQuestNum())
                {
                    questContent_.GetChild(k).transform.SetSiblingIndex(0);
                }
            }
        }

        // QuestUIを一度、全て表示状態にする
        questUI.SetActive(true);

        // クリアしたクエスト以外は非表示にする(questContent_の子供を見る)
        for (int k = 0; k < questContent_.childCount; k++)
        {
            if (tmp.Count < k + 1)   // 単純な個数で考えるからk+1で記述
            {
                questContent_.GetChild(k).gameObject.SetActive(false);
            }
        }
    }

    public void ClickBackButton()
    {
        // 前の画面に戻す
        nowPage_ = NOWPAGE.NON;
        questCanvas_.gameObject.SetActive(true);
        questUI.SetActive(!questUI.activeSelf);
        Debug.Log("戻るボタンが押されました");
    }

    //　クエストを返す
    public Quest GetQuest(int num)
	{
		return questList_[num];
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
        Debug.Log("QuestMngで" + num + "を受け取りました");

        if(nowPage_ == NOWPAGE.LOOK_QUEST)
        {
            // クエストを選択したため受注ボタン表示
            questOrderButton_.SetActive(true);
        }
        else if(nowPage_ == NOWPAGE.REPORT_QUEST)
        {
            // クエストを選択したため報告ボタン表示
            questReportButton_.SetActive(true);
        }
    }
}