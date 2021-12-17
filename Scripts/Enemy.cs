using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isRun = Animator.StringToHash("isRun");      // �ړ����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDamage = Animator.StringToHash("isDamage");// �_���[�W���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDeath = Animator.StringToHash("isDeath");  // ���S���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)

    private CharacterSetting set_;                  // �L�������̏��       
    private bool deathFlg_ = false;                 // ���S��Ԃ��m�F����ϐ�
    private readonly string[] bossName = { "YellowKabosu","WaterMonster","BossGolem","BossParty","PoisonSlime" };    // �{�X�̖��O

    // name,num,animator�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g��Ȃ�������폜����B
    public Enemy(string name, int objNum, Animator animator, EnemyList.Param param) : base(name, objNum, animator, param)
    {
        set_ = GetSetting();  // CharaBase.cs����Get�֐��ŏ����ݒ肷��
    }

    public bool Attack()
    {
        if (set_.HP <= 0)
        {
            Debug.Log(set_.name + "�͂��łɎ��S���Ă���");
            return true;
        }
        else
        {
            Debug.Log(set_.name + "�̍U��");
            if (!set_.isMove)
            {
                if (set_.animator != null)
                {
                    set_.animator.SetBool(key_isAttack, true);
                }
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
        if (set_.animator == null)
        {
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }

        // �_���[�W����Ɉړ�����������ۂɂ́A�_���[�W���[�V������false�ɖ߂��Ă���
        if(set_.animator.GetBool(key_isDamage))
        {
            set_.animator.SetBool(key_isDamage, false);
        }

        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= set_.MoveDistance)   // �ڕW�n�_0.0f�ɂ���ƓG�ƏՓ˂��Ă��݂��������ł���
        {
            // �w��ӏ��܂ł�����true��Ԃ�
            Debug.Log("�G�ړI�G����");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }
        else
        {
            // �L�����̍��W�ړ�
            return (Vector3.Lerp(myPos, targetPos, time), false);
        }
    }

    public (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        if (set_.animator == null)
        {
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }

        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 0.5f)  // �ڕW�n�_0.0f�ɂ���ƌ덷�ŒH�蒅���Ȃ��Ȃ邩��0.5f�ɂ��Ă���
        {
            // �w��ӏ��܂ł�����true��Ԃ�
            Debug.Log("�G�ړI�G����");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, 1.0f), true);
        }
        else
        {
            // �L�����̍��W�ړ�
            return (Vector3.Lerp(myPos, targetPos, time), false);
        }
    }

    public void SetHP(int hp)
    {
        set_.HP = hp;

        // �܂��A�j���[�V�������Ȃ��Ƃ��͏����𔲂���
        if (set_.animator == null)
        {
            return;
        }

        if (set_.HP <= 0)
        {
            // ���S�A�j���[�V�����ɐ؂�ւ���
            set_.animator.SetBool(key_isDeath, true);
        }
        else
        {
            // ��_���[�W�A�j���[�V�������J�n
            set_.animator.SetBool(key_isDamage, true);
        }
    }

    // CharaBase�N���X�̒��ۃ��\�b�h����������
    public override void LevelUp()
    {
        Debug.Log("���x���A�b�v�I");
    }

    public override void Weapon()
    {
        Debug.Log("����ؑցI");
    }
    public override int Defence(bool flag)
    {
        return set_.Defence;
    }
    public override int MagicPower()
    {
        return set_.MagicAttack;
    }
    public override void Item()
    {
        Debug.Log("�A�C�e���I");
    }

    // ���̐������Ȃ������B�U��Motion�I���m�F�Ɏg������
    public override bool ChangeNextChara()
    {
        if (!set_.isMove)
        {
            return false;
        }

        // �������牺�́AisMove��true�̏��
        // isMove��true�Ȃ�A�������܂ōU�����[�V�������������Ƃ��킩��
        // �Đ����ԗp�ɊԂ��󂯂�(�G���ɕK�v�ȊԂ��Ⴄ����AAnimMax�͊O���f�[�^�ǂݍ���)
        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            // animTime�l��1.0���������Ƃ�
            // animTime�l�̏������ƁA�U�����[�V�����̏I������������isMove��false�֖߂�
            set_.animTime = 0.0f;
            set_.isMove = false;

            if (set_.animator != null)
            {
                set_.animator.SetBool(key_isAttack, false);
            }

            return true;
        }
    }

    public int Weak()
    {
        return set_.Weak;
    }

    public int Speed()
    {
        return set_.Speed;
    }

    public int Luck()
    {
        return set_.Luck;
    }

    public string Name()
    {
        return set_.name;
    }

    public float MoveTime()
    {
        return set_.MoveTime;
    }

    public string WeaponTagObjName()
    {
        return set_.WeaponTagObjName;
    }

    public bool GetDeathFlg()
    {
        return deathFlg_;
    }

    public void SetDeathFlg(bool flag)
    {
        deathFlg_ = flag;
    }

    public override void DamageAnim()
    {
        // �܂��A�j���[�V�������Ȃ��Ƃ��͏����𔲂���
        if (set_.animator == null)
        {
            return;
        }

        // ���������_���[�W���󂯂ĂȂ��Ƃ��͏����𔲂���
        if (!set_.animator.GetBool(key_isDamage))
        {
            return;
        }

        // ���ԂŃ��[�V�������I�������邩���肷��
        if (set_.animTime < (set_.AnimMax * 0.5f))
        {
            set_.animTime += Time.deltaTime;
        }
        else
        {
            // ��_���[�W�A�j���[�V�������I��
            set_.animator.SetBool(key_isDamage, false);
            set_.animTime = 0.0f;
        }
    }

    public bool DeathAnim()
    {
        // �܂��A�j���[�V�������Ȃ��Ƃ��͏����𔲂���
        if (set_.animator == null)
        {
            return true;
        }

        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            set_.animTime = 0.0f;
            return true;
        }
    }

    public bool HalfAttackAnimTime()
    {
        // �܂��A�j���[�V�������Ȃ��Ƃ��͏����𔲂���
        if (set_.animator == null)
        {
            return false;
        }

        // AnimMax�̔����̒l�����ݒl���z������true��������
        if (set_.animTime >= (set_.AnimMax * 0.5f))
        {
            return true;
        }
        return false;
    }

    public override void SetBS((int,int)num, int hitNum)
    {
        // ���炩�̃o�X�e���ʂ����閂�@��������A���A�L�����̖��������G�̍K�^�l+�����_���l��荂���Ƃ�
        if (num.Item1 >= 0 && hitNum > set_.Luck + Random.Range(0, 80))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item1 - 1].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�G�͏�Ԉُ�ɂ�������");
        }

        if (num.Item2 >= 0 && hitNum > set_.Luck + Random.Range(0, 80))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item2 - 1].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�G�͏�Ԉُ�ɂ�������");
        }

        // ���O�����̃A���_�[�o�[�ŕ�����
        var name = set_.name.Split('_');
        for (int i = 0; i < bossName.Length; i++)
        {
            // ���O��v��(=�{�X)
            if (name[0] == bossName[i])
            {
                // ���������Ƃ���
                set_.condition[(int)CONDITION.DEATH - 1].Item2 = false;
            }
        }
    }

    public override (CONDITION, bool)[] GetBS()
    {
        return set_.condition;
    }

    public override void SetSpeed(int num)
    {
        set_.Speed = num;
    }

    public int Bst()
    {
        return set_.Bst;
    }

    public override void ConditionReset(bool allReset, int targetReset = -1)
    {
        if(allReset)
        {
            set_.condition[(int)CONDITION.NON - 1].Item2 = true;
            set_.condition[(int)CONDITION.POISON - 1].Item2 = false;
            set_.condition[(int)CONDITION.DARK - 1].Item2 = false;
            set_.condition[(int)CONDITION.PARALYSIS - 1].Item2 = false;
            set_.condition[(int)CONDITION.DEATH - 1].Item2 = false;
        }
    }
}