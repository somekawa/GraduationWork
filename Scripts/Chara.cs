using System.Collections.Generic;
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

    //private int[] statusUp = new int[8];            // �ꎞ�A�b�v�̐��l��ۑ�����p
    private int[] saveKeep_ = new int[8];           // �Z�[�u���ɁA�ꎞ�A�b�v�̐����������p

    private readonly int[] statusMap_ = new int[4];
    private Dictionary<int, (int, int)> buffMap_
        = new Dictionary<int, (int, int)>();        // �o�t��̒l�ƃ^�[�������Ǘ�����<���[�h��,(���ʒl,�o�t�^�[����)>

    // ����o�t(��{�㏑��,�������NON�ɖ߂�)
    public enum SPECIALBUFF
    {
        NON = 1,    // ����
        REF,        // ��������
        REF_M,      // ���@����
        ABS,        // �����z��
        ABS_M       // ���@����
    }

    private SPECIALBUFF spBuff_ = SPECIALBUFF.NON;

    // name,num,animator�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g��Ȃ�������폜����B
    public Chara(string name, int objNum, Animator animator) : base(name,objNum,animator,null)
    {
        set_ = GetSetting();  // CharaBase.cs����Get�֐��ŏ����ݒ肷��

        // �����̏�����
        //for (int i = 0; i < statusUp.Length; i++)
        //{
        //    saveKeep_[i] = 0;
        //    statusUp[i] = 0;
        //}

        //SetStatusUpByCook(GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>().StatusNum());
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
        return buffMap_[1].Item1;
        //return set_.Attack;
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

        if (distance <= 0.5f)  // �ڕW�n�_0.0f�ɂ���ƌ덷�ŒH�蒅���Ȃ��Ȃ邩��0.5f�ɂ��Ă���
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

    public int CharacterMaxExp()
    {
        return set_.CharacterMaxExp;
    }
    public int CharacterExp()
    {
        return set_.CharacterExp;
    }

    // CharaBase�N���X�̒��ۃ��\�b�h����������
    public override void LevelUp(int[] num)
    {
        set_.Attack += num[0];
        set_.MagicAttack += num[1];
        set_.Defence += num[2];
        set_.Speed += num[3];
        set_.Luck += num[4];
        set_.maxHP += num[5];
        set_.HP += num[5];
        set_.maxMP += num[6];
        set_.MP += num[6];
        set_.Level += num[7];
        if (num[7] != 0)
        {
            // ���x�����オ����������Max�l�������Ȃ�
            set_.CharacterMaxExp = num[9];
        }
        // exp�{�w���͊�{�I�Ƀ��x�����オ��1�O�܂ŏグ��
        set_.CharacterExp = num[7] == 0 ? set_.CharacterMaxExp - 1 : num[8];
        Debug.Log(set_.CharacterExp + "           " + set_.CharacterMaxExp);
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
        return buffMap_[3].Item1 + barrierNum_;
        //return set_.Defence + barrierNum_;
    }

    public override int MagicPower()
    {
        return buffMap_[2].Item1;
        //return set_.MagicAttack;
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

        statusMap_[0] = set_.Attack;
        statusMap_[1] = set_.MagicAttack;
        statusMap_[2] = set_.Defence;
        statusMap_[3] = set_.Speed;

        buffMap_.Clear();
        for (int i = 0; i < statusMap_.Length; i++)
        {
            buffMap_.Add(i + 1, (statusMap_[i], -1));  // ���[�h�̏��ԂƑ�����
        }
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
        set_.maxHP = set.maxHP;
        set_.maxMP = set.maxMP;
        set_.Attack = set.Attack;
        set_.MagicAttack = set.MagicAttack;
        set_.Defence = set.Defence;
        set_.Speed = set.Speed;
        set_.Luck = set.Luck;
        set_.AnimMax = set.AnimMax;
        set_.Magic = set.Magic;
        set_.CharacterExp = set.CharacterExp;
        set_.CharacterMaxExp = set.CharacterMaxExp;
        set_.statusUp = set.statusUp;

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
        set_.maxHP += num[5];
        set_.maxMP += num[6];
        set_.Exp += num[7];
        set_.HP += num[5];
        set_.MP += num[6];


        // �ꎞ�A�b�v�̐����ۑ�
        for (int i = 0; i < set_.statusUp.Length; i++)
        {
            set_.statusUp[i] = num[i];
        }
    }

    public int[] GetStatusUpByCook(bool flag)
    {
        if(flag)
        {
            return set_.statusUp;
        }
        else
        {
            return saveKeep_;
        }
    }

    public void DeleteStatusUpByCook(bool loadFlag = false)
    {
        // �ꎞ�A�b�v���A�e�X�e�[�^�X����}�C�i�X����
        set_.Attack -= set_.statusUp[0];
        set_.MagicAttack -= set_.statusUp[1];
        set_.Defence -= set_.statusUp[2];
        set_.Speed -= set_.statusUp[3];
        set_.Luck -= set_.statusUp[4];
        set_.maxHP -= set_.statusUp[5];
        set_.maxMP -= set_.statusUp[6];
        set_.Exp -= set_.statusUp[7];
        set_.HP -= set_.statusUp[5];
        set_.MP -= set_.statusUp[6];

        if (loadFlag)
        {
            // �ۑ���ʂɂ����āA�ꎞ�A�b�v�̐���������������
            for (int i = 0; i < set_.statusUp.Length; i++)
            {
                saveKeep_[i] = set_.statusUp[i];

                if(set_.name == "Jack")
                {
                    set_.statusUp[i] = 0;
                }
            }
        }
        else
        {
            // �ꎞ�A�b�v�̐���������������
            for (int i = 0; i < set_.statusUp.Length; i++)
            {
                if (set_.name == "Jack")
                {
                    set_.statusUp[i] = 0;
                }
            }
        }
    }

    public int Speed()
    {
        return buffMap_[4].Item1;
        //return set_.Speed;
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
        // �A�j���[�V�����؂�ւ�
        set_.animator.SetBool(key_isDeath, flag);
        deathFlg_ = flag;
    }

    public void ButtleInit(bool flag = true)
    {
        if(flag)
        {
            // �ŏ��̃��[�V�����ݒ�
            set_.animator.Play("Standing@loop");
        }

        // �o�X�e��S�ď���
        ConditionReset(true);

        // �o�t��S�ď���
        for (int i = 1; i <= buffMap_.Count; i++)
        {
            // �����o�t���p�����Ă�����
            if (buffMap_[i].Item2 > 0)
            {
                buffMap_[i] = (statusMap_[i - 1], -1);
            }
        }
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
        if (num.Item1 > 0 &&  hitNum > set_.Luck + Random.Range(0, 100))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item1 - 1].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�L�����͏�Ԉُ�ɂ�������");
        }

        if (num.Item2 > 0 && hitNum > set_.Luck + Random.Range(0, 100))
        {
            // �Y������o�b�h�X�e�[�^�X��true�ɂ���
            set_.condition[num.Item2 - 4].Item2 = true;
            // NON��false�ɂ���
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("�L�����͏�Ԉُ�ɂ�������");
        }

        Debug.Log("�L�����͏�Ԉُ�ɂ�����Ȃ�����");
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

    public override (int, bool) SetBuff(int tail, int buff)
    {
        bool flag = false;
        // �o�t/�f�o�t���e�𔽉f������
        // �܂��͌��ʗʂ̐ݒ�Ƃ��āA�З͐���������
        // ��(0)->2��,��(1)->4��,��(2)->6���Ƃ���ɂ́Atail+1*2�ɂ���΂���
        float alphaNum = (((float)buffMap_[buff].Item1 - (float)statusMap_[buff - 1]) / (float)statusMap_[buff - 1]) * 10.0f;   // �o�t�̏d�ˊ|���p
        float buffNum = ((tail + 1) * 2 + alphaNum) * 0.1f;
        float num = statusMap_[buff - 1] * buffNum;
        if (num <= 1.0f)
        {
            num = 1.0f;
        }

        // �o�t�̏d�ˊ|�������c��̌��ʃ^�[�������݂Ĕ��f����
        if(buffMap_[buff].Item2 > 0)
        {
            // �e���ڂɉ������㏸����(�Œ��+1�^�[���̉����Ƃ��Ă���)
            buffMap_[buff] = (statusMap_[buff - 1] + (int)num, buffMap_[buff].Item2 + 1);
            flag = true;
        }
        else
        {
            // �e���ڂɉ������㏸����(�Œ��4�^�[����񕜂Ƃ��Ă���)
            buffMap_[buff] = (statusMap_[buff - 1] + (int)num, 4);
        }

        // (���ݒl - ���l) / ���l = �{�� * 100%
        float tmp = (((float)buffMap_[buff].Item1 - (float)statusMap_[buff - 1]) / (float)statusMap_[buff - 1]) * 100.0f;

        // �ǂ́��̈�ɂ��邩�ŕԂ�������ύX����
        if(tmp > 0 && tmp <= 30)
        {
            tmp = 0;
        }
        else if(tmp > 30 && tmp <= 70)
        {
            tmp = 1;
        }
        else if(tmp > 70 && tmp <= 100)
        {
            tmp = 2;
        }
        else
        {
            tmp = -1;
        }

        return ((int)tmp,flag);
    }

    public override bool CheckBuffTurn()
    {
        bool flag = true;
        for (int i = 1; i <= buffMap_.Count; i++)
        {
            // �����o�t���p�����Ă�����
            if (buffMap_[i].Item2 > 0)
            {
                // -1�^�[������
                buffMap_[i] = (buffMap_[i].Item1, buffMap_[i].Item2 - 1);

                if (buffMap_[i].Item2 <= 0)    // ��قǂ�-1��0�ȉ��ɂȂ����Ȃ��
                {
                    Debug.Log("�����̃o�t����������܂���");
                    buffMap_[i] = (statusMap_[i - 1], -1);
                    flag = false;
                }
            }
        }
        return flag;
    }

    // ���˂��z���̃o�t����(��{�͌��ʏ㏑��)
    public void SetReflectionOrAbsorption(int sub2,int num)
    {
        spBuff_ = (SPECIALBUFF)(sub2 + num);
    }

    // �_���[�W�������ɂ�т������
    public SPECIALBUFF GetReflectionOrAbsorption()
    {
        return spBuff_;
    }

    public override Dictionary<int, (int, int)> GetBuff()
    {
        return buffMap_;
    }

    public override int Level()
    {
        return set_.Level;
    }
}
