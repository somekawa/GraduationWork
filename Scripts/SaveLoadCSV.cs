using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveLoadCSV : MonoBehaviour
{
    // セーブ内容
    public enum SAVEDATA
    {
        CHARACTER,
        OTHER,
        MAX
    }

    private const string saveDataFilePath_ = @"Assets/Resources/data.csv";
    private const string otherSaveDataFilePath_ = @"Assets/Resources/otherData.csv";
    private StreamWriter sw;
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    // 書き込み始めに呼ぶ
    public void SaveStart(SAVEDATA saveData)
    {
        if(saveData == SAVEDATA.CHARACTER)
        {
            TextAsset saveFile = Resources.Load("data") as TextAsset;

            if (saveFile == null)
            {
                // Resourcesフォルダ内へ新規で作成する
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

            // ステータスの項目見出し
            string[] s1 = { "Name", "Level", "HP", "MP", "Attack", "MagicAttack",
            "Defence", "Speed", "Luck", "AnimMax","Magic0" ,"Magic1","Magic2" ,"Magic3" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
        else if(saveData == SAVEDATA.OTHER)
        {
            TextAsset saveFile = Resources.Load("otherData") as TextAsset;

            if (saveFile == null)
            {
                // Resourcesフォルダ内へ新規で作成する
                sw = new StreamWriter(otherSaveDataFilePath_, true, Encoding.UTF8);
                Debug.Log("新規ファイルへ書き込み");
            }
            else
            {
                // 古いデータを削除
                File.Delete(otherSaveDataFilePath_);
                sw = new StreamWriter(otherSaveDataFilePath_, true, Encoding.UTF8);
                Debug.Log("古いデータを削除してファイル書き込み");
            }
        }
        else
        {
            // 何も処理を行わない
        }
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveData(CharaBase.CharacterSetting set)
    {
        // 実際のステータス値
        string[] data = { set.name, set.Level.ToString(),set.HP.ToString(),set.MP.ToString(),
                          set.Attack.ToString(),set.MagicAttack.ToString(),set.Defence.ToString(),
                          set.Speed.ToString(),set.Luck.ToString(),set.AnimMax.ToString(),
                          set.Magic[0].ToString(),set.Magic[1].ToString(),
                          set.Magic[2].ToString(),set.Magic[3].ToString()};
        string write = string.Join(",", data);
        sw.WriteLine(write);
    }

    // 「進行度,所持金,クエストのクリア状態,宝箱と強制戦闘壁の取得とクリア状態」を保存する
    public void OtherSaveData()
    {
        // 項目見出し
        //string[] s1 = { "EventNum", "Money", "Quest", "ForcedButtleAndTreasure" };
        //string s2 = string.Join("\n", s1);
        //sw.WriteLine(s2);

        // Quest項目名までをとりあえず保存させる
        string[] data = { "EventNum",EventMng.GetChapterNum().ToString(), "Money",SceneMng.GetHaveMoney().ToString(),"Quest"};
        string write = string.Join("\n", data);
        sw.WriteLine(write);

        // クエスト
        var quest = QuestMng.questClearCnt;
        foreach (var pair in quest)
        {
            sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString()));
        }

        // 宝箱と強制戦闘壁の取得とクリア状態の項目名
        sw.WriteLine(string.Join("\n", "Treasure"));

        // 宝箱と強制戦闘壁の取得とクリア状態
        var treasure = FieldMng.treasureList;
        foreach (var pair in treasure)
        {
            sw.WriteLine(string.Join("\n", pair.Item1.ToString() + "," + pair.Item2.ToString()));
        }

        sw.WriteLine(string.Join("\n", "ForcedButtle"));

        var wall = FieldMng.forcedButtleWallList;
        foreach (var pair in wall)
        {
            sw.WriteLine(string.Join("\n", pair.Item1.ToString() + "," + pair.Item2.ToString()));
        }
    }

    // ファイルを閉じるときに呼ぶ
    public void SaveEnd()
    {
        Debug.Log("書き込みファイルを閉じた");
        sw.Close();
    }

    public void LoadData(SAVEDATA saveData)
    {
        csvDatas.Clear();

        if (saveData == SAVEDATA.CHARACTER)
        {
            // 行分けだけにとどめる
            string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < texts.Length; i++)
            {
                // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
                csvDatas.Add(texts[i].Split(','));
            }

            Debug.Log("データ数" + csvDatas.Count);

            // キャラクター数分のfor文を回す
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                //// 一時変数に入れてからじゃないとsetに入れられない
                int[] tmpArray = { int.Parse(csvDatas[i + 1][10]),
                                int.Parse(csvDatas[i + 1][11]),
                                int.Parse(csvDatas[i + 1][12]),
                                int.Parse(csvDatas[i + 1][13]) };

                CharaBase.CharacterSetting set = new CharaBase.CharacterSetting
                {
                    name = csvDatas[i + 1][0],
                    Level = int.Parse(csvDatas[i + 1][1]),
                    HP = int.Parse(csvDatas[i + 1][2]),
                    MP = int.Parse(csvDatas[i + 1][3]),
                    Attack = int.Parse(csvDatas[i + 1][4]),
                    MagicAttack = int.Parse(csvDatas[i + 1][5]),
                    Defence = int.Parse(csvDatas[i + 1][6]),
                    Speed = int.Parse(csvDatas[i + 1][7]),
                    Luck = int.Parse(csvDatas[i + 1][8]),
                    AnimMax = float.Parse(csvDatas[i + 1][9]),
                    Magic = tmpArray,
                };
                Debug.Log(csvDatas[i + 1][0] + "            キャラデータをロード中。残り" + i);
                SceneMng.SetCharasSettings(i, set);
            }
        }
        else if(saveData == SAVEDATA.OTHER)
        {
            // 途中から/rが入ってて判断できなくなってる
            // 行分けだけにとどめる
            string[] texts = File.ReadAllText(otherSaveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < texts.Length; i++)
            {
                if(texts[i] == "EventNum" || texts[i] == "EventNum\r")
                {
                    EventMng.chapterNum = int.Parse(texts[i + 1]);
                }
                else if(texts[i] == "Money" || texts[i] == "Money\r")
                {
                    SceneMng.SetHaveMoney(int.Parse(texts[i + 1]));
                }
                else if(texts[i] == "Quest" || texts[i] == "Quest\r")
                {
                    // クエスト処理
                    // 次の項目の見出しが出るまで、代入する
                    for (int k = i + 1; k < texts.Length; k++)
                    {
                        // 次の項目が現れたらfor文を抜ける
                        if (texts[k] == "Treasure" || texts[k] == "Treasure\r")
                        {
                            break;
                        }

                        // FieldMngにあるリストに追加していく
                        var split = texts[k].Split(',');

                        bool tmp = false;
                        // すでにデータがある場合、
                        for (int a = 0; a < QuestMng.questClearCnt.Count; a++)
                        {
                            // 名前が一致したところを書き換える
                            if (a == int.Parse(split[0]))
                            {
                                QuestMng.questClearCnt[a] = int.Parse(split[1]);
                                tmp = true;
                                break;
                            }
                        }

                        // まだデータが存在しない場合、
                        if (!tmp)
                        {
                            QuestMng.questClearCnt.Add(int.Parse(split[0]), int.Parse(split[1]));
                        }
                    }
                }
                else if(texts[i] == "Treasure" || texts[i] == "Treasure\r")
                {
                    CommonLoad(i + 1, texts, FieldMng.treasureList, "ForcedButtle");
                }
                else if (texts[i] == "ForcedButtle" || texts[i] == "ForcedButtle\r")
                {
                    CommonLoad(i + 1, texts, FieldMng.forcedButtleWallList,"");
                }
                else
                {
                    // 何も処理を行わない
                }
            }
        }
        else
        {
            // 何も処理を行わない
        }
    }

    // 同じリストを使っている宝箱と壁の処理は共通なのでまとめられる
    private void CommonLoad(int num,string[] texts,List<(string,bool)> list,string breakword)
    {
        // 次の項目の見出しが出るまで、代入する
        for (int k = num; k < texts.Length; k++)
        {
            // 次の項目が現れたらfor文を抜ける
            if (texts[k] == breakword || texts[k] == breakword + "\r")
            {
                break;
            }

            // FieldMngにあるリストに追加していく
            var split = texts[k].Split(',');
            bool flag;
            if (split[1] == "False" || split[1] == "False\r") // フラグはstringからboolにできないので、文字列で判断する
            {
                flag = false;
            }
            else
            {
                flag = true;
            }

            bool tmp = false;
            // すでにデータがある場合、
            for (int a = 0; a < list.Count; a++)
            {
                // 名前が一致したところを書き換える
                if (list[a].Item1 == split[0])
                {
                    list[a] = (split[0], flag);
                    tmp = true;
                    break;
                }
            }

            // まだデータが存在しない場合、
            if (!tmp)
            {
                list.Add((split[0], flag));
            }
        }
    }
}