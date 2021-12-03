using System.Collections;
using UnityEngine;

public class CameraSample : MonoBehaviour
{
    private GameObject player_;   // �v���C���[���i�[�p
    private Vector3 offset_;      // ���΋����擾�p
    private IEnumerator rest_;    // �R���[�`����ۑ�����

    public void Init()
    {
        //unitychan�̏����擾
        this.player_ = GameObject.Find("Uni");

        if (SceneMng.nowScene == SceneMng.SCENE.FIELD3)
        {
            // ���A�^�̃t�B�[���h���������낵�^�J�����ɂ���
            offset_ = new Vector3(0.0f, 4.0f, -2.0f);
        }
        else
        {
            offset_ = new Vector3(0.0f, 3.0f, -3.0f);
        }

        if(rest_ == null)
        {
            rest_ = CameraPosCoroutine();
        }
        else
        {
            StopCoroutine(rest_); //�ꎞ��~
            rest_ = null;         //���Z�b�g
            rest_ = CameraPosCoroutine(); //����Ȃ���
        }

        StartCoroutine(rest_);
    }

    // �J�������ړ������邽�߂̃R���[�`��
    private IEnumerator CameraPosCoroutine()
    {
        bool tmpFlg = false;
        while(!tmpFlg)
        {
            yield return null;
            //�V�����g�����X�t�H�[���̒l��������
            transform.position = player_.transform.position + offset_;
        }
    }
}