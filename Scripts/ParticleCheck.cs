using UnityEngine;

public class ParticleCheck : MonoBehaviour
{
    private int maxChildCnt_ = 0;// �I�u�W�F�N�g�̌�
    private float distance_ = 0.0f;// �I�u�W�F�ƃ��j�̋���������
    private float targetDistance_ = 25.0f;// �Đ���������
  
    private GameObject uniChan_;// ���j�̍��W�m�F�̂���
    private GameObject[] obj_;// �z�u����Ă�I�u�W�F�N�g
    private ParticleSystem[] objParicle_;// �I�u�W�F�N�g�ɂ��Ă���p�[�e�B�N��

    void Start()
    {
        uniChan_ = GameObject.Find("Uni");

        maxChildCnt_ = this.transform.childCount;

        obj_ = new GameObject[maxChildCnt_];
        objParicle_ = new ParticleSystem[maxChildCnt_];
        for (int i = 0; i < maxChildCnt_; i++)
        {
            obj_[i] = this.transform.GetChild(i).gameObject;

            // �e�I�u�W�F�N�g�̎q�Ƀp�[�e�B�N�������Ă���
            objParicle_[i] = obj_[i].transform.GetChild(0).GetComponent<ParticleSystem>();
        }
    }

    void Update()
    {
        for (int i = 0; i < maxChildCnt_; i++)
        {
            // ���j�ƃI�u�W�F�N�g�̋���������
            distance_ = (uniChan_.transform.position - obj_[i].transform.position).sqrMagnitude;
            if (distance_ < targetDistance_ * targetDistance_)
            {
                // �߂��ʒu�Ȃ�
                if (objParicle_[i].isPlaying == false)
                {
                    // Particle���Đ�����ĂȂ���΍Đ�
                    objParicle_[i].Play();
                }
            }
            else
            {
                if (objParicle_[i].isPlaying == true)
                {
                    objParicle_[i].Stop();// Particle�Đ����Ȃ��~
                }
            }
        }
    }
}