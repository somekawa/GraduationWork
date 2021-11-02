using UnityEngine;

public class ForcedButtle : MonoBehaviour
{
    public GameObject eventEnemy;                // 強制戦闘時の敵をアタッチする

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            // 敵の種類と数を指定する
            GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().SetEnemySpawn(eventEnemy,1);

            // 強制戦闘が発生する
            FieldMng.nowMode = FieldMng.MODE.BUTTLE;
            Debug.Log("ユニが強制戦闘用の壁を通過しました");

            // オブジェクトを非アクティブにする(非アクティブにしないと、連続で戦闘が発生する)
            this.gameObject.SetActive(false);
        }
    }
}
