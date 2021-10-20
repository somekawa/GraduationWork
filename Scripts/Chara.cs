using UnityEngine;

//　抽象クラス(CharaBase)とインタフェース(InterfaceButtle)を継承してCharaクラスを作成
public class Chara : CharaBase,InterfaceButtle
{
    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)
    private CharacterSetting set_;                  // キャラ毎の情報            

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Chara(string name, int objNum, Animator animator) : base(name,objNum,animator,null)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する
    }

    public bool Attack()
    {
        Debug.Log(set_.name + "の攻撃！");
        set_.animator.SetBool(key_isAttack, true);
        // isMoveがfalseのときだけ攻撃エフェクトのInstanceとisMoveのtrue化処理を行うようにして、
        // エフェクトがボタン連打で大量発生するのを防ぐ
        if (!set_.isMove)
        {
            //AttackStart((int)nowTurnChar_); // characterMng.cs側でやる
            set_.isMove = true;
            //selectFlg_ = false; // characterMng.cs側でやる
            return true;
        }
        return false;
    }

    public void Damage()
    {
        Debug.Log(set_.name + "はダメージ受けた！");
    }

    public int HP()
    {
        return set_.HP;
    }

    public int MaxHP()
    {
        return set_.maxHP;
    }

    public (Vector3,bool) RunMove(float time,Vector3 myPos, Vector3 targetPos)
    {
        if (!set_.animator.GetBool("isRun"))
        {
            set_.animator.SetBool("isRun", true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 1.5f)   // 目標地点0.0fにすると敵と衝突してお互い吹っ飛んでいくから1.5fにしている
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("ジャック目的敵到着");
            set_.animator.SetBool("isRun", false);
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
        if (!set_.animator.GetBool("isRun"))
        {
            set_.animator.SetBool("isRun", true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 0.1f)  // 目標地点0.0fにすると誤差で辿り着けなくなるから0.1fにしている
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("ジャック目的敵到着");
            set_.animator.SetBool("isRun", false);
            return (Vector3.Lerp(myPos, targetPos, 1.0f),true);
        }
        else
        {
            // キャラの座標移動
            return (Vector3.Lerp(myPos, targetPos, time),false);
        }
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
    public override void Defence()
    {
        Debug.Log("防御！");
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
        // 再生時間用に間を空ける
        // キャラ毎に必要な間が違うかもしれないから、1.0fの所は外部データ読み込みで、charSetting[(int)nowTurnChar_].maxAnimTimeとかを用意したほうがいい
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
            set_.animator.SetBool(key_isAttack, false);

            return true;

            // CharacterMng.cs側でやる
            // 次のキャラが行動できるようにする
            // 最大まで加算されたら、初期値に戻す(前演算子重要)
            //if (++nowTurnChar_ >= CharcterNum.MAX)
            //{
            //    nowTurnChar_ = CharcterNum.UNI;
            //}
        }
    }

    public Vector3 GetButtlePos()
    {
        return set_.buttlePos;
    }

    public void SetButtlePos(Vector3 pos)
    {
        set_.buttlePos = pos;
    }

    // 戦闘の始めに初期化する
    public void SetTurnInit()
    {
        set_.isMove = false;
        set_.animTime = 0.0f;
    }

    public bool GetIsMove()
    {
        return set_.isMove;
    }

    // SceneMng.csから呼び出す
    public CharacterSetting GetCharaSetting()
    {
        return set_;
    }

    // SceneMng.csから呼び出す
    public void SetCharaSetting(CharacterSetting set)
    {
        set_.name = set.name;
        set_.Level = set.Level;
        set_.HP = set.HP;
        set_.MP = set.MP;
        set_.Attack = set.Attack;
        set_.Defence = set.Defence;
        set_.Speed = set.Speed;
        set_.Luck = set.Luck;
        set_.AnimMax = set.AnimMax;

        SetTurnInit();
        // アニメーターはセットしてはいけない
        //setting_.animator = animator;
    }

}
