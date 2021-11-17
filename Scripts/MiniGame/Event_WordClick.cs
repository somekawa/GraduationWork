using UnityEngine;
using UnityEngine.EventSystems;

public class Event_WordClick : MonoBehaviour
{
    private MagicCreate wordCheck_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�

    public void OnClickWordBtn()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            wordCheck_ = GameObject.Find("MagicCreateMng").GetComponent<MagicCreate>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        wordCheck_.SetWord(clickbtn_.name);// �{�^���̖��O����
    } 
}