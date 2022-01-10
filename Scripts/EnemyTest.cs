using UnityEngine;

public class EnemyTest : MonoBehaviour
{
    private Animator animator_;
    private float attackTime_ = 0.0f;   // 攻撃モーション時間
    private float idleTime_   = 0.0f;   // 左右振り向きモーション時間
    private float hitTime_    = 0.0f;   // ダメージを受けたときのモーション時間
    private int myHP = 10;              // テスト用HP
    int[] AnimParamHash_ = new int[5];  // アニメーション名

    void Start()
    {
        animator_ = GetComponent<Animator>();
        AnimParamHash_[0] = Animator.StringToHash("isAttack");
        AnimParamHash_[1] = Animator.StringToHash("isRun");
        AnimParamHash_[2] = Animator.StringToHash("isIdle");
        AnimParamHash_[3] = Animator.StringToHash("isDamage");
        AnimParamHash_[4] = Animator.StringToHash("isDeath");
    }

    void Update()
    {
        Attack();
        //DirRightLeft();
        Damage();

        // テスト用アニメーション切り替え
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator_.SetBool(AnimParamHash_[0], true);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            animator_.SetBool(AnimParamHash_[1], true);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            animator_.SetBool(AnimParamHash_[2], true);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            myHP -= 5;  // HPの減少処理

            if (myHP > 0)    // 0より大きいとき
            {
                // 攻撃を受けたときに、自分のHPを見てHPが1以上ならHit→待機,0以下ならHit->Deathにする
                animator_.SetBool(AnimParamHash_[3], true);
            }
            else            // 0以下
            {
                // Deathにする
                animator_.SetBool(AnimParamHash_[4], true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            animator_.SetBool(AnimParamHash_[4], true);
        }
    }

    // 攻撃モーション処理
    private void Attack()
    {
        // 攻撃モーションが1回発生したら、移動に戻す
        // (移動から待機に戻す処理は入れてないから、定位置に戻ったら待機になるようにする)
        if (animator_.GetBool(AnimParamHash_[0]))
        {
            if (attackTime_ < 0.5f)
            {
                attackTime_ += Time.deltaTime;
            }
            else
            {
                animator_.SetBool(AnimParamHash_[0], false);
                attackTime_ = 0.0f;
            }
        }
    }

    // 待機時間が一定以上で左右振り向きを発生させる処理
    private void DirRightLeft()
    {
        bool tmp = false;
        for (int i = 0; i < AnimParamHash_.Length; i++)
        {
            // 1つでもtrueなら終了する
            if (animator_.GetBool(AnimParamHash_[i]))
            {
                tmp = true;
                break;
            }
        }
        if (!tmp)    // どのモーションもしていないとき
        {
            if (idleTime_ < 5.0f)
            {
                idleTime_ += Time.deltaTime;
            }
            else
            {
                // 左右振り向き
                animator_.SetBool(AnimParamHash_[2], true);
                idleTime_ = 2.0f;
            }
        }

        // 左右振り向きから待機に戻る
        if (animator_.GetBool(AnimParamHash_[2]))
        {
            if (idleTime_ > 0.0f)
            {
                idleTime_ -= Time.deltaTime;
            }
            else
            {
                animator_.SetBool(AnimParamHash_[2], false);
                idleTime_ = 0.0f;
            }
        }
    }

    // 攻撃を受けた際のモーション処理
    private void Damage()
    {
        if (animator_.GetBool(AnimParamHash_[3]))
        {
            if (hitTime_ < 0.5f)
            {
                hitTime_ += Time.deltaTime;
            }
            else
            {
                // すべてのフラグをfalseへ
                for (int i = 0; i < AnimParamHash_.Length; i++)
                {
                    animator_.SetBool(AnimParamHash_[i], false);
                }
            }
        }
    }
}