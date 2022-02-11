
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
using System.Collections.Generic;
using UnityEngine;

public class Guild : HouseBase
{
    private QuerySDEmotionalController npcController_;

    public override bool CheckEvent()
    {
        int num = EventMng.GetChapterNum();

        // �C�x���g����
        if (num == 1)
        {
            SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);   // ���Ԍo��
            EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        else if (num == 7 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            // ���ԑт������ɓ���Ȃ��ƁA���A�N�G�X�g�I������ɔ�������\��������
            //@ �u�����Ƃ������[�h���W���b�N������炤�v
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB1, 0, 13);// ����
            EventMng.SetChapterNum(7, SceneMng.SCENE.CONVERSATION);
        }
        else if (num == 10 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            EventMng.SetChapterNum(10, SceneMng.SCENE.CONVERSATION);
        }
        else
        {
            if (num < 15)
            {
                return false;
            }

            for (int i = 0; i < Bag_Word.data.Length; i++)
            {
                if (Bag_Word.data[i].name != "�h���")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // �h���
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 3, 18);
                    EventMng.SetChapterNum(104, SceneMng.SCENE.CONVERSATION);
                    return true;
                }
            }

        }
        return false;
    }

    // �N�G�X�g���󒍂ŃC�x���g���i�s����Ƃ��Ɏg�p
    // QuestMng�ŃC���X�^���X���ČĂяo��
    public void GuildQuestEvent(int questNum, bool clearFlg, Dictionary<int, int> clearNum)
    {
        if (!clearFlg)   // �N�G�X�g�󒍃^�C�~���O
        {
            switch (EventMng.GetChapterNum())
            {
                case 2: // ���݂̐i�s�x��2�ŁA�N�G�X�g0�Ԃ��󒍂�����
                    if (questNum == 0)
                    {
                        // �V�[���J�ږ����Ői�s�x��3�ɂ���B
                        EventMng.SetChapterNum(3, SceneMng.SCENE.NON,true);
                    }
                    break;
                case 12:// ���݂̐i�s�x��12�ŁA�N�G�X�g4�Ԃ��󒍂�����
                    if (questNum == 4)
                    {
                        EventMng.SetChapterNum(12, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 15:// ���݂̐i�s�x��15�ŁA�N�G�X�g5�Ԃ��󒍂�����
                    if (questNum == 5)
                    {
                        EventMng.SetChapterNum(15, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 18:// ���݂̐i�s�x��18�ŁA�N�G�X�g6�Ԃ��󒍂�����
                    if (questNum == 6)
                    {
                        EventMng.SetChapterNum(18, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 21:// ���݂̐i�s�x��21�ŁA�N�G�X�g7�Ԃ��󒍂�����
                    if (questNum == 7)
                    {
                        EventMng.SetChapterNum(21, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                default:
                    break;
            }
        }
        else�@�@�@�@�@�@// �N�G�X�g�񍐃^�C�~���O
        {
            switch (EventMng.GetChapterNum())
            {
                case 6: // ���݂̐i�s�x��6�ŁA�N�G�X�g0�Ԃ�B��������
                    if (questNum == 0)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // ���Ԍo��
                        EventMng.SetChapterNum(6, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 9: // ���݂̐i�s�x��8�ŁA�N�G�X�g1�Ԃ�B��������
                    if (questNum == 1)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // ���Ԍo��
                        EventMng.SetChapterNum(9, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 11:// ���݂̐i�s�x��11�̂Ƃ�
                    if (clearNum[2] >= 1 && clearNum[3] >= 1)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(11, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 14:// ���݂̐i�s�x��14�ŁA�N�G�X�g4�Ԃ�B��������
                    if (questNum == 4)
                    {
                        // ������
                        GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.HEAD, 1, 1);
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(14, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 17:// ���݂̐i�s�x��17�ŁA�N�G�X�g5�Ԃ�B��������
                    if (questNum == 5)
                    {
                        // �S��
                        GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.HEAD, 2, 2);
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(17, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 20:// ���݂̐i�s�x��20�ŁA�N�G�X�g6�Ԃ�B��������
                    if (questNum == 6)
                    {
                        // ����
                        GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB1_AND_SUB2, 3, 23);
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(20, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                case 23:// ���݂̐i�s�x��23�ŁA�N�G�X�g7�Ԃ�B��������
                    if (questNum == 7)
                    {
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(23, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // NPC�̕\���ω�������
    public void ChangeNPCFace(int faceNum)
    {
        if(npcController_ == null)
        {
            npcController_ = GameObject.Find("HouseInterior/Guild/Query").GetComponent<QuerySDEmotionalController>();
        }

        npcController_.ChangeEmotion((QuerySDEmotionalController.QueryChanSDEmotionalType)faceNum,true);
    }
}
