using UnityEngine;

// �{�X�����n�N�G�X�g�̃N���A�Ǘ�

public class KnockDownEnemy : MonoBehaviour
{
    // �I�u�W�F�N�g����\���ɂȂ����Ƃ��ɑ���֐�
    void OnDisable()
    {
        // �C���X�^���X�����Ƃ���[1]���Ă����ԍ��ɂȂ邩�疼�O�łƂ�Ȃ��I�I
        // ��Weapon�^�O�������Ă�I�u�W�F�N�g��WaterMonster�Ƃ������O�Ō��悤
        if (gameObject.name == "WaterMonster")  // DesertField�̃{�X�Ȃ�
        {
            // ���C���N�G�X�g�u�I�A�V�X���S�点�āv���N���A�ɂ���B
            QuestClearCheck.QuestClear(4);
        }
    }
}