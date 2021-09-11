using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 町の外での画面管理をする。(町中ではnowModeをNONにする)
// 探索・戦闘・メニュー画面の切り替わり時にenumで変更を行う
// 他のMngとかも、このスクリプトにあるnowMode_を参照して変更を行う

// このスクリプトとは別に、SceneMng的なのを用意して、シーンのロード/アンロードとnowModeを切り替えてくれるものをつくったほうがいいと思う。

public class FieldMng : MonoBehaviour
{
    // 様々なクラスからMODEの状態は見られることになるから、nowMode_はstatic変数にしたほうがいい
    // このクラスは画面状態の遷移を管理するだけで、それ以外の画面処理は他のScriptで行う

    // 画面状態一覧
    public enum MODE
    {
        NON,
        SEARCH,     // 探索中
        BUTTLE,     // 戦闘中
        MENU,       // メニュー画面中
        MAX
    }

    public static MODE nowMode = MODE.NON;          // 現在のモード
    public static float charaRunSpeed = 0.0f;

    private float toButtleTime_ = 30.0f;            // 30秒経過でバトルへ遷移する
    private float time_ = 0.0f;                     // 現在の経過時間

    private UnitychanController player_;            // プレイヤー情報格納用
    private CameraMng cameraMng_;

    void Start()
    {
        //unitychanの情報を取得
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (player_ == null)
        {
            Debug.Log("FieldMng.csで取得しているPlayer情報がnullです");
        }

        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("FieldMng.csで取得しているCameraMngがnullです");
        }
    }

    void Update()
    {
        //Debug.Log("現在のMODE" + nowMode_);

        //Debug.Log(time_);

        switch (nowMode)
        {
            case MODE.NON:
                charaRunSpeed = 8.0f;
                break;

            case MODE.SEARCH :
                cameraMng_.SetChangeCamera(false);   // メインカメラアクティブ
                charaRunSpeed = 4.0f;

                if (player_.GetMoveFlag() && time_ < toButtleTime_)
                {
                    time_ += Time.deltaTime;
                }
                else if(time_ >= toButtleTime_)
                {
                    nowMode = MODE.BUTTLE;
                    time_ = 0.0f;
                }
                else
                {
                    // 何も処理を行わない
                }
            break;

            case MODE.BUTTLE:
                cameraMng_.SetChangeCamera(true);   // サブカメラアクティブ
                charaRunSpeed = 0.0f;

                if (time_ < toButtleTime_)
                {
                    time_ += Time.deltaTime;
                }
                else
                {
                    nowMode = MODE.SEARCH;
                    time_ = 0.0f;
                }
            break;

            default:
                Debug.Log("画面状態一覧でエラーです");
            break;
        }
    }

    // 現在値 / エンカウント発生値
    public float GetNowEncountTime()
    {
        return time_ / toButtleTime_;
    }

}
