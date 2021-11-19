using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// 攻撃モーション(全キャラで名前を揃える必要がある)
    private int key_isRun = Animator.StringToHash("isRun");      // 移動モーション(全キャラで名前を揃える必要がある)
    private int key_isDamage = Animator.StringToHash("isDamage");// ダメージモーション(全キャラで名前を揃える必要がある)
    private int key_isDeath = Animator.StringToHash("isDeath");  // 死亡モーション(全キャラで名前を揃える必要がある)

    private CharacterSetting set_;                  // キャラ毎の情報       
    private bool deathFlg_ = false;                 // 死亡状態か確認する変数

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Enemy(string name, int objNum, Animator animator, EnemyList.Param param) : base(name, objNum, animator, param)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する
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
        return set_.Attack;
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

    // テスト用
    public void sethp(int hp)
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
    public override int Defence()
    {
        return set_.Defence;
    }
    public override void Magic()
    {
        Debug.Log("魔法！");
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
        return set_.Speed;
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
}