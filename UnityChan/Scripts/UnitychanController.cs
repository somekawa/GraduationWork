using UnityEngine;

// キャラの基本移動と方向転換のみのScript
// →戦闘用Scriptは別

public class UnitychanController : MonoBehaviour
{
    private Rigidbody rigid_;      // Rigidbodyコンポーネント
    private Animator animator_;    // Animator コンポーネント

    // アニメーション
    private readonly int runParamHash_ = Animator.StringToHash("isRun");
    private readonly int attackParamHash_ = Animator.StringToHash("isAttack");

    // 押下状態を確認したいキーをまとめたもの
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    private Vector3 moveDir_ = Vector3.zero;

    void Start()
    {
        rigid_ = GetComponent<Rigidbody>();
        animator_= GetComponent<Animator>();
    }

    void Update()
    {
        // 探索中以外はアニメーションが変わらないようにする
        if (FieldMng.nowMode != FieldMng.MODE.SEARCH)
        {
            return;
        }

         bool tmpFlg = false;       // 座標移動のボタン押下時にtrueになる
        foreach (KeyCode i in keyArray_)
        {
            // keyArray_に設定したKeyCodeの中で、押下されているボタンがあるかを調べる
            if (Input.GetKey(i))
            {
                // WaitからRunに遷移する
                this.animator_.SetBool(runParamHash_, true);
                tmpFlg = true;
                break;  // それ以上回す必要がないので、breakで抜ける
            }
        }

        if (!tmpFlg) // 上のforeachを通ってもfalseのままだった場合は、走るアニメーションをfalseにする(=待機)
        {
            // RunからWaitに遷移する
            this.animator_.SetBool(runParamHash_, false);
            return; // 待機アニメーションということは下の座標移動処理を行う必要がないため、returnする
        }

        // 矢印下ボタンを押下している
        if (Input.GetKey(keyArray_[0]) || Input.GetKey(keyArray_[1]))
        {
            // 上キー or 下キー
            moveDir_.z = Input.GetAxis("Vertical");
        }

        if (Input.GetKey(keyArray_[2]) || Input.GetKey(keyArray_[3]))
        {
            // 左キー or 右キー
            moveDir_.x = Input.GetAxis("Horizontal");
        }
    }

    // rigidbodyを使用する移動計算は、FixedUpdateを利用して一定周期でおこなうようにする
    void FixedUpdate()
    {
        // 探索モード以外で自由に動かれたらいけないので、return処理を加える。
        if (FieldMng.nowMode != FieldMng.MODE.SEARCH)
        {
            // キャラにかかっている慣性を一時的に止める
            rigid_.velocity = Vector3.zero;
            rigid_.angularVelocity = Vector3.zero;

            // ここでRunのアニメーションを変更しておかないと、モードが切り替わる瞬間まで走っていたら
            // 走りモーションが戦闘中に継続してしまう。
            this.animator_.SetBool(runParamHash_, false);
            return;
        }
        else
        {
            // 戦闘から探索に戻ってきたときに、攻撃モーションの途中なら切り上げるようにする
            this.animator_.SetBool(attackParamHash_, false);
        }

        // グローバル座標に変換すると、キャラの方向転換後に+-がバグが起きた
        //Vector3 globaldir = transform.TransformDirection(movedir);
        //controller_.Move(globaldir * Time.deltaTime);

        if (moveDir_ != Vector3.zero)
        {
            // 速度ベクトルを作成（3次元用）Y座標は0.0fで必ず固定する
            var speed = new Vector3(moveDir_.x, 0.0f, moveDir_.z);
            // 速度に正規化したベクトルに、移動速度をかけて代入する
            rigid_.velocity = speed.normalized * SceneMng.charaRunSpeed;

            // 座標更新
            // キャラクターを移動させる処理
            rigid_.MovePosition(rigid_.position + rigid_.velocity * Time.deltaTime);
            // キャラ方向転換
            transform.rotation = Quaternion.LookRotation(moveDir_);
        }

        moveDir_ = Vector3.zero;
        rigid_.velocity = Vector3.zero;
    }

    // キャラが移動中か
    public bool GetMoveFlag()
    {
        return this.animator_.GetBool(runParamHash_);
    }

    // キャラの走りアニメーションを止める
    public void StopUniRunAnim()
    {
        // アニメーションがあるかnullチェックを行う
        if(this.animator_ != null)
        {
            this.animator_.SetBool(runParamHash_, false);
        }
    }

}