using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DropFieldMateria : MonoBehaviour
{
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
    private string[] getMaterial_ = new string[(int)items.MAX]; // 取得した素材の名前

    // ーーーーーーーーーその他
    private UnitychanController uniCtl_;
    private Camera mainCamera;      // 座標空間変更時に使用

    // アイテムを取得した回数
   // private MenuActive menuActive_;
    private Bag_Materia materia_;
    private int itemNumberCheck_;
    private int fieldNumber_;

    private InitPopList materiaList_;

    void Start()
    {
        materiaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        fieldNumber_ = materiaList_.SetNowFieldMateriaList();

        uniCtl_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        parentCanvas_ = GameObject.Find("ItemCanvas").GetComponent<RectTransform>();
        materiaImage_ = parentCanvas_.transform.Find("MaterialImage").GetComponent<Image>();
      //  menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
        var gameObject = DontDestroyMng.Instance;
        materia_ = gameObject.transform.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();


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
    }

    public void SetItemName(int num, string name, bool flag)
    {
        // 接触したObjの名前,whileに入ってよいかのフラグ
        NameAndPosCheck(num, name);
        //itemCnt_++;
        StartCoroutine(UpPosImages(name, flag));// 取得したアイテムをポップアップさせる
    }

    private void NameAndPosCheck(int num, string name)
    {
        itemNumberCheck_ = num;

        //if (materia_ == null)
        //{
        //    materia_ = menuActive_.GetBagMateria();
        //}
        materia_.MateriaGetCheck( itemNumberCheck_, getMaterial_[num],3);

        telopText_.text = getMaterial_[num];
        materiaImage_.gameObject.SetActive(true);
        telopImage_.gameObject.SetActive(true);
        uniCtl_.enabled = false;
        // materiaImage_.sprite = materialIcon_[num];
        
        
        
        
        materiaImage_.sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][fieldNumber_* itemNumberCheck_ + itemNumberCheck_];
       
        
        
        
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

    private IEnumerator UpPosImages(string name, bool flag)
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
}