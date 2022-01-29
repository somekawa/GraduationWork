using UnityEngine;

// �X�̒��̒�_�J������퓬���̃o�g���J�����̓T�u�J�����Ƃ��ĊǗ����Ă���
// ���C���J�����́A�X�ł��t�B�[���h�ł��L�����Ɍ�납��Ǐ]����悤�ɂ��Ă���

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // ���C���J�����i�[�p
    public GameObject subCamera;       // �t�B�[���h�Ȃ�o�g���J�����i�[,�X�Ȃ��_�J�����i�[

    // �X��퓬���̃T�u�J�����ʒu��ύX����Ƃ��ɌĂ΂��
    public void SetSubCameraPos(Vector3 pos)
    {
        subCamera.transform.position = pos;
    }

    public void SetSubCameraRota(Quaternion rota)
    {
        subCamera.transform.rotation = rota;
    }

    // �O������J������Ԃ̐ؑւ��s����悤�ɂ���
    public void SetChangeCamera(bool flag, bool allfalseFlag = false)
    {
        if(allfalseFlag)
        {
            mainCamera.GetComponent<AudioListener>().enabled = false;
            mainCamera.SetActive(false);

            subCamera.GetComponent<AudioListener>().enabled = false;
            subCamera.SetActive(false);
        }
        else
        {
            mainCamera.GetComponent<AudioListener>().enabled = !flag;
            mainCamera.SetActive(!flag);

            subCamera.GetComponent<AudioListener>().enabled = flag;
            subCamera.SetActive(flag);
        }

        // ���j�n�E�X��Scene�ł͌Ă΂�Ȃ��悤�ɂ���
        if (mainCamera.activeSelf && SceneMng.nowScene != SceneMng.SCENE.UNIHOUSE)
        {
            mainCamera.GetComponent<CameraSample>().Init();
        }
    }

    // FieldMng.cs�̌��݃V�[���m���ɌĂяo������
    public void MainCameraPosInit()
    {
        // �ݒ���Ăяo��
        mainCamera.GetComponent<CameraSample>().Init();
    }
}
