using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    // データ系
    private SaveCSV_Materia saveCsvSc_;// SceneMng内にあるセーブ関連スクリプト
    private const string saveDataFilePath_ = @"Assets/Resources/materiaData.csv";
    List<string[]> csvDatas_ = new List<string[]>(); // CSVの中身を入れるリスト;

    private InitPopList popMateriaList_;

    [SerializeField]
    private RectTransform materiaParent;    // 素材を拾ったときに生成されるプレハブ

    public struct MateriaData
    {
        public int number;
        public GameObject box;  // 素材のオブジェクト
        public Image image;     // 素材の画像
        public Text cntText;    // 持っている素材の個数を表示
        public int haveCnt;     // 指定素材の持ってる個数
        public string name;     // 素材の名前
        public string info;
        public bool getFlag;    // 1つ以上持っているか
    }
    public static MateriaData[] materiaState;
    public static MateriaData[] data;

    // 他Scriptで指定するワードは番号を取得しておく
    public static int emptyMateriaNum;// 空のマテリアの番号

    // プレハブから生成されたオブジェクトを代入
    private int maxCnt_ = 0;            // すべての素材数

    private int maxHaveCnt_ = 99;// 指定素材の所持数上限

    private int clickMateriaNum_ = -1;
    private Text info_; // クリックしたアイテムを説明する欄
    private Button throwAwayBtn_;

    private static bool newGameFlag_ = false;
    public void NewGameInit()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Materia>();
        // タイトルでNewGameボタンを押したときに呼ばれるInit
        newGameFlag_ = true;
        maxCnt_ = popMateriaList_.SetMaxMateriaCount();
        data = new MateriaData[maxCnt_];
        for (int i = 0; i < maxCnt_; i++)
        {
            data[i].number = i;
            data[i].name = InitPopList.materiaData[i].name;
            data[i].haveCnt = 0;
        }
        DataSave();
        DataLoad();
    }

    public void Init()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        saveCsvSc_ = GameObject.Find("SceneMng/SaveMng").GetComponent<SaveCSV_Materia>();        

        if (maxCnt_ == 0|| newGameFlag_ ==true)
        {
            maxCnt_ = popMateriaList_.SetMaxMateriaCount();
            materiaState = new MateriaData[maxCnt_];
            data = new MateriaData[maxCnt_];
            newGameFlag_ = false;
            Debug.Log(maxCnt_+"      "+materiaParent.transform);
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i] = new MateriaData
                {
                    box = InitPopList.materiaData[i].box,
                    name = InitPopList.materiaData[i].name,
                    info = InitPopList.materiaData[i].info,
                    getFlag = false,
                    number = int.Parse(csvDatas_[i + 1][0]),
                    haveCnt = int.Parse(csvDatas_[i + 1][2]),
                };
             //   Debug.Log(i + "番目のアイテム" + materiaState[i].name + "の所持数" + materiaState[i].haveCnt);

                materiaState[i].box.transform.SetParent(materiaParent.transform);
                // 名前に番号をつけて素材クリック時に情報を取得しやすくする
                materiaState[i].box.name = materiaState[i].name + i;

                // 生成したプレハブの子になっているImageを見つける
                materiaState[i].image = materiaState[i].box.transform.Find("MateriaIcon").GetComponent<Image>();
                materiaState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];

                // 生成したプレハブの子になっているTextを見つける
                materiaState[i].cntText = materiaState[i].box.transform.Find("NumBack/Num").GetComponent<Text>();
                materiaState[i].cntText.text = materiaState[i].haveCnt.ToString();

                if (materiaState[i].name == "空のマテリア")
                {
                    emptyMateriaNum = i;
                }

                materiaState[i].getFlag = 0 < materiaState[i].haveCnt ? true : false;
                materiaState[i].box.SetActive(materiaState[i].getFlag);
              //  Debug.Log(i + "       " + materiaState[i].getFlag);
            }
        }
        else
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].cntText.text = materiaState[i].haveCnt.ToString();
                materiaState[i].getFlag = 0 < materiaState[i].haveCnt ? true : false;
                materiaState[i].box.SetActive(materiaState[i].getFlag);
            }
        }

        if (materiaState[0].box.transform.parent != materiaParent.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].box.transform.SetParent(materiaParent.transform);
            }
        }
    }

    public void MateriaGetCheck(int materiaNum, int getCnt)
    {
        materiaState[materiaNum].getFlag = true;
        if (getCnt < 0)
        {
            // 個数が減る場合は減るを優先する
            materiaState[materiaNum].haveCnt += getCnt;
            // 所持数が0以下になったら0を入れておく。違うならそのまま
            materiaState[materiaNum].haveCnt = materiaState[materiaNum].haveCnt <= 0 ? 0 : materiaState[materiaNum].haveCnt;
        }
        else
        {
            // 最大所持数は99個                // 呼ばれた素材番号の素材の所持数を加算
            materiaState[materiaNum].haveCnt = maxHaveCnt_ <= materiaState[materiaNum].haveCnt ? maxHaveCnt_ : materiaState[materiaNum].haveCnt + getCnt;
        }
       // Debug.Log(materiaState[materiaNum].haveCnt + "     増減個数：" + getCnt);

        if (materiaState[materiaNum].haveCnt < 1)
        {
            // 所持数が1以下なら非表示に
            materiaState[materiaNum].box.SetActive(false);
        }
        else
        {
            // 1つでも持っていたら表示する
            materiaState[materiaNum].box.SetActive(true);
        }
        materiaState[materiaNum].cntText.text = materiaState[materiaNum].haveCnt.ToString();

        data[materiaNum].haveCnt = materiaState[materiaNum].haveCnt;
    }

    public int GetMaxHaveMateriaCnt()
    {
        return materiaParent.transform.childCount;
    }

    public void SetMateriaNumber(int num)
    {
        // 捨てるボタンを表示
        if (throwAwayBtn_ == null)
        {
            throwAwayBtn_ = GameObject.Find("ItemBagMng/InfoBack/MateriaDelete").GetComponent<Button>();
            info_ = GameObject.Find("ItemBagMng/InfoBack/InfoText").GetComponent<Text>();
        }
        throwAwayBtn_.gameObject.SetActive(true);

        // 選択されたアイテムの番号を保存
        clickMateriaNum_ = num;
    }

    public void OnClickThrowAwayButton()
    {
        if (materiaParent.gameObject.activeSelf == false)
        {
            //materiaState[clickMateriaNum_].box.SetActive(false);
            return;
        }

        // 捨てるボタンを押された場合
        materiaState[clickMateriaNum_].haveCnt--;// 所持数を1減らす
                                                 // 表示中の所持数を更新
        materiaState[clickMateriaNum_].cntText.text = materiaState[clickMateriaNum_].haveCnt.ToString();
        if (materiaState[clickMateriaNum_].haveCnt < 1)
        {
            // 所持数が0になったら非表示にする
            materiaState[clickMateriaNum_].box.SetActive(false);
            throwAwayBtn_.gameObject.SetActive(false);
            info_.text = "";
        }
        data[clickMateriaNum_].haveCnt = materiaState[clickMateriaNum_].haveCnt;
    }

    public void DataLoad()
    {
        // Debug.Log("ロードします");

        csvDatas_.Clear();

        // 行分けだけにとどめる
        string[] texts = File.ReadAllText(saveDataFilePath_).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < texts.Length; i++)
        {
            // カンマ区切りでリストへ登録していく(2次元配列状態になる[行番号][カンマ区切り])
            csvDatas_.Add(texts[i].Split(','));
        }
        Init();
    }

    public void DataSave()
    {
        //  Debug.Log("魔法が生成されました。セーブします");

        saveCsvSc_.SaveStart();

        // 魔法の個数分回す
        for (int i = 0; i < maxCnt_; i++)
        {
            saveCsvSc_.SaveMateriaData(data[i]);
        }
        saveCsvSc_.SaveEnd();
    }

}