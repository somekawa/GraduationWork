using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �T����/�퓬����킸�A�L�����N�^�[�Ɋ֘A������̂��Ǘ�����

public class CharacterMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������

    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charList;
    public GameObject buttleWarpPointPack;  // �퓬���Ƀt�B�[���h��̐퓬�|�C���g�ɃL���������[�v������

    //�@�ʏ�U���e�̃v���n�u
    [SerializeField]
    private GameObject uniAttackPrefab_;

    // �L�������ʗpenum
    enum CharcterNum
    {
        UNI,    // ��O
        DEMO,   // ��
        MAX
    }

    private const string key_isAttack = "isAttack"; // �U�����[�V����(�S�L�����Ŗ��O�𑵂���K�v������)

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[
    private const int buttleCharMax_ = 2;           // �o�g���Q���\�L�������̍ő�l(�ŏI�I�ɂ�3�ɂ���)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // �퓬���̔z�u�ʒu��ۑ����Ă����ϐ�
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // �퓬���̉�]�p�x��ۑ����Ă����ϐ�(�N�H�[�^�j�I��)

    // ���ꂼ��̃L�����p�ɁA�K�v�Ȃ��̂�struct�ł܂Ƃ߂�
    // Animator�Ƃ�HP�Ƃ�
    public struct CharcterSetting
    {
        public Animator animator;   // �e�L�����ɂ��Ă���Animator�̃R���|�[�l���g
        public bool isMove;         // Wait���[�V��������false
        public float animTime;      // ���̃L�����̍s���ɑJ�ڂ���܂ł̊�
        public Vector3 buttlePos;   // �퓬�J�n���ɐݒ肳���|�W�V����(�U���G�t�F�N�g����Instance�ʒu�ɗ��p)
    }

    // ��L�̍\���̂�z��ɂ�������
    private CharcterSetting[] charSetting = new CharcterSetting[(int)CharcterNum.MAX];

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CharcterNum, GameObject> charMap_;

    private ImageRotate buttleCommandUI_;           // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�
    private EnemyInstanceMng enemyInstanceMng_;     // �G�C���X�^���X�Ǘ��N���X�̏��

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

        nowTurnChar_ = CharcterNum.UNI;

        // ���[�v�|�C���g�̐��Ԃ�Afor������
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // �ʂɃ��[�v�|�C���g��ϐ��֕ۑ����Ă���
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleCommandUI_ = buttleUICanvas.transform.Find("Image").GetComponent<ImageRotate>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();
    }

    void Update()
    {
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            nowTurnChar_ = CharcterNum.UNI;
        }
    }

    // �퓬�J�n���ɐݒ肳��鍀��(ButtleMng.cs�ŎQ��)
    public void ButtleSetCallOnce()
    {
        // �퓬�p���W�Ɖ�]�p�x��������
        // �L�����̊p�x��ύX�́AButtleWarpPoint�̔��̊p�x����]������Ɖ\�B(1��1�̌�����ς��邱�Ƃ��ł���)
        foreach (KeyValuePair<CharcterNum, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // �����ō��W��ۑ����Ă������ƂŁA���j���[��ʂł̕��ёւ��ł����f�ł��邾�낤���A
            // �U���G�t�F�N�g�̔����ʒu�̖ڈ��ɂȂ�
            charSetting[(int)character.Key].buttlePos = character.Value.gameObject.transform.position;
        }
    }

    // �L�����̐퓬���Ɋւ��鏈��(ButtleMng.cs�ŎQ��)
    public void Buttle()
    {
        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �I�����ꂽ�R�}���h�ɑ΂��鏈��
            switch(buttleCommandUI_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:
                    Debug.Log("�U���R�}���h���L���R�}���h�ł�");
                    charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
                    // isMove��false�̂Ƃ������U���G�t�F�N�g��Instance��isMove��true���������s���悤�ɂ��āA
                    // �G�t�F�N�g���{�^���A�łő�ʔ�������̂�h��
                    if (!charSetting[(int)nowTurnChar_].isMove)
                    {
                        AttackStart((int)nowTurnChar_);
                        charSetting[(int)nowTurnChar_].isMove = true;
                    }
                    break;
                case ImageRotate.COMMAND.MAGIC:
                    Debug.Log("���@�R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.ITEM:
                    Debug.Log("�A�C�e���R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.ESCAPE:
                    Debug.Log("�����R�}���h���L���R�}���h�ł�");
                    break;
                default:
                    Debug.Log("�����ȃR�}���h�ł�");
                    break;
            }
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
                // animTime�l��1.0���������Ƃ�
                // animTime�l�̏������ƁA���[�V������Wait�ɂȂ�����isMove��false�֖߂�
                charSetting[(int)nowTurnChar_].animTime = 0.0f;
                charSetting[(int)nowTurnChar_].isMove = false;

                // ���̃L�������s���ł���悤�ɂ���
                // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
                if (++nowTurnChar_ >= CharcterNum.MAX)
                {
                    nowTurnChar_ = CharcterNum.UNI;
                }
            }
        }

        //Debug.Log(nowTurnChar_ + "�̍U��");
    }

    void AttackStart(int charNum)
    {
        // �G�̈ʒu���擾����
        var enePos = enemyInstanceMng_.GetEnemyPos(1);
        enePos.y = 0.0f;        // ������0.0f�ɂ��Ȃ��Ǝ΂ߏ�����ɔ��ł��܂�

        // �ʏ�U���e�̕����̌v�Z
        var dir = (enePos - charSetting[charNum].buttlePos).normalized;

        // �G�t�F�N�g�̔����ʒu��������
        var adjustPos = new Vector3(charSetting[charNum].buttlePos.x, charSetting[charNum].buttlePos.y + 0.5f, charSetting[charNum].buttlePos.z);

        //�@�ʏ�U���e�v���n�u���C���X�^���X��
        //var uniAttackInstance = Instantiate(uniAttackPrefab_, transform.position + transform.forward, Quaternion.identity);
        var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);

        //�@�ʏ�U���e�̔��ł����������w��
        //uniAttackInstance.GetComponent<MagicMove>().SetDirection(transform.forward);
        uniAttackInstance.GetComponent<MagicMove>().SetDirection(dir);
    }
}
