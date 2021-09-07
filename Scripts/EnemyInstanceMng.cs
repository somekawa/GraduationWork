using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject EnemyInstancePointPack;   // �G�̏o���ʒu��ݒ肵������
    public GameObject EnemyCube;                // �e�X�g�p�̓G

    // �L�[��int , value��List Vector3��[�e���[�v�|�C���g�̐�]��map����������
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();

    void Start()
    {
        // EnemyInstancePointPack�̎q�ŉ�
        foreach (Transform childTransform in EnemyInstancePointPack.gameObject.transform)
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
    }

    void Update()
    {

    }

    // �G�̃C���X�^���X����(�z�u�|�W�V������ButtleMng.cs�Ŏw��ł���悤�Ɉ�����p�ӂ��Ă���)
    public void EnemyInstance(int mapNum)
    {
        int num = 1;
        // �w�肳�ꂽ�}�b�v�̃��X�g�����o���āAforeach���ŉ�
        foreach(Vector3 pos in enemyPosSetMap_[mapNum])
        {
            //�@�G�v���n�u���C���X�^���X��
            //Instantiate(EnemyCube, pos, Quaternion.identity);
            GameObject enemy = Instantiate(EnemyCube, pos, Quaternion.identity) as GameObject;
            enemy.name = num.ToString();

            num++;
        }
    }

    // CharacterMng.cs�ɍ��W����n��
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }
}
