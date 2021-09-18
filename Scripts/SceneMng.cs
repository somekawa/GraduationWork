using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMng : MonoBehaviour
{
    // シーンの種類
    public enum SCENE
    {
        TOWN,   // 街シーン
        FIELD,  // フィールドシーン
        MAX
    }

    public static SCENE nowScene=SCENE.MAX;               // 現在のシーン
    public static float charaRunSpeed = 0.0f;   // キャラの移動速度(MODE毎に調整をする)

    //void Awake()
    //{
    //    // シーンを跨いでも消えないオブジェクトに設定する
    //    DontDestroyOnLoad(gameObject);
    //}

    // 外部から設定されるシーン状態遷移
    public static void SetNowScene(SCENE scene)
    {
        // 現在のシーンに合わせてキャラの移動速度を変更する
        if (nowScene != scene)
        {
            switch (scene)
            {
                case SCENE.TOWN:
                    charaRunSpeed = 8.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.FIELD:
                    charaRunSpeed = 4.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                default:
                    break;
            }
            nowScene = scene;
        }
    }

    // シーンのロード/アンロード
    public static void SceneLoadUnLoad(int load , int unload)
    {
        // int番号は、ビルド設定の数値
        SceneManager.LoadScene(load);
        SceneManager.UnloadSceneAsync(unload);
    }
}
