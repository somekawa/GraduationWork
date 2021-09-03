using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMove : MonoBehaviour
{
    //　飛んでいく方向
    private Vector3 direction;
    //　飛ばす力
    [SerializeField]
    private float power;
    //　Rigidbody
    private Rigidbody rigid;
    //　パーティクルシステム
    private ParticleSystem particle;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        //　自身の子要素からParticleSystemを取得
        particle = GetComponentInChildren<ParticleSystem>();

        // 5秒後に消える？
        //Destroy(this.gameObject,5);
    }

    void Update()
    {
        //　パーティクルの再生が終わったらゲームオブジェクトを消す
        if (!particle.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    //　飛んでいく方向を設定
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    //　力を加えて飛ばす
    void FixedUpdate()
    {
        rigid.AddForce(direction * power);
    }

    void OnTriggerEnter(Collider col)
    {
        // 呼ばれてはいる
        // 敵に当たった場合
        if (col.tag == "Enemy")
        {
            Debug.Log("Hit");
            Destroy(col.gameObject);
            Destroy(this.gameObject);
            col = null; // Destroy後にnull代入処理
        }
    }
}
