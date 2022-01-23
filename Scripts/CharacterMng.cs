using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharaBase;
using static SceneMng;

// �T����/�퓬����킸�A�L�����N�^�[�Ɋ֘A������̂��Ǘ�����
public class CharacterMng : MonoBehaviour
{
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;

    public Canvas buttleUICanvas;           // �\��/��\�������̃N���X�ŊǗ������
    public GameObject buttleWarpPointPack;  // �퓬���Ƀt�B�[���h��̐퓬�|�C���g�ɃL���������[�v������

    //�@�ʏ�U���e�̃v���n�u
    [SerializeField]
    private GameObject uniAttackPrefab_;

    CHARACTERNUM oldTurnChar_ = CHARACTERNUM.UNI;     // �O�ɍs����������Ă��Ă����L�����N�^�[
    CHARACTERNUM nowTurnChar_ = CHARACTERNUM.MAX;     // ���ݍs����������Ă��Ă���L�����N�^�[
    private bool selectFlg_ = false;                  // �G��I�𒆂��̃t���O
    private bool lastEnemytoAttackFlg_ = false;       // �L�����̍U���Ώۂ��Ō�̓G�ł��邩     

    private const int buttleCharMax_ = 2;             // �o�g���Q���\�L�������̍ő�l(�ŏI�I�ɂ�3�ɂ���)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // �퓬���̔z�u�ʒu��ۑ����Ă����ϐ�
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // �퓬���̉�]�p�x��ۑ����Ă����ϐ�(�N�H�[�^�j�I��)

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private Dictionary<CHARACTERNUM, GameObject> charMap_;
    // Chara.cs���L�������Ƀ��X�g������
    private List<Chara> charasList_ = new List<Chara>();
    // �e�L�����̃o�X�e�A�C�R��
    private GameObject[] charaBstIconImage_ = new GameObject[(int)CHARACTERNUM.MAX];

    private TMPro.TextMeshProUGUI buttleAnounceText_;             // �o�g�����̈ē�
    private readonly string[] announceText_ = new string[2] { " ���V�t�g�L�[�F\n �퓬���瓦����", " T�L�[�F\n �R�}���h�֖߂�" };

    private ImageRotate magicButtleCommandRotate_;                // ���@�p��ImageRotate
    private ImageRotate buttleCommandRotate_;                     // �o�g�����̃R�}���hUI���擾���āA�ۑ����Ă����ϐ�
    private GameObject buttleCommandFrame_;                       // ��g��frame�����̉摜
    private GameObject[] buttleCommandImage_ = new GameObject[4]; // �o�g���R�}���h�̉摜4���
    private EnemySelect buttleEnemySelect_;                       // �o�g�����̑I���A�C�R�����
    private GameObject buttleItemBackButtonObj_;                  // �o�g�����̃A�C�e����ʂ���߂�A�C�R��
    private GameObject buttleDamageIconsObj_;                     // �o�g�����̃_���[�W�A�C�R���̐e�I�u�W�F�N�g
    private Vector3[] buttleDamgeIconPopUpPos_ = new Vector3[2];  // �_���[�W�A�C�R���̕\���ʒu
    private GameObject buttleMagicInfoFrame_;                     // �o�g�����ɖ��@�R�}���h��I�������Ƃ��̖��@������
    private TMPro.TextMeshProUGUI magicInfoText_;                 // ���@�̐����e�L�X�g
    private ButtleMng buttleMng_;                                 // ButtleMng.cs�̎擾
    private BadStatusMng badStatusMng_;

    private GameObject setMagicObj_;                    // ���@�R�}���h�I�����ɕ\��������I�u�W�F�N�g
    private int magicCommandNumOld_;                    // ���@�R�}���h��1�t���[���O�̐���  
    private Image[] magicImage_ = new Image[4];         // ���@�摜�̓\��t����(�ő�4�\���ɂȂ�)

    private int enemyNum_ = 0;                                    // �o�g�����̓G�̐�
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // �G�̃C���X�^���X�ʒu�̑S���

    private Vector3 charaPos_;                         // �L�����N�^�[���W
    private Vector3 enePos_;                           // �ڕW�̓G���W

    private CharaUseMagic useMagic_;
    private int mpDecrease_ = 0;                       // ���@�������Ɍ���������MP�̗�

    private IEnumerator rest_;
    private bool myTurnOnceFlg_;                       // �����̃^�[���ɂȂ����ŏ���1�񂾂��Ă΂��悤�ɂ���t���O

    private readonly int deathBstNoEffectItemNum_ = 17; // ����������A�C�e���̔ԍ�

    public struct EachCharaData
    {
        // �e�L������HP���
        public (HPMPBar, HPMPBar) charaHPMPMap;
        // �e�L�����̑I����
        public GameObject charaArrowImage;
        // �e�L�����̃o�b�h�X�e�[�^�X�񕜂܂ł̃^�[����
        public Dictionary<CONDITION, int> charaBstTurn;
        // �e�L�����̃o�t�摜
        public GameObject buffIconParent;
        // �e�L�����̋z��or���˃o�t
        public GameObject specialBuff;
        // �e�L�����̍s�����Ԃ̐���
        public TMPro.TextMeshProUGUI turnNum;
    }

    private EachCharaData[] eachCharaData_ = new EachCharaData[(int)CHARACTERNUM.MAX];

    void Start()
    {
        // SceneMng����L�����̏������炤(charMap_��charasList_)
        charMap_ = SceneMng.charMap_;
        charasList_ = SceneMng.charasList_;

        nowTurnChar_ = CHARACTERNUM.UNI;

        // ���[�v�|�C���g�̐��Ԃ�Afor������
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // �ʂɃ��[�v�|�C���g��ϐ��֕ۑ����Ă���
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleAnounceText_ = buttleUICanvas.transform.Find("AnnounceText").GetComponent<TMPro.TextMeshProUGUI>();

        var commandImage = buttleUICanvas.transform.Find("Command/Image");
        buttleCommandRotate_ = commandImage.GetComponent<ImageRotate>();
        for(int i = 0; i < commandImage.childCount; i++)
        {
            // �R�}���h�摜4��ނ��擾
            buttleCommandImage_[i] = commandImage.GetChild(i).gameObject;
        }
        buttleCommandFrame_ = buttleUICanvas.transform.Find("Command/Frame").gameObject;
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();
        buttleItemBackButtonObj_ = buttleUICanvas.transform.Find("ItemBackButton").gameObject;
        buttleItemBackButtonObj_.SetActive(false);  // �ŏ��͔�\��
        buttleDamageIconsObj_ = buttleUICanvas.transform.Find("DamageIcons").gameObject;
        buttleDamgeIconPopUpPos_[0] = new Vector3(-260,   0, 0);
        buttleDamgeIconPopUpPos_[1] = new Vector3(-260, 100, 0);
        buttleMagicInfoFrame_ = buttleUICanvas.transform.Find("MagicInfoFrame").gameObject;
        magicInfoText_ = buttleMagicInfoFrame_.transform.Find("MagicInfoText").GetComponent<TMPro.TextMeshProUGUI>();
        buttleMagicInfoFrame_.SetActive(false);     // �ŏ��͔�\��

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();

        var buttleMng = GameObject.Find("ButtleMng");
        buttleMng_ = buttleMng.GetComponent<ButtleMng>();
        badStatusMng_ = buttleMng.GetComponent<BadStatusMng>();

        setMagicObj_ = buttleUICanvas.transform.Find("SetMagicObj").gameObject;
        // ���@�摜�̕\�����ݒ肷��
        for (int i = 0; i < magicImage_.Length; i++)
        {
            magicImage_[i] = setMagicObj_.transform.GetChild(i).GetComponent<Image>();
        }
        magicButtleCommandRotate_ = setMagicObj_.GetComponent<ImageRotate>();
        magicButtleCommandRotate_.SetEnableAndActive(false);

        // ���j�ƃW���b�N�̕��ŉ�
        for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            // �L������+CharaData�̃X�e�[�^�X�\����AHP/MP�����擾����
            eachCharaData_[i].charaHPMPMap =
                (buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/HPSlider").GetComponent<HPMPBar>(), buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/MPSlider").GetComponent<HPMPBar>());
            // ����HP����
            eachCharaData_[i].charaHPMPMap.Item1.SetHPMPBar(charasList_[i].HP(), charasList_[i].MaxHP());
            // ����MP����
            eachCharaData_[i].charaHPMPMap.Item2.SetHPMPBar(charasList_[i].MP(), charasList_[i].MaxMP());

