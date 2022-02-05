using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneMng : MonoBehaviour
{
    // キャラクターの種類
    public enum CHARACTERNUM
    {
        UNI,    // 手前
        JACK,   // 奥
        MAX
    }

    // シーンの種類
    public enum SCENE
    {
        NON = -1,
        TITLE,          // タイトルに戻るボタン
        CONVERSATION,   // 会話シーン
        TOWN,           // 街シーン
        UNIHOUSE,       // ユニちゃんの家
        FIELD0,          // フィールドシーン
        FIELD1,          // フィールドシーン
        FIELD2,          // フィールドシーン
        FIELD3,          // フィールドシーン
        FIELD4,          // フィールドシーン
        END,// エンド用シーン
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

    private static SEAudioMng seAudio_;

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    public static Dictionary<CHARACTERNUM, GameObject> charMap_;
    public static List<Chara> charasList_ = new List<Chara>();          // Chara.csをキャラ毎にリスト化する

    public static SCENE nowScene = SCENE.NON;   // 現在のシーン
    public static float charaRunSpeed = 0.0f;   // キャラの移動速度(MODE毎に調整をする)

    private static string houseName_ = "Mob";   // Excelから読み込んだ建物名

    private static TIMEGEAR timeGrar_ = TIMEGEAR.MORNING;               // 1日の経過時間の情報を入れる
    private static (TIMEGEAR,bool) finStatusUpTime_;                    // 料理効果がキレる時間を保存する

    private static int haveMoney_ = 1000;       // 現在の所持金(初期所持金1000)

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
                {CHARACTERNUM.JACK,charaObjList[(int)CHARACTERNUM.JACK]},
            };

        // charMap_でforeachを回して、キャラクターのリストを作成
        foreach (KeyValuePair<CHARACTERNUM, GameObject> anim in charMap_)
        {
            // Charaクラスの生成
            charasList_.Add(new Chara(anim.Value.name,0,anim.Value.GetComponent<Animator>()));
        }

        // 初回のみキャラステータスを初期値で登録
        if(singleton == null)
        {
            CharaData.SetCharaData(0,charasList_[0].GetCharaSetting());
            CharaData.SetCharaData(1,charasList_[1].GetCharaSetting());
            singleton = this;
        }

        // ここでcharasDataのステータス値をcharasList_に代入
        charasList_[0].SetCharaSetting(CharaData.GetCharaData(0));
        charasList_[1].SetCharaSetting(CharaData.GetCharaData(1));
    }

    // ロードデータを代入する
    public static void SetCharasSettings(int num,CharaBase.CharacterSetting set)
    {
        charasList_[num].SetCharaSetting(set);
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

                case SCENE.FIELD2:
                    charaRunSpeed = 10.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.FIELD3:
                    charaRunSpeed = 5.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.FIELD4:
                    charaRunSpeed = 10.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                case SCENE.UNIHOUSE:
                    charaRunSpeed = 8.0f;
                    Debug.Log("移動速度変更" + charaRunSpeed);
                    break;

                default:
                    break;
            }

            // sceneでnowSceneを上書き前に、今まで動いていたシーンがフィールドであれば時間経過させる
            if(nowScene != SCENE.TOWN && nowScene != SCENE.UNIHOUSE && nowScene != SCENE.CONVERSATION && nowScene != SCENE.NON)    
            {
                if(scene != SCENE.CONVERSATION) // このif文がないと、フィールドをはしごして会話シーンに飛んだら時刻が2つ進んでしまう
                {
                    // タウンでもユニハウスでもNONでもない = どこかのフィールド
                    SetTimeGear(timeGrar_ + 1);
                }
            }

            nowScene = scene;

            // DontDestroyCanvas内のオブジェクトの非表示管理
            if (nowScene == SCENE.TOWN)
            {
                // ギルドにいる場合はバッグを非表示に
                var interiorMng = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
                if (interiorMng.GetInHouseName() == "Guild")
                {
                    // 会話シーンならMENUのみ非表示に
                    MenuSetActive(false);
                }
            }
            else if (nowScene == SCENE.CONVERSATION)
            {
                // 会話シーンならMENUのみ非表示に
                MenuSetActive(false);
            }
            else
            {
                return;
            }
        }
    }

    // この書き方のほうが確実にMenuを取得して表示/非表示を変更できる
    public static void MenuSetActive(bool flag)
    {
        var tmp = GameObject.Find("DontDestroyCanvas").GetComponent<RectTransform>();
        for (int i = 0; i < tmp.childCount; i++)
        {
            if (tmp.GetChild(i).gameObject.name == "Menu")
            {
                tmp.GetChild(i).gameObject.SetActive(flag);
                break;
            }
        }
    }

    // シーンのロード
    public static void SceneLoad(int load,bool allDeath = false)
    {
        // NONが入っていたらreturnする
        if(load == -1)
        {
            return;
        }

        //GameObject.Find("Uni") = false;

        //@ ここでcharasList_のステータス値をcharasDataに避難させる？ 
        CharaData.SetCharaData(0,charasList_[0].GetCharaSetting());
        CharaData.SetCharaData(1,charasList_[1].GetCharaSetting());

        // int番号は、ビルド設定の数値
        //SceneManager.LoadScene(load);
        if(GameObject.Find("CameraController"))
        {
            GameObject.Find("CameraController").GetComponent<CameraMng>().SetChangeCamera(true, true);
        }

        // 全滅時はロード処理に入らない(入るとゲームが固まる)
        if(allDeath)
        {
            SceneManager.LoadScene(load);
        }
        else
        {
            GameObject.Find("DontDestroyCanvas/LoadingCamera").GetComponent<Loading>().NextScene(load);
        }
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
        if (dayTime > TIMEGEAR.NIGHT) // 今が夜なら、朝が入るようにする
        {
            SetSE(1);
            timeGrar_ = TIMEGEAR.MORNING;
        }
        else
        {
            SetSE(1);
            timeGrar_ = dayTime;
        }

        // 料理の効果が切れる時間ならフラグをfalseにする
        if(timeGrar_ == finStatusUpTime_.Item1)
        {
            finStatusUpTime_.Item2 = false;
            GameObject.Find("DontDestroyCanvas/TimeGear/CookPowerIcon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Debug.Log("料理効果が切れました");
        }
    }

    public static TIMEGEAR GetTimeGear()
    {
        return timeGrar_;
    }

    // お金の設定
    public static void SetHaveMoney(int money)
    {
        haveMoney_ = money;
    }

    // お金を取得する
    public static int GetHaveMoney()
    {
        return haveMoney_;
    }

    // 料理効果が切れる時刻を保存する
    public static void SetFinStatusUpTime(int num = -1,bool loadFlag = false)
    {
        if(loadFlag)
        {
            // 計算した数字やそのままの数字を代入して設定する
            finStatusUpTime_ = ((TIMEGEAR)num, true);
            GameObject.Find("DontDestroyCanvas/TimeGear/CookPowerIcon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Debug.Log("料理効果が切れるのが、" + finStatusUpTime_ + "に設定されました");
        }
        else
        {
            // 2つ先の時刻を設定する
            var tmpNum = (int)timeGrar_ + 2;

            // 2つ先の時刻が夜を越えた数字になったら
            if (tmpNum > (int)TIMEGEAR.NIGHT)
            {
                // 越えた数字から4を引けばいい
                tmpNum -= 4;
            }

            // 計算した数字やそのままの数字を代入して設定する
            finStatusUpTime_ = ((TIMEGEAR)tmpNum, true);
            Debug.Log("料理効果が切れるのが、" + finStatusUpTime_ + "に設定されました");
        }
    }

    // 料理効果が切れたかフラグを取得する
    public static (TIMEGEAR,bool) GetFinStatusUpTime()
    {
        return finStatusUpTime_;
    }

    public static void SetSE(int num)
    {
        if(seAudio_ == null)
        {
            if(GameObject.Find("Audio/SE_Audio"))
            {
                seAudio_ = GameObject.Find("Audio/SE_Audio").GetComponent<SEAudioMng>();
            }
            else
            {
                seAudio_ = GameObject.Find("FieldMng/SE_Audio").GetComponent<SEAudioMng>();
            }
        }
        seAudio_.OnceShotSE(num);
    }
}
