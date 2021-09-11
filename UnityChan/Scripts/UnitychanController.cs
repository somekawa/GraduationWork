using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラの基本移動と方向転換のみのScript
// →戦闘用Scriptは別

public class UnitychanController : MonoBehaviour
{
    private Rigidbody rigid;      // Rigidbodyコンポーネント
    private Animator animator_;   // Animator コンポーネント

    private const string key_isRun = "isRun";         // Animatorで自分で設定したフラグの名前

    // 押下状態を確認したいキーをまとめたもの
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator_= GetComponent<Animator>();
    }

    void Update()
    {
        // 探索モード以外で自由に動かれたらいけないので、return処理を加える。
        // 町の中でもSEARCHにしておけばいいかな？
        if(FieldMng.nowMode != FieldMng.MODE.SEARCH && FieldMng.nowMode != FieldMng.MODE.NON)
        {
            // ここでRunのアニメーションを変更しておかないと、モードが切り替わる瞬間まで走っていたら
            // 走りモーションが戦闘中に継続してしまう。
            this.animator_.SetBool(key_isRun, false);
            return;
        }

        bool tmpFlg = false;       // 座標移動のボタン押下時にtrueになる
        foreach (KeyCode i in keyArray_)
        {
            // keyArray_に設定したKeyCodeの中で、押下されているボタンがあるかを調べる
            if (Input.GetKey(i))
            {
                // WaitからRunに遷移する
                this.animator_.SetBool(key_isRun, true);
                tmpFlg = true;
                break;  // それ以上回す必要がないので、breakで抜ける
            }
        }

        if (!tmpFlg) // 上のforeachを通ってもfalseのままだった場合は、走るアニメーションをfalseにする(=待機)
        {
            // RunからWaitに遷移する
            this.animator_.SetBool(key_isRun, false);
            return; // 待機アニメーションということは下の座標移動処理を行う必要がないため、returnする
        }

        Vector3 movedir = Vector3.zero;

        // 矢印下ボタンを押下している
        if (Input.GetKey(keyArray_[0]) || Input.GetKey(keyArray_[1]))
        {
            // 上キー or 下キー
            movedir.z = Input.GetAxis("Vertical") * FieldMng.charaRunSpeed;
        }

        if (Input.GetKey(keyArray_[2]) || Input.GetKey(keyArray_[3]))
        {
            // 左キー or 右キー
            movedir.x = Input.GetAxis("Horizontal") * FieldMng.charaRunSpeed;
        }

        // グローバル座標に変換すると、キャラの方向転換後に+-がバグが起きた
        //Vector3 globaldir = transform.TransformDirection(movedir);
        //controller_.Move(globaldir * Time.deltaTime);

        if (movedir != Vector3.zero)
        {
            // 座標更新
            // キャラクターを移動させる処理
            rigid.MovePosition(rigid.position + movedir * Time.deltaTime);
            // キャラ方向転換
            transform.rotation = Quaternion.LookRotation(movedir);
        }
    }

    // キャラが移動中か
    public bool GetMoveFlag()
    {
        return this.animator_.GetBool(key_isRun);
    }
}