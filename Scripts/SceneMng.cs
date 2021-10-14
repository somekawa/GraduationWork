using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMng : MonoBehaviour
{
    // キャラクターの種類
    public enum CHARACTERNUM
    {
        UNI,    // 手前
        DEMO,   // 奥
        MAX
    }

    // シーンの種類
    public enum SCENE
    {
        NON = -1,
        CONVERSATION,   // 会話シーン
        TOWN,           // 街シーン
        UNIHOUSE,       // ユニちゃんの家
        FIELD0,          // フィールドシーン
        FIELD1,          // フィールドシーン
        FIELD2,          // フィールドシーン
        FIELD3,          // フィールドシーン
        FIELD4,          // フィールドシーン
        CANCEL,          // ワープ先表示用
        MAX
    }

    // 1日の時間経過
    public enum TIMEGEAR
    {
        MORNING,    // 朝
        NOON,       // 昼
        EVENING,    // 夕
        NIGHT       // 夜
    }

    public static SceneMng singleton;

    // enumとキャラオブジェクトをセットにしたmapを制作するためのリスト
    // キャラオブジェクトを要素としてアタッチできるようにしておく
    public List<GameObject> charaObjList;

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    public static Dictionary<CHARACTERNUM, GameObject> charMap_;
    public static List<Chara> charasList_ = new List<Chara>();          // Chara.csをキャラ毎にリスト化する

    public static SCENE nowScene = SCENE.NON;   // 現在のシーン
    public static float charaRunSpeed = 0.0f;   // キャラの移動速度(MODE毎に調整をする)

    private static string houseName_ = "Mob";    // Excelから読み込んだ建物名
    private static TIMEGEAR timeGrar_ = TIMEGEAR.MORNING;               // 1日の経過時間の情報を入れる

    void Awake()
    {
        //　スクリプトが設定されていなければゲームオブジェクトを残しつつスクリプトを設定
        if (singleton == null)
        {
            // シーンを跨いでも消えないオブジェクトに設定する
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //　既にGameStartスクリプトがあればこのシーンの同じゲームオブジェクトを削除
            Destroy(gameObject);
        }

        // キャラのゲームオブジェクト情報がシーンを跨ぐとMissingになる関係で、Scene毎に一度情報を消す必要がある
        if(charMap_ != null)
        {
            charMap_.Clear();
        }
        if(charasList_ != null)
        {
            charasList_.Clear();
        }

        // 要素が入っていない時はreturnする
        if(charaObjList.Count <= 0)
        {
            Debug.Log("SceneMngの引数に要素が設定されていないのでreturnします");
            return;
        }

        // キャラクターの情報をゲームオブジェクトとして最初に取得しておく
        charMap_ = new Dictionary<CHARACTERNUM, GameObject>(){
                {CHARACTERNUM.UNI,charaObjList[(int)CHARACTERNUM.UNI]},
                {CHARACTERNUM.DEMO,charaObjList[(int)CHARACTERNUM.DEMO]},
            };

        // charMap_でforeachを回して、キャラクターのリストを作成
        foreach (KeyValuePair<CHARACTERNUM, GameObject> anim in charMap_)
        {
            // Charaクラスの生成
            charasList_.Add(new Chara(anim.Value.name, anim.Key, anim.Value.GetComponent<Animator>()));
        }

        // 初回のみキャラステータスを初期値で登録
        if(singleton == null)
        {
            CharaData.SetCharaData(charasList_[0].GetCharaSetting());
            singleton = this;
        }

        //@ ここでcharasDataのステータス値をcharasList_に代入？
        // ステータス値代入テスト
        charasList_[0].SetCharaSetting(CharaData.GetCharaData());

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
                    charaRunSpeed = 10.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.FIELD0:
                    charaRunSpeed = 8.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.FIELD1:
                    charaRunSpeed = 8.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.UNIHOUSE:
                    charaRunSpeed = 8.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                default:
                    break;
            }
            nowScene = scene;
        }
    }

    // シーンのロード
    public static void SceneLoad(int load)
    {
        // NONが入っていたらreturnする
        if(load == -1)
        {
            return;
        }

        //@ ここでcharasList_のステータス値をcharasDataに避難させる？ 
        CharaData.SetCharaData(charasList_[0].GetCharaSetting());

        // int番号は、ビルド設定の数値
        SceneManager.LoadScene(load);
    }

    // Excelから読み込んだ建物名を保存しておく
    public static void SetHouseName(string name)
    {
        houseName_ = name;
    }

    // Excelから読み込んだ建物名を渡す
    public static string GetHouseName()
    {
        return houseName_;
    }

    public static void SetTimeGear(TIMEGEAR dayTime)
    {
        timeGrar_ = dayTime;
    }

    public static TIMEGEAR GetTimeGear()
    {
        return timeGrar_;
    }
}
