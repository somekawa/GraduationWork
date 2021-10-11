using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    private int deleteNum_ = -1;

    // �󒍒��̃N�G�X�g��ۑ����郊�X�g(�ő�3��)
    private static List<(GameObject,bool)> orderQuestsList_ = new List<(GameObject,bool)>();
    // �N���A�ς݂̃N�G�X�g��ۑ����郊�X�g
    private static List<GameObject> clearedQuestsList_ = new List<GameObject>();
    // �`���[�g���A���N�G�X�g�p�̃��X�g(�o���肵���������L�^����)
    public static List<string> buildList = new List<string>();

    void Update()
    {
        // �e�X�g�p(�N�G�X�g��S�ĒB����Ԃɂł���悤�ɂ���)
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    for (int i = 0; i < orderQuestsList_.Count; i++)
        //    {
        //        // �ꎞ�ϐ��ɕۑ����Ă�����������@�őΏ�����
        //        (GameObject, bool) tmpData = orderQuestsList_[i];
        //        tmpData.Item2 = true;
        //        orderQuestsList_[i] = tmpData;
        //    }
        //}

        // ���X�g���񂵂āA�t���O��true�̂��̂����邩�T��
        foreach (var tmp in orderQuestsList_)
        {
            if (tmp.Item2)
            {
                // �폜�\��ԍ��Ƃ��đ������
                deleteNum_ = int.Parse(tmp.Item1.name);
                Debug.Log(tmp.Item1.name + "�̃N�G�X�g�́A�N���A�����𖞂����Ă��܂�");
                break;
            }
        }

        // �폜�ԍ��������l��-1�ł͂Ȃ��ꍇ�͏������s��
        if(deleteNum_ >= 0)
        {
            for(int i = 0; i < orderQuestsList_.Count; i++)
            {
                // �폜�ԍ��ƈ�v���Ă�����
                if(orderQuestsList_[i].Item1.name == deleteNum_.ToString())
                {
                    // �N���A�ς݃��X�g�փR�s�[����
                    clearedQuestsList_.Add(orderQuestsList_[i].Item1);
                    // ���������N�G�X�g�Ɉ�ԍŌ�̗v�f���ڂ�
                    //(�폜�ԍ���0�Ń��X�g�̍Ōオ2�Ȃ�A2��0�̂������Ƃ���ɃR�s�[���āA������2���폜�������)
                    orderQuestsList_[i] = orderQuestsList_[orderQuestsList_.Count - 1];
                    // ��ԍŌ�̗v�f���폜
                    orderQuestsList_.RemoveAt(orderQuestsList_.Count - 1);
                    // �폜�ԍ��̏�����
                    deleteNum_ = -1;
                    break;
                }
            }
        }
    }

    public static void SetOrderQuestsList(GameObject obj)
    {
        // ���X�g�ɒǉ�����
        orderQuestsList_.Add((obj,false));
    }

    public static List<(GameObject, bool)> GetOrderQuestsList()
    {
        return orderQuestsList_;
    }

    public static void SetClearedQuestsList(int num)
    {
        for (int i = 0; i < clearedQuestsList_.Count; i++)
        {
            // �폜�ԍ��ƈ�v���Ă�����
            if (clearedQuestsList_[i].name == num.ToString())
            {
                // ���������N�G�X�g�Ɉ�ԍŌ�̗v�f���ڂ�
                clearedQuestsList_[i] = clearedQuestsList_[clearedQuestsList_.Count - 1];
                // ��ԍŌ�̗v�f���폜
                clearedQuestsList_.RemoveAt(clearedQuestsList_.Count - 1);
                break;
            }
        }
    }

    public static List<GameObject> GetClearedQuestsList()
    {
        return clearedQuestsList_;
    }

    // �܂��V�K�ŃN�G�X�g���󂯂��邩�m�F����
    public static bool CanOrderNewQuest(int num)
    {
        // null���Z�q( if(list != null && list.Count < 3)�Ɠ����Ӗ� )
        if (orderQuestsList_?.Count < 3)
        {
            // �����N�G�X�g���󂯂悤�Ƃ��Ă��邩�m�F����
            foreach (var tmp in orderQuestsList_)
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
        if (orderQuestsList_.Count <= 0 || int.Parse(orderQuestsList_[0].Item1.name) != 0)
        {
            return;
        }

        // �N���A�ς݂Ȃ�return����
        if (orderQuestsList_[0].Item2)
        {
            return;
        }

        // ���񂾂�if�����ɓ���悤�ɂ���
        if (buildList.Count <= 0 )
        {
            // ��ɂ��������̊֌W���Ȃ���������o�^���Ă���
            buildList.Add("MayorHouse");
            buildList.Add("Guild");
            buildList.Add("UniHouse");
        }

        bool tmpFlg = false;    // ���łɓo�^�ς݂̌�������̑ޏo�Ȃ�true
        foreach (string list in buildList)
        {
            if(list == name)
            {
                tmpFlg = true;
            }
        }

        // ���o�^�̌�������������Aadd����
        if(!tmpFlg)
        {
            buildList.Add(name);
        }

        // �S�Ă̌����ւ�������肪�ł���(��������6�ӏ��������Ă���)
        if(buildList.Count >= 6)
        {
            for(int i = 0; i < orderQuestsList_.Count; i++)
            {
                if(int.Parse(orderQuestsList_[i].Item1.name) == 0)
                {
                    // �G���[�̂ł鏑����
                    // completeQuestsList[i].Item2 = true;

                    // �ꎞ�ϐ��ɕۑ����Ă�����������@�őΏ�����
                    (GameObject, bool) tmpData = orderQuestsList_[i];
                    tmpData.Item2 = true;
                    orderQuestsList_[i] = tmpData;

                    // �C�x���g�i�s�x��6�ɂ���
                    EventMng.SetChapterNum(6, SceneMng.SCENE.NON);
                }
            }

            // �N���A���������͏I�������̂ŁA���X�g���폜���Ďg���Ȃ��悤�ɂ��Ă���
            buildList.Clear();
            buildList = null;
        }
    }
}
