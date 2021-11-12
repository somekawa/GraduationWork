using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Buy : MonoBehaviour
{
    private InitPopList popItemsList_;

    [SerializeField]
    private RectTransform buyParent;   // �\���ʒu�̐e

    private GameObject[] activeObj_;    //�v���n�u�������Ɏg�p
    private Text[] activePrice_;        // �\������l�i
    private Text[] activeText_;         // �\������f�ނ̖��O

    private int maxCnt_ = 0;            // ���ׂĂ̑f�ސ�
    private int singleCnt_ = 0;         // 1�̃V�[�g�ɋL�ڂ���Ă�ő��

    private int nowFieldNum_ = 2;       // ���݂̃t�B�[���h

    private void Start()
    {
        popItemsList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
     
        maxCnt_ = popItemsList_.SetMaxItemCount();
        activeObj_ = new GameObject[maxCnt_];
        activePrice_ = new Text[maxCnt_];
        activeText_ = new Text[maxCnt_];
        
        for (int i = 0; i < maxCnt_; i++)
        {
            //  Debug.Log("�X�Ŕ��������" + PopMateriaList.activeObj_[i].name);
            activeObj_[i] = PopListInTown.materiaPleate[i];

            // �\�����閼�O��ύX����
            activeText_[i] = activeObj_[i].transform.Find("Name").GetComponent<Text>();
            activeText_[i].text = activeObj_[i].name;

            // ������\������Text
            activePrice_[i] = activeObj_[i].transform.Find("Price").GetComponent<Text>();
        }
    }

    public void SetActiveBuy()
    {
        // ���ݐi�s���Ă���t�B�[���h�̑f�ނ����\�����Ȃ��悤�ɂ���
        for (int i = 0; i < singleCnt_ * nowFieldNum_; i++)
        {
            // �e�ʒu��ς���
            activeObj_[i].transform.SetParent(buyParent.transform);
          
            // �\�����闿���𔃂��l�ɕύX
            activePrice_[i].text = InitPopList.materiaData[i].buyPrice.ToString() + "�r�b�g";

            // ���ׂăA�N�e�B�u��Ԃɂ���
            activeObj_[i].SetActive(true);
        }
    }
}