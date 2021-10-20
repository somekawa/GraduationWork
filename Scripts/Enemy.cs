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
        Debug.Log(set_.name + "�̍U���I");
        return true;
    }

    public void Damage()
    {
        Debug.Log(set_.name + "�̓_���[�W�󂯂��I");
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
    public override void Defence()
    {
        Debug.Log("�h��I");
    }
    public override void Magic()
    {
        Debug.Log("���@�I");
    }
    public override void Item()
    {
        Debug.Log("�A�C�e���I");
    }

    public override bool ChangeNextChara()
    {
        return true;
    }
}
