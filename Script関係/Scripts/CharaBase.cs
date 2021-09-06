using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    // 名前
    private string name;
    // 番号
    private CharacterMng.CharcterNum charNum;

    public struct CharacterSetting
    {
        public Animator animator;   // 各キャラについているAnimatorのコンポーネント
        public bool isMove;         // Waitモーション時はfalse
        public float animTime;      // 次のキャラの行動に遷移するまでの間
        public Vector3 buttlePos;   // 戦闘開始時に設定されるポジション(攻撃エフェクト等のInstance位置に利用)
    }

    private CharacterSetting setting;

    public CharaBase(string name, CharacterMng.CharcterNum num, CharacterSetting set)
    {
        this.name = name;
        charNum = num;

        setting.animator = set.animator;
        setting.isMove = set.isMove;
        setting.animTime = set.animTime;
        setting.buttlePos = set.buttlePos;
    }

    // CharaBaseクラス特有の関数(名前を取得）
    public string GetName()
    {
        return name;
    }

    public CharacterMng.CharcterNum GetNum()
    {
        return charNum;
    }

    public CharacterSetting GetSetting()
    {
        return setting;
    }

    // 抽象関数(中身は継承先で実装する必要がある）
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract void Defence();
    public abstract void Magic();
    public abstract void Item();
    public abstract bool ChangeNextChara(); // 次のキャラの操作に切り替える為の準備処理

    //// override出来る関数
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("これから話します");
    //}

    //// 抽象関数(中身は継承先で実装する必要がある）
    //public abstract void Walk();
}