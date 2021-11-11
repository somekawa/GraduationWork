using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;

public class Bag_Magic : MonoBehaviour
{
    [SerializeField]
    private SaveCSV_Magic saveCsvSc_;    // 素材を拾ったときに生成されるプレハブ

    // データ系
    //private SaveCSV_Magic saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/magicData.csv";
    List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト;

    public struct MagicData
    {
        public string name;
        public int power;
        public int rate;
        public int ability;
    }
    public static MagicData[] data_ = new MagicData[50];

    [SerializeField]
    private GameObject magicUIPrefab;     // 素材を拾ったときに生成されるプレハブ

    [SerializeField]
    private RectTransform magicParent_;

    public static GameObject[] magicObject = new GameObject[50];
    private Text[] magicName_ = new Text[50];
    private int number_ = 0;


    public void Init()
    // void Start()
    {
        //saveCsvSc_ = magicCreateMng_.transform.GetComponent<SaveCSV_Magic>();
        magicObject = new GameObject[50];
        magicName_ = new Text[50];

        if (magicObject[0] == null)
        {
            if (0 < number_)
            {
                Debug.Log(number_);
                for (int i = 0; i < number_; i++)
                {
                    magicObject[i] = Instantiate(magicUIPrefab,
                             new Vector2(0, 0), Quaternion.identity, magicParent_.transform);
                    magicName_[i] = magicObject[i].transform.Find("Word").GetComponent<Text>();
                    magicName_[i].text = csvDatas[i + 1][0];
                    data_[i].name = csvDatas[i + 1][0];
                    data_[i].power = int.Parse(csvDatas[i + 1][1]);
                    data_[i].ability = int.Parse(csvDatas[i + 1][1]);
                    data_[i].rate = int.Parse(csvDatas[i + 1][1]);
                    Debug.Log(data_[i].name + "            残り" + i);
                }
            }
        }
    }

    public void MagicCreateCheck(string magic, int pow, int rateNum, int abilityNum)
    {
        magicObject[number_] = Instantiate(magicUIPrefab,
              new Vector2(0, 0), Quaternion.identity, magicParent_.transform);
        magicName_[number_] = magicObject[number_].transform.Find("Word").GetComponent<Text>();
        magicName_[number_].text = magic;
        data_[number_].name = magic;
        data_[number_].power = pow;
        data_[number_].rate = rateNum;
        data_[number_].ability = abilityNum;
        Debug.Log(data_[number_].name + "  " + data_[number_].power + "  " + data_[number_].rate + "  " + data_[number_].ability);
        number_++;
        DataSave();

    }

    public int MagicNumber()
    {
        return number_;
    }

    public void DataLoad()
    {
        Debug.Log("ロードします");

        csvDatas.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas.Add(texts[i].Split(','));
        }
        Debug.Log("データ数" + csvDatas.Count);
        number_ = csvDatas.Count - 1;


        // キャラクター数分のfor文を回す
        // 魔法の個数分回す
        for (int i = 1; i < number_; i++)
        {
            MagicData set = new MagicData
            {
                name = csvDatas[i + 1][0],// name自体はこのScriptをアタッチ指定オブジェクト名が入ってる
                power = int.Parse(csvDatas[i + 1][1]),
                ability = int.Parse(csvDatas[i + 1][1]),
                rate = int.Parse(csvDatas[i + 1][1]),
            };
        }
        Init();
    }

    private void DataSave()
    {
        Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < number_; i++)
        {
            saveCsvSc_.SaveMagicData(data_[i]);
            Debug.Log(data_[i].name);
        }
        saveCsvSc_.SaveEnd();
    }


}