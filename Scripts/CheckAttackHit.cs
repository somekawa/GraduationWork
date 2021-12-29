using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �U���������������̊m�F�p

public class CheckAttackHit : MonoBehaviour
{
    // �ڕW�̓G�����ʂ���ԍ�
    private int targetNum_ = -1;
    // �U�����鑤�̔ԍ�(�U�����ˏ����p)
    private int fromAttackNum_ = -1;

    // �G�G�t�F�N�g�̍폜���X�g
    private readonly List<string> enemyEffectDeleteList_ = new List<string>()
    {
        "KabosuAttack(Clone)","SpiderAttack(Clone)"
    };

    // �L�����̖��@�G�t�F�N�g�̍폜
    private string charaMagicStr_ = "";

    // �L��������G�ւ̍U���ł�fromNum���K�v�Ȃ�����-1�Ƃ��Ă���
    public void SetTargetNum(int num,int fromNum)
    {
        targetNum_ = num;
        fromAttackNum_ = fromNum;
    }

    public void SetCharaMagicStr(string str)
    {
        charaMagicStr_ = str;
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
                if(this.gameObject.name == "UniAttack(Clone)")
                {
                    Destroy(this.gameObject);
                }
                else if(this.gameObject.name == charaMagicStr_)
                {
                    var tmpStr = charaMagicStr_.Split('-');

                    if(tmpStr.Length >= 4)
                    {
                        // �⏕���@
                        GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().Debuff(targetNum_ - 1,int.Parse(tmpStr[0]),int.Parse(tmpStr[2]));

                        if(this.gameObject.GetComponent<MagicMove>())   // ����MagicMove.cs���A�^�b�`����Ă���Ȃ�
                        {
                            // �A�j���[�V�����I���܂ō폜�҂�
                            this.gameObject.GetComponent<MagicMove>().MoveStop();
                        }
                        else
                        {
                            Destroy(this.gameObject);
                        }
                    }
                    else
                    {
                        // �U�����@
                        // ��З͂��ɑ�З͂��y���@�������@�̒��Ȃ�
                        if (tmpStr[1] == "2(Clone)" ||
                            tmpStr[1] == "3(Clone)" ||
                            tmpStr[0] == "4" ||
                           (tmpStr[0] == "5" && tmpStr[1] == "1(Clone)"))
                        {
                            // �A�j���[�V�����I���܂ō폜�҂�
                            this.gameObject.GetComponent<MagicMove>().MoveStop();
                        }
                        else
                        {
                            // �y���@�ȊO�͂����ɍ폜����(����́A)
                            Destroy(this.gameObject);
                        }
                    }
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

            if (this.gameObject.name == "UniAttack(Clone)" || this.gameObject.name == charaMagicStr_)
            {
                return;
            }

            // �ڕW�̃L�����ɓ��������ꍇ
            if (SceneMng.charasList_[targetNum_].Name() == col.name)
            {
                Debug.Log("Hit");

                // HP��������
                GameObject.Find("CharacterMng").GetComponent<CharacterMng>().HPdecrease(targetNum_, fromAttackNum_);

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
