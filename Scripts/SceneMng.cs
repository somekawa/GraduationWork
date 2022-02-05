using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneMng : MonoBehaviour
{
    // �L�����N�^�[�̎��
    public enum CHARACTERNUM
    {
        UNI,    // ��O
        JACK,   // ��
        MAX
    }

    // �V�[���̎��
    public enum SCENE
    {
        NON = -1,
        TITLE,          // �^�C�g���ɖ߂�{�^��
        CONVERSATION,   // ��b�V�[��
        TOWN,           // �X�V�[��
        UNIHOUSE,       // ���j�����̉�
        FIELD0,          // �t�B�[���h�V�[��
        FIELD1,          // �t�B�[���h�V�[��
        FIELD2,          // �t�B�[���h�V�[��
        FIELD3,          // �t�B�[���h�V�[��
        FIELD4,          // �t�B�[���h�V�[��
        END,// �G���h�p�V�[��
        CANCEL,          // ���[�v��\���p
        MAX
    }

    // 1���̎��Ԍo��
    public enum TIMEGEAR
    {
        MORNING,    // ��
        NOON,       // ��
        EVENING,    // �[
        NIGHT       // ��
    }

    public static SceneMng singleton;

    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charaObjList;

    private static SEAudioMng seAudio_;

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    public static Dictionary<CHARACTERNUM, GameObject> charMap_;
    public static List<Chara> charasList_ = new List<Chara>();          // Chara.cs���L�������Ƀ��X�g������

    public static SCENE nowScene = SCENE.NON;   // ���݂̃V�[��
    public static float charaRunSpeed = 0.0f;   // �L�����̈ړ����x(MODE���ɒ���������)

    private static string houseName_ = "Mob";   // Excel����ǂݍ��񂾌�����

    private static TIMEGEAR timeGrar_ = TIMEGEAR.MORNING;               // 1���̌o�ߎ��Ԃ̏�������
    private static (TIMEGEAR,bool) finStatusUpTime_;                    // �������ʂ��L���鎞�Ԃ�ۑ�����

    private static int haveMoney_ = 1000;       // ���݂̏�����(����������1000)

    void Awake()
    {
        //�@�X�N���v�g���ݒ肳��Ă��Ȃ���΃Q�[���I�u�W�F�N�g���c���X�N���v�g��ݒ�
        if (singleton == null)
        {
            // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɐݒ肷��
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //�@����GameStart�X�N���v�g������΂��̃V�[���̓����Q�[���I�u�W�F�N�g���폜
            Destroy(gameObject);
        }

        // �L�����̃Q�[���I�u�W�F�N�g��񂪃V�[�����ׂ���Missing�ɂȂ�֌W�ŁAScene���Ɉ�x���������K�v������
        if(charMap_ != null)
        {
            charMap_.Clear();
        }
        if(charasList_ != null)
        {
            charasList_.Clear();
        }

        // �v�f�������Ă��Ȃ�����return����
        if(charaObjList.Count <= 0)
        {
            Debug.Log("SceneMng�̈����ɗv�f���ݒ肳��Ă��Ȃ��̂�return���܂�");
            return;
        }

        // �L�����N�^�[�̏����Q�[���I�u�W�F�N�g�Ƃ��čŏ��Ɏ擾���Ă���
        charMap_ = new Dictionary<CHARACTERNUM, GameObject>(){
                {CHARACTERNUM.UNI,charaObjList[(int)CHARACTERNUM.UNI]},
                {CHARACTERNUM.JACK,charaObjList[(int)CHARACTERNUM.JACK]},
            };

        // charMap_��foreach���񂵂āA�L�����N�^�[�̃��X�g���쐬
        foreach (KeyValuePair<CHARACTERNUM, GameObject> anim in charMap_)
        {
            // Chara�N���X�̐���
            charasList_.Add(new Chara(anim.Value.name,0,anim.Value.GetComponent<Animator>()));
        }

        // ����̂݃L�����X�e�[�^�X�������l�œo�^
        if(singleton == null)
        {
            CharaData.SetCharaData(0,charasList_[0].GetCharaSetting());
            CharaData.SetCharaData(1,charasList_[1].GetCharaSetting());
            singleton = this;
        }

        // ������charasData�̃X�e�[�^�X�l��charasList_�ɑ��
        charasList_[0].SetCharaSetting(CharaData.GetCharaData(0));
        charasList_[1].SetCharaSetting(CharaData.GetCharaData(1));
    }

    // ���[�h�f�[�^��������
    public static void SetCharasSettings(int num,CharaBase.CharacterSetting set)
    {
        charasList_[num].SetCharaSetting(set);
    }

    // MenuMng.cs�ŌĂяo��
    public static CharaBase.CharacterSetting GetCharasSettings(int num)
    {
        return charasList_[num].GetCharaSetting();
    }

    // �O������ݒ肳���V�[����ԑJ��
    public static void SetNowScene(SCENE scene)
    {
        // ���݂̃V�[���ɍ��킹�ăL�����̈ړ����x��ύX����
        if (nowScene != scene)
        {
            switch (scene)
            {
                case SCENE.TOWN:
                    charaRunSpeed = 10.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD0:
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD1:
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD2:
                    charaRunSpeed = 10.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD3:
                    charaRunSpeed = 5.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD4:
                    charaRunSpeed = 10.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.UNIHOUSE:
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                default:
                    break;
            }

            // scene��nowScene���㏑���O�ɁA���܂œ����Ă����V�[�����t�B�[���h�ł���Ύ��Ԍo�߂�����
            if(nowScene != SCENE.TOWN && nowScene != SCENE.UNIHOUSE && nowScene != SCENE.CONVERSATION && nowScene != SCENE.NON)    
            {
                if(scene != SCENE.CONVERSATION) // ����if�����Ȃ��ƁA�t�B�[���h���͂������ĉ�b�V�[���ɔ�񂾂玞����2�i��ł��܂�
                {
                    // �^�E���ł����j�n�E�X�ł�NON�ł��Ȃ� = �ǂ����̃t�B�[���h
                    SetTimeGear(timeGrar_ + 1);
                }
            }

            nowScene = scene;

            // DontDestroyCanvas���̃I�u�W�F�N�g�̔�\���Ǘ�
            if (nowScene == SCENE.TOWN)
            {
                // �M���h�ɂ���ꍇ�̓o�b�O���\����
                var interiorMng = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
                if (interiorMng.GetInHouseName() == "Guild")
                {
                    // ��b�V�[���Ȃ�MENU�̂ݔ�\����
                    MenuSetActive(false);
                }
            }
            else if (nowScene == SCENE.CONVERSATION)
            {
                // ��b�V�[���Ȃ�MENU�̂ݔ�\����
                MenuSetActive(false);
            }
            else
            {
                return;
            }
        }
    }

    // ���̏������̂ق����m����Menu���擾���ĕ\��/��\����ύX�ł���
    public static void MenuSetActive(bool flag)
    {
        var tmp = GameObject.Find("DontDestroyCanvas").GetComponent<RectTransform>();
        for (int i = 0; i < tmp.childCount; i++)
        {
            if (tmp.GetChild(i).gameObject.name == "Menu")
            {
                tmp.GetChild(i).gameObject.SetActive(flag);
                break;
            }
        }
    }

    // �V�[���̃��[�h
    public static void SceneLoad(int load,bool allDeath = false)
    {
        // NON�������Ă�����return����
        if(load == -1)
        {
            return;
        }

        //GameObject.Find("Uni") = false;

        //@ ������charasList_�̃X�e�[�^�X�l��charasData�ɔ�����H 
        CharaData.SetCharaData(0,charasList_[0].GetCharaSetting());
        CharaData.SetCharaData(1,charasList_[1].GetCharaSetting());

        // int�ԍ��́A�r���h�ݒ�̐��l
        //SceneManager.LoadScene(load);
        if(GameObject.Find("CameraController"))
        {
            GameObject.Find("CameraController").GetComponent<CameraMng>().SetChangeCamera(true, true);
        }

        // �S�Ŏ��̓��[�h�����ɓ���Ȃ�(����ƃQ�[�����ł܂�)
        if(allDeath)
        {
            SceneManager.LoadScene(load);
        }
        else
        {
            GameObject.Find("DontDestroyCanvas/LoadingCamera").GetComponent<Loading>().NextScene(load);
        }
    }

    // Excel����ǂݍ��񂾌�������ۑ����Ă���
    public static void SetHouseName(string name)
    {
        houseName_ = name;
    }

    // Excel����ǂݍ��񂾌�������n��
    public static string GetHouseName()
    {
        return houseName_;
    }

    public static void SetTimeGear(TIMEGEAR dayTime)
    {
        if (dayTime > TIMEGEAR.NIGHT) // ������Ȃ�A��������悤�ɂ���
        {
            SetSE(1);
            timeGrar_ = TIMEGEAR.MORNING;
        }
        else
        {
            SetSE(1);
            timeGrar_ = dayTime;
        }

        // �����̌��ʂ��؂�鎞�ԂȂ�t���O��false�ɂ���
        if(timeGrar_ == finStatusUpTime_.Item1)
        {
            finStatusUpTime_.Item2 = false;
            GameObject.Find("DontDestroyCanvas/TimeGear/CookPowerIcon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            Debug.Log("�������ʂ��؂�܂���");
        }
    }

    public static TIMEGEAR GetTimeGear()
    {
        return timeGrar_;
    }

    // �����̐ݒ�
    public static void SetHaveMoney(int money)
    {
        haveMoney_ = money;
    }

    // �������擾����
    public static int GetHaveMoney()
    {
        return haveMoney_;
    }

    // �������ʂ��؂�鎞����ۑ�����
    public static void SetFinStatusUpTime(int num = -1,bool loadFlag = false)
    {
        if(loadFlag)
        {
            // �v�Z���������₻�̂܂܂̐����������Đݒ肷��
            finStatusUpTime_ = ((TIMEGEAR)num, true);
            GameObject.Find("DontDestroyCanvas/TimeGear/CookPowerIcon").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            Debug.Log("�������ʂ��؂��̂��A" + finStatusUpTime_ + "�ɐݒ肳��܂���");
        }
        else
        {
            // 2��̎�����ݒ肷��
            var tmpNum = (int)timeGrar_ + 2;

            // 2��̎���������z���������ɂȂ�����
            if (tmpNum > (int)TIMEGEAR.NIGHT)
            {
                // �z������������4�������΂���
                tmpNum -= 4;
            }

            // �v�Z���������₻�̂܂܂̐����������Đݒ肷��
            finStatusUpTime_ = ((TIMEGEAR)tmpNum, true);
            Debug.Log("�������ʂ��؂��̂��A" + finStatusUpTime_ + "�ɐݒ肳��܂���");
        }
    }

    // �������ʂ��؂ꂽ���t���O���擾����
    public static (TIMEGEAR,bool) GetFinStatusUpTime()
    {
        return finStatusUpTime_;
    }

    public static void SetSE(int num)
    {
        if(seAudio_ == null)
        {
            if(GameObject.Find("Audio/SE_Audio"))
            {
                seAudio_ = GameObject.Find("Audio/SE_Audio").GetComponent<SEAudioMng>();
            }
            else
            {
                seAudio_ = GameObject.Find("FieldMng/SE_Audio").GetComponent<SEAudioMng>();
            }
        }
        seAudio_.OnceShotSE(num);
    }
}
