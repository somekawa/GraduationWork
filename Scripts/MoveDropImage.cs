using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveDropImage : MonoBehaviour
{
    private RectTransform parentCanvas_;    // �A�C�e���֘A��\������L�����o�X
    private DropFieldMateria drop_;

    private Image materiaImage_;        // �擾�����A�C�e����\������摜
    private Vector2 middolePos_;        // ���g�̈ʒu�ƖړI�n�܂ł̒��ԓ_
    private Vector2 destinationPos_;    // �ړI�n
    private float iconSpeed_ = 0.0f;    // �������ړ�����ۂ̃X�s�[�h
    private Vector2 saveTopPos_;        // ���_�ɂ���Ƃ��̍��W��ۑ�

    // �A���t�@�l�̌������J�n������W
    private Vector2 minusAlphaPos_ = new Vector2(-450.0f, -200.0f);
    private float alphaNum = 1.0f;  // �摜�̃A���t�@�l

    void Start()
    {
        materiaImage_ = transform.GetComponent<Image>();
        parentCanvas_ = GameObject.Find("FieldUICanvas").GetComponent<RectTransform>();

        drop_ = GameObject.Find("MateriaPoints").GetComponent<DropFieldMateria>();
        StartCoroutine(UpPosImages());// �擾�����A�C�e�����|�b�v�A�b�v������
    }

    private IEnumerator UpPosImages()
    {
        // �㏸�����ǂ���
        bool upFlag = true;
       // Debug.Log(transform.name+ "�̉摜���ړ������܂��@�X�P�[���ƃA���t�@�l");
        // �x�W�F�Ȑ��p�̕ϐ��̐錾
        float t = 0.0f;
        while (true)
        {
            yield return null;
            if (upFlag == true)
            {
                if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
                {
                    // �摜�ړ����Ƀo�g�����n�܂�����j�󂷂�
                    Destroy(gameObject);
                }

                // ���ݍ��W���o�����WY+40���Ⴂ�ʒu��������
                if (drop_.GetShootArrowFlag() == false)
                {
                    // �f�މ摜�Ɩ��O���㏸������
                    transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
                }
                else
                {
                    // �n�_�A�I�_�A�n�_�ƏI�_�Ԃ̋�����2����1�i0.5�j��
                    middolePos_ = Vector3.Lerp(transform.localPosition, destinationPos_, 0.5f);
                    // ���ԍ��W�����߂�@
                    middolePos_ = new Vector2(middolePos_.x,
                        middolePos_.y * (-1) + transform.localPosition.y);
                    // �I�_
                    destinationPos_ = -parentCanvas_.sizeDelta / 2; 

                    // �ړ��X�s�[�h
                    iconSpeed_ = 10 / Vector3.Distance(transform.localPosition, destinationPos_);
                    saveTopPos_ = transform.localPosition;  // ���_�ɂ���Ƃ��̍��W��ۑ�
                    upFlag = false;                         // �㏸����������ړ��ɕύX
                }
            }
            else
            {
                if (t > 1)
                {
                    // �I���_�ł��̃I�u�W�F�N�g���폜
                    if (transform.name == "0")
                    {
                        // 1�ł���������Ă���Ƃ������O��0�Ԃ̎�����
                        drop_.SetMoveFinish(false);
                    }
                    Debug.Log(transform.name + "���폜");
                    Destroy(gameObject);      // �I�u�W�F�N�g���j�󂳂ꂽ��R���[�`�����~�܂�
                }

                // �x�W�F�Ȑ��̏���
                t += iconSpeed_ * Time.deltaTime * 80.0f;
                Vector3 a = Vector3.Lerp(saveTopPos_, middolePos_, t);
                Vector3 b = Vector3.Lerp(middolePos_, destinationPos_, t);
                transform.localPosition = Vector3.Lerp(a, b, t);                // ���W����

                // ��������`���Ă���ԃe���b�v�͏㏸������
                if (transform.name == "0")
                {
                    // 1�ł���������Ă���Ƃ������O��0�Ԃ̎�����
                    drop_.SetMoveFinish(true);
                }

                if (transform.localPosition.x < minusAlphaPos_.x &&
                transform.localPosition.y < minusAlphaPos_.y)
                {
                    // ��ʍ��[�ɏo��O�ɃA���t�@�l�������ăt�F�[�h�A�E�g������
                    alphaNum -= 0.05f;
                    materiaImage_.color = new Color(1.0f, 1.0f, 1.0f, alphaNum);
                }
            }
        }
    }
}
