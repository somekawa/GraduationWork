
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
using UnityEngine;

public class Restaurant : HouseBase
{
    private UnityChan.FaceUpdate npcController_;
    private Animator animator_;
    private int key_Move = Animator.StringToHash("Move");

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
                if (Bag_Word.data[i].name != "����/���")
                {
                    continue;
                }
                if (Bag_Word.data[i].getFlag == 0)
                {
                    // ����/���
                    GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().WordGetCheck(InitPopList.WORD.SUB2, 4, 19);
                    EventMng.SetChapterNum(103, SceneMng.SCENE.CONVERSATION);
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
