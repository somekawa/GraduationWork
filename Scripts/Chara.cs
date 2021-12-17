using UnityEngine;

//�@���ۃN���X(CharaBase)�ƃC���^�t�F�[�X(InterfaceButtle)���p������Chara�N���X���쐬
public class Chara : CharaBase,InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isRun = Animator.StringToHash("isRun");      // �ړ����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDamage = Animator.StringToHash("isDamage");// �_���[�W���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private int key_isDeath = Animator.StringToHash("isDeath");  // ���S���[�V����(�S�L�����Ŗ��O�𑵂���K�v������)

    private CharacterSetting set_;                  // �L�������̏��   
    private int barrierNum_ = 0;                    // �h�䎞�ɒl������
    private bool deathFlg_ = false;                 // ���S��Ԃ��m�F����ϐ�

    private int[] statusUp = new int[5];            // �ꎞ�A�b�v�̐��l��ۑ�����p

    // name,num,animator�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g��Ȃ�������폜����B
    public Chara(string name, int objNum, Animator animator) : base(name,objNum,animator,null)
    {
        set_ = GetSetting();  // CharaBase.cs����Get�֐��ŏ����ݒ肷��

        // �����̏�����
        for(int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = 0;
        }
    }

    public bool Attack()
    {
        Debug.Log(set_.name + "�̍U���I");
        set_.animator.SetBool(key_isAttack, true);
        // isMove��false�̂Ƃ������U���G�t�F�N�g��Instance��isMove��true���������s���悤�ɂ��āA
        // �G�t�F�N�g���{�^���A�łő�ʔ�������̂�h��
        if (!set_.isMove)
        {
            //AttackStart((int)nowTurnChar_); // characterMng.cs���ł��
            set_.isMove = true;
            //selectFlg_ = false; // characterMng.cs���ł��
            return true;
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

    public int MP()
    {
        return set_.MP;
    }

    public int MaxMP()
    {
        return set_.maxMP;
    }

    public (Vector3,bool) RunMove(float time,Vector3 myPos, Vector3 targetPos)
    {
        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 1.5f)   // �ڕW�n�_0.0f�ɂ���ƓG�ƏՓ˂��Ă��݂��������ł�������1.5f�ɂ��Ă���
        {
            // �w��ӏ��܂ł�����true��Ԃ�
            Debug.Log("�W���b�N�ړI�G����");
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
        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 0.15f)  // �ڕW�n�_0.0f�ɂ���ƌ덷�ŒH�蒅���Ȃ��Ȃ邩��0.15f�ɂ��Ă���
        {
            // �w��ӏ��܂ł�����true��Ԃ�
            Debug.Log("�W���b�N�ړI�G����");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, 1.0f),true);
        }
        else
        {
            // �L�����̍��W�ړ�
            return (Vector3.Lerp(myPos, targetPos, time),false);
        }
    }


    public void SetHP(int hp)
    {
        set_.HP = hp;

        // �ŏ��l��0
        if(set_.HP < 0)
        {
            set_.HP = 0;
        }

        // �ő�l��HP��MAX�l
        if(set_.HP > set_.maxHP)
        {
            set_.HP = set_.maxHP;
        }
    }

    public void SetMP(int mp)
    {
        set_.MP = mp;
    }

    // CharaBase�N���X�̒��ۃ��\�b�h����������
    public override void LevelUp()
    {
        set_.Level += 1;
        set_.HP += 10;
        set_.MP += 5;
        set_.Attack += 3;
        set_.MagicAttack += 2;
        set_.Defence += 3;
        set_.Speed += 2;
        set_.Luck += 1;

        Debug.Log("���x���A�b�v�I");
    }

    public override void Weapon()
    {
        Debug.Log("����ؑցI");
    }

    public override int Defence(bool flag)
    {
        // ��_���[�W�A�j���[�V�������J�n
        set_.animator.SetBool(key_isDamage, flag);
        return set_.Defence + barrierNum_;
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
        // �Đ����ԗp�ɊԂ��󂯂�
        // �L�������ɕK�v�ȊԂ��Ⴄ��������Ȃ�����A1.0f�̏��͊O���f�[�^�ǂݍ��݂ŁAcharSetting[(int)nowTurnChar_].maxAnimTime�Ƃ���p�ӂ����ق�������
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
            set_.animator.SetBool(key_isAttack, false);

            return true;

            // CharacterMng.cs���ł��
            // ���̃L�������s���ł���悤�ɂ���
            // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
            //if (++nowTurnChar_ >= CharcterNum.MAX)
            //{
            //    nowTurnChar_ = CharcterNum.UNI;
            //}
        }
    }

    public Vector3 GetButtlePos()
    {
        return set_.buttlePos;
    }

    public void SetButtlePos(Vector3 pos)
    {
        set_.buttlePos = pos;
    }

    // �퓬�̎n�߂ɏ���������
    public void SetTurnInit()
    {
        set_.isMove = false;
        set_.animTime = 0.0f;
    }

    public bool GetIsMove()
    {
        return set_.isMove;
    }

    // SceneMng.cs����Ăяo��
    public CharacterSetting GetCharaSetting()
    {
        return set_;
    }

    // SceneMng.cs����Ăяo��
    public void SetCharaSetting(CharacterSetting set)
    {
        set_.name = set.name;
        set_.Level = set.Level;
        set_.HP = set.HP;
        set_.MP = set.MP;
        set_.Attack = set.Attack;
        set_.MagicAttack = set.MagicAttack;
        set_.Defence = set.Defence;
        set_.Speed = set.Speed;
        set_.Luck = set.Luck;
        set_.AnimMax = set.AnimMax;
        set_.Magic = set.Magic;

        SetTurnInit();
        // �A�j���[�^�[�̓Z�b�g���Ă͂����Ȃ�
        //setting_.animator = animator;
    }

    // RestaurantMng.cs�ŌĂяo��(�����܂ł��ꎞ�I��Up������A���ꂼ��ɉ��Z�����l��ʂ̕ϐ��ɓ���Ă���)
    public void SetStatusUpByCook(int[] num)
    {
        set_.Attack += num[0];
        set_.MagicAttack += num[1];
        set_.Defence += num[2];
        set_.Speed += num[3];
        set_.Luck += num[4];

        // �ꎞ�A�b�v�̐����ۑ�
        for (int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = num[i];
        }
    }

    public void DeleteStatusUpByCook()
    {
        // �ꎞ�A�b�v���A�e�X�e�[�^�X����}�C�i�X����
        set_.Attack -= statusUp[0];
        set_.MagicAttack -= statusUp[1];
        set_.Defence -= statusUp[2];
        set_.Speed -= statusUp[3];
        set_.Luck -= statusUp[4];

        // �ꎞ�A�b�v�̐���������������
        for (int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = 0;
        }
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

    public void SetBarrierNum(int num = 0)
    {
        barrierNum_ = num;
    }

    public void SetMagicNum(int arrayNum,int num = 0)
    {
        set_.Magic[arrayNum] = num;
    }

    public Bag_Magic.MagicData GetMagicNum(int arrayNum)
    {
        return Bag_Magic.data[set_.Magic[arrayNum]];
    }

    public Sprite GetMagicImage(int num)
    {
        // ���@��������
        if(set_.Magic[num] == 0)
        {
            return null;
        }

        return ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][Bag_Magic.data[set_.Magic[num]].element];
    }

    public bool CheckMagicNum(int num)
    {
        if(num < 0 || num >= 4)
        {
            return false;
        }

        return true;
    }

    public bool GetDeathFlg()
    {
        return deathFlg_;
    }

    public void SetDeathFlg(bool flag)
    {
        // ���S�A�j���[�V�����ɐ؂�ւ���
        set_.animator.SetBool(key_isDeath, true);
        deathFlg_ = flag;
    }

    public override void DamageAnim()
    {
        // ���������_���[�W���󂯂ĂȂ��Ƃ��͏����𔲂���
        if (!set_.animator.GetBool(key_isDamage))
        {
            return;
        }

        // ���ԂŃ��[�V�������I�������邩���肷��
        if (set_.animTime < set_.AnimMax)
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

    public override void SetBS((int, int) num,int hitNum)
    {
        // ���炩�̃o�X�e���ʂ����閂�@��������A���A�G�̖��������L�����̍K�^�l+�����_���l��荂���Ƃ�
        if (num.Item1 >= 0 &&  hitNum > set_.Luck + Random.Range(0, 100))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item1 - 1].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�L�����͏�Ԉُ�ɂ�������");
        }

        if (num.Item2 >= 0 && hitNum > set_.Luck + Random.Range(0, 100))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item2 - 1].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�L�����͏�Ԉُ�ɂ�������");
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
        else
        {
            // ����̏�Ԉُ킾���񕜂���
            if (targetReset > -1)
            {
                set_.condition[targetReset].Item2 = false;
            }
        }
    }
}
