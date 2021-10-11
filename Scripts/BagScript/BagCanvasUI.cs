using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagCanvasUI : MonoBehaviour
{
    public static BagCanvasUI singleton;
    private static GameObject mInstance;

    public static GameObject Instance
    {
        get
        {
            return mInstance;
        }
    }
    // private GameObject prefab_;
    void Awake()
    {
        // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɂ���
        // ���̃I�u�W�F�N�g�������Ă��܂��ƁAQuestClearCheck.cs�̎󒍒��̃N�G�X�g��ۑ����Ă��郊�X�g��null�ɂȂ��Ă��܂�����
        //�@�X�N���v�g���ݒ肳��Ă��Ȃ���΃Q�[���I�u�W�F�N�g���c���X�N���v�g��ݒ�
        if (singleton == null)
        {
            // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɐݒ肷��
            DontDestroyOnLoad(gameObject);
            singleton = this;
            mInstance = this.gameObject;

        }
        else
        {
            //�@����GameStart�X�N���v�g������΂��̃V�[���̓����Q�[���I�u�W�F�N�g���폜
            Destroy(gameObject);
        }
    }
    public void SetMyName(string num)
    {
        // �����̃I�u�W�F�N�g�����A�N�G�X�g�ԍ��ɕϊ�
        // ���X�N���v�g���q�G�����L�[���炱�̃I�u�W�F�N�g����������Ƃ��ɔԍ�+�^�O(Quest)�Ŕ��ʂł���悤�ɂ���
        this.gameObject.name = num.ToString();
    }

    //public GameObject Instance()
    //{
    //        return this.gameObject;
    //}

}
