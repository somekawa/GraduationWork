using System.Collections.Generic;
using UnityEngine;

public class QuestClearCheck : MonoBehaviour
{
    private static List<(GameObject,bool)> completeQuestsList = new List<(GameObject,bool)>();

    private void Update()
    {
        // リストを回して、フラグがtrueのものがあるか探す
        foreach (var tmp in completeQuestsList)
        {
            if(tmp.Item2)
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
}
