using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    private static List<(GameObject,bool)> completeQuestsList = new List<(GameObject,bool)>();

    private static List<string> buildList_ = new List<string>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);
        }

        // リストを回して、フラグがtrueのものがあるか探す
        foreach (var tmp in completeQuestsList)
        {
            if (tmp.Item2)
            {
                Debug.Log(tmp.Item1.name + "のクエストは、クリア条件を満たしています");
            }
        }
    }

    public static void SetList(GameObject obj)
    {
        // リストに追加する
        completeQuestsList.Add((obj,false));
    }

    // まだ新規でクエストを受けられるか確認する
    public static bool CanOrderNewQuest(int num)
    {
        // null演算子( if(list != null && list.Count < 3)と同じ意味 )
        if (completeQuestsList?.Count < 3)
        {
            // 同じクエストを受けようとしているか確認する
            foreach (var tmp in completeQuestsList)
            {
                if (tmp.Item1.name == num.ToString())
                {
                    Debug.Log("現在受けているクエストと同じものは受けられません");
                    return false;
                }
            }

            // list数が2までなら、まだクエストを受けられる
            return true;
        }

        Debug.Log("既に3つクエストを受けているため、これ以上新規で受けられません");
        return false;   // 新規クエストを受けられない
    }

    public static void SetBuildName(string name)
    {
        // 0番のクエストを受けていなければreturnする
        foreach (var tmp in completeQuestsList)
        {
            if (int.Parse(tmp.Item1.name) != 0)
            {
                return;
            }
        }

        // 初回だけif文内に入るようにする
        if (buildList_.Count <= 0 )
        {
            // 先にあいさつ回りの関係がない建物名を登録しておく
            buildList_.Add("MayorHouse");
            buildList_.Add("Guild");
            buildList_.Add("UniHouse");
        }

        bool tmpFlg = false;    // すでに登録済みの建物からの退出ならtrue
        foreach (string list in buildList_)
        {
            if(list == name)
            {
                tmpFlg = true;
            }
        }

        if(!tmpFlg)
        {
            buildList_.Add(name);
        }

        // 全ての建物へあいさつ回りができた
        if(buildList_.Count >= 6)
        {
            for(int i = 0; i < completeQuestsList.Count; i++)
            {
                if(int.Parse(completeQuestsList[i].Item1.name) == 0)
                {
                    // エラーの書き方
                    // completeQuestsList[i].Item2 = true;

                    // 一時変数に保存してから代入する方法で対処する
                    (GameObject, bool) tmpData = completeQuestsList[i];
                    tmpData.Item2 = true;
                    completeQuestsList[i] = tmpData;
                }
            }

            // クリア条件処理は終了したので、リストを削除して使えないようにしておく
            buildList_.Clear();
            buildList_ = null;
        }
    }
}
