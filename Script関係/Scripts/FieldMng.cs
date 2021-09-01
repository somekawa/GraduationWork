using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 町の外での画面管理をする。
// 探索・戦闘・メニュー画面の切り替わり時にenumで変更を行う
// 他のMngとかも、このスクリプトにあるnowMode_を参照して変更を行う

public class FieldMng : MonoBehaviour
{
    // 様々なクラスからMODEの状態は見られることになるから、nowMode_はstatic変数にしたほうがいい

    // 画面状態一覧
    public enum MODE
    {
        NON,
        SEARCH,     // 探索中
        BUTTLE,     // 戦闘中
        MENU,       // メニュー画面中
        MAX
    }

    public static MODE nowMode = MODE.SEARCH;       // 現在のモード

    private float toButtleTime_ = 5.0f;              // 5秒経過でバトルへ遷移する
    private float time_ = 0.0f;                      // 現在の経過時間

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("現在のMODE" + nowMode_);

        switch(nowMode)
        {
            case MODE.SEARCH :
            if (time_ < toButtleTime_)
            {
                time_ += Time.deltaTime;
            }
            else
            {
                nowMode = MODE.BUTTLE;
                time_ = 0.0f;
            }
            break;

            case MODE.BUTTLE:
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
}
