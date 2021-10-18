using UnityEngine;

// 攻撃が当たったかの確認用

public class CheckAttackHit : MonoBehaviour
{
    // 目標の敵か判別する番号
    private int targetNum_;

    public void SetTargetNum(int num)
    {
        targetNum_ = num;
    }

    void OnTriggerEnter(Collider col)
    {
        // 敵に当たった場合(col.tag == "Enemy"と書くより、処理が速い)
        if (col.CompareTag("Enemy"))
        {
            // 目標の敵に当たった場合
            if (targetNum_ == int.Parse(col.name))
            {
                Debug.Log("Hit");
                Destroy(col.gameObject);

                // 魔法の弾の時だけ魔法の弾を削除する
                if(this.gameObject.name == "UniAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }

                col = null; // Destroy後にnull代入処理
            }
        }
    }
}
