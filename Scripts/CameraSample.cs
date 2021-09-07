using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSample : MonoBehaviour
{
    private GameObject player_;   // �v���C���[���i�[�p
    private Vector3 offset_;      // ���΋����擾�p

    void Start()
    {
        //unitychan�̏����擾
        this.player_ = GameObject.Find("Uni");

        // MainCamera(�������g)��player�Ƃ̑��΋��������߂�
        offset_ = transform.position - player_.transform.position;
    }

    void Update()
    {
        //�V�����g�����X�t�H�[���̒l��������
        transform.position = player_.transform.position + offset_;
    }
}