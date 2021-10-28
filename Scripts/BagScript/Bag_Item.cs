using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Item : MonoBehaviour
{
    private enum EVENT_ITEM
    {
        NON=-1,
        FREE_MATERIA,
        BEGINNER_RECIPE,
        MAX
    }
    private string[] itemName= new string[30];

    [SerializeField]
    private GameObject itemUIPrefab;
    [SerializeField]
    private RectTransform itemParent_;    // 素材を拾ったときに生成されるプレハブ

    private static GameObject[] itemBox_ = new GameObject[30];
    private Image[] instanceImages_ = new Image[30];
    private Text[] instanceTexts_ = new Text[30];
    //private int instanceNum_ = 0;
    private static int itemNum_ = 0;// 何番目の生成なのか
    private static int[] itemCnt_ = new int[30];// 1つの素材に対するの所持数
    private static string[] saveItemName_ = new string[(int)EVENT_ITEM.MAX];    // 拾った素材の名前を保存
    private static int[] saveItemNum_ = new int[30];    // 何番目を拾ったか保存
    private static int[] saveChapterNum_ = new int[(int)EVENT_ITEM.MAX];
    private static int chapterCnt_ = 0;
    // 表示する画像のX座標
    private static float[] boxPosX_ = new float[5] {
        -285.0f,-95.0f,100.0f,290.0f,490.0f
    };
    private static float[] boxPosY_ = new float[2] {
        150.0f,-50.0f
    };
    private static int xCount_ = 0;// X座標をずらすためのカウント
    private static int yCount_ = 0;// Y座標をずらすためのカウント

    void Start()
    {
        //menuActive_ = menuActive;

        //  itemBox_ = transform.Find("ItemBox").GetComponent<Image>();
       // gameObject.SetActive(false);

        // StartCoroutine(ActiveItem(menuActive));
    }

    public IEnumerator ActiveItem(ItemBagMng itemBagMng)
    {
        gameObject.SetActive(true);
        Debug.Log("Item表示中です");
        while (true)
        {
            yield return null;
            if (itemBagMng.GetStringNumber() != (int)ItemBagMng.topic.ITEM)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    // public void ItemGetCheck(int chapterNum)
    public void ItemGetCheck(string itemName)
    {
        if (saveItemName_[0] == null)
        {
            saveItemName_[0] = itemName;
            //saveItemNum_[0] = itemNum;
            itemCnt_[0]++;// 所持数を加算

            Debug.Log(saveItemName_[0] + "を作りました");
            // 画像を生成　(元になるprefab、座標、回転、親)
            itemBox_[0] = Instantiate(itemUIPrefab,
                new Vector2(0, 0), Quaternion.identity,
                itemParent_);

            // 生成したプレハブの子になっているImageを見つける
            instanceImages_[0] = itemBox_[0].transform.Find("ItemIcon").GetComponent<Image>();
            instanceImages_[0].sprite =  ItemImageMng.spriteMap_[ItemImageMng.IMAGE.ITEM][0, 1];

            // 生成したプレハブの子になっているTextを見つける
            instanceTexts_[0] = itemBox_[0].transform.Find("ItemNum").GetComponent<Text>();
            instanceTexts_[0].text = itemCnt_[0].ToString();

            // 名前を設定
            itemBox_[itemNum_].GetComponent<OwnedMateria>().SetMyName(itemName);
            // picture_.GetMateriakinds(fieldNum, itemNum);
        }



        //for (int chapterCnt_ = 0; chapterCnt_ < (int)ItemBagMng.topic.MAX; chapterCnt_++)
        //{
        //    if (saveChapterNum_[chapterCnt_] < EventMng.GetChapterNum())
        //    {
        //        itemName[chapterCnt_] = saveItemName_[chapterCnt_];
        //        //saveItemNum_[0] = itemNum;
        //        itemCnt_[chapterCnt_] = 5;
        //        // 画像を生成　(元になるprefab、座標、回転、親)
        //        itemBox_[chapterCnt_] = Instantiate(itemUIPrefab,
        //            new Vector2(0, 0), Quaternion.identity, this.transform.Find("Viewport/Content"));
        //        // 表示位置をずらす
        //        itemBox_[chapterCnt_].transform.localPosition = new Vector2(boxPosX_[chapterCnt_], boxPosY_[0]);

        //        // 生成したプレハブの子になっているImageを見つける
        //        instanceImages_[chapterCnt_] = itemBox_[chapterCnt_].transform.Find("ItemIcon").GetComponent<Image>();
        //        instanceImages_[chapterCnt_].sprite = ItemImageMng.itemIcon_[1, 0];

        //        // 生成したプレハブの子になっているTextを見つける
        //        instanceTexts_[chapterCnt_] = itemBox_[chapterCnt_].transform.Find("ItemNum").GetComponent<Text>();
        //        instanceTexts_[chapterCnt_].text = itemCnt_[chapterCnt_].ToString();
        //       // chapterCnt_++;
        //    }
        //}



        //if (saveChapterNum_[chapterCnt_] < EventMng.GetChapterNum())
        //{
        //    itemName[0] = saveItemName_[0];
        //    //saveItemNum_[0] = itemNum;
        //    itemCnt_[0] = 5;
        //    // 画像を生成　(元になるprefab、座標、回転、親)
        //    itemBox_[0] = Instantiate(itemUIPrefab,
        //        new Vector2(0, 0), Quaternion.identity, this.transform);
        //    // 表示位置をずらす
        //    itemBox_[0].transform.localPosition = new Vector2(boxPosX_[1], boxPosY_[0]);

        //    // 生成したプレハブの子になっているImageを見つける
        //    instanceImages_[0] = itemBox_[0].transform.Find("ItemIcon").GetComponent<Image>();
        //    instanceImages_[0].sprite = ItemImageMng.materialIcon_[1, 0];

        //    // 生成したプレハブの子になっているTextを見つける
        //    instanceTexts_[0] = itemBox_[0].transform.Find("ItemNum").GetComponent<Text>();
        //    instanceTexts_[0].text = itemCnt_[0].ToString();
        //}
        // chapterCnt_++;

    }

    public void SetItemKinds(ItemList list)
    {
        // 素材名のリストを取得
        if (list != null)
        {
            for (int i = 0; i < (int)EVENT_ITEM.MAX; i++)
            {
                // 現在フィールドの素材名を保存
                saveItemName_[i] = list.param[i].ItemName;
                saveChapterNum_[i] = list.param[i].ChapterNumber;
            }
        }        
    }
}
