
// HouseBaseを継承している町長の家を管理するクラス
using UnityEngine;

public class ItemStore : HouseBase
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
                if (list == "ItemStore")
                {
                    tmpFlg = true;
                }
            }

            if(!tmpFlg)
            {
                // Chapter3の挨拶クエストのときに、空のマテリアを5個もらう
                GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>().MateriaGetCheck(34,5);
                EventMng.SetChapterNum(3, SceneMng.SCENE.CONVERSATION);
                return true;
            }
        }
        else
        {
            if (num < 15)
            {
                return false;
            }

            for (int i = 0; i < Bag_Word.data.Length; i++)
            {
                if (Bag_Word.data[i].name != "魔法攻撃")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // 魔法攻撃力
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 2, 17);
                    EventMng.SetChapterNum(102, SceneMng.SCENE.CONVERSATION);
                    return true;
                }
            }
        }


        return false;
    }

    public void ChangeNPCFace(string faceStr)
    {
        if (npcController_ == null)
        {
            npcController_ = GameObject.Find("HouseInterior/ItemStore/Akaza").GetComponent<UnityChan.FaceUpdate>();
        }

        npcController_.OnCallChangeFace(faceStr);
    }

}
