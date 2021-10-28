using UnityEngine;

public class DontDestroyMng : MonoBehaviour
{
    public static DontDestroyMng singleton;
    private static GameObject mInstance;

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
            mInstance = this.gameObject;
        }
        else
        {
            //�@����GameStart�X�N���v�g������΂��̃V�[���̓����Q�[���I�u�W�F�N�g���폜
            Destroy(gameObject);
        }
    }
}
