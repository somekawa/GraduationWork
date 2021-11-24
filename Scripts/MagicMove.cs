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

    private bool moveStopFlg_ = false;

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

    // 敵と衝突したときに移動速度を0にする
    public void MoveStop()
    {
        moveStopFlg_ = true;
    }

    //　力を加えて飛ばす
    void FixedUpdate()
    {
        if(moveStopFlg_)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
            return;
        }
        rigid.AddForce(direction * power);
    }
}
