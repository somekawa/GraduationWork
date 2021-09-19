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
        TOWN,   // �X�V�[��
        FIELD,  // �t�B�[���h�V�[��
        MAX
    }

    // enum�ƃL�����I�u�W�F�N�g���Z�b�g�ɂ���map�𐧍삷�邽�߂̃��X�g
    // �L�����I�u�W�F�N�g��v�f�Ƃ��ăA�^�b�`�ł���悤�ɂ��Ă���
    public List<GameObject> charaObjList;

    // �L�[���L��������enum,�l��(�L�������ʂɑΉ�����)�L�����I�u�W�F�N�g�ō����map
    private static Dictionary<CharcterNum, GameObject> charMap_;
    private static List<Chara> charasList_ = new List<Chara>();          // Chara.cs���L�������Ƀ��X�g������

    public static SCENE nowScene = SCENE.MAX;   // ���݂̃V�[��
    public static float charaRunSpeed = 0.0f;   // �L�����̈ړ����x(MODE���ɒ���������)

    void Awake()
    {
        // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɐݒ肷��
        DontDestroyOnLoad(gameObject);

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
    }

    // CharacterMng.cs��Start�֐��ŌĂ΂��
    public static Dictionary<CharcterNum, GameObject> GetCharMap()
    {
        return charMap_;
    }

    // CharacterMng.cs��Start�֐��ŌĂ΂��
    public static List<Chara> GetCharasList()
    {
        return charasList_;
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
                    charaRunSpeed = 8.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                case SCENE.FIELD:
                    charaRunSpeed = 4.0f;
                    Debug.Log("�ړ����x�ύX" + charaRunSpeed);
                    break;

                default:
                    break;
            }
            nowScene = scene;
        }
    }

    // �V�[���̃��[�h/�A�����[�h
    public static void SceneLoadUnLoad(int load , int unload)
    {
        // int�ԍ��́A�r���h�ݒ�̐��l
        SceneManager.LoadScene(load);
        SceneManager.UnloadSceneAsync(unload);
    }
}
