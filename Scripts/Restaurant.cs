
// HouseBaseを継承している町長の家を管理するクラス
public class Restaurant : HouseBase
{
    public override bool CheckEvent()
    {
        // イベント発生
        //if (EventMng.GetChapterNum() == 1)
        //{
        //    EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
        //    return true;
        //}
        return false;
    }
}
