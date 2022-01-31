using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SaveCSV_Book : MonoBehaviour
{
    //List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;
    private const string bookSaveDataFilePath_ = @"Assets/Resources/Save/bookData.csv";
    private StreamWriter sw_;

    // 書き込み始めに呼ぶ
    public void SaveStart()
    {
        TextAsset saveFile = Resources.Load("Save/bookData") as TextAsset;

        if (saveFile == null)
        {
            // Resourcesフォルダ内のSavaDataフォルダへ新規で作成する
            sw_ = new StreamWriter(bookSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("新規ファイルへ書き込み");
        }
        else
        {
            // 古いデータを削除
            File.Delete(bookSaveDataFilePath_);
            sw_ = new StreamWriter(bookSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("古いデータを削除してファイル書き込み");
            // すでに存在する場合は、上書き保存する(第二引数をfalseにすることで、上書きに切り替えられる)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        // ステータスの項目見出し
        string[] s1 = { "Number","Name", "ReadCheck" };
        string s2 = string.Join(",", s1);
        sw_.WriteLine(s2);
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveBookData(BookStoreMng.BookData set)
    {
        // 実際のステータス値
        string[] data = { set.bookNumber.ToString(), set.bookName, set.readFlag.ToString() };

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


    //public void DataLoad()
    //{
    //    // Debug.Log("ロードします");

    //    csvDatas_.Clear();

    //    // 行分けだけにとどめる
    //    string[] texts = File.ReadAllText(bookSaveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

    //    for (int i = 0; i < texts.Length; i++)
    //    {
    //        // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
    //        csvDatas_.Add(texts[i].Split(','));
    //    }
    //}


}
