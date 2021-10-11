using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagCanvasUI : MonoBehaviour
{
    public static BagCanvasUI singleton;
    private static GameObject mInstance;

    public static GameObject Instance
    {
        get
        {
            return mInstance;
        }
    }
    // private GameObject prefab_;
    void Awake()
    {
        // シーンを跨いでも消えないオブジェクトにする
        // このオブジェクトが消えてしまうと、QuestClearCheck.csの受注中のクエストを保存しているリストがnullになってしまうから
        //　スクリプトが設定されていなければゲームオブジェクトを残しつつスクリプトを設定
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
    public void SetMyName(string num)
    {
        // 自分のオブジェクト名を、クエスト番号に変換
        // 他スクリプトがヒエラルキーからこのオブジェクトを検索するときに番号+タグ(Quest)で判別できるようにした
        this.gameObject.name = num.ToString();
    }

    //public GameObject Instance()
    //{
    //        return this.gameObject;
    //}

}
