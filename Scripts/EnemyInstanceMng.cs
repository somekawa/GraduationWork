using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharaBase;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject enemyInstancePointPack;   // �G�̏o���ʒu��ݒ肵������
    public GameObject enemyTest;                // �e�X�g�p�̓G
    public GameObject enemyHPBar;               // �G�p��HP�o�[
    public EnemySelect enemySelectObj;          // �G�I��p�A�C�R��
                                                // �ʏ�U���e�̃v���n�u
    [SerializeField]
    private GameObject enemyAttackPrefab_;     // ���j�̒ʏ�U���Ɠ������̂Ńe�X�g����
    [SerializeField]
    private GameObject soulEffect_;             // �G���S���̍��I�ȉ���(�G�t�F�N�g)

    // �L�[��int , value��List Vector3��[�e���[�v�|�C���g�̐�]��map����������
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();
    private Vector3 enemyHPPosOffset_;          // �����ʒu�̓G�p�ɃI�t�Z�b�g�ł���悤�ɂ��Ă���
    private Dictionary<int, GameObject> enemyMap_ = new Dictionary<int, GameObject>();

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;        // �t�B�[���h���̓G����ۑ�����

    public static List<(Enemy,HPBar)> enemyList_ = new List<(Enemy, HPBar)>();   // Enemy.cs���L�������Ƀ��X�g������

    private ButtleMng buttleMng_;
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;
    private int mapNum_ = 0;                    // �}�b�v��ɔz�u�����G�̐�
    private Vector3 enemyPos_;
    private Vector3 charaPos_;
    private int attackTarget_ = -1;             // �U���Ώ�(�G����L������)

    // Item1:�C�x���g�ɂ�苭���퓬����ۂ̓G�̎��,Item2:�G�̐�
    private (GameObject, int) eventEnemy_ = (null, -1);

    private BoxCollider changeEnableBoxCollider_;

    void Start()
    {
        // HP�o�[�\���ʒu�̐ݒ�
        // 1��
        Vector3[] tmp1 = new Vector3[1];
        tmp1[0] = new Vector3(1000.0f, 530.0f);
        enemyHPPos_[1] = tmp1;
        // 2��
        Vector3[] tmp2 = new Vector3[2];
        tmp2[0] = new Vector3(870.0f, 560.0f);
        tmp2[1] = new Vector3(1030.0f, 530.0f);
        enemyHPPos_[2] = tmp2;
        // 3��
        Vector3[] tmp3 = new Vector3[3];
        tmp3[0] = new Vector3(780.0f, 570.0f);
        tmp3[1] = new Vector3(940.0f, 540.0f);
        tmp3[2] = new Vector3(1150.0f, 510.0f);
        enemyHPPos_[3] = tmp3;
        // 4��
        Vector3[] tmp4 = new Vector3[4];
        tmp4[0] = new Vector3(760.0f,  580.0f);
        tmp4[1] = new Vector3(830.0f,  540.0f);
        tmp4[2] = new Vector3(1000.0f, 520.0f);
        tmp4[3] = new Vector3(1170.0f, 470.0f);
        enemyHPPos_[4] = tmp4;

        // EnemyInstancePointPack�̎q�ŉ�
        foreach (Transform childTransform in enemyInstancePointPack.gameObject.transform)
        {
            // ���X�g�ɍ��W���ꎞ�ۑ��ł���悤�ɂ���
            List<Vector3> posList = new List<Vector3>();

            // EnemyInstancePointPack�̑��ŉ�
            foreach (Transform grandChildTransform in childTransform)
            {
                // ���X�g�ɍ��W��ۑ����Ă���
                posList.Add(grandChildTransform.gameObject.transform.position);
            }

            // �ꎞ�ۑ����Ă������W���X�g���A�}�b�v�֑������
            enemyPosSetMap_[int.Parse(childTransform.name)] = posList;
        }


        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������

        // �G(�������t�B�[���h�ɂ���ď�����������悤�ɂ��Ƃ��Ȃ��Ƃ����Ȃ�)
        if (enemyData_ == null)
        {
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, 1, name);
        }

        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();

        // �����̒l�̌��ɂȂ�l(=�V�[�h�l)�����݂̎��Ԃ������ď���������
        // ���V�[�h�l��ύX���Ȃ���΋K���I�ɓ������Ԃœ����ԍ�����������Ă��܂�����
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    // ButtleMng.cs��Update�֐��̂悤�Ɏg�p����
    public void Buttle(int num)
    {
        if(anim_ == ANIMATION.NON)
        {
            anim_ = ANIMATION.BEFORE;
        }

        // AnimMax�̔����̒l�ɓ��B������true�ɂ���
        if (enemyList_[num].Item1.HalfAttackAnimTime())
        {
            if(changeEnableBoxCollider_ != null)
            {
                changeEnableBoxCollider_.enabled = true;
            }
        }

        if (anim_ == ANIMATION.ATTACK && enemyList_[num].Item1.ChangeNextChara())
        {
            anim_ = ANIMATION.AFTER;
        }

        if (oldAnim_ != anim_)
        {
            oldAnim_ = anim_;
            AnimationChange(num);
        }
    }

    private void BeforeAttack(int num)
    {
        // ���łɎ��S�t���O�������Ă�����return����
        if (!enemyMap_[num + 1].activeSelf || enemyList_[num].Item1.GetDeathFlg())
        {
            // ���S���Ă��邩���\���ɂ���Ă���
            anim_ = ANIMATION.IDLE;
            return;
        }

        // �����̈ʒu���擾����
        enemyPos_ = enemyPosSetMap_[mapNum_][num];

        // �����_���ȃL�������擾����(�������A���S�����L�����͏��O����)
        //do
        //{
        //    attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);    // ���j�ȏ�MAX�����őI��
        //} while (SceneMng.charasList_[attackTarget_].HP() <= 0);
        attackTarget_ = (int)SceneMng.CHARACTERNUM.UNI;

        // �_���[�W�Ƒ��x��n��
        buttleMng_.SetDamageNum(enemyList_[num].Item1.Damage());
        buttleMng_.SetSpeedNum(enemyList_[num].Item1.Speed());
        buttleMng_.SetLuckNum(enemyList_[num].Item1.Luck());

        charaPos_ = SceneMng.charasList_[attackTarget_].GetButtlePos();

        Debug.Log("�U���Ώۂ�" + (SceneMng.CHARACTERNUM)attackTarget_);

        // �s�����̓G���A�U���Ώۂ̕����ɑ̂�������
        enemyMap_[num + 1].transform.localRotation = Quaternion.LookRotation(charaPos_ - enemyPos_);


        // �������U���^�̓G���g�����@�̒e����
        if (enemyList_[num].Item1.MoveTime() < 0)
        {
            GameObject obj = null;
            // enemyAttackPrefab_�̎q����A����̓G���g�p����G�t�F�N�g�𖼑O�ŒT���o��
            for (int m = 0; m < enemyAttackPrefab_.transform.childCount; m++)
            {
                // Excel���ł�(Clone)�܂ŕt���Ė��O�ۑ����Ă邩�疼�O+(Clone)�Ƃ���
                var name = enemyAttackPrefab_.transform.GetChild(m).gameObject.name + "(Clone)";
                if (name == enemyList_[num].Item1.WeaponTagObjName())
                {
                    // ����g�p����G�t�F�N�g�����������Ƃ��́A���̃G�t�F�N�g��obj�ϐ��ɕۑ�����
                    obj = enemyAttackPrefab_.transform.GetChild(m).gameObject;
                    break;
                }
            }

            // �������U���^�̓G���g�����@�̒e����
            if (enemyList_[num].Item1.MoveTime() < 0)
            {
                // �ʏ�U���e�̕����̌v�Z
                var dir = (charaPos_ - enemyPos_).normalized;
                // �G�t�F�N�g�̔����ʒu��������
                // �L�����ƃG�t�F�N�g�̔����������t������Aforward�̌��Z�ɋC��t���鎖(Z���̓}�C�i�X���Ȃ��Ƃ���)
                var adjustPos = new Vector3(enemyPos_.x, enemyPos_.y + 0.3f, enemyPos_.z - transform.forward.z);

                // �ʏ�U���e�v���n�u���C���X�^���X��(�ۑ����Ă���obj�ϐ��̃I�u�W�F�N�g���g�p����)
                var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity);
                MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
                // �ʏ�U���e�̔��ł����������w��
                magicMove.SetDirection(dir);
            }
        }

        // [Weapon]�̃^�O�����Ă���I�u�W�F�N�g��S�Č�������
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        int tryParseNum = -1;
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // �������I�u�W�F�N�g�̖��O���r���āA����U���Ɉ�������ɂ��Ă���CheckAttackHit�֐��̐ݒ���s��
            if (weaponTagObj[i].name == enemyList_[num].Item1.WeaponTagObjName())
            {
                // int�^�ɕϊ��ł������m�F����
                bool checkChangeInt = int.TryParse(weaponTagObj[i].transform.root.gameObject.name, out tryParseNum);

                if(checkChangeInt)
                {
                    // �ϊ����ł���(�ߋ����U���^)
                    // ���������̂������ł��邩�A�e�̔ԍ������Ĕ��肷��
                    if (tryParseNum == (num + 1))
                    {
                        changeEnableBoxCollider_ = weaponTagObj[i].GetComponent<BoxCollider>();

                        // �U���Ώۂ̃L�����̔ԍ���n��
                        weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_);
                    }
                }
                else
                {
                    // �ϊ����o���Ȃ�����(�������U���^)
                    // �U���Ώۂ̃L�����̔ԍ���n��
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_);
                }
            }
        }

        // �U���ΏۂɌ������đ��鏈��
        StartCoroutine(MoveToPlayerPos(num));
    }

    // �U���ւ̈ړ��R���[�`��  
    private IEnumerator MoveToPlayerPos(int num)
    {
        bool flag = false;

        // �������U���^�̓G�͈ړ����K�v�Ȃ�����
        if (enemyList_[num].Item1.MoveTime() < 0)
        {
            flag = true;
        }

        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / enemyList_[num].Item1.MoveTime();  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = enemyList_[num].Item1.RunMove(time, enemyMap_[num + 1].transform.localPosition, charaPos_);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            enemyMap_[num + 1].transform.localPosition = tmp.Item1;     // ���W��������
        }

        anim_ = ANIMATION.ATTACK;    // �U�����[�V�����ڍs�m�F�ؑ�

    }

    // �G�̃C���X�^���X����(�z�u�|�W�V������ButtleMng.cs�Ŏw��ł���悤�Ɉ�����p�ӂ��Ă���)
    public int EnemyInstance(int mapNum,Canvas parentCanvas)
    {
        // �C�x���g�p�̕ϐ���1�ȏ�̒l�������ꍇ�A�C�x���g�p���l��D�悷��
        if (eventEnemy_.Item2 >= 1)
        {
            mapNum_ = eventEnemy_.Item2;
        }
        else
        {
            mapNum_ = mapNum;
        }

        // ����g�p�O�ɏ���������
        enemyList_.Clear(); 
        enemyMap_.Clear();

        int num = 1;
        // �w�肳�ꂽ�}�b�v�̃��X�g�����o���āAforeach���ŉ�
        foreach(Vector3 pos in enemyPosSetMap_[mapNum_])
        {
            // �G�v���n�u���C���X�^���X
            GameObject enemy = null;
            enemyHPPosOffset_ = Vector3.zero;

            // �ԍ��łǂ̓G���C���X�^���X���邩���߂�
            int enemyNum = Random.Range(0, enemyTest.transform.childCount);
            enemyNum = 3;   // �C�[�O���Œ�

            if (eventEnemy_.Item1 == null)
            {
                // �t�B�[���h��̓G�������_���ŏo��
                // �q�̏��ƃG�N�Z���ԍ������킹��K�v������
                enemy = Instantiate(enemyTest.transform.GetChild(enemyNum).gameObject,
                                    pos, Quaternion.identity) as GameObject;
                // �G�̑̂̌�����ς���
                enemy.transform.Rotate(0, 180, 0);

                // �G��Eagle�̂Ƃ��́AHP�o�[�̍�������̂ق��ɒ������Ȃ��Ƃ����Ȃ�
                if(enemyTest.transform.GetChild(enemyNum).gameObject.name == "Enemy_Eagle")
                {
                    enemyHPPosOffset_ = new Vector3(0.0f, 60.0f,0.0f);
                }

            }
            else
            {
                // �C�x���g�p�̓G���o��
                enemy = Instantiate(eventEnemy_.Item1, pos, Quaternion.identity) as GameObject;
                // �C�x���g�p�̓G�̔ԍ�������enemyNum�Ƃ��ēK�p����(�G�N�Z���ԍ������킹��K�v������)
                enemyNum = int.Parse(eventEnemy_.Item1.name.Split('_')[1]);
            }

            enemy.name = num.ToString();

            // �GHP���C���X�^���X
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum_][num - 1] + enemyHPPosOffset_, Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]��x�͏o��������G�̍s�ԍ�
            // Animator���A�^�b�`����Ă��邩�m�F(�J�{�X�͂Ȃ�����null�������)
            if(enemy.GetComponent<Animator>() == null)
            {
                enemyList_.Add((new Enemy(num.ToString(), 1, null, enemyData_.param[enemyNum]), hpBar.GetComponent<HPBar>()));
            }
            else
            {
                enemyList_.Add((new Enemy(num.ToString(), 1, enemy.GetComponent<Animator>(), enemyData_.param[enemyNum]), hpBar.GetComponent<HPBar>()));
            }

            // �GHP�̐ݒ�
            enemyList_[num - 1].Item2.SetHPBar(enemyList_[num - 1].Item1.HP(), enemyList_[num - 1].Item1.MaxHP());

            // �G�I�u�W�F�N�g��ϐ��ɓ����
            enemyMap_.Add(num, enemy);

            num++;
        }

        // 1�x�ǂݍ��񂾂�eventEnemy_��null�ɂ���
        if(eventEnemy_.Item1)
        {
            eventEnemy_ = (null, -1);
        }

        return mapNum_;
    }

    void AnimationChange(int num)
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:
                // �L���ɂ��Ă����_���[�W�p�R���C�_�[��false�ɖ߂�
                if(changeEnableBoxCollider_ != null)
                {
                    changeEnableBoxCollider_.enabled = false;
                }

                // �s�����I���������^�[�����ڂ�
                buttleMng_.SetMoveTurn();
                anim_ = ANIMATION.NON;
                break;
            case ANIMATION.BEFORE:
                BeforeAttack(num);    // �U������
                break;
            case ANIMATION.ATTACK:    // ���ۂ̍U������
                Attack(num);
                break;
            case ANIMATION.AFTER:
                AfterAttack(num);        // �U���I���� 
                break;
            //case ANIMATION.DEATH:
            //    break;
            default:
                break;
        }
    }

    public (int,string) EnemyTurnSpeed(int num)
    {
        return (enemyList_[num].Item1.Speed(), enemyList_[num].Item1.Name());
    }

    private void Attack(int num)
    {
        enemyList_[num].Item1.Attack();
    }

    void AfterAttack(int num)
    {
        // �������ʒu�ɖ߂鏈��
        StartCoroutine(MoveToInitPos(num));
    }

    // �U������߂��Ă���R���[�`��  
    private IEnumerator MoveToInitPos(int num)
    {
        bool flag = false;

        // �������U���^�̓G�͈ړ����K�v�Ȃ����� �܂��́A�U���㎩������HP��0�̓G�͈ړ����K�v�Ȃ�
        if (enemyList_[num].Item1.MoveTime() < 0 || enemyList_[num].Item1.HP() <= 0)
        {
            flag = true;
        }

        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / (enemyList_[num].Item1.MoveTime() / 2.0f);  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = enemyList_[num].Item1.BackMove(time, enemyMap_[num + 1].transform.localPosition, enemyPosSetMap_[mapNum_][num]);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            enemyMap_[num + 1].transform.localPosition = tmp.Item1;     // ���W��������
        }

        anim_ = ANIMATION.IDLE;

    }

    // CharacterMng.cs�ɍ��W����n��
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }

    public void HPdecrease(int num)
    {
        var damage = 0;

        // �N���e�B�J���̌v�Z������(��b�l�ƍK�^�l�ŏ�������߂�)
        int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
        if (criticalRand <= 10 + buttleMng_.GetLuckNum())
        {
            // �N���e�B�J������(�K��+�_���[�W2�{)10�̓N���e�B�J���̊�b�l
            Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "�Ȃ̂ŁA�L�����̍U�����N���e�B�J���I");
            // �N���e�B�J���_���[�W
            damage = (buttleMng_.GetDamageNum() * 2) - enemyList_[num].Item1.Defence();
        }
        else
        {
            // �N���e�B�J������Ȃ��Ƃ�
            Debug.Log(criticalRand + ">" + (10 + buttleMng_.GetLuckNum()) + "�Ȃ̂ŁA�L�����̍U���̓N���e�B�J���ł͂Ȃ�");

            // �����v�Z������
            // �@�U�����鑤��Speed / �U������鑤��Speed * 100 = ���̏o��
            var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)enemyList_[num].Item1.Speed() * 100.0f);
            // �A�L�������G��+10���̕␳�l�����ăL������������(������Luck * 5)�����v���X����B
            var hitProbabilityOffset = hitProbability + 10 + (buttleMng_.GetLuckNum() * 5);
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
        }

        // �_���[�W�l�̎Z�o
        damage = buttleMng_.GetDamageNum() - enemyList_[num].Item1.Defence();
        if(damage <= 0)
        {
            Debug.Log("�L�����̍U���͂��G�̖h��͂��������̂Ń_���[�W��0�ɂȂ�܂���");
            damage = 0;
        }

        StartCoroutine(enemyList_[num].Item2.MoveSlideBar(enemyList_[num].Item1.HP() - damage));
        // �������l�̕ύX���s��
        enemyList_[num].Item1.sethp(enemyList_[num].Item1.HP() - damage);

        // ����HP��0�ɂȂ�����A�I�u�W�F�N�g���폜����
        if(enemyList_[num].Item1.HP() <= 0)
        {
            // �����Ŗ�󏈗����Ăяo��(HP������������󏈗��Ƃ��Ȃ��ƁAHP��0�̏ꏊ�ɖ�󂪏o�Ă��܂�����)
            enemySelectObj.ResetSelectPoint();

            // �G�I�u�W�F�N�g���폜����(�^�O����)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (int.Parse(obj.name) == num + 1)
                {
                    // �G�t�F�N�g�̐���
                    //Instantiate(soulEffect_, enemyMap_[num + 1].transform.position, Quaternion.identity);
                    //Destroy(obj);   // �G�̍폜

                    // ���S�p�̃t���O�𗧂Ă�
                    enemyList_[num].Item1.SetDeathFlg(true);
                }
            }

            Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HP�o�[�̍폜
        }
    }

    public void SetEnemySpawn(GameObject obj,int num)
    {
        eventEnemy_ = (obj, num);
    }

    public void NotMyTurn()
    {
        // �_���[�W���󂯂��Ƃ��̃��[�V�������K�莞�ԂŏI��������
        for (int i = 0; i < enemyMap_.Count; i++)
        {
            enemyList_[i].Item1.DamageAnim();
        }

        // �G�̎��S����
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // ���łɔ�\���ɂ��ꂽ���͔̂�΂�
            if(obj.activeSelf)
            {
                if (enemyList_[int.Parse(obj.name) - 1].Item1.GetDeathFlg() && enemyList_[int.Parse(obj.name) - 1].Item1.DeathAnim())
                {
                    // �G�t�F�N�g�̐���
                    Instantiate(soulEffect_, enemyMap_[int.Parse(obj.name)].transform.position, Quaternion.identity);

                    // �����ɍ폜�����ɁA��\���ؑւɂ���
                    obj.SetActive(false);
                }
            }
        }
    }

    public void DeleteEnemy()
    {
        // �G�̍폜����
        for(int i = 0; i < enemyMap_.Count; i++)
        {
            Destroy(enemyMap_[i + 1]);
        }
    }

    public bool AllAnimationFin()
    {
        for (int i = 0; i < enemyMap_.Count; i++)
        {
            // �܂�1�̂ł����S�A�j���[�V�������Ȃ�false�ŕԂ�
            if(enemyMap_[i + 1].activeSelf)
            {
                return false;
            }
        }

        // Finish�^�O�����Ă�����̂��������āA����Ƃ���false�ŕԂ�
        if (GameObject.FindGameObjectsWithTag("Finish").Length > 0)
        {
            return false;
        }


        // �S�Ă̓G�����S�A�j���[�V�����܂ŏI��������true�ŕԂ�
        return true;
    }
}
