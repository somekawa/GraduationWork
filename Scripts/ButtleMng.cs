using System.Collections.Generic;
using UnityEngine;

// 戦闘全体について管理する

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public Canvas fieldUICanvas;            // 表示/非表示をこのクラスで管理される

    public int debugEnemyNum = 1;           // インスペクターから敵の生成数を変えれるように 

    private bool setCallOnce_ = false;      // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

    //private Transform buttleCommandUI_;         // 金の大枠を含めた情報を取得

    private CharacterMng characterMng_;         // キャラクター管理クラスの情報
    private EnemyInstanceMng enemyInstanceMng_; // 敵インスタンス管理クラスの情報

    private List<(int,string)> moveTurnList_ = new List<(int, string)>();   // キャラと敵の行動順をまとめるリスト
    private int moveTurnCnt_ = 0;           // 自分の行動が終わったら値を増やす
    private int damageNum_ = 0;             // ダメージの値
    private int speedNum_ = 0;              // 命中判定用の値
    private int luckNum_ = 0;               // 幸運値の値
    private int element_ = 0;               // エレメント情報

    private bool lastEnemyFlg_;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        //buttleCommandUI_ = buttleUICanvas.transform.Find("Command");
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
            lastEnemyFlg_ = false;
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            moveTurnList_.Clear();
            moveTurnCnt_ = 0;
            damageNum_ = 0;

            characterMng_.ButtleSetCallOnce();

            // 敵のインスタンス(1～4)
            // イベント戦闘の場合、敵の数が異なる場合があるので返り値で正しい値を受け取るようにする
            var correctEnemyNum = enemyInstanceMng_.EnemyInstance(debugEnemyNum, buttleUICanvas);

            // 敵の名前と行動速度を受け取ってリストに入れる
            for (int i = 0; i < correctEnemyNum; i++)
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
            characterMng_.SetEnemyNum(correctEnemyNum);
        }

        // 自分の行動外のときにも呼ばれる関数
        characterMng_.NotMyTurn();
        enemyInstanceMng_.NotMyTurn();

        if (moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // キャラクターの行動
            characterMng_.Buttle();
            // コマンド状態の表示/非表示切替
            //buttleCommandUI_.gameObject.SetActive(!characterMng_.GetSelectFlg());

            // キャラクターの攻撃対象が最後の敵だった時
            if (lastEnemyFlg_)
            {
                if(enemyInstanceMng_.AllAnimationFin())
                {
                    enemyInstanceMng_.DeleteEnemy();

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

        // 行動が切り替わる毎に、敵の状態を確認する
        lastEnemyFlg_ = characterMng_.GetLastEnemyToAttackFlg();
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

    public void SetSpeedNum(int num)
    {
        Debug.Log("*SetSpeedNum" + num);
        speedNum_ = num;
    }

    public int GetSpeedNum()
    {
        Debug.Log("***GetSpeedNum" + speedNum_);
        return speedNum_;
    }

    public void SetLuckNum(int num)
    {
        Debug.Log("*SetLuckNum" + num);
        luckNum_ = num;
    }

    public int GetLuckNum()
    {
        Debug.Log("***GetLuckNum" + luckNum_);
        return luckNum_;
    }

    public void SetElement(int num)
    {
        Debug.Log("*SetElement" + num);
        element_ = num;
    }

    public int GetElement()
    {
        Debug.Log("***GetElement" + element_);
        return element_;
    }

    // ユニたちが逃げる処理の時に使用する
    public void CallDeleteEnemy()
    {
        enemyInstanceMng_.DeleteEnemy();
    }
}
