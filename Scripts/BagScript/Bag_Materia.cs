using System.Collections;
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

    // プレハブから生成されたオブジェクトを代入
    private static GameObject[] materiaBox_ = new GameObject[30];
    private static Image[] instanceImages_ = new Image[30];
    private static Text[] instanceTexts_ = new Text[30];

    public static int materiaNum_ = 0;// 何番目の生成なのか
    private static int[] materiaCnt_ = new int[30];// 1つの素材に対するの所持数
    public static string[] saveMateriaName_ = new string[30];    // 拾った素材の名前を保存
    private static int[] saveMateriaNum_ = new int[30];    // 何番目を拾ったか保存

    //// 表示する画像のX座標
    //private static float[] boxPosX_ = new float[5] {
    //    -285.0f,-95.0f,100.0f,290.0f,490.0f
    //};
    //private static float[] boxPosY_ = new float[2] {
    //    150.0f,-50.0f
    //};
    //private static int xCount_ = 0;// X座標をずらすためのカウント
    //private static int yCount_ = 0;// Y座標をずらすためのカウント

  //  private PictureBookCheck picture_;

    // void Start()
    public void Init()
    {
        //
        //menuActive_ = menuActive;
        // GameObject.Find("DontDestroyCanvas/OtherUI/PictureBookMng").GetComponent<PictureBookCheck>().Init();
       // picture_ = GameObject.Find("SceneMng").GetComponent<PictureBookCheck>();

        //gameObject.SetActive(false);

    }

    public IEnumerator ActiveMateria(ItemBagMng itemBagMng)
    {
        gameObject.SetActive(true);
        Debug.Log("Materia表示中です");
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

            if (itemBagMng.GetStringNumber() != (int)ItemBagMng.topic.MATERIA)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public void MateriaGetCheck(int fieldNum, int itemNum, string materiaName)
    {
        // デバッグ用
        saveMateriaName_[0] = materiaName;
        saveMateriaNum_[0] = itemNum;
        materiaCnt_[0] = 5;// 所持数を加算
        materiaBox_[0] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // 生成したプレハブの子になっているImageを見つける
        instanceImages_[0] = materiaBox_[0].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[0].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 5];
        // 生成したプレハブの子になっているTextを見つける
        instanceTexts_[0] = materiaBox_[0].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[0].text = materiaCnt_[0].ToString();
        // 名前を設定
        materiaBox_[0].GetComponent<OwnedMateria>().SetMyName(materiaName);

        materiaNum_++;
        
        saveMateriaName_[materiaNum_] = "酢の橘";
        saveMateriaNum_[materiaNum_] = itemNum;
        materiaCnt_[materiaNum_] = 1;// 所持数を加算
        materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // 生成したプレハブの子になっているImageを見つける
        instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[materiaNum_].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 0];
        // 生成したプレハブの子になっているTextを見つける
        instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();
        // 名前を設定
        materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);

        materiaNum_++;

        saveMateriaName_[materiaNum_] = "花の蜜";
        saveMateriaNum_[materiaNum_] = itemNum;
        materiaCnt_[materiaNum_] = 1;// 所持数を加算
        materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // 生成したプレハブの子になっているImageを見つける
        instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[materiaNum_].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 1];
        // 生成したプレハブの子になっているTextを見つける
        instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();
        // 名前を設定
        materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);

        //-----------------------


        //if (saveMateriaName_[0] == null)
        //{

        //    saveMateriaName_[0] = materiaName;
        //    saveMateriaNum_[0] = itemNum;
        //    materiaCnt_[0]=5;// 所持数を加算

        //    Debug.Log(materiaNum_ + "つ目の素材を拾いました");
        //    // 画像を生成　(元になるprefab、座標、回転、親)
        //    materiaBox_[0] = Instantiate(materiaUIPrefab,
        //        new Vector2(0, 0), Quaternion.identity, 
        //        GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);


        //    // 表示位置をずらす
        //   // materiaBox_[0].transform.localPosition = new Vector2(boxPosX_[0], boxPosY_[0]);

        //    // 生成したプレハブの子になっているImageを見つける
        //    instanceImages_[0] = materiaBox_[0].transform.Find("MateriaIcon").GetComponent<Image>();
        //    // instanceImages_[0].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];
        //    instanceImages_[0].sprite = ItemImageMng.materialIcon_[0, 5];

        //    // 生成したプレハブの子になっているTextを見つける
        //    instanceTexts_[0] = materiaBox_[0].transform.Find("MateriaNum").GetComponent<Text>();
        //    instanceTexts_[0].text = materiaCnt_[0].ToString();

        //    // 名前を設定
        //    materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
        //   // picture_.GetMateriakinds(fieldNum, itemNum);
        //}
        //else
        //{
        //    if (materiaName == saveMateriaName_[0])
        //    {
        //        materiaCnt_[0]++;
        //        instanceTexts_[0].text = materiaCnt_[0].ToString();
        //    }
        //    else
        //    {
        //        // 取得した素材が０番以外だったら
        //        bool flag = false;
        //        for (int i = 0; i < 25; i++)
        //        {
        //            if (saveMateriaName_[i] == materiaName)
        //            {
        //                // 同じ名前の素材を拾ったら所持数を加算する
        //                materiaCnt_[i]++;// 所持数加算
        //                instanceTexts_[i].text = materiaCnt_[i].ToString();
        //                flag = true;
        //                break;
        //            }
        //        }

        //        if (flag == false)
        //        {
        //            materiaNum_++;  // アイテムの所持最大種類をふやす
        //            saveMateriaName_[materiaNum_] = materiaName;
        //            saveMateriaNum_[materiaNum_] = itemNum;

        //            //xCount_++;
        //            //if (xCount_ == 5)
        //            //{
        //            //    xCount_ = 0;
        //            //    yCount_++;
        //            //}
        //            Debug.Log(materiaNum_ + "つ目の素材を拾いました");
        //            // 画像を生成　(元になるprefab、座標、回転、親)
        //            materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
        //                new Vector2(0, 0), Quaternion.identity, 
        //                GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        //            // 表示位置をずらす
        //           // materiaBox_[materiaNum_].transform.localPosition = new Vector2(boxPosX_[xCount_], boxPosY_[yCount_]);

        //            // 生成したプレハブの子になっているImageを見つける
        //            instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        //            instanceImages_[materiaNum_].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];

        //            // 生成したプレハブの子になっているTextを見つける
        //            instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        //            materiaCnt_[materiaNum_] = 1;   // 所持数
        //            instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();

        //            //  var prefab = Instantiate(materiaUIPrefab);
        //            // クエスト番号の設定
        //            materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
        //            //picture_.GetMateriakinds(fieldNum, itemNum);
        //        }
        //    }
        //}

        //  Debug.Log("フィールド番号：" + fieldNum + "     アイテム番号：" + itemNum + "     アイテム名：" + materiaName);
    }
}