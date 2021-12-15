using UnityEngine;

public class ParticleCheck : MonoBehaviour
{
    private int maxChildCnt_ = 0;// オブジェクトの個数
    private float distance_ = 0.0f;// オブジェとユニの距離を見る
    private float targetDistance_ = 25.0f;// 再生距離条件
  
    private GameObject uniChan_;// ユニの座標確認のため
    private GameObject[] obj_;// 配置されてるオブジェクト
    private ParticleSystem[] objParicle_;// オブジェクトについているパーティクル

    void Start()
    {
        uniChan_ = GameObject.Find("Uni");

        maxChildCnt_ = this.transform.childCount;

        obj_ = new GameObject[maxChildCnt_];
        objParicle_ = new ParticleSystem[maxChildCnt_];
        for (int i = 0; i < maxChildCnt_; i++)
        {
            obj_[i] = this.transform.GetChild(i).gameObject;

            // 各オブジェクトの子にパーティクルがついている
            objParicle_[i] = obj_[i].transform.GetChild(0).GetComponent<ParticleSystem>();
        }
    }

    void Update()
    {
        for (int i = 0; i < maxChildCnt_; i++)
        {
            // ユニとオブジェクトの距離を見る
            distance_ = (uniChan_.transform.position - obj_[i].transform.position).sqrMagnitude;
            if (distance_ < targetDistance_ * targetDistance_)
            {
                // 近い位置なら
                if (objParicle_[i].isPlaying == false)
                {
                    // Particleが再生されてなければ再生
                    objParicle_[i].Play();
                }
            }
            else
            {
                if (objParicle_[i].isPlaying == true)
                {
                    objParicle_[i].Stop();// Particle再生中なら停止
                }
            }
        }
    }
}