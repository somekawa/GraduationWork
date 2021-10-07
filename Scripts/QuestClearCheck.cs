using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    // 受注中のクエストを保存するリスト(最大3つ)
    private static List<(GameObject,bool)> orderQuestsList_ = new List<(GameObject,bool)>();
    // チュートリアルクエスト用のリスト(出入りした建物を記録する)
    public static List<string> buildList = new List<string>();

    private void Update()
    {
        // リストを回して、フラグがtrueのものがあるか探す
        foreach (var tmp in orderQuestsList_)
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
        orderQuestsList_.Add((obj,false));
    }

    // まだ新規でクエストを受けられるか確認する
    public static bool CanOrderNewQuest(int num)
    {
        // null演算子( if(list != null && list.Count < 3)と同じ意味 )
        if (orderQuestsList_?.Count < 3)
        {
            // 同じクエストを受けようとしているか確認する
            foreach (var tmp in orderQuestsList_)
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
        // チュートリアルクエストを受注した時は、クエストが1つしかギルドに表示されていない設定にする
        if (int.Parse(orderQuestsList_[0].Item1.name) != 0)
        {
            return;
        }

        // クリア済みならreturnする
        if (orderQuestsList_[0].Item2)
        {
            return;
        }

        // 初回だけif文内に入るようにする
        if (buildList.Count <= 0 )
        {
            // 先にあいさつ回りの関係がない建物名を登録しておく
            buildList.Add("MayorHouse");
            buildList.Add("Guild");
            buildList.Add("UniHouse");
        }

        bool tmpFlg = false;    // すでに登録済みの建物からの退出ならtrue
        foreach (string list in buildList)
        {
            if(list == name)
            {
                tmpFlg = true;
            }
        }

        // 未登録の建物名だったら、addする
        if(!tmpFlg)
        {
            buildList.Add(name);
        }

        // 全ての建物へあいさつ回りができた(建物名が6箇所分入っている)
        if(buildList.Count >= 6)
        {
            for(int i = 0; i < orderQuestsList_.Count; i++)
            {
                if(int.Parse(orderQuestsList_[i].Item1.name) == 0)
                {
                    // エラーのでる書き方
                    // completeQuestsList[i].Item2 = true;

                    // 一時変数に保存してから代入する方法で対処する
                    (GameObject, bool) tmpData = orderQuestsList_[i];
                    tmpData.Item2 = true;
                    orderQuestsList_[i] = tmpData;

                    // イベント進行度を6にする
                    EventMng.SetChapterNum(6, SceneMng.SCENE.NON);
                }
            }

            // クリア条件処理は終了したので、リストを削除して使えないようにしておく
            buildList.Clear();
            buildList = null;
        }
    }
}
