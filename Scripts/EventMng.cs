using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventMng
{
    public static int chapterNum = 0;   // ���݂̃`���v�^�[�i�s�x(0����X�^�[�g)

    // �`���v�^�[�i�s�x�̍X�V(�����Ȃ��Ȃ�+1�ɂ���)
    // �ǂݕԂ��@�\���쐬����Ƃ��ɂ͈��������ɊY������`���v�^�[�ԍ�������悤�ɂ���
    public static void SetChapterNum(int num = 1)
    {
        chapterNum += num;
    }
}
