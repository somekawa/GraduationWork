
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
public class ItemStore : HouseBase
{
    // 2��ړ������Ƃ����܂���b�����Ⴄ�o�O
    public override bool CheckEvent()
    {
        int num = EventMng.GetChapterNum();

        // �C�x���g����
        if (num >= 3 && num <= 5)   // �M���h����or���X����or���X�g��������
        {
            bool tmpFlg = false;
            // ���X�g���ɂ��łɖ��O�����������b�����Ȃ�
            foreach (string list in QuestClearCheck.buildList)
            {
                if (list == "ItemStore")
                {
                    tmpFlg = true;
                }
            }

            if(!tmpFlg)
            {
                EventMng.SetChapterNum(3, SceneMng.SCENE.CONVERSATION);
                return true;
            }
        }

        return false;
    }
}