using System.Collections.Generic;
using UnityEngine;

// �퓬�S�̂ɂ��ĊǗ�����

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public Canvas fieldUICanvas;            // �\��/��\�������̃N���X�ŊǗ������

    public int debugEnemyNum = 1;           // �C���X�y�N�^�[����G�̐�������ς����悤�� 

    private bool setCallOnce_ = false;      // �퓬���[�h�ɐ؂�ւ�����ŏ��̃^�C�~���O�����؂�ւ��

    //private Transform buttleCommandUI_;         // ���̑�g���܂߂������擾

    private CharacterMng characterMng_;         // �L�����N�^�[�Ǘ��N���X�̏��
    private EnemyInstanceMng enemyInstanceMng_; // �G�C���X�^���X�Ǘ��N���X�̏��

    private List<(int,string)> moveTurnList_ = new List<(int, string)>();   // �L�����ƓG�̍s�������܂Ƃ߂郊�X�g
    private int moveTurnCnt_ = 0;           // �����̍s�����I�������l�𑝂₷
    private int damageNum_ = 0;             // �_���[�W�̒l
    private int speedNum_ = 0;              // ��������p�̒l
    private int luckNum_ = 0;               // �K�^�l�̒l
    private int element_ = 0;               // �G�������g���

    private bool lastEnemyFlg_;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        //buttleCommandUI_ = buttleUICanvas.transform.Find("Command");
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
            lastEnemyFlg_ = false;
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            moveTurnList_.Clear();
            moveTurnCnt_ = 0;
            damageNum_ = 0;

            characterMng_.ButtleSetCallOnce();

            // �G�̃C���X�^���X(1�`4)
            // �C�x���g�퓬�̏ꍇ�A�G�̐����قȂ�ꍇ������̂ŕԂ�l�Ő������l���󂯎��悤�ɂ���
            var correctEnemyNum = enemyInstanceMng_.EnemyInstance(debugEnemyNum, buttleUICanvas);

            // �G�̖��O�ƍs�����x���󂯎���ă��X�g�ɓ����
            for (int i = 0; i < correctEnemyNum; i++)
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
            characterMng_.SetEnemyNum(correctEnemyNum);
        }

        // �����̍s���O�̂Ƃ��ɂ��Ă΂��֐�
        characterMng_.NotMyTurn();
        enemyInstanceMng_.NotMyTurn();

        if (moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // �L�����N�^�[�̍s��
            characterMng_.Buttle();
            // �R�}���h��Ԃ̕\��/��\���ؑ�
            //buttleCommandUI_.gameObject.SetActive(!characterMng_.GetSelectFlg());

            // �L�����N�^�[�̍U���Ώۂ��Ō�̓G��������
            if (lastEnemyFlg_)
            {
                if(enemyInstanceMng_.AllAnimationFin())
                {
                    enemyInstanceMng_.DeleteEnemy();

                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    characterMng_.SetCharaFieldPos();
                }
            }
        }
        else
        {
            string[] arr = moveTurnList_[moveTurnCnt_].Item2.Split('_');
            // �G�̍s��
            enemyInstanceMng_.Buttle(int.Parse(arr[1]) - 1);
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

        // �s�����؂�ւ�閈�ɁA�G�̏�Ԃ��m�F����
        lastEnemyFlg_ = characterMng_.GetLastEnemyToAttackFlg();
    }

    public void SetDamageNum(int num)
    {
        Debug.Log("*SetDamageNum" + num);
        damageNum_ = num;
    }

    public int GetDamageNum()
    {
        Debug.Log("***GetDamageNum" + damageNum_);
        return damageNum_;
    }

    public void SetSpeedNum(int num)
    {
        Debug.Log("*SetSpeedNum" + num);
        speedNum_ = num;
    }

    public int GetSpeedNum()
    {
        Debug.Log("***GetSpeedNum" + speedNum_);
        return speedNum_;
    }

    public void SetLuckNum(int num)
    {
        Debug.Log("*SetLuckNum" + num);
        luckNum_ = num;
    }

    public int GetLuckNum()
    {
        Debug.Log("***GetLuckNum" + luckNum_);
        return luckNum_;
    }

    public void SetElement(int num)
    {
        Debug.Log("*SetElement" + num);
        element_ = num;
    }

    public int GetElement()
    {
        Debug.Log("***GetElement" + element_);
        return element_;
    }

    // ���j�����������鏈���̎��Ɏg�p����
    public void CallDeleteEnemy()
    {
        enemyInstanceMng_.DeleteEnemy();
    }
}
