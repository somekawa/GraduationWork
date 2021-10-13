using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MenuMng : MonoBehaviour
{
    private TMPro.TextMeshProUGUI statusInfo_;  // キャラのステータス描画先
    private GameObject buttons_;                // ボタン類をまとめたオブジェクト


    void Start()
    {
        statusInfo_ = this.transform.Find("StatusInfo").GetComponent<TMPro.TextMeshProUGUI>();
        buttons_ = this.transform.Find("Buttons").gameObject;
    }

    public void OnClickStatus()
    {
        Debug.Log("ステータス確認ボタンが押された");
        buttons_.SetActive(false);

        // キャラのステータス値を表示させたい
        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // 表示する文字の作成
        string str = "名前  :"   + data.name               + "\n" +
                     "レベル:"   + data.Level.ToString()   + "\n" +
                     "HP    :"   + data.HP.ToString()      + "\n" +
                     "MP    :"   + data.MP.ToString()      + "\n" +
                     "体    :"   + data.Constitution.ToString() + "\n" +
                     "精神  :"   + data.Power.ToString()   + "\n" +
                     "攻撃力:"   + data.Attack.ToString()  + "\n" +
                     "防御力:"   + data.Defence.ToString() + "\n" +
                     "素早さ:"   + data.Speed.ToString()   + "\n" +
                     "幸運  :"   + data.Luck.ToString();

        // 作成した文字を入れる
        statusInfo_.text = str;
    }

    public void OnClickSave()
    {
        Debug.Log("セーブボタンが押された");

        var data = SceneMng.GetCharasSettings((int)SceneMng.CHARACTERNUM.UNI);

        // データ書き出しテスト
        StreamWriter swLEyeLog;
        FileInfo fiLEyeLog;

        // 保存位置
        fiLEyeLog = new FileInfo(Application.dataPath + "/saveData.csv");

        swLEyeLog = fiLEyeLog.AppendText();

        // 書き込み内容の作成
        string str = data.name + "," +
                     data.Level.ToString()   + "," +
                     data.HP.ToString()      + "," +
                     data.MP.ToString()      + "," +
                     data.Constitution.ToString() + "," +
                     data.Power.ToString()   + "," +
                     data.Attack.ToString()  + "," +
                     data.Defence.ToString() + "," +
                     data.Speed.ToString()   + "," +
                     data.Luck.ToString();

        swLEyeLog.Write(str);   // 書き込み
        swLEyeLog.Flush();
        swLEyeLog.Close();
    }

    public void OnClickCancel()
    {
        Debug.Log("キャンセルボタンが押された");

        if(buttons_.activeSelf)
        {
            // フィールドに戻す
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
        }
        else
        {
            // ボタン選択に戻す
            buttons_.SetActive(true);
            statusInfo_.text = "";
        }
    }
}
