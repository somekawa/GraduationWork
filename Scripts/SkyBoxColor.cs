using System.Collections.Generic;
using UnityEngine;

// Directional Lightにアタッチして太陽の位置を変更し、空の色を変える
// 街やフィールドに夜用の光源を置かないといけない

public class SkyBoxColor : MonoBehaviour
{
    // スクリプトからAtmosphereThicknessの変更方法がわからなかったため、
    // 外部アタッチで朝・昼・夜用と夕方用で使用を分けるようにした。

    public Material eveningSkyBox;  // 夕方用
    public Material defaultSkyBox;  // 朝・昼・夜用

    // キー:時間経過enum , 値:ディレクショナルライトの位置
    private Dictionary<SceneMng.TIMEGEAR, Vector3> dayTimeLight_ = new Dictionary<SceneMng.TIMEGEAR, Vector3>();

    private SceneMng.TIMEGEAR day_;
    private SceneMng.TIMEGEAR oldDay_;

    void Start()
    {
        day_ = SceneMng.TIMEGEAR.MORNING;

        // 初期化
        dayTimeLight_ = new Dictionary<SceneMng.TIMEGEAR, Vector3>(){
            {SceneMng.TIMEGEAR.MORNING, new Vector3(55.0f,28.0f ,0.0f)},
            {SceneMng.TIMEGEAR.NOON   , new Vector3(55.0f, 0.0f ,-45.0f)},
            {SceneMng.TIMEGEAR.EVENING, new Vector3(20.0f,-56.0f,0.0f)},
            {SceneMng.TIMEGEAR.NIGHT  , new Vector3(-9.0f,300.0f,-50.0f)},
        };
    }

    void Update()
    {
        day_ = SceneMng.GetTimeGear();

        if (oldDay_ != day_)
        {
            if (day_ == SceneMng.TIMEGEAR.EVENING)
            {
                RenderSettings.skybox = eveningSkyBox;
            }
            else
            {
                RenderSettings.skybox = defaultSkyBox;
            }


            // 目標角度をオイラー角からクォータニオンにする
            transform.rotation = Quaternion.Euler(dayTimeLight_[day_]);

            oldDay_ = day_;
        }
    }
}
