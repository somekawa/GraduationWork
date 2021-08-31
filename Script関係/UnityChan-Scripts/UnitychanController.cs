using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラの基本移動と方向転換のみのScript
// →戦闘用Scriptは別で用意する
public class UnitychanController : MonoBehaviour
{
    private Rigidbody rigid;      // Rigidbodyコンポーネント
    private Animator animator_;   // Animator コンポーネント

    private const string key_isRun = "isRun";         // Animatorで自分で設定したフラグの名前
    //private const string key_isAttack = "isAttack";

    // 押下状態を確認したいキーをまとめたもの
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator_= GetComponent<Animator>();
    }

    void Update()
    {
        // 攻撃モーションテスト用
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    this.animator_.SetBool(key_isAttack, true);
        //}
        //else
        //{
        //    this.animator_.SetBool(key_isAttack, false);
        //}

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
            movedir.z = Input.GetAxis("Vertical") * 3.0f;
        }

        if (Input.GetKey(keyArray_[2]) || Input.GetKey(keyArray_[3]))
        {
            // 左キー or 右キー
            movedir.x = Input.GetAxis("Horizontal") * 3.0f;
        }

        // グローバル座標に変換すると、キャラの方向転換後に+-がバグが起きた
        //Vector3 globaldir = transform.TransformDirection(movedir);
        //controller_.Move(globaldir * Time.deltaTime);

        if (movedir != Vector3.zero)
        {
            // 座標更新
            //　キャラクターを移動させる処理
            rigid.MovePosition(rigid.position + movedir * Time.deltaTime);
            // キャラ方向転換
            transform.rotation = Quaternion.LookRotation(movedir);
        }
    }
}