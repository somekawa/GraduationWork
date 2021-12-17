using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV_Book : MonoBehaviour
{
    private const string magicSaveDataFilePath_ = @"Assets/Resources/Save/bookData.csv";
    private StreamWriter sw_;

    // 書き込み始めに呼ぶ
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("Save/bookData") as TextAsset;

        if (saveFile == null)
        {
            // Resourcesフォルダ内のSavaDataフォルダへ新規で作成する
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("新規ファイルへ書き込み");
        }
        else
        {
            // 古いデータを削除
            File.Delete(magicSaveDataFilePath_);
            sw_ = new StreamWriter(magicSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("古いデータを削除してファイル書き込み");
            // すでに存在する場合は、上書き保存する(第二引数をfalseにすることで、上書きに切り替えられる)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        // ステータスの項目見出し
        string[] s1 = { "Name", "ReadCheck" };
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveBookData(BookStoreMng.BookData set)
    {
        // 実際のステータス値
        string[] data = { set.name, set.readFlag.ToString() };

        // set.sprite.ToString() 
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
