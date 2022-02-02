using System.IO;
using System.Text;
using UnityEngine;

public class Word_SaveCSV : MonoBehaviour
{
    private string wordSaveDataFilePath_;
    private StreamWriter sw_;
    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        wordSaveDataFilePath_ = Application.streamingAssetsPath + "/Save/wordData.csv";

        // �Â��f�[�^���폜
        File.Delete(wordSaveDataFilePath_);
        sw_ = new StreamWriter(wordSaveDataFilePath_, true, Encoding.UTF8);
        Debug.Log("worddata,�Â��f�[�^���폜���ăt�@�C����������");
        // ���łɑ��݂���ꍇ�́A�㏑���ۑ�����(��������false�ɂ��邱�ƂŁA�㏑���ɐ؂�ւ�����)
        //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));

        // �X�e�[�^�X�̍��ڌ��o��
        string[] s1 = {"Number", "Word", "GetCheck" };
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // �L�����̃X�e�[�^�X�������ł���ď������݂�����
    public void SaveWordData(Bag_Word.WordData set)
    {
        // ���ۂ̃X�e�[�^�X�l
        string[] data = { set.number.ToString() ,set.name, set.getFlag.ToString() };

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
}
