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

    private bool moveStopFlg_ = false;

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

    // �G�ƏՓ˂����Ƃ��Ɉړ����x��0�ɂ���
    public void MoveStop()
    {
        moveStopFlg_ = true;
    }

    //�@�͂������Ĕ�΂�
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
