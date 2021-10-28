using UnityEngine;

public class SelfEffectDestroy : MonoBehaviour
{
    private ParticleSystem particle_;

    void Start()
    {
        particle_ = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particle_.isStopped) //パーティクルが終了したか判別
        {
            Destroy(this.gameObject);//パーティクル用ゲームオブジェクトを削除
        }
        else
        {
            if(gameObject.transform.localScale.x > 0.0f)    // サイズの変更速度はどれも同じだから代表してxで調べてる 
            {
                // エフェクトサイズの変更
                Vector3 tmp = gameObject.transform.localScale;  // 現在のサイズ
                tmp = tmp - new Vector3(0.1f, 0.1f, 0.1f);      // サイズを変更
                gameObject.transform.localScale = tmp;          // 変更したサイズを現在サイズに代入

                // エフェクト位置の変更
                Vector3 tmp2 = gameObject.transform.localPosition;  // 現在の位置
                tmp2.y = tmp2.y + 0.05f;                            // 位置を変更(上にいかせたいからyのみ変更)
                gameObject.transform.localPosition = tmp2;          // 変更した位置を現在位置に代入
            }
        }
    }
}
