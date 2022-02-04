using UnityEngine;

public static class EventMng
{
    // 自由に店に出入りしたいときは99とかいれとくといいかも。本来は0
    private static int chapterNum = 0;   // 現在のチャプター進行度(0からスタート)
    private static int oldNum = 0;

    // チャプター進行度の更新
    // 読み返し機能を作成するときには引数部分に該当するチャプター番号を入れるようにする
    public static void SetChapterNum(int num ,SceneMng.SCENE scene,bool loadFlg = false)
    {
        if(chapterNum == 8 && num == 9)
        {
            // チャプター9へ移動するさいに回復ポーション(小)を3つ受け取る
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>().ItemGetCheck(0,0,3);
        }

        if (!loadFlg)
        {
            // 現在のシーンをCONVERSATIONとする
            SceneMng.SetNowScene(SceneMng.SCENE.CONVERSATION);
        }

        bool tmpFlg = false;
        if (num == 100)
        {
            // 全滅時の特殊会話時には、全滅前のチャプター番号を保存しておく
            oldNum = chapterNum;
            tmpFlg = true;

            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                // バトルで死亡したまま終了していたときは、HP1の状態で立ち上がらせる
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
            // さっきまでの会話が全滅時の特殊会話だったときは、番号を100の前に入れてたやつに戻す
            chapterNum = oldNum;
        }
        else
        {
            // 通常の会話処理
            chapterNum = num;
        }

        SceneMng.SceneLoad((int)scene, tmpFlg);
    }

    public static int GetChapterNum()
    {
        return chapterNum;
    }

    // 進行上、その建物に入ってもいいか判断する
    // string hitName : ユニちゃんと当たり判定した建物
    public static string CheckEventHouse(string hitName)
    {
        if (hitName != "MayorHouse" && chapterNum == 0)
        {
            // チャプター0のときに、町長の家以外に当たった時の処理
            return "MayorHouse";
        }
        else if(hitName != "Guild" && chapterNum == 1)
        {
            // チャプター1のときに、ギルド以外に当たった時の処理
            return "Guild";
        }
        else if(hitName != "Guild" && chapterNum == 2)
        {
            // チャプター2のときに、ギルド以外に当たった時の処理
            return "Guild";
        }
        return "";      // イベント発生の建物なら空白を返す
    }
}
