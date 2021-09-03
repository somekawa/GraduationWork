using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMove : MonoBehaviour
{
    //�@���ł�������
    private Vector3 direction;
    //�@��΂���
    [SerializeField]
    private float power;
    //�@Rigidbody
    private Rigidbody rigid;
    //�@�p�[�e�B�N���V�X�e��
    private ParticleSystem particle;

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

    //�@�͂������Ĕ�΂�
    void FixedUpdate()
    {
        rigid.AddForce(direction * power);
    }

    void OnTriggerEnter(Collider col)
    {
        // �Ă΂�Ă͂���
        // �G�ɓ��������ꍇ
        if (col.tag == "Enemy")
        {
            Debug.Log("Hit");
            Destroy(col.gameObject);
            Destroy(this.gameObject);
            col = null; // Destroy���null�������
        }
    }
}
