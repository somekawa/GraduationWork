using UnityEngine;

// ボス討伐系クエストのクリア管理

public class KnockDownEnemy : MonoBehaviour
{
    private Bag_Word bagWord_;

    void Start()
    {
        // OnDisableでGameObject.Find（）を使用するとエラーがでるから、Start関数でFindしておく
        bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();
    }

    // オブジェクトが非表示になったときに走る関数
    void OnDisable()
    {
        // インスタンスされるときは[1]っていう番号になるから名前でとれない！！
        // →WeaponタグがつけられてるオブジェクトのWaterMonsterという名前で見よう
        if (gameObject.name == "WaterMonster")  // DesertFieldのボスなら
        {
            // メインクエスト「オアシスを甦らせて」をクリアにする。
            QuestClearCheck.QuestClear(4);
            // 水
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 1, 6);// 水
        }
        else if(gameObject.name == "BossGolem")
        {
            // メインクエスト「ゴーレム大量発生」をクリアにする。
            QuestClearCheck.QuestClear(5);
            // 土
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 2, 7);// 土
        }
        else if(gameObject.name == "PoisonSlime")
        {
            // メインクエスト「毒の霧を晴らして」をクリアにする。
            QuestClearCheck.QuestClear(6);
            // 風
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 3, 8);// 風
        }
        else if (gameObject.name == "Dragon")
        {
            // メインクエスト「拝啓、ユニちゃん」をクリアにする。
            QuestClearCheck.QuestClear(7);
        }
        else
        {
            // 何も処理を行わない
        }
    }
}
