using UnityEngine;

public class OnceLoad : MonoBehaviour
{
    public static OnceLoad singleton;
    private static GameObject mInstance;
    private static bool loadFlag = false;
    private static bool newGameFlag = false;

    public static GameObject Instance
    {
        // �j�󂳂��X�N���v�g������������ł���悤��
        get
        {
            return mInstance;
        }
    }

    void Start()
    {
        if (singleton == null)
        {
            // �V�[�����ׂ��ł������Ȃ��I�u�W�F�N�g�ɐݒ肷��
            DontDestroyOnLoad(gameObject);
            singleton = this;
            mInstance = gameObject;
        }
    }

    public void SetLoadFlag(bool flag)
    {
        loadFlag = flag;

        Debug.Log("�t���O��"+loadFlag+"�ɂȂ�܂���");

        if (loadFlag==true)
        {
            // �Q�[���ŏ���Load�����ꂽ��v���n�u��j�󂷂�
            Debug.Log(this+"�v���n�u��j�󂵂܂�");
            Destroy(gameObject);
        }
    }

    public void SetNewGameFlag(bool flag)
    {
        newGameFlag = flag;
        if (newGameFlag == true)
        {
            // �Q�[���ŏ���Load�����ꂽ��v���n�u��j�󂷂�
            Debug.Log(this + "�v���n�u��j�󂵂܂�");
            Destroy(gameObject);
        }

    }
}

