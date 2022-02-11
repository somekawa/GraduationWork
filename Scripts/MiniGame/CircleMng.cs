using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleMng : MonoBehaviour
{
    [SerializeField]
    private RectTransform miniGameMng;    // �J�E���g�_�E���A����\���p

    [SerializeField] 
    private RectTransform magicCreateMng;    // ���@����
    private MagicCreate magicCreate_;

    [SerializeField] 
    private RectTransform AlchemyMng;    // �A�C�e������
    private ItemCreateMng itemCreate_;

    [SerializeField]
    private Material[] elementMats;// �}�e���A���@7��(6�ڂ̓A�C�e�������p)
    private Material[] elementMaterial = new Material[1];// �}�e���A�����Z�b�g��
    private MeshRenderer elementRenderer;// �}�e���A�����Z�b�g������̃����_�[

    [SerializeField]
    private Material[] judgeMats;// ����̃}�e���A�� 4��
    private Material[] judgeMaterial = new Material[1];// ������Z�b�g������
    MeshRenderer judgeMeshRender_;// ����p�}�e���A�����Z�b�g���邽�߂̃����_�[

    Texture2D tex;
    private Transform miniGameObj_;

    private Transform rotateCenter;// ��]�n�_
    private Transform needleTop;// �j�̐�


    public enum JUDGE
    {
        NON = -1,
        BAD,
        NORMAL,
        GOOD,
        MAX
    }
    private JUDGE judge_ = JUDGE.NON;
    private Image judgeBack_;
    private TMPro.TextMeshProUGUI judgeText_;
    // �J�E���g�_�E���\��
    private Image countImage_;
    private TMPro.TextMeshProUGUI countText_;

    AssetBundle assetBundle_;


    public void Init(int elementNum, int judgeNum)
    {
        // �A�C�e�����������@���������A�N�e�B�u��ԂŔ��f
        if (magicCreateMng.gameObject.activeSelf == true)
        {
            magicCreate_ = magicCreateMng.GetComponent<MagicCreate>();
        }
        else
        {
            itemCreate_ = AlchemyMng.GetComponent<ItemCreateMng>();
        }

        if (elementRenderer == null)
        {
            miniGameObj_ = GameObject.Find("MiniGameObj").GetComponent<Transform>();
            judgeMeshRender_ = GetComponent<MeshRenderer>();
            // ���@�������̑����p�}�e���A���Z�b�g�̂���
            elementRenderer = GameObject.Find("ElementCircle").GetComponent<MeshRenderer>();

            // �j�֘A
            rotateCenter = GameObject.Find("RotateCenter").GetComponent<Transform>();
            needleTop = rotateCenter.Find("NeedleQuad/NeedleTop").GetComponent<Transform>();

            // �J�E���g�_�E��
            countImage_ = miniGameMng.Find("CountImage").GetComponent<Image>();
            countText_ = countImage_.transform.Find("Count").GetComponent<TMPro.TextMeshProUGUI>();

            // ����
            judgeBack_ = miniGameMng.transform.Find("JudgeBack").GetComponent<Image>();
            judgeText_ = judgeBack_.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            judgeMeshRender_.gameObject.SetActive(false);
            elementRenderer.gameObject.SetActive(false);
        }
        // ����̏�����
        judge_ = JUDGE.NON;
        judgeText_.text = null;

        rotateCenter.gameObject.SetActive(false);

        elementMaterial[0] = elementMats[elementNum];
        elementRenderer.materials = elementMaterial;

        judgeMaterial[0] = judgeMats[judgeNum];
        judgeMeshRender_.materials = judgeMaterial;
        gameObject.transform.localEulerAngles += new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

        // �}�e���A�����Z�b�g����ĂȂ��\�������邽�߃Z�b�g��A�N�e�B�u��Ԃɂ���
        elementRenderer.gameObject.SetActive(true);
        judgeMeshRender_.gameObject.SetActive(true);

        miniGameMng.gameObject.SetActive(true);

        if (assetBundle_ == null)
        {
            assetBundle_ = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/create");
        }

        // �J�E���g�_�E���֘A
        countImage_.gameObject.SetActive(true);// �J�E���g�_�E���p�̉摜��\������
        StartCoroutine(CountDown());        // �J�E���g�_�E�����J�n����
    }

    public IEnumerator CountDown()
    {
        countText_.text = "3";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "2";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "1";
        yield return new WaitForSeconds(1.0f);
        // �J�E���g�_�E���̕\��������
        countImage_.gameObject.SetActive(false);
        // �j��\��������
        rotateCenter.gameObject.SetActive(true);
        // �~�j�Q�[�����J�n����
        StartCoroutine(ResultMiniGame());
        yield break;

    }

    public IEnumerator ResultMiniGame()
    {
        bool flag = false;
        while (true)
        {
            yield return null;
            if (flag == false)
            {
                // �j����]������
                rotateCenter.transform.localEulerAngles = 
                    new Vector3(rotateCenter.transform.localEulerAngles.x, rotateCenter.transform.localEulerAngles.y, Mathf.Sin(Time.time) * -360.0f);

                // �X�y�[�X�L�[�����ŉ�]�X�g�b�v
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Vector3 pos = needleTop.transform.position;

                    // �΂�13.5�x�ɕϊ�
                    // 13.5�x�̂Ƃ��Asin(0.2334),cos(0.9724)
                    Ray ray1 = new Ray(pos, new Vector3(0, -0.23f, 0.97f));

                    RaycastHit hit;
                    tex = judgeMeshRender_.material.mainTexture as Texture2D;

                    //  if (EventSystem.current.IsPointerOverGameObject()) return;
                    // �j�̐�[���疂�@�w�֌������ă��C���΂�
                    if (Physics.Raycast(pos, ray1.direction, out hit, Mathf.Infinity))
                    {
                        Vector2 uv = hit.textureCoord;
                        Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
                        Debug.Log(pix[0].ToString());
                        Debug.Log(pix[0]);

                        //text.text =pix[0].ToString();// new Color(pix[0].r, pix[0].g, pix[0].b, 1.0f).ToString();// 
                        //image.color =             pix[0];//new Color(pix[0].r, pix[0].g, pix[0].b, 1.0f);//    
                        //Debug.DrawRay(Vector3 start(ray���J�n����ʒu), Vector3 dir(ray�̕����ƒ���), Color color(���C���̐F), float duration(���C���̕\������鎞��), bool depthTest(���C�����J��������߂��I�u�W�F�N�g�ɂ���ĉB���ꂽ�ꍇ�Ƀ��C�����B�����ǂ���));
                        Debug.DrawRay(pos, ray1.direction * 1000.0f, Color.red, 100.0f, false);
                        // �F���Ȃ��������ƃA���t�@�l��0���擾�ł���
                        GameObject obj = null;
                        judgeBack_.gameObject.SetActive(true);
                        if (pix[0].a == 0.0f)
                        {
                            SceneMng.SetSE(1);
                            // �A���t�@�l��0�������h���ĂȂ��ӏ�
                            judgeText_.text = "����";
                            Debug.Log("�������܂���");
                            judge_ = JUDGE.NORMAL;
                        }
                        else
                        {
                            if (0.8f < pix[0].r && pix[0].r <= 1.0f)
                            {
                                obj = assetBundle_.LoadAsset<GameObject>("CreateGood");
                                Instantiate(obj, this.gameObject.transform.position, obj.transform.rotation);

                                SceneMng.SetSE(2);
                                // ��
                                judgeText_.text = "�听��";
                                Debug.Log("�听�����܂���");
                                judge_ = JUDGE.GOOD;
                            }
                            else if (0.0f <= pix[0].r && pix[0].r <= 0.1f)
                            {
                                obj = assetBundle_.LoadAsset<GameObject>("CreateMiss");
                                Instantiate(obj, this.gameObject.transform.position, obj.transform.rotation);

                                SceneMng.SetSE(3);
                                // ��
                                judgeText_.text = "���s";
                                Debug.Log("���s���܂���");
                                judge_ = JUDGE.BAD;
                            }
                        }

                        // �~�j�Q�[�����U���g�Ɉڂ�
                        flag = true;
                    }
                }
            }
            else
            {
                // �~�j�Q�[���̃��U���g
                // �A�C�e�����������@�쐬�����A�N�e�B�u��ԂŔ��f
                if (magicCreateMng.gameObject.activeSelf == true)
                {
                   magicCreate_.ResultMagicCreate();
                }
                else
                {
                    itemCreate_.AlchemyRecipeSelect();                    
                }

                yield return new WaitForSeconds(2.0f);

                judgeBack_.gameObject.SetActive(false);
                miniGameMng.gameObject.SetActive(false);
                judgeText_.text = null;

                judgeMeshRender_.gameObject.SetActive(false);
                elementRenderer.gameObject.SetActive(false);
                // �������}�e���A���Z�b�g��͍폜���Ă���
                Destroy(judgeMeshRender_.material);
                Destroy(elementRenderer.material);
                miniGameObj_.gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public JUDGE GetMiniGameJudge()
    {
        return judge_;
    }
}