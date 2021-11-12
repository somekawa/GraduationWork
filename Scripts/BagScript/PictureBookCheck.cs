using UnityEngine;
using UnityEngine.UI;

public class PictureBookCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject pictureUI;    // 素材を拾ったときに生成されるプレハブ

    //private Text pageName_;
    //private string[] nameString_ = new string[(int)ItemGet.items.MAX]{
    //    "Field_0","Field_0","Field_0","Field_0","Field_0" };

    private GameObject[] activePicture_;
    public static Image[] instanceImages_ = new Image[30];
    private float[] posX_;

    void Start()
    //    public void Init()
    {
        activePicture_ = new GameObject[(int)DropFieldMateria.items.MAX];
        posX_ = new float[(int)DropFieldMateria.items.MAX] {
        -400.0f,-200.0f,0.0f,200.0f,400.0f };
       //gameObject.SetActive(false);

    }

    //public void GetMateriakinds(int fieldNum, int itemNum)
    //{
    //    activePicture_[itemNum] = Instantiate(pictureUI,
    //                    new Vector2(0, 0), 
    //                    Quaternion.identity,
    //                    transform.Find("DontDestroyCanvas/OtherUI/PictureBookMng").GetComponent<RectTransform>());
    //    // 表示位置をずらす
    //    activePicture_[itemNum].transform.localPosition = new Vector2(posX_[itemNum], 0.0f);
    //    Debug.Log("フィールド番号：" + fieldNum + "     アイテム番号：" + itemNum);
    //    // 生成したプレハブの子になっているImageを見つける
    //    instanceImages_[itemNum] = activePicture_[itemNum].GetComponent<Image>();
    //    instanceImages_[itemNum].sprite =  ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][fieldNum* itemNum + itemNum];

    //    // var prefab = Instantiate(materiaUIPrefab);
    //    // クエスト番号の設定
    //    // activePicture_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);


    //}
}
