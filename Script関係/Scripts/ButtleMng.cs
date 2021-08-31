using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�����̃^�[�����Ǘ�����

public class ButtleMng : MonoBehaviour
{
    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charList;

    // �L�������ʗpenum
    enum CharcterNum
    {
        UNI,    // ��O
        DEMO,   // ��
        MAX
    }

    private GameObject buttleCamera;                // �o�g���J�����i�[�p 
    private const string key_isAttack = "isAttack"; // �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[

    // ���ꂼ��̃L�����p�ɁA�K�v�Ȃ��̂�struct�ł܂Ƃ߂�
    // Animator�Ƃ�HP�Ƃ�
    public struct CharcterSetting
    {
        public Animator animator;   // �e�L�����ɂ��Ă���Animator�̃R���|�[�l���g
        public bool isMove;         // Wait���[�V��������false
        public float animTime;      // ���̃L�����̍s���ɑJ�ڂ���܂ł̊�
    }

    // ��L�̍\���̂�z��ɂ�������
    private CharcterSetting[] charSetting = new CharcterSetting[(int)CharcterNum.MAX];

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CharcterNum, GameObject> charMap_;

    void Start()
    {
        // (�����Ɏg���邩������Ȃ�����A)�L�����̏��̓Q�[���I�u�W�F�N�g�Ƃ��čŏ��Ɏ擾���Ă���
        charMap_ = new Dictionary<CharcterNum, GameObject>(){
            {CharcterNum.UNI,charList[(int)CharcterNum.UNI]},
            {CharcterNum.DEMO,charList[(int)CharcterNum.DEMO]},
        };

        // charMap_��foreach���񂵂āAAnimator���擾����
        foreach (KeyValuePair<CharcterNum, GameObject> anim in charMap_) 
        {
            // �\���̂�animator�ɁA�L��������Animator��������
            charSetting[(int)anim.Key].animator = anim.Value.GetComponent<Animator>();
            charSetting[(int)anim.Key].isMove = false;
            charSetting[(int)anim.Key].animTime = 0.0f;
        }

        buttleCamera = GameObject.Find("ButtleCamera");
        nowTurnChar_ = CharcterNum.UNI;
    }

    void Update()
    {
        // ButtoleCamera��OFF�Ȃ炱����return����
        if (!buttleCamera.activeSelf)
        {
            return;
        }

        Debug.Log(nowTurnChar_ + "�̍U��");

        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
            charSetting[(int)nowTurnChar_].isMove = true;
        }
        else
        {
            charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, false);

            if (!charSetting[(int)nowTurnChar_].isMove)
            {
                return;
            }

            // �������牺�́AisMove��true�̏��
            // isMove��true�Ȃ�A�������܂ōU�����[�V�������������Ƃ��킩��
            // �Đ����ԗp�ɊԂ��󂯂�
            // �L�������ɕK�v�ȊԂ��Ⴄ��������Ȃ�����A1.0f�̏��͊O���f�[�^�ǂݍ��݂ŁAcharSetting[(int)nowTurnChar_].maxAnimTime�Ƃ���p�ӂ����ق�������
            if (charSetting[(int)nowTurnChar_].animTime < 1.0f)
            {
                charSetting[(int)nowTurnChar_].animTime += Time.deltaTime;
                return;
            }
            else
            {
                // animTime�l�̏������ƁA���[�V������Wait�ɂȂ�����isMove��false�֖߂�
                charSetting[(int)nowTurnChar_].animTime = 0.0f;
                charSetting[(int)nowTurnChar_].isMove = false;

                // animTime��1.0f�ȏ�ɂȂ����玟�̃L�������s���ł���悤�ɂ���
                nowTurnChar_++;
                // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�
                if (nowTurnChar_ >= CharcterNum.MAX)
                {
                    nowTurnChar_ = CharcterNum.UNI;
                }
            }
        }

        // �����̃L�������[�V��������
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
        //    //nowTurnChar_++; // �����ŉ��Z�����false�ɒH�蒅���ĂȂ��̂��ςȓ����ɂȂ�
        //}
        //else
        //{
        //    charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, false);
        //}
    }
}
