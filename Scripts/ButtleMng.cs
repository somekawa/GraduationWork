using System.Collections.Generic;
using UnityEngine;

// 戦闘全体について管理する

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public Canvas fieldUICanvas;            // 表示/非表示をこのクラスで管理される

    private int instanceEnemyNum_;          // 敵の出現数調整(フィールド毎にStart関数で調整する)

    private bool setCallOnce_ = false;      // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

    private CharacterMng characterMng_;         // キャラクター管理クラスの情報
    private EnemyInstanceMng enemyInstanceMng_; // 敵インスタンス管理クラスの情報

    private Transform EneSelObj_;           // 敵の指定マーク

    private List<(int, string)> moveTurnList_ = new List<(int, string)>();   // キャラと敵の行動順をまとめるリスト
    private int moveTurnCnt_ = 0;           // 自分の行動が終わったら値を増やす
    private int damageNum_ = 0;             // ダメージの値
    private int speedNum_ = 0;              // 命中判定用の値
    private int luckNum_ = 0;               // 幸運値の値
    private int element_ = 0;               // エレメント情報
    private (int, int) badStatusNum_;       // 状態異常の数字
    private int refNum_ = -1;               // 攻撃反射対象の番号を保存する変数
    private bool autoHitFlg_ = false;       // 命中効果のフラグ
    private Vector3 keepPos_;
    private bool isAttackMagicFlg_ = false; // 敵の攻撃が物理か魔法かを判定する(近距離は物理:false,遠距離は魔法:true)

    private bool lastEnemyFlg_;

    public static string forcedButtleWallName;

    private ButtleResult buttleResult_;     // 戦闘リザルト用
    private bool resultFlg_ = false;        // リザルト処理に1度入ったらtrueにする
    private int[] saveEnemyNum_ = new int[5];
    private bool bossFlag_ = false;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();
        buttleUICanvas.gameObject.SetActive(false);
        buttleResult_ = gameObject.GetComponent<ButtleResult>();
    }

    void Update()
    {
        // FieldMngで遭遇タイミングを調整しているため、それを参照し、戦闘モード以外ならreturnする
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE && FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
        {
            setCallOnce_ = false;

            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // 戦闘開始時に設定される項目
        if (!setCallOnce_)
        {
            resultFlg_ = false;
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

            // 敵数の上限を設定する(Start関数では完全にシーンが移動しきれていないので値が正しくならない)
            if (SceneMng.nowScene == SceneMng.SCENE.FIELD0)
            {
                instanceEnemyNum_ = 3;  // 3未満(=2体まで)
            }
            else if (SceneMng.nowScene == SceneMng.SCENE.FIELD1)
            {
                instanceEnemyNum_ = 4;  // 4未満(=3体まで)
            }
            else
            {
                instanceEnemyNum_ = 5;  // 5未満(=4体まで)
            }
            var correctEnemyNum = enemyInstanceMng_.EnemyInstance(Random.Range(1, instanceEnemyNum_), buttleUICanvas);

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

            // 順番用の数字を作る為の一時変数
            for (int i = 0; i < moveTurnList_.Count; i++)
            {
                // キャラの行動ターン数字を代入する(0からカウントしてるからi+1とする) 
                if (!characterMng_.SetMoveSpeedNum(i + 1, moveTurnList_[i].Item2))
                {
                    // 敵の行動ターン数字を代入する
                    string[] arr = moveTurnList_[i].Item2.Split('_');
                    enemyInstanceMng_.SetMoveSpeedNum(i + 1, arr[1]);
                }
            }

            // Character管理クラスに敵の出現数を渡す
            characterMng_.SetEnemyNum(correctEnemyNum);
        }

        // 自分の行動外のときにも呼ばれる関数
        characterMng_.NotMyTurn();
        enemyInstanceMng_.NotMyTurn(refNum_);

        if (moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // キャラクターの行動
            characterMng_.Buttle();

            // キャラクターの攻撃対象が最後の敵だった時
            if (lastEnemyFlg_)
            {
                if (enemyInstanceMng_.AllAnimationFin() && !resultFlg_)
                {
                    // 現在が強制戦闘中だった時
                    if (FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE)
                    {
                        // リストの中のbool部分をtrue(=取得済み)にする
                        var tmp = FieldMng.forcedButtleWallList;
                        for (int i = 0; i < tmp.Count; i++)
                        {
                            if (tmp[i].Item1 == forcedButtleWallName)
                            {
                                // flagをtrueに上書きする
                                (string, bool) content = (forcedButtleWallName, true);
                                tmp[i] = content;
                            }
                        }
                    }

                    CallDeleteEnemy();

                    // リザルト処理： エネミーの数、エネミーの番号（配列）
                    buttleResult_.DropCheck(EneSelObj_.childCount, saveEnemyNum_, bossFlag_);
                    resultFlg_ = true;
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
        if (++moveTurnCnt_ > moveTurnList_.Count - 1)
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

    public void SetBadStatus(int sub1, int sub2)
    {
        badStatusNum_ = (sub1, sub2);
    }

    public (int, int) GetBadStatus()
    {
        return badStatusNum_;
    }

    public void SetRefEnemyNum(int num)
    {
        refNum_ = num;
    }

    // ユニたちが逃げる処理や戦闘終了時に使用する
    public void CallDeleteEnemy()
    {
        enemyInstanceMng_.DeleteEnemy();
        if (EneSelObj_ == null)
        {
            EneSelObj_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").transform;
        }
        // EnemySelectObjからその戦闘でつかった敵のHPバーとかを削除する
        for (int i = 0; i < EneSelObj_.childCount; ++i)
        {
            Destroy(EneSelObj_.GetChild(i).gameObject);
        }

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            // バトルで死亡したまま終了していたときは、HP1の状態で立ち上がらせる
            if (SceneMng.charasList_[i].GetDeathFlg())
            {
                SceneMng.charasList_[i].SetDeathFlg(false);
                SceneMng.charasList_[i].SetHP(1);
            }
            SceneMng.charasList_[i].ButtleInit();
        }
    }

    // アイテムによる固定ダメージ
    public void ItemDamage(int itemDamageNum)
    {
        SetDamageNum(itemDamageNum);    // ダメージ値をセット
        enemyInstanceMng_.ItemDamage(); // 全エネミー分HPdecrease関数を回すようにする
    }

    public Vector3 GetFieldPos()
    {
        return keepPos_;
    }

    public void SetEnemyNum(int[] num, bool flag)
    {
        saveEnemyNum_ = num;
        bossFlag_ = flag;
    }

    public void SetFieldPos(Vector3 pos)
    {
        keepPos_ = pos;
    }

    public void SetAutoHit(bool flag)
    {
        autoHitFlg_ = flag;
    }

    public bool GetAutoHit()
    {
        return autoHitFlg_;
    }

    public void OnClickItemBackButton()
    {
        // アイテム画面を閉じる
        GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
        buttleUICanvas.transform.Find("ItemBackButton").gameObject.SetActive(false);
    }

    public void SetIsAttackMagicFlg(bool flag)
    {
        isAttackMagicFlg_ = flag;
    }

    public bool GetIsAttackMagicFlg()
    {
        return isAttackMagicFlg_;
    }
}
