using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMng : MonoBehaviour
{
    // キャラクターの種類
    public enum CharcterNum
    {
        UNI,    // 手前
        DEMO,   // 奥
        MAX
    }

    // シーンの種類
    public enum SCENE
    {
        TOWN,   // 街シーン
        FIELD,  // フィールドシーン
        MAX
    }

    // enumとキャラオブジェクトをセットにしたmapを制作するためのリスト
    // キャラオブジェクトを要素としてアタッチできるようにしておく
    public List<GameObject> charaObjList;

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    private static Dictionary<CharcterNum, GameObject> charMap_;
    private static List<Chara> charasList_ = new List<Chara>();          // Chara.csをキャラ毎にリスト化する

    public static SCENE nowScene = SCENE.MAX;   // 現在のシーン
    public static float charaRunSpeed = 0.0f;   // キャラの移動速度(MODE毎に調整をする)

    void Awake()
    {
        // シーンを跨いでも消えないオブジェクトに設定する
        DontDestroyOnLoad(gameObject);

        // キャラクターの情報をゲームオブジェクトとして最初に取得しておく
        charMap_ = new Dictionary<CharcterNum, GameObject>(){
            {CharcterNum.UNI,charaObjList[(int)CharcterNum.UNI]},
            {CharcterNum.DEMO,charaObjList[(int)CharcterNum.DEMO]},
        };

        // charMap_でforeachを回して、キャラクターのリストを作成
        foreach (KeyValuePair<CharcterNum, GameObject> anim in charMap_)
        {
            // Charaクラスの生成
            charasList_.Add(new Chara(anim.Value.name, anim.Key, anim.Value.GetComponent<Animator>()));
        }
    }

    // CharacterMng.csのStart関数で呼ばれる
    public static Dictionary<CharcterNum, GameObject> GetCharMap()
    {
        return charMap_;
    }

    // CharacterMng.csのStart関数で呼ばれる
    public static List<Chara> GetCharasList()
    {
        return charasList_;
    }

    // MenuMng.csで呼び出す
    public static CharaBase.CharacterSetting GetCharasSettings(int num)
    {
        return charasList_[num].GetCharaSetting();
    }

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
