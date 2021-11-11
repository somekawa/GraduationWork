using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Sell : MonoBehaviour
{
    private PopListInTown popItemsList_; 

    [SerializeField]
    private RectTransform sellParent_;  // �\���ʒu�̐e

    private GameObject[] activeObj_;    //�v���n�u�������Ɏg�p
    private Text[] activePrice_;        // �\������l�i
    private Text[] activeText_;         // �\������f�ނ̖��O

    private int maxCnt_ = 0;            // ���ׂĂ̑f�ސ�

    private void Start()
    {
        popItemsList_ = GameObject.Find("SceneMng").GetComponent<PopListInTown>();
       
        maxCnt_ = popItemsList_.SetMaxItemsCount();
        activeObj_ = new GameObject[maxCnt_];//�v���n�u�������Ɏg�p
        activePrice_ = new Text[maxCnt_];
        activeText_ = new Text[maxCnt_];

        for (int i = 0; i < maxCnt_; i++)
        {
            activeObj_[i] = PopListInTown.activeObj_[i];
            //  Debug.Log("�o�b�O�̒��g" + PopMateriaList.activeObj_[i].name);
            // �\�����閼�O��ύX����
            activeText_[i] = activeObj_[i].transform.Find("Name").GetComponent<Text>();
            activeText_[i].text = activeObj_[i].name;

            // ������\������Text
            activePrice_[i] = activeObj_[i].transform.Find("Price").GetComponent<Text>();
            activeObj_[i].SetActive(false);
        }
    }

    public void SetActiveSell()
    {
        // Debug.Log(objParent_.transform.childCount);
        for (int i = 0; i < maxCnt_; i++)
        {
            // �e�ʒu��ς���
            activeObj_[i].transform.SetParent(sellParent_.transform);

            // �\�����闿���𔄒l�ɕύX
            activePrice_[i].text = PopListInTown.mateiraSellPrice_[i].ToString() + "�r�b�g";

            // ��������0�ȏ�Ńo�b�O�̒��g�Ɠ������̂�����Ε\��
            if (0 < Bag_Materia.materiaState[i].haveCnt)
            {
                activeObj_[i].SetActive(true);
            }
            else
            {
                activeObj_[i].SetActive(false);
            }
        }
    }

    public void SetHaveCntCheck(int materiaNum)
    {
        // �w��f�ނ̏�������0�ɂȂ�����Ăяo�����
       activeObj_[materiaNum].SetActive(false);
    }
}

