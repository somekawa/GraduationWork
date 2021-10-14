using System.Collections.Generic;
using UnityEngine;

public class TimeGear : MonoBehaviour
{
    private static TimeGear timgeGrarSingleton_;    // �V���O���g���p�ϐ�

    // �L�[��enum��TIMEGEAR�ŁA�l��Z���̖ڕW��]�l
    private Dictionary<SceneMng.TIMEGEAR, float> rotateTimeGearMap_;
    private GameObject panel_;  // ���w�i�p�̃p�l��

    void Awake()
    {
        // �Q�[���I�u�W�F�N�g���܂�����Ă��Ȃ��Ƃ�
        if (timgeGrarSingleton_ == null)
        {
            panel_ = GameObject.Find("DontDestroyCanvas/TimeGear/Panel");
            panel_.SetActive(false);

            timgeGrarSingleton_ = this;

            // �����̐e(Canvas)�������Ȃ��I�u�W�F�N�g�Ƃ��ēo�^
            // �e��o�^����΁A�q��Image�������Ȃ��I�u�W�F�N�g�Ɣ��肳���݂���
            DontDestroyOnLoad(transform.root.gameObject);

            // �L�[�ƒl��o�^����
            rotateTimeGearMap_ = new Dictionary<SceneMng.TIMEGEAR, float>{
            {SceneMng.TIMEGEAR.MORNING, 0.0f},
            {SceneMng.TIMEGEAR.NOON   , 90.0f},
            {SceneMng.TIMEGEAR.EVENING, 180.0f},
            {SceneMng.TIMEGEAR.NIGHT  , 270.0f},
            };
        }
        else
        {
            //�@���ɓ����X�N���v�g������΂��̃V�[���̓����Q�[���I�u�W�F�N�g���폜
            Destroy(transform.root.gameObject);
        }

    }

    void Update()
    {
        // ���Ԍo�߃e�X�g�p
        if (Input.GetKeyDown(KeyCode.O))
        {
            var aa = SceneMng.GetTimeGear();
            if (aa >= SceneMng.TIMEGEAR.NIGHT) // ������Ȃ�A��������悤�ɂ���
            {
                SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING);
            }
            else
            {
                SceneMng.SetTimeGear((SceneMng.TIMEGEAR)((int)aa + 1));
            }
        }

        // �ڕW�p�x���I�C���[�p����N�H�[�^�j�I���ɂ���
        var target = Quaternion.Euler(new Vector3(0.0f, 0.0f, rotateTimeGearMap_[SceneMng.GetTimeGear()]));

        // ���݂̃N�H�[�^�j�I���̎擾
        var now_rot = transform.rotation;

        // Quaternion.Angle��2�̃N�H�[�^�j�I���̊Ԃ̊p�x�����߂�
        if (Quaternion.Angle(now_rot, target) <= 1.0f)
        {
            panel_.SetActive(false);

            // Angle�̒l���w��̕��ȉ��ɂȂ�����ڕW�n�_�ɗ��������ɂ��āA�������~�߂�
            transform.rotation = target;
        }
        else
        {
            panel_.SetActive(true);

            // ��]������
            transform.Rotate(new Vector3(0.0f, 0.0f, 2.0f));
        }
    }
}
