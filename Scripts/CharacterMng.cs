using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharaBase;
using static SceneMng;

// �T����/�퓬����킸�A�L�����N�^�[�Ɋ֘A������̂��Ǘ�����

// Chara.cs���C���X�^���X����Ƃ��ɊO���f�[�^�̃L�����f�[�^�����̑O�ɓǂݍ���ł����āAnew�̈����ɓ���ēn���悤�ɂ���
// ����������A�e�L�����ɂ��ꂼ��̃X�e�[�^�X�l��n����B�͂��B���Ԃ�B�B�B

public class CharacterMng : MonoBehaviour
{
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;

    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public GameObject buttleWarpPointPack;  // �퓬���Ƀt�B�[���h��̐퓬�|�C���g�ɃL���������[�v������

    //�@�ʏ�U���e�̃v���n�u
    [SerializeField]
    private GameObject uniAttackPrefab_;

    CHARACTERNUM oldTurnChar_ = CHARACTERNUM.UNI;     // �O�ɍs����������Ă��Ă����L�����N�^�[
    CHARACTERNUM nowTurnChar_ = CHARACTERNUM.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[
    private bool selectFlg_ = false;                  // �G��I�𒆂��̃t���O
    private bool lastEnemytoAttackFlg_ = false;       // �L�����̍U���Ώۂ��Ō�̓G�ł��邩     

    private Vector3 keepFieldPos_;                    // �퓬�ɓ��钼�O�̃L�����̍��W��ۑ����Ă��� 
    private const int buttleCharMax_ = 2;             // �o�g���Q���\�L�������̍ő�l(�ŏI�I�ɂ�3�ɂ���)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // �퓬���̔z�u�ʒu��ۑ����Ă����ϐ�
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // �퓬���̉�]�p�x��ۑ����Ă����ϐ�(�N�H�[�^�j�I��)

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CHARACTERNUM, GameObject> charMap_;
    // Chara.cs���L�������Ƀ��X�g������
    private List<Chara> charasList_ = new List<Chara>();
    // �e�L������HP���
    private Dictionary<CHARACTERNUM, HPBar> charHPMap_ = new Dictionary<CHARACTERNUM, HPBar>();

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // �o�g�����̈ē�
    private readonly string[] announceText_ = new string[2] { " ���V�t�g�L�[�F\n �퓬���瓦����", " T�L�[�F\n �R�}���h�֖߂�" };

    private ImageRotate buttleCommandRotate_;                     // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�
    private EnemySelect buttleEnemySelect_;                       // �o�g�����̑I���A�C�R�����
    private ButtleMng buttleMng_;                                 // ButtleMng.cs�̎擾

    private int enemyNum_ = 0;                                    // �o�g�����̓G�̐�
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // �G�̃C���X�^���X�ʒu�̑S���

    private Vector3 charaPos_;                         // �L�����N�^�[���W
    private Vector3 enePos_;                           // �ڕW�̓G���W

    void Start()
    {
        // SceneMng����L�����̏������炤(charMap_��charasList_)
        charMap_ = SceneMng.charMap_;
        charasList_ = SceneMng.charasList_;

        // �e�X�g�p
        //charasList_[0].sethp(charasList_[0].HP() - 10);
        //Debug.Log("HHHPPP"+charasList_[0].HP());
        //SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);

        nowTurnChar_ = CHARACTERNUM.UNI;

        // ���[�v�|�C���g�̐��Ԃ�Afor������
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // �ʂɃ��[�v�|�C���g��ϐ��֕ۑ����Ă���
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleAnounceText_ = buttleUICanvas.transform.Find("AnnounceText").GetComponent<TMPro.TextMeshProUGUI>();

        buttleCommandRotate_ = buttleUICanvas.transform.Find("Command").transform.Find("Image").GetComponent<ImageRotate>();
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();

        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
        //@ �L������+CharaData�̃X�e�[�^�X�\����AHP�����擾����
        charHPMap_[CHARACTERNUM.UNI] = buttleUICanvas.transform.Find("UniCharaData/HPSlider").GetComponent<HPBar>();
        charHPMap_[CHARACTERNUM.JACK] = buttleUICanvas.transform.Find("JackCharaData/HPSlider").GetComponent<HPBar>();
        //@ ����HP����
        charHPMap_[CHARACTERNUM.UNI].SetHPBar(charasList_[(int)CHARACTERNUM.UNI].HP(), charasList_[(int)CHARACTERNUM.UNI].MaxHP());
        charHPMap_[CHARACTERNUM.JACK].SetHPBar(charasList_[(int)CHARACTERNUM.JACK].HP(), charasList_[(int)CHARACTERNUM.JACK].MaxHP());
    }

    // ButtleMng.cs����G�̐����󂯎��
    public void SetEnemyNum(int enemyNum)
    {
        enemyNum_ = enemyNum;

        // ���A�C�R�����\���ł���悤�ɍ��W��n��
        // �ꎞ�ϐ��ɔ����ʒu���R�s�[���Ă���������邱�ƂŁA�G�̔����ʒu�̍���������������̂�h��
        List<Vector3> tmpInsPos = new List<Vector3>(enemyInstancePos_[enemyNum_]);
        buttleEnemySelect_.SetPosList(tmpInsPos);

        // NG�ȏ�����
        // ���̏������ł́A���̓G�̔����ʒu���W������������`�Ŗ��A�C�R������������āA2��ڈȍ~�G�̔����ʒu�����A�C�R���̍����ɂȂ��Ă��܂�
        //buttleEnemySelect_.SetPosList(enemyInstancePos_[enemyNum_]);
    }

    // �퓬�J�n���ɐݒ肳��鍀��(ButtleMng.cs�ŎQ��)
    public void ButtleSetCallOnce()
    {
        if (buttleUICanvas.gameObject.activeSelf)
        {
            buttleCommandRotate_.ResetRotate();   // UI�̉�]����ԍŏ��ɖ߂�
        }

        anim_ = ANIMATION.IDLE;
        oldAnim_ = ANIMATION.IDLE;

        buttleAnounceText_.text = announceText_[0];

        // �ŏ��̍s���L�������w�肷��
        nowTurnChar_ = CHARACTERNUM.UNI;

        // �ŏ��̍s���L������HP�o�[��\������
        //charaHPBar.SetHPBar(charasList_[(int)nowTurnChar_].HP(), charasList_[(int)nowTurnChar_].MaxHP());

        // �t���O�̏��������s��
        lastEnemytoAttackFlg_ = false;

        // �퓬�O�̍��W��ۑ����Ă���
        keepFieldPos_ = charMap_[CHARACTERNUM.UNI].gameObject.transform.position;

        // �퓬�p���W�Ɖ�]�p�x��������
        // �L�����̊p�x��ύX�́AButtleWarpPoint�̔��̊p�x����]������Ɖ\�B(1��1�̌�����ς��邱�Ƃ��ł���)
        foreach (KeyValuePair<CHARACTERNUM, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // �����ō��W��ۑ����Ă������ƂŁA���j���[��ʂł̕��ёւ��ł����f�ł��邾�낤���A
            // �U���G�t�F�N�g�̔����ʒu�̖ڈ��ɂȂ�
            //charSetting[(int)character.Key].buttlePos  = character.Value.gameObject.transform.position;
            charasList_[(int)character.Key].SetButtlePos(character.Value.gameObject.transform.position);

            // �s�����Ɋ֘A����l������������
            charasList_[(int)character.Key].SetTurnInit();
        }
    }

    public (int, string) CharaTurnSpeed(int num)
    {
        return (charasList_[num].Speed(), charasList_[num].Name());
    }

    // �L�����̐퓬���Ɋւ��鏈��(ButtleMng.cs�ŎQ��)
    public void Buttle()
    {
        //Debug.Log(anim_);

        // ���S���Ă�����
        if (charasList_[(int)nowTurnChar_].GetDeathFlg())
        {
            if (charHPMap_[nowTurnChar_].GetColFlg())
            {
                return;
            }
            else
            {
                oldAnim_ = anim_;
                Debug.Log("���S��������s�����΂�");
                anim_ = ANIMATION.IDLE;
                oldAnim_ = ANIMATION.NON;

                // �S�ł������m�F���鏈��
                bool allDeathFlg = true;
                for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                {
                    if (!charasList_[i].GetDeathFlg())
                    {
                        // 1�l�ł�������Ԃ��ƕ������break���Ĕ�����
                        allDeathFlg = false;
                        break;
                    }
                }
                // �S�Ŏ��͒����̉Ƃ֔�΂�
                if (allDeathFlg)
                {
                    EventMng.SetChapterNum(100, SCENE.CONVERSATION);
                }
            }

            //if (charaHPBar.GetColFlg())
            //{
            //    return;
            //}
            //else
            //{
            //    oldAnim_ = anim_;
            //    Debug.Log("���S��������s�����΂�");
            //    anim_ = ANIMATION.IDLE;
            //    oldAnim_ = ANIMATION.NON;

            //    // �S�ł������m�F���鏈��
            //    bool allDeathFlg = true;
            //    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            //    {
            //        if (!charasList_[i].GetDeathFlg())
            //        {
            //            // 1�l�ł�������Ԃ��ƕ������break���Ĕ�����
            //            allDeathFlg = false;
            //            break;
            //        }
            //    }
            //    // �S�Ŏ��͒����̉Ƃ֔�΂�
            //    if (allDeathFlg)
            //    {
            //        EventMng.SetChapterNum(100, SCENE.CONVERSATION);
            //    }
            //}
        }

        // �e�X�g�p(���x���A�b�v����)
        if (Input.GetKeyDown(KeyCode.L))
        {
            charasList_[0].LevelUp();
            charasList_[1].LevelUp();
        }


        // �퓬���瓦����
        if (!selectFlg_ && Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �G�I�u�W�F�N�g���폜����(�^�O����)
            //foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            //{
            //    Destroy(obj);
            //}

            buttleMng_.CallDeleteEnemy();

            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            charMap_[CHARACTERNUM.UNI].gameObject.transform.position = keepFieldPos_;

            Debug.Log("Uni�͓����o����");
        }

        // ATTACK�œG�I�𒆂ɁA����̃L�[(����T�L�[)���������ꂽ��R�}���h�I���ɖ߂�
        if (selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            anim_ = ANIMATION.NON;
            selectFlg_ = false;
            buttleCommandRotate_.SetRotaFlg(!selectFlg_);   // �R�}���h��]��L����
            buttleAnounceText_.text = announceText_[0];
        }

        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �I�����ꂽ�R�}���h�ɑ΂��鏈��
            switch (buttleCommandRotate_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:
                    if (!selectFlg_)
                    {
                        // �����̍s���̑O�̐l������I����Ă��邩���ׂ�
                        if (!charasList_[(int)oldTurnChar_].GetIsMove())
                        {
                            if (anim_ == ANIMATION.IDLE || anim_ == ANIMATION.NON)
                            {
                                anim_ = ANIMATION.BEFORE;
                            }
                        }
                        else
                        {
                            Debug.Log("�O�̃L�������A�j���[�V������");
                        }
                    }
                    else
                    {
                        if (anim_ == ANIMATION.BEFORE)
                        {
                            BeforeAttack((int)nowTurnChar_);    // �U������
                        }
                    }

                    break;
                case ImageRotate.COMMAND.MAGIC:
                    Debug.Log("���@�R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.ITEM:
                    Debug.Log("�A�C�e���R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.BARRIER:
                    // ���̎����̃^�[���܂Ŗh��͂�1.5�{�ɂ���
                    charasList_[(int)nowTurnChar_].SetBarrierNum(charasList_[(int)nowTurnChar_].Defence() / 2);
                    // ���̃L����or�G�ɍs�������悤��anim_��oldAnim_��ݒ肷��
                    anim_ = ANIMATION.IDLE;
                    oldAnim_ = ANIMATION.NON;

                    Debug.Log("�h��R�}���h���L���R�}���h�ł�");
                    break;
                default:
                    Debug.Log("�����ȃR�}���h�ł�");
                    break;
            }
        }

        if (oldAnim_ != anim_)
        {
            oldAnim_ = anim_;
            AnimationChange();
        }

        if (anim_ == ANIMATION.ATTACK && charasList_[(int)nowTurnChar_].ChangeNextChara())
        {
            anim_ = ANIMATION.AFTER;
        }
    }

    public void NotMyTurn()
    {
        // �_���[�W���󂯂��Ƃ��̃��[�V�������K�莞�ԂŏI��������
        for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            charasList_[i].DamageAnim();
        }
    }

