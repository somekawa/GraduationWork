using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�@���ۃN���X(CharaBase)�ƃC���^�t�F�[�X(InterfaceButtle)���p������Chara�N���X���쐬
public class Chara : CharaBase,InterfaceButtle
{
    // name�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g���Ȃ�������폜����B
    public Chara(string name, CharacterMng.CharcterNum num) : base(name,num)
    {
    }

    public void Attack()
    {
        Debug.Log("�U���I");
    }

    public void Damage()
    {
        Debug.Log("�_���[�W�󂯂��I");
    }

    public void HP()
    {
        Debug.Log("HP�I");
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
}
