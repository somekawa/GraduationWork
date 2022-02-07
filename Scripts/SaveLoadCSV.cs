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

    private string saveDataFilePath_;
    private string otherSaveDataFilePath_;
    private string bookFilePath_;

    private StreamWriter sw;
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;
    List<string[]> bookCsvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;

    private int[] tmpStatus = new int[8];

    // 書き込み始めに呼ぶ
    public void SaveStart(SAVEDATA saveData)
    {
        saveDataFilePath_ = Application.streamingAssetsPath + "/data.csv";
        otherSaveDataFilePath_ = Application.streamingAssetsPath + "/otherData.csv";
        bookFilePath_ = Application.streamingAssetsPath + "/Save/bookData.csv";

        if (saveData == SAVEDATA.CHARACTER)
        {
            File.Delete(saveDataFilePath_);
            sw = new StreamWriter(saveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("CharacterData,古いデータを削除してファイル書き込み");

            // ステータスの項目見出し
            string[] s1 = { "Name", "Level", "HP","MaxHP", "MP","MaxMP", "Attack", "MagicAttack",
            "Defence", "Speed", "Luck", "AnimMax","Magic0" ,"Magic1","Magic2" ,"Magic3","Exp","MaxExp" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
        else if(saveData == SAVEDATA.OTHER)
        {
            File.Delete(otherSaveDataFilePath_);
            sw = new StreamWriter(otherSaveDataFilePath_, true, Encoding.UTF8);
            Debug.Log("OtherData,古いデータを削除してファイル書き込み");
        }
        else if(saveData == SAVEDATA.BOOK)
        {
            File.Delete(bookFilePath_);
            sw = new StreamWriter(bookFilePath_, true, Encoding.UTF8);
            Debug.Log("Book,古いデータを削除してファイル書き込み");

            // ステータスの項目見出し
            string[] s1 = { "Number", "Name", "ReadCheck" };
            string s2 = string.Join(",", s1);
            sw.WriteLine(s2);
        }
        else
        {
            // 何も処理を行わない
        }
    }

    // キャラのステータスを引数でいれて書き込みをする
    public void SaveData(CharaBase.CharacterSetting set,int num)
    {
        // 実際のステータス値
        string[] data = { set.name, set.Level.ToString(),set.HP.ToString(),set.maxHP.ToString(),set.MP.ToString(),set.maxMP.ToString(),
                          set.Attack.ToString(),set.MagicAttack.ToString(),set.Defence.ToString(),
                          set.Speed.ToString(),set.Luck.ToString(),set.AnimMax.ToString(),
                          set.Magic[0].ToString(),set.Magic[1].ToString(),
                          set.Magic[2].ToString(),set.Magic[3].ToString(),
                          set.CharacterExp.ToString(),set.CharacterMaxExp.ToString()};
        Debug.Log(set.name + set.Level.ToString() + set.HP.ToString() + set.maxHP.ToString() + set.MP.ToString() + set.maxMP.ToString());
        Debug.Log(set.CharacterExp.ToString() + "     " + set.CharacterMaxExp.ToString());
        string write = string.Join(",", data);
        sw.WriteLine(write);

        // 消した上昇分を戻す
        SceneMng.charasList_[num].SetStatusUpByCook(SceneMng.charasList_[num].GetStatusUpByCook(false));
    }

    // 「進行度,所持金,クエストのクリア状態,宝箱と強制戦闘壁の取得とクリア状態」を保存する
    public void OtherSaveData()
    {
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

        // 料理効果時間のセーブ
        sw.WriteLine(string.Join("\n", "StatusUpFinTime"));
        if (SceneMng.GetFinStatusUpTime().Item2)
        {
            sw.WriteLine(string.Join("\n", (int)SceneMng.GetFinStatusUpTime().Item1));
        }

        // 時間帯のセーブ
        sw.WriteLine(string.Join("\n", "TimeGear"));
        sw.WriteLine(string.Join("\n", SceneMng.GetTimeGear()));

        // 料理のステータス上昇値
        sw.WriteLine(string.Join("\n", "StatusUpChara"));
        sw.WriteLine(string.Join("\n", SceneMng.charasList_[0].GetStatusUpByCook(true)));

        SceneMng.charasList_[0].DeleteStatusUpByCook(true);
        SceneMng.charasList_[1].DeleteStatusUpByCook(true);
    }

    // NewGameの際にbookData.csvの初期化をする
    public void NewGameInit()
    {
        // その他データのセーブ
        SaveStart(SAVEDATA.OTHER);
        OtherSaveData();
        SaveEnd();

        // キャラステータス
        SaveStart(SAVEDATA.CHARACTER);
        // キャラクター数分のfor文を回す
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            SaveData(SceneMng.GetCharasSettings(i),i);
        }
        SaveEnd();

        // Book
        //var DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        // StreamingAssetsからAssetBundleをロードする
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle開く");
        // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        var DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // 不要になったAssetBundleのメタ情報をアンロードする
        assetBundle.Unload(false);
        Debug.Log("破棄");

        List<string> bookname_ = new List<string>();
        int cnt = 0;

        for (int i = 0; i < 5; i++)  // .xlsのページ数分まわす
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
                BookStoreMng.bookState_[i].bookNumber = i;
                BookStoreMng.bookState_[i].bookName = bookname_[i];
                BookStoreMng.bookState_[i].readFlag = 0;
            }
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
        saveDataFilePath_ = Application.streamingAssetsPath + "/data.csv";
        otherSaveDataFilePath_ = Application.streamingAssetsPath + "/otherData.csv";
        bookFilePath_ = Application.streamingAssetsPath + "/Save/bookData.csv";

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
                // 一時変数に入れてからじゃないとsetに入れられない
                int[] tmpArray = { int.Parse(csvDatas[i + 1][12]),
                                int.Parse(csvDatas[i + 1][13]),
                                int.Parse(csvDatas[i + 1][14]),
                                int.Parse(csvDatas[i + 1][15]) };

                CharaBase.CharacterSetting set = new CharaBase.CharacterSetting
                {
                    name = csvDatas[i + 1][0],
                    Level = int.Parse(csvDatas[i + 1][1]),
                    HP = int.Parse(csvDatas[i + 1][2]) + tmpStatus[5],
                    maxHP = int.Parse(csvDatas[i + 1][3]) + tmpStatus[5],
                    MP = int.Parse(csvDatas[i + 1][4]) + tmpStatus[6],
                    maxMP = int.Parse(csvDatas[i + 1][5]) + tmpStatus[6],
                    Attack = int.Parse(csvDatas[i + 1][6]) + tmpStatus[0],
                    MagicAttack = int.Parse(csvDatas[i + 1][7]) + tmpStatus[1],
                    Defence = int.Parse(csvDatas[i + 1][8]) + tmpStatus[2],
                    Speed = int.Parse(csvDatas[i + 1][9]) + tmpStatus[3],
                    Luck = int.Parse(csvDatas[i + 1][10]) + tmpStatus[4],
                    AnimMax = float.Parse(csvDatas[i + 1][11]),
                    Magic = tmpArray,
                    CharacterExp = int.Parse(csvDatas[i + 1][16]),
                   CharacterMaxExp = int.Parse(csvDatas[i + 1][17])
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
                    CommonLoad(i + 1, texts, FieldMng.forcedButtleWallList, "StatusUpFinTime");
                }
                else if (texts[i] == "StatusUpFinTime" || texts[i] == "StatusUpFinTime\r")
                {
                    if (texts[i+1] != "TimeGear" && texts[i+1] != "TimeGear\r")
                    {
                        SceneMng.SetFinStatusUpTime(int.Parse(texts[i + 1]), true);
                    }
                }
                else if(texts[i] == "TimeGear" || texts[i] == "TimeGear\r")
                {
                    string a = texts[i+1];

                    if(a.Contains("\r"))
                    {
                        a = a.Substring(0, a.IndexOf("\r"));
                    }

                    var tmp = (SceneMng.TIMEGEAR)Enum.Parse(typeof(SceneMng.TIMEGEAR), a);
                    SceneMng.SetTimeGear(tmp);
                }
                else if (texts[i] == "StatusUpChara" || texts[i] == "StatusUpChara\r")
                {
                    int[] tmp = { int.Parse(texts[i + 1]), int.Parse(texts[i + 2]), int.Parse(texts[i + 3]), int.Parse(texts[i + 4]), int.Parse(texts[i + 5]), int.Parse(texts[i + 6]), int.Parse(texts[i + 7]), 0 };
                    tmpStatus = tmp;    // キャラデータ確定まで一時保存
                }
                else
                {
                    // 何も処理を行わない
                }
            }
        }
        else
        {
            bookCsvDatas_.Clear();

            // 行分けだけにとどめる
            string[] texts = File.ReadAllText(bookFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < texts.Length; i++)
            {
                // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
                bookCsvDatas_.Add(texts[i].Split(','));
            }
            Debug.Log("レシピ関連");

            if (BookStoreMng.bookState_ == null)
            {
                BookStoreMng.bookState_ = new BookStoreMng.BookData[texts.Length - 1];  // 項目の行を-1する
            }

            // レシピ本があるタイミングで表示する本を進行に合わせて区切る
            for (int i = 0; i < 32; i++)
            {
                // イベント進行具合のMaxだと進行途中でロードするときにエラーする可能性があるため
                // 一旦すべて読み込んでおく
                BookStoreMng.bookState_[i].bookNumber = i;
                BookStoreMng.bookState_[i].bookName = bookCsvDatas_[i + 1][1];
                BookStoreMng.bookState_[i].readFlag = int.Parse(bookCsvDatas_[i + 1][2]);
            }
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

    public void SaveBookData(BookStoreMng.BookData set)
    {
        // 実際のステータス値
        string[] data = { set.bookNumber.ToString(), set.bookName, set.readFlag.ToString() };

        // set.sprite.ToString() 
        string write = string.Join(",", data);
        sw.WriteLine(write);
    }

    public int[] StatusNum()
    {
        return tmpStatus;
    }
}