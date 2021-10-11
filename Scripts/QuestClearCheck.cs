using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    private int deleteNum_ = -1;

    // 受注中のクエストを保存するリスト(最大3つ)
    private static List<(GameObject,bool)> orderQuestsList_ = new List<(GameObject,bool)>();
    // クリア済みのクエストを保存するリスト
    private static List<GameObject> clearedQuestsList_ = new List<GameObject>();
    // チュートリアルクエスト用のリスト(出入りした建物を記録する)
    public static List<string> buildList = new List<string>();

    void Update()
    {
        // テスト用(クエストを全て達成状態にできるようにする)
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    for (int i = 0; i < orderQuestsList_.Count; i++)
        //    {
        //        // 一時変数に保存してから代入する方法で対処する
        //        (GameObject, bool) tmpData = orderQuestsList_[i];
        //        tmpData.Item2 = true;
        //        orderQuestsList_[i] = tmpData;
        //    }
        //}

        // リストを回して、フラグがtrueのものがあるか探す
        foreach (var tmp in orderQuestsList_)
        {
            if (tmp.Item2)
            {
                // 削除予定番号として代入する
                deleteNum_ = int.Parse(tmp.Item1.name);
                Debug.Log(tmp.Item1.name + "のクエストは、クリア条件を満たしています");
                break;
            }
        }

        // 削除番号が初期値の-1ではない場合は処理を行う
        if(deleteNum_ >= 0)
        {
            for(int i = 0; i < orderQuestsList_.Count; i++)
            {
                // 削除番号と一致していたら
                if(orderQuestsList_[i].Item1.name == deleteNum_.ToString())
                {
                    // クリア済みリストへコピーする
                    clearedQuestsList_.Add(orderQuestsList_[i].Item1);
                    // 消したいクエストに一番最後の要素を移す
                    //(削除番号が0でリストの最後が2なら、2を0のあったところにコピーして、末尾の2を削除する方式)
                    orderQuestsList_[i] = orderQuestsList_[orderQuestsList_.Count - 1];
                    // 一番最後の要素を削除
                    orderQuestsList_.RemoveAt(orderQuestsList_.Count - 1);
                    // 削除番号の初期化
                    deleteNum_ = -1;
                    break;
                }
            }
        }
    }

    public static void SetOrderQuestsList(GameObject obj)
    {
        // リストに追加する
        orderQuestsList_.Add((obj,false));
    }

    public static List<(GameObject, bool)> GetOrderQuestsList()
    {
        return orderQuestsList_;
    }

    public static void SetClearedQuestsList(int num)
    {
        for (int i = 0; i < clearedQuestsList_.Count; i++)
        {
            // 削除番号と一致していたら
            if (clearedQuestsList_[i].name == num.ToString())
            {
                // 消したいクエストに一番最後の要素を移す
                clearedQuestsList_[i] = clearedQuestsList_[clearedQuestsList_.Count - 1];
                // 一番最後の要素を削除
                clearedQuestsList_.RemoveAt(clearedQuestsList_.Count - 1);
                break;
            }
        }
    }

    public static List<GameObject> GetClearedQuestsList()
    {
        return clearedQuestsList_;
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
        if (orderQuestsList_.Count <= 0 || int.Parse(orderQuestsList_[0].Item1.name) != 0)
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
