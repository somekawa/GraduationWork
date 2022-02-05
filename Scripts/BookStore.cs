
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
using UnityEngine;

public class BookStore : HouseBase
{
    private UnityChan.FaceUpdate npcController_;

    public override bool CheckEvent()
    {
        int num = EventMng.GetChapterNum();

        // �C�x���g����
        if (num >= 3 && num <= 5)   // �M���h����or���X����or���X�g��������
        {
            bool tmpFlg = false;
            // ���X�g���ɂ��łɖ��O�����������b�����Ȃ�
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
                if (Bag_Word.data[i].name != "�����U��")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // �����U����
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 1, 16);
                    EventMng.SetChapterNum(101, SceneMng.SCENE.CONVERSATION);
                    return true;
                }
            }
        }

        return false;
    }

    // NPC�̕\���ω�������
    public void ChangeNPCFace(string faceStr)
    {
        if (npcController_ == null)
        {
            npcController_ = GameObject.Find("HouseInterior/BookStore/Marie").GetComponent<UnityChan.FaceUpdate>();
        }

        npcController_.OnCallChangeFace(faceStr);
    }

}
