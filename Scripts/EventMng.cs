using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventMng
{
    private static int chapterNum = 0;   // 現在のチャプター進行度(0からスタート)

    // チャプター進行度の更新
    // 読み返し機能を作成するときには引数部分に該当するチャプター番号を入れるようにする
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
