using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtleResult : MonoBehaviour
{
    [SerializeField]
    private Canvas resultCanvas;    // �퓬������ɕ\�����郊�U���g�p�L�����o�X
    [SerializeField]
    private GameObject dropPrefab;    // Drop����\������摜
    [SerializeField]
    private Sprite[] CharaImage;// �L�����摜

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;        // �t�B�[���h���̓G����ۑ�����
    private List<Enemy> enemyList_ = new List<Enemy>();

    // �o���l�֘A
    private Slider[] expSlider_ = new Slider[(int)SceneMng.CHARACTERNUM.MAX];    // Exp�p��Slider
    private TMPro.TextMeshProUGUI[] expText_ = new TMPro.TextMeshProUGUI[(int)SceneMng.CHARACTERNUM.MAX];  // ���ݐ��l��\������e�L�X�g
    private TMPro.TextMeshProUGUI[] levelText_ = new TMPro.TextMeshProUGUI[(int)SceneMng.CHARACTERNUM.MAX];  // ���ݐ��l��\������e�L�X�g
    private static int[] level_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// �L�����̌��݃��x��
    private static int[] maxExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// �L�����̃��x���ɑ΂������o���l
    private static int[] nowExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// �L�����̌o���l
    private static int[] nextExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// ���̃��x���܂łɕK�v��EXP
    private int[] oldLevel_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// ���x���A�b�v����O�̃��x��
    private int[] getExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// �l��EXP
                                                                    //   private int saveSumExp_ = 0;// �l��EXP�̍��v
    private int[] saveSumExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];// �l��EXP�̍��v
    private int[] saveSumMaxExp_ = new int[(int)SceneMng.CHARACTERNUM.MAX];

    // �G�̃��x��
    private int[] enemyLv_;//
    // �h���b�v�֘A
    private GameObject[] dropObj_;  // �v���n�u�������Ɏg�p
    private Image[] dropImage_;     // �ǂ̑f�ނ��E������
    private Text[] dropCntText_;    // ���h���b�v��������\��
    private int enemyCnt_ = 0;// �G�̐����h���b�v�\����
    private int[] dropCnt_;        // ���h���b�v������

    // Chara.cs���L�������Ƀ��X�g������
    private List<Chara> charasList_ = new List<Chara>();
    public static bool onceFlag_ = false;// ���[�h���Ă����1�񂵂�����Ȃ��Ă����ӏ�

    // ���x���A�b�v���̃X�e�[�^�X�\���֘A
    private RectTransform levelMng;    // ���x���A�b�v���Ɏg�p����Mng
    private Image charaImage_;      // ���x�����オ�����L�����̕\��
    private RectTransform charaRect_;
    private Text levelUpText_;        // �ǂ̂��炢���x�����オ������
    private Text nextExpText_;      // ���̃��x���܂ł̕K�v�o���l
    private Text statusText_;       // �X�e�[�^�X�̎��
    private Text statusNumText_;    // ���Z���ꂽ�X�e�[�^�X�̒l�i�ω����Ȃ����̂��܂ށj

    // �o�b�O
    private Bag_Materia bagMateria_;

    void Start()
    {
        resultCanvas.gameObject.SetActive(false);
        //DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        // StreamingAssets����AssetBundle�����[�h����
        var assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/datapop");
        Debug.Log("assetBundle�J��");
        // AssetBundle���̃A�Z�b�g�ɂ̓r���h���̃A�Z�b�g�̃p�X�A�܂��̓t�@�C�����A�t�@�C�����{�g���q�ŃA�N�Z�X�ł���
        DataPopPrefab_ = assetBundle.LoadAsset<GameObject>("DataPop.prefab");
        // �s�v�ɂȂ���AssetBundle�̃��^�����A�����[�h����
        assetBundle.Unload(false);
        Debug.Log("�j��");

        Debug.Log("resultCanvas" + resultCanvas.transform.Find("UniIconFrame/EXPSlider").GetComponent<Slider>());
        expSlider_[(int)SceneMng.CHARACTERNUM.UNI] = resultCanvas.transform.Find("UniIconFrame/EXPSlider").GetComponent<Slider>();
        expSlider_[(int)SceneMng.CHARACTERNUM.JACK] = resultCanvas.transform.Find("JackIconFrame/EXPSlider").GetComponent<Slider>();

        charasList_ = SceneMng.charasList_;

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            if (onceFlag_ == false)
            {
                level_[i] = charasList_[i].Level();
                maxExp_[i] = charasList_[i].CharacterMaxExp();
                nowExp_[i] = charasList_[i].CharacterExp();
            }

            // ���x���֘A
            oldLevel_[i] = level_[i];
            levelText_[i] = expSlider_[i].transform.Find("LvText").GetComponent<TMPro.TextMeshProUGUI>();
            levelText_[i].text = "Lv " + level_[i].ToString();

            // �o���l�֘A
            expText_[i] = expSlider_[i].transform.Find("AddExpText").GetComponent<TMPro.TextMeshProUGUI>();
            expSlider_[i].maxValue = maxExp_[i];
            expSlider_[i].value = nowExp_[i];
            Debug.Log(i + "   " + level_[i] + "���x��" + charasList_[i].Level());
            Debug.Log(i + "   " + charasList_[i].CharacterExp() + "�o���l" + charasList_[i].CharacterMaxExp());
        }
        onceFlag_ = true;

        // ���x���A�b�v���̃X�e�[�^�X�\���֘A
        levelMng = resultCanvas.transform.Find("LvUpMng").GetComponent<RectTransform>();
        levelUpText_ = levelMng.Find("LevelBackImage/LevelUpText").GetComponent<Text>();
        nextExpText_ = levelMng.Find("NextLevelBackImage/NextLevelText").GetComponent<Text>();
        statusText_ = levelMng.Find("StatusUpBack/StatusNameText").GetComponent<Text>();
        statusNumText_ = levelMng.Find("StatusUpBack/StatusNumText").GetComponent<Text>();
        charaImage_ = levelMng.Find("CharaImage").GetComponent<Image>();
        charaRect_ = charaImage_.GetComponent<RectTransform>();
        levelMng.gameObject.SetActive(false);

        // �f�ގ擾�p
        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();
    }

    public void DropCheck(int enemyCnt, int[] num, bool bossFlag, List<GameObject> list)
    {
        Debug.Log("���U���g��\�����܂�");
        enemyCnt_ = enemyCnt;

        // �f�ތn
        if (enemyData_ == null)
        {
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, (int)SceneMng.nowScene - (int)SceneMng.SCENE.FIELD0, name);
        }

        int[] materiaNum = new int[enemyCnt_];
        enemyLv_ = new int[enemyCnt_];
        Debug.Log(enemyCnt_);
        for (int i = 0; i < enemyCnt_; i++)
        {
            Debug.Log(enemyCnt + "�̒� " + num[i] + "�̖�");
            enemyList_.Add(new Enemy(num[i].ToString(), 1, null, enemyData_.param[num[i]]));
            Debug.Log(i + "�ԖځF" + enemyList_[i].GetExp());
            Debug.Log(i + "�ԖځF" + enemyList_[i].DropMateria());
            // Drop���̔ԍ����m�F����
            materiaNum[i] = int.Parse(Regex.Replace(enemyList_[i].DropMateria(), @"[^0-9]", ""));
            enemyLv_[i] = enemyList_[i].Level();  //saveSumMaxExp_[i] = 10;
            for (int c = 0; c < (int)SceneMng.CHARACTERNUM.MAX; c++)
            {
                // ���x�����ɂ��o���l�ʃ`�F�b�N
                saveSumExp_[c] += ExpCheck(c, i);
            }

            // �����N�G�X�g�̓������m�F
            for (int k = 0; k < list.Count; k++)
            {
                // ���O�����̃A���_�[�o�[�ŕ�����
                var name = enemyList_[i].Name().Split('_');

                // �����Ώۂ̓G��|�����̂��m�F����
                if (list[k].GetComponent<CompleteQuest>().GetEnemyName() == name[0])
                {
                    list[k].GetComponent<CompleteQuest>().SetFinSubjugation(1);
                }
            }
        }

        resultCanvas.gameObject.SetActive(true);
        // �\������e�̈ʒu���m��
        Transform dropParent = resultCanvas.transform.Find("DropMng/Viewport/DropParent").GetComponent<Transform>();

        dropCnt_ = new int[enemyCnt_];
        dropObj_ = new GameObject[enemyCnt_];
        dropImage_ = new Image[enemyCnt_];
        dropCntText_ = new Text[enemyCnt_];

        for (int i = 0; i < enemyCnt; i++)
        {
            if (bossFlag == true)
            {
                if(SceneMng.SCENE.FIELD4==SceneMng.nowScene)
                {
                    // �e�A�C�e���̃h���b�v����1�`5�̃����_���Ŏ擾
                    dropCnt_[i] = Random.Range(1, 5);

                    // �h���b�v����\�����邽�߂Ƀv���n�u�𐶐�
                    dropObj_[i] = Instantiate(dropPrefab,
                        new Vector2(0, 0), Quaternion.identity, dropParent);
                    dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                    dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                    dropCntText_[i].text = "�~" + dropCnt_[i];
                    dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum[i]];
                    bagMateria_.MateriaGetCheck(materiaNum[i], dropCnt_[i]);

                }
                else
                {
                    // �{�X�������ꍇDrop�����[�h�ɂȂ邽�ߎ擾���[�h�Ɣԍ��𓾂�
                    var nameCheck = enemyList_[i].DropMateria().Split('_');
                    string wordName = nameCheck[0];// �ǂ̃��[�h��
                    materiaNum[i] = int.Parse(nameCheck[1]);// �摜�ԍ��i�G�������g�j

                    dropObj_[i] = Instantiate(dropPrefab,
                        new Vector2(0, 0), Quaternion.identity, dropParent);
                    dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                    dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                    dropCntText_[i].text = wordName;
                    dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][materiaNum[i]];
                }
            }
            else
            {
                // �e�A�C�e���̃h���b�v����1�`5�̃����_���Ŏ擾
                dropCnt_[i] = Random.Range(1, 5);

                // �h���b�v����\�����邽�߂Ƀv���n�u�𐶐�
                dropObj_[i] = Instantiate(dropPrefab,
                    new Vector2(0, 0), Quaternion.identity, dropParent);
                dropCntText_[i] = dropObj_[i].transform.Find("DropCnt").GetComponent<Text>();
                dropImage_[i] = dropObj_[i].transform.Find("DropImage").GetComponent<Image>();

                dropCntText_[i].text = "�~" + dropCnt_[i];
                dropImage_[i].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][materiaNum[i]];
                bagMateria_.MateriaGetCheck(materiaNum[i], dropCnt_[i]);
            }
        }

        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            // �o���l�̃X���C�_�[�𓮂���
            StartCoroutine(ActiveExpSlider(i, charasList_[i].GetDeathFlg(), saveSumExp_[i]));
        }
    }



    private int ExpCheck(int charaNum, int enemyNum)
    {
        int getExp = 0;
        int LvCheck = level_[charaNum] - enemyLv_[enemyNum];
        Debug.Log("���x����" + LvCheck);
        if (LvCheck <= 0)
        {
            // �L�����̃��x���̂ق����Ⴂ�ꍇ ���̂܂܂̌o���l��n��
            getExp = enemyList_[enemyNum].GetExp();
        }
        else
        {
            getExp = enemyList_[enemyNum].GetExp() - (LvCheck * 5);
            if (getExp < 5)
            {
                // �擾Exp��5�ȉ��̎���5�ɂ���
                getExp = 5;
            }
        }
        return getExp;
    }

    private IEnumerator ActiveExpSlider(int charaNum, bool deathFlag, int sumExp)
    {
        float saveValue = 0;
        // �o�g���Ŏ��S�����܂܏I�����Ă����Ƃ���,�l���o���l�𔼕���
        int nowExp = deathFlag == true ? sumExp / 2 : sumExp;
        getExp_[charaNum] = nowExp;
        expText_[charaNum].text = "+" + nowExp.ToString();
        Debug.Log(deathFlag + "���S�m�F     �l��EXP" + nowExp);
        int sumMaxExp = (int)expSlider_[charaNum].maxValue;
        bool onceFlag = true;
        while (true)
        {
            yield return null;
            if (getExp_[charaNum] <= saveValue)
            {
                if (onceFlag == true)
                {
                    // ���Z�������X���C�_�[���ړ���������ړ����I��������
                    Debug.Log(saveValue + "       �X���C�_�[�̈ړ����I�����܂���");
                    nextExp_[charaNum] = (int)(expSlider_[charaNum].maxValue - expSlider_[charaNum].value);

                    if (expSlider_[charaNum].value == expSlider_[charaNum].maxValue)
                    {
                        // �������l�ŏI����Ă��܂����ꍇ
                        level_[charaNum]++;
                        levelText_[charaNum].text = "Lv " + level_[charaNum].ToString();
                        // �����ύX
                        expSlider_[charaNum].maxValue = (int)(expSlider_[charaNum].maxValue * 1.1f);
                        Debug.Log(charaNum + "    " + level_[charaNum] + "  ���" + expSlider_[charaNum].maxValue);
                        saveSumMaxExp_[charaNum] += (int)expSlider_[charaNum].maxValue;
                        // value������܂ŗ�����0�ɖ߂�
                        expSlider_[charaNum].value = 0.0f;
                        nextExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                    }
                    onceFlag = false;
                }

                if (charaNum == (int)SceneMng.CHARACTERNUM.UNI)
                {
                    maxExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                    nowExp_[charaNum] = (int)expSlider_[charaNum].value;
                    yield break;
                }
                else
                {
                    // �W���b�N�̃X���C�_�[�ړ��܂ŏI������烌�x�����オ�������̃`�F�b�N������
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        maxExp_[charaNum] = (int)expSlider_[charaNum].maxValue;
                        nowExp_[charaNum] = (int)expSlider_[charaNum].value;
                        StartCoroutine(ActiveResult());
                        yield break;
                    }
                }
            }
            else
            {
                if (expSlider_[charaNum].maxValue <= expSlider_[charaNum].value)
                {
                    // �Y���L�����̃��x�����グ��
                    level_[charaNum]++;
                    levelText_[charaNum].text = "Lv " + level_[charaNum].ToString();
                    nowExp -= (int)expSlider_[charaNum].maxValue;
                    sumMaxExp += (int)expSlider_[charaNum].maxValue;
                    // �����ύX
                    expSlider_[charaNum].maxValue = (int)(expSlider_[charaNum].maxValue * 1.1f);
                    Debug.Log(charaNum + "    " + level_[charaNum] + "  ���" + expSlider_[charaNum].maxValue);
                    saveSumMaxExp_[charaNum] += (int)expSlider_[charaNum].maxValue;
                    // value������܂ŗ�����0�ɖ߂�
                    expSlider_[charaNum].value = 0.0f;
                }
                else
                {
                    // ����ɗ���܂ŉ��Z����
                    saveValue += 1;//
                    expSlider_[charaNum].value += 1;
                    // Debug.Log(expSlider_[charaNum].value + "  ���");
                }
            }
        }
    }

    private IEnumerator ActiveResult()
    {
        // �W���b�N�̌o���l�X���C�_�[�܂ŏI������珈��������
        // 0 Uni    1 Jack
        bool[] levelUpFlag = new bool[(int)SceneMng.CHARACTERNUM.MAX] { false, false };
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            Debug.Log(i + "�Ԗڂ̃L�����̃��x���J�ځF" + oldLevel_[i] + "��" + level_[i]);
            levelUpFlag[i] = oldLevel_[i] < level_[i] ? true : false;
            saveSumExp_[i] = 0;
        }

        // �N�̃��x�����オ���ĂȂ������牽�����Ȃ�
        if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == false
         && levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == false)
        {
            FieldMng.nowMode = FieldMng.MODE.SEARCH;
            levelMng.gameObject.SetActive(false);
            resultCanvas.gameObject.SetActive(false);
            for (int i = 0; i < enemyCnt_; i++)
            {
                // ���U���g��\����Drop�I�u�W�F�N�g�폜
                Destroy(dropObj_[i]);
            }
            enemyCnt_ = 0;
            enemyList_.Clear();
            yield break;
        }

        while (true)
        {
            yield return null;
            if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == false
             && levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == false)
            {
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                levelMng.gameObject.SetActive(false);
                resultCanvas.gameObject.SetActive(false);
                enemyCnt_ = 0;
                for (int i = 0; i < enemyCnt_; i++)
                {
                    // ���U���g��\����Drop�I�u�W�F�N�g�폜
                    Destroy(dropObj_[i]);
                }
                enemyList_.Clear();
                yield break;
            }

            if (levelMng.gameObject.activeSelf == false)
            {
                levelMng.gameObject.SetActive(true);
            }

            // ���j�̃��x���A�b�v�p�̉摜��\������
            if (levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] == true)
            {
                if (oldLevel_[(int)SceneMng.CHARACTERNUM.UNI] != level_[(int)SceneMng.CHARACTERNUM.UNI])
                {
                    LevelRelation(new Vector2(-350.0f, -105.0f), SceneMng.CHARACTERNUM.UNI,
                    oldLevel_[(int)SceneMng.CHARACTERNUM.UNI],
                    level_[(int)SceneMng.CHARACTERNUM.UNI],
                    nextExp_[(int)SceneMng.CHARACTERNUM.UNI],
                    getExp_[(int)SceneMng.CHARACTERNUM.UNI]);
                }
                // ���{�^���������X�y�[�X�L�[�����Ń��x���A�b�v�p�̉摜��\��
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    // �m�F���I�������false�ɂ���
                    levelUpFlag[(int)SceneMng.CHARACTERNUM.UNI] = false;
                }
            }
            else
            {
                if (levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] == true)
                {
                    // ���j�����x���A�b�v���ĕ\�����Ă���\�������邽�߃`�F�b�N����
                    if (oldLevel_[(int)SceneMng.CHARACTERNUM.JACK] != level_[(int)SceneMng.CHARACTERNUM.JACK])
                    {
                        LevelRelation(new Vector2(-350.0f, -240.0f),
                            SceneMng.CHARACTERNUM.JACK,
                        oldLevel_[(int)SceneMng.CHARACTERNUM.JACK],
                        level_[(int)SceneMng.CHARACTERNUM.JACK],
                        nextExp_[(int)SceneMng.CHARACTERNUM.JACK],
                          getExp_[(int)SceneMng.CHARACTERNUM.JACK]);
                    }
                    // ���{�^���������X�y�[�X�L�[�����Ń��x���A�b�v�p�̉摜��\��
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                    {
                        levelUpFlag[(int)SceneMng.CHARACTERNUM.JACK] = false;
                    }
                }
            }
        }
    }

    private void LevelRelation(Vector2 pos, SceneMng.CHARACTERNUM chara,
        int oldLevel, int nowLevel, int nextExp, int exp)
    {
        SceneMng.SetSE(15);

        int differenceLv = nowLevel - oldLevel;
        // �㏸�����镪������
        int[] tmp = new int[10] { 0, 0, 0, 0, 0, 0, 0, differenceLv, maxExp_[(int)chara]-nextExp, maxExp_[(int)chara] };
        // 0Attack 1MagicAttack 2Defence 3Speed 4Luck 5HP 6MP 7Level 8exp 9maxExp
        // ���j�A�W���b�N���ʕ���
        if ((nowLevel % 3 == 0) || (3 <= differenceLv))
        {
            tmp[3] = 2;            // 3���x�����ɃX�s�[�h���グ��
        }
        if ((nowLevel % 5 == 0) || (5 <= differenceLv))
        {
            tmp[4] = 1;            // 5���x�����ɍK�^���グ��
        }

        // �������x���オ�����ꍇ
        for (int i = oldLevel; i < nowLevel; i++)
        {
            // �����l�������邽�߃��x���A�b�v�ŏオ��l�͓����ɂ���
            if (i % 2 == 1)
            {
                tmp[2] += 2;// 2Defence
            }
            else
            {
                tmp[0] += 2;// 0Attack 
                tmp[1] += 2;// 1MagicAttack
            }
        }
        // 1���x���㏸���邽�тɉ��Z
        tmp[5] = differenceLv * 5;// 5HP
        tmp[6] = differenceLv * 2;// 6MP

        charaRect_.sizeDelta = new Vector2(CharaImage[(int)chara].rect.width, CharaImage[(int)chara].rect.height);
        charaImage_.sprite = CharaImage[(int)chara];
        charaRect_.localPosition = pos;

        levelUpText_.text = "Lv�@" + oldLevel.ToString() + "��" + nowLevel.ToString();
        nextExpText_.text = "���̃��x���܂Ł@" + nextExp.ToString() + "EXP";
        statusText_.text = "HP\nMP\nAttack\nMagicAttack\nDefence\nSpeed\nLuck";
        statusNumText_.text = "+" + tmp[5].ToString() +
                            "\n+" + tmp[6].ToString() +
                            "\n+" + tmp[0].ToString() +
                            "\n+" + tmp[1].ToString() +
                            "\n+" + tmp[2].ToString() +
                            "\n+" + tmp[3].ToString() +
                            "\n+" + tmp[4].ToString();

        Debug.Log(chara + " stetas" + tmp);
        // �X�e�[�^�X���グ��
        SceneMng.charasList_[(int)chara].LevelUp(tmp);

        oldLevel_[(int)chara] = level_[(int)chara];
    }
}