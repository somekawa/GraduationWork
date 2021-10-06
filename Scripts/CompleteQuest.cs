using UnityEngine;

// このスクリプトの内容をクエストのクリア条件のあるゲームオブジェクトにアタッチする

public class CompleteQuest : MonoBehaviour
{
    void Awake()
    {
        // シーンを跨いでも消えないオブジェクトにする
        DontDestroyOnLoad(this);
    }

    public void SetMyNum(int num)
    {
        // 自分のオブジェクト名を、クエスト番号に変換
        // 他スクリプトがヒエラルキーからこのオブジェクトを検索するときに番号+タグ(Quest)で判別できるようにした
        this.gameObject.name = num.ToString();
    }

    ////　クエストの管理スクリプト
    //[SerializeField]
    //private QuestMng questMng_ = null;
    ////　このクエストの番号
    //[SerializeField]
    //private int num;

    //void Update()
    //{
    //	if (Input.GetKeyDown("a"))
    //	{
    //		questMng_.SetQuestFlag(num);
    //		Debug.Log(questMng_.GetQuest(num).GetInformation());
    //	}
    //}
}