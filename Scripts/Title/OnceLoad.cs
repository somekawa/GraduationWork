using UnityEngine;

public class OnceLoad : MonoBehaviour
{
    public static OnceLoad singleton;
    private static GameObject mInstance;
    private static bool loadFlag = false;

    public static GameObject Instance
    {
        // 破壊されるスクリプトからも検索ができるように
        get
        {
            return mInstance;
        }
    }

    void Start()
    {
        if (singleton == null)
        {
            // シーンを跨いでも消えないオブジェクトに設定する
            DontDestroyOnLoad(gameObject);
            singleton = this;
            mInstance = gameObject;
        }
    }

    public void SetLoadFlag(bool flag)
    {
        loadFlag = flag;

        Debug.Log("フラグが"+loadFlag+"になりました");

        if (loadFlag==true)
        {
            // ゲーム最初のLoadがされたらプレハブを破壊する
            Debug.Log(this+"プレハブを破壊します");
            Destroy(gameObject);
        }
    }
}

