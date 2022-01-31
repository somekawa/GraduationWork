using System.Collections.Generic;
using UnityEngine;

// �퓬�S�̂ɂ��ĊǗ�����

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public Canvas fieldUICanvas;            // �\��/��\�������̃N���X�ŊǗ������

    private int instanceEnemyNum_;          // �G�̏o��������(�t�B�[���h����Start�֐��Œ�������)

    private bool setCallOnce_ = false;      // �퓬���[�h�ɐ؂�ւ�����ŏ��̃^�C�~���O�����؂�ւ��

    private CharacterMng characterMng_;         // �L�����N�^�[�Ǘ��N���X�̏��
    private EnemyInstanceMng enemyInstanceMng_; // �G�C���X�^���X�Ǘ��N���X�̏��

    private Transform EneSelObj_;           // �G�̎w��}�[�N

    private List<(int, string)> moveTurnList_ = new List<(int, string)>();   // �L�����ƓG�̍s�������܂Ƃ߂郊�X�g
    private int moveTurnCnt_ = 0;           // �����̍s�����I�������l�𑝂₷
    private int damageNum_ = 0;             // �_���[�W�̒l
    private int speedNum_ = 0;              // ��������p�̒l
    private int luckNum_ = 0;               // �K�^�l�̒l
    private int element_ = 0;               // �G�������g���
    private (int, int) badStatusNum_;       // ��Ԉُ�̐���
    private int refNum_ = -1;               // �U�����ˑΏۂ̔ԍ���ۑ�����ϐ�
    private bool autoHitFlg_ = false;       // �������ʂ̃t���O
    private Vector3 keepPos_;
    private bool isAttackMagicFlg_ = false; // �G�̍U�������������@���𔻒肷��(�ߋ����͕���:false,�������͖��@:true)

    private bool lastEnemyFlg_;

    public static string forcedButtleWallName;

    private ButtleResult buttleResult_;     // �퓬���U���g�p
    private bool resultFlg_ = false;        // ���U���g������1�x��������true�ɂ���
    private int[] saveEnemyNum_ = new int[5];
    private bool bossFlag_ = false;

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();
        buttleUICanvas.gameObject.SetActive(false);
        buttleResult_ = gameObject.GetComponent<ButtleResult>();
    }

    void Update()
    {
        // FieldMng�ő����^�C�~���O�𒲐����Ă��邽�߁A������Q�Ƃ��A�퓬���[�h�ȊO�Ȃ�return����
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE && FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
        {
            setCallOnce_ = false;

            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // �퓬�J�n���ɐݒ肳��鍀��
        if (!setCallOnce_)
        {
            resultFlg_ = false;
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

            // �G���̏����ݒ肷��(Start�֐��ł͊��S�ɃV�[�����ړ�������Ă��Ȃ��̂Œl���������Ȃ�Ȃ�)
            if (SceneMng.nowScene == SceneMng.SCENE.FIELD0)
            {
                instanceEnemyNum_ = 3;  // 3����(=2�̂܂�)
            }
            else if (SceneMng.nowScene == SceneMng.SCENE.FIELD1)
            {
                instanceEnemyNum_ = 4;  // 4����(=3�̂܂�)
            }
            else
            {
                instanceEnemyNum_ = 5;  // 5����(=4�̂܂�)
            }
            var correctEnemyNum = enemyInstanceMng_.EnemyInstance(Random.Range(1, instanceEnemyNum_), buttleUICanvas);

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

            // ���ԗp�̐��������ׂ̈ꎞ�ϐ�
            for (int i = 0; i < moveTurnList_.Count; i++)
            {
                // �L�����̍s���^�[��������������(0����J�E���g���Ă邩��i+1�Ƃ���) 
                if (!characterMng_.SetMoveSpeedNum(i + 1, moveTurnList_[i].Item2))
                {
                    // �G�̍s���^�[��������������
                    string[] arr = moveTurnList_[i].Item2.Split('_');
                    enemyInstanceMng_.SetMoveSpeedNum(i + 1, arr[1]);
                }
            }

            // Character�Ǘ��N���X�ɓG�̏o������n��
            characterMng_.SetEnemyNum(correctEnemyNum);
        }

        // �����̍s���O�̂Ƃ��ɂ��Ă΂��֐�
        characterMng_.NotMyTurn();
        enemyInstanceMng_.NotMyTurn(refNum_);

        if (moveTurnList_[moveTurnCnt_].Item2 == "Uni" || moveTurnList_[moveTurnCnt_].Item2 == "Jack")
        {
            // �L�����N�^�[�̍s��
            characterMng_.Buttle();

            // �L�����N�^�[�̍U���Ώۂ��Ō�̓G��������
            if (lastEnemyFlg_)
            {
                if (enemyInstanceMng_.AllAnimationFin() && !resultFlg_)
                {
                    // ���݂������퓬����������
                    if (FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE)
                    {
                        // ���X�g�̒���bool������true(=�擾�ς�)�ɂ���
                        var tmp = FieldMng.forcedButtleWallList;
                        for (int i = 0; i < tmp.Count; i++)
                        {
                            if (tmp[i].Item1 == forcedButtleWallName)
                            {
                                // flag��true�ɏ㏑������
                                (string, bool) content = (forcedButtleWallName, true);
                                tmp[i] = content;
                            }
                        }
                    }

                    CallDeleteEnemy();

                    // ���U���g�����F �G�l�~�[�̐��A�G�l�~�[�̔ԍ��i�z��j
                    buttleResult_.DropCheck(EneSelObj_.childCount, saveEnemyNum_, bossFlag_);
                    resultFlg_ = true;
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
        if (++moveTurnCnt_ > moveTurnList_.Count - 1)
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

    public void SetBadStatus(int sub1, int sub2)
    {
        badStatusNum_ = (sub1, sub2);
    }

    public (int, int) GetBadStatus()
    {
        return badStatusNum_;
    }

    public void SetRefEnemyNum(int num)
    {
        refNum_ = num;
    }

    // ���j�����������鏈����퓬�I�����Ɏg�p����
    public void CallDeleteEnemy()
    {
        enemyInstanceMng_.DeleteEnemy();
        if (EneSelObj_ == null)
        {
            EneSelObj_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").transform;
        }
        // EnemySelectObj���炻�̐퓬�ł������G��HP�o�[�Ƃ����폜����
        for (int i = 0; i < EneSelObj_.childCount; ++i)
        {
            Destroy(EneSelObj_.GetChild(i).gameObject);
        }

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            // �o�g���Ŏ��S�����܂܏I�����Ă����Ƃ��́AHP1�̏�Ԃŗ����オ�点��
            if (SceneMng.charasList_[i].GetDeathFlg())
            {
                SceneMng.charasList_[i].SetDeathFlg(false);
                SceneMng.charasList_[i].SetHP(1);
            }
            SceneMng.charasList_[i].ButtleInit();
        }
    }

    // �A�C�e���ɂ��Œ�_���[�W
    public void ItemDamage(int itemDamageNum)
    {
        SetDamageNum(itemDamageNum);    // �_���[�W�l���Z�b�g
        enemyInstanceMng_.ItemDamage(); // �S�G�l�~�[��HPdecrease�֐����񂷂悤�ɂ���
    }

    public Vector3 GetFieldPos()
    {
        return keepPos_;
    }

    public void SetEnemyNum(int[] num, bool flag)
    {
        saveEnemyNum_ = num;
        bossFlag_ = flag;
    }

    public void SetFieldPos(Vector3 pos)
    {
        keepPos_ = pos;
    }

    public void SetAutoHit(bool flag)
    {
        autoHitFlg_ = flag;
    }

    public bool GetAutoHit()
    {
        return autoHitFlg_;
    }

    public void OnClickItemBackButton()
    {
        // �A�C�e����ʂ����
        GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
        buttleUICanvas.transform.Find("ItemBackButton").gameObject.SetActive(false);
    }

    public void SetIsAttackMagicFlg(bool flag)
    {
        isAttackMagicFlg_ = flag;
    }

    public bool GetIsAttackMagicFlg()
    {
        return isAttackMagicFlg_;
    }
}
