using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    // ���O
    private string name;
    // �ԍ�
    private CharacterMng.CharcterNum charNum;

    public CharaBase(string name = "DefaultName", CharacterMng.CharcterNum num = CharacterMng.CharcterNum.MAX)
    {
        this.name = name;
        charNum = num;
    }

    // CharaBase�N���X���L�̊֐�(���O���擾�j
    public string GetName()
    {
        return name;
    }

    public CharacterMng.CharcterNum GetNum()
    {
        return charNum;
    }

    // ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract void Defence();
    public abstract void Magic();
    public abstract void Item();

    //// override�o����֐�
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("���ꂩ��b���܂�");
    //}

    //// ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    //public abstract void Walk();
}