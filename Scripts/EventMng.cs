
public static class EventMng
{
    // 自由に店に出入りしたいときは99とかいれとくといいかも。本来は0
    private static int chapterNum = 6;   // 現在のチャプター進行度(0からスタート)

    // チャプター進行度の更新
    // 読み返し機能を作成するときには引数部分に該当するチャプター番号を入れるようにする
    public static void SetChapterNum(int num ,SceneMng.SCENE scene)
    {
        chapterNum = num;
        SceneMng.SceneLoad((int)scene);
    }

    public static int GetChapterNum()
    {
        return chapterNum;
    }

    // 進行上、その建物に入ってもいいか判断する
    // string hitName : ユニちゃんと当たり判定した建物
    public static string CheckEventHouse(string hitName)
    {
        if (hitName != "MayorHouse" && chapterNum == 0)
        {
            // チャプター0のときに、町長の家以外に当たった時の処理
            return "MayorHouse";
        }
        else if(hitName != "Guild" && chapterNum == 1)
        {
            // チャプター1のときに、ギルド以外に当たった時の処理
            return "Guild";
        }
        else if(hitName != "Guild" && chapterNum == 2)
        {
            // チャプター2のときに、ギルド以外に当たった時の処理
            return "Guild";
        }
        return "";      // イベント発生の建物なら空白を返す
    }
}
