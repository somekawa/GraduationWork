using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneMng : MonoBehaviour
{
    // ���h�����悭���邽�߁i�Q�[���N����͔�\��
    private Image panel_;

    // �J�����ύX�^�C�~���O�Ńt�F�[�h�A�E�g�A�C��������
    private Fade fade_;
    private bool fadeFlag_=false;

    // �c�iZ�j�ړ��J����
    private Camera zMoveCamera;
    private Vector3 zC_StartPos_ = new Vector3(2.0f, 31.5f, 108.5f);// �X�^�[�g���W
    private Vector3 zC_MaxPos_ = new Vector3(2.0f, 31.5f, 120.0f);// �}�b�N�X���W

    // ���iX)�ړ��J����
    private Camera xMoveCamera;
    private Vector3 xC_StartPos_ = new Vector3(-55.0f, 6.5f, 90.0f);// �X�^�[�g���W
    private Vector3 xC_MaxPos_ = new Vector3(40.0f, 6.5f, 90.0f);// �}�b�N�X���W

    // �V�[���J�ڐ�̖��O
    private string sceneName_ = "";

    void Start()
    {
        panel_ = GameObject.Find("Canvas/Panel").GetComponent<Image>();
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        zMoveCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        zMoveCamera.transform.position = zC_StartPos_;
        zMoveCamera.depth = 1;

        xMoveCamera = GameObject.Find("SubCamera").GetComponent<Camera>();
        xMoveCamera.transform.position = xC_StartPos_;
        xMoveCamera.depth = -1;

        StartCoroutine(FadeOutAndIn()); 
    }

    private IEnumerator FadeOutAndIn()
    {
        fade_.FadeIn(1);
        fadeFlag_ = true;
        yield return new WaitForSeconds(0.5f);

        fade_.FadeOut(1);

        if (panel_.gameObject.activeSelf == true)
        {
            panel_.gameObject.SetActive(false);
            StartCoroutine(MoveCamera());
        }
        else
        {
            // �J�����ύX
            if (zMoveCamera.depth == 1)
            {
                zMoveCamera.depth = -1;// �����Ȃ�
                xMoveCamera.depth = 1;// ����
                // �����ʒu�ɖ߂�
                zMoveCamera.transform.position = zC_StartPos_;
            }
            else
            {
                zMoveCamera.depth = 1;// ����
                xMoveCamera.depth = -1;// �����Ȃ�
                // �����ʒu�ɖ߂�
                xMoveCamera.transform.position = xC_StartPos_;
            }

            if (sceneName_ != "")
            {
                Debug.Log("�V�[���ړ����܂�");
                // �V�[���J�ڂ���^�C�~���O�œ���
                // �J�����̓������~�߂�
                StopCoroutine(MoveCamera());
                SceneManager.LoadScene(sceneName_);// �w��̃V�[���Ɉړ�
            }
        }
        fadeFlag_ = false;
        yield break; // ���[�v�𔲂���
    }

    private IEnumerator MoveCamera()
    {
        while (true)
        {
            yield return null;
            if (fadeFlag_ == false)
            {
                if (zMoveCamera.depth == 1)
                {
                    if (zC_MaxPos_.z - 1.0f < zMoveCamera.transform.position.z)
                    {
                        // �t�F�[�h�A�E�g�J�n�^�C�~���O
                        StartCoroutine(FadeOutAndIn());
                    }
                }
                else
                {
                    if (xC_MaxPos_.x - 1.0f < xMoveCamera.transform.position.x)
                    {
                        // �t�F�[�h�A�E�g�J�n�^�C�~���O
                        StartCoroutine(FadeOutAndIn());
                    }
                }
            }
            if (zMoveCamera.depth == 1)
            {
                zMoveCamera.transform.position += new Vector3(0.0f, 0.0f, 0.05f);// �ړ�
            }
            else
            {
                xMoveCamera.transform.position += new Vector3(0.03f, 0.0f, 0.0f);// �ړ�
            }
        }
    }

    public void OnClickNewGame()
    {
        sceneName_ = "conversationdata";
        StartCoroutine(FadeOutAndIn());
    }

    public void OnClickLoadGame()
    {
        sceneName_ = "Town";
        StartCoroutine(FadeOutAndIn());
    }
}