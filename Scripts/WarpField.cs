using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpField : MonoBehaviour
{
    // �\���֘A
    public Canvas locationSelCanvas;        // ���[�v����o��Canvas�i�e�j

    // �t�B�[���h�Ƀ��[�v���鎞
    private enum kindsField
    {
        NON = -1,     // -1
        TOWN,       // 0 ��
        unitydata,  // 1 �t�B�[���h1
        FIELD_2,    // 2 �t�B�[���h2
        CANCEL,     // 3 �I���L�����Z��
        MAX         // 4
    }
    private GameObject[] warpObject_;   // �}�b�v�[�̃��[�v�I�u�W�F��ۑ�

    // Canvas-Image,Text�B����ɘA�Ȃ鑷0Image 1Text;  
    private Transform[] canvasChild_ = new Transform[2];     // �I���ł���t�B�[���h(locationSelCanvas�̎q
    private Image[] selectFieldImage_ = new Image[(int)kindsField.MAX];      // �I���ł���t�B�[���h�̔w�i�ilocationSelCanvas�̑�
    private Text[] selectFieldText_ = new Text[(int)kindsField.MAX];        // �I���ł���t�B�[���h�̕����ilocationSelCanvas�̑�

    // [���݂�scene�����m�F�A�\������t�B�[���h��]
    private string[,] sceneName_ = new string[2, (int)kindsField.MAX]{
            { "Town","ForestField","TestField","cansel" },
            { "Town","Field01","Field02","cansel" }
    };

    private float changeSelectCnt_ = 0.0f;  // �A����Space�L�[�̏����ɓ���Ȃ��悤�ɂ��邽��

    private int saveNowField_;    // ���݂���t�B�[���h��ۑ�
    private int selectFieldNum_;  // �ǂ̃t�B�[���h��I��ł��邩�i1�X�^�[�g

    private Color nowImageColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f);  // �I�𒆂̐F�i�j
    private Color resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);     // �I���O�̐F�i���j

    private bool fieldEndHit = false;   // ���[�v�I�u�W�F�N�g�ɐڐG�������ǂ���
    private bool nowTownFlag_ = false;  // �����[�v����̃t�B�[���h���[�v��
    private bool warpNowFlag_ = false;  // �t�B�[���h���[�v��I�𒆂̎�

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

    private GameObject UniChan;         // ���j
    private Vector3 saveUniRot_ = new Vector3(0.0f, 0.0f, 0.0f);    // ���j�������Ă������ۑ�
    private Vector3 enterPos_ = new Vector3(0.0f, 0.0f, 0.0f);      // �ڐG�����u�Ԃ̍��W��ۑ�
    private Vector3 addPos_ = new Vector3(0.0f, 0.0f, 0.0f);        // �L�����Z�����ɔ��Ε����ɒe��

    // �i���j���W�[���[�v�I�u�W�F�j�𐳋K��
    private Vector3 uniNormalized_ = new Vector3(0.0f, 0.0f, 0.0f);
    private float rotateNormalized_ = 0.0f;// �����Ă�����̐��K����ۑ�


    void Start()
    {
        // ���W�Ɖ�]��ς���\�������邽�߃��j���擾
        UniChan = GameObject.Find("Uni");

        // �t�B�[���h�I���L�����o�X���\��
        locationSelCanvas.gameObject.SetActive(false);

        // �\������w�i�ƕ����̐e�����Ă���
        canvasChild_[0] = locationSelCanvas.transform.GetChild(0).GetComponent<Transform>();
        canvasChild_[1] = locationSelCanvas.transform.GetChild(1).GetComponent<Transform>();

        if(SceneManager.GetActiveScene().name=="InHouseAndUniHouse")
        {
            sceneName_[0, 0] = "InHouseAndUniHouse";
        }

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            selectFieldImage_[i] = canvasChild_[0].transform.GetChild(i).GetComponent<Image>();
            selectFieldText_[i] = canvasChild_[1].transform.GetChild(i).GetComponent<Text>();
            selectFieldText_[i].text = sceneName_[1, i];// Text�ɕ���������

            if (SceneManager.GetActiveScene().name == sceneName_[0, i])
            {
                saveNowField_ = i;// ���݂̃V�[�����ƍ����Ă�����̂�ۑ�
            }
        }
        // �ۑ������݂���V�[���@�I�ׂȂ��F�ɂ���
        selectFieldImage_[saveNowField_].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        // �I�𒆂̏����ꏊ�����߂�
        if (saveNowField_ == (int)kindsField.TOWN)
        {
            selectFieldNum_ = (int)kindsField.unitydata;
        }
        else
        {
            selectFieldNum_ = (int)kindsField.TOWN;
        }

        // �}�b�v�[�ɂ���I�u�W�F�N�g�������i�t�B�[���h�ɂ���Č����Ⴄ���ߎq�̌��Ō���j
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "���̃��[�v���\" + warpObject_[i].transform.position);
        }
    }

    // �R���[�`��  
    private IEnumerator Select()
    {
        // �R���[�`���̏���(�Ԃ�l��true�Ȃ珈���𑱍s����)  
        while (SelectGoToFiled())
        {
            yield return null;
        }
    }

    private bool SelectGoToFiled()
    {
        // �I�𒆂̉摜�̐F��ύX
        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            if (saveNowField_ == i)
            {
                continue;  // ���݂���Scene�̐F��Start�Ŏw�肵���F�B�ω����Ȃ�
            }

            if (selectFieldNum_ == i)
            {
                selectFieldImage_[selectFieldNum_].color = nowImageColor_;  // ���ݑI�𒆂̎��i��
            }
            else
            {
                selectFieldImage_[i].color = resetColor_; // �I������ĂȂ����i��
            }
        }

        // �t�B�[���h�I�𒆂�Space�L�[����������Ȃ��悤�ɂ��邽��
        if (changeSelectCnt_ < 0.5f)
        {
            changeSelectCnt_ += Time.deltaTime;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectFieldNum_ < (int)kindsField.CANCEL)
            {
                selectFieldNum_++;      // ���Ɉړ�
            }
            if (selectFieldNum_ == saveNowField_)
            {
                selectFieldNum_++;// ���݃V�[���̏ꍇ�͂�����x���Z
            }
            return true;
            //Debug.Log("���Ɉړ�" + selectFieldNum_);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (saveNowField_ != (int)kindsField.TOWN)
            {
                if ((int)kindsField.TOWN < selectFieldNum_)
                {
                    selectFieldNum_--;    // ��Ɉړ�
                }
                if (selectFieldNum_ == saveNowField_)
                {
                    selectFieldNum_--;
                }
            }
            else
            {
                // ���ɂ���Ƃ��͒�����ԏ�̂��ߌ��Z���Ăق����Ȃ�
                if ((int)kindsField.TOWN + 1 < selectFieldNum_)
                {
                    selectFieldNum_--;    // ��Ɉړ�
                }
            }
            return true;
        }
        else
        {
            // �����������s��Ȃ�
        }

        // �s�挈��
        if (Input.GetKey(KeyCode.Space))
        {
            // �L�����Z���ȊO�̎����V�[���J�ڂ�����
            if (selectFieldNum_ != (int)kindsField.CANCEL)
            {
                Debug.Log("�R���[�`���X�g�b�v");
                StopCoroutine(Select());                // �R���[�`���X�g�b�v
                //Debug.Log(selectFieldNum_+ "��I�𒆁BScene���ړ����܂�");
                WarpTown.warpNum_ = 0;// �t�B�[���h����^�E���ɖ߂������̂��߂�0�ɖ߂��Ă���
                SceneMng.SceneLoad(selectFieldNum_);
            }
            else
            {
                // �t�B�[���h�[�ɐڐG�����ۂ̓��j����]�����ĉ����Ԃ��K�v������
                if (fieldEndHit == true)
                {
                    UniPushBack();
                }

                // �I�𒆂̈ʒu��������
                if (saveNowField_ == (int)kindsField.TOWN)
                {
                    selectFieldNum_ = (int)kindsField.unitydata; // �t�B�[���h�̍s��������Z�b�g
                }
                else
                {
                    selectFieldNum_ = (int)kindsField.TOWN; // �t�B�[���h�̍s��������Z�b�g
                }

                // �t�B�[���h�I���L�����o�X��\��
                locationSelCanvas.gameObject.SetActive(false);
                changeSelectCnt_ = 0.0f;

                Debug.Log("�R���[�`���X�g�b�v");
                StopCoroutine(Select());                // �R���[�`���X�g�b�v
            }
            nowTownFlag_ = false;
            warpNowFlag_ = false;
        }
        return nowTownFlag_;
    }


    private void UniPushBack()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            uniNormalized_ = (enterPos_ - warpObject_[i].transform.position).normalized;
            //Debug.Log(warpObject_[i].name + "�Ƃ̐��K��" + uniNormalized_);
            if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
            {
                rotateNormalized_ = uniNormalized_.z;
                addPos_ = new Vector3(0.0f, 0.0f, 0.5f);
                Debug.Log("�ォ������ڐG");
            }
            else
            {
                rotateNormalized_ = uniNormalized_.x;
                addPos_ = new Vector3(-0.5f, 0.0f, 0.0f);
                Debug.Log("�E��������ڐG");
            }
        }

        if (rotateNormalized_ < 0.0f)
        {
            // �o�悤�Ƃ��������̔��Α��ɉ��Z
            addPos_ = -addPos_;
        }
        // +180�x�Ŕ��Ε������ނ�����
        UniChan.transform.rotation = Quaternion.Euler(0.0f, saveUniRot_.y + 180, 0.0f);
        UniChan.transform.position = enterPos_ + addPos_;
        fieldEndHit = false;

    }

    private void CheckUniTransfoem()
    {
        // ���j�̍��W�ƌ����Ă������ۑ�
        enterPos_ = UniChan.transform.position;
        saveUniRot_ = UniChan.transform.localEulerAngles;

        if ((checkRot_[4] < saveUniRot_.y && saveUniRot_.y < checkRot_[5])
         || (checkRot_[0] <= saveUniRot_.y && saveUniRot_.y < checkRot_[1]))
        {
            nowRotate_ = rotate.UP;            // �㑤
        }
        else
        {
            // �㑤�ȊO�̕����̎�
            for (int i = 1; i < (int)rotate.MAX; i++)
            {
                if (checkRot_[i] <= saveUniRot_.y && saveUniRot_.y < checkRot_[i + 1])
                {
                    nowRotate_ = (rotate)i;
                    Debug.Log(saveUniRot_.y + "   �����Ă����" + nowRotate_);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[���t�B�[���h�[�ɐڐG�������ǂ���
        if (other.CompareTag("Player"))
        {
            // �t�B�[���h�̈ړ����\��
            locationSelCanvas.gameObject.SetActive(true);
            fieldEndHit = true;
            warpNowFlag_ = true;
            CheckUniTransfoem();// ���j�������Ă�������m��
            SetNowTownFlag(true);            // �t�B�[���h�[�ɐڐG�������p�̃��[�v
        }
    }

    // ���[�v�I�𒆂��ǂ���
    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }

    // �X���Ńt�B�[���h�ɍs�����߂̃L�����o�X��\�����邩�ǂ���
    public bool GetLocationSelActive()
    {
        return locationSelCanvas.gameObject.activeSelf;
    }

    public void SetLocationSelActive(bool flag)
    {
        locationSelCanvas.gameObject.SetActive(flag);
    }

    public void SetNowTownFlag(bool flag)
    {
        // �X���Ńt�B�[���h�Ƀ��[�v���鎞
        // �t�B�[���h��Ń��[�v���鎞�AUpdata���Ȃ����߂����o�R�ŌĂяo���K�v������
        nowTownFlag_ = flag;

        if (flag)
        {
            Debug.Log("�R���[�`���X�^�[�g");
            // �R���[�`���X�^�[�g
            StartCoroutine(Select());
        }
    }

}