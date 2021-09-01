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
        NON,
        LEFT,
        RIGHT
    }

    private DIR rotateDir_ = DIR.NON;   // ��]�����̎w��
    private float targetRotate_ = 0.0f; // ��]�p�x
    
    void Start()
    {
    }

    void Update()
    {
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

        // �ڕW�p�x���I�C���[�p����N�H�[�^�j�I���ɂ���
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, targetRotate_));

        // ���݂̃N�H�[�^�j�I���̎擾
        var now_rot = transform.rotation;

        // Quaternion.Angle��2�̃N�H�[�^�j�I���̊Ԃ̊p�x�����߂�
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            // Angle�̒l���w��̕��ȉ��ɂȂ�����ڕW�n�_�ɗ��������ɂ��āA�������~�߂�
            transform.rotation = target;
            rotateDir_ = DIR.NON;
        }
        else
        {
            // �w��̕��ɓ͂��Ă��Ȃ��ꍇ�́A��]�������m�F���āA��]�𑱂���
            if(rotateDir_ == DIR.RIGHT)
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
}
