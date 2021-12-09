
public static class EventMng
{
    // ���R�ɓX�ɏo���肵�����Ƃ���99�Ƃ�����Ƃ��Ƃ��������B�{����0
    private static int chapterNum = 17;   // ���݂̃`���v�^�[�i�s�x(0����X�^�[�g)
    private static int oldNum = 0;

    // �`���v�^�[�i�s�x�̍X�V
    // �ǂݕԂ��@�\���쐬����Ƃ��ɂ͈��������ɊY������`���v�^�[�ԍ�������悤�ɂ���
    public static void SetChapterNum(int num ,SceneMng.SCENE scene,bool loadFlg = false)
    {
        if(!loadFlg)
        {
            // ���݂̃V�[����CONVERSATION�Ƃ���
            SceneMng.SetNowScene(SceneMng.SCENE.CONVERSATION);
        }

        if (num == 100)
        {
            // �S�Ŏ��̓����b���ɂ́A�S�őO�̃`���v�^�[�ԍ���ۑ����Ă���
            oldNum = chapterNum;
        }

        if (oldNum != 100 && num == 101)
        {
            // �������܂ł̉�b���S�Ŏ��̓����b�������Ƃ��́A�ԍ���100�̑O�ɓ���Ă���ɖ߂�
            chapterNum = oldNum;
        }
        else
        {
            // �ʏ�̉�b����
            chapterNum = num;
        }

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
