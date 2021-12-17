using UnityEngine;

// バッドステータスの効果を発動させるためのクラス

public class BadStatusMng : MonoBehaviour
{
    // EnemyInstanceMng.csやCharacterMng.csで呼び出す(攻撃後)
    public void BadStateMoveAfter<T>((CharaBase.CONDITION, bool)[] bs,T obj,HPMPBar hpmpBar,bool isCharacterFlg)
    {
        // for文でCONDITION分回してtrueになっている場所を探す
        // trueの場所毎に効果を発動させる

        for(int i = 0; i < (int)CharaBase.CONDITION.DEATH; i++)
        {
            if(bs[i].Item2)
            {
                switch(bs[i].Item1)
                {
                    case CharaBase.CONDITION.NON:
                        break;
                    case CharaBase.CONDITION.POISON:
                        if (!isCharacterFlg) // 敵
                        {
                            var enemy = (Enemy)(object)obj;
                            int damage = (int)(enemy.MaxHP() * 0.2f);
                            StartCoroutine(hpmpBar.MoveSlideBar(enemy.HP() - damage));
                            // 内部数値の変更を行う
                            enemy.SetHP(enemy.HP() - damage);

                            Debug.Log(enemy.Name() + "は、毒でHPを" + damage + "減らします");

                        }
                        else // キャラ
                        {
                            var chara = (Chara)(object)obj;
                            int damage = (int)(chara.MaxHP() * 0.2f);
                            StartCoroutine(hpmpBar.MoveSlideBar(chara.HP() - damage));
                            // 内部数値の変更を行う
                            chara.SetHP(chara.HP() - damage);

                            Debug.Log(chara.Name() + "は、毒でHPを" + damage + "減らします");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // EnemyInstanceMng.csやCharacterMng.csで呼び出す(攻撃前)
    public (CharaBase.CONDITION,bool) BadStateMoveBefore<T>((CharaBase.CONDITION, bool)[] bs, T obj, HPMPBar hpmpBar, bool isCharacterFlg)
    {
        (CharaBase.CONDITION, bool) tmpConditionCheck = (CharaBase.CONDITION.NON, false);

        for (int i = 0; i < (int)CharaBase.CONDITION.DEATH; i++)
        {
            if (bs[i].Item2)
            {
                switch (bs[i].Item1)
                {
                    case CharaBase.CONDITION.NON:
                        break;
                    case CharaBase.CONDITION.DARK:
                        if (!isCharacterFlg) // 敵
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "は、暗闇状態なので命中/回避の値を減らします");
                            // speedの値を現在の半分にする
                            // 行動速度には影響ないはず
                            enemy.SetSpeed(enemy.Speed() / 2);
                        }
                        else // キャラ
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "は、暗闇状態なので命中/回避の値を減らします");
                            chara.SetSpeed(chara.Speed() / 2);
                        }
                        break;
                    case CharaBase.CONDITION.PARALYSIS:
                        int rand = UnityEngine.Random.Range(0, 100);

                        if (!isCharacterFlg) // 敵
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "は、麻痺状態なので動けるか動けないかの判定をします");

                            // 麻痺で動けないときは(CharaBase.CONDITION.PARALYSIS, true),動けるときは後ろのboolをfalseにする
                            if (30 + enemy.Luck() < rand)   // 3割りの確率に自身の幸運値を足して計算
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, true);  // 動けない
                                Debug.Log(30 + enemy.Luck() + "<" + rand + "なので麻痺の効果で動けない");
                            }
                            else
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, false);  // 動ける
                                Debug.Log(30 + enemy.Luck() + ">=" + rand + "なので麻痺の効果中だが、動ける");
                            }
                        }
                        else // キャラ
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "は、麻痺状態なので動けるか動けないかの判定をします");

                            // 麻痺で動けないときは(CharaBase.CONDITION.PARALYSIS, true),動けるときは後ろのboolをfalseにする
                            if (30 + chara.Luck() < rand)   // 3割りの確率に自身の幸運値を足して計算
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, true);  // 動けない
                                Debug.Log(30 + chara.Luck() + "<" + rand + "なので麻痺の効果で動けない");
                            }
                            else
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, false);  // 動ける
                                Debug.Log(30 + chara.Luck() + ">=" + rand + "なので麻痺の効果中だが、動ける");
                            }
                        }
                        break;
                    case CharaBase.CONDITION.DEATH:
                        if (!isCharacterFlg) // 敵
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "は、即死状態なので死亡判定をします");
                            tmpConditionCheck = (CharaBase.CONDITION.DEATH, true); 
                        }
                        else // キャラ
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "は、即死状態なので死亡判定をします");
                            tmpConditionCheck = (CharaBase.CONDITION.DEATH, true);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return tmpConditionCheck;
    }
}
