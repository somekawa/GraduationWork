using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    private GameObject DataPopPrefab_;

    // ���O
    private string name_;
    // �ԍ�
    private CharacterMng.CharcterNum charNum_;

    public struct CharacterSetting
    {
        public Animator animator;   // �e�L�����ɂ��Ă���Animator�̃R���|�[�l���g
        public bool isMove;         // Wait���[�V��������false
        public float animTime;      // ���̃L�����̍s���ɑJ�ڂ���܂ł̊�
        public Vector3 buttlePos;   // �퓬�J�n���ɐݒ肳���|�W�V����(�U���G�t�F�N�g����Instance�ʒu�ɗ��p)

        // Excel�ǂݍ��݂œ���f�[�^(�ŏI�I�ɂ̓Z�[�u/���[�h�p�ɍ\���̕������ق�����������)
        // �\���̕�������A�\����(CharacterSetting)�̒��ɍ\����(�V�K)�����銴���ɂ���
        public int Level;
        public int HP;
        public int MP;
        public int Constitution;    // ���̒l�Ƀ|�C���g��U�蕪�����HP��������
        public int Power;           // ���̒l�Ƀ|�C���g��U�蕪�����MP��������
        public int Attack;          // ���̒l�Ƀ|�C���g��U�蕪����ƍU���͂��オ��
        public int Defence;         // ���̒l�Ƀ|�C���g��U�蕪����Ɩh��͂��オ��
        public int Speed;           // ���̒l�Ƀ|�C���g��U�蕪����Ƒf�������オ��
        public int Luck;            // ���̒l�Ƀ|�C���g��U�蕪����ƍK�^���オ��
    }

    private CharacterSetting setting_;

    public CharaBase(string name, CharacterMng.CharcterNum num, Animator animator)
    {
        name_ = name;

        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        var popCharacter = DataPopPrefab_.GetComponent<PopList>().GetData<CharacterList>(PopList.ListData.CHARACTER, 0, name);
        setting_.Level = popCharacter.param[0].Level;
        setting_.HP = popCharacter.param[0].HP;
        setting_.MP = popCharacter.param[0].MP;
        setting_.Constitution = popCharacter.param[0].Constitution;
        setting_.Power   = popCharacter.param[0].Power;
        setting_.Attack  = popCharacter.param[0].Attack;
        setting_.Defence = popCharacter.param[0].Defence;
        setting_.Speed   = popCharacter.param[0].Speed;
        setting_.Luck    = popCharacter.param[0].Luck;

        charNum_ = num;

        setting_.animator  = animator;
        setting_.isMove    = false;
        setting_.animTime  = 0.0f;
        //setting.buttlePos = set.buttlePos;    // �ݒ�̃^�C�~���O���قȂ�
    }

    // CharaBase�N���X���L�̊֐�(���O���擾�j
    public string GetName()
    {
        return name_;
    }

    public CharacterMng.CharcterNum GetNum()
    {
        return charNum_;
    }

    public CharacterSetting GetSetting()
    {
        return setting_;
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