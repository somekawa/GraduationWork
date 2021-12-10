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
        NON,    // 回転なし
        LEFT,   // 左回転
        RIGHT   // 右回転
    }

    // 選択コマンド
    public enum COMMAND
    {
        NON,    // 回転中で選択中コマンドがない状態
        ATTACK, // 攻撃コマンド(初期コマンド)
        MAGIC,  // 魔法コマンド
        ITEM,   // アイテムコマンド
        BARRIER,// 防御コマンド
        MAX     
    }

    private DIR rotateDir_ = DIR.NON;   // 回転方向の指定
    private float targetRotate_ = 0.0f; // 回転角度

    // キーを角度,値をCOMMANDのenumで作ったmap
    private Dictionary<int, COMMAND> commandMap_;
    public COMMAND nowCommand_ = COMMAND.ATTACK;   // 現在の選択中コマンド

    private bool rotaFlg_ = true;       // 回転をしてもいいかを判定する(true:回転してよい)

    private bool flag = true;

    void Start()
    {
        // コマンド状態の追加
        commandMap_ = new Dictionary<int, COMMAND>(){
            {0,COMMAND.ATTACK},
            {90,COMMAND.MAGIC},
            {180,COMMAND.BARRIER},
            {270,COMMAND.ITEM},
        };
    }

    void Update()
    {
        //Debug.Log(targetRotate_);
        //Debug.Log(nowCommand_);

        if(!rotaFlg_ || !flag)
        {
            return;
        }

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

        // 360以上か以下なら、targetRotate_を0に戻す
        if (targetRotate_ >= 360 || targetRotate_ <= -360)
        {
            targetRotate_ = 0;
        }

        // 目標角度をオイラー角からクォータニオンにする
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, targetRotate_));

        // 現在のクォータニオンの取得
        var now_rot = transform.rotation;

        // Quaternion.Angleで2つのクォータニオンの間の角度を求める
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            // 回転終了
            int rota = (int)targetRotate_;
            if (rota < 0)   // 左回転の場合は、rotaがマイナス値になるため+360して右回転と同じ値に変更する
            {
                rota += 360;
            }
            // 現在の回転角度を見て、COMMANDを決定する
            nowCommand_ = commandMap_[rota];

            // Angleの値が指定の幅以下になったら目標地点に来た扱いにして、処理を止める
            transform.rotation = target;
            rotateDir_ = DIR.NON;
        }
        else
        {
            // 回転中
            nowCommand_ = COMMAND.NON;

            // 指定の幅に届いていない場合は、回転方向を確認して、回転を続ける
            if (rotateDir_ == DIR.RIGHT)
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

    // 戦闘モードが終了した直後に、回転を初期値に戻す
    public void ResetRotate()
    {
        var resetRotate = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        transform.rotation = resetRotate;
        targetRotate_ = 0.0f;
    }

    // 現在のコマンド状態をreturnする
    public COMMAND GetNowCommand()
    {
        return nowCommand_;
    }

    // 表示状態とスクリプトの実行状態を切り替える
    public void SetEnableAndActive(bool flag)
    {
        gameObject.SetActive(flag);
        enabled = flag;
    }
}
