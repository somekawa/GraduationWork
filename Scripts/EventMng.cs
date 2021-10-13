
public static class EventMng
{
    // ���R�ɓX�ɏo���肵�����Ƃ���99�Ƃ�����Ƃ��Ƃ��������B�{����0
    private static int chapterNum = 6;   // ���݂̃`���v�^�[�i�s�x(0����X�^�[�g)

    // �`���v�^�[�i�s�x�̍X�V
    // �ǂݕԂ��@�\���쐬����Ƃ��ɂ͈��������ɊY������`���v�^�[�ԍ�������悤�ɂ���
    public static void SetChapterNum(int num ,SceneMng.SCENE scene)
    {
        chapterNum = num;
        SceneMng.SceneLoad((int)scene);
    }

    public static int GetChapterNum()
    {
        return chapterNum;
    }

    // �i�s��A���̌����ɓ����Ă����������f����
    // string hitName : ���j�����Ɠ����蔻�肵������
    public static string CheckEventHouse(string hitName)
    {
        if (hitName != "MayorHouse" && chapterNum == 0)
        {
            // �`���v�^�[0�̂Ƃ��ɁA�����̉ƈȊO�ɓ����������̏���
            return "MayorHouse";
        }
        else if(hitName != "Guild" && chapterNum == 1)
        {
            // �`���v�^�[1�̂Ƃ��ɁA�M���h�ȊO�ɓ����������̏���
            return "Guild";
        }
        else if(hitName != "Guild" && chapterNum == 2)
        {
            // �`���v�^�[2�̂Ƃ��ɁA�M���h�ȊO�ɓ����������̏���
            return "Guild";
        }
        return "";      // �C�x���g�����̌����Ȃ�󔒂�Ԃ�
    }
}