    void AnimationChange()
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:

                // �O�̃L�����̍s�����I���������^�[�����ڂ�
                buttleMng_.SetMoveTurn();

                buttleAnounceText_.text = announceText_[0];

                // ���̃L�������s���ł���悤�ɂ���
                // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
                if (++nowTurnChar_ >= CHARACTERNUM.MAX)
                {
                    nowTurnChar_ = CHARACTERNUM.UNI;
                }

                if (charasList_[(int)nowTurnChar_].HP() <= 0)
                {
                    Debug.Log("�L���������S");
                    //anim_ = ANIMATION.DEATH;
                }
                else
                {
                    anim_ = ANIMATION.IDLE;
                }

                // ���̍s���L������HP�o�[��\������
                //charaHPBar.SetHPBar(charasList_[(int)nowTurnChar_].HP(), charasList_[(int)nowTurnChar_].MaxHP());

                // �h��p�̒l��0�ɖ߂�
                charasList_[(int)nowTurnChar_].SetBarrierNum();

                selectFlg_ = false;

                // ���ʒu�̃��Z�b�g���s��(false�Ȃ�A�G��S�ē|�����Ƃ������ƂȂ̂Ńt���O��؂�ւ���)
                lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();

                break;
            case ANIMATION.BEFORE:
                oldTurnChar_ = nowTurnChar_;

