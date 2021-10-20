using UnityEngine;

// �U���������������̊m�F�p

public class CheckAttackHit : MonoBehaviour
{
    // �ڕW�̓G�����ʂ���ԍ�
    private int targetNum_;

    public void SetTargetNum(int num)
    {
        targetNum_ = num;
    }

    void OnTriggerEnter(Collider col)
    {
        // �G�ɓ��������ꍇ(col.tag == "Enemy"�Ə������A����������)
        if (col.CompareTag("Enemy"))
        {
            // �ڕW�̓G�ɓ��������ꍇ
            if (targetNum_ == int.Parse(col.name))
            {
                Debug.Log("Hit");

                // HP��������
                GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().HPdecrease(targetNum_ - 1);

                // ���@�̒e�̎��������@�̒e���폜����
                if(this.gameObject.name == "UniAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    // ����R���C�_�[�̗L����(���i�q�b�g��h��)
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }
        }
    }
}
