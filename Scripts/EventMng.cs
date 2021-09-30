using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventMng
{
    public static int chapterNum = 0;   // 現在のチャプター進行度(0からスタート)

    // チャプター進行度の更新(引数なしなら+1にする)
    // 読み返し機能を作成するときには引数部分に該当するチャプター番号を入れるようにする
    public static void SetChapterNum(int num = 1)
    {
        chapterNum += num;
    }
}
