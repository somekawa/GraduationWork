using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キー操作によって、バトル選択コマンドが回転する

// Unityの場合、角度はオイラー角ではなくクォータニオンで持っているので、それでAngleを比較して指定の角度で止まるようにします。

public class ImageRotate : MonoBehaviour
{
    // 回転方向
    enum DIR
    {
        NON,
        LEFT,
        RIGHT
    }

    private DIR rotateDir_ = DIR.NON;   // 回転方向の指定
    private float targetRotate_ = 0.0f; // 回転角度
    
    void Start()
    {
    }

    void Update()
    {
        // キーによって、回転方向を決定する
        if (Input.GetKeyDown(KeyCode.J))
        {
            // 右回転
            targetRotate_ += 90.0f;
            rotateDir_ = DIR.RIGHT;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            // 左回転
            targetRotate_ -= 90.0f;
            rotateDir_ = DIR.LEFT;
        }
        else
        {
            // 何も処理を行わない
        }

        // 目標角度をオイラー角からクォータニオンにする
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, targetRotate_));

        // 現在のクォータニオンの取得
        var now_rot = transform.rotation;

        // Quaternion.Angleで2つのクォータニオンの間の角度を求める
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            // Angleの値が指定の幅以下になったら目標地点に来た扱いにして、処理を止める
            transform.rotation = target;
            rotateDir_ = DIR.NON;
        }
        else
        {
            // 指定の幅に届いていない場合は、回転方向を確認して、回転を続ける
            if(rotateDir_ == DIR.RIGHT)
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f));
            }
            else if(rotateDir_ == DIR.LEFT)
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, -0.5f));
            }
            else
            {
                // 何も処理を行わない
            }
        }
    }
}
