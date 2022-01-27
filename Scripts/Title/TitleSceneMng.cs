using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneMng : MonoBehaviour
{
    [SerializeField]
    private GameObject LoadPrefab;  // �e���X�g���Ăяo��
    private GameObject loadPrefab_;

    // ���h�����悭���邽�߁i�Q�[���N����͔�\��
    private Image panel_;

    // �J�����ύX�^�C�~���O�Ńt�F�[�h�A�E�g�A�C��������
    private Fade fade_;
    private bool fadeFlag_=false;

    // �c�iZ�j�ړ��J����
    private AudioListener zCameraListener_;
    private Camera zMoveCamera_;
    private Vector3 zC_StartPos_ = new Vector3(2.0f, 31.5f, 8.5f);// �X�^�[�g���W
    private Vector3 zC_MaxPos_ = new Vector3(2.0f, 31.5f, 120.0f);// �}�b�N�X���W

    // ���iX)�ړ��J����
    private AudioListener xCameraListener_;
    private Camera xMoveCamera_;
    private Vector3 xC_StartPos_ = new Vector3(-55.0f, 6.5f, 90.0f);// �X�^�[�g���W
    private Vector3 xC_MaxPos_ = new Vector3(40.0f, 6.5f, 90.0f);// �}�b�N�X���W

    // �V�[���J�ڐ�̖��O
    private string sceneName_ = "";

    void Start()
    {
        // �Z�[�u�f�[�^�����邩���ׂāA�Ȃ��Ƃ��̓{�^����interactable��false�ɂ���
        TextAsset saveFile = Resources.Load("data") as TextAsset;

        if (saveFile == null)
        {
            GameObject.Find("Canvas/LoadGameBtn").GetComponent<Button>().interactable = false;
        }

        // �e�͕K�v�Ȃ�����null
        loadPrefab_ = Instantiate(LoadPrefab,
                               new Vector2(0, 0), Quaternion.identity, null);
        loadPrefab_.name = "LoadPrefab";

        panel_ = GameObject.Find("Canvas/Panel").GetComponent<Image>();
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        zMoveCamera_ = GameObject.Find("MainCamera").GetComponent<Camera>();
        zCameraListener_ = zMoveCamera_.gameObject.GetComponent < AudioListener >();
        zCameraListener_.enabled = true;
        zMoveCamera_.transform.position = zC_StartPos_;
        zMoveCamera_.depth = 1;

        xMoveCamera_ = GameObject.Find("SubCamera").GetComponent<Camera>();
        xCameraListener_ = xMoveCamera_.gameObject.GetComponent<AudioListener>();
        xCameraListener_.enabled = false;
        xMoveCamera_.transform.position = xC_StartPos_;
        xMoveCamera_.depth = -1;

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
            if (zMoveCamera_.depth == 1)
            {
                zMoveCamera_.depth = -1;// �����Ȃ�
                xMoveCamera_.depth = 1;// ����
                zCameraListener_.enabled = false;
                xCameraListener_.enabled = true;
                // �����ʒu�ɖ߂�
                zMoveCamera_.transform.position = zC_StartPos_;
            }
            else
            {
                zMoveCamera_.depth = 1;// ����
                xMoveCamera_.depth = -1;// �����Ȃ�
                zCameraListener_.enabled = true;
                xCameraListener_.enabled = false;
                // �����ʒu�ɖ߂�
                xMoveCamera_.transform.position = xC_StartPos_;
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
                if (zMoveCamera_.depth == 1)
                {
                    if (zC_MaxPos_.z - 1.0f < zMoveCamera_.transform.position.z)
                    {
                        // �t�F�[�h�A�E�g�J�n�^�C�~���O
                        StartCoroutine(FadeOutAndIn());
                    }
                }
                else
                {
                    if (xC_MaxPos_.x - 1.0f < xMoveCamera_.transform.position.x)
                    {
                        // �t�F�[�h�A�E�g�J�n�^�C�~���O
                        StartCoroutine(FadeOutAndIn());
                    }
                }
            }
            if (zMoveCamera_.depth == 1)
            {
                zMoveCamera_.transform.position += new Vector3(0.0f, 0.0f, 0.05f);// �ړ�
            }
            else
            {
                xMoveCamera_.transform.position += new Vector3(0.03f, 0.0f, 0.0f);// �ړ�
            }
        }
    }

    public void OnClickNewGame()
    {
        // sceneName_ = "conversationdata";
        sceneName_ = "InHouseAndUniHouse";//�f�o�b�O�p
        StartCoroutine(FadeOutAndIn());
    }

    public void OnClickLoadGame()
    {
        sceneName_ = "Town";
        StartCoroutine(FadeOutAndIn());
    }
}