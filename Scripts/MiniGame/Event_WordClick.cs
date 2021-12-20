using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Event_WordClick : MonoBehaviour
{
    private MagicCreate wordCheck_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private Bag_Word bagWord_;
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private Text info_; // �N���b�N�����A�C�e����������闓

    public void OnClickWordBtn()
    {
        if (SceneMng.nowScene != SceneMng.SCENE.UNIHOUSE)
        {
            if (eventSystem_ == null)
            {
                eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
                bagWord_ = GameObject.Find("Managers").GetComponent<Bag_Word>(); 
                info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
                info_.text = "";
            }
            clickbtn_ = eventSystem_.currentSelectedGameObject;
            int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
            Debug.Log("�������ɕ\�����郏�[�h�F"+Bag_Word.data[number].name);
            info_.text = Bag_Word.data[number].name; //  number.ToString();
            // bagWord_.wordState[,number].name + "\n" + Bag_Materia.materiaState[number].info;
            //bagWord_.SetMateriaNumber(number);// �ǂ̃{�^�������������ۑ�����
        }
        else
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
}