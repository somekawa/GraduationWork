using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//　抽象クラス(CharaBase)とインタフェース(InterfaceButtle)を継承してCharaクラスを作成
public class Chara : CharaBase,InterfaceButtle
{
    // nameは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使えなかったら削除する。
    public Chara(string name, CharacterMng.CharcterNum num) : base(name,num)
    {
    }

    public void Attack()
    {
        Debug.Log("攻撃！");
    }

    public void Damage()
    {
        Debug.Log("ダメージ受けた！");
    }

    public void HP()
    {
        Debug.Log("HP！");
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
}
