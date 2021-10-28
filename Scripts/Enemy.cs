using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)
    private CharacterSetting set_;                  // キャラ毎の情報            

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Enemy(string name,int objNum, Animator animator,EnemyList.Param param) : base(name,objNum, animator,param)
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
                //set_.animator.SetBool(key_isAttack, true);
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
        return (Vector3.Lerp(myPos, targetPos, time), true);
    }

    public (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        return (Vector3.Lerp(myPos, targetPos, 1.0f), true);
    }

    // テスト用
    public void sethp(int hp)
    {
        set_.HP = hp;
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
            //set_.animator.SetBool(key_isAttack, false);

            return true;
        }
    }

    public int Speed()
    {
        return set_.Speed;
    }

    public string Name()
    {
        return set_.name;
    }
}
