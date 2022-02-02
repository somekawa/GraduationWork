using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_HaveItem : MonoBehaviour
{
    private string saveDataFilePath_;
    private StreamWriter sw_;

    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        saveDataFilePath_ = Application.streamingAssetsPath + "/HaveItemList.csv";

        // �Â��f�[�^���폜
        File.Delete(saveDataFilePath_);
        sw_ = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
        Debug.Log("HaveItem,�Â��f�[�^���폜���ăt�@�C����������");

        // �X�e�[�^�X�̍��ڌ��o��
        string[] s1 = {"Number", "ItemName", "Cnt"};
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // �L�����̃X�e�[�^�X�������ł���ď������݂�����
    public void SaveItemData(Bag_Item.ItemData set)
    {
        // ���ۂ̃X�e�[�^�X�l
        string[] data = {set.number.ToString(), set.name, set.haveCnt.ToString() };
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
