using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SaveCSV_Book : MonoBehaviour
{
    //List<string[]> csvDatas_ = new List<string[]>(); // CSV�̒��g�����郊�X�g;
    private const string bookSaveDataFilePath_ = @"Assets/Resources/Save/bookData.csv";
    private StreamWriter sw_;

    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("Save/bookData") as TextAsset;

        if (saveFile == null)
        {
            // Resources�t�H���_����SavaData�t�H���_�֐V�K�ō쐬����
            sw_ = new StreamWriter(bookSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("�V�K�t�@�C���֏�������");
        }
        else
        {
            // �Â��f�[�^���폜
            File.Delete(bookSaveDataFilePath_);
            sw_ = new StreamWriter(bookSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("�Â��f�[�^���폜���ăt�@�C����������");
            // ���łɑ��݂���ꍇ�́A�㏑���ۑ�����(��������false�ɂ��邱�ƂŁA�㏑���ɐ؂�ւ�����)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        // �X�e�[�^�X�̍��ڌ��o��
        string[] s1 = { "Number","Name", "ReadCheck" };
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // �L�����̃X�e�[�^�X�������ł���ď������݂�����
    public void SaveBookData(BookStoreMng.BookData set)
    {
        // ���ۂ̃X�e�[�^�X�l
        string[] data = { set.bookNumber.ToString(), set.bookName, set.readFlag.ToString() };

        // set.sprite.ToString() 
        string write = string.Join(",", data);
        sw_.WriteLine(write);
    }

    // �t�@�C�������Ƃ��ɌĂ�
    public void SaveEnd()
    {
        //  Debug.Log("�������݃t�@�C�������");
        sw_.Close();
    }


    //public void DataLoad()
    //{
    //    // Debug.Log("���[�h���܂�");

    //    csvDatas_.Clear();

    //    // �s���������ɂƂǂ߂�
    //    string[] texts = File.ReadAllText(bookSaveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    //    for (int i = 0; i < texts.Length; i++)
    //    {
    //        // �J���}��؂�Ń��X�g�֓o�^���Ă���(2�����z���ԂɂȂ�[�s�ԍ�][�J���}��؂�])
    //        csvDatas_.Add(texts[i].Split(','));
    //    }
    //}


}
