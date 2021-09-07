using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�@���ۃN���X(CharaBase)�ƃC���^�t�F�[�X(InterfaceButtle)���p������Chara�N���X���쐬
public class Chara : CharaBase,InterfaceButtle
{
    private const string key_isAttack = "isAttack"; // �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)
    private CharacterSetting tmpset;                // �L�������̏��            

    // name,num,animator�͐e�N���X�̃R���X�g���N�^���Ăяo���Đݒ�
    // num�́ACharacterNum��enum�̎擾�Ŏg����������������p�ӂ��Ă݂��B�g��Ȃ�������폜����B
    public Chara(string name, CharacterMng.CharcterNum num, Animator animator) : base(name,num,animator)
    {
        tmpset = GetSetting();  // CharaBase.cs����Get�֐��ŏ����ݒ肷��


    }

    public bool Attack()
    {
        Debug.Log(GetName() + "�̍U���I");
        tmpset.animator.SetBool(key_isAttack, true);
        // isMove��false�̂Ƃ������U���G�t�F�N�g��Instance��isMove��true���������s���悤�ɂ��āA
        // �G�t�F�N�g���{�^���A�łő�ʔ�������̂�h��
        if (!tmpset.isMove)
        {
            //AttackStart((int)nowTurnChar_); // characterMng.cs���ł��
            tmpset.isMove = true;
            //selectFlg_ = false; // characterMng.cs���ł��
            return true;
        }
        return false;
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

    public override bool ChangeNextChara()
    {
        tmpset.animator.SetBool(key_isAttack, false);

        if (!tmpset.isMove)
        {
            return false;
        }

        // �������牺�́AisMove��true�̏��
        // isMove��true�Ȃ�A�������܂ōU�����[�V�������������Ƃ��킩��
        // �Đ����ԗp�ɊԂ��󂯂�
        // �L�������ɕK�v�ȊԂ��Ⴄ��������Ȃ�����A1.0f�̏��͊O���f�[�^�ǂݍ��݂ŁAcharSetting[(int)nowTurnChar_].maxAnimTime�Ƃ���p�ӂ����ق�������
        if (tmpset.animTime < 1.0f)
        {
            tmpset.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            // animTime�l��1.0���������Ƃ�
            // animTime�l�̏������ƁA���[�V������Wait�ɂȂ�����isMove��false�֖߂�
            tmpset.animTime = 0.0f;
            tmpset.isMove = false;

            return true;

            // CharacterMng.cs���ł��
            // ���̃L�������s���ł���悤�ɂ���
            // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
            //if (++nowTurnChar_ >= CharcterNum.MAX)
            //{
            //    nowTurnChar_ = CharcterNum.UNI;
            //}
        }

        return false;
    }

    public Vector3 GetButtlePos()
    {
        return tmpset.buttlePos;
    }

    public void SetButtlePos(Vector3 pos)
    {
        tmpset.buttlePos = pos;
    }

}
