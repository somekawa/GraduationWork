using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    [SerializeField]
    private GameObject nightLights; // �X��

    void Start()
    {
        // ���݂̃V�[����UNIHOUSE�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);

        // ������Ȃ�΃��C�g�_���A����ȊO�Ȃ烉�C�g����
        if (SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.NIGHT)
        {
            nightLights.SetActive(true);
        }
        else
        {
            nightLights.SetActive(false);
        }

        // WarpTown.cs�̏������֐����Ă�
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();

        // WarpField.cs�̏������֐����ɌĂ�
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // ���C���J�������ŏ��ɃA�N�e�B�u�ɂ���
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);
    }

    public void ClickSleepButton()
    {
        if(EventMng.GetChapterNum() < 7)    // �i�s�x��0�`6�̂Ƃ�
        {
            Debug.Log("���݂̐i�s�x��7�����̂��߁A�x�߂܂���");
            return; // �x�ރ{�^���������Ă��������Ȃ��悤�ɂ���
        }

        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING);   // ���Ԍo��
        Debug.Log("�x�ރ{�^������������܂���");
    }
}