            // �e�L�����ɂ��Ă���I����̃I�u�W�F�N�g���擾����
            eachCharaData_[i].charaArrowImage = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/ArrowImage").gameObject;
            eachCharaData_[i].charaArrowImage.SetActive(false);

            // �o�b�h�X�e�[�^�X�����Ǘ��p
            eachCharaData_[i].charaBstTurn = new Dictionary<CONDITION, int>();

            charaBstIconImage_[i] = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/BadStateImages").gameObject;

            // �o�t�摜�p
            eachCharaData_[i].buffIconParent = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/BuffImages").gameObject;
            eachCharaData_[i].specialBuff = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/SpecialBuff").gameObject;

            // �s�����Ԃ̐���
            eachCharaData_[i].turnNum = buttleUICanvas.transform.Find(charasList_[i].Name() + "CharaData/MoveSpeed").GetComponent<TMPro.TextMeshProUGUI>();
        }

        useMagic_ = new CharaUseMagic();
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
            magicButtleCommandRotate_.ResetRotate();
        }

        for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            eachCharaData_[i].charaHPMPMap.Item1.SetHPMPBar(charasList_[i].HP(), charasList_[i].MaxHP());
            eachCharaData_[i].charaHPMPMap.Item2.SetHPMPBar(charasList_[i].MP(), charasList_[i].MaxMP());

            // �o�X�e��o�t�̃A�C�R��������
            badStatusMng_.SetBstIconImage(i, -1, charaBstIconImage_, charasList_[i].GetBS(), true);
            // ���ʂ��؂ꂽ(=�^�[����0�ȉ�)
            var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild(i);
            if (child.GetComponent<Image>().sprite != null)
            {
                // �A�C�R����null�ɂ��āA�㏸������\���ɂ���
                child.GetComponent<Image>().sprite = null;
                for (int m = 0; m < child.childCount; m++)
                {
                    child.GetChild(m).gameObject.SetActive(false);
                }
            }
        }

        anim_ = ANIMATION.IDLE;
        oldAnim_ = ANIMATION.IDLE;

        buttleAnounceText_.text = announceText_[0];
        magicCommandNumOld_ = -1;

        // �ŏ��̍s���L�������w�肷��
        //@ �L�������m�ő��x�����āA�����l�����Ȃ��Ƃ���
        nowTurnChar_ = CHARACTERNUM.UNI;

        // �t���O�̏��������s��
        lastEnemytoAttackFlg_ = false;

        // �퓬�O�̍��W��ۑ����Ă���
        buttleMng_.SetFieldPos(charMap_[CHARACTERNUM.UNI].gameObject.transform.position);

        // �퓬�p���W�Ɖ�]�p�x��������
        // �L�����̊p�x��ύX�́AButtleWarpPoint�̔��̊p�x����]������Ɖ\�B(1��1�̌�����ς��邱�Ƃ��ł���)
        foreach (KeyValuePair<CHARACTERNUM, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // �����ō��W��ۑ����Ă������ƂŁA���j���[��ʂł̕��ёւ��ł����f�ł��邾�낤���A
            // �U���G�t�F�N�g�̔����ʒu�̖ڈ��ɂȂ�
            charasList_[(int)character.Key].SetButtlePos(character.Value.gameObject.transform.position);

            // �s�����Ɋ֘A����l������������
            charasList_[(int)character.Key].SetTurnInit();
        }

        useMagic_.Init();
    }

    public (int, string) CharaTurnSpeed(int num)
    {
        return (charasList_[num].Speed(), charasList_[num].Name());
    }

    // �L�����̐퓬���Ɋւ��鏈��(ButtleMng.cs�ŎQ��)
    public void Buttle()
    {
        //Debug.Log(anim_);

        // ���S���Ă�����
        if (charasList_[(int)nowTurnChar_].GetDeathFlg())
        {
            if (eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1.GetColFlg())
            {
                return;
            }
            else
            {
                // �o�X�e�ƃo�t�̍폜
                charasList_[(int)nowTurnChar_].ButtleInit(false);
                // �o�X�e��o�t�̃A�C�R��������
                badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                // ���ʂ��؂ꂽ(=�^�[����0�ȉ�)
                var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild((int)nowTurnChar_);
                if (child.GetComponent<Image>().sprite != null)
                {
                    // �A�C�R����null�ɂ��āA�㏸������\���ɂ���
                    child.GetComponent<Image>().sprite = null;
                    for (int m = 0; m < child.childCount; m++)
                    {
                        child.GetChild(m).gameObject.SetActive(false);
                    }
                }

                oldAnim_ = anim_;
                Debug.Log("���S��������s�����΂�");
                anim_ = ANIMATION.IDLE;
                oldAnim_ = ANIMATION.NON;

                // �S�ł������m�F���鏈��
                bool allDeathFlg = true;
                for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                {
                    if (!charasList_[i].GetDeathFlg())
                    {
                        // 1�l�ł�������Ԃ��ƕ������break���Ĕ�����
                        allDeathFlg = false;
                        break;
                    }
                }
                // �S�Ŏ��͒����̉Ƃ֔�΂�
                if (allDeathFlg)
                {
                    // �����퓬�p�̕ǂ̖��O��ۑ����Ă��鏊��������
                    ButtleMng.forcedButtleWallName = "";
                    EventMng.SetChapterNum(100, SCENE.CONVERSATION);
                }
            }
        }

        // �L�����ɂ��A�C�e���̎g�p��Ԃ��m�F
        if (Bag_Item.itemUseFlg)
        {
            Bag_Item.itemUseFlg = false;
            // �A�C�e����ʂ����
            GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
            buttleItemBackButtonObj_.SetActive(false);  

            for (int k = 0; k < (int)CHARACTERNUM.MAX; k++)
            {
                // HPMP�o�[�̍X�V
                eachCharaData_[k].charaHPMPMap.Item1.SetHPMPBar(charasList_[k].HP(), charasList_[k].MaxHP());
                eachCharaData_[k].charaHPMPMap.Item2.SetHPMPBar(charasList_[k].MP(), charasList_[k].MaxMP());

                // �o�X�e�̍X�V
                var tmpbs = charasList_[k].GetBS();
                var tmpflg = false;
                // ��������Ԉُ��T���āA��Ԉُ�A�C�R�����O��
                for (int i = 0; i < (int)CONDITION.DEATH; i++)
                {
                    // NON���ŏ�����true�Ȃ�break�Ŕ�΂�
                    if (tmpbs[0].Item2)
                    {
                        break;
                    }

                    if (!tmpbs[i].Item2)
                    {
                        badStatusMng_.SetBstIconImage(k, -1, charaBstIconImage_, charasList_[k].GetBS(), true);
                    }
                    tmpflg |= tmpbs[i].Item2;
                }

                // GetBS�őS��false�ɂȂ��Ă�����A�A�C�e���őS�Ă̏�Ԉُ킪�������Ƃ����؋�
                if (!tmpflg)
                {
                    // CONDITION��NON�ɖ߂�
                    charasList_[k].ConditionReset(true);
                    badStatusMng_.SetBstIconImage(k, -1, charaBstIconImage_, charasList_[k].GetBS(), true);
                    Debug.Log("�L������Ԉُ킪�S�Ď�����");
                }
            }

            // ���̃L����or�G�ɍs�������悤��anim_��oldAnim_��ݒ肷��
            anim_ = ANIMATION.IDLE;
            oldAnim_ = ANIMATION.NON;
            Debug.Log("�A�C�e�����g�p�����̂ŁA�s���I��");
        }

        // �s���O�ɔ�������o�b�h�X�e�[�^�X�̏���
        if (!myTurnOnceFlg_)
        {
            myTurnOnceFlg_ = true;
            var bst = badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

            if (bst == (CONDITION.PARALYSIS, true))
            {
                // ��Ⴢœ����Ȃ�
                Debug.Log("��Ⴢ�����s�����΂�");
                anim_ = ANIMATION.IDLE;
                oldAnim_ = ANIMATION.NON;
                oldAnim_ = anim_;
                AnimationChange();
                return;
            }
        }

        // �e�X�g�p(���x���A�b�v����)
        if (Input.GetKeyDown(KeyCode.L))
        {
            charasList_[0].LevelUp();
            charasList_[1].LevelUp();
        }


        // �퓬���瓦����
        if (!selectFlg_ && Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
            {
                buttleMng_.CallDeleteEnemy();

                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                charMap_[CHARACTERNUM.UNI].gameObject.transform.position = buttleMng_.GetFieldPos();

                Debug.Log("Uni�͓����o����");
            }
            else
            {
                Debug.Log("�����퓬���I�������Ȃ��I");
            }
        }

        // ATTACK�œG�I�𒆂ɁA����̃L�[(����T�L�[)���������ꂽ��R�}���h�I���ɖ߂�
        if (selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            anim_ = ANIMATION.NON;
            selectFlg_ = false;

            buttleCommandRotate_.SetEnableAndActive(true);
            buttleCommandFrame_.SetActive(true);

            buttleAnounceText_.text = announceText_[0];

            if(rest_ != null)
            {
                StopCoroutine(rest_);
                rest_ = null;
            }

            for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                eachCharaData_[i].charaArrowImage.SetActive(false);
            }

            // ���@�R�}���h���L����������
            if (setMagicObj_.activeSelf || buttleEnemySelect_.gameObject.activeSelf)
            {
                Debug.Log("���@�R�}���h�̑I�����������܂�");
                buttleMagicInfoFrame_.SetActive(false);
                magicButtleCommandRotate_.SetEnableAndActive(false);

                // �R�}���h�摜��\���ɂ���
                for (int i = 0; i < buttleCommandImage_.Length; i++)
                {
                    buttleCommandImage_[i].SetActive(true);
                }
                // UI�̉�]����ԍŏ��ɖ߂�
                magicButtleCommandRotate_.ResetRotate();
            }
        }

        // �L�������̃��[�V�������Ă�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(setMagicObj_.activeSelf)    // ���@�R�}���h�I�𒆂̂Ƃ�
            {
                // ���@�R�}���h�I�𒆂̂Ƃ�
                buttleAnounceText_.text = announceText_[1];

                var tmp = (int)magicButtleCommandRotate_.GetNowCommand() - 1;
                mpDecrease_ = useMagic_.MPdecrease(charasList_[(int)nowTurnChar_].GetMagicNum(tmp));

                Debug.Log(tmp + "�Ԃ̖��@���g�p���悤�Ƃ��Ă��܂�");

                // ��]���͖��@�����擾���Ȃ��悤�ɂ���B���A�͈͊O���̊m�F������
                if (charasList_[(int)nowTurnChar_].CheckMagicNum(tmp) && charasList_[(int)nowTurnChar_].GetMagicNum(tmp).number > 0)
                {
                    // ���݂�MP�ʂƁA�����ɕK�v��MP�ʂ��r����
                    if(charasList_[(int)nowTurnChar_].MP() < mpDecrease_)
                    {
                        Debug.Log("���@�𔭓����邽�߂�MP���s�����Ă��܂�");
                        mpDecrease_ = 0;
                    }
                    else
                    {
                        // CharaUseMagic.cs�ɏ���n��
                        useMagic_.CheckUseMagic(charasList_[(int)nowTurnChar_].GetMagicNum(tmp), charasList_[(int)nowTurnChar_].MagicPower());

                        magicButtleCommandRotate_.SetEnableAndActive(false);
                        buttleCommandRotate_.SetEnableAndActive(false);
                        buttleCommandFrame_.SetActive(false);
                    }
                }
                else
                {
                    Debug.Log("�͈͊O�̖��@�ԍ��ł��B�I�����Ȃ����Ă�������");
                }
            }
            else
            {
                // �I�����ꂽ�R�}���h�ɑ΂��鏈��
                switch (buttleCommandRotate_.GetNowCommand())
                {
                    case ImageRotate.COMMAND.ATTACK:
                        if (!selectFlg_)
                        {
                            // �����̍s���̑O�̐l������I����Ă��邩���ׂ�
                            if (!charasList_[(int)oldTurnChar_].GetIsMove())
                            {
                                if (anim_ == ANIMATION.IDLE || anim_ == ANIMATION.NON)
                                {
                                    anim_ = ANIMATION.BEFORE;
                                    buttleCommandFrame_.SetActive(false);
                                }
                            }
                            else
                            {
                                Debug.Log("�O�̃L�������A�j���[�V������");
                            }
                        }
                        else
                        {
                            if (anim_ == ANIMATION.BEFORE)
                            {
                                // �h��p�̒l��0�ɖ߂�
                                charasList_[(int)nowTurnChar_].SetBarrierNum();
                                BeforeAttack();    // �U������
                                // �U��������-1�ɂ���(��_�Ƃ���������Ԃɂ���)
                                buttleMng_.SetElement(-1);
                                // �o�X�e�̕t�^�𖳂��ɖ߂�
                                buttleMng_.SetBadStatus(-1,-1);
                            }
                        }

                        break;
                    case ImageRotate.COMMAND.MAGIC:

                        if (!selectFlg_)
                        {
                            // �����̍s���̑O�̐l������I����Ă��邩���ׂ�
                            if (!charasList_[(int)oldTurnChar_].GetIsMove())
                            {
                                anim_ = ANIMATION.NON;

                                // �h��p�̒l��0�ɖ߂�
                                charasList_[(int)nowTurnChar_].SetBarrierNum();

                                // �R�}���h�摜���\���ɂ���
                                for(int i = 0; i < buttleCommandImage_.Length; i++)
                                {
                                    buttleCommandImage_[i].SetActive(false);
                                }

                                Debug.Log("���@�R�}���h���L���R�}���h�ł�");
                                selectFlg_ = true;

                                buttleMagicInfoFrame_.SetActive(true);
                                magicButtleCommandRotate_.SetEnableAndActive(true);
                                buttleCommandRotate_.SetEnableAndActive(false);

                                //@ �s�����̃L�����ɐݒ肳�ꂽ���@�摜��`�悷��
                                for (int i = 0; i < 4; i++)
                                {
                                    if (charasList_[(int)nowTurnChar_].GetMagicImage(i) == null)
                                    {
                                        // null�̂Ƃ��͖��@��ݒ肵�Ă��Ȃ����瓧�߂���
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                                    }
                                    else
                                    {
                                        magicImage_[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                                        magicImage_[i].sprite = charasList_[(int)nowTurnChar_].GetMagicImage(i);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("�O�̃L�������A�j���[�V������");
                            }
                        }
                        else
                        {
                            MagicAttack();
                        }

                        break;
                    case ImageRotate.COMMAND.ITEM:
                        // �h��p�̒l��0�ɖ߂�
                        charasList_[(int)nowTurnChar_].SetBarrierNum();
                        Debug.Log("�A�C�e���R�}���h���L���R�}���h�ł�");

                        // �A�C�e����ʂ��J��
                        GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(true);
                        buttleItemBackButtonObj_.SetActive(true);
                        break;
                    case ImageRotate.COMMAND.BARRIER:
                        // ���̎����̃^�[���܂Ŗh��͂�1.5�{�ɂ���
                        charasList_[(int)nowTurnChar_].SetBarrierNum(charasList_[(int)nowTurnChar_].Defence(false) / 2);
                        // ���̃L����or�G�ɍs�������悤��anim_��oldAnim_��ݒ肷��
                        anim_ = ANIMATION.IDLE;
                        oldAnim_ = ANIMATION.NON;

                        Debug.Log("�h��R�}���h���L���R�}���h�ł�");
                        break;
                    default:
                        // �h��p�̒l��0�ɖ߂�
                        charasList_[(int)nowTurnChar_].SetBarrierNum();
                        Debug.Log("�����ȃR�}���h�ł�");
                        break;
                }
            }
        }
        else
        {
            if (setMagicObj_.activeSelf)    // ���@�R�}���h�I�𒆂̂Ƃ�
            {
                var tmp = (int)magicButtleCommandRotate_.GetNowCommand() - 1;
                if(tmp >= 0 && magicCommandNumOld_ != tmp)
                {
                    if(charasList_[(int)nowTurnChar_].GetMagicNum(tmp).number > 0)
                    {
                        magicInfoText_.text = useMagic_.MagicInfoMake(charasList_[(int)nowTurnChar_].GetMagicNum(tmp));
                    }
                    else
                    {
                        magicInfoText_.text = "";       // ���@���Z�b�g���Ă��Ȃ��Ƃ���ɃR�}���h����]�����Ƃ��́A���e�𖢋L���ɂ���
                    }
                }
                magicCommandNumOld_ = tmp;
                Debug.Log("���@�I��-----���݂�" + tmp + "�Ԃ̃R�}���h��");
            }
        }

        if (oldAnim_ != anim_)
        {
            oldAnim_ = anim_;
            AnimationChange();
        }

        if (anim_ == ANIMATION.ATTACK && charasList_[(int)nowTurnChar_].ChangeNextChara())
        {
            anim_ = ANIMATION.AFTER;
        }
    }

    public void NotMyTurn()
    {
        // �_���[�W���󂯂��Ƃ��̃��[�V�������K�莞�ԂŏI��������
        for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
        {
            charasList_[i].DamageAnim();
        }
    }

    // �s���^�[��������������
    public bool SetMoveSpeedNum(int num,string name)
    {
        if(name == "Uni")
        {
            eachCharaData_[(int)CHARACTERNUM.UNI].turnNum.text = num.ToString();
            return true;
        }
        else if(name == "Jack")
        {
            eachCharaData_[(int)CHARACTERNUM.JACK].turnNum.text = num.ToString();
            return true;
        }
        else
        {
            // �����������s��Ȃ�
        }
        return false;
    }

    void AnimationChange()
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:

                // �ł̏����������
                badStatusMng_.BadStateMoveAfter(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);
                // �o�X�e�̎����^�[������-1����
                for (int i = 0; i < (int)CONDITION.DEATH; i++)
                {
                    // �L�[�����݂��Ȃ���΂Ƃ΂�
                    if (!eachCharaData_[(int)nowTurnChar_].charaBstTurn.ContainsKey((CONDITION)i))
                    {
                        continue;
                    }
                    // -1�^�[������
                    eachCharaData_[(int)nowTurnChar_].charaBstTurn[(CONDITION)i]--;
                    // �^�[������0�ȉ��ɂȂ�����A�}�b�v����폜����
                    if(eachCharaData_[(int)nowTurnChar_].charaBstTurn[(CONDITION)i] <= 0)
                    {
                        charasList_[(int)nowTurnChar_].ConditionReset(false, i);    // 0�ȉ��ɂȂ������̂�����
                        eachCharaData_[(int)nowTurnChar_].charaBstTurn.Remove((CONDITION)i);
                        badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                    }
                }
                // �S�Ă̏�Ԉُ킪�������Ƃ�
                if(eachCharaData_[(int)nowTurnChar_].charaBstTurn.Count <= 0)
                {
                    // CONDITION��NON�ɖ߂�
                    charasList_[(int)nowTurnChar_].ConditionReset(true);
                    badStatusMng_.SetBstIconImage((int)nowTurnChar_, -1, charaBstIconImage_, charasList_[(int)nowTurnChar_].GetBS(), true);
                    Debug.Log("�L������Ԉُ킪�S�Ď�����");
                }

                // �o�t��1�^�[������������
                if(!charasList_[(int)nowTurnChar_].CheckBuffTurn())
                {
                    // false�̏��(=�����̃o�t�����ꂽ��)
                    var buffMap = charasList_[(int)nowTurnChar_].GetBuff();
                    for(int i = 0; i < buffMap.Count; i++)
                    {
                        if (buffMap[i + 1].Item2 > 0)   
                        {
                            continue;
                        }

                        // ���ʂ��؂ꂽ(=�^�[����0�ȉ�)
                        var child = eachCharaData_[(int)nowTurnChar_].buffIconParent.transform.GetChild(i);
                        if(child.GetComponent<Image>().sprite != null)
                        {
                            // �A�C�R����null�ɂ��āA�㏸������\���ɂ���
                            child.GetComponent<Image>().sprite = null;
                            for (int m = 0; m < child.childCount; m++)
                            {
                                child.GetChild(m).gameObject.SetActive(false);
                            }
                        }
                    }
                }

                // �O�̃L�����̍s�����I���������^�[�����ڂ�
                buttleMng_.SetMoveTurn();

                buttleAnounceText_.text = announceText_[0];

                // ���̃L�������s���ł���悤�ɂ���
                // �ő�܂ŉ��Z���ꂽ��A�����l�ɖ߂�(�O���Z�q�d�v)
                if (++nowTurnChar_ >= CHARACTERNUM.MAX)
                {
                    nowTurnChar_ = CHARACTERNUM.UNI;
                }

                if (charasList_[(int)nowTurnChar_].HP() <= 0)
                {
                    Debug.Log("�L���������S");
                    //anim_ = ANIMATION.DEATH;
                }
                else
                {
                    anim_ = ANIMATION.IDLE;
                }

                selectFlg_ = false;
                buttleCommandRotate_.SetEnableAndActive(true);
                buttleCommandFrame_.SetActive(true);

                myTurnOnceFlg_ = false;

                // �R�}���h�摜��\���ɂ���
                for (int i = 0; i < buttleCommandImage_.Length; i++)
                {
                    buttleCommandImage_[i].SetActive(true);
                }

                for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                {
                    eachCharaData_[i].charaArrowImage.SetActive(false);
                }
                break;
            case ANIMATION.BEFORE:
                oldTurnChar_ = nowTurnChar_;

                Debug.Log("�O�̃L�������s���I��");
                selectFlg_ = true;
                buttleAnounceText_.text = announceText_[1];

                buttleCommandRotate_.gameObject.SetActive(false);
                buttleEnemySelect_.SetActive(true);

                break;
            case ANIMATION.ATTACK:
                if (charasList_[(int)nowTurnChar_].Attack())
                {
                    // �����ŃL�����̍U���͂Ƒ��x�ƍK�^��ButtleMng.cs�ɓn��
                    buttleMng_.SetDamageNum(charasList_[(int)nowTurnChar_].Damage());
                    buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
                    buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

                    AttackStart((int)nowTurnChar_);
                    buttleCommandRotate_.gameObject.SetActive(true);
                    buttleEnemySelect_.SetActive(false);
                }
                break;
            case ANIMATION.AFTER:
                AfterAttack((int)nowTurnChar_);
                break;
            default:
                break;
        }
    }

    // �U����������
    void BeforeAttack()
    {
        // �s���O�ɔ�������o�b�h�X�e�[�^�X�̏���
        badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

        // �L�����̈ʒu���擾����
        charaPos_ = charasList_[(int)nowTurnChar_].GetButtlePos();
        // �G�̈ʒu���擾����
        enePos_ = buttleEnemySelect_.GetSelectEnemyPos(buttleEnemySelect_.GetSelectNum()[0]);
        enePos_.y = 0.0f;        // ������0.0f�ɂ��Ȃ��Ǝ΂ߏ�����ɔ��ł��܂�

        // �s�����̃L�������A�U���Ώۂ̕����ɑ̂�������
        // charMap_�̏��𒼐ڕύX����K�v�����邽�߁AcharMap_[nowTurnChar_]�ƋL�q���Ă���
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos_ - charaPos_);


        if ((int)nowTurnChar_ == (int)CHARACTERNUM.JACK)
        {
            // �G�Ɍ������đ��鏈��
            StartCoroutine(MoveToEnemyPos());
        }
        else
        {
            anim_ = ANIMATION.ATTACK;    // ���j
        }

    }

    // �U���ւ̈ړ��R���[�`��  
    private IEnumerator MoveToEnemyPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = charasList_[(int)nowTurnChar_].RunMove(time, charMap_[nowTurnChar_].transform.localPosition, enePos_);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // �L�������W��������

            //Debug.Log("�W���b�N���ݒl" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.ATTACK;    // �U�����[�V�����ڍs�m�F�ؑ�

    }

    void AfterAttack(int charNum)
    {
        // �L�����̈ʒu���擾����
        charaPos_ = charasList_[charNum].GetButtlePos();

        if (charNum == (int)CHARACTERNUM.JACK)
        {
            // �������ʒu�ɖ߂鏈��
            StartCoroutine(MoveToInitPos());
        }
        else
        {
            anim_ = ANIMATION.IDLE;    // ���j
        }
    }

    // �U������߂��Ă���R���[�`��  
    private IEnumerator MoveToInitPos()
    {
        bool flag = false;
        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / 25.0f;  // deltaTime�������ƈړ����������邽�߁A�C�ӂ̒l�Ŋ���

            var tmp = charasList_[(int)nowTurnChar_].BackMove(time, charMap_[nowTurnChar_].transform.localPosition, buttleWarpPointsPos_[(int)CHARACTERNUM.JACK]);
            flag = tmp.Item2;   // while���𔲂��邩�t���O��������
            charMap_[nowTurnChar_].transform.localPosition = tmp.Item1;     // �L�������W��������

            //Debug.Log("�W���b�N���ݒl" + charMap_[nowTurnChar_].transform.localPosition);
        }

        anim_ = ANIMATION.IDLE;

    }


    void AttackStart(int charNum)
    {
        string str = "";

        if (charNum == (int)CHARACTERNUM.UNI)
        {
            // �ʏ�U���e�̕����̌v�Z
            var dir = (enePos_ - charaPos_).normalized;
            // �G�t�F�N�g�̔����ʒu��������
            var adjustPos = new Vector3(charaPos_.x, charaPos_.y + 0.5f, charaPos_.z);

            // �ʏ�U���e�v���n�u���C���X�^���X��
            var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);
            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // �ʏ�U���e�̔��ł����������w��
            magicMove.SetDirection(dir);

            // ���O�̐ݒ�
            str = "UniAttack(Clone)";
        }
        else if (charNum == (int)CHARACTERNUM.JACK)
        {
            // ���O�̐ݒ�
            str = "Axe1h";
        }
        else
        {
            return;  // �����������s��Ȃ�
        }

        if (str == "")
        {
            Debug.Log("�G���[�F�����������Ă��܂���");
            return; // �����������Ă��Ȃ��ꍇ��return����
        }

        // [Weapon]�̃^�O�����Ă���I�u�W�F�N�g��S�Č�������
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // �������I�u�W�F�N�g�̖��O���r���āA����U���Ɉ�������ɂ��Ă���CheckAttackHit�֐��̐ݒ���s��
            if (weaponTagObj[i].name == str)
            {
                // ����R���C�_�[�̗L����
                if (str == "Axe1h")
                {
                    weaponTagObj[i].GetComponent<BoxCollider>().enabled = true;
                }

                // �I�������G�̔ԍ���n��
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(buttleEnemySelect_.GetSelectNum()[0] + 1,-1);
            }
        }

    }

    void MagicAttack()
    {
        // �s���O�ɔ�������o�b�h�X�e�[�^�X�̏���
        badStatusMng_.BadStateMoveBefore(charasList_[(int)nowTurnChar_].GetBS(), charasList_[(int)nowTurnChar_], eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item1, true);

        // �L�����̈ʒu���擾����
        charaPos_ = charasList_[(int)nowTurnChar_].GetButtlePos();

        // ���x�ƍK�^��n��
        buttleMng_.SetSpeedNum(charasList_[(int)nowTurnChar_].Speed());
        buttleMng_.SetLuckNum(charasList_[(int)nowTurnChar_].Luck());

        // MP��������
        StartCoroutine(eachCharaData_[(int)nowTurnChar_].charaHPMPMap.Item2.MoveSlideBar
        (charasList_[(int)nowTurnChar_].MP() - mpDecrease_, charasList_[(int)nowTurnChar_].MP()));
        // �������l�̕ύX���s��
        charasList_[(int)nowTurnChar_].SetMP(charasList_[(int)nowTurnChar_].MP() - mpDecrease_);

        // ���@�ł̍U���Ώۂ����肵���Ƃ��ɓ���
        if (useMagic_.GetElementAndSub1Num().Item1 >= 2 ||  // �������@�U���������́A�G�ւ̃f�o�t(�⏕���@)
            useMagic_.GetElementAndSub1Num().Item1 == 1 && useMagic_.GetElementAndSub1Num().Item2 == 1) 
        {
            // �I�������G�̔ԍ���n��
            var tmp = buttleEnemySelect_.GetSelectNum();
            //int[] tmp = { 0, 0, 0, 0 }; // �f�o�b�O�p(�D���Ȑ��l�ōU���Ώۂ����߂���)
            for (int i = 0; i < tmp.Length; i++)
            {
                // tmp���e��-1�ȊO�Ȃ�U���Ώۂ�����̂ŏ������s��
                if (tmp[i] >= 0)
                {
                    // �G�̈ʒu���擾����
                    enePos_ = buttleEnemySelect_.GetSelectEnemyPos(tmp[i]);
                    enePos_.y = 0.0f;        // ������0.0f�ɂ��Ȃ��Ǝ΂ߏ�����ɔ��ł��܂�
                                             // ����ݒ肷��
                    useMagic_.InstanceMagicInfo(charaPos_, enePos_, tmp[i], i);
                }
            }
            StartCoroutine(useMagic_.InstanceMagicCoroutine());
        }

        buttleCommandRotate_.gameObject.SetActive(true);
        buttleEnemySelect_.SetActive(false);
        mpDecrease_ = 0;

        // IDLE�ɐ؂�ւ��ƓG�̎��S�����̔�����s���̂ŁA�Ԃ��J����悤�ɃR���[�`���Œ��߂���
        StartCoroutine(ChangeIDLETiming());
    }

    private IEnumerator ChangeIDLETiming()
    {
        Debug.Log("�X�^�[�g");
        yield return new WaitForSeconds(3.0f);
        Debug.Log("�X�^�[�g����3�b��");

        anim_ = ANIMATION.IDLE;
    }

    // ButtleMng.cs�ŎQ��
    public bool GetLastEnemyToAttackFlg()
    {
        // ���ʒu�̃��Z�b�g���s��(false�Ȃ�A�G��S�ē|�����Ƃ������ƂȂ̂Ńt���O��؂�ւ���)
        lastEnemytoAttackFlg_ = !buttleEnemySelect_.ResetSelectPoint();
        return lastEnemytoAttackFlg_;
    }

    public void SetCharaFieldPos()
    {
        charMap_[CHARACTERNUM.UNI].gameObject.transform.position = buttleMng_.GetFieldPos();
    }

    public void HPdecrease(int num,int fromNum)
    {
        int hitProbabilityOffset = 0;   // ������
        var damage = 0;                 // �_���[�W�l�̎Z�o
        bool criticallFlg = false;      // �N���e�B�J�����f�p�̃t���O

        // �N���e�B�J���̌v�Z������(��b�l�ƍK�^�l�ŏ�������߂�)
        int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
        if(criticalRand <= 10 + buttleMng_.GetLuckNum())
        {
            // �N���e�B�J������(�K��+�_���[�W2�{)10�̓N���e�B�J���̊�b�l
            Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "�Ȃ̂ŁA�G�̍U�����N���e�B�J���I");
            // �N���e�B�J���_���[�W
            damage = (buttleMng_.GetDamageNum() * 2) - charasList_[num].Defence(true);

            hitProbabilityOffset = 200; // 100�ȏ�̐������K�v�ɂȂ�(�o�X�e�t�^���ɍK�^�l+�����_����100���z����\�������邩��)
            criticallFlg = true;
        }
        else
        {
            // �N���e�B�J������Ȃ��Ƃ�
            Debug.Log(criticalRand + ">" + (10 + charasList_[num].Luck()) + "�Ȃ̂ŁA�G�̍U���̓N���e�B�J���ł͂Ȃ�");

            // �����v�Z������
            // �@�U�����鑤��Speed / �U������鑤��Speed * 100 = ���̏o��
            var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)charasList_[num].Speed() * 100.0f);
            // �A�L�������G��+10���̕␳�l������B
            hitProbabilityOffset = hitProbability + 10;
            // �BhitProbabilityOffset��100�ȏ�Ȃ玩�������ŁA����ȉ��Ȃ烉���_���l�����B
            if (hitProbabilityOffset < 100)
            {
                int rand = Random.Range(0, 100);
                Debug.Log("������" + hitProbabilityOffset + "�����_���l" + rand);

                if (rand <= hitProbabilityOffset)
                {
                    // ����
                    Debug.Log(rand + "<=" + hitProbabilityOffset + "�Ȃ̂ŁA����");
                }
                else
                {
                    // ���
                    Debug.Log(rand + ">" + hitProbabilityOffset + "�Ȃ̂ŁA���");
                    DamageIcon(num, "�~�X",false,false);
                    return;
                }
            }
            else
            {
                Debug.Log("������" + hitProbabilityOffset + "��100�ȏ�Ȃ̂ŁA��������");
            }

            int tmpLuck;

            // �������ɂ�Luck�ŉ�𔻒������
            // ����͈̔͂́A100 - ���݂�Luck���ő�l�ɂ��āA����𐬌��ɋ߂Â���
            if (charasList_[num].Luck() <= 10)
            {
                tmpLuck = 10;
                Debug.Log("Luck��10�ȉ��Ȃ̂ŁA10��K�p���ĉ�𔻒�����܂�");
            }
            else
            {
                tmpLuck = charasList_[num].Luck();
                Debug.Log("Luck��10�ȏ�Ȃ̂ŁA���݂̃X�e�[�^�X��Luck���g���ĉ�𔻒�����܂�");
            }

            int randLuck = Random.Range(0, 100 - tmpLuck);
            if (randLuck <= tmpLuck)
            {
                Debug.Log(randLuck + "<=" + tmpLuck + "�ȉ��Ȃ̂ŁA��𐬌�");
                DamageIcon(num, "�~�X", false, false);
                return;
            }
            else
            {
                Debug.Log(randLuck + ">" + tmpLuck + "�ȉ��Ȃ̂ŁA������s");
                damage = buttleMng_.GetDamageNum() - charasList_[num].Defence(true);
            }
        }

        // �ʏ�_���[�W
        if (damage <= 0)
        {
            Debug.Log("�G�̍U���͂��L�����̖h��͂��������̂Ń_���[�W��1�ɂȂ�܂���");
            damage = 1;
        }

        // �o�X�e��_���[�W����������ɁA���˂��z���̃o�t�𒣂��Ă��邩�m�F����
        var spbuff = charasList_[num].GetReflectionOrAbsorption();
        if(spbuff == Chara.SPECIALBUFF.REF || spbuff == Chara.SPECIALBUFF.REF_M)         // ���ˏ���
        {
            bool tmpFlg = false;
            if(!buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.REF)
            {
                // �����U���ɕ�������
                tmpFlg = true;
            }
            else if(buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.REF_M)
            {
                // ���@�U���ɖ��@����
                tmpFlg = true;
            }
            else
            {
                Debug.Log("�G�̍U���Ɣ��˂̏������Ⴄ�̂ŏ����Ȃ�");
            }

            if(tmpFlg)
            {
                // �U���l�������HP�����炷
                buttleMng_.SetRefEnemyNum(fromNum);
                charasList_[num].SetReflectionOrAbsorption(0, 1);  // NON�ɖ߂�
                Debug.Log("�U������");

                eachCharaData_[num].specialBuff.GetComponent<Image>().sprite = null;
                eachCharaData_[num].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                return;
            }
        }
        else if(spbuff == Chara.SPECIALBUFF.ABS || spbuff == Chara.SPECIALBUFF.ABS_M)    // �z������
        {
            bool tmpFlg = false;
            if (!buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.ABS)
            {
                // �����U���ɕ����z��
                tmpFlg = true;
            }
            else if (buttleMng_.GetIsAttackMagicFlg() && spbuff == Chara.SPECIALBUFF.ABS_M)
            {
                // ���@�U���ɖ��@�z��
                tmpFlg = true;
            }
            else
            {
                Debug.Log("�G�̍U���Ƌz���̏������Ⴄ�̂ŏ����Ȃ�");
            }

            if (tmpFlg)
            {
                // �U���l�̔�����HP�񕜂���(�h��͂���������l�ɂȂ�Ȃ��悤��Get�֐����璼�ڒl���Ƃ��Ă���)
                charasList_[num].SetHP(charasList_[num].HP() + (buttleMng_.GetDamageNum() / 2));
                charasList_[num].SetReflectionOrAbsorption(0, 1);  // NON�ɖ߂�
                Debug.Log("�U���z��");

                eachCharaData_[num].specialBuff.GetComponent<Image>().sprite = null;
                eachCharaData_[num].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "";
                return;
            }
        }
        else
        {
            // �����������s��Ȃ�
        }

        DamageIcon(num,damage.ToString(), false,criticallFlg);

        // �o�b�h�X�e�[�^�X���t�^����邩����
        charasList_[num].SetBS(buttleMng_.GetBadStatus(), hitProbabilityOffset);

        var getBs = charasList_[num].GetBS();
        // �o�X�e�̌��ʎ����^�[����ݒ肷��
        for(int i = 0; i < (int)CONDITION.DEATH; i++)
        {
            // ���N��ԈȊO�̃R���f�B�V�����̃t���O��true�ɂȂ��Ă�����
            if(getBs[i].Item2 && getBs[i].Item1 != CONDITION.NON)
            {
                if (!eachCharaData_[num].charaBstTurn.ContainsKey(getBs[i].Item1))
                {
                    eachCharaData_[num].charaBstTurn.Add(getBs[i].Item1, Random.Range(1, 5));// 1�ȏ�5����
                    badStatusMng_.SetBstIconImage(num, (int)getBs[i].Item1, charaBstIconImage_, charasList_[num].GetBS());
                    break;
                }
            }
        }

        // �����p�ɏ����Ăяo��
        var bst = badStatusMng_.BadStateMoveBefore(getBs, charasList_[num], eachCharaData_[num].charaHPMPMap.Item1, true);
        if (bst == (CONDITION.DEATH, true))   // ��������
        {
            // �g����A�C�e���������Ă��邩���ׂ�
            if(Bag_Item.itemState[deathBstNoEffectItemNum_].haveCnt > 0)
            {
                Bag_Item.itemState[deathBstNoEffectItemNum_].haveCnt--;
                Debug.Log("�A�C�e���������������肵�Ă��ꂽ");
                return;
            }

            StartCoroutine(eachCharaData_[num].charaHPMPMap.Item1.MoveSlideBar(charasList_[num].HP() - 999, charasList_[num].HP()));
            charasList_[num].SetHP(charasList_[num].HP() - 999);
        }
        else
        {
            // �L������HP�����(�X���C�h�o�[�ύX)
            StartCoroutine(eachCharaData_[num].charaHPMPMap.Item1.MoveSlideBar(charasList_[num].HP() - damage, charasList_[num].HP()));
            // �������l�̕ύX���s��
            charasList_[num].SetHP(charasList_[num].HP() - damage);
        }

        if (charasList_[num].HP() <= 0)
        {
            Debug.Log("�L���������S");
            charasList_[num].SetHP(0);
            // Chara.cs�Ɏ��S��������
            charasList_[num].SetDeathFlg(true);
        }
    }

    //@ �s����������(���S���̉񕜂�noeffect�ɂ����ق�������)
    public void SetCharaArrowActive(bool allflag, bool randFlg,int whatHeal,int sub3Num)
    {
        // �N��1�l��true�ɂ���ꍇ�́A�ォ�珇�Ȃ̂�0�Ԃ̐l��true�ɂ���
        if(!allflag)
        {
            eachCharaData_[0].charaArrowImage.SetActive(true);
        }
        else
        {
            // �S���\����Ԃɂ���Ȃ�for���ŉ�
            for(int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                eachCharaData_[i].charaArrowImage.SetActive(true);
            }
        }

        // �������l��-1�ɂ��Ă���
        int[] tmpArray = { -1, -1, -1, -1 };

        // �������S�̂ŏ����𕪂���
        if (randFlg)
        {
            // �񕜉񐔂������_���ɂ���(2�`4��Ƃ���)
            int randHealNum = Random.Range(2, 5); // 2�ȏ�5�����̒l���ł�
            Debug.Log("������HP�񕜂̉񐔂�" + randHealNum + "��Ɍ��肵�܂���");

            // ���S���̃L�������܂߂ă����_���Ō��肷��
            for (int i = 0; i < randHealNum; i++)
            {
                tmpArray[i] = Random.Range(0, (int)CHARACTERNUM.MAX);// 0�ȏ�MAX�����̒l���ł�
            }
        }
        else
        {
            // �S��
            // ���S���̃L�������܂߂đS�̉񕜂Ƃ���
            for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
            {
                tmpArray[i] = i;
            }
        }

        if(useMagic_.GetElementAndSub1Num().Item1 == 0) 
        {
            // �񕜏�����
            rest_ = SelectToHealMagicChara(allflag, tmpArray, whatHeal);
        }
        else
        {
            // �⏕������(�o�t)
            rest_ = SelectToBuffMagicChara(allflag, tmpArray, whatHeal,sub3Num);
        }

        StartCoroutine(rest_);
    }

    private IEnumerator SelectToHealMagicChara(bool allflag, int[] array,int whatHeal)
    {
        bool flag = false;
        var selectChara = CHARACTERNUM.UNI;

        while (!flag)
        {
            yield return null;

            if (!allflag)    // �P�̉񕜖��@�̔�����
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    // ���j��萔�l���������Ȃ�Ȃ��悤�ɂ���
                    if (--selectChara < CHARACTERNUM.UNI)
                    {
                        selectChara = CHARACTERNUM.UNI;
                    }
                    // 1�x�S��false�ɂ���
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // �Y������L�����̖�󂾂�true�ɂ���
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    // �W���b�N��萔�l���傫���Ȃ�Ȃ��悤�ɂ���
                    if (++selectChara > CHARACTERNUM.JACK)
                    {
                        selectChara = CHARACTERNUM.JACK;
                    }
                    // 1�x�S��false�ɂ���
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // �Y������L�����̖�󂾂�true�ɂ���
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else
                {
                    // �����������s��Ȃ�
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!allflag)    // �P�̉�
                {
                    // enePos��targetNum�͓���Ȃ��Ă悢(charaPos_�ɔ�������̍��W������悤�ɂ���)
                    useMagic_.InstanceMagicInfo(charasList_[(int)selectChara].GetButtlePos(), new Vector3(-1, -1, -1), -1, 0);

                    if(whatHeal == 0)   // HP��
                    {
                        // �L������HP�𑝂₷(�X���C�h�o�[�ύX)
                        StartCoroutine(eachCharaData_[(int)selectChara].charaHPMPMap.Item1.MoveSlideBar(charasList_[(int)selectChara].HP() + useMagic_.GetHealPower(), charasList_[(int)selectChara].HP()));
                        // �������l�̕ύX���s��
                        charasList_[(int)selectChara].SetHP(charasList_[(int)selectChara].HP() + useMagic_.GetHealPower());
                    }
                    else
                    {
                        // �ŁE�ÈŁE��჉�
                        charasList_[(int)selectChara].ConditionReset(false,whatHeal);
                        badStatusMng_.SetBstIconImage((int)selectChara, -1, charaBstIconImage_, charasList_[(int)selectChara].GetBS(), true);
                    }
                }
                else
                {
                    // ������or�S�̉�
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= -1)
                        {
                            break;
                        }

                        // enePos��targetNum�͓���Ȃ��Ă悢(charaPos_�ɔ�������̍��W������悤�ɂ���)
                        useMagic_.InstanceMagicInfo(charasList_[array[i]].GetButtlePos(), new Vector3(-1, -1, -1), -1, i);

                        if (whatHeal == 0)
                        {
                            // �L������HP�𑝂₷(�X���C�h�o�[�ύX)
                            StartCoroutine(eachCharaData_[array[i]].charaHPMPMap.Item1.MoveSlideBar(charasList_[array[i]].HP() + useMagic_.GetHealPower(), charasList_[array[i]].HP()));
                            // �������l�̕ύX���s��
                            charasList_[array[i]].SetHP(charasList_[array[i]].HP() + useMagic_.GetHealPower());
                            Debug.Log((CHARACTERNUM)array[i] + "��HP���A" + useMagic_.GetHealPower() + "�񕜂��܂���");
                        }
                        else
                        {
                            // �ŁE�ÈŁE��჉�
                            charasList_[array[i]].ConditionReset(false, whatHeal);
                            badStatusMng_.SetBstIconImage(array[i], -1, charaBstIconImage_, charasList_[array[i]].GetBS(), true);
                            Debug.Log((CHARACTERNUM)array[i] + "�̏�Ԉُ���񕜂��܂���");
                        }
                    }
                }

                StartCoroutine(useMagic_.InstanceMagicCoroutine());

                flag = true;
            }
        }
    }

    private IEnumerator SelectToBuffMagicChara(bool allflag, int[] array, int whatBuff,int sub3Num)
    {
        bool flag = false;
        var selectChara = CHARACTERNUM.UNI;

        // �o�t�̃A�C�R������
        System.Action<int,int> action = (int charaNum,int buffnum) => {
            var bufftra = eachCharaData_[charaNum].buffIconParent.transform;
            for (int i = 0; i < bufftra.childCount; i++)
            {
                if (bufftra.GetChild(i).GetComponent<Image>().sprite == null)
                {
                    // �A�C�R���������
                    bufftra.GetChild(i).GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff - 1];
                    // ���ŃA�b�v�{���������
                    // ��*1 = �o�t��1% ~30%,��*2 = �o�t��31% ~70%,��*3 = �o�t��71%~100%
                    for (int m = 0; m < bufftra.GetChild(i).childCount; m++)
                    {
                        if (m <= buffnum)    // buffnum�̐����ȉ��Ȃ�true�ɂ��ėǂ�
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(true);
                        }
                        else
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(false);
                        }
                    }

                    break;
                }
            }
        };

        while (!flag)
        {
            yield return null;

            if (!allflag)    // �P�̉񕜖��@�̔�����
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    // ���j��萔�l���������Ȃ�Ȃ��悤�ɂ���
                    if (--selectChara < CHARACTERNUM.UNI)
                    {
                        selectChara = CHARACTERNUM.UNI;
                    }
                    // 1�x�S��false�ɂ���
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // �Y������L�����̖�󂾂�true�ɂ���
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else if (Input.GetKeyDown(KeyCode.J))
                {
                    // �W���b�N��萔�l���傫���Ȃ�Ȃ��悤�ɂ���
                    if (++selectChara > CHARACTERNUM.JACK)
                    {
                        selectChara = CHARACTERNUM.JACK;
                    }
                    // 1�x�S��false�ɂ���
                    for (int i = 0; i < (int)CHARACTERNUM.MAX; i++)
                    {
                        eachCharaData_[i].charaArrowImage.SetActive(false);
                    }
                    // �Y������L�����̖�󂾂�true�ɂ���
                    eachCharaData_[(int)selectChara].charaArrowImage.SetActive(true);
                }
                else
                {
                    // �����������s��Ȃ�
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!allflag)    // �P�̃o�t
                {
                    // enePos��targetNum�͓���Ȃ��Ă悢(charaPos_�ɔ�������̍��W������悤�ɂ���)
                    useMagic_.InstanceMagicInfo(charasList_[(int)selectChara].GetButtlePos(), new Vector3(-1, -1, -1), -1, 0);

                    if(sub3Num == 0)
                    {
                        // �㏸
                        var buffnum = charasList_[(int)selectChara].SetBuff(useMagic_.GetTailNum(), whatBuff);
                        if(!buffnum.Item2)
                        {
                            action((int)selectChara, buffnum.Item1);
                        }
                    }
                    else
                    {
                        // ���˂��z��
                        charasList_[(int)selectChara].SetReflectionOrAbsorption(whatBuff-1, sub3Num);
                        eachCharaData_[(int)selectChara].specialBuff.GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff + 3];
                        if(sub3Num == 2)
                        {
                            eachCharaData_[(int)selectChara].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "��";
                        }
                        else
                        {
                            eachCharaData_[(int)selectChara].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "�z";
                        }
                    }
                }
                else
                {
                    // ������or�S�̉�
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= -1)
                        {
                            break;
                        }

                        // enePos��targetNum�͓���Ȃ��Ă悢(charaPos_�ɔ�������̍��W������悤�ɂ���)
                        useMagic_.InstanceMagicInfo(charasList_[array[i]].GetButtlePos(), new Vector3(-1, -1, -1), -1, i);

                        if (sub3Num == 0)
                        {
                            // �㏸
                            var buffnum = charasList_[array[i]].SetBuff(useMagic_.GetTailNum(), whatBuff);
                            if(!buffnum.Item2)
                            {
                                action(array[i], buffnum.Item1);
                            }
                        }
                        else
                        {
                            // ���˂��z��
                            charasList_[array[i]].SetReflectionOrAbsorption(whatBuff-1, sub3Num);
                            eachCharaData_[array[i]].specialBuff.GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff + 3];
                            if (sub3Num == 2)
                            {
                                eachCharaData_[array[i]].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "��";
                            }
                            else
                            {
                                eachCharaData_[array[i]].specialBuff.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "�z";
                            }
                        }
                    }
                }
                StartCoroutine(useMagic_.InstanceMagicCoroutine());
                flag = true;
            }
        }
    }

    // �_���[�W�A�C�R���̕\������
    private void DamageIcon(int num,string str,bool weakFlg,bool criticallFlg)
    {
        // �_���[�W�A�C�R��
        for (int i = 0; i < buttleDamageIconsObj_.transform.childCount; i++)
        {
            var tmp = buttleDamageIconsObj_.transform.GetChild(i).gameObject;
            if (tmp.activeSelf)
            {
                continue;
            }

            // �܂���\����Ԃ̎g���Ă��Ȃ��A�C�R�����������Ƃ�
            // ���W���_���[�W�L�����̓���ɂ���
            buttleDamageIconsObj_.transform.GetChild(i).transform.localPosition = buttleDamgeIconPopUpPos_[num];
            // �_���[�W���l������
            tmp.transform.GetChild(0).GetComponent<Text>().text = str;
            // �\����Ԃɂ���
            tmp.SetActive(true);

            tmp.transform.GetChild(0).Find("Weak").gameObject.SetActive(weakFlg);
            tmp.transform.GetChild(0).Find("Critical").gameObject.SetActive(criticallFlg);

            break;
        }
    }
}
