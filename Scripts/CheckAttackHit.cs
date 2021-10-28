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
            if (this.gameObject.name == "KabosuAttack(Clone)")
            {
                return;
            }

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
        else if (col.CompareTag("Player"))
        {
            if (this.gameObject.name == "UniAttack(Clone)")
            {
                return;
            }

            // �ڕW�̃L�����ɓ��������ꍇ
            if (SceneMng.charasList_[targetNum_].Name() == col.name)
            {
                Debug.Log("Hit");

                // HP��������
                GameObject.Find("CharacterMng").GetComponent<CharacterMng>().HPdecrease(targetNum_);

                // ���@�̒e�̎��������@�̒e���폜����
                if (this.gameObject.name == "KabosuAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }
            }
        }

    }
}
