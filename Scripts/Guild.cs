
// HouseBaseを継承している町長の家を管理するクラス
using System.Collections.Generic;
using UnityEngine;

public class Guild : HouseBase
{
    private QuerySDEmotionalController npcController_;

    public override bool CheckEvent()
    {
        // イベント発生
        if (EventMng.GetChapterNum() == 1)
        {
            SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);   // 時間経過
            EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        else if (EventMng.GetChapterNum() == 7 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
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
    public void GuildQuestEvent(int questNum, bool clearFlg, Dictionary<int, int> clearNum)
    {
        if (!clearFlg)   // クエスト受注タイミング
        {
            switch (EventMng.GetChapterNum())
            {
                case 2: // 現在の進行度が2で、クエスト0番を受注したら
                    if (questNum == 0)
                    {
                        // シーン遷移無しで進行度を3にする。
                        EventMng.SetChapterNum(3, SceneMng.SCENE.NON);
                    }
                    break;
                case 12:// 現在の進行度が12で、クエスト4番を受注したら
                    if (questNum == 4)
                    {
                        EventMng.SetChapterNum(12, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 15:// 現在の進行度が15で、クエスト5番を受注したら
                    if (questNum == 5)
                    {
                        EventMng.SetChapterNum(15, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                default:
                    break;
            }
        }
        else　　　　　　// クエスト報告タイミング
        {
            switch (EventMng.GetChapterNum())
            {
                case 6: // 現在の進行度が6で、クエスト0番を達成したら
                    if (questNum == 0)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // 時間経過
                        EventMng.SetChapterNum(6, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 8: // 現在の進行度が8で、クエスト1番を達成したら
                    if (questNum == 1)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // 時間経過
                        EventMng.SetChapterNum(9, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 11:// 現在の進行度が11のとき
                    if (clearNum[2] >= 1 && clearNum[3] >= 1)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // 時間経過
                        EventMng.SetChapterNum(11, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 14:// 現在の進行度が14で、クエスト4番を達成したら
                    if (questNum == 4)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // 時間経過
                        EventMng.SetChapterNum(14, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // NPCの表情を変化させる
    public void ChangeNPCFace(int faceNum)
    {
        if(npcController_ == null)
        {
            npcController_ = GameObject.Find("HouseInterior/Guild/Query").GetComponent<QuerySDEmotionalController>();
        }

        npcController_.ChangeEmotion((QuerySDEmotionalController.QueryChanSDEmotionalType)faceNum,true);
    }
}
