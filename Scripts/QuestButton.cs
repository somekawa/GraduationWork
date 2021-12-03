using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour
{
    public Button button;
    private int questNum_;  // �������C���X�^���X���ꂽ���ɐݒ肳�ꂽ�N�G�X�g�ԍ�

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
        Debug.Log(questNum_ + "�̔ԍ���n���܂�");
        GameObject.Find("HouseInterior/Guild/QuestMng").GetComponent<QuestMng>().SetSelectQuest(questNum_);
    }

    public void TestRButton()
    {
        Debug.Log(questNum_ + "�̔ԍ���n���܂�");
        GameObject.Find("HouseInterior/Restaurant/RestaurantMng").GetComponent<RestaurantMng>().SetSelectOrder(questNum_);
    }
}