
// HouseBaseを継承している町長の家を管理するクラス
using UnityEngine;

public class Restaurant : HouseBase
{
    private UnityChan.FaceUpdate npcController_;
    private Animator animator_;
    private int key_Move = Animator.StringToHash("Move");

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
                if (list == "Restaurant")
                {
                    tmpFlg = true;
                }
            }

            if (!tmpFlg)
            {
                EventMng.SetChapterNum(5, SceneMng.SCENE.CONVERSATION);
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
                if (Bag_Word.data[i].name != "命中/回避")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // 命中/回避
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 4, 19);
                    EventMng.SetChapterNum(103, SceneMng.SCENE.CONVERSATION);
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
            npcController_ = GameObject.Find("HouseInterior/Restaurant/Toko").GetComponent<UnityChan.FaceUpdate>();
        }

        npcController_.OnCallChangeFace(faceStr);
    }

    public void ChangeMotion(bool flag)
    {
        if (animator_ == null)
        {
            animator_ = GameObject.Find("HouseInterior/Restaurant/Toko").GetComponent<Animator>();
        }

        animator_.SetBool(key_Move,flag);
    }
}
