using UnityEngine;

// ボス討伐系クエストのクリア管理

public class KnockDownEnemy : MonoBehaviour
{
    // オブジェクトが非表示になったときに走る関数
    void OnDisable()
    {
        // インスタンスされるときは[1]っていう番号になるから名前でとれない！！
        // →WeaponタグがつけられてるオブジェクトのWaterMonsterという名前で見よう
        if (gameObject.name == "WaterMonster")  // DesertFieldのボスなら
        {
            // メインクエスト「オアシスを甦らせて」をクリアにする。
            QuestClearCheck.QuestClear(4);
        }
        else if(gameObject.name == "BossGolem")
        {
            // メインクエスト「ゴーレム大量発生」をクリアにする。
            QuestClearCheck.QuestClear(5);
        }
        else if(gameObject.name == "PoisonSlime")
        {
            // メインクエスト「毒の霧を晴らして」をクリアにする。
            QuestClearCheck.QuestClear(6);
        }
        else
        {
            // 何も処理を行わない
        }
    }
}
