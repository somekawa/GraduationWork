using UnityEngine;

public class PopList : MonoBehaviour
{
    public enum ListData
    {
        CHAPTER,        // ストーリー
        CHARACTER,      // キャラクター初期情報
        ENEMY,          // 敵情報
        QUESTINFO,      // クエスト情報
        CHEST,          // 宝箱情報
        MATERIA,
        ITEM,
        WORD,
        BOOK_STORE,
        RESTAURANT,     // レストラン情報
        MAX
    }

    private T GetList<T>(string dataName)
    {
        T list = (T)(object)Resources.Load(dataName);

        //// StreamingAssetsからAssetBundleをロードする
        //var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        //// AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        //// フォルダ名を入れる必要はない。例えば、testフォルダにobjobjプレハブが入っていても、LoadAssetにはobjobjだけ記載する
        ////var a = assetBundle.LoadAsset<GameObject>("objobj");
        //T a = (T)(object)assetBundle.LoadAsset<GameObject>("Quest0");
        //// 不要になったAssetBundleのメタ情報をアンロードする
        //assetBundle.Unload(false);


        if (list == null)
        {
            Debug.Log(dataName + "PopMob.csのlist情報がnullです");
        }
        else
        {
            Debug.Log(dataName + "の読み込み完了");
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

            case ListData.ENEMY:
                tmpStr = "EnemyData/Field" + num;
                return (T)(object)GetList<EnemyList>(tmpStr);

            case ListData.QUESTINFO:
                tmpStr = "Quest" + num;
                return (T)(object)GetList<QuestInfo>(tmpStr);

            case ListData.CHEST:
                tmpStr = "Chest";
                return (T)(object)GetList<ChestList>(tmpStr);

            case ListData.MATERIA:
                tmpStr = "MateriaList/M_Field" + num;
                return (T)(object)GetList<MateriaList>(tmpStr);

            case ListData.ITEM:
                tmpStr = "ItemList/ItemList";
                return (T)(object)GetList<ItemList>(tmpStr);

            case ListData.WORD:
                tmpStr = "WordList/WordList0";
                return (T)(object)GetList<WordList>(tmpStr);

            case ListData.BOOK_STORE:
                tmpStr = "BookList/BookList" + num;
                return (T)(object)GetList<BookList>(tmpStr);
            case ListData.RESTAURANT:
                tmpStr = "Cook" + num;
                return (T)(object)GetList<Cook0>(tmpStr);
            default:
                return default(T);
        }
    }
}
