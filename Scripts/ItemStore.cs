
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
using UnityEngine;

public class ItemStore : HouseBase
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
                if (list == "ItemStore")
                {
                    tmpFlg = true;
                }
            }

            if(!tmpFlg)
            {
                // Chapter3�̈��A�N�G�X�g�̂Ƃ��ɁA��̃}�e���A��5���炤
                GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>().MateriaGetCheck(34,5);
                EventMng.SetChapterNum(3, SceneMng.SCENE.CONVERSATION);
                return true;
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
