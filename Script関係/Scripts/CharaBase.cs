using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    // ���O
    private string name;
    // �ԍ�
    private CharacterMng.CharcterNum charNum;

    public struct CharacterSetting
    {
        public Animator animator;   // �e�L�����ɂ��Ă���Animator�̃R���|�[�l���g
        public bool isMove;         // Wait���[�V��������false
        public float animTime;      // ���̃L�����̍s���ɑJ�ڂ���܂ł̊�
        public Vector3 buttlePos;   // �퓬�J�n���ɐݒ肳���|�W�V����(�U���G�t�F�N�g����Instance�ʒu�ɗ��p)
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

    // CharaBase�N���X���L�̊֐�(���O���擾�j
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

    // ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract void Defence();
    public abstract void Magic();
    public abstract void Item();
    public abstract bool ChangeNextChara(); // ���̃L�����̑���ɐ؂�ւ���ׂ̏�������

    //// override�o����֐�
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("���ꂩ��b���܂�");
    //}

    //// ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    //public abstract void Walk();
}