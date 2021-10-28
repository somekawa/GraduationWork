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
            if (this.gameObject.name == "KabosuAttack(Clone)")
            {
                return;
            }

            // 目標の敵に当たった場合
            if (targetNum_ == int.Parse(col.name))
            {
                Debug.Log("Hit");

                // HP減少処理
                GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().HPdecrease(targetNum_ - 1);

                // 魔法の弾の時だけ魔法の弾を削除する
                if(this.gameObject.name == "UniAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    // 武器コライダーの有効化(多段ヒットを防ぐ)
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
        else if (col.CompareTag("Player"))
        {
            if (this.gameObject.name == "UniAttack(Clone)")
            {
                return;
            }

            // 目標のキャラに当たった場合
            if (SceneMng.charasList_[targetNum_].Name() == col.name)
            {
                Debug.Log("Hit");

                // HP減少処理
                GameObject.Find("CharacterMng").GetComponent<CharacterMng>().HPdecrease(targetNum_);

                // 魔法の弾の時だけ魔法の弾を削除する
                if (this.gameObject.name == "KabosuAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }
            }
        }

    }
}
