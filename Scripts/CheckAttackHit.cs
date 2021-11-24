using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 攻撃が当たったかの確認用

public class CheckAttackHit : MonoBehaviour
{
    // 目標の敵か判別する番号
    private int targetNum_ = -1;

    // 敵エフェクトの削除リスト
    private readonly List<string> enemyEffectDeleteList_ = new List<string>()
    {
        "KabosuAttack(Clone)","SpiderAttack(Clone)"
    };

    // キャラの魔法エフェクトの削除
    private string charaMagicStr_ = "";

    public void SetTargetNum(int num)
    {
        targetNum_ = num;
    }

    public void SetCharaMagicStr(string str)
    {
        charaMagicStr_ = str;
    }

    void OnTriggerEnter(Collider col)
    {
        // 敵に当たった場合(col.tag == "Enemy"と書くより、処理が速い)
        if (col.CompareTag("Enemy"))
        {
            for(int i = 0; i < enemyEffectDeleteList_.Count; i++)
            {
                if (this.gameObject.name == enemyEffectDeleteList_[i])
                {
                    return;
                }
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
                else if(this.gameObject.name == charaMagicStr_)
                {
                    if (int.Parse(charaMagicStr_.Split('-')[0]) == 4)    // 土魔法なら
                    {
                        // アニメーション終了まで削除待つ
                        this.gameObject.GetComponent<MagicMove>().MoveStop();
                    }
                    else
                    {
                        // 土魔法以外はすぐに削除する(現状は、)
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    // 武器コライダーの有効化(多段ヒットを防ぐ)
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }

                targetNum_ = -1;
            }
        }
        else if (col.CompareTag("Player"))
        {
            // targetNum_が-1ということは、攻撃対象が設定されていない状態だから、returnを返す
            if(targetNum_ < 0)
            {
                return;
            }

            if (this.gameObject.name == "UniAttack(Clone)" || this.gameObject.name == charaMagicStr_)
            {
                return;
            }

            // 目標のキャラに当たった場合
            if (SceneMng.charasList_[targetNum_].Name() == col.name)
            {
                Debug.Log("Hit");

                // HP減少処理
                GameObject.Find("CharacterMng").GetComponent<CharacterMng>().HPdecrease(targetNum_);

                // 自分の名前がBombMonsterであれば、自爆して自分も削除する
                if (gameObject.name == "BombMonster")
                {
                    GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().HPdecrease(int.Parse(transform.root.gameObject.name) - 1);
                }

                for (int i = 0; i < enemyEffectDeleteList_.Count; i++)
                {
                    // 魔法の弾の時だけ魔法の弾を削除する
                    if (this.gameObject.name == enemyEffectDeleteList_[i])
                    {
                        Destroy(this.gameObject);
                    }
                }

                targetNum_ = -1;
            }
        }
    }
}
