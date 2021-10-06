using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    private static List<(GameObject,bool)> completeQuestsList = new List<(GameObject,bool)>();

    private void Update()
    {
        // ���X�g���񂵂āA�t���O��true�̂��̂����邩�T��
        foreach (var tmp in completeQuestsList)
        {
            if(tmp.Item2)
            {
                Debug.Log(tmp.Item1.name + "�̃N�G�X�g�́A�N���A�����𖞂����Ă��܂�");
            }
        }
    }

    public static void SetList(GameObject obj)
    {
        // ���X�g�ɒǉ�����
        completeQuestsList.Add((obj,false));
    }

    // �܂��V�K�ŃN�G�X�g���󂯂��邩�m�F����
    public static bool CanOrderNewQuest(int num)
    {
        // null���Z�q( if(list != null && list.Count < 3)�Ɠ����Ӗ� )
        if (completeQuestsList?.Count < 3)
        {
            // �����N�G�X�g���󂯂悤�Ƃ��Ă��邩�m�F����
            foreach (var tmp in completeQuestsList)
            {
                if (tmp.Item1.name == num.ToString())
                {
                    Debug.Log("���ݎ󂯂Ă���N�G�X�g�Ɠ������͎̂󂯂��܂���");
                    return false;
                }
            }

            // list����2�܂łȂ�A�܂��N�G�X�g���󂯂���
            return true;
        }

        Debug.Log("����3�N�G�X�g���󂯂Ă��邽�߁A����ȏ�V�K�Ŏ󂯂��܂���");
        return false;   // �V�K�N�G�X�g���󂯂��Ȃ�
    }
}
