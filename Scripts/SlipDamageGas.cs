using UnityEngine;

public class SlipDamageGas : MonoBehaviour
{
    private float nowTime_ = 0.0f;                 // ���݂̎���
    private const float checkTime_ = 0.5f;         // �X���b�v�_���[�W��������
    private const int slipDamage_ = 2;             // �X���b�v�_���[�W�̒l
    private UnitychanController player_;           // �v���C���[���i�[�p

    void Start()
    {
        player_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (player_ == null)
        {
            Debug.Log("FieldMng.cs�Ŏ擾���Ă���Player���null�ł�");
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Player")) //col.tag == "Player"�Ə������A����������
        {
            if(!player_.GetMoveFlag())
            {
                return;     // �v���C���[���ړ����łȂ��ꍇ�́A���Ԃ����Z���Ȃ�
            }

            nowTime_ += Time.deltaTime;
            if(nowTime_ >= checkTime_)
            {
                Debug.Log("�X���b�v�_���[�W");
                for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
                {
                    SceneMng.SetSE(16);
                    if(SceneMng.charasList_[i].HP() < slipDamage_)
                    {
                        continue;   // ����HP��2����(=1)�̂Ƃ��͌��Z�������Ȃ�
                    }
                    SceneMng.charasList_[i].SetHP(SceneMng.charasList_[i].HP() - slipDamage_);
                }
                nowTime_ = 0.0f;    // ���ԏ�����
            }
        }
    }
}
