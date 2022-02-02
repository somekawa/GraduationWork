using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_HaveItem : MonoBehaviour
{
    private string saveDataFilePath_;
    private StreamWriter sw_;

    // 書き込み始めに呼ぶ
    public void SaveStart()
    {
        saveDataFilePath_ = Application.streamingAssetsPath + "/HaveItemList.csv";

        // 古いデータを削除
        File.Delete(saveDataFilePath_);
        sw_ = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
        Debug.Log("HaveItem,古いデータを削除してファイル書き込み");

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
