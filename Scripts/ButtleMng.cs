using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �퓬�S�̂ɂ��ĊǗ�����

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public Canvas fieldUICanvas;            // �\��/��\�������̃N���X�ŊǗ������

    public int debugEnemyNum = 1;           // �C���X�y�N�^�[����G�̐�������ς����悤�� 

    private bool setCallOnce_ = false;      // �퓬���[�h�ɐ؂�ւ�����ŏ��̃^�C�~���O�����؂�ւ��

    private ImageRotate buttleCommandUI_;   // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�

    private CharacterMng characterMng_;         // �L�����N�^�[�Ǘ��N���X�̏��
    private EnemyInstanceMng enemyInstanceMng_; // �G�C���X�^���X�Ǘ��N���X�̏��

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        buttleCommandUI_ = buttleUICanvas.transform.Find("Image").GetComponent<ImageRotate>();
        buttleUICanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // FieldMng�ő����^�C�~���O�𒲐����Ă��邽�߁A������Q�Ƃ��A�퓬���[�h�ȊO�Ȃ�return����
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            setCallOnce_ = false;

            if(buttleUICanvas.gameObject.activeSelf)
            {
                buttleCommandUI_.ResetRotate();   // UI�̉�]����ԍŏ��ɖ߂�
            }
            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // �퓬�J�n���ɐݒ肳��鍀��
        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            characterMng_.ButtleSetCallOnce();

            // �G�̃C���X�^���X(1�`4)
            enemyInstanceMng_.EnemyInstance(debugEnemyNum);

            // Character�Ǘ��N���X�ɓG�̏o������n��
            characterMng_.SetEnemyNum(debugEnemyNum);
        }

        characterMng_.Buttle();
    }
}
