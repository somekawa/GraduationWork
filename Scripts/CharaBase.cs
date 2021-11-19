using UnityEngine;

public abstract class CharaBase : object
{
    private GameObject DataPopPrefab_;

    public struct CharacterSetting
    {
        public Animator animator;   // �e�L�����ɂ��Ă���Animator�̃R���|�[�l���g
        public bool isMove;         // Wait���[�V��������false
        public float animTime;      // ���̃L�����̍s���ɑJ�ڂ���܂ł̊�
        public Vector3 buttlePos;   // �퓬�J�n���ɐݒ肳���|�W�V����(�U���G�t�F�N�g����Instance�ʒu�ɗ��p)

        public string name;
        public int maxHP;           // �ő�HP
        public int maxMP;           // �ő�MP

        // Excel�ǂݍ��݂œ���f�[�^(�ŏI�I�ɂ̓Z�[�u/���[�h�p�ɍ\���̕������ق�����������)
        // �\���̕�������A�\����(CharacterSetting)�̒��ɍ\����(�V�K)�����銴���ɂ���
        public int Level;
        public int HP;
        public int MP;
        public int Attack;          // ���̒l�Ƀ|�C���g��U�蕪����ƍU���͂��オ��
        public int MagicAttack;
        public int Defence;         // ���̒l�Ƀ|�C���g��U�蕪����Ɩh��͂��オ��
        public int Speed;           // ���̒l�Ƀ|�C���g��U�蕪����Ƒf�������オ��
        public int Luck;            // ���̒l�Ƀ|�C���g��U�蕪����ƍK�^���オ��
        public float AnimMax;       // �U�����[�V�����̃t���[�������Ԃɒ������l�������Ă���(���[�V�����؂�ւ��Ŏg�p����)
        public int Magic0;
        public int Magic1;
        public int Magic2;
        public int Magic3;

        // �G�p�̏��
        public int Exp;             // ���̓G��|�����ۂɃL������������o���l
        public string Drop;         // ���̓G��|�����ۂɃL������������h���b�v�f��
        public float MoveTime;      // �ړ�����deltaTime������l
        public float MoveDistance;  // �L�����Ƃ̋������e�͈�
        public string WeaponTagObjName;  // CheckAttackHit.cs���A�^�b�`����Ă���I�u�W�F�N�g�̖��O
    }

    public enum ANIMATION
    {
        NON,
        IDLE,
        BEFORE,
        ATTACK,
        AFTER,
        //DEATH
    };

    private CharacterSetting setting_;

    public CharaBase(string name,int objNum, Animator animator,EnemyList.Param param)
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������

        if(objNum == 0)
        {
            // �L�����N�^�[
            var popCharacter = DataPopPrefab_.GetComponent<PopList>().GetData<CharacterList>(PopList.ListData.CHARACTER, 0, name);
            setting_.name = name;
            setting_.Level = popCharacter.param[0].Level;
            setting_.HP = popCharacter.param[0].HP;
            setting_.MP = popCharacter.param[0].MP;
            setting_.Attack = popCharacter.param[0].Attack;
            setting_.MagicAttack = popCharacter.param[0].MagicAttack;
            setting_.Defence = popCharacter.param[0].Defence;
            setting_.Speed = popCharacter.param[0].Speed;
            setting_.Luck = popCharacter.param[0].Luck;
            setting_.AnimMax = popCharacter.param[0].AnimMax;

            setting_.animator = animator;
            setting_.isMove = false;
            setting_.animTime = 0.0f;
            //setting.buttlePos = set.buttlePos;    // �ݒ�̃^�C�~���O���قȂ�

            setting_.maxHP = setting_.HP;
            setting_.maxMP = setting_.MP;
        }
        else
        {
            // �G���
            setting_.name = param.Name + "_" +name;
            setting_.Level = param.Level;
            setting_.HP = param.HP;
            setting_.MP = param.MP;
            setting_.Attack = param.Attack;
            setting_.MagicAttack = param.MagicAttack;
            setting_.Defence = param.Defence;
            setting_.Speed = param.Speed;
            setting_.Luck = param.Luck;
            setting_.AnimMax = param.AnimMax;

            setting_.Exp = param.Exp;
            setting_.Drop = param.Drop;
            setting_.MoveTime = param.MoveTime;
            setting_.MoveDistance = param.MoveDistance;
            setting_.WeaponTagObjName = param.WeaponTagObjName;

            setting_.animator = animator;
            setting_.isMove = false;
            setting_.animTime = 0.0f;
            //setting.buttlePos = set.buttlePos;    // �ݒ�̃^�C�~���O���قȂ�

            setting_.maxHP = setting_.HP;
            setting_.maxMP = setting_.MP;
        }
    }

    public CharacterSetting GetSetting()
    {
        return setting_;
    }

    // ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract int Defence();
    public abstract void Magic();
    public abstract void Item();
    public abstract bool ChangeNextChara(); // ���̃L�����̑���ɐ؂�ւ���ׂ̏�������
    public abstract void DamageAnim();      // �U�����󂯂��Ƃ��̃��[�V��������

    //// override�o����֐�
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("���ꂩ��b���܂�");
    //}

    //// ���ۊ֐�(���g�͌p����Ŏ�������K�v������j
    //public abstract void Walk();
}