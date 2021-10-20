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
        Debug.Log(set_.name + "の攻撃！");
        return true;
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

    public override bool ChangeNextChara()
    {
        return true;
    }
}
