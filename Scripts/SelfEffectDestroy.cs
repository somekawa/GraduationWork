using UnityEngine;

public class SelfEffectDestroy : MonoBehaviour
{
    private ParticleSystem particle_;

    void Start()
    {
        particle_ = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (particle_.isStopped) //�p�[�e�B�N�����I������������
        {
            Destroy(this.gameObject);//�p�[�e�B�N���p�Q�[���I�u�W�F�N�g���폜
        }
        else
        {
            if(gameObject.transform.localScale.x > 0.0f)    // �T�C�Y�̕ύX���x�͂ǂ�������������\����x�Œ��ׂĂ� 
            {
                // �G�t�F�N�g�T�C�Y�̕ύX
                Vector3 tmp = gameObject.transform.localScale;  // ���݂̃T�C�Y
                tmp = tmp - new Vector3(0.1f, 0.1f, 0.1f);      // �T�C�Y��ύX
                gameObject.transform.localScale = tmp;          // �ύX�����T�C�Y�����݃T�C�Y�ɑ��

                // �G�t�F�N�g�ʒu�̕ύX
                Vector3 tmp2 = gameObject.transform.localPosition;  // ���݂̈ʒu
                tmp2.y = tmp2.y + 0.05f;                            // �ʒu��ύX(��ɂ�������������y�̂ݕύX)
                gameObject.transform.localPosition = tmp2;          // �ύX�����ʒu�����݈ʒu�ɑ��
            }
        }
    }
}
