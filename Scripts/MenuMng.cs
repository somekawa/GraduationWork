using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MenuMng : MonoBehaviour
{
    private TMPro.TextMeshProUGUI statusInfo_;  // �L�����̃X�e�[�^�X�`���
    private GameObject buttons_;                // �{�^���ނ��܂Ƃ߂��I�u�W�F�N�g


    void Start()
    {
        statusInfo_ = this.transform.Find("StatusInfo").GetComponent<TMPro.TextMeshProUGUI>();
        buttons_ = this.transform.Find("Buttons").gameObject;
    }

    public void OnClickStatus()
    {
        Debug.Log("�X�e�[�^�X�m�F�{�^���������ꂽ");
        buttons_.SetActive(false);

        // �L�����̃X�e�[�^�X�l��\����������
        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // �\�����镶���̍쐬
        string str = "���O  :"   + data.name               + "\n" +
                     "���x��:"   + data.Level.ToString()   + "\n" +
                     "HP    :"   + data.HP.ToString()      + "\n" +
                     "MP    :"   + data.MP.ToString()      + "\n" +
                     "��    :"   + data.Constitution.ToString() + "\n" +
                     "���_  :"   + data.Power.ToString()   + "\n" +
                     "�U����:"   + data.Attack.ToString()  + "\n" +
                     "�h���:"   + data.Defence.ToString() + "\n" +
                     "�f����:"   + data.Speed.ToString()   + "\n" +
                     "�K�^  :"   + data.Luck.ToString();

        // �쐬��������������
        statusInfo_.text = str;
    }

    public void OnClickSave()
    {
        Debug.Log("�Z�[�u�{�^���������ꂽ");

        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // �f�[�^�����o���e�X�g
        StreamWriter swLEyeLog;
        FileInfo fiLEyeLog;

        // �ۑ��ʒu
        fiLEyeLog = new FileInfo(Application.dataPath + "/saveData.csv");

        swLEyeLog = fiLEyeLog.AppendText();

        // �������ݓ��e�̍쐬
        string str = data.name + "," +
                     data.Level.ToString()   + "," +
                     data.HP.ToString()      + "," +
                     data.MP.ToString()      + "," +
                     data.Constitution.ToString() + "," +
                     data.Power.ToString()   + "," +
                     data.Attack.ToString()  + "," +
                     data.Defence.ToString() + "," +
                     data.Speed.ToString()   + "," +
                     data.Luck.ToString();

        swLEyeLog.Write(str);   // ��������
        swLEyeLog.Flush();
        swLEyeLog.Close();
    }

    public void OnClickCancel()
    {
        Debug.Log("�L�����Z���{�^���������ꂽ");

        if(buttons_.activeSelf)
        {
            // �t�B�[���h�ɖ߂�
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
        }
        else
        {
            // �{�^���I���ɖ߂�
            buttons_.SetActive(true);
            statusInfo_.text = "";
        }
    }
}
