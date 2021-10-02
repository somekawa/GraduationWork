using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMng : MonoBehaviour
{
    // �L�����N�^�[�̎��
    public enum CharcterNum
    {
        UNI,    // ��O
        DEMO,   // ��
        MAX
    }

    // �V�[���̎��
    public enum SCENE
    {
        NON = -1,
        CONVERSATION,   // ��b�V�[��
        TOWN,           // �X�V�[��
        FIELD,          // �t�B�[���h�V�[��
        UNIHOUSE        // ���j�����̉�
    }

    public static SceneMng singleton;

    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charaObjList;

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    public static Dictionary<CharcterNum, GameObject> charMap_;
    public static List<Chara> charasList_ = new List<Chara>();          // Chara.cs���L�������Ƀ��X�g������

    public static SCENE nowScene = SCENE.NON;   // ���݂̃V�[��
    public static float charaRunSpeed = 0.0f;   // �L�����̈ړ����x(MODE���ɒ���������)

    public static string houseName_ = "Mob";    // Excel����ǂݍ��񂾌�����

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
        charMap_ = new Dictionary<CharcterNum, GameObject>(){
                {CharcterNum.UNI,charaObjList[(int)CharcterNum.UNI]},
                {CharcterNum.DEMO,charaObjList[(int)CharcterNum.DEMO]},
            };

        // charMap_��foreach���񂵂āA�L�����N�^�[�̃��X�g���쐬
        foreach (KeyValuePair<CharcterNum, GameObject> anim in charMap_)
        {
            // Chara�N���X�̐���
            charasList_.Add(new Chara(anim.Value.name, anim.Key, anim.Value.GetComponent<Animator>()));
        }

        // ����̂݃L�����X�e�[�^�X�������l�œo�^
        if(singleton == null)
        {
            CharaData.SetCharaData(charasList_[0].GetCharaSetting());
            singleton = this;
        }

        //@ ������charasData�̃X�e�[�^�X�l��charasList_�ɑ���H
        // �X�e�[�^�X�l����e�X�g
        charasList_[0].SetCharaSetting(CharaData.GetCharaData());

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

                case SCENE.FIELD:
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.UNIHOUSE:
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                default:
                    break;
            }
            nowScene = scene;
        }
    }

    // �V�[���̃��[�h
    public static void SceneLoad(int load)
    {
        // NON�������Ă�����return����
        if(load == -1)
        {
            return;
        }

        //@ ������charasList_�̃X�e�[�^�X�l��charasData�ɔ�����H 
        CharaData.SetCharaData(charasList_[0].GetCharaSetting());

        // int�ԍ��́A�r���h�ݒ�̐��l
        SceneManager.LoadScene(load);
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
}
