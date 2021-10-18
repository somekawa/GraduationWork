using UnityEngine;

public class MagicMove : MonoBehaviour
{
    // 飛んでいく方向
    private Vector3 direction;
    // 飛ばす力
    [SerializeField]
    private float power;
    // Rigidbody
    private Rigidbody rigid;
    // パーティクルシステム
    private ParticleSystem particle;
    // 目標の敵か判別する番号
    //private int targetNum_;

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

    //public void SetTargetNum(int num)
    //{
    //    targetNum_ = num;
    //}

    //　力を加えて飛ばす
    void FixedUpdate()
    {
        rigid.AddForce(direction * power);
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    // 敵に当たった場合(col.tag == "Enemy"と書くより、処理が速い)
    //    if (col.CompareTag("Enemy"))
    //    {
    //        // 目標の敵に当たった場合
    //        if(targetNum_ == int.Parse(col.name))
    //        {
    //            Debug.Log("Hit");
    //            Destroy(col.gameObject);
    //            Destroy(this.gameObject);
    //            col = null; // Destroy後にnull代入処理
    //        }
    //    }
    //}
}
