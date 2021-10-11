using System.Collections;
using System.Collections.Generic;
//using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    [SerializeField]
    private GameObject materiaUIPrefab;    // 素材を拾ったときに生成されるプレハブ

    //[SerializeField]
    //// Materiaを取得した際に生成されるprefab
    //private GameObject getMateria;
    //[SerializeField]
    //// Materiaを取得した際に生成されるprefab
    //private GameObject materiaCanvas;

    //[SerializeField]
    //private GameObject canvasPrefab;    // 素材を拾ったときに生成されるプレハブ
    //private Transform tPrefab_;    // 素材を拾ったときに生成されるプレハブ
    //private GameObject gPrefab_;    // 素材を拾ったときに生成されるプレハブ

    // プレハブから生成されたオブジェクトを代入
    public static GameObject[] materiaBox_ = new GameObject[30];
    public static Image[] instanceImages_ = new Image[30];
    public static Text[] instanceTexts_ = new Text[30];

    public static int materiaNum_ = 0;// 何番目の生成なのか
    public static int[] materiaCnt_ = new int[30];// 1つの素材に対するの所持数
    public static string[] saveMateriaName_ = new string[30];    // 拾った素材の名前を保存
    public static int[] saveMateriaNum_ = new int[30];    // 何番目を拾ったか保存

    // 表示する画像のX座標
    public static float[] boxPosX_ = new float[5] {
        -285.0f,-95.0f,100.0f,290.0f,490.0f
    };
    public static float[] boxPosY_ = new float[2] {
        150.0f,-50.0f
    };
    public static int xCount_ = 0;// X座標をずらすためのカウント
    public static int yCount_ = 0;// Y座標をずらすためのカウント

    //struct InstanceUIs
    //{
    //    public Image activeImage; // どの背景画像か
    //    public Text numText;       // どのテキストであるか
    //    public int materiaNumber_;
    //    public string materiaName_;
    //    public int materiaCnt_;
    //    //public bool checkFlag;      // 指定の行動をとったかどうか
    //    //public bool activeFlag;     // 指示を終わらせたかどうか
    //}
    //private InstanceUIs[] status_;
   
    //void Awake()
    //{
    //    DontDestroyOnLoad(gameObject);
    //    Debug.Log(gameObject + "はシーンをまたいでオブジェクトを残します");
    //}

    void Start()
    // public void Init(MenuActive menuActive)
    {

        //if (gPrefab_ == null)
        //{
        //    gPrefab_ = Instantiate(canvasPrefab);
        //    tPrefab_ = gPrefab_.transform;
        //}
        //menuActive_ = menuActive;

        //gameObject.SetActive(false);

        // StartCoroutine(ActiveMateria(menuActive));
        //itemkinds_ = new int[(int)SceneMng.SCENE.MAX, (int)ItemGet.items.MAX];

        //for(int i=0;i<materiaNum_;i++)
        //{
        //    status_[i] = new InstanceUIs()
        //    {

        //    };
        //}
    }

    public IEnumerator ActiveMateria(MenuActive menuActive)
    {
        gameObject.SetActive(true);
        while (true)
        {
            yield return null;

            //// debug
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    ItemGetCheck(0, 0, "ぜろばんめ");
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    ItemGetCheck(0, 1, "いちばんめ");
            //}
            //else if (Input.GetKeyDown(KeyCode.D))
            //{
            //    ItemGetCheck(0, 2, "にばんめ");
            //}
            ////

            if (menuActive.GetStringNumber() != (int)MenuActive.topic.MATERIA)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public void ItemGetCheck(int fieldNum, int itemNum, string materiaName)
    {
        if (saveMateriaName_[0] == null)
        {
            saveMateriaName_[0] = materiaName;
            saveMateriaNum_[0] = itemNum;
            materiaCnt_[0]++;// 所持数を加算

            Debug.Log(materiaNum_ + "つ目の素材を拾いました");
            // 画像を生成　(元になるprefab、座標、回転、親)
            materiaBox_[0] = Instantiate(materiaUIPrefab,
                new Vector2(0, 0), Quaternion.identity, this.transform);
            // 表示位置をずらす
            materiaBox_[0].transform.localPosition = new Vector2(boxPosX_[0], boxPosY_[0]);

            // 生成したプレハブの子になっているImageを見つける
            instanceImages_[0] = materiaBox_[0].transform.Find("MateriaIcon").GetComponent<Image>();
            instanceImages_[0].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];

            // 生成したプレハブの子になっているTextを見つける
            instanceTexts_[0] = materiaBox_[0].transform.Find("MateriaNum").GetComponent<Text>();
            instanceTexts_[0].text = materiaCnt_[0].ToString();

           // var prefab = Instantiate(materiaUIPrefab);
            // クエスト番号の設定
            materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
        }
        else
        {
            if (materiaName == saveMateriaName_[0])
            {
                materiaCnt_[0]++;
                instanceTexts_[0].text = materiaCnt_[0].ToString();
            }
            else
            {
                // 取得した素材が０番以外だったら
                bool flag = false;
                for (int i = 0; i < 25; i++)
                {
                    if (saveMateriaName_[i] == materiaName)
                    {
                        // 同じ名前の素材を拾ったら所持数を加算する
                        materiaCnt_[i]++;// 所持数加算
                        instanceTexts_[i].text = materiaCnt_[i].ToString();
                        flag = true;
                        break;
                    }
                }

                if (flag == false)
                {
                    materiaNum_++;  // アイテムの所持最大種類をふやす
                    saveMateriaName_[materiaNum_] = materiaName;
                    saveMateriaNum_[materiaNum_] = itemNum;

                    xCount_++;
                    if (xCount_ == 5)
                    {
                        xCount_ = 0;
                        yCount_++;
                    }
                    Debug.Log(materiaNum_ + "つ目の素材を拾いました");
                    // 画像を生成　(元になるprefab、座標、回転、親)
                    materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
                        new Vector2(0, 0), Quaternion.identity, this.transform);
                    // 表示位置をずらす
                    materiaBox_[materiaNum_].transform.localPosition = new Vector2(boxPosX_[xCount_], boxPosY_[yCount_]);

                    // 生成したプレハブの子になっているImageを見つける
                    instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
                    instanceImages_[materiaNum_].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];

                    // 生成したプレハブの子になっているTextを見つける
                    instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
                    materiaCnt_[materiaNum_] = 1;   // 所持数
                    instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();

                  //  var prefab = Instantiate(materiaUIPrefab);
                    // クエスト番号の設定
                    materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
                }
            }
        }
        Debug.Log("フィールド番号：" + fieldNum + "     アイテム番号：" + itemNum + "     アイテム名：" + materiaName);
        //Debug.Log("表示する画像" + materiaNum_ + "つめ");
        // Debug.Log("取得したアイテム名：" + materiaName);
    }

    public static int GetMateriaDate()
    {
        return materiaNum_;
    }
    public static Image GetMateriaImage()
    {
        return instanceImages_[0];
    }
    public static Text GetMateriaText()
    {
        return instanceTexts_[0];
    }
}