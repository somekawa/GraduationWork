
// HouseBaseを継承している町長の家を管理するクラス
public class Guild : HouseBase
{
    public override bool CheckEvent()
    {
        // イベント発生
        if (EventMng.GetChapterNum() == 1)
        {
            SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);   // 時間経過
            EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        else if(EventMng.GetChapterNum() == 7 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            // 時間帯を条件に入れないと、挨拶クエスト終了直後に発生する可能性が高い
            EventMng.SetChapterNum(7, SceneMng.SCENE.CONVERSATION); 
        }
        else if (EventMng.GetChapterNum() == 10 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            EventMng.SetChapterNum(10, SceneMng.SCENE.CONVERSATION);
        }
        return false;
    }

    // クエストを受注でイベントが進行するときに使用
    // QuestMngでインスタンスして呼び出す
    public void GuildQuestEvent(int questNum,bool clearFlg = false)
    {
        if (EventMng.GetChapterNum() == 2 && questNum == 0) // 現在の進行度が2で、クエスト0番を受注したら
        {
            // シーン遷移無し
            EventMng.SetChapterNum(3, SceneMng.SCENE.NON);
        }
        else if(EventMng.GetChapterNum() == 6 && questNum == 0) // 現在の進行度が6で、クエスト0番を達成したら
        {
            if(clearFlg)
            {
                SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // 時間経過
                EventMng.SetChapterNum(6, SceneMng.SCENE.CONVERSATION);
            }
        }
        else if(EventMng.GetChapterNum() == 9 && questNum == 1) // 現在の進行度が8で、クエスト1番を達成したら
        {
            if (clearFlg)
            {
                SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // 時間経過
                EventMng.SetChapterNum(9, SceneMng.SCENE.CONVERSATION);
            }
        }
    }
}
