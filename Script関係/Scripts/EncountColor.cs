using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncountColor : MonoBehaviour
{
    // 決められた時間内に赤まで必ず到達する仕組み
    // redStartFlg_は、時間値でtrueに変化する

    private FieldMng fieldMng_;    // FieldMngから[現在値 / エンカウント発生値]を計算した値を受け取る

    // intだとエラーがでる
    private readonly float red   = 255.0f;
    private readonly float green = 255.0f;
    private readonly float blue  = 255.0f;

    private float r = 0.0f;
    private float g = 0.0f;
    private float b = 0.0f;

    private bool redStartFlg_ = false;  // 青の加算が一定値を上回ったらtrueにする

    private Image image_;

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
        float time = fieldMng_.GetNowEncountTime();

        Debug.Log(time);

        if (time >= 0.6f)
        {
            redStartFlg_ = true;
        }

        if (redStartFlg_)
        {
            // redは0から255へ向かわせたい
            r = red * time;
            if (r > 255.0f)
            {
                r = 255.0f;
            }

            if (time >= 0.65f)
            {
                // 現在値から0へ向かわせたい
                g = 255.0f - (green * (time - 0.3f));
                if (g < 0.0f)
                {
                    g = 0.0f;
                }
            }
            else
            {
                // 色の加算を続ける
                g = green * time;
                if (g > 255.0f)
                {
                    g = 255.0f;
                }
            }

            // redStartFlg_がtrueになったら、減算を早くする
            b = 255.0f - (blue * (time + 0.2f));
            if (b < 0.0f)
            {
                b = 0.0f;
            }
        }
        else
        {
            // greenは0から255へ向かうようにする。
            g = green * time;
            if (g > 255.0f)
            {
                g = 255.0f;
            }

            // (time_ / toButtleTime_)は、(現在値 / エンカウント発生時間)なので0～1の値にできる
            // 上記のやつにblueを乗算すると、0から255へ向かう値になる。
            // blueは明るい状態から暗くしていきたいので、255- を先頭につけて値を反転させる(255から0へ向かう値になる)
            b = 255.0f - (blue * time);
            if (b < 0.0f)
            {
                b = 0.0f;
            }
        }

        // マテリアルの色設定に緑色を設定
        image_.color= new Color(r/255.0f , g/255.0f , b/255.0f, 1.0f);
    }
}
