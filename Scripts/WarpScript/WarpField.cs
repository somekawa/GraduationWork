using UnityEngine;
using UnityEngine.UI;

public class WarpField : MonoBehaviour
{
    // �\���֘A
    private Canvas dontDestroyCanvas_;
    private RectTransform locationSelMng_;  // ���[�v����o��Canvas�i�e�j
    public static Image[] btnMng_;          // �����������̂�ۑ�����I�u�W�F�N�g
    private Text[] sceneText_;              // �������ꂽ���̂̒�����Text��ۑ�
    private RectTransform btnParent_;       // �{�^���̐e�ɂ�����I�u�W�F�N�g

    private string[] sceneName = new string[(int)SceneMng.SCENE.MAX] {
    "","town","house","field0","field1","field2","field3","field4","cancel"};
    private int storyProgress_ = (int)SceneMng.SCENE.FIELD2;// �ǂ̏͂܂Ői��ł��邩

    private bool fieldEndHit = false;   // ���[�v�I�u�W�F�N�g�ɐڐG�������ǂ���
    private bool nowTownFlag_ = false;  // �����[�v����̃t�B�[���h���[�v��
    private bool warpNowFlag_ = false;  // �t�B�[���h���[�v��I�𒆂̎�

    private GameObject[] warpObject_;   // �}�b�v�[�̃��[�v�I�u�W�F��ۑ�

    // �}�b�v�[����t�B�[���h�I�����L�����Z�������ꍇ�֘A
    private enum rotate
    {
        UP,     // 0 �� 315<=360&&0<45
        RIGHT,  // 1 �E 45<=135
        DOWN,   // 2 �� 135<=225
        LEFT,   // 3 �� 225<=315
        MAX
    }
    private rotate nowRotate_;// �㉺���E�ǂ̕����������Ă��邩
    // ���j�������Ă�����o�����߂͈̔�
    private int[] checkRot_ = new int[6] { 0, 45, 135, 225, 315, 360 };

    private GameObject UniChan_;         // ���j
    private UnitychanController UniChanController_;                 // ���j�̃R���g���[���[���
    private Vector3[] uniPositions_ = new Vector3[3]{
        new Vector3(0.0f, 0.0f, 0.0f),    // ���j�������Ă������ۑ�
        new Vector3(0.0f, 0.0f, 0.0f),     // �ڐG�����u�Ԃ̍��W��ۑ�
        new Vector3(0.0f, 0.0f, 0.0f)       // �L�����Z�����ɔ��Ε����ɒe��
    };

    // �i���j���W�[���[�v�I�u�W�F�j�𐳋K��
    private Vector3 uniNormalized_ = new Vector3(0.0f, 0.0f, 0.0f);
    private float rotateNormalized_ = 0.0f;// �����Ă�����̐��K����ۑ�

