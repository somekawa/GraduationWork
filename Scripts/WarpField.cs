using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpField : MonoBehaviour
{
    enum rotate
    {
        UP,// ��
        DOWN,// ��
        LEFT,// ��
        RIGHT,// �E
        MAX
    }
    private rotate nowRotate_;// �㉺���E�x�̕����������Ă��邩
    private float nowUniRotate_;// �����Ă�����̐��K����ۑ�
    // up315~360 0~45 down135~225 left225~315 right15~135
    private int[] checkRot_ = new int[6] { 0, 45, 135, 225, 315, 360 };// ���j�������Ă�����o�����߂͈̔�
    private Vector3 saveUniRot_;// ���j�������Ă������ۑ�

    // �t�B�[���h��Ń��[�v��I����
    enum kindsField
    {
        //  NON,
        TOWN,       // ��
        unitydata,  // �t�B�[���h1
        FIELD_2,    // �t�B�[���h2
        CANCEL,     // �I���L�����Z��
        MAX
    }

    // �\���֘A
    public Canvas locationSelCanvas;        // ���[�v����o��Canvas�i�e�j

    private Transform[] canvasChild_;       // �I���ł���t�B�[���h(locationSelCanvas�̎q
    private Image[] selectFieldImage_ = new Image[(int)kindsField.MAX];      // �I���ł���t�B�[���h�̔w�i�ilocationSelCanvas�̑�
    private Text[] selectFieldText_ = new Text[(int)kindsField.MAX];        // �I���ł���t�B�[���h�̕����ilocationSelCanvas�̑�
    private string[] fieldShowName_;            // �I���ł���t�B�[���h�̖��O(Text�ɑ��
    private string[] sceneName_;            // ���݂�scene�����m�F

    private float changeSelectCnt_ = 0.0f;  // �A����Space�L�[�̏����ɓ���Ȃ��悤�ɂ��邽��
    private bool fieldEndHit = false;       // ���[�v�I�u�W�F�N�g�ɐڐG�������ǂ���

    private int saveNowField_ ;    // ���݂���t�B�[���h��ۑ�
    private int selectFieldNum_; // �ǂ̃t�B�[���h��I��ł��邩�i1�X�^�[�g

    private Color nowImageColor_;               // �I�𒆂̐F�i�j
    private Color resetColor_;                  // �I���O�̐F�i���j

    // ���[�v�I�u�W�F�N�g�֘A
    private GameObject UniChan;         // ���j�����
    private GameObject[] warpObject_;   // ���[�v�I�u�W�F��ۑ�
    private Vector3[] saveObjPos_;

    private int maxWarpObjNum_ = 0;     // ���[�v�I�u�W�F�N�g�̍ő��
    private Vector3 uniNormalized_;     // �i���j���W�[���[�v�I�u�W�F�j�𐳋K��
    private Vector3 enterPos_;          // �����蔻����ɓ������u�Ԃ̍��W
    private Vector3 addPos_ = new Vector3(0.0f, 0.0f, 0.0f);       // �L�����Z�����������ۂɔ��Ε����ɂ͂���

    private bool nowTownFlag_ = false;
    private bool warpNowFlag_ = false;

    void Start()
    {
        // ���W�Ɖ�]��ς���\�������邽�߃��j���擾
        UniChan = GameObject.Find("Uni");

        nowImageColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f); // ��
        resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // ��
        locationSelCanvas.enabled = false;// �t�B�[���h�I���L�����o�X���\��

        sceneName_ = new string[(int)kindsField.MAX] {
            //"non",
            "towndata","unitydata","TestField","cansel"
        };
        fieldShowName_ = new string[(int)kindsField.MAX] {
            //"non",
            "Town","Field01","Field02","cansel"
        };

        // �\������w�i�ƕ����̐e�����Ă���
        canvasChild_ = new Transform[2];// 0Image 1Text
        canvasChild_[0] = locationSelCanvas.transform.GetChild(0).GetComponent<Transform>();
        canvasChild_[1] = locationSelCanvas.transform.GetChild(1).GetComponent<Transform>();

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {

            selectFieldImage_[i] = canvasChild_[0].transform.GetChild(i).GetComponent<Image>();
            selectFieldText_[i] = canvasChild_[1].transform.GetChild(i).GetComponent<Text>();

            selectFieldText_[i].text = fieldShowName_[i];// Text�ɕ���������


            //Debug.Log("fieldName_�F" + sceneName_[i]);
            //Debug.Log("�t�B�[���h�̊O�ړ���" + i + ";" + canvasChild_[0].transform.GetChild(i).GetComponent<Image>());

            if (SceneManager.GetActiveScene().name == sceneName_[i])
            {
                saveNowField_ = i;
            }
        }
        selectFieldImage_[saveNowField_].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        if(saveNowField_==(int)kindsField.TOWN)
        {
            selectFieldNum_ = (int)kindsField.unitydata;
        }
        else
        {
            selectFieldNum_ = (int)kindsField.TOWN;
        }


        //Debug.Log("SceneName�F" + SceneManager.GetActiveScene().name);



        maxWarpObjNum_ = this.transform.childCount;
        warpObject_ = new GameObject[maxWarpObjNum_];
        saveObjPos_ = new Vector3[maxWarpObjNum_];
        for (int i = 0; i < maxWarpObjNum_; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            saveObjPos_[i] = warpObject_[i].transform.position;
            //Debug.Log(warpObject_[i].name + "���̃��[�v���\" + warpObject_[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(nowTownFlag_==true)
        {
            SelectGoToFiled();
            return;
        }

        //  Debug.Log("�I�𒆂̃t�B�[���h" + selectFieldNum_);
        if (fieldEndHit == true)
        {
            SelectGoToFiled();
        }
    }

    private void TownFieldSelect()
    {

    }

    private void SelectGoToFiled()
    {
        // Debug.Log(choiceFieldNum_);

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            if (saveNowField_ == i)
            {
                continue;
            }

            if (selectFieldNum_ == i)
            {
                selectFieldImage_[selectFieldNum_].color = nowImageColor_;  // �I�𒆂͐�
            }
            else
            {
                selectFieldImage_[i].color = resetColor_;            // ����ȊO�͔�
            }
        }

        // warp��field���̂���warp�̎���2�{
        if (changeSelectCnt_ < 0.5f)
        {
            // �t�B�[���h�I�𒆂�Space�L�[����������Ȃ��悤�ɂ��邽��
            changeSelectCnt_ += Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectFieldNum_ < (int)kindsField.CANCEL)
            {
                selectFieldNum_++;      // ���Ɉړ�
            }
            if (selectFieldNum_ == saveNowField_)
            {
                selectFieldNum_++;
            }

            Debug.Log("���Ɉړ�" + selectFieldNum_);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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

        if (Input.GetKey(KeyCode.Space))
        {
            // �L�����Z���I����
            if (selectFieldNum_ == (int)kindsField.CANCEL)
            {
                // �t�B�[���h�[�ɐڐG������
                if (fieldEndHit == true)
                {
                    RotateCheck();
                    for (int i = 0; i < maxWarpObjNum_; i++)
                    {
                        uniNormalized_ = (enterPos_ - warpObject_[i].transform.position).normalized;
                        //Debug.Log(warpObject_[i].name + "�Ƃ̐��K��" + uniNormalized_);
                        if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
                        {
                            nowUniRotate_ = uniNormalized_.z;
                            addPos_ = new Vector3(0.0f, 0.0f, 0.5f);
                        }
                        else
                        {
                            nowUniRotate_ = uniNormalized_.x;
                            addPos_ = new Vector3(-0.5f, 0.0f, 0.0f);
                        }
                    }
                    if (nowUniRotate_ < 0.0f)
                    {
                        // �o�悤�Ƃ��������̔��Α��ɉ��Z
                        addPos_ = -addPos_;
                    }
                    UniChan.transform.rotation = Quaternion.Euler(0.0f, UniChan.transform.rotation.y*180, 0.0f);
                    UniChan.transform.position = new Vector3(
                                enterPos_.x + addPos_.x,
                                enterPos_.y,
                                enterPos_.z + addPos_.z);

                    fieldEndHit = false;
                }

                if (saveNowField_ == (int)kindsField.TOWN)
                {
                    selectFieldNum_ = (int)kindsField.unitydata; // �t�B�[���h�̍s��������Z�b�g
                }
                else
                {
                    selectFieldNum_ = (int)kindsField.TOWN; // �t�B�[���h�̍s��������Z�b�g
                }
                locationSelCanvas.enabled = false;    // �t�B�[���h�I���L�����o�X��\��
                changeSelectCnt_ = 0.0f;
            }
            else
            {
                Debug.Log(selectFieldNum_+ "��I�𒆁BScene���ړ����܂�");
                //  SceneMng.SceneLoadUnLoad(selectFieldNum_, saveNowField_);
            }
            nowTownFlag_ = false;
            warpNowFlag_ = false;
        }
    }

    private void RotateCheck()
    {
        if (checkRot_[1] <= saveUniRot_.y && saveUniRot_.y < checkRot_[2])
        {
            // �E��
            nowRotate_ = rotate.RIGHT;
        }
        else if (checkRot_[2] <= saveUniRot_.y && saveUniRot_.y < checkRot_[3])
        {
            // ����
            nowRotate_ = rotate.DOWN;
        }
        else if (checkRot_[3] <= saveUniRot_.y && saveUniRot_.y < checkRot_[4])
        {
            // ����
            nowRotate_ = rotate.LEFT;
        }
        else if ((checkRot_[4] < saveUniRot_.y && saveUniRot_.y < checkRot_[5])
            || (checkRot_[0] <= saveUniRot_.y && saveUniRot_.y < checkRot_[1]))
        {
            // �㑤
            nowRotate_ = rotate.UP;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fieldEndHit == false)
        {
            if (other.CompareTag("Player"))
            {
                locationSelCanvas.enabled = true;         // �t�B�[���h�̈ړ����\��
                fieldEndHit = true;
                enterPos_ = UniChan.transform.position;// �[�ƐڐG�������A���j�̍��W��ۑ�
                saveUniRot_ = UniChan.transform.localEulerAngles;


                warpNowFlag_ = true;
            }
        }
    }

    //public void SetWarpFieldFlag(bool flag)
    //{
    //    fieldEndHit = flag;
    //}

    //public bool GetWarpFieldFlag()
    //{
    //    return fieldEndHit;
    //}

    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }


    // �X���Ńt�B�[���h�Ƀ��[�v���鎞�p
    public void SetNowTownFlag(bool flag)
    {
        nowTownFlag_ = flag;
    }


    public bool GetLocationSelActive()
    {
        return locationSelCanvas.enabled;
    }

    public void SetLocationSelActive(bool flag)
    {
        locationSelCanvas.enabled = flag;
    }
}