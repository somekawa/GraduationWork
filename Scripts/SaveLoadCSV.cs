using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveLoadCSV : MonoBehaviour
{
    [SerializeField]
    // クエストを受注したときに生成されるプレハブ
    private GameObject completePrefab;

    // セーブ内容
    public enum SAVEDATA
    {
        CHARACTER,
        OTHER,
        BOOK,
        MAX
    }

    private const string saveDataFilePath_ = @"Assets/Resources/data.csv";
    private const string otherSaveDataFilePath_ = @"Assets/Resources/otherData.csv";
    private const string bookFilePath_ = @"Assets/Resources/Save/bookData.csv";

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
            string[] s1 = { "Name", "Level", "HP","MaxHP", "MP","MaxMP", "Attack", "MagicAttack",
            "Defence", "Speed", "Luck", "AnimMax","Magic0" ,"Magic1","Magic2" ,"Magic3","Exp","MaxExp","SumExp" };
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
        else if(saveData == SAVEDATA.BOOK)
        {
            TextAsset saveFile = Resources.Load("Save/bookData") as TextAsset;

            if (saveFile == null)
            {
                // Resourcesフォルダ内へ新規で作成する
                sw = new StreamWriter(bookFilePath_, true, Encoding.UTF8);
                Debug.Log("新規ファイルへ書き込み");
            }
            else
            {
                // 古いデータを削除
                File.Delete(bookFilePath_);
                sw = new StreamWriter(bookFilePath_, true, Encoding.UTF8);
                Debug.Log("古いデータを削除してファイル書き込み");
            }

            // ステータスの項目見出し
            string[] s1 = { "Name", "ReadCheck" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
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
        string[] data = { set.name, set.Level.ToString(),set.HP.ToString(),set.maxHP.ToString(),set.MP.ToString(),set.maxMP.ToString(),
                          set.Attack.ToString(),set.MagicAttack.ToString(),set.Defence.ToString(),
                          set.Speed.ToString(),set.Luck.ToString(),set.AnimMax.ToString(),
                          set.Magic[0].ToString(),set.Magic[1].ToString(),
                          set.Magic[2].ToString(),set.Magic[3].ToString(),
                          set.CharacterExp.ToString(),set.CharacterMaxExp.ToString(),set.CharacterSumExp.ToString()};
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
        var orderQuest = QuestClearCheck.GetOrderQuestsList();
        var clearQuest = QuestClearCheck.GetClearedQuestsList();
        bool tmpFlg = false;
        foreach (var pair in quest)
        {
            // 受注中で、クリアしてない状態のもの→回数の末尾に"_"をつけるようにする。
            for (int i = 0; i < orderQuest.Count; i++)
            {
                if(orderQuest[i].Item1.name == pair.Key.ToString())
                {
                    if(QuestMng.rewardGradeUp.ContainsKey(pair.Key))
                    {
                        sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString() + "_," + QuestMng.rewardGradeUp[pair.Key]));
                    }
                    else
                    {
                        sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString() + "_," + 0.0));
                    }
                    tmpFlg = true;
                    continue;
                }
            }

            // 受注中で、クリアしたが報告していないもの→回数の末尾に"_c"をつけるようにする。
            for (int i = 0; i < clearQuest.Count; i++)
            {
                if (clearQuest[i].name == pair.Key.ToString())
                {
                    if (QuestMng.rewardGradeUp.ContainsKey(pair.Key))
                    {
                        sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString() + "_c," + QuestMng.rewardGradeUp[pair.Key]));
                    }
                    else
                    {
                        sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString() + "_c," + 0.0));
                    }
                    tmpFlg = true;
                    continue;
                }
            }

            if(!tmpFlg)
            {
                sw.WriteLine(string.Join("\n", pair.Key.ToString() + "," + pair.Value.ToString()));
            }
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

    // NewGameの際にbookData.csvの初期化をする
    public void NewGameInit()
    {
        var DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        List<string> bookname_ = new List<string>();
        int cnt = 0;

        for(int i = 0; i < 5; i++)  // .xlsのページ数分まわす
        {
            var tmp = DataPopPrefab_.GetComponent<PopList>().GetData<BookList>(PopList.ListData.BOOK_STORE, i);
            for (int b = 0; b < tmp.param.Count; b++)   // 列分まわす
            {
                bookname_.Add(tmp.param[b].BookName);
                cnt++;
            }
        }

        // まだ本データが存在しないとき、名前とフラグだけいれる
        if (BookStoreMng.bookState_ == null)
        {
            BookStoreMng.bookState_ = new BookStoreMng.BookData[bookname_.Count];

            for (int i = 0; i < bookname_.Count; i++)
            {
                BookStoreMng.bookState_[i].name = bookname_[i];
                BookStoreMng.bookState_[i].readFlag = 0;
            }
        }

        //SaveStart(SAVEDATA.BOOK);
        //for (int i = 0; i < bookname_.Count; i++)
        //{
        //    string[] data = { bookname_[i], 0.ToString() };
        //    string write = string.Join(",", data);
        //    sw.WriteLine(write);
        //}
        //SaveEnd();
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
                int[] tmpArray = { int.Parse(csvDatas[i + 1][12]),
                                int.Parse(csvDatas[i + 1][13]),
                                int.Parse(csvDatas[i + 1][14]),
                                int.Parse(csvDatas[i + 1][15]) };

                CharaBase.CharacterSetting set = new CharaBase.CharacterSetting
                {
                    name = csvDatas[i + 1][0],
                    Level = int.Parse(csvDatas[i + 1][1]),
                    HP = int.Parse(csvDatas[i + 1][2]),
                    maxHP = int.Parse(csvDatas[i + 1][3]),
                    MP = int.Parse(csvDatas[i + 1][4]),
                    maxMP = int.Parse(csvDatas[i + 1][5]),
                    Attack = int.Parse(csvDatas[i + 1][6]),
                    MagicAttack = int.Parse(csvDatas[i + 1][7]),
                    Defence = int.Parse(csvDatas[i + 1][8]),
                    Speed = int.Parse(csvDatas[i + 1][9]),
                    Luck = int.Parse(csvDatas[i + 1][10]),
                    AnimMax = float.Parse(csvDatas[i + 1][11]),
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
                    EventMng.SetChapterNum(int.Parse(texts[i + 1]),SceneMng.SCENE.NON,true);
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

                        if(split[1].Contains("_c") || split[1].Contains("_"))
                        {
                            // 受注中クエストの共通処理
                            // プレハブのインスタンス
                            var prefab = Instantiate(completePrefab);
                            // クエスト番号の設定
                            prefab.GetComponent<CompleteQuest>().SetMyNum(int.Parse(split[0]));
                            // クエストクリアを確認するスクリプトのリストに登録する
                            QuestClearCheck.SetOrderQuestsList(prefab);

                            // 指定したキーが存在するかどうか
                            if (!QuestMng.rewardGradeUp.ContainsKey(int.Parse(split[0])))
                            {
                                QuestMng.rewardGradeUp.Add(int.Parse(split[0]), 0.0f);    
                            }

                            // 受注中で、クリアしたが報告していないもの→回数の末尾に"_c"をつけるようにする。
                            if (split[1].Contains("_c"))
                            {
                                QuestClearCheck.QuestClear(int.Parse(split[0]));
                            }
                        }

                        bool tmp = false;
                        // すでにデータがある場合、
                        for (int a = 0; a < QuestMng.questClearCnt.Count; a++)
                        {
                            // 名前が一致したところを書き換える
                            if (a == int.Parse(split[0]))
                            {
                                QuestMng.questClearCnt[a] = int.Parse(System.Text.RegularExpressions.Regex.Replace(split[1], @"[^0-9]", ""));
                                tmp = true;
                                break;
                            }
                        }

                        // まだデータが存在しない場合、
                        if (!tmp)
                        {
                            QuestMng.questClearCnt.Add(int.Parse(split[0]), int.Parse(System.Text.RegularExpressions.Regex.Replace(split[1], @"[^0-9]", "")));
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