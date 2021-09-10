using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �T����/�퓬����킸�A�L�����N�^�[�Ɋ֘A������̂��Ǘ�����

// Chara.cs���C���X�^���X����Ƃ��ɊO���f�[�^�̃L�����f�[�^�����̑O�ɓǂݍ���ł����āAnew�̈����ɓ���ēn���悤�ɂ���
// ����������A�e�L�����ɂ��ꂼ��̃X�e�[�^�X�l��n����B�͂��B���Ԃ�B�B�B

public class CharacterMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������

    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charaObjList;
    public GameObject buttleWarpPointPack;  // �퓬���Ƀt�B�[���h��̐퓬�|�C���g�ɃL���������[�v������

    //�@�ʏ�U���e�̃v���n�u
    [SerializeField]
    private GameObject uniAttackPrefab_;

    // �L�������ʗpenum
    public enum CharcterNum
    {
        UNI,    // ��O
        DEMO,   // ��
        MAX
    }

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[
    private bool selectFlg_ = false;

    private const int buttleCharMax_ = 2;           // �o�g���Q���\�L�������̍ő�l(�ŏI�I�ɂ�3�ɂ���)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // �퓬���̔z�u�ʒu��ۑ����Ă����ϐ�
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // �퓬���̉�]�p�x��ۑ����Ă����ϐ�(�N�H�[�^�j�I��)

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CharcterNum, GameObject> charMap_;

    private ImageRotate buttleCommandUI_;                         // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�
    private EnemySelect buttleEnemySelect_;                       // �o�g�����̑I���A�C�R�����

    private int enemyNum_ = 0;                                    // �o�g�����̓G�̐�
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // �G�̃C���X�^���X�ʒu�̑S���

    private List<Chara> charasList_ = new List<Chara>();          // Chara.cs���L�������Ƀ��X�g������

    void Start()
    {
        // (�����Ɏg���邩������Ȃ�����A)�L�����̏��̓Q�[���I�u�W�F�N�g�Ƃ��čŏ��Ɏ擾���Ă���
        charMap_ = new Dictionary<CharcterNum, GameObject>(){
            {CharcterNum.UNI,charaObjList[(int)CharcterNum.UNI]},
            {CharcterNum.DEMO,charaObjList[(int)CharcterNum.DEMO]},
        };

        // charMap_��foreach���񂵂āAAnimator���擾����
        foreach (KeyValuePair<CharcterNum, GameObject> anim in charMap_)
        {
            // Chara�N���X�̐���
            charasList_.Add(new Chara(anim.Value.name, anim.Key, anim.Value.GetComponent<Animator>()));
        }

        nowTurnChar_ = CharcterNum.UNI;

        // ���[�v�|�C���g�̐��Ԃ�Afor������
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // �ʂɃ��[�v�|�C���g��ϐ��֕ۑ����Ă���
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleCommandUI_   = buttleUICanvas.transform.Find("Image").GetComponent<ImageRotate>();
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();
    }

    void Update()
    {
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            nowTurnChar_ = CharcterNum.UNI;
        }
    }

    // ButtleMng.cs����G�̐����󂯎��
    public void SetEnemyNum(int enemyNum)
    {
        enemyNum_ = enemyNum;

        // ���A�C�R�����\���ł���悤�ɍ��W��n��
        // �ꎞ�ϐ��ɔ����ʒu���R�s�[���Ă���������邱�ƂŁA�G�̔����ʒu�̍���������������̂�h��
        List<Vector3> tmpInsPos = new List<Vector3>(enemyInstancePos_[enemyNum_]);
        buttleEnemySelect_.SetPosList(tmpInsPos);

        // NG�ȏ�����
        // ���̏������ł́A���̓G�̔����ʒu���W������������`�Ŗ��A�C�R������������āA2��ڈȍ~�G�̔����ʒu�����A�C�R���̍����ɂȂ��Ă��܂�
        //buttleEnemySelect_.SetPosList(enemyInstancePos_[enemyNum_]);
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
            //charSetting[(int)character.Key].buttlePos  = character.Value.gameObject.transform.position;
            charasList_[(int)character.Key].SetButtlePos(character.Value.gameObject.transform.position);
        }
    }

    // �L�����̐퓬���Ɋւ��鏈��(ButtleMng.cs�ŎQ��)
    public void Buttle()
    {
        // ATTACK�œG�I�𒆂ɁA����̃L�[(����T�L�[)���������ꂽ��R�}���h�I���ɖ߂�
        if(selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            selectFlg_ = false;
            buttleCommandUI_.SetRotaFlg(!selectFlg_);   // �R�}���h��]��L����
        }

        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �I�����ꂽ�R�}���h�ɑ΂��鏈��
            switch(buttleCommandUI_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:
                    if(!selectFlg_)
                    {
                        selectFlg_ = true;
                    }
                    else
                    {
                        if(charasList_[(int)nowTurnChar_].Attack())
                        {
                            AttackStart((int)nowTurnChar_);
                            selectFlg_ = false;
                        }
                    }

                    buttleCommandUI_.SetRotaFlg(!selectFlg_);
                    buttleEnemySelect_.SetActive(selectFlg_);

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
            if(charasList_[(int)nowTurnChar_].ChangeNextChara())
            {
                // ���̃L�������s���ł���悤�ɂ���
                // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
                if (++nowTurnChar_ >= CharcterNum.MAX)
                {
                    nowTurnChar_ = CharcterNum.UNI;
                }
            }
        }
    }

    void AttackStart(int charNum)
    {
        // �L�����̈ʒu���擾����
        Vector3 charaPos = charasList_[charNum].GetButtlePos();
        // �G�̈ʒu���擾����
        Vector3 enePos = buttleEnemySelect_.GetSelectEnemyPos();
        enePos.y = 0.0f;        // ������0.0f�ɂ��Ȃ��Ǝ΂ߏ�����ɔ��ł��܂�

        // �ʏ�U���e�̕����̌v�Z
        var dir = (enePos - charaPos).normalized;

        // �s�����̃L�������A�U���Ώۂ̕����ɑ̂�������
        // charMap_�̏��𒼐ڕύX����K�v�����邽�߁AcharMap_[nowTurnChar_]�ƋL�q���Ă���
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos - charaPos);

        // �G�t�F�N�g�̔����ʒu��������
        var adjustPos = new Vector3(charaPos.x, charaPos.y + 0.5f, charaPos.z);

        //�@�ʏ�U���e�v���n�u���C���X�^���X��
        //var uniAttackInstance = Instantiate(uniAttackPrefab_, transform.position + transform.forward, Quaternion.identity);
        var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);

        MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
        //�@�ʏ�U���e�̔��ł����������w��
        //magicMove.SetDirection(transform.forward);
        magicMove.SetDirection(dir);

        // �I�������G�̔ԍ���n��
        magicMove.SetTargetNum(buttleEnemySelect_.GetSelectNum() + 1);

        // ���ʒu�̃��Z�b�g���s��
        buttleEnemySelect_.ResetSelectPoint();
    }
}