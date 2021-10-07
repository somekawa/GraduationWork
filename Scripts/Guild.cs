
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
public class Guild : HouseBase
{
    public override bool CheckEvent()
    {
        // �C�x���g����
        if (EventMng.GetChapterNum() == 1)
        {
            EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
            return true;
        }
        return false;
    }

    // �N�G�X�g���󒍂ŃC�x���g���i�s����Ƃ��Ɏg�p
    // QuestMng�ŃC���X�^���X���ČĂяo��
    public void GuildQuestEvent(int questNum)
    {
        if (EventMng.GetChapterNum() == 2 && questNum == 0) // ���݂̐i�s�x��2�ŁA�N�G�X�g0�Ԃ��󒍂�����
        {
            // �V�[���J�ږ���
            EventMng.SetChapterNum(3, SceneMng.SCENE.NON);
        }
    }
}
