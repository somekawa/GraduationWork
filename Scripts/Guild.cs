
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
using System.Collections.Generic;

public class Guild : HouseBase
{
    public override bool CheckEvent()
    {
        // �C�x���g����
        if (EventMng.GetChapterNum() == 1)
        {
            SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);   // ���Ԍo��
            EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        else if(EventMng.GetChapterNum() == 7 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            // ���ԑт������ɓ���Ȃ��ƁA���A�N�G�X�g�I������ɔ�������\��������
            EventMng.SetChapterNum(7, SceneMng.SCENE.CONVERSATION); 
        }
        else if (EventMng.GetChapterNum() == 10 && SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.MORNING)
        {
            EventMng.SetChapterNum(10, SceneMng.SCENE.CONVERSATION);
        }
        return false;
    }

    // �N�G�X�g���󒍂ŃC�x���g���i�s����Ƃ��Ɏg�p
    // QuestMng�ŃC���X�^���X���ČĂяo��
    public void GuildQuestEvent(int questNum,bool clearFlg, Dictionary<int, int> clearNum)
    {
        if(!clearFlg)   // �N�G�X�g�󒍃^�C�~���O
        {
            switch (EventMng.GetChapterNum())
            {
                case 2: // ���݂̐i�s�x��2�ŁA�N�G�X�g0�Ԃ��󒍂�����
                    if (questNum == 0)
                    {
                        // �V�[���J�ږ����Ői�s�x��3�ɂ���B
                        EventMng.SetChapterNum(3, SceneMng.SCENE.NON);
                    }
                    break;
                case 12:// ���݂̐i�s�x��12�ŁA�N�G�X�g4�Ԃ��󒍂�����
                    if (questNum == 4)
                    {
                        EventMng.SetChapterNum(12, SceneMng.SCENE.CONVERSATION);
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
                case 8: // ���݂̐i�s�x��8�ŁA�N�G�X�g1�Ԃ�B��������
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
                        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.NOON);      // ���Ԍo��
                        EventMng.SetChapterNum(14, SceneMng.SCENE.CONVERSATION);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
