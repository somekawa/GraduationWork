
// HouseBase���p�����Ă��钬���̉Ƃ��Ǘ�����N���X
public class MayorHouse : HouseBase
{
    public override bool CheckEvent()
    {
        // �C�x���g����
        if (EventMng.GetChapterNum() == 0)
        {
            EventMng.SetChapterNum(1, (int)SceneMng.SCENE.CONVERSATION);
            return true;
        }
        return false;
    }
}
