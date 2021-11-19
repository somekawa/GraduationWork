using System.IO;
using System.Text;
using UnityEngine;

public class SaveCSV : MonoBehaviour
{
    private const string saveDataFilePath_ = @"Assets/Resources/data.csv";
    private StreamWriter sw;

    // 書き込み始めに呼ぶ
    public void SaveStart()
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
        // ステータスの項目見出し
        string[] s1 = { "Name", "Level", "HP", "MP", "Attack", "MagicAttack",
            "Defence", "Speed", "Luck", "AnimMax","Magic0" ,"Magic1","Magic2" ,"Magic3" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveData(CharaBase.CharacterSetting set)
    {
        // 実際のステータス値
        string[] data = { set.name, set.Level.ToString(),set.HP.ToString(),set.MP.ToString(),
                          set.Attack.ToString(),set.MagicAttack.ToString(),set.Defence.ToString(),
                          set.Speed.ToString(),set.Luck.ToString(),set.AnimMax.ToString(),
                          set.Magic0.ToString(),set.Magic1.ToString(),
                          set.Magic2.ToString(),set.Magic3.ToString()};
        string write = string.Join(",", data);
        sw.WriteLine(write);
    }

    // ファイルを閉じるときに呼ぶ
    public void SaveEnd()
    {
        Debug.Log("書き込みファイルを閉じた");
        sw.Close();
    }
}