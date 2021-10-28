using UnityEngine;

public class DontDestroyMng : MonoBehaviour
{
    public static DontDestroyMng singleton;
    private static GameObject mInstance;

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
            mInstance = this.gameObject;
        }
        else
        {
            //　既にGameStartスクリプトがあればこのシーンの同じゲームオブジェクトを削除
            Destroy(gameObject);
        }
    }
}
