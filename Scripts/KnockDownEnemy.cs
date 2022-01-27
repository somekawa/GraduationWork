using UnityEngine;

// �{�X�����n�N�G�X�g�̃N���A�Ǘ�

public class KnockDownEnemy : MonoBehaviour
{
    private Bag_Word bagWord_;

    void Start()
    {
        // OnDisable��GameObject.Find�i�j���g�p����ƃG���[���ł邩��AStart�֐���Find���Ă���
        bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();
    }

    // �I�u�W�F�N�g����\���ɂȂ����Ƃ��ɑ���֐�
    void OnDisable()
    {
        // �C���X�^���X�����Ƃ���[1]���Ă����ԍ��ɂȂ邩�疼�O�łƂ�Ȃ��I�I
        // ��Weapon�^�O�������Ă�I�u�W�F�N�g��WaterMonster�Ƃ������O�Ō��悤
        if (gameObject.name == "WaterMonster")  // DesertField�̃{�X�Ȃ�
        {
            // ���C���N�G�X�g�u�I�A�V�X���S�点�āv���N���A�ɂ���B
            QuestClearCheck.QuestClear(4);
            // ��
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 1, 6);// ��
        }
        else if(gameObject.name == "BossGolem")
        {
            // ���C���N�G�X�g�u�S�[������ʔ����v���N���A�ɂ���B
            QuestClearCheck.QuestClear(5);
            // �y
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 2, 7);// �y
        }
        else if(gameObject.name == "PoisonSlime")
        {
            // ���C���N�G�X�g�u�ł̖��𐰂炵�āv���N���A�ɂ���B
            QuestClearCheck.QuestClear(6);
            // ��
            bagWord_.WordGetCheck(InitPopList.WORD.ELEMENT_ATTACK, 3, 8);// ��
        }
        else if (gameObject.name == "Dragon")
        {
            // ���C���N�G�X�g�u�q�[�A���j�����v���N���A�ɂ���B
            QuestClearCheck.QuestClear(7);
        }
        else
        {
            // �����������s��Ȃ�
        }
    }
}
