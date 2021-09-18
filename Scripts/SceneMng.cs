using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneMng : MonoBehaviour
{
    // �V�[���̎��
    public enum SCENE
    {
        TOWN,   // �X�V�[��
        FIELD,  // �t�B�[���h�V�[��
        MAX
    }

    public static SCENE nowScene=SCENE.MAX;               // ���݂̃V�[��
    public static float charaRunSpeed = 0.0f;   // �L�����̈ړ����x(MODE���ɒ���������)

    //void Awake()
    //{
    //    // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɐݒ肷��
    //    DontDestroyOnLoad(gameObject);
    //}

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
