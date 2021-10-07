using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    // �󒍒��̃N�G�X�g��ۑ����郊�X�g(�ő�3��)
    private static List<(GameObject,bool)> completeQuestsList_ = new List<(GameObject,bool)>();
    // �`���[�g���A���N�G�X�g�p�̃��X�g(�o���肵���������L�^����)
    private static List<string> buildList_ = new List<string>();

    private void Update()
    {
        // ���X�g���񂵂āA�t���O��true�̂��̂����邩�T��
        foreach (var tmp in completeQuestsList_)
        {
            if (tmp.Item2)
            {
                Debug.Log(tmp.Item1.name + "�̃N�G�X�g�́A�N���A�����𖞂����Ă��܂�");
            }
        }
    }

    public static void SetList(GameObject obj)
    {
        // ���X�g�ɒǉ�����
        completeQuestsList_.Add((obj,false));
    }

    // �܂��V�K�ŃN�G�X�g���󂯂��邩�m�F����
    public static bool CanOrderNewQuest(int num)
    {
        // null���Z�q( if(list != null && list.Count < 3)�Ɠ����Ӗ� )
        if (completeQuestsList_?.Count < 3)
        {
            // �����N�G�X�g���󂯂悤�Ƃ��Ă��邩�m�F����
            foreach (var tmp in completeQuestsList_)
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

    public static void SetBuildName(string name)
    {
        // 0�Ԃ̃N�G�X�g���󂯂Ă��Ȃ����return����
        // �`���[�g���A���N�G�X�g���󒍂������́A�N�G�X�g��1�����M���h�ɕ\������Ă��Ȃ��ݒ�ɂ���
        if (int.Parse(completeQuestsList_[0].Item1.name) != 0)
        {
            return;
        }

        // ���񂾂�if�����ɓ���悤�ɂ���
        if (buildList_.Count <= 0 )
        {
            // ��ɂ��������̊֌W���Ȃ���������o�^���Ă���
            buildList_.Add("MayorHouse");
            buildList_.Add("Guild");
            buildList_.Add("UniHouse");
        }

        bool tmpFlg = false;    // ���łɓo�^�ς݂̌�������̑ޏo�Ȃ�true
        foreach (string list in buildList_)
        {
            if(list == name)
            {
                tmpFlg = true;
            }
        }

        // ���o�^�̌�������������Aadd����
        if(!tmpFlg)
        {
            buildList_.Add(name);
        }

        // �S�Ă̌����ւ�������肪�ł���(��������6�ӏ��������Ă���)
        if(buildList_.Count >= 6)
        {
            for(int i = 0; i < completeQuestsList_.Count; i++)
            {
                if(int.Parse(completeQuestsList_[i].Item1.name) == 0)
                {
                    // �G���[�̂ł鏑����
                    // completeQuestsList[i].Item2 = true;

                    // �ꎞ�ϐ��ɕۑ����Ă�����������@�őΏ�����
                    (GameObject, bool) tmpData = completeQuestsList_[i];
                    tmpData.Item2 = true;
                    completeQuestsList_[i] = tmpData;
                }
            }

            // �N���A���������͏I�������̂ŁA���X�g���폜���Ďg���Ȃ��悤�ɂ��Ă���
            buildList_.Clear();
            buildList_ = null;
        }
    }
}
