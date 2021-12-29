using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// 攻撃モーション(全キャラで名前を揃える必要がある)
    private int key_isRun = Animator.StringToHash("isRun");      // 移動モーション(全キャラで名前を揃える必要がある)
    private int key_isDamage = Animator.StringToHash("isDamage");// ダメージモーション(全キャラで名前を揃える必要がある)
    private int key_isDeath = Animator.StringToHash("isDeath");  // 死亡モーション(全キャラで名前を揃える必要がある)

    private CharacterSetting set_;                  // キャラ毎の情報       
    private bool deathFlg_ = false;                 // 死亡状態か確認する変数
    private readonly string[] bossName_ = { "YellowKabosu","WaterMonster","BossGolem","BossParty","PoisonSlime" };    // ボスの名前
    private readonly int[] statusMap_ = new int[4];
    private Dictionary<int, (int, int)> Debuffmap_ 
        = new Dictionary<int, (int, int)>();   // デバフ後の値とターン数を管理する<ワード順,(効果値,デバフターン数)>

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Enemy(string name, int objNum, Animator animator, EnemyList.Param param) : base(name, objNum, animator, param)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する

        statusMap_[0] = set_.Attack;
        statusMap_[1] = set_.MagicAttack;
        statusMap_[2] = set_.Defence;
        statusMap_[3] = set_.Speed;

        for(int i = 0; i < statusMap_.Length; i++)
        {
            Debuffmap_.Add(i+1, (statusMap_[i], -1));  // ワードの順番と揃える
        }
    }

    public bool Attack()
    {
        if (set_.HP <= 0)
        {
            Debug.Log(set_.name + "はすでに死亡している");
            return true;
        }
        else
        {
            Debug.Log(set_.name + "の攻撃");
            if (!set_.isMove)
            {
                if (set_.animator != null)
                {
                    set_.animator.SetBool(key_isAttack, true);
                }
                set_.isMove = true;
                return true;
            }
        }
        return false;
    }

    public int Damage()
    {
        return Debuffmap_[1].Item1;
        //return set_.Attack;
    }

    public int HP()
    {
        return set_.HP;
    }

    public int MaxHP()
    {
        return set_.maxHP;
    }

    public (Vector3, bool) RunMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        if (set_.animator == null)
        {
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }

        // ダメージ直後に移動処理をする際には、ダメージモーションをfalseに戻しておく
        if(set_.animator.GetBool(key_isDamage))
        {
            set_.animator.SetBool(key_isDamage, false);
        }

        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= set_.MoveDistance)   // 目標地点0.0fにすると敵と衝突してお互い吹っ飛んでいく
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("敵目的敵到着");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }
        else
        {
            // キャラの座標移動
            return (Vector3.Lerp(myPos, targetPos, time), false);
        }
    }

    public (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        if (set_.animator == null)
        {
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }

        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 0.5f)  // 目標地点0.0fにすると誤差で辿り着けなくなるから0.5fにしている
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("敵目的敵到着");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, 1.0f), true);
        }
        else
        {
            // キャラの座標移動
            return (Vector3.Lerp(myPos, targetPos, time), false);
        }
    }

    public void SetHP(int hp)
    {
        set_.HP = hp;

        // まずアニメーションがないときは処理を抜ける
        if (set_.animator == null)
        {
            return;
        }

        if (set_.HP <= 0)
        {
            // 死亡アニメーションに切り替える
            set_.animator.SetBool(key_isDeath, true);
        }
        else
        {
            // 被ダメージアニメーションを開始
            set_.animator.SetBool(key_isDamage, true);
        }
    }

    // CharaBaseクラスの抽象メソッドを実装する
    public override void LevelUp()
    {
        Debug.Log("レベルアップ！");
    }

    public override void Weapon()
    {
        Debug.Log("武器切替！");
    }
    public override int Defence(bool flag)
    {
        return Debuffmap_[3].Item1;
        //return set_.Defence;
    }
    public override int MagicPower()
    {
        return Debuffmap_[2].Item1;
        //return set_.MagicAttack;
    }
    public override void Item()
    {
        Debug.Log("アイテム！");
    }

    // 名称正しくないかも。攻撃Motion終了確認に使いたい
    public override bool ChangeNextChara()
    {
        if (!set_.isMove)
        {
            return false;
        }

        // ここから下は、isMoveがtrueの状態
        // isMoveがtrueなら、さっきまで攻撃モーションだったことがわかる
        // 再生時間用に間を空ける(敵毎に必要な間が違うから、AnimMaxは外部データ読み込む)
        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            // animTime値が1.0を上回ったとき
            // animTime値の初期化と、攻撃モーションの終了処理をしてisMoveをfalseへ戻す
            set_.animTime = 0.0f;
            set_.isMove = false;

            if (set_.animator != null)
            {
                set_.animator.SetBool(key_isAttack, false);
            }

            return true;
        }
    }

    public int Weak()
    {
        return set_.Weak;
    }

    public int Speed()
    {
        return Debuffmap_[4].Item1;
        //return set_.Speed;
    }

    public int Luck()
    {
        return set_.Luck;
    }

    public string Name()
    {
        return set_.name;
    }

    public float MoveTime()
    {
        return set_.MoveTime;
    }

    public string WeaponTagObjName()
    {
        return set_.WeaponTagObjName;
    }

    public bool GetDeathFlg()
    {
        return deathFlg_;
    }

    public void SetDeathFlg(bool flag)
    {
        deathFlg_ = flag;
    }

    public override void DamageAnim()
    {
        // まずアニメーションがないときは処理を抜ける
        if (set_.animator == null)
        {
            return;
        }

        // そもそもダメージを受けてないときは処理を抜ける
        if (!set_.animator.GetBool(key_isDamage))
        {
            return;
        }

        // 時間でモーションを終了させるか判定する
        if (set_.animTime < (set_.AnimMax * 0.5f))
        {
            set_.animTime += Time.deltaTime;
        }
        else
        {
            // 被ダメージアニメーションを終了
            set_.animator.SetBool(key_isDamage, false);
            set_.animTime = 0.0f;
        }
    }

    public bool DeathAnim()
    {
        // まずアニメーションがないときは処理を抜ける
        if (set_.animator == null)
        {
            return true;
        }

        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            set_.animTime = 0.0f;
            return true;
        }
    }

    public bool HalfAttackAnimTime()
    {
        // まずアニメーションがないときは処理を抜ける
        if (set_.animator == null)
        {
            return false;
        }

        // AnimMaxの半分の値を現在値が越えたらtrueをかえす
        if (set_.animTime >= (set_.AnimMax * 0.5f))
        {
            return true;
        }
        return false;
    }

    public override void SetBS((int,int)num, int hitNum)
    {
        // 何らかのバステ効果がある魔法があたる、かつ、キャラの命中率が敵の幸運値+ランダム値より高いとき
        if (num.Item1 > 0 && hitNum > set_.Luck + Random.Range(0, 80))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item1 - 1].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("敵は状態異常にかかった");
        }

        if (num.Item2 > 0 && hitNum > set_.Luck + Random.Range(0, 80))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item2 - 1].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("敵は状態異常にかかった");
        }

        // 名前部分のアンダーバーで分ける
        var name = set_.name.Split('_');
        for (int i = 0; i < bossName_.Length; i++)
        {
            // 名前一致時(=ボス)
            if (name[0] == bossName_[i])
            {
                // 即死無効とする
                set_.condition[(int)CONDITION.DEATH - 1].Item2 = false;
            }
        }
    }

    public override (CONDITION, bool)[] GetBS()
    {
        return set_.condition;
    }

    public override void SetSpeed(int num)
    {
        set_.Speed = num;
    }

    public int Bst()
    {
        return set_.Bst;
    }

    public override void ConditionReset(bool allReset, int targetReset = -1)
    {
        if (allReset)
        {
            set_.condition[(int)CONDITION.NON - 1].Item2 = true;
            set_.condition[(int)CONDITION.POISON - 1].Item2 = false;
            set_.condition[(int)CONDITION.DARK - 1].Item2 = false;
            set_.condition[(int)CONDITION.PARALYSIS - 1].Item2 = false;
            set_.condition[(int)CONDITION.DEATH - 1].Item2 = false;
        }
        else
        {
            // 特定の状態異常だけ回復する
            if (targetReset > -1)
            {
                set_.condition[targetReset].Item2 = false;
            }
        }
    }

    public override (int, bool) SetBuff(int tail, int buff)
    {
        bool flag = false;
        // バフ/デバフ内容を反映させる
        // まずは効果量の設定として、威力数字をつくる
        // 小(0)->2割,中(1)->4割,大(2)->6割とするには、tail+1*2にすればいい
        float alphaNum = (((float)Debuffmap_[buff].Item1 - (float)statusMap_[buff - 1]) / (float)statusMap_[buff - 1]) * 10.0f;   // デバフの重ね掛け用
        float buffNum = ((tail + 1) * 2 + alphaNum) * 0.1f;

        float num = statusMap_[buff - 1] * buffNum;
        if(num <= 1.0f)
        {
            num = 1.0f;
        }

        // デバフの重ね掛けかを残りの効果ターン数をみて判断する
        if (Debuffmap_[buff].Item2 > 0)
        {
            // 各項目に応じた低下処理(固定で+1ターンの延長としておく)
            Debuffmap_[buff] = (statusMap_[buff - 1] - (int)num, Debuffmap_[buff].Item2 + 1);
            flag = true;
        }
        else
        {
            // 各項目に応じた低下処理(固定で3ターン後回復としておく)
            Debuffmap_[buff] = (statusMap_[buff - 1] - (int)num, 1);
        }

        // (元値 - 現在値) / 元値 = 倍率 * 100%
        float tmp = (((float)statusMap_[buff - 1] - (float)Debuffmap_[buff].Item1) / (float)statusMap_[buff - 1]) * 100.0f;

        // どの％領域にいるかで返す数字を変更する
        if (tmp > 0 && tmp <= 30)
        {
            tmp = 0;
        }
        else if (tmp > 30 && tmp <= 70)
        {
            tmp = 1;
        }
        else if (tmp > 70 && tmp <= 100)
        {
            tmp = 2;
        }
        else
        {
            tmp = -1;
        }

        return ((int)tmp, flag);
    }

    public override bool CheckBuffTurn()
    {
        bool flag = true;
        for(int i = 1; i <= Debuffmap_.Count; i++)
        {
            // もしバフが継続していたら
            if(Debuffmap_[i].Item2 > 0)
            {
                // -1ターンする
                Debuffmap_[i] = (Debuffmap_[i].Item1, Debuffmap_[i].Item2 - 1);

                if(Debuffmap_[i].Item2 <= 0)    // 先ほどの-1で0以下になったならば
                {
                    Debug.Log("敵のデバフが解除されました");
                    Debuffmap_[i] = (statusMap_[i - 1],-1);
                    flag = false;
                }
            }
        }
        return flag;
    }

    public override Dictionary<int, (int, int)> GetBuff()
    {
        return Debuffmap_;
    }
}