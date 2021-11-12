using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    private InitPopList popMateriaList_;

    [SerializeField]
    private RectTransform materiaParent;    // 素材を拾ったときに生成されるプレハブ

    public struct materia
    {
        public GameObject box;  // 素材のオブジェクト
        public Image image;     // 素材の画像
        public Text cntText;    // 持っている素材の個数を表示
        public int haveCnt;     // 指定素材の持ってる個数
        public string name;     // 素材の名前
        public bool getFlag;    // 1つ以上持っているか
    }
    public static materia[] materiaState;

    // 他Scriptで指定するワードは番号を取得しておく
    public static int emptyMateriaNum;// 空のマテリアの番号

    // プレハブから生成されたオブジェクトを代入
    private int maxCnt_ = 0;            // すべての素材数

    private int maxHaveCnt_ = 99;// 指定素材の所持数上限

    public void Init()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

        if (maxCnt_ == 0)
        {
            maxCnt_ = popMateriaList_.SetMaxMateriaCount();
            materiaState = new materia[maxCnt_];
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i] = new materia
                {
                    box = InitPopList.materiaData[i].box,
                    getFlag=false,
                    haveCnt = 0,
                };
                materiaState[i].name = InitPopList.materiaData[i].name;

                materiaState[i].box.transform.SetParent(materiaParent.transform);
                materiaState[i].box.name = materiaState[i].name;

                // 生成したプレハブの子になっているImageを見つける
                materiaState[i].image= materiaState[i].box.transform.Find("MateriaIcon").GetComponent<Image>();
                materiaState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];

                // 生成したプレハブの子になっているTextを見つける
                materiaState[i].cntText = materiaState[i].box.transform.Find("MateriaNum").GetComponent<Text>();
                materiaState[i].cntText.text = materiaState[i].name;

                if(materiaState[i].name == "空のマテリア")
                {
                    emptyMateriaNum = i;
                }

                materiaState[i].box.SetActive(false);// すべて非表示にする
            }
        }
        if (materiaState[0].box.transform.parent != materiaParent.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].box.transform.SetParent(materiaParent.transform);
            }
        }


        // デバッグ用 全部の素材を5個取得した状態で始まる
        for (int i = 0; i < maxCnt_; i++)
        {
            MateriaGetCheck(i, materiaState[i].name, 5);
           // Debug.Log(i + "番目の素材" + materiaBox_[i].name);
        }

    }

    public void MateriaGetCheck(int itemNum, string materiaName, int getCnt)
    {
        materiaState[itemNum].getFlag = true;

        // Debug.Log("     アイテム番号：" + itemNum + "     アイテム名：" + materiaName);
        if (maxHaveCnt_ <= materiaState[itemNum].haveCnt)
        {
            // 最大所持数は99個
            materiaState[itemNum].haveCnt = maxHaveCnt_;
        }
        else
        {
            // 呼ばれた素材番号の素材の所持数を加算
            materiaState[itemNum].haveCnt += getCnt;
        }

        if (materiaState[itemNum].haveCnt < 1)
        {
            // 所持数が1以下なら非表示に
            materiaState[itemNum].box.SetActive(false);
        }
        else
        {
            // 1つでも持っていたら表示する
            materiaState[itemNum].box.SetActive(true);
        }
        materiaState[itemNum].cntText.text = materiaState[itemNum].haveCnt.ToString();
    }

    public int GetMaxHaveMateriaCnt()
    {
        return materiaParent.transform.childCount;
    }
}