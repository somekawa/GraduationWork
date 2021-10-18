using UnityEngine;

public class MagicMove : MonoBehaviour
{
    // ���ł�������
    private Vector3 direction;
    // ��΂���
    [SerializeField]
    private float power;
    // Rigidbody
    private Rigidbody rigid;
    // �p�[�e�B�N���V�X�e��
    private ParticleSystem particle;
    // �ڕW�̓G�����ʂ���ԍ�
    //private int targetNum_;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        //�@���g�̎q�v�f����ParticleSystem���擾
        particle = GetComponentInChildren<ParticleSystem>();

        // 5�b��ɏ�����H
        //Destroy(this.gameObject,5);
    }

    void Update()
    {
        //�@�p�[�e�B�N���̍Đ����I�������Q�[���I�u�W�F�N�g������
        if (!particle.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    //�@���ł���������ݒ�
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    //public void SetTargetNum(int num)
    //{
    //    targetNum_ = num;
    //}

    //�@�͂������Ĕ�΂�
    void FixedUpdate()
    {
        rigid.AddForce(direction * power);
    }

    //void OnTriggerEnter(Collider col)
    //{
    //    // �G�ɓ��������ꍇ(col.tag == "Enemy"�Ə������A����������)
    //    if (col.CompareTag("Enemy"))
    //    {
    //        // �ڕW�̓G�ɓ��������ꍇ
    //        if(targetNum_ == int.Parse(col.name))
    //        {
    //            Debug.Log("Hit");
    //            Destroy(col.gameObject);
    //            Destroy(this.gameObject);
    //            col = null; // Destroy���null�������
    //        }
    //    }
    //}
}
