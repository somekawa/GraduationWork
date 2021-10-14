using System.Collections.Generic;
using UnityEngine;

public class TimeGear : MonoBehaviour
{
    private static TimeGear timgeGrarSingleton_;    // シングルトン用変数

    // キーがenumのTIMEGEARで、値がZ軸の目標回転値
    private Dictionary<SceneMng.TIMEGEAR, float> rotateTimeGearMap_;
    private GameObject panel_;  // 黒背景用のパネル

    void Awake()
    {
        // ゲームオブジェクトがまだ作られていないとき
        if (timgeGrarSingleton_ == null)
        {
            panel_ = GameObject.Find("DontDestroyCanvas/TimeGear/Panel");
            panel_.SetActive(false);

            timgeGrarSingleton_ = this;

            // 自分の親(Canvas)を消えないオブジェクトとして登録
            // 親を登録すれば、子のImageも消えないオブジェクトと判定されるみたい
            DontDestroyOnLoad(transform.root.gameObject);

            // キーと値を登録する
            rotateTimeGearMap_ = new Dictionary<SceneMng.TIMEGEAR, float>{
            {SceneMng.TIMEGEAR.MORNING, 0.0f},
            {SceneMng.TIMEGEAR.NOON   , 90.0f},
            {SceneMng.TIMEGEAR.EVENING, 180.0f},
            {SceneMng.TIMEGEAR.NIGHT  , 270.0f},
            };
        }
        else
        {
            //　既に同じスクリプトがあればこのシーンの同じゲームオブジェクトを削除
            Destroy(transform.root.gameObject);
        }

    }

    void Update()
    {
        // 時間経過テスト用
        if (Input.GetKeyDown(KeyCode.O))
        {
            var aa = SceneMng.GetTimeGear();
            if (aa >= SceneMng.TIMEGEAR.NIGHT) // 今が夜なら、朝が入るようにする
            {
                SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING);
            }
            else
            {
                SceneMng.SetTimeGear((SceneMng.TIMEGEAR)((int)aa + 1));
            }
        }

        // 目標角度をオイラー角からクォータニオンにする
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, rotateTimeGearMap_[SceneMng.GetTimeGear()]));

        // 現在のクォータニオンの取得
        var now_rot = transform.rotation;

        // Quaternion.Angleで2つのクォータニオンの間の角度を求める
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            panel_.SetActive(false);

            // Angleの値が指定の幅以下になったら目標地点に来た扱いにして、処理を止める
            transform.rotation = target;
        }
        else
        {
            panel_.SetActive(true);

            // 回転させる
            transform.Rotate(new Vector3(0.0f, 0.0f, 2.0f));
        }
    }
}
