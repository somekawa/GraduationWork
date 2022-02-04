using UnityEngine;

public static class EventMng
{
    // ���R�ɓX�ɏo���肵�����Ƃ���99�Ƃ�����Ƃ��Ƃ��������B�{����0
    private static int chapterNum = 0;   // ���݂̃`���v�^�[�i�s�x(0����X�^�[�g)
    private static int oldNum = 0;

    // �`���v�^�[�i�s�x�̍X�V
    // �ǂݕԂ��@�\���쐬����Ƃ��ɂ͈��������ɊY������`���v�^�[�ԍ�������悤�ɂ���
    public static void SetChapterNum(int num ,SceneMng.SCENE scene,bool loadFlg = false)
    {
        if(chapterNum == 8 && num == 9)
        {
            // �`���v�^�[9�ֈړ����邳���ɉ񕜃|�[�V����(��)��3�󂯎��
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>().ItemGetCheck(0,0,3);
        }

        if (!loadFlg)
        {
            // ���݂̃V�[����CONVERSATION�Ƃ���
            SceneMng.SetNowScene(SceneMng.SCENE.CONVERSATION);
        }

        bool tmpFlg = false;
        if (num == 100)
        {
            // �S�Ŏ��̓����b���ɂ́A�S�őO�̃`���v�^�[�ԍ���ۑ����Ă���
            oldNum = chapterNum;
            tmpFlg = true;

            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                // �o�g���Ŏ��S�����܂܏I�����Ă����Ƃ��́AHP1�̏�Ԃŗ����オ�点��
                if (SceneMng.charasList_[i].GetDeathFlg())
                {
                    SceneMng.charasList_[i].SetDeathFlg(false);
                    SceneMng.charasList_[i].SetHP(1);
                }
                SceneMng.charasList_[i].ButtleInit();
            }
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

        SceneMng.SceneLoad((int)scene, tmpFlg);
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
