using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_HaveItem : MonoBehaviour
{
    private const string magicSaveDataFilePath_ = @"Assets/Resources/HaveItemList.csv";
    private StreamWriter sw_;

    // �������ݎn�߂ɌĂ�
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("HaveItemList") as TextAsset;

        if (saveFile == null)
        {
            // Resources�t�H���_����SavaData�t�H���_�֐V�K�ō쐬����
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
         //   Debug.Log("�V�K�t�@�C���֏�������");
        }
        else
        {
            // �Â��f�[�^���폜
            File.Delete(magicSaveDataFilePath_);
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
          //  Debug.Log("�Â��f�[�^���폜���ăt�@�C����������");
            // ���łɑ��݂���ꍇ�́A�㏑���ۑ�����(��������false�ɂ��邱�ƂŁA�㏑���ɐ؂�ւ�����)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

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
