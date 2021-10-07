
// HouseBaseを継承している町長の家を管理するクラス
public class MayorHouse : HouseBase
{
    public override bool CheckEvent()
    {
        // イベント発生
        if (EventMng.GetChapterNum() == 0)
        {
            EventMng.SetChapterNum(1, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        return false;
    }
}
