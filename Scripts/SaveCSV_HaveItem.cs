using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_HaveItem : MonoBehaviour
{
    private const string magicSaveDataFilePath_ = @"Assets/Resources/HaveItemList.csv";
    private StreamWriter sw;

    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("data") as TextAsset;

        if (saveFile == null)
        {
            // Resources�t�H���_����SavaData�t�H���_�֐V�K�ō쐬����
            sw = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("�V�K�t�@�C���֏�������");
        }
        else
        {
            // �Â��f�[�^���폜
            File.Delete(magicSaveDataFilePath_);
            sw = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("�Â��f�[�^���폜���ăt�@�C����������");
            // ���łɑ��݂���ꍇ�́A�㏑���ۑ�����(��������false�ɂ��邱�ƂŁA�㏑���ɐ؂�ւ�����)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        //string[] s1 = { "F", "J", "time" };
        // �X�e�[�^�X�̍��ڌ��o��
        string[] s1 = { "ItemName", "Cnt"};
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    // �L�����̃X�e�[�^�X�������ł���ď������݂�����
    public void SaveItemData(Bag_Item.ItemData set)
    {
        // ���ۂ̃X�e�[�^�X�l
        string[] data = { set.name, set.haveCnt.ToString() };
        string write = string.Join(",", data);
        sw.WriteLine(write);
    }

    // �t�@�C�������Ƃ��ɌĂ�
    public void SaveEnd()
    {
        Debug.Log("�������݃t�@�C�������");
        sw.Close();
    }
}
