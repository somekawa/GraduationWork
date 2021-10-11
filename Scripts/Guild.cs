
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
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
        return false;
    }

    // �N�G�X�g���󒍂ŃC�x���g���i�s����Ƃ��Ɏg�p
    // QuestMng�ŃC���X�^���X���ČĂяo��
    public void GuildQuestEvent(int questNum,bool clearFlg = false)
    {
        if (EventMng.GetChapterNum() == 2 && questNum == 0) // ���݂̐i�s�x��2�ŁA�N�G�X�g0�Ԃ��󒍂�����
        {
            // �V�[���J�ږ���
            EventMng.SetChapterNum(3, SceneMng.SCENE.NON);
        }
        else if(EventMng.GetChapterNum() == 6 && questNum == 0) // ���݂̐i�s�x��6�ŁA�N�G�X�g0�Ԃ�B��������
        {
            if(clearFlg)
            {
                SceneMng.SetTimeGear(SceneMng.TIMEGEAR.EVENING);    // ���Ԍo��
                EventMng.SetChapterNum(6, SceneMng.SCENE.CONVERSATION);
            }
        }
    }
}
