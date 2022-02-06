using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveDropImage : MonoBehaviour
{
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス
    private DropFieldMateria drop_;

    private Image materiaImage_;        // 取得したアイテムを表示する画像
    private Vector2 middolePos_;        // 自身の位置と目的地までの中間点
    private Vector2 destinationPos_;    // 目的地
    private float iconSpeed_ = 0.0f;    // 放物線移動する際のスピード
    private Vector2 saveTopPos_;        // 頂点にいるときの座標を保存

    // アルファ値の減少を開始する座標
    private Vector2 minusAlphaPos_ = new Vector2(-450.0f, -200.0f);
    private float alphaNum = 1.0f;  // 画像のアルファ値

    void Start()
    {
        materiaImage_ = transform.GetComponent<Image>();
        parentCanvas_ = GameObject.Find("FieldUICanvas").GetComponent<RectTransform>();

        drop_ = GameObject.Find("MateriaPoints").GetComponent<DropFieldMateria>();
        StartCoroutine(UpPosImages());// 取得したアイテムをポップアップさせる
    }

    private IEnumerator UpPosImages()
    {
        // 上昇中かどうか
        bool upFlag = true;
       // Debug.Log(transform.name+ "の画像を移動させます　スケールとアルファ値");
        // ベジェ曲線用の変数の宣言
        float t = 0.0f;
        while (true)
        {
            yield return null;
            if (upFlag == true)
            {
                if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
                {
                    // 画像移動中にバトルが始まったら破壊する
                    Destroy(gameObject);
                }

                // 現在座標が出現座標Y+40より低い位置だったら
                if (drop_.GetShootArrowFlag() == false)
                {
                    // 素材画像と名前を上昇させる
                    transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
                }
                else
                {
                    // 始点、終点、始点と終点間の距離を2分の1（0.5）に
                    middolePos_ = Vector3.Lerp(transform.localPosition, destinationPos_, 0.5f);
                    // 中間座標を求める　
                    middolePos_ = new Vector2(middolePos_.x,
                        middolePos_.y * (-1) + transform.localPosition.y);
                    // 終点
                    destinationPos_ = -parentCanvas_.sizeDelta / 2; 

                    // 移動スピード
                    iconSpeed_ = 10 / Vector3.Distance(transform.localPosition, destinationPos_);
                    saveTopPos_ = transform.localPosition;  // 頂点にいるときの座標を保存
                    upFlag = false;                         // 上昇から放物線移動に変更
                }
            }
            else
            {
                if (t > 1)
                {
                    // 終着点でこのオブジェクトを削除
                    if (transform.name == "0")
                    {
                        // 1つでも生成されているとき＝名前が0番の時だけ
                        drop_.SetMoveFinish(false);
                    }
                    Debug.Log(transform.name + "を削除");
                    Destroy(gameObject);      // オブジェクトが破壊されたらコルーチンも止まる
                }

                // ベジェ曲線の処理
                t += iconSpeed_ * Time.deltaTime * 80.0f;
                Vector3 a = Vector3.Lerp(saveTopPos_, middolePos_, t);
                Vector3 b = Vector3.Lerp(middolePos_, destinationPos_, t);
                transform.localPosition = Vector3.Lerp(a, b, t);                // 座標を代入

                // 放物線を描いている間テロップは上昇させる
                if (transform.name == "0")
                {
                    // 1つでも生成されているとき＝名前が0番の時だけ
                    drop_.SetMoveFinish(true);
                }

                if (transform.localPosition.x < minusAlphaPos_.x &&
                transform.localPosition.y < minusAlphaPos_.y)
                {
                    // 画面左端に出る前にアルファ値を下げてフェードアウトさせる
                    alphaNum -= 0.05f;
                    materiaImage_.color = new Color(1.0f, 1.0f, 1.0f, alphaNum);
                }
            }
        }
    }
}
