using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventMng
{
    private static int chapterNum = 0;   // ���݂̃`���v�^�[�i�s�x(0����X�^�[�g)

    // �`���v�^�[�i�s�x�̍X�V
    // �ǂݕԂ��@�\���쐬����Ƃ��ɂ͈��������ɊY������`���v�^�[�ԍ�������悤�ɂ���
    public static void SetChapterNum(int num ,SceneMng.SCENE scene)
    {
        chapterNum += num;
        SceneMng.SceneLoad((int)scene);
    }

    public static int GetChapterNum()
    {
        return chapterNum;
    }
}
