using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnedMateria : MonoBehaviour
{
    void Awake()
    {
        // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɂ���
        // ���̃I�u�W�F�N�g�������Ă��܂��ƁAQuestClearCheck.cs�̎󒍒��̃N�G�X�g��ۑ����Ă��郊�X�g��null�ɂȂ��Ă��܂�����
        DontDestroyOnLoad(this);
    }

    public void SetMyName(string num)
    {
        // �����̃I�u�W�F�N�g�����A�N�G�X�g�ԍ��ɕϊ�
        // ���X�N���v�g���q�G�����L�[���炱�̃I�u�W�F�N�g����������Ƃ��ɔԍ�+�^�O(Quest)�Ŕ��ʂł���悤�ɂ���
        this.gameObject.name = num.ToString();
    }
}
