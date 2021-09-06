using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    // 名前
    private string name;
    // 番号
    private CharacterMng.CharcterNum charNum;

    public CharaBase(string name = "DefaultName", CharacterMng.CharcterNum num = CharacterMng.CharcterNum.MAX)
    {
        this.name = name;
        charNum = num;
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

    // 抽象関数(中身は継承先で実装する必要がある）
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract void Defence();
    public abstract void Magic();
    public abstract void Item();

    //// override出来る関数
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("これから話します");
    //}

    //// 抽象関数(中身は継承先で実装する必要がある）
    //public abstract void Walk();
}