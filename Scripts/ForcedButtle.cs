using UnityEngine;

public class ForcedButtle : MonoBehaviour
{
    public GameObject eventEnemy;                // �����퓬���̓G���A�^�b�`����

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            // �G�̎�ނƐ����w�肷��
            GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().SetEnemySpawn(eventEnemy,1);

            // �����퓬����������
            FieldMng.nowMode = FieldMng.MODE.BUTTLE;
            Debug.Log("���j�������퓬�p�̕ǂ�ʉ߂��܂���");

            // �I�u�W�F�N�g���A�N�e�B�u�ɂ���(��A�N�e�B�u�ɂ��Ȃ��ƁA�A���Ő퓬����������)
            this.gameObject.SetActive(false);
        }
    }
}
