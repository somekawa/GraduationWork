using UnityEngine;

public class SlipDamageGas : MonoBehaviour
{
    private float nowTime_ = 0.0f;                 // 現在の時間
    private const float checkTime_ = 0.5f;         // スリップダメージ発生時間
    private const int slipDamage_ = 2;             // スリップダメージの値
    private UnitychanController player_;           // プレイヤー情報格納用

    void Start()
    {
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (player_ == null)
        {
            Debug.Log("FieldMng.csで取得しているPlayer情報がnullです");
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player")) //col.tag == "Player"と書くより、処理が速い
        {
            if(!player_.GetMoveFlag())
            {
                return;     // プレイヤーが移動中でない場合は、時間を加算しない
            }

            nowTime_ += Time.deltaTime;
            if(nowTime_ >= checkTime_)
            {
                Debug.Log("スリップダメージ");
                for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
                {
                    SceneMng.SetSE(16);
                    if(SceneMng.charasList_[i].HP() < slipDamage_)
                    {
                        continue;   // 現在HPが2未満(=1)のときは減算処理しない
                    }
                    SceneMng.charasList_[i].SetHP(SceneMng.charasList_[i].HP() - slipDamage_);
                }
                nowTime_ = 0.0f;    // 時間初期化
            }
        }
    }
}
