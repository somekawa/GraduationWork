using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIconClick : MonoBehaviour
{
    private EventSystem eventSystem_;// �{�^���N���b�N�̂��߂̃C�x���g����
    private GameObject clickbtn_;    // �ǂ̃{�^�����N���b�N�������������ϐ�
    private Text info_; // �N���b�N�����A�C�e����������闓

    private Bag_Item bagItem_;
    private Bag_Materia bagMateria_;
    private InitPopList popItemList_;
    private int maxCnt_ = 0;
    public void OnClickBagItemIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
            info_.text = "";
            popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
            bagItem_ = GameObject.Find("Managers").GetComponent<Bag_Item>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        maxCnt_ = popItemList_.SetMaxItemCount();
        // �{�^�������琔���݂̂����o��
        int nameNum = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        int infoNum = nameNum;
        if(maxCnt_<= infoNum)
        {
            infoNum -= maxCnt_;
        }
        info_.text = Bag_Item.itemState[nameNum].name + "\n" + Bag_Item.itemState[infoNum].info;
        bagItem_.SetItemNumber(nameNum);// �ǂ̃{�^�������������ۑ�����
    }

    public void OnClickBagMateriaIcon()
    {
        if (eventSystem_ == null)
        {
            eventSystem_ = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            info_ = GameObject.Find("InfoBack/InfoText").GetComponent<Text>();
            info_.text = "";
            bagMateria_ = GameObject.Find("Managers").GetComponent<Bag_Materia>();
        }
        clickbtn_ = eventSystem_.currentSelectedGameObject;
        // �{�^�������琔���݂̂����o��
        int number = int.Parse(Regex.Replace(clickbtn_.name, @"[^0-9]", ""));
        info_.text = Bag_Materia.materiaState[number].name + "\n" + Bag_Materia.materiaState[number].info;
        bagMateria_.SetMateriaNumber(number);// �ǂ̃{�^�������������ۑ�����
    }
}
