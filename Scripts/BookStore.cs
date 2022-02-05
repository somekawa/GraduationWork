
// HouseBaseを継承している町長の家を管理するクラス
using UnityEngine;

public class BookStore : HouseBase
{
    private UnityChan.FaceUpdate npcController_;

    public override bool CheckEvent()
    {
        int num = EventMng.GetChapterNum();

        // イベント発生
        if (num >= 3 && num <= 5)   // ギルドからor書店からorレストランから
        {
            bool tmpFlg = false;
            // リスト内にすでに名前があったら会話を入れない
            foreach (string list in QuestClearCheck.buildList)
            {
                if (list == "BookStore")
                {
                    tmpFlg = true;
                }
            }

            if (!tmpFlg)
            {
                EventMng.SetChapterNum(4, SceneMng.SCENE.CONVERSATION);
                return true;
            }
        }
        else
        {
            if(num < 15)
            {
                return false;
            }

            for (int i = 0; i < Bag_Word.data.Length; i++)
            {
                if (Bag_Word.data[i].name != "物理攻撃")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // 物理攻撃力
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 1, 16);
                    EventMng.SetChapterNum(101, SceneMng.SCENE.CONVERSATION);
                    return true;
                }
            }
        }

        return false;
    }

    // NPCの表情を変化させる
    public void ChangeNPCFace(string faceStr)
    {
        if (npcController_ == null)
        {
            npcController_ = GameObject.Find("HouseInterior/BookStore/Marie").GetComponent<UnityChan.FaceUpdate>();
        }

        npcController_.OnCallChangeFace(faceStr);
    }

}
