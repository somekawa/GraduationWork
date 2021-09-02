using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncountColor : MonoBehaviour
{
    // 決められた時間内に赤まで必ず到達する仕組み
    // redStartFlg_は、時間値でtrueに変化する

    // intだとエラーがでる
    private readonly float red   = 255.0f;
    private readonly float green = 255.0f;
    private readonly float blue  = 255.0f;

    private float r = 0.0f;
    private float g = 0.0f;
    private float b = 0.0f;

    private bool redStartFlg_ = false;    // 青の加算が一定値を上回ったらtrueにする

    private FieldMng fieldMng_ = null;    // FieldMngスクリプトの情報
    private float encountTime_ = 0.0f;    // FieldMngから[現在値 / エンカウント発生値]を計算した値を受け取る
    private Image image_ = null;

    void Start()
    {
        // 現在使用されているマテリアルを取得
        image_ = this.GetComponent<Image>();

        fieldMng_ = GameObject.Find("FieldMng").GetComponent<FieldMng>();
    }

    void Update()
    {
        if(FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        {
            // 戦闘モード中なら計算を行わない
            // 値の初期化
            redStartFlg_ = false;
            r = 0.0f;
            g = 0.0f;
            b = 255.0f;
            return;
        }

        // 毎フレーム取得する必要がある
        encountTime_ = fieldMng_.GetNowEncountTime();

        //Debug.Log(encountTime_);

        if (encountTime_ >= 0.6f)
        {
            redStartFlg_ = true;
        }

        if (redStartFlg_)
        {
            // redは0から255へ向かわせたい
            r = ColorValueCalculation(red, true);

            if (encountTime_ >= 0.65f)
            {
                // 現在値から0へ向かわせたい
                g = ColorValueCalculation(green, false, -0.3f);
            }
            else
            {
                // 色の加算を続ける
                g = ColorValueCalculation(green, true);
            }

            // redStartFlg_がtrueになったら、減算を早くする
            b = ColorValueCalculation(blue, false, +0.2f);
        }
        else
        {
            // greenは0から255へ向かうようにする。
            g = ColorValueCalculation(green, true);

            // (time_ / toButtleTime_)は、(現在値 / エンカウント発生時間)なので0～1の値にできる
            // 上記のやつにblueを乗算すると、0から255へ向かう値になる。
            // blueは明るい状態から暗くしていきたいので、255- を先頭につけて値を反転させる(255から0へ向かう値になる)
            b = ColorValueCalculation(blue, false);
        }

        // マテリアルの色設定に緑色を設定
        image_.color= new Color(r/255.0f , g/255.0f , b/255.0f, 1.0f);
    }

    // カラー値計算をして返す
    // 引数(color : 受け取った色,valueUpFlg : 値の加算ならtrue,減算ならfalse,timeAdjust : 色合い調整用の時間(記入無しなら0.0f))
    private float ColorValueCalculation(float color, bool valueUpFlg, float timeAdjust = 0.0f)
    {
        if (valueUpFlg)
        {
            // 色の加算
            float col = color * encountTime_;
            if (col > 255.0f)
            {
                col = 255.0f;
            }
            return col;
        }
        else
        {
            // 色の減算
            float col = 255.0f - (color * (encountTime_ + timeAdjust));
            if (col < 0.0f)
            {
                col = 0.0f;
            }
            return col;
        }

        Debug.Log("EncountColor.csの関数でエラー");
        return 0.0f;
    }
}