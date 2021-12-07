// 建物内処理の親クラス 共通処理はvirtualで継承先に処理を書く

public class HouseBase
{
    // イベントを発生させるか確認する
    public virtual bool CheckEvent()
    {
        return false;
    }
}
