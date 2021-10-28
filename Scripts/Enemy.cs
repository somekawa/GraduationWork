using UnityEngine;

public class Enemy : CharaBase, InterfaceButtle
{
    private const string key_isAttack = "isAttack"; // �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private CharacterSetting set_;                  // �L�������̏��            

    // name,num,animator�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g��Ȃ�������폜����B
    public Enemy(string name,int objNum, Animator animator,EnemyList.Param param) : base(name,objNum, animator,param)
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
                //set_.animator.SetBool(key_isAttack, true);
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
        return (Vector3.Lerp(myPos, targetPos, time), true);
    }

    public (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        return (Vector3.Lerp(myPos, targetPos, 1.0f), true);
    }

    // �e�X�g�p
    public void sethp(int hp)
    {
        set_.HP = hp;
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
            //set_.animator.SetBool(key_isAttack, false);

            return true;
        }
    }

    public int Speed()
    {
        return set_.Speed;
    }

    public string Name()
    {
        return set_.name;
    }
}
