using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPMPBar : MonoBehaviour
{
    private int maxNum_;               // 最大数値
    private int currentNum_;           // 現在の数値

    private bool colFlg_ = false;     // コルーチン用フラグ(true:コルーチン中,false:非コルーチン)

    private Slider slider_;
    private TMPro.TextMeshProUGUI currentNumText_;  // 現在数値を表示するテキスト

    void Start()
    {
        if (slider_ == null)
        {
            slider_ = this.GetComponent<Slider>();
        }
        slider_.value = 1.0f;           // Sliderを満タンにする。

        // 現在数値表示場所の取得
        SettingCurrntNum();
    }

    // コルーチン
    public IEnumerator MoveSlideBar(int num)
    {
        while(currentNum_ > num)   // 現在値が目標値より大きかったら減らして、while文続行
        {
            colFlg_ = true;

            currentNum_ -= 1;

            if (currentNum_ < 0)
            {
                currentNumText_.text = 0.ToString();
            }
            else
            {
                currentNumText_.text = currentNum_.ToString();
            }

            // スライドバーへ反映
            slider_.value = (float)currentNum_ / (float)maxNum_;

            yield return null;
        }

        // 0以下は全て0と表記する
        if (currentNum_ <= 0)
        {
            currentNum_ = 0;
            currentNumText_.text = currentNum_.ToString();
            colFlg_ = false;
            yield return null;
        }

        while (currentNum_ < num)   // 現在値が目標値より小さかったら足して、while文続行
        {
            colFlg_ = true;

            currentNum_ += 1;
            if (currentNum_ > maxNum_)
            {
                currentNumText_.text = maxNum_.ToString();
            }
            else
            {
                currentNumText_.text = currentNum_.ToString();
            }

            // スライドバーへ反映
            slider_.value = (float)currentNum_ / (float)maxNum_;

            yield return null;
        }

        // HPの最大値以上は全て最大値と表記する
        if(currentNum_ > maxNum_)
        {
            currentNum_ = maxNum_;
            currentNumText_.text = currentNum_.ToString();
            colFlg_ = false;
        }
    }

    public void SetHPMPBar(int nowHp,int maxHp)
    {
        currentNum_ = nowHp;
        maxNum_ = maxHp;

        if(slider_ == null)
        {
            slider_ = this.GetComponent<Slider>();
        }
        // スライドバーへ反映
        slider_.value = (float)currentNum_ / (float)maxNum_;

        SettingCurrntNum();
    }

    // コルーチン処理中かを確かめるフラグの取得
    public bool GetColFlg()
    {
        return colFlg_;
    }

    private void SettingCurrntNum()
    {
        if (currentNumText_ == null && gameObject.transform.Find("CurrentNum"))
        {
            currentNumText_ = gameObject.transform.Find("CurrentNum").GetComponent<TMPro.TextMeshProUGUI>();
        }

        if (currentNumText_ != null)
        {
            currentNumText_.text = currentNum_.ToString();
        }
    }
}