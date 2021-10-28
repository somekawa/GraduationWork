using System.Collections.Generic;
using UnityEngine;

// 戦闘全体について管理する

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public Canvas fieldUICanvas;            // 表示/非表示をこのクラスで管理される

    public int debugEnemyNum = 1;           // インスペクターから敵の生成数を変えれるように 

    private bool setCallOnce_ = false;      // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

    private Transform buttleCommandUI_;         // 金の大枠を含めた情報を取得

    private CharacterMng characterMng_;         // キャラクター管理クラスの情報
    private EnemyInstanceMng enemyInstanceMng_; // 敵インスタンス管理クラスの情報

    private List<(int,string)> moveTurnList_ = new List<(int, string)>();   // キャラと敵の行動順をまとめるリスト
    private int moveTurnCnt_ = 0;           // 自分の行動が終わったら値を増やす
    private int damageNum_ = 0;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        buttleCommandUI_ = buttleUICanvas.transform.Find("Command");
        buttleUICanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // FieldMngで遭遇タイミングを調整しているため、それを参照し、戦闘モード以外ならreturnする
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            setCallOnce_ = false;

            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // 戦闘開始時に設定される項目
        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            moveTurnList_.Clear();
            moveTurnCnt_ = 0;
            damageNum_ = 0;

            characterMng_.ButtleSetCallOnce();

            // 敵のインスタンス(1〜4)
            enemyInstanceMng_.EnemyInstance(debugEnemyNum, buttleUICanvas);

            // 敵の名前と行動速度を受け取ってリストに入れる
            for (int i = 0; i < debugEnemyNum; i++)
            {
                moveTurnList_.Add(enemyInstanceMng_.EnemyTurnSpeed(i));
            }

            // キャラの名前と行動速度を受け取ってリストに入れる
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                moveTurnList_.Add(characterMng_.CharaTurnSpeed(i));
            }

            moveTurnList_.Sort();    // 昇順にソート
            moveTurnList_.Reverse(); // 降順にするために逆転させる

            // Character管理クラスに敵の出現数を渡す
            characterMng_.SetEnemyNum(debugEnemyNum);
        }

        if(moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // キャラクターの行動
            characterMng_.Buttle();
            // コマンド状態の表示/非表示切替
            buttleCommandUI_.gameObject.SetActive(!characterMng_.GetSelectFlg());

            // キャラクターの攻撃対象が最後の敵だった時
            if (characterMng_.GetLastEnemyToAttackFlg())
            {
                // Enemyタグの数を見て該当する物がない(= 0)なら、MODEを探索に切り替える
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                {
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    characterMng_.SetCharaFieldPos();
                }
            }
        }
        else
        {
            string[] arr = moveTurnList_[moveTurnCnt_].Item2.Split('_');
            // 敵の行動
            enemyInstanceMng_.Buttle(int.Parse(arr[1]) - 1);
        }
    }

    // 行動が終わったときに呼び出されて自動で加算される
    public void SetMoveTurn()
    {
        // 加算値がリストの上限を越えたら0に戻す
        if(++moveTurnCnt_ > moveTurnList_.Count - 1)
        {
            moveTurnCnt_ = 0;
        }
    }

    public void SetDamageNum(int num)
    {
        Debug.Log("*SetDamageNum" + num);
        damageNum_ = num;
    }

    public int GetDamageNum()
    {
        Debug.Log("***GetDamageNum" + damageNum_);
        return damageNum_;
    }
}
