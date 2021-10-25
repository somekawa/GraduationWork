using System.Collections.Generic;
using UnityEngine;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject enemyInstancePointPack;   // �G�̏o���ʒu��ݒ肵������
    public GameObject enemyTest;                // �e�X�g�p�̓G
    public GameObject enemyHPBar;               // �G�p��HP�o�[
    public EnemySelect enemySelectObj;          // �G�I��p�A�C�R��

    // �L�[��int , value��List Vector3��[�e���[�v�|�C���g�̐�]��map����������
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;    // �t�B�[���h���̓G����ۑ�����

    public static List<(Enemy,HPBar)> enemyList_ = new List<(Enemy, HPBar)>();   // Enemy.cs���L�������Ƀ��X�g������

    private ButtleMng buttleMng_;

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
    }

    // �G�̃C���X�^���X����(�z�u�|�W�V������ButtleMng.cs�Ŏw��ł���悤�Ɉ�����p�ӂ��Ă���)
    public void EnemyInstance(int mapNum,Canvas parentCanvas)
    {
        enemyList_.Clear(); // ����g�p�O�ɏ���������

        int num = 1;
        // �w�肳�ꂽ�}�b�v�̃��X�g�����o���āAforeach���ŉ�
        foreach(Vector3 pos in enemyPosSetMap_[mapNum])
        {
            // �G�v���n�u���C���X�^���X
            GameObject enemy = Instantiate(enemyTest, pos, Quaternion.identity) as GameObject;
            enemy.name = num.ToString();

            // �GHP���C���X�^���X
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum][num - 1], Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]��x�͏o��������G�̍s�ԍ�(���܂̓J�{�X�Œ�)
            enemyList_.Add((new Enemy(num.ToString(), 1,null,enemyData_.param[0]), hpBar.GetComponent<HPBar>()));

            // �GHP�̐ݒ�
            enemyList_[num - 1].Item2.SetHPBar(enemyList_[num - 1].Item1.HP(), enemyList_[num - 1].Item1.MaxHP());

            num++;
        }
    }

    public (int,string) EnemyTurnSpeed(int num)
    {
        return (enemyList_[num].Item1.Speed(), enemyList_[num].Item1.Name());
    }

    public void Attack(int num)
    {
        enemyList_[num].Item1.Attack();

        // �_���[�W��n��
        buttleMng_.SetDamageNum(enemyList_[num].Item1.Damage());

        // �s�����I���������^�[�����ڂ�(���܂͂Ƃ肠���������Ń^�[���ڍs)
        buttleMng_.SetMoveTurn();
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
                    Destroy(obj);   // �G�̍폜
                }
            }

            Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HP�o�[�̍폜
        }
    }
}
