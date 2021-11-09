using System.Collections;
using UnityEngine;

public class ForcedButtle : MonoBehaviour
{
    public GameObject eventEnemy;                // �����퓬���̓G���A�^�b�`����
    public int eventEnemyNum;                    // �����퓬���̓G�̐����O������w�肷��

    private GameObject uniChan_;                 // ���j
    private UnitychanController controller_;     // ���j�̑�����
    private FieldMng fieldMng_;

    void Start()
    {
        // ���W�ړ��Ɏg�p����
        uniChan_ = GameObject.Find("Uni");
        // ���j�̃R���g���[���[���擾����
        controller_ = GameObject.Find("Uni").GetComponent<UnitychanController>();

        fieldMng_ = GameObject.Find("FieldMng").GetComponent<FieldMng>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            StartCoroutine(SelectForcedButtle());
        }
    }

    // �R���[�`��  
    private IEnumerator SelectForcedButtle()
    {
        // �A�N�e�B�u�ɂ���
        fieldMng_.ChangeFieldUICanvasPopUpActive(-1, -1, false);

        bool tmpFlg = true;    // true:�͂�,false:������
        fieldMng_.MoveArrowIcon(tmpFlg);

        // ���j�̃A�j���[�V�������~�߂�
        controller_.StopUniRunAnim();
        // �I������I�ԊԂ́A�R���g���[���[����ł��Ȃ����Ă���
        controller_.enabled = false;

        while (!controller_.enabled)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("�I�����u�������v");
                tmpFlg = false;
                fieldMng_.MoveArrowIcon(tmpFlg);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("�I�����u�͂��v");
                tmpFlg = true;
                fieldMng_.MoveArrowIcon(tmpFlg);
            }
            else
            {
                // �����������s��Ȃ�
            }

            // �X�y�[�X�L�[�őI���������肵�Aenabled��true�ɂ��邱�Ƃ�while�����甲����悤�ɂ���
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("�I�����̌���");
                // ����\��Ԃɂ���
                controller_.enabled = true;
                // ��A�N�e�B�u�ɂ��邽�߂ɍēx�Ăяo��
                fieldMng_.ChangeFieldUICanvasPopUpActive(-1, -1, false);

                if (tmpFlg)
                {
                    // �͂�
                    // �G�̎�ނƐ����w�肷��
                    GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().SetEnemySpawn(eventEnemy, eventEnemyNum);

                    // �����퓬����������
                    FieldMng.nowMode = FieldMng.MODE.BUTTLE;
                    Debug.Log("���j�������퓬�p�̕ǂ�ʉ߂��܂���");

                    // �I�u�W�F�N�g���A�N�e�B�u�ɂ���(��A�N�e�B�u�ɂ��Ȃ��ƁA�A���Ő퓬����������)
                    this.gameObject.SetActive(false);
                }
                else
                {
                    // ������
                    // -transform.forward�����
                    Vector3 velocity = (-gameObject.transform.forward) * 3.0f;
                    uniChan_.transform.position += velocity;
                }
            }
        }
    }

}
