using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_HaveItem : MonoBehaviour
{
    private const string magicSaveDataFilePath_ = @"Assets/Resources/HaveItemList.csv";
    private StreamWriter sw_;

    // 書き込み始めに呼ぶ
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("HaveItemList") as TextAsset;

        if (saveFile == null)
        {
            // Resourcesフォルダ内のSavaDataフォルダへ新規で作成する
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
         //   Debug.Log("新規ファイルへ書き込み");
        }
        else
        {
            // 古いデータを削除
            File.Delete(magicSaveDataFilePath_);
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
          //  Debug.Log("古いデータを削除してファイル書き込み");
            // すでに存在する場合は、上書き保存する(第二引数をfalseにすることで、上書きに切り替えられる)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        // ステータスの項目見出し
        string[] s1 = {"Number", "ItemName", "Cnt"};
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveItemData(Bag_Item.ItemData set)
    {
        // 実際のステータス値
        string[] data = {set.number.ToString(), set.name, set.haveCnt.ToString() };
        string write = string.Join(",", data);
        sw_.WriteLine(write);
    }

    // ファイルを閉じるときに呼ぶ
    public void SaveEnd()
    {
        //  Debug.Log("書き込みファイルを閉じた");
        sw_.Close();
    }
}
