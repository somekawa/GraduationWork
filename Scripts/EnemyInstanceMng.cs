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
    private GameObject kabosuAttackPrefab_;     // ���j�̒ʏ�U���Ɠ������̂Ńe�X�g����
    [SerializeField]
    private GameObject soulEffect_;             // �G���S���̍��I�ȉ���(�G�t�F�N�g)

    // �L�[��int , value��List Vector3��[�e���[�v�|�C���g�̐�]��map����������
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();
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
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, 0, name);
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
        if(enemyMap_[num + 1] == null)
        {
            // ���S���Ă��邩��Destroy����Ă���
            anim_ = ANIMATION.IDLE;
            return;
        }

        // �����̈ʒu���擾����
        enemyPos_ = enemyPosSetMap_[mapNum_][num];

        // �����_���ȃL�������擾����(�������A���S�����L�����͏��O����)
        do
        {
            attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);    // ���j�ȏ�MAX�����őI��
        } while (SceneMng.charasList_[attackTarget_].HP() <= 0);
        //attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);

        charaPos_ = SceneMng.charasList_[attackTarget_].GetButtlePos();

        Debug.Log("�U���Ώۂ�" + (SceneMng.CHARACTERNUM)attackTarget_);

        // �s�����̓G���A�U���Ώۂ̕����ɑ̂�������
        enemyMap_[num + 1].transform.localRotation = Quaternion.LookRotation(charaPos_ - enemyPos_);

        anim_ = ANIMATION.ATTACK;    
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

            if(eventEnemy_.Item1 == null)
            {
                // �t�B�[���h��̓G�������_���ŏo��
                enemy = Instantiate(enemyTest, pos, Quaternion.identity) as GameObject;
            }
            else
            {
                // �C�x���g�p�̓G���o��
                enemy = Instantiate(eventEnemy_.Item1, pos, Quaternion.identity) as GameObject;
            }

            enemy.name = num.ToString();

            // �GHP���C���X�^���X
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum_][num - 1], Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]��x�͏o��������G�̍s�ԍ�(���܂̓J�{�X�Œ�)
            enemyList_.Add((new Enemy(num.ToString(), 1,null,enemyData_.param[0]), hpBar.GetComponent<HPBar>()));

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
                AfterAttack();        // �U���I���� 
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

        AttackStart(num);
    }

    private void AttackStart(int num)
    {
        // �_���[�W��n��
        buttleMng_.SetDamageNum(enemyList_[num].Item1.Damage());

        string str = "";

        // �ʏ�U���e�̕����̌v�Z
        var dir = (charaPos_ - enemyPos_).normalized;
        // �G�t�F�N�g�̔����ʒu��������
        // �L�����ƃG�t�F�N�g�̔����������t������Aforward�̌��Z�ɋC��t���鎖(Z���̓}�C�i�X���Ȃ��Ƃ���)
        var adjustPos = new Vector3(enemyPos_.x, enemyPos_.y + 0.3f, enemyPos_.z - transform.forward.z);

        // �ʏ�U���e�v���n�u���C���X�^���X��
        var uniAttackInstance = Instantiate(kabosuAttackPrefab_, adjustPos, Quaternion.identity);
        MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
        // �ʏ�U���e�̔��ł����������w��
        magicMove.SetDirection(dir);

        // ���O�̐ݒ�
        str = "KabosuAttack(Clone)";

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
                // �U���Ώۂ̃L�����̔ԍ���n��
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_);
            }
        }
    }

    void AfterAttack()
    {
        anim_ = ANIMATION.IDLE;
    }

    // CharacterMng.cs�ɍ��W����n��
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }

    public void HPdecrease(int num)
    {
        // �_���[�W�l�̎Z�o
        var damage = buttleMng_.GetDamageNum() - enemyList_[num].Item1.Defence();
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
                    Instantiate(soulEffect_, enemyMap_[num + 1].transform.position, Quaternion.identity);

                    Destroy(obj);   // �G�̍폜
                }
            }

            Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HP�o�[�̍폜
        }
    }

    public void SetEnemySpawn(GameObject obj,int num)
    {
        eventEnemy_ = (obj, num);
    }
}
