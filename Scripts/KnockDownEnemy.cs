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
    }
}
