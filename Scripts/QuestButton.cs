using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Button button;
    private int questNum_;  // 自分がインスタンスされた時に設定されたクエスト番号

    public void SetQuestNum(int num)
    {
        questNum_ = num;
    }

    public int GetQuestNum()
    {
        return questNum_;
    }

    public void OnClickButton()
    {
        Debug.Log(questNum_ + "の番号を渡します");
        GameObject.Find("HouseInterior/Guild/QuestMng").GetComponent<QuestMng>().SetSelectQuest(questNum_);
    }

    public void TestRButton()
    {
        Debug.Log(questNum_ + "の番号を渡します");
        GameObject.Find("HouseInterior/Restaurant/RestaurantMng").GetComponent<RestaurantMng>().SetSelectOrder(questNum_);
    }
}