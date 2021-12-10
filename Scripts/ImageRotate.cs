using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�[����ɂ���āA�o�g���I���R�}���h����]����

// Unity�̏ꍇ�A�p�x�̓I�C���[�p�ł͂Ȃ��N�H�[�^�j�I���Ŏ����Ă���̂ŁA�����Angle���r���Ďw��̊p�x�Ŏ~�܂�悤�ɂ��܂��B

public class ImageRotate : MonoBehaviour
{
    // ��]����
    enum DIR
    {
        NON,    // ��]�Ȃ�
        LEFT,   // ����]
        RIGHT   // �E��]
    }

    // �I���R�}���h
    public enum COMMAND
    {
        NON,    // ��]���őI�𒆃R�}���h���Ȃ����
        ATTACK, // �U���R�}���h(�����R�}���h)
        MAGIC,  // ���@�R�}���h
        ITEM,   // �A�C�e���R�}���h
        BARRIER,// �h��R�}���h
        MAX     
    }

    private DIR rotateDir_ = DIR.NON;   // ��]�����̎w��
    private float targetRotate_ = 0.0f; // ��]�p�x

    // �L�[���p�x,�l��COMMAND��enum�ō����map
    private Dictionary<int, COMMAND> commandMap_;
    public COMMAND nowCommand_ = COMMAND.ATTACK;   // ���݂̑I�𒆃R�}���h

    private bool rotaFlg_ = true;       // ��]�����Ă��������𔻒肷��(true:��]���Ă悢)

    private bool flag = true;

    void Start()
    {
        // �R�}���h��Ԃ̒ǉ�
        commandMap_ = new Dictionary<int, COMMAND>(){
            {0,COMMAND.ATTACK},
            {90,COMMAND.MAGIC},
            {180,COMMAND.BARRIER},
            {270,COMMAND.ITEM},
        };
    }

    void Update()
    {
        //Debug.Log(targetRotate_);
        //Debug.Log(nowCommand_);

        if(!rotaFlg_ || !flag)
        {
            return;
        }

        // �L�[�ɂ���āA��]���������肷��
        if (Input.GetKeyDown(KeyCode.J))
        {
            // �E��]
            targetRotate_ += 90.0f;
            rotateDir_ = DIR.RIGHT;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            // ����]
            targetRotate_ -= 90.0f;
            rotateDir_ = DIR.LEFT;
        }
        else
        {
            // �����������s��Ȃ�
        }

        // 360�ȏォ�ȉ��Ȃ�AtargetRotate_��0�ɖ߂�
        if (targetRotate_ >= 360 || targetRotate_ <= -360)
        {
            targetRotate_ = 0;
        }

        // �ڕW�p�x���I�C���[�p����N�H�[�^�j�I���ɂ���
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, targetRotate_));

        // ���݂̃N�H�[�^�j�I���̎擾
        var now_rot = transform.rotation;

        // Quaternion.Angle��2�̃N�H�[�^�j�I���̊Ԃ̊p�x�����߂�
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            // ��]�I��
            int rota = (int)targetRotate_;
            if (rota < 0)   // ����]�̏ꍇ�́Arota���}�C�i�X�l�ɂȂ邽��+360���ĉE��]�Ɠ����l�ɕύX����
            {
                rota += 360;
            }
            // ���݂̉�]�p�x�����āACOMMAND�����肷��
            nowCommand_ = commandMap_[rota];

            // Angle�̒l���w��̕��ȉ��ɂȂ�����ڕW�n�_�ɗ��������ɂ��āA�������~�߂�
            transform.rotation = target;
            rotateDir_ = DIR.NON;
        }
        else
        {
            // ��]��
            nowCommand_ = COMMAND.NON;

            // �w��̕��ɓ͂��Ă��Ȃ��ꍇ�́A��]�������m�F���āA��]�𑱂���
            if (rotateDir_ == DIR.RIGHT)
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, 0.5f));
            }
            else if(rotateDir_ == DIR.LEFT)
            {
                transform.Rotate(new Vector3(0.0f, 0.0f, -0.5f));
            }
            else
            {
                // �����������s��Ȃ�
            }
        }
    }

    // �퓬���[�h���I����������ɁA��]�������l�ɖ߂�
    public void ResetRotate()
    {
        var resetRotate = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        transform.rotation = resetRotate;
        targetRotate_ = 0.0f;
    }

    // ���݂̃R�}���h��Ԃ�return����
    public COMMAND GetNowCommand()
    {
        return nowCommand_;
    }

    // �\����ԂƃX�N���v�g�̎��s��Ԃ�؂�ւ���
    public void SetEnableAndActive(bool flag)
    {
        gameObject.SetActive(flag);
        enabled = flag;
    }
}
