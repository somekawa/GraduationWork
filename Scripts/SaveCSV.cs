using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV : MonoBehaviour
{
    private const string saveDataFilePath_ = @"Assets/Resources/data.csv";
    private StreamWriter sw;

    void Start()
    {
    }

    // この部分をキャラのステータスとか引数でいれたらいいよね
    //public void SaveData(string txt1, string txt2, string txt3)
    //{
    //    string[] s1 = { txt1, txt2, txt3 };
    //    string s2 = string.Join(",", s1);
    //    sw.WriteLine(s2);
    //}

    public void SaveData(CharaBase.CharacterSetting set)
    {
        TextAsset saveFile = Resources.Load("data") as TextAsset;

        if (saveFile == null)
        {
            // Resourcesフォルダ内のSavaDataフォルダへ新規で作成する
            sw = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("新規ファイルへ書き込み");
        }
        else
        {
            // 古いデータを削除
            File.Delete(saveDataFilePath_);
            sw = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("古いデータを削除してファイル書き込み");
            // すでに存在する場合は、上書き保存する(第二引数をfalseにすることで、上書きに切り替えられる)
            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
        }

        //string[] s1 = { "F", "J", "time" };
        string[] s1 = { "Name", "Level" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);

        string[] data = { set.name, set.Level.ToString() };
        string write = string.Join(",", data);
        sw.WriteLine(write);

        Debug.Log("書き込みファイルを閉じた");
        sw.Close();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        TextAsset saveFile = Resources.Load("data") as TextAsset;

    //        if (saveFile == null)
    //        {
    //            // Resourcesフォルダ内のSavaDataフォルダへ新規で作成する
    //            sw = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
    //            Debug.Log("新規ファイルへ書き込み");
    //        }
    //        else
    //        {
    //            // 古いデータを削除
    //            File.Delete(saveDataFilePath_);
    //            sw = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
    //            Debug.Log("古いデータを削除してファイル書き込み");
    //            // すでに存在する場合は、上書き保存する(第二引数をfalseにすることで、上書きに切り替えられる)
    //            //sw = new StreamWriter(saveDataFilePath_, false, Encoding.GetEncoding("Shift_JIS"));
    //        }

    //        //string[] s1 = { "F", "J", "time" };
    //        string[] s1 = { "Name","Level" };
    //        string s2 = string.Join(",", s1);
    //        sw.WriteLine(s2);
    //    }

    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        Debug.Log("書き込みファイルを閉じた");
    //        sw.Close();
    //    }

    //}
}
