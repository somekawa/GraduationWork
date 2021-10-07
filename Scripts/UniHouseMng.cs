using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���݂̃V�[����UNIHOUSE�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);

        // WarpTown.cs�̏������֐����Ă�
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // ���C���J�������ŏ��ɃA�N�e�B�u�ɂ���
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);
    }
}
