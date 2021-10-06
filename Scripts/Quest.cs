using UnityEngine;
using System.Collections;

public class Quest : object
{
    // クエストタイトル
    private string title_;

    // クエスト内容
    private string information_;

	//　クエストクラスのコンストラクタ
	public Quest(string title = "タイトルなし", string info = "内容なし")
	{
		this.title_ = title;
		this.information_ = info;
	}
	//　クエストのタイトルを返す
	public string GetTitle()
	{
		return title_;
	}
	//　クエスト情報を返す
	public string GetInformation()
	{
		return information_;
	}
}