                Debug.Log("�O�̃L�������s���I��");
                selectFlg_ = true;
                buttleAnounceText_.text = announceText_[1];

                buttleCommandRotate_.SetRotaFlg(false);
                buttleEnemySelect_.SetActive(true);

                break;
            case ANIMATION.ATTACK:
                if (charasList_[(int)nowTurnChar_].Attack())
                {
                    // �����ŃL�����̍U���͂Ƒ��x�ƍK�^��ButtleMng.cs�ɓn��
                    buttleMng_.SetDamageNum(charasList_[(int)nowTurnChar_].Damage());
                    buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
                    buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

                    AttackStart((int)nowTurnChar_);
                    buttleCommandRotate_.SetRotaFlg(true);
                    buttleEnemySelect_.SetActive(false);
                }
                break;
            case ANIMATION.AFTER:
                AfterAttack((int)nowTurnChar_);
                break;
            //case ANIMATION.DEATH:
                //Debug.Log("���S��������s�����΂�");
                //anim_ = ANIMATION.IDLE;
                //break;
            default:
                break;
        }
    }

    // �U����������
    void BeforeAttack(int charNum)
    {
        // �L�����̈ʒu���擾����
        charaPos_ = charasList_[charNum].GetButtlePos();
        // �G�̈ʒu���擾����
        enePos_ = buttleEnemySelect_.GetSelectEnemyPos();
        enePos_.y = 0.0f;        // ������0.0f�ɂ��Ȃ��Ǝ΂ߏ�����ɔ��ł��܂�

        // �s�����̃L�������A�U���Ώۂ̕����ɑ̂�������
        // charMap_�̏��𒼐ڕύX����K�v�����邽�߁AcharMap_[nowTurnChar_]�ƋL�q���Ă���
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos_ - charaPos_);


        if (charNum == (int)CHARACTERNUM.JACK)
        {
            // �G�Ɍ������đ��鏈��
            StartCoroutine(MoveToEnemyPos());
        }
        else
        {
            anim_ = ANIMATION.ATTACK;    // ���j
        }

    }

    // �U���ւ̈ړ��R���[�`��  
    private IEnumerator MoveToEnemyPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = charasList_[(int)nowTurnChar_].RunMove(time, charMap_[nowTurnChar_].transform.localPosition, enePos_);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // �L�������W��������

            //Debug.Log("�W���b�N���ݒl" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.ATTACK;    // �U�����[�V�����ڍs�m�F�ؑ�

    }

    void AfterAttack(int charNum)
    {
        // �L�����̈ʒu���擾����
        charaPos_ = charasList_[charNum].GetButtlePos();

        if (charNum == (int)CHARACTERNUM.JACK)
        {
            // �������ʒu�ɖ߂鏈��
            StartCoroutine(MoveToInitPos());
        }
        else
        {
            anim_ = ANIMATION.IDLE;    // ���j
        }
    }

    // �U������߂��Ă���R���[�`��  
    private IEnumerator MoveToInitPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = charasList_[(int)nowTurnChar_].BackMove(time, charMap_[nowTurnChar_].transform.localPosition, buttleWarpPointsPos_[(int)CHARACTERNUM.JACK]);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // �L�������W��������

            //Debug.Log("�W���b�N���ݒl" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.IDLE;

    }


    void AttackStart(int charNum)
    {
        string str = "";

        if (charNum == (int)CHARACTERNUM.UNI)
        {
            // �ʏ�U���e�̕����̌v�Z
            var dir = (enePos_ - charaPos_).normalized;
            // �G�t�F�N�g�̔����ʒu��������
            var adjustPos = new Vector3(charaPos_.x, charaPos_.y + 0.5f, charaPos_.z);

            // �ʏ�U���e�v���n�u���C���X�^���X��
            var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);
            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // �ʏ�U���e�̔��ł����������w��
            magicMove.SetDirection(dir);

            // ���O�̐ݒ�
            str = "UniAttack(Clone)";
        }
        else if (charNum == (int)CHARACTERNUM.JACK)
        {
            // ���O�̐ݒ�
            str = "Axe1h";
        }
        else
        {
            return;  // �����������s��Ȃ�
        }

        if (str == "")
        {
            Debug.Log("�G���[�F�����������Ă��܂���");
            return; // �����������Ă��Ȃ��ꍇ��return����
        }

        // [Weapon]�̃^�O�����Ă���I�u�W�F�N�g��S�Č�������
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // �������I�u�W�F�N�g�̖��O���r���āA����U���Ɉ�������ɂ��Ă���CheckAttackHit�֐��̐ݒ���s��
            if (weaponTagObj[i].name == str)
            {
                // ����R���C�_�[�̗L����
                if (str == "Axe1h")
                {
                    weaponTagObj[i].GetComponent<BoxCollider>().enabled = true;
                }

                // �I�������G�̔ԍ���n��
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(buttleEnemySelect_.GetSelectNum() + 1);
            }
        }

    }

    // ButtleMng.cs�ŎQ��
    public bool GetLastEnemyToAttackFlg()
    {
        return lastEnemytoAttackFlg_;
    }

    // ButtleMng.cs�ŎQ��
    public bool GetSelectFlg()
    {
        return selectFlg_;
    }

    public void SetCharaFieldPos()
    {
        charMap_[CHARACTERNUM.UNI].gameObject.transform.position = keepFieldPos_;
    }

    public void HPdecrease(int num)
    {
        // �_���[�W�l�̎Z�o
        var damage = 0;

        // �N���e�B�J���̌v�Z������(��b�l�ƍK�^�l�ŏ�������߂�)
        int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
        if(criticalRand <= 10 + buttleMng_.GetLuckNum())
        {
            // �N���e�B�J������(�K��+�_���[�W2�{)10�̓N���e�B�J���̊�b�l
            Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "�Ȃ̂ŁA�G�̍U�����N���e�B�J���I");
            // �N���e�B�J���_���[�W
            damage = (buttleMng_.GetDamageNum() * 2) - charasList_[num].Defence();
        }
        else
        {
            // �N���e�B�J������Ȃ��Ƃ�
            Debug.Log(criticalRand + ">" + (10 + charasList_[(int)nowTurnChar_].Luck()) + "�Ȃ̂ŁA�G�̍U���̓N���e�B�J���ł͂Ȃ�");

            // �����v�Z������
            // �@�U�����鑤��Speed / �U������鑤��Speed * 100 = ���̏o��
            var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)charasList_[(int)nowTurnChar_].Speed() * 100.0f);
            // �A�L�������G��+10���̕␳�l������B
            var hitProbabilityOffset = hitProbability + 10;
            // �BhitProbabilityOffset��100�ȏ�Ȃ玩�������ŁA����ȉ��Ȃ烉���_���l�����B
            if (hitProbabilityOffset < 100)
            {
                int rand = Random.Range(0, 100);
                Debug.Log("������" + hitProbabilityOffset + "�����_���l" + rand);

                if (rand <= hitProbabilityOffset)
                {
                    // ����
                    Debug.Log(rand + "<=" + hitProbabilityOffset + "�Ȃ̂ŁA����");
                }
                else
                {
                    // ���
                    Debug.Log(rand + ">" + hitProbabilityOffset + "�Ȃ̂ŁA���");
                    return;
                }
            }
            else
            {
                Debug.Log("������" + hitProbabilityOffset + "��100�ȏ�Ȃ�̂ŁA��������");
            }

            int tmpLuck = 0;

            // �������ɂ�Luck�ŉ�𔻒������
            // ����͈̔͂́A100 - ���݂�Luck���ő�l�ɂ��āA����𐬌��ɋ߂Â���
            if (charasList_[(int)nowTurnChar_].Luck() <= 10)
            {
                tmpLuck = 10;
                Debug.Log("Luck��10�ȉ��Ȃ̂ŁA10��K�p���ĉ�𔻒�����܂�");
            }
            else
            {
                tmpLuck = charasList_[(int)nowTurnChar_].Luck();
                Debug.Log("Luck��10�ȏ�Ȃ̂ŁA���݂̃X�e�[�^�X��Luck���g���ĉ�𔻒�����܂�");
            }

            int randLuck = Random.Range(0, 100 - tmpLuck);
            if (randLuck <= tmpLuck)
            {
                Debug.Log(randLuck + "<=" + tmpLuck + "�ȉ��Ȃ̂ŁA��𐬌�");
                return;
            }
            else
            {
                Debug.Log(randLuck + ">" + tmpLuck + "�ȉ��Ȃ̂ŁA������s");
            }
        }

        // �ʏ�_���[�W
        damage = buttleMng_.GetDamageNum() - charasList_[num].Defence();
        if (damage <= 0)
        {
            Debug.Log("�G�̍U���͂��L�����̖h��͂��������̂Ń_���[�W��0�ɂȂ�܂���");
            damage = 0;
        }

        // �L������HP�����(�X���C�h�o�[�ύX)
        //StartCoroutine(charaHPBar.MoveSlideBar(charasList_[num].HP() - damage));
        StartCoroutine(charHPMap_[(CHARACTERNUM)num].MoveSlideBar(charasList_[num].HP() - damage));

        // �������l�̕ύX���s��
        charasList_[num].sethp(charasList_[num].HP() - damage);

        if (charasList_[num].HP() <= 0)
        {
            Debug.Log("�L���������S");
            charasList_[num].sethp(0);
            //anim_ = ANIMATION.DEATH;
            // Chara.cs�Ɏ��S��������
            charasList_[num].SetDeathFlg(true);
        }
    }
}
