
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
public class Restaurant : HouseBase
{
    public override bool CheckEvent()
    {
        // �C�x���g����
        //if (EventMng.GetChapterNum() == 1)
        //{
        //    EventMng.SetChapterNum(2, SceneMng.SCENE.CONVERSATION);
        //    return true;
        //}
        return false;
    }
}
