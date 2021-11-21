using System.Collections.Generic;
using UnityEngine;

// �U���������������̊m�F�p

public class CheckAttackHit : MonoBehaviour
{
    // �ڕW�̓G�����ʂ���ԍ�
    private int targetNum_ = -1;

    // �G�G�t�F�N�g�̍폜���X�g
    private readonly List<string> enemyEffectDeleteList_ = new List<string>()
    {
        "KabosuAttack(Clone)","SpiderAttack(Clone)"
    };

    public void SetTargetNum(int num)
    {
        targetNum_ = num;
    }

    void OnTriggerEnter(Collider col)
    {
        // �G�ɓ��������ꍇ(col.tag == "Enemy"�Ə������A����������)
        if (col.CompareTag("Enemy"))
        {
            for(int i = 0; i < enemyEffectDeleteList_.Count; i++)
            {
                if (this.gameObject.name == enemyEffectDeleteList_[i])
                {
                    return;
                }
            }

            // �ڕW�̓G�ɓ��������ꍇ
            if (targetNum_ == int.Parse(col.name))
            {
                Debug.Log("Hit");

                // HP��������
                GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().HPdecrease(targetNum_ - 1);

                // ���@�̒e�̎��������@�̒e���폜����
                if(this.gameObject.name == "UniAttack(Clone)" || this.gameObject.name == "2-0(Clone)")
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    // ����R���C�_�[�̗L����(���i�q�b�g��h��)
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }

                targetNum_ = -1;
            }
        }
        else if (col.CompareTag("Player"))
        {
            // targetNum_��-1�Ƃ������Ƃ́A�U���Ώۂ��ݒ肳��Ă��Ȃ���Ԃ�����Areturn��Ԃ�
            if(targetNum_ < 0)
            {
                return;
            }

            if (this.gameObject.name == "UniAttack(Clone)" || this.gameObject.name == "2-0(Clone)")
            {
                return;
            }

            // �ڕW�̃L�����ɓ��������ꍇ
            if (SceneMng.charasList_[targetNum_].Name() == col.name)
            {
                Debug.Log("Hit");

                // HP��������
                GameObject.Find("CharacterMng").GetComponent<CharacterMng>().HPdecrease(targetNum_);

                // �����̖��O��BombMonster�ł���΁A�������Ď������폜����
                if (gameObject.name == "BombMonster")
                {
                    GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().HPdecrease(int.Parse(transform.root.gameObject.name) - 1);
                }

                for (int i = 0; i < enemyEffectDeleteList_.Count; i++)
                {
                    // ���@�̒e�̎��������@�̒e���폜����
                    if (this.gameObject.name == enemyEffectDeleteList_[i])
                    {
                        Destroy(this.gameObject);
                    }
                }

                targetNum_ = -1;
            }
        }

    }
}
