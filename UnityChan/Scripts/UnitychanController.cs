using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�����̊�{�ړ��ƕ����]���݂̂�Script
// ���퓬�pScript�͕�

public class UnitychanController : MonoBehaviour
{
    private Rigidbody rigid;      // Rigidbody�R���|�[�l���g
    private Animator animator_;   // Animator �R���|�[�l���g

    private const string key_isRun = "isRun";         // Animator�Ŏ����Őݒ肵���t���O�̖��O

    // ������Ԃ��m�F�������L�[���܂Ƃ߂�����
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator_= GetComponent<Animator>();
    }

    void Update()
    {
        // �T�����[�h�ȊO�Ŏ��R�ɓ����ꂽ�炢���Ȃ��̂ŁAreturn������������B
        if(FieldMng.nowMode != FieldMng.MODE.SEARCH)
        {
            // ������Run�̃A�j���[�V������ύX���Ă����Ȃ��ƁA���[�h���؂�ւ��u�Ԃ܂ő����Ă�����
            // ���胂�[�V�������퓬���Ɍp�����Ă��܂��B
            this.animator_.SetBool(key_isRun, false);
            return;
        }

        bool tmpFlg = false;       // ���W�ړ��̃{�^����������true�ɂȂ�
        foreach (KeyCode i in keyArray_)
        {
            // keyArray_�ɐݒ肵��KeyCode�̒��ŁA��������Ă���{�^�������邩�𒲂ׂ�
            if (Input.GetKey(i))
            {
                // Wait����Run�ɑJ�ڂ���
                this.animator_.SetBool(key_isRun, true);
                tmpFlg = true;
                break;  // ����ȏ�񂷕K�v���Ȃ��̂ŁAbreak�Ŕ�����
            }
        }

        if (!tmpFlg) // ���foreach��ʂ��Ă�false�̂܂܂������ꍇ�́A����A�j���[�V������false�ɂ���(=�ҋ@)
        {
            // Run����Wait�ɑJ�ڂ���
            this.animator_.SetBool(key_isRun, false);
            return; // �ҋ@�A�j���[�V�����Ƃ������Ƃ͉��̍��W�ړ��������s���K�v���Ȃ����߁Areturn����
        }

        Vector3 movedir = Vector3.zero;

        // ��󉺃{�^�����������Ă���
        if (Input.GetKey(keyArray_[0]) || Input.GetKey(keyArray_[1]))
        {
            // ��L�[ or ���L�[
            movedir.z = Input.GetAxis("Vertical");
        }

        if (Input.GetKey(keyArray_[2]) || Input.GetKey(keyArray_[3]))
        {
            // ���L�[ or �E�L�[
            movedir.x = Input.GetAxis("Horizontal");
        }

        // �O���[�o�����W�ɕϊ�����ƁA�L�����̕����]�����+-���o�O���N����
        //Vector3 globaldir = transform.TransformDirection(movedir);
        //controller_.Move(globaldir * Time.deltaTime);

        if (movedir != Vector3.zero)
        {
            // ���x�x�N�g�����쐬�i3�����p�jY���W��0.0f�ŕK���Œ肷��
            var speed = new Vector3(movedir.x, 0.0f, movedir.z);
            // ���x�ɐ��K�������x�N�g���ɁA�ړ����x�������đ������
            rigid.velocity = speed.normalized * SceneMng.charaRunSpeed;

            // ���W�X�V
            // �L�����N�^�[���ړ������鏈��
            rigid.MovePosition(rigid.position + rigid.velocity * Time.deltaTime);
            // �L���������]��
            transform.rotation = Quaternion.LookRotation(movedir);
        }
    }

    // �L�������ړ�����
    public bool GetMoveFlag()
    {
        return this.animator_.GetBool(key_isRun);
    }
}