    public void Init()
    {
        // ���W�Ɖ�]��ς���\�������邽�߃��j���擾
        UniChan_ = GameObject.Find("Uni");
        UniChanController_ = UniChan_.GetComponent<UnitychanController>();

        btnMng_ = new Image[(int)SceneMng.SCENE.MAX];
        sceneText_ = new Text[(int)SceneMng.SCENE.MAX];

        dontDestroyCanvas_ = GameObject.Find("DontDestroyCanvas").GetComponent<Canvas>();
        GameObject game = DontDestroyMng.Instance;
        locationSelMng_ = dontDestroyCanvas_.transform.Find("LocationSelMng").GetComponent<RectTransform>();
       
       btnParent_ = locationSelMng_.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        for (int i = (int)SceneMng.SCENE.CONVERSATION; i < (int)SceneMng.SCENE.MAX; i++)
        {
            btnMng_[i] = btnParent_.transform.GetChild(i).GetComponent<Image>();
            sceneText_[i] = btnMng_[i].transform.GetChild(0).GetComponent<Text>();
            sceneText_[i].text = sceneName[i];
            btnMng_[i].name = sceneName[i];
            // ���݃X�g�[���ȍ~�̃t�B�[���h�ƌ��݂���V�[���͔�\��
            if (((int)SceneMng.nowScene == i) || (storyProgress_ < i))
            {
                btnMng_[i].gameObject.SetActive(false);
            }
            else
            {
                btnMng_[i].gameObject.SetActive(true);
            }
        }
        btnMng_[(int)SceneMng.SCENE.CONVERSATION].gameObject.SetActive(false);// 0�Ԗڂ͂����Ɣ�\��
        btnMng_[(int)SceneMng.SCENE.CANCEL].gameObject.SetActive(true);// �L�����Z���͂����ƕK�v�͂����Ɣ�\��

        // �}�b�v�[�ɂ���I�u�W�F�N�g�������i�t�B�[���h�ɂ���Č����Ⴄ���ߎq�̌��Ō���j
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "���̃��[�v���\" + warpObject_[i].transform.position);
        }
        // �t�B�[���h�I���L�����o�X���\��
        locationSelMng_.gameObject.SetActive(false);
    }

    private void UniPushBack()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            uniNormalized_ = (uniPositions_[1] - warpObject_[i].transform.position).normalized;
            //Debug.Log(warpObject_[i].name + "�Ƃ̐��K��" + uniNormalized_);
            if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
            {
                rotateNormalized_ = uniNormalized_.z;
                uniPositions_[2] = new Vector3(0.0f, 0.0f, 0.5f);
                Debug.Log("�ォ������ڐG");
            }
            else
            {
                rotateNormalized_ = uniNormalized_.x;
                uniPositions_[2] = new Vector3(-0.5f, 0.0f, 0.0f);
                Debug.Log("�E��������ڐG");
            }
        }

        if (rotateNormalized_ < 0.0f)
        {
            // �o�悤�Ƃ��������̔��Α��ɉ��Z
            uniPositions_[2] = -uniPositions_[2];
        }
        // +180�x�Ŕ��Ε������ނ�����
        UniChan_.transform.rotation = Quaternion.Euler(0.0f, uniPositions_[0].y + 180, 0.0f);
        UniChan_.transform.position = uniPositions_[1] + uniPositions_[2];
        fieldEndHit = false;
    }

    private void CheckUniTransfoem()
    {
        // ���j�̍��W�ƌ����Ă������ۑ�
        uniPositions_[1] = UniChan_.transform.position;
        uniPositions_[0] = UniChan_.transform.localEulerAngles;

        if ((checkRot_[4] < uniPositions_[0].y && uniPositions_[0].y < checkRot_[5])
         || (checkRot_[0] <= uniPositions_[0].y && uniPositions_[0].y < checkRot_[1]))
        {
            nowRotate_ = rotate.UP;            // �㑤
        }
        else
        {
            // �㑤�ȊO�̕����̎�
            for (int i = 1; i < (int)rotate.MAX; i++)
            {
                if (checkRot_[i] <= uniPositions_[0].y && uniPositions_[0].y < checkRot_[i + 1])
                {
                    nowRotate_ = (rotate)i;
                    Debug.Log(uniPositions_[0].y + "   �����Ă����" + nowRotate_);
                }
            }
        }
    }

    public void CancelCheck()
    {
        if (fieldEndHit == true)
        {
            UniPushBack();
        }
        else
        {
            SetNowTownFlag(false);
        }
        UniChanController_.enabled = true;
        warpNowFlag_ = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[���t�B�[���h�[�ɐڐG�������ǂ���
        if (other.CompareTag("Player"))
        {
            // �t�B�[���h�̈ړ����\��
            locationSelMng_.gameObject.SetActive(true);
            fieldEndHit = true;
            SetWarpNowFlag(true);
            CheckUniTransfoem();// ���j�������Ă�������m��
            SetNowTownFlag(true);            // �t�B�[���h�[�ɐڐG�������p�̃��[�v
        }
    }

    // ���[�v�I�𒆂��ǂ���
    // ���j�����̃A�j���[�V�������~�߂���悤�ɁA�����ē����X�N���v�g���ł���������t���O��ύX����悤�ɂ��Ă���
    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;
        if(flag==false)
        {
            return;
        }
        // ���[�v�I�𒆂̓��j�����̃A�j���[�V�������~�߂āA�ړ�����s��
        UniChanController_.StopUniRunAnim();
        UniChanController_.enabled = false;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }

    // �X���Ńt�B�[���h�ɍs�����߂̃L�����o�X��\�����邩�ǂ���
    public bool GetLocationSelActive()
    {
        return locationSelMng_.gameObject.activeSelf;
    }

    public void SetNowTownFlag(bool flag)
    {
        // �X���Ńt�B�[���h�Ƀ��[�v���鎞
        // �t�B�[���h��Ń��[�v���鎞�AUpdata���Ȃ����߂����o�R�ŌĂяo���K�v������
        nowTownFlag_ = flag;
        locationSelMng_.gameObject.SetActive(flag);
    }
}