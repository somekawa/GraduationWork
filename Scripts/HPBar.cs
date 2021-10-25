using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    //int damage = 10;        // 敵の攻撃力とキャラの防御力で変動(攻撃力-防御力)
    private int maxHp_;               // 最大HP
    private int currentHp_;           // 現在のHP
    private bool colFlg_ = false;     // コルーチン用フラグ(true:コルーチン中,false:非コルーチン)

    private Slider slider_;

    void Start()
    {
        if (slider_ == null)
        {
            slider_ = this.GetComponent<Slider>();
        }
        slider_.value = 1.0f;           // Sliderを満タンにする。

        Debug.Log("Start currentHp : " + currentHp_);
    }

    // コルーチン
    public IEnumerator MoveSlideBar(int num)
    {
        while(currentHp_ > num)   // 現在値が目標値より大きかったら減らして、while文続行
        {
            colFlg_ = true;

            currentHp_ -= 1;

            // スライドバーへ反映
            slider_.value = (float)currentHp_ / (float)maxHp_;

            yield return null;
        }

        colFlg_ = false;
    }

    public void SetHPBar(int nowHp,int maxHp)
    {
        currentHp_ = nowHp;
        maxHp_ = maxHp;

        if(slider_ == null)
        {
            slider_ = this.GetComponent<Slider>();
        }
        // スライドバーへ反映
        slider_.value = (float)currentHp_ / (float)maxHp_;
    }

    // 現在未使用
    //ColliderオブジェクトのIsTriggerにチェック入れること。
    private void OnTriggerEnter(Collider collider)
    {
        //Enemyタグのオブジェクトに触れると発動
        if (collider.gameObject.tag == "Enemy")
        {
            //ダメージは1～100の中でランダムに決める。
            int damage = 10;
            Debug.Log("damage : " + damage);

            //現在のHPからダメージを引く
            currentHp_ -= damage;
            Debug.Log("After currentHp : " + currentHp_);

            //最大HPにおける現在のHPをSliderに反映。
            //int同士の割り算は小数点以下は0になるので、
            //(float)をつけてfloatの変数として振舞わせる。
            slider_.value = (float)currentHp_ / (float)maxHp_; ;
            Debug.Log("slider.value : " + slider_.value);
        }
    }

    // コルーチン処理中かを確かめるフラグの取得
    public bool GetColFlg()
    {
        return colFlg_;
    }
}