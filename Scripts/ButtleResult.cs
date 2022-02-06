using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtleResult : MonoBehaviour
{
    [SerializeField]
    private Canvas resultCanvas;    // 戦闘処理後に表示するリザルト用キャンバス
    [SerializeField]
    private GameObject dropPrefab;    // Drop物を表示する画像
    [SerializeField]
    private Sprite[] CharaImage;// キャラ画像

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;        // フィールド毎の敵情報を保存する
    private List<Enemy> enemyList_ = new List<Enemy>();

    // 経験値関連
    private Slider[] expSlider_ = new Slider[(int)SceneMng.CHARACTERNUM.MAX];    // Exp用のSlider
    private TMPro.TextMeshProUGUI[] expText_ = new TMPro.TextMeshProUGUI[(int)SceneMng.CHARACTERNUM.MAX];  // 現在数値を表示するテキスト
    private TMPro.TextMeshProUGUI[] levelText_ = new TMPro.TextMeshProUGUI[(int)SceneMng.CHARACTERNUM.MAX];  // 現在数値を表示するテキスト
    private static int[] level_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// キャラの現在レベル
    private static int[] maxExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// キャラのレベルに対する上限経験値
    private static int[] nowExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// キャラの経験値
    private static int[] nextExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// 次のレベルまでに必要なEXP
    private int[] oldLevel_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// レベルアップする前のレベル
    private int[] getExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// 獲得EXP
                                                                    //   private int saveSumExp_ = 0;// 獲得EXPの合計
    private int[] saveSumExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// 獲得EXPの合計
    private int[] saveSumMaxExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];

    // 敵のレベル
    private int[] enemyLv_;//
    // ドロップ関連
    private GameObject[] dropObj_;  // プレハブ生成時に使用
    private Image[] dropImage_;     // どの素材を拾ったか
    private Text[] dropCntText_;    // 何個ドロップしたかを表示
    private int enemyCnt_ = 0;// 敵の数＝ドロップ表示数
    private int[] dropCnt_;        // 何個ドロップしたか

    // Chara.csをキャラ毎にリスト化する
    private List<Chara> charasList_ = new List<Chara>();
    public static bool onceFlag_ = false;// ロードしてからの1回しか入らなくていい箇所

    // レベルアップ時のステータス表示関連
    private RectTransform levelMng;    // レベルアップ時に使用するMng
    private Image charaImage_;      // レベルが上がったキャラの表示
    private RectTransform charaRect_;
    private Text levelUpText_;        // どのくらいレベルが上がったか
    private Text nextExpText_;      // 次のレベルまでの必要経験値
    private Text statusText_;       // ステータスの種類
    private Text statusNumText_;    // 加算されたステータスの値（変化しないものも含む）

    // バッグ
    private Bag_Materia bagMateria_;

    void Start()
    {
        resultCanvas.gameObject.SetActive(false);
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        // StreamingAssetsからAssetBundleをロードする
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle開く");
        // AssetBundle内のアセットにはビルド時のアセットのパス、またはファイル名、ファイル名＋拡張子でアクセスできる
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // 不要になったAssetBundleのメタ情報をアンロードする
        assetBundle.Unload(false);
        Debug.Log("破棄");

        Debug.Log("resultCanvas" + resultCanvas.transform.Find("UniIconFrame/EXPSlider").GetComponent<Slider>());
        expSlider_[(int)SceneMng.CHARACTERNUM.UNI] = resultCanvas.transform.Find("UniIconFrame/EXPSlider").GetComponent<Slider>();
        expSlider_[(int)SceneMng.CHARACTERNUM.JACK] = resultCanvas.transform.Find("JackIconFrame/EXPSlider").GetComponent<Slider>();

        charasList_ = SceneMng.charasList_;

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            if (onceFlag_ == false)
            {
                level_[i] = charasList_[i].Level();
                maxExp_[i] = charasList_[i].CharacterMaxExp();
                nowExp_[i] = charasList_[i].CharacterExp();
            }

            // レベル関連
            oldLevel_[i] = level_[i];
            levelText_[i] = expSlider_[i].transform.Find("LvText").GetComponent<TMPro.TextMeshProUGUI>();
            levelText_[i].text = "Lv " + level_[i].ToString();

            // 経験値関連
            expText_[i] = expSlider_[i].transform.Find("AddExpText").GetComponent<TMPro.TextMeshProUGUI>();
            expSlider_[i].maxValue = maxExp_[i];
            expSlider_[i].value = nowExp_[i];
            Debug.Log(i + "   " + level_[i] + "レベル" + charasList_[i].Level());
            Debug.Log(i + "   " + charasList_[i].CharacterExp() + "経験値" + charasList_[i].CharacterMaxExp());
        }
        onceFlag_ = true;

        // レベルアップ時のステータス表示関連
        levelMng = resultCanvas.transform.Find("LvUpMng").GetComponent<RectTransform>();
        levelUpText_ = levelMng.Find("LevelBackImage/LevelUpText").GetComponent<Text>();
        nextExpText_ = levelMng.Find("NextLevelBackImage/NextLevelText").GetComponent<Text>();
        statusText_ = levelMng.Find("StatusUpBack/StatusNameText").GetComponent<Text>();
        statusNumText_ = levelMng.Find("StatusUpBack/StatusNumText").GetComponent<Text>();
        charaImage_ = levelMng.Find("CharaImage").GetComponent<Image>();
        charaRect_ = charaImage_.GetComponent<RectTransform>();
        levelMng.gameObject.SetActive(false);

        // 素材取得用
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();
    }

    public void DropCheck(int enemyCnt, int[] num, bool bossFlag, List<GameObject> list)
    {
        Debug.Log("リザルトを表示します");
        enemyCnt_ = enemyCnt;

        // 素材系
        if (enemyData_ == null)
        {
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, (int)SceneMng.nowScene - (int)SceneMng.SCENE.FIELD0, name);
        }

        int[] materiaNum = new int[enemyCnt_];
        enemyLv_ = new int[enemyCnt_];
        Debug.Log(enemyCnt_);
        for (int i = 0; i < enemyCnt_; i++)
        {
            Debug.Log(enemyCnt + "体中 " + num[i] + "体目");
            enemyList_.Add(new Enemy(num[i].ToString(), 1, null, enemyData_.param[num[i]]));
            Debug.Log(i + "番目：" + enemyList_[i].GetExp());
            Debug.Log(i + "番目：" + enemyList_[i].DropMateria());
            // Drop物の番号を確認する
            materiaNum[i] = int.Parse(Regex.Replace(enemyList_[i].DropMateria(), @"[^0-9]", ""));
            enemyLv_[i] = enemyList_[i].Level();  //saveSumMaxExp_[i] = 10;
            for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
            {
                // レベル差による経験値量チェック
                saveSumExp_[c] += ExpCheck(c, i);
            }

            // 討伐クエストの討伐数確認
            for (int k = 0; k < list.Count; k++)
            {
                // 名前部分のアンダーバーで分ける
                var name = enemyList_[i].Name().Split('_');

                // 討伐対象の敵を倒したのか確認する
                if (list[k].GetComponent<CompleteQuest>().GetEnemyName() == name[0])
                {
                    list[k].GetComponent<CompleteQuest>().SetFinSubjugation(1);
                }
            }
        }

        resultCanvas.gameObject.SetActive(true);
        // 表示する親の位置を確定
        Transform dropParent = resultCanvas.transform.Find("DropMng/Viewport/DropParent").GetComponent<Transform>();

        dropCnt_ = new int[enemyCnt_];
        dropObj_ = new GameObject[enemyCnt_];
        dropImage_ = new Image[enemyCnt_];
        dropCntText_ = new Text[enemyCnt_];

        for (int i = 0; i < enemyCnt; i++)
        {
            if (bossFlag == true)
            {
                if(SceneMng.SCENE.FIELD4==SceneMng.nowScene)
                {
                    // 各アイテムのドロップ数を1～5のランダムで取得
                    dropCnt_[i] = Random.Range(1, 5);

                    // ドロップ物を表示するためにプレハブを生成
                    dropObj_[i] = Instantiate(dropPrefab,
                        new Vector2(0, 0), Quaternion.identity, dropParent);
                    dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                    dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                    dropCntText_[i].text = "×" + dropCnt_[i];
                    dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum[i]];
                    bagMateria_.MateriaGetCheck(materiaNum[i], dropCnt_[i]);

                }
                else
                {
                    // ボスだった場合Dropがワードになるため取得ワードと番号を得る
                    var nameCheck = enemyList_[i].DropMateria().Split('_');
                    string wordName = nameCheck[0];// どのワードか
                    materiaNum[i] = int.Parse(nameCheck[1]);// 画像番号（エレメント）

                    dropObj_[i] = Instantiate(dropPrefab,
                        new Vector2(0, 0), Quaternion.identity, dropParent);
                    dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                    dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                    dropCntText_[i].text = wordName;
                    dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][materiaNum[i]];
                }
            }
            else
            {
                // 各アイテムのドロップ数を1～5のランダムで取得
                dropCnt_[i] = Random.Range(1, 5);

                // ドロップ物を表示するためにプレハブを生成
                dropObj_[i] = Instantiate(dropPrefab,
                    new Vector2(0, 0), Quaternion.identity, dropParent);
                dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                dropCntText_[i].text = "×" + dropCnt_[i];
                dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum[i]];
                bagMateria_.MateriaGetCheck(materiaNum[i], dropCnt_[i]);
            }
        }

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            // 経験値のスライダーを動かす
            StartCoroutine(ActiveExpSlider(i, charasList_[i].GetDeathFlg(), saveSumExp_[i]));
        }
    }



    private int ExpCheck(int charaNum, int enemyNum)
    {
        int getExp = 0;
        int LvCheck = level_[charaNum] - enemyLv_[enemyNum];
        Debug.Log("レベル差" + LvCheck);
        if (LvCheck <= 0)
        {
            // キャラのレベルのほうが低い場合 そのままの経験値を渡す
            getExp = enemyList_[enemyNum].GetExp();
        }
        else
        {
            getExp = enemyList_[enemyNum].GetExp() - (LvCheck * 5);
            if (getExp < 5)
            {
                // 取得Expが5以下の時は5にする
                getExp = 5;
            }
        }
        return getExp;
    }

    private IEnumerator ActiveExpSlider(int charaNum, bool deathFlag, int sumExp)
    {
        float saveValue = 0;
        // バトルで死亡したまま終了していたときは,獲得経験値を半分に
        int nowExp = deathFlag == true ? sumExp / 2 : sumExp;
        getExp_[charaNum] = nowExp;
        expText_[charaNum].text = "+" + nowExp.ToString();
        Debug.Log(deathFlag + "死亡確認     獲得EXP" + nowExp);
        int sumMaxExp = (int)expSlider_[charaNum].maxValue;
        bool onceFlag = true;
        while (true)
        {
            yield return null;
            if (getExp_[charaNum] <= saveValue)
            {
                if (onceFlag == true)
                {
                    // 加算分だけスライダーを移動させたら移動を終了させる
                    Debug.Log(saveValue + "       スライダーの移動が終了しました");
                    nextExp_[charaNum] = (int)(expSlider_[charaNum].maxValue - expSlider_[charaNum].value);

                    if (expSlider_[charaNum].value == expSlider_[charaNum].maxValue)
                    {
                        // もし同値で終わってしまった場合
                        level_[charaNum]++;
                        levelText_[charaNum].text = "Lv " + level_[charaNum].ToString();
                        // 上限を変更
                        expSlider_[charaNum].maxValue = (int)(expSlider_[charaNum].maxValue * 1.1f);
                        Debug.Log(charaNum + "    " + level_[charaNum] + "  上限" + expSlider_[charaNum].maxValue);
                        saveSumMaxExp_[charaNum] += (int)expSlider_[charaNum].maxValue;
                        // valueが上限まで来たら0に戻す
                        expSlider_[charaNum].value = 0.0f;
                        nextExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                    }
                    onceFlag = false;
                }

                if (charaNum == (int)SceneMng.CHARACTERNUM.UNI)
                {
                    maxExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                    nowExp_[charaNum] = (int)expSlider_[charaNum].value;
                    yield break;
                }
                else
                {
                    // ジャックのスライダー移動まで終わったらレベルが上がったかのチェックをする
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        maxExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                        nowExp_[charaNum] = (int)expSlider_[charaNum].value;
                        StartCoroutine(ActiveResult());
                        yield break;
                    }
                }
            }
            else
            {
                if (expSlider_[charaNum].maxValue <= expSlider_[charaNum].value)
                {
                    // 該当キャラのレベルを上げる
                    level_[charaNum]++;
                    levelText_[charaNum].text = "Lv " + level_[charaNum].ToString();
                    nowExp -= (int)expSlider_[charaNum].maxValue;
                    sumMaxExp += (int)expSlider_[charaNum].maxValue;
                    // 上限を変更
                    expSlider_[charaNum].maxValue = (int)(expSlider_[charaNum].maxValue * 1.1f);
                    Debug.Log(charaNum + "    " + level_[charaNum] + "  上限" + expSlider_[charaNum].maxValue);
                    saveSumMaxExp_[charaNum] += (int)expSlider_[charaNum].maxValue;
                    // valueが上限まで来たら0に戻す
                    expSlider_[charaNum].value = 0.0f;
                }
                else
                {
                    // 上限に来るまで加算する
                    saveValue += 1;//
                    expSlider_[charaNum].value += 1;
                    // Debug.Log(expSlider_[charaNum].value + "  上限");
                }
            }
        }
    }

    private IEnumerator ActiveResult()
    {
        // ジャックの経験値スライダーまで終わったら処理をする
        // 0 Uni    1 Jack
        bool[] levelUpFlag = new bool[(int)SceneMng.CHARACTERNUM.MAX] { false, false };
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            Debug.Log(i + "番目のキャラのレベル遷移：" + oldLevel_[i] + "→" + level_[i]);
            levelUpFlag[i] = oldLevel_[i] < level_[i] ? true : false;
            saveSumExp_[i] = 0;
        }

        // 誰のレベルも上がってなかったら何もしない
        if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == false
         && levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == false)
        {
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            levelMng.gameObject.SetActive(false);
            resultCanvas.gameObject.SetActive(false);
            for (int i = 0; i < enemyCnt_; i++)
            {
                // リザルト非表示にDropオブジェクト削除
                Destroy(dropObj_[i]);
            }
            enemyCnt_ = 0;
            enemyList_.Clear();
            yield break;
        }

        while (true)
        {
            yield return null;
            if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == false
             && levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == false)
            {
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                levelMng.gameObject.SetActive(false);
                resultCanvas.gameObject.SetActive(false);
                enemyCnt_ = 0;
                for (int i = 0; i < enemyCnt_; i++)
                {
                    // リザルト非表示にDropオブジェクト削除
                    Destroy(dropObj_[i]);
                }
                enemyList_.Clear();
                yield break;
            }

            if (levelMng.gameObject.activeSelf == false)
            {
                levelMng.gameObject.SetActive(true);
            }

            // ユニのレベルアップ用の画像を表示する
            if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == true)
            {
                if (oldLevel_[(int)SceneMng.CHARACTERNUM.UNI] != level_[(int)SceneMng.CHARACTERNUM.UNI])
                {
                    LevelRelation(new Vector2(-350.0f, -105.0f), SceneMng.CHARACTERNUM.UNI,
                    oldLevel_[(int)SceneMng.CHARACTERNUM.UNI],
                    level_[(int)SceneMng.CHARACTERNUM.UNI],
                    nextExp_[(int)SceneMng.CHARACTERNUM.UNI],
                    getExp_[(int)SceneMng.CHARACTERNUM.UNI]);
                }
                // 左ボタン押下かスペースキー押下でレベルアップ用の画像を表示
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    // 確認し終わったらfalseにする
                    levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] = false;
                }
            }
            else
            {
                if (levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == true)
                {
                    // ユニがレベルアップして表示している可能性があるためチェックする
                    if (oldLevel_[(int)SceneMng.CHARACTERNUM.JACK] != level_[(int)SceneMng.CHARACTERNUM.JACK])
                    {
                        LevelRelation(new Vector2(-350.0f, -240.0f),
                            SceneMng.CHARACTERNUM.JACK,
                        oldLevel_[(int)SceneMng.CHARACTERNUM.JACK],
                        level_[(int)SceneMng.CHARACTERNUM.JACK],
                        nextExp_[(int)SceneMng.CHARACTERNUM.JACK],
                          getExp_[(int)SceneMng.CHARACTERNUM.JACK]);
                    }
                    // 左ボタン押下かスペースキー押下でレベルアップ用の画像を表示
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] = false;
                    }
                }
            }
        }
    }

    private void LevelRelation(Vector2 pos, SceneMng.CHARACTERNUM chara,
        int oldLevel, int nowLevel, int nextExp, int exp)
    {
        SceneMng.SetSE(15);

        int differenceLv = nowLevel - oldLevel;
        // 上昇させる分を入れる
        int[] tmp = new int[10] { 0, 0, 0, 0, 0, 0, 0, differenceLv, maxExp_[(int)chara]-nextExp, maxExp_[(int)chara] };
        // 0Attack 1MagicAttack 2Defence 3Speed 4Luck 5HP 6MP 7Level 8exp 9maxExp
        // ユニ、ジャック共通部分
        if ((nowLevel % 3 == 0) || (3 <= differenceLv))
        {
            tmp[3] = 2;            // 3レベル毎にスピードを上げる
        }
        if ((nowLevel % 5 == 0) || (5 <= differenceLv))
        {
            tmp[4] = 1;            // 5レベル毎に幸運を上げる
        }

        // 複数レベル上がった場合
        for (int i = oldLevel; i < nowLevel; i++)
        {
            // 初期値差があるためレベルアップで上がる値は同じにする
            if (i % 2 == 1)
            {
                tmp[2] += 2;// 2Defence
            }
            else
            {
                tmp[0] += 2;// 0Attack 
                tmp[1] += 2;// 1MagicAttack
            }
        }
        // 1レベル上昇するたびに加算
        tmp[5] = differenceLv * 5;// 5HP
        tmp[6] = differenceLv * 2;// 6MP

        charaRect_.sizeDelta = new Vector2(CharaImage[(int)chara].rect.width, CharaImage[(int)chara].rect.height);
        charaImage_.sprite = CharaImage[(int)chara];
        charaRect_.localPosition = pos;

        levelUpText_.text = "Lv　" + oldLevel.ToString() + "→" + nowLevel.ToString();
        nextExpText_.text = "次のレベルまで　" + nextExp.ToString() + "EXP";
        statusText_.text = "HP\nMP\nAttack\nMagicAttack\nDefence\nSpeed\nLuck";
        statusNumText_.text = "+" + tmp[5].ToString() +
                            "\n+" + tmp[6].ToString() +
                            "\n+" + tmp[0].ToString() +
                            "\n+" + tmp[1].ToString() +
                            "\n+" + tmp[2].ToString() +
                            "\n+" + tmp[3].ToString() +
                            "\n+" + tmp[4].ToString();

        Debug.Log(chara + " stetas" + tmp);
        // ステータスを上げる
        SceneMng.charasList_[(int)chara].LevelUp(tmp);

        oldLevel_[(int)chara] = level_[(int)chara];
    }
}