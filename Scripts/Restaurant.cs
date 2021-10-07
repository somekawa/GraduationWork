
// HouseBaseを継承している町長の家を管理するクラス
public class Restaurant : HouseBase
{
    public override bool CheckEvent()
    {
        int num = EventMng.GetChapterNum();

        // イベント発生
        if (num >= 3 && num <= 5)   // ギルドからor書店からorレストランから
        {
            bool tmpFlg = false;
            // リスト内にすでに名前があったら会話を入れない
            foreach (string list in QuestClearCheck.buildList)
            {
                if (list == "Restaurant")
                {
                    tmpFlg = true;
                }
            }

            if (!tmpFlg)
            {
                EventMng.SetChapterNum(5, SceneMng.SCENE.CONVERSATION);
                return true;
            }
        }

        return false;
    }
}
