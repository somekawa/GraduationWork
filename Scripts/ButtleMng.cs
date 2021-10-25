using System.Collections.Generic;
using UnityEngine;

// �퓬�S�̂ɂ��ĊǗ�����

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public Canvas fieldUICanvas;            // �\��/��\�������̃N���X�ŊǗ������

    public int debugEnemyNum = 1;           // �C���X�y�N�^�[����G�̐�������ς����悤�� 

    private bool setCallOnce_ = false;      // �퓬���[�h�ɐ؂�ւ�����ŏ��̃^�C�~���O�����؂�ւ��

    private Transform buttleCommandUI_;         // ���̑�g���܂߂������擾

    private CharacterMng characterMng_;         // �L�����N�^�[�Ǘ��N���X�̏��
    private EnemyInstanceMng enemyInstanceMng_; // �G�C���X�^���X�Ǘ��N���X�̏��

    private List<(int,string)> moveTurnList_ = new List<(int, string)>();   // �L�����ƓG�̍s�������܂Ƃ߂郊�X�g

    private int moveTurnCnt_ = 0;           // �����̍s�����I�������l�𑝂₷

    private int damageNum_ = 0;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        buttleCommandUI_ = buttleUICanvas.transform.Find("Command");
        buttleUICanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // FieldMng�ő����^�C�~���O�𒲐����Ă��邽�߁A������Q�Ƃ��A�퓬���[�h�ȊO�Ȃ�return����
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            setCallOnce_ = false;

            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // �퓬�J�n���ɐݒ肳��鍀��
        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            characterMng_.ButtleSetCallOnce();

            // �G�̃C���X�^���X(1�`4)
            enemyInstanceMng_.EnemyInstance(debugEnemyNum, buttleUICanvas);

            // �G�̖��O�ƍs�����x���󂯎���ă��X�g�ɓ����
            for (int i = 0; i < debugEnemyNum; i++)
            {
                moveTurnList_.Add(enemyInstanceMng_.EnemyTurnSpeed(i));
            }

            // �L�����̖��O�ƍs�����x���󂯎���ă��X�g�ɓ����
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                moveTurnList_.Add(characterMng_.CharaTurnSpeed(i));
            }

            moveTurnList_.Sort();    // �����Ƀ\�[�g
            moveTurnList_.Reverse(); // �~���ɂ��邽�߂ɋt�]������

            // Character�Ǘ��N���X�ɓG�̏o������n��
            characterMng_.SetEnemyNum(debugEnemyNum);
        }

        if(moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // �L�����N�^�[�̍s��
            characterMng_.Buttle();
            // �R�}���h��Ԃ̕\��/��\���ؑ�
            buttleCommandUI_.gameObject.SetActive(!characterMng_.GetSelectFlg());

            // �L�����N�^�[�̍U���Ώۂ��Ō�̓G��������
            if (characterMng_.GetLastEnemyToAttackFlg())
            {
                // Enemy�^�O�̐������ĊY�����镨���Ȃ�(= 0)�Ȃ�AMODE��T���ɐ؂�ւ���
                if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
                {
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    characterMng_.SetCharaFieldPos();
                }
            }
        }
        else
        {
            string[] arr = moveTurnList_[moveTurnCnt_].Item2.Split('_');
            // �G�̍s��
            enemyInstanceMng_.Attack(int.Parse(arr[1]) - 1);
        }
    }

    // �s�����I������Ƃ��ɌĂяo����Ď����ŉ��Z�����
    public void SetMoveTurn()
    {
        // ���Z�l�����X�g�̏�����z������0�ɖ߂�
        if(++moveTurnCnt_ > moveTurnList_.Count - 1)
        {
            moveTurnCnt_ = 0;
        }
    }

    public void SetDamageNum(int num)
    {
        damageNum_ = num;
    }

    public int GetDamageNum()
    {
        return damageNum_;
    }
}
