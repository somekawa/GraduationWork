using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isRun = Animator.StringToHash("isRun");      // �ړ����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDamage = Animator.StringToHash("isDamage");// �_���[�W���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDeath = Animator.StringToHash("isDeath");  // ���S���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)

    private CharacterSetting set_;                  // �L�������̏��       
    private bool deathFlg_ = false;                 // ���S��Ԃ��m�F����ϐ�

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

    // �e�X�g�p
    public void sethp(int hp)
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
    public override int Defence()
    {
        return set_.Defence;
    }
    public override void Magic()
    {
        Debug.Log("���@�I");
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
}