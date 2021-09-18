using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    CharcterNum oldTurnChar_ = CharcterNum.UNI;     // �O�ɍs����������Ă��Ă����L�����N�^�[
    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[
    private bool selectFlg_ = false;                // �G��I�𒆂��̃t���O
    private bool lastEnemytoAttackFlg_ = false;     // �L�����̍U���Ώۂ��Ō�̓G�ł��邩     

    private Vector3 keepFieldPos_;                  // �퓬�ɓ��钼�O�̃L�����̍��W��ۑ����Ă��� 
    private const int buttleCharMax_ = 2;           // �o�g���Q���\�L�������̍ő�l(�ŏI�I�ɂ�3�ɂ���)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // �퓬���̔z�u�ʒu��ۑ����Ă����ϐ�
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // �퓬���̉�]�p�x��ۑ����Ă����ϐ�(�N�H�[�^�j�I��)

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CharcterNum, GameObject> charMap_;

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // �o�g�����̈ē�
    private readonly string[] announceText_ = new string[2]{ " ���V�t�g�L�[�F\n �퓬���瓦����", " T�L�[�F\n �R�}���h�֖߂�" };

    private ImageRotate buttleCommandRotate_;                     // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�
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

        buttleAnounceText_ = buttleUICanvas.transform.Find("AnnounceText").GetComponent<TMPro.TextMeshProUGUI>();

        buttleCommandRotate_ = buttleUICanvas.transform.Find("Command").transform.Find("Image").GetComponent<ImageRotate>();
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();
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
        if (buttleUICanvas.gameObject.activeSelf)
        {
            buttleCommandRotate_.ResetRotate();   // UI�̉�]����ԍŏ��ɖ߂�
        }

        buttleAnounceText_.text = announceText_[0];

        // �ŏ��̍s���L�������w�肷��
        nowTurnChar_ = CharcterNum.UNI;

        // �t���O�̏��������s��
        lastEnemytoAttackFlg_ = false;

        // �퓬�O�̍��W��ۑ����Ă���
        keepFieldPos_ = charMap_[CharcterNum.UNI].gameObject.transform.position;

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

            // �s�����Ɋ֘A����l������������
            charasList_[(int)character.Key].SetTurnInit();
        }
    }

    // �L�����̐퓬���Ɋւ��鏈��(ButtleMng.cs�ŎQ��)
    public void Buttle()
    {
        // �퓬���瓦����
        if (!selectFlg_ && Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �G�I�u�W�F�N�g���폜����(�^�O����)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(obj);
            }
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            charMap_[CharcterNum.UNI].gameObject.transform.position = keepFieldPos_;

            Debug.Log("Uni�͓����o����");
        }

        // ATTACK�œG�I�𒆂ɁA����̃L�[(����T�L�[)���������ꂽ��R�}���h�I���ɖ߂�
        if (selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            selectFlg_ = false;
            buttleCommandRotate_.SetRotaFlg(!selectFlg_);   // �R�}���h��]��L����
            buttleAnounceText_.text = announceText_[0];
        }

        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // �I�����ꂽ�R�}���h�ɑ΂��鏈��
            switch(buttleCommandRotate_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:
                    if(!selectFlg_)
                    {
                        // �����̍s���̑O�̐l������I����Ă��邩���ׂ�
                        if (!charasList_[(int)oldTurnChar_].GetIsMove())
                        {
                            oldTurnChar_ = nowTurnChar_;

                            Debug.Log("�O�̃L�������s���I��");
                            selectFlg_ = true;
                            buttleAnounceText_.text = announceText_[1];
                        }
                        else
                        {
                            Debug.Log("�O�̃L�������A�j���[�V������");
                        }
                    }
                    else
                    {
                        if(charasList_[(int)nowTurnChar_].Attack())
                        {
                            AttackStart((int)nowTurnChar_);
                            selectFlg_ = false;
                        }
                    }

                    buttleCommandRotate_.SetRotaFlg(!selectFlg_);
                    buttleEnemySelect_.SetActive(selectFlg_);

                    break;
                case ImageRotate.COMMAND.MAGIC:
                    Debug.Log("���@�R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.ITEM:
                    Debug.Log("�A�C�e���R�}���h���L���R�}���h�ł�");
                    break;
                case ImageRotate.COMMAND.BARRIER:
                    Debug.Log("�h��R�}���h���L���R�}���h�ł�");
                    break;
                default:
                    Debug.Log("�����ȃR�}���h�ł�");
                    break;
            }
        }
        else
        {
            if (charasList_[(int)nowTurnChar_].ChangeNextChara())
            {
                buttleAnounceText_.text = announceText_[0];

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

        // ���ʒu�̃��Z�b�g���s��(false�Ȃ�A�G��S�ē|�����Ƃ������ƂȂ̂Ńt���O��؂�ւ���)
        lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();
    }

    // ButtleMng.cs�ŎQ��
    public bool GetLastEnemyToAttackFlg()
    {
        return lastEnemytoAttackFlg_;
    }

    // ButtleMng.cs�ŎQ��
    public bool GetSelectFlg()
    {
        return selectFlg_;
    }

    public void SetCharaFieldPos()
    {
        charMap_[CharcterNum.UNI].gameObject.transform.position = keepFieldPos_;
    }
}
