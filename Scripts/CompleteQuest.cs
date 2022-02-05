using UnityEngine;

public class CompleteQuest : MonoBehaviour
{
    private string enemyName_ = "";          // 敵番号(Excelと一致させる)
    private int needSubjugation_ = -1;       // 必要討伐数
    private int finSubjugation_ = -1;        // 討伐完了数

    void Awake()
    {
        // シーンを跨いでも消えないオブジェクトにする
        // このオブジェクトが消えてしまうと、QuestClearCheck.csの受注中のクエストを保存しているリストがnullになってしまうから
        DontDestroyOnLoad(this);
    }

    public void SetMyNum(int num)
    {
        // 自分のオブジェクト名を、クエスト番号に変換
        // 他スクリプトがヒエラルキーからこのオブジェクトを検索するときに番号+タグ(Quest)で判別できるようにした
        this.gameObject.name = num.ToString();
        enemyName_ = "";             
        needSubjugation_ = -1;      
        finSubjugation_ = -1;       
}

    public void SetEnemyNameAndNeedSubjugation(string name,int need)
    {
        enemyName_ = name;
        needSubjugation_ = need;
        finSubjugation_ = 0;
    }

    public void SetFinSubjugation(int num)
    {
        finSubjugation_ += num;
        if(finSubjugation_ >= needSubjugation_)
        {
            // クエスト終了処理
            QuestClearCheck.QuestClear(int.Parse(this.gameObject.name));
        }
    }

    public int GetNeedSubjugation()
    {
        return needSubjugation_;
    }

    public string GetEnemyName()
    {
        return enemyName_;
    }

}