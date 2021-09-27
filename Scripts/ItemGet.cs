using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ItemGet : MonoBehaviour
{
    private enum items
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
    private string[] objName = new string[(int)items.MAX] {
         "Item0","Item1","Item2","Item3","Item4"
    };
    // 素材オブジェクトを保存　ポジションチェックで使う
    private GameObject[] itemPointChildren_;

    private string hitObjName_; // 接触したオブジェクトの名前を代入
    private bool materiaGetFlag_ = false;// 素材を取得したかどうか

    // ーーーーーーーーー画像関連
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス

    // 素材イラスト
    private Image materiaImage_;        // 取得したアイテムを表示する画像
    private RectTransform materiaRect_; // materiaImage_の座標関連
    private Vector3 appearPos_;         // 画像出現位置
    private Vector2 middolePos_;        // 自身の位置と目的地までの中間点
    private Vector2 destinationPos_;    // 目的地
    private float iconSpeed_ = 0.0f;      // 放物線移動する際のスピード
    private float saveAnchoredY = 0.0f; // 表示位置のY座標を保存

    // テロップ（素材名とその背景）
    private Image telopImage_;              // 素材名の背景画像
    private RectTransform telopRect_;       // popTextBack_の座標関連
    private Text telopText_;                // 素材名表示
    private float telopAlpha_ = 0.0f;       // 画像と文字のアルファ値
    private float telopScale_ = 1.0f;       // 画像と文字のスケール
    private bool telopSmallFlag_ = false;   // 画像が縮小して良いか
    private bool telopFlag_ = false;        // 素材イラストをポップアップして良いか

    // ーーーーーーーーーエクセルから読み込んだもの
    private Texture2D texture;          // 表示したい素材イラストの全体
    private Sprite[] materialIcon_ = new Sprite[(int)items.MAX];// 取得した素材のイラスト
    private string[] getMaterial_ = new string[(int)items.MAX]; // 取得した素材の名前

    // ーーーーーーーーーその他
    private UnitychanController uniCtl_;
    private Camera mainCamera;      // 座標空間変更時に使用

    void Start()
    {
        uniCtl_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        parentCanvas_ = GameObject.Find("ItemCanvas").GetComponent<RectTransform>();

        materiaRect_ = parentCanvas_.gameObject.transform.Find("MaterialImage").GetComponent<RectTransform>();
        materiaImage_ = materiaRect_.GetComponent<Image>();

        telopRect_ = parentCanvas_.gameObject.transform.Find("TelopBackImage").GetComponent<RectTransform>();
        telopImage_ = telopRect_.GetComponent<Image>();

        telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
        telopRect_.gameObject.SetActive(false);

        telopText_ = telopRect_.transform.GetChild(0).GetComponent<Text>();
        telopText_.color = new Color(1.0f, 0.0f, 0.7f, telopAlpha_);
        materiaRect_.gameObject.SetActive(false);
        // Debug.Log(popUpImage_.transform.position);

        itemPointChildren_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < (int)items.MAX; i++)
        {
            itemPointChildren_[i] = this.transform.GetChild(i).gameObject;
        }
        // Debug.Log(telopImage_.transform.localPosition);
    }

    void Update()
    {
        if (materiaGetFlag_ == true && telopFlag_ == false)
        {
            NameAndPosCheck();
        }
        else if (materiaGetFlag_ == true && telopFlag_ == true)
        {
            // 現在座標が出現座標Y+50より低い位置だったら
            if (materiaRect_.transform.localPosition.y < saveAnchoredY + 40.0f)
            {
                // 素材画像と名前を上昇させる
                materiaRect_.transform.localPosition += new Vector3(0.0f, 30.0f * Time.deltaTime, 0.0f);
                telopRect_.transform.localPosition += new Vector3(0.0f, 30.0f * Time.deltaTime, 0.0f);
                // 名前はアルファ値0スタートのため加算
                telopAlpha_ += 0.3f * Time.deltaTime;
                Debug.Log("上昇" + materiaRect_.anchoredPosition.y +
                    "       回転" + telopImage_.transform.rotation.y);
            }
            else
            {
                uniCtl_.enabled = true;
                telopAlpha_ = 1.0f;
                if (telopScale_ < 1.5f)
                {
                    telopScale_ += 0.8f * Time.deltaTime;
                }
                else
                {
                    telopSmallFlag_ = true;

                    // 始点、終点、始点と終点間の距離を2分の1（0.5）に
                    middolePos_ = Vector3.Lerp(materiaRect_.anchoredPosition, destinationPos_, 0.5f);
                    // 中間座標を求める　
                    middolePos_ = new Vector2(middolePos_.x, middolePos_.y * (-1) + materiaRect_.anchoredPosition.y);

                    // 移動スピード
                    iconSpeed_ = 10 / Vector3.Distance(materiaRect_.anchoredPosition, destinationPos_);
                    //Debug.Log("表示後のImage座標" + popUpImage_.transform.localPosition);
                    StartCoroutine(ShootArrow(materiaRect_.anchoredPosition, destinationPos_, middolePos_, iconSpeed_));
                    telopFlag_ = false;
                    materiaGetFlag_ = false;
                    telopScale_ = 1.0f;
                }
                telopImage_.transform.localScale = new Vector3(telopScale_, telopScale_, telopScale_);
            }
            telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
            telopText_.color = new Color(1.0f, 0.0f, 0.7f, telopAlpha_);
        }

        if (telopSmallFlag_ == true)
        {
            if (materiaRect_.gameObject.activeSelf == true)
            {
                telopRect_.transform.localPosition += new Vector3(0.0f, 30.0f * Time.deltaTime, 0.0f);
                telopScale_ -= 0.5f * Time.deltaTime;
                telopAlpha_ -= 0.5f * Time.deltaTime;
            }
            else
            {
                telopSmallFlag_ = false;
                telopImage_.gameObject.SetActive(false);
                telopAlpha_ = 0.0f;
                telopScale_ = 1.0f;
            }
            telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
            telopText_.color = new Color(1.0f, 0.0f, 0.7f, telopAlpha_);
            telopImage_.transform.localScale = new Vector3(telopScale_, telopScale_, telopScale_);
        }
    }

    private void NameAndPosCheck()
    {
        // どのアイテムが呼ばれたのか一致するものを探す
        for (int i = 0; i < (int)items.MAX; i++)
        {
            if (hitObjName_ == objName[i])
            {
                telopText_.text = getMaterial_[i];
                materiaRect_.gameObject.SetActive(true);
                telopRect_.gameObject.SetActive(true);
                uniCtl_.enabled = false;
                materiaImage_.sprite = materialIcon_[i];

                // オブジェクトのワールド空間positionをビューポート空間に変換
                appearPos_ = mainCamera.WorldToViewportPoint(itemPointChildren_[i].transform.position);
                Debug.Log(appearPos_);
            }
        }

        // 取得したアイテムをポップアップさせる
        telopFlag_ = true;

        // ビューポートの原点は左下、Canvasは中央のためCanvasのRectTransformのサイズの1/2を引く
        var WorldObject_ScreenPosition = (appearPos_ * parentCanvas_.sizeDelta) - (parentCanvas_.sizeDelta * 0.5f);
        // 表示画像のアンカーに座標を代入して座標を決定
        materiaRect_.anchoredPosition = WorldObject_ScreenPosition;

        // Yの初期位置を保存
        saveAnchoredY = materiaRect_.anchoredPosition.y;

        // アイテム名は素材アイコンよりY+40の位置に表示
        telopRect_.anchoredPosition = new Vector2(WorldObject_ScreenPosition.x,
            WorldObject_ScreenPosition.y + 40);
        Debug.Log("表示後のImage座標" + materiaRect_.anchoredPosition);

        // 終点
        destinationPos_ = -parentCanvas_.sizeDelta / 2;
    }


    private IEnumerator ShootArrow(Vector3 start, Vector3 end, Vector3 middle, float arrow_speed)
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
                materiaRect_.gameObject.SetActive(false);
                yield break; // ループを抜ける
            }

            // ベジェ曲線の処理
            t += arrow_speed * Time.deltaTime * 50;
            Vector3 a = Vector3.Lerp(start, middle, t);
            Vector3 b = Vector3.Lerp(middle, end, t);
            // 座標を代入
            materiaRect_.anchoredPosition = Vector3.Lerp(a, b, t);

            yield return null;
        }
    }

    public void SetItemName(string name, bool flag)
    {
        hitObjName_ = name;
        materiaGetFlag_ = flag;
    }

    public void SetMaterialKinds(int filedNum, MaterialList list, MaterialsImage image)
    {
        // 素材名のリストを取得
        if (list != null)
        {
            for (int i = 0; i < list.param.Count; i++)
            {
                // 現在フィールドの素材名を保存
                getMaterial_[i] = list.param[i].ItemName;
            }
        }

        // 表示したい素材の最大個数を代入
        materialIcon_ = new Sprite[image.param.Count];

        //Assets直下のBack.pngというテクスチャを取得する
        texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Texture/" + image.param[0].MaterialImage + ".png");

        for (int x = 0; x < image.param.Count; x++)
        {
            // 取得した画像を１つ128*128のサイズに分割
            Rect rect = new Rect(x * 128, 0, 128, 128);
            materialIcon_[x] =
                Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f),
                                1.0f, 0, SpriteMeshType.FullRect);
        }

    }
}