using UnityEngine;

public class PopList : MonoBehaviour
{
    public enum ListData
    {
        CHAPTER,        // ストーリー
        CHARACTER,      // キャラクター初期情報(あとで、すべてCSV形式からの読み込み書き出しに変えるかも)
        QUESTINFO,      // クエスト情報
        MAX
    }

    private T GetList<T>(string dataName)
    {
        T list = (T)(object)Resources.Load(dataName);

        if (list == null)
        {
            Debug.Log("PopMob.csのlist情報がnullです");
        }

        return list;
    }

    public T GetData<T>(ListData data, int num = 0, string str = "")
    {
        string tmpStr;

        switch (data)
        {
            case ListData.CHAPTER:
                tmpStr = "Chapter/Chapter" + num;
                //object(任意のTにキャストできる)にキャストし、そこから別の型にキャストする必要がある
                return (T)(object)GetList<ChapterList>(tmpStr);

            case ListData.CHARACTER:
                tmpStr = "Chara" + str;
                return (T)(object)GetList<CharacterList>(tmpStr);

            case ListData.QUESTINFO:
                tmpStr = "Quest" + num;
                return (T)(object)GetList<QuestInfo>(tmpStr);

            default:
                return default(T);
        }
    }
}
