using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class ItemGet : MonoBehaviour
{
    //[SerializeField]
    //public GameObject materiaUIPrefab;    // 素材を拾ったときに生成されるプレハブ
    ////[SerializeField]
    //public GameObject canvasPrefab;    // 素材を拾ったときに生成されるプレハブ
    //private GameObject parentObjPrefab_;
    //private RectTransform parentRectPrefab_;

    public enum items
    {
        NON = -1,
        ITEM0,  // カボス    酢の橘
        ITEM1,  // ハチミツ
        ITEM2,  // 花の蜜
        ITEM3,  // きのこ
        ITEM4,  // 妖精の羽
        MAX
    }
    private int itemNumber_ = (int)items.MAX;
    // オブジェクトの名前
    public static string[] objName = new string[(int)items.MAX] {
         "Item0","Item1","Item2","Item3","Item4"
    };
    // 素材オブジェクトを保存　ポジションチェックで使う
    private GameObject[] itemPointChildren_;

    // ーーーーーーーーー画像関連
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス

    // 素材イラスト
    private Image materiaImage_;        // 取得したアイテムを表示する画像
    private Vector3 appearPos_;         // 画像出現位置
    private Vector2 middolePos_;        // 自身の位置と目的地までの中間点
    private Vector2 destinationPos_;    // 目的地
    private float iconSpeed_ = 0.0f;      // 放物線移動する際のスピード

    // テロップ（素材名とその背景）
    private Image telopImage_;              // 素材名の背景画像
    private Text telopText_;                // 素材名表示
    private float telopAlpha_ = 0.0f;       // 画像と文字のアルファ値
    private float maxScale_ = 1.5f;
    private float telopScale_ = 0.8f;       // 画像と文字のスケール

    // ーーーーーーーーーエクセルから読み込んだもの
    private Texture2D texture;          // 表示したい素材イラストの全体
    private Sprite[] materialIcon_ = new Sprite[(int)items.MAX];// 取得した素材のイラスト
    private string[] getMaterial_ = new string[(int)items.MAX]; // 取得した素材の名前
    private static bool onceFlag_ = false;

    // ーーーーーーーーーその他
    private UnitychanController uniCtl_;
    private Camera mainCamera;      // 座標空間変更時に使用

    // アイテムを取得した回数
    //private int itemCnt_=0;
    private MenuActive menuActive_;
    private Bag_Materia materia_;
    private string getItemName_ = "";
    private int itemNumberCheck_;
    private int fieldNumber_;

    void Start()
    {
        uniCtl_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        parentCanvas_ = GameObject.Find("ItemCanvas").GetComponent<RectTransform>();
        materiaImage_ = parentCanvas_.transform.Find("MaterialImage").GetComponent<Image>();
       menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
      
        telopImage_ = parentCanvas_.transform.Find("TelopBackImage").GetComponent<Image>();
        telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
        telopImage_.gameObject.SetActive(false);

        telopText_ = telopImage_.transform.GetChild(0).GetComponent<Text>();
        telopText_.color = new Color(1.0f, 0.0f, 0.7f, telopAlpha_);
        materiaImage_.gameObject.SetActive(false);
        // Debug.Log(popUpImage_.transform.position);

        itemPointChildren_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < (int)items.MAX; i++)
        {
            itemPointChildren_[i] = this.transform.GetChild(i).gameObject;
        }
        // Debug.Log(telopImage_.transform.localPosition);

        //if (parentObjPrefab_ == null)
        //{
        //    parentObjPrefab_ = Instantiate(canvasPrefab);
        //    //parentRectPrefab_=parentObjPrefab_.transform;
        //}
    }

    public void SetItemName(int num,string name, bool flag)
    {
        getItemName_ = name;
        // 接触したObjの名前,whileに入ってよいかのフラグ
        NameAndPosCheck(num, name);
        //itemCnt_++;
        StartCoroutine(UpPosImages(name, flag));// 取得したアイテムをポップアップさせる
    }

    private void NameAndPosCheck(int num, string name)
    {
        itemNumberCheck_ = num;

        if (materia_ == null)
        {
            materia_ = menuActive_.GetBagMateria();
        }
        materia_.ItemGetCheck(fieldNumber_, itemNumberCheck_, getMaterial_[num]); 
        //itemNumber_ = num;


        telopText_.text = getMaterial_[num];
        materiaImage_.gameObject.SetActive(true);
        telopImage_.gameObject.SetActive(true);
        uniCtl_.enabled = false;
        // materiaImage_.sprite = materialIcon_[num];
        materiaImage_.sprite = ItemImageMng.materialIcon_[fieldNumber_, itemNumberCheck_];
        itemPointChildren_[num].SetActive(false);

        // オブジェクトのワールド空間positionをビューポート空間に変換
        appearPos_ = mainCamera.WorldToViewportPoint(itemPointChildren_[num].transform.position);
        Debug.Log(appearPos_);

        // ビューポートの原点は左下、Canvasは中央のためCanvasのRectTransformのサイズの1/2を引く
        var WorldObject_ScreenPosition = (appearPos_ * parentCanvas_.sizeDelta) - (parentCanvas_.sizeDelta * 0.5f);
        // 表示画像のアンカーに座標を代入して座標を決定
        materiaImage_.transform.localPosition = WorldObject_ScreenPosition;

        // アイテム名は素材アイコンよりY+40の位置に表示
        telopImage_.transform.localPosition = new Vector2(WorldObject_ScreenPosition.x,
            WorldObject_ScreenPosition.y + 70);
       // Debug.Log("表示後のImage座標" + materiaImage_.anchoredPosition);

        // 終点
        destinationPos_ = -parentCanvas_.sizeDelta / 2;
    }

    private IEnumerator UpPosImages(string name,bool flag)
    {
        while (flag)
        {
            // 現在座標が出現座標Y+40より低い位置だったら
            if (telopScale_ < maxScale_)
            {
                // 素材画像と名前を上昇させる
                materiaImage_.transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
                telopImage_.transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
                ChangeNums(0.8f, 0.8f);
            }
            else
            {
                uniCtl_.enabled = true;// ユニが動くようにする
                // 始点、終点、始点と終点間の距離を2分の1（0.5）に
                middolePos_ = Vector3.Lerp(materiaImage_.transform.localPosition, destinationPos_, 0.5f);
                // 中間座標を求める　
                middolePos_ = new Vector2(middolePos_.x,
                    middolePos_.y * (-1) + materiaImage_.transform.localPosition.y);

                // 移動スピード
                iconSpeed_ = 10 / Vector3.Distance(materiaImage_.transform.localPosition, destinationPos_);
                //Debug.Log("表示後のImage座標" + popUpImage_.transform.localPosition);
                StartCoroutine(ShootArrow(materiaImage_.transform.localPosition,
                    destinationPos_, middolePos_, iconSpeed_));
                ChangeNums(1.0f, 1.0f);// 不透明度をなくしスケールが変化しないように
                flag = false;
            }
            yield return null;
        }
    }

    private IEnumerator ShootArrow(Vector3 start, Vector3 end, Vector3 middle, float speed)
    {
        // ベジェ曲線用の変数の宣言
        float t = 0.0f;

        // ループを抜けるまで以下を繰り返す
        while (true)
        {
            if (t > 1)
            {
                // 終着点でこのオブジェクトを削除
                //Debug.Log("移動終了");
                materiaImage_.gameObject.SetActive(false);
                telopImage_.gameObject.SetActive(false);
                yield break; // ループを抜ける
            }

            // ベジェ曲線の処理
            t += speed * Time.deltaTime * 80.0f;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            // 座標を代入
            materiaImage_.transform.localPosition = Vector3.Lerp(a, b, t);

            // テロップ画像上昇
            telopImage_.transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
            ChangeNums(-0.8f, -0.8f);
            yield return null;
        }
    }

    private void ChangeNums(float alpha, float scale)
    {
        telopAlpha_ += alpha * Time.deltaTime;
        telopScale_ += scale * Time.deltaTime;
        telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
        telopText_.color = new Color(1.0f, 0.0f, 0.7f, telopAlpha_);
        telopImage_.transform.localScale = new Vector3(telopScale_, telopScale_, telopScale_);
    }

   public void SetMaterialKinds(int fieldNum, MaterialList list)
    {
        // 素材名のリストを取得
        if (list != null)
        {
            for (int i = 0; i < (int)items.MAX; i++)
            {
                // 現在フィールドの素材名を保存
                getMaterial_[i] = list.param[i].MateriaName;
            }
        }
        fieldNumber_ = fieldNum;


        //// Texture2Dとして画像を読み込む
        //string str = Application.streamingAssetsPath + "/Test/" + list.param[0].ImageName + ".png";
        //byte[] bytes = File.ReadAllBytes(str);
        //Texture2D texture = new Texture2D(128, 128);
        //texture.LoadImage(bytes);

        //// Sprite.Createで作られたものは動的に削除
        //// 1回目はnullのため2回目から削除する
        //if (onceFlag_ == true)
        //{
        //    for (int x = 0; x < list.param.Count; x++)
        //    {
        //        Destroy(materialIcon_[x]);
        //        Debug.Log(x + "番が破壊されました");
        //    }
        //}

        //// 表示したい素材の最大個数を代入
        //materialIcon_ = new Sprite[list.param.Count];

        //for (int x = 0; x < list.param.Count; x++)
        //{
        //    // 取得した画像を１つ128*128のサイズに分割
        //    Rect rect = new Rect(x * 128, 0, 128, 128);
        //    materialIcon_[x] =
        //        Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f),
        //                        1.0f, 0, SpriteMeshType.FullRect);
        //}
        //onceFlag_ = true;
    }


}