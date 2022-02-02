using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_Materia : MonoBehaviour
{
    private string magicSaveDataFilePath_;
    private StreamWriter sw_;

    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        magicSaveDataFilePath_ = Application.streamingAssetsPath + "/materiaData.csv";

        // �Â��f�[�^���폜
        File.Delete(magicSaveDataFilePath_);
        sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
        Debug.Log("materiaData,�Â��f�[�^���폜���ăt�@�C����������");
        // ���łɑ��݂���ꍇ�́A�㏑���ۑ�����(��������false�ɂ��邱�ƂŁA�㏑���ɐ؂�ւ�����)
        //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));

        // �X�e�[�^�X�̍��ڌ��o��
        string[] s1 = { "Number", "MateriaName", "Cnt" };
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // �L�����̃X�e�[�^�X�������ł���ď������݂�����
    public void SaveMateriaData(Bag_Materia.MateriaData set)
    {
        // ���ۂ̃X�e�[�^�X�l
        string[] data = { set.number.ToString(), set.name, set.haveCnt.ToString() };

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
