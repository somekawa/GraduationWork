using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaChange : MonoBehaviour
{
    private float alpha_ = 0.0f;                // 現在のα値を保存する
    private float alphaChangeSpeed_ = 0.01f;    // α値の変化速度

    // true:Image,false:TMPro
    private (GameObject,bool)[] chiledObj_;     // ImageとTMProのオブジェクト情報とフラグを保存する
    private List<int> notAlphaChangeList_ = new List<int>();    // α値の処理を行わない子供の番号

    // PopUpがアクティブになる度に呼び出される
    void OnEnable()
    {
        alpha_ = 0.0f;
        chiledObj_ = new (GameObject, bool)[gameObject.transform.childCount];

        // 自分の子供の数でfor文を回す
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            // どのコンポーネントがアタッチされているか調べる
            if(gameObject.transform.GetChild(i).gameObject.GetComponent<Image>())
            {
                // ゲームオブジェクトとtrueで配列に保存し、α値を0.0fにする
                chiledObj_[i] = (gameObject.transform.GetChild(i).gameObject,true);
                chiledObj_[i].Item1.GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f, alpha_);
            }
            else if(gameObject.transform.GetChild(i).gameObject.GetComponent<TMPro.TextMeshProUGUI>())
            {
                // ゲームオブジェクトとfalseで配列に保存し、α値を0.0fにする
                chiledObj_[i] = (gameObject.transform.GetChild(i).gameObject,false);
                chiledObj_[i].Item1.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
            }
            else
            {
                // コルーチンでの処理を行わない子供の番号をリストに保存する
                notAlphaChangeList_.Add(i);
            }
        }

        StartCoroutine(PopUpAlpha());
    }

    private IEnumerator PopUpAlpha()
    {
        // α値が1.0fより小さい間はwhile文を回り続ける
        while (alpha_ < 1.0f)
        {
            yield return null;

            bool tmpFlg = true;

            // α値を加算する
            alpha_ += alphaChangeSpeed_;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                // α値加算処理をしない番号か調べる
                for(int k = 0; k < notAlphaChangeList_.Count; k++)
                {
                    // 加算してほしくないときはbreakで処理を飛ばす
                    if(notAlphaChangeList_[k] == i)
                    {
                        tmpFlg = false;
                        break;
                    }
                }

                // break処理から下に来た時以外は通常のα値処理をする
                if(tmpFlg)
                {
                    // 各オブジェクトのα値を処理する
                    if (chiledObj_[i].Item2)
                    {
                        // Imageのα値を変更する
                        chiledObj_[i].Item1.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
                    }
                    else
                    {
                        // TMProのα値を変更する
                        chiledObj_[i].Item1.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
                    }
                }
            }
        }
    }

}
