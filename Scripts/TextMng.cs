using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ��X�AQuestMng�Ƃ������삳��āA���C���N�G�X�g���i�ނ��тɁA���̃X�N���v�g���Ăяo����
// nowChapterNum_�����Z���A�V�����e�L�X�g��ǂݍ��ނ悤�ɂ��Ă�����΂������ȂƎv���B

// �e�L�X�g���X�L�b�v�����Ƃ��ɂ��܂������Ȃ��E�E�E

public class TextMng : MonoBehaviour
{
    public GameObject ConversationCanvas;
    public GameObject CharacterList;

    private GameObject DataPopPrefab_;

    private TMPro.TextMeshProUGUI name_;
    private TMPro.TextMeshProUGUI message_;
    private float messageTime_ = 0.0f;              // �\�����x����
    private readonly float messageSpeed_ = 0.1f;    // �������葬�x

    private GameObject icon_;        // ���̃e�L�X�g�ւ̍��}������A�C�R��
    private Image iconColor_;        // �_�ŏ����Ŏg�p����

    private ChapterList popChapter_; // ���ݕK�v�ȃe�L�X�g���������擾����
    private int nowChapterNum_ = 0;  // ���݂̃`���v�^�[�i�s�x(���ۂ�0����X�^�[�g)
    private int nowText_ = 0;        // ���݂̃e�L�X�g(�`���v�^�[�i�s�x���؂�ւ������0�ɏ��������邱��)

    private bool skipFlg_  = false;  // �e�L�X�g���؂�ւ��^�C�~���O��true�ɂȂ�
    private float skipItv_ = 0.0f;   // �����̑S�\���Ɏ���܂ł̃C���^�[�o��

    // �L�[���L������,�l���e�L�����̊��؂�ւ���N���X�̃}�b�v
    private Dictionary<string, UnityChan.FaceUpdate> charFacesMap_ = new Dictionary<string, UnityChan.FaceUpdate>();

    void Start()
    {
        // CharacterList�̎q�������ԂɎ擾���Ă����AcharFacesMap_�ɓo�^����
        Transform tmpt = CharacterList.gameObject.transform;
        for (int i = 0; i < tmpt.childCount; i++)
        {
            var childName = tmpt.GetChild(i).name;  // �q�̖��O������
            charFacesMap_.Add(childName, tmpt.Find(childName).GetComponent<UnityChan.FaceUpdate>());    // �o�^
        }

        //popChapter_ = TextPopPrefab_.GetComponent<PopList>().GetChapterList(nowChapterNum_);
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        popChapter_ = DataPopPrefab_.GetComponent<PopList>().GetData<ChapterList>(PopList.ListData.CHAPTER, nowChapterNum_);

        // Frame_text�̎q�ɂ���Message�Ƃ����e�L�X�g�I�u�W�F�N�g��T��
        message_ = ConversationCanvas.transform.Find("Frame_text/Message").GetComponent<TMPro.TextMeshProUGUI>();
        // Frame_name�̎q�ɂ���Name�Ƃ����e�L�X�g�I�u�W�F�N�g��T��
        name_ = ConversationCanvas.transform.Find("Frame_name/Name").GetComponent<TMPro.TextMeshProUGUI>();
        // nextMessage_icon�Ƃ����I�u�W�F�N�g��T��
        icon_ = ConversationCanvas.transform.Find("nextMessage_icon").gameObject;
        iconColor_ = icon_.GetComponent<Image>();
        TextAndFaceSetting();
    }

    void Update()
    {
        Skip();

        // �S�ĕ������\�����ꂽ��A�����I��false�ɂ���(���ꂪ�Ȃ��ƁA�r���Ńe�L�X�g���i�܂Ȃ��Ȃ�)
        if (message_.maxVisibleCharacters >= message_.text.Length)
        {
            skipFlg_ = false;
        }

        if (messageTime_ < messageSpeed_)    
        {
            // ���Ԃ𖞂����Ă��Ȃ���Ή��Z����
            messageTime_ += Time.deltaTime;
            return;
        }
        else
        {
            // �����̍ő�l�Ɣ�r����
            if (message_.maxVisibleCharacters < message_.text.Length)
            {
                // �\�����镶�����𑝂₵�Ă������ƂŁA�����珇�ɕ������łĂ���悤�Ɍ�����
                message_.maxVisibleCharacters++;
                messageTime_ = 0.0f;
            }
            else
            {
                // �A�C�R���̕\��
                icon_.SetActive(true);
                // �A�C�R���̓_�ŏ���(Time.time * 5.0��[5.0]�͓_�ő��x�����p�̐��l�ł�)
                iconColor_.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sin(Time.time * 5.0f) / 2 + 0.5f);

                // �}�E�X�̍��N���b�N������
                if (Input.GetMouseButtonDown(0) && !skipFlg_)
                {
                    // Excel�̗�̍ő吔���z���Ȃ��悤�ɂ���(Count��-1�����Ȃ��ƃG���[�ɂȂ�)
                    if (nowText_ < popChapter_.param.Count - 1)
                    {
                        Debug.Log("���̃e�L�X�g");
                        skipFlg_ = true;
                        skipItv_ = 0.0f;

                        nowText_++;
                        TextAndFaceSetting();
                    }
                    else
                    {
                        Debug.Log("��b�I���ł�");
                        // �z������b�̏I���̍��}�Ƃ��Ďg����
                    }
                }
            }
        }
    }

    // ���O,���b�Z�[�W,�L�����̕\���ݒ肷��
    void TextAndFaceSetting()
    {
        // Excel���ɉ��s����[\n]����������A���s���ĕ\������
        // ���b�Z�[�W�X�V
        if (popChapter_.param[nowText_].message.Contains("\\n"))
        {
            message_.text = popChapter_.param[nowText_].message.Replace("\\n", System.Environment.NewLine);
        }
        else
        {
            message_.text = popChapter_.param[nowText_].message;
        }

        // ���O�X�V
        name_.text = popChapter_.param[nowText_].name1;

        // ��̕\���ύX����
        charFacesMap_[popChapter_.param[nowText_].name2].OnCallChangeFace(popChapter_.param[nowText_].face);

        // �\���������̏�����
        // maxVisibleCharacters�F�ő�\��������
        message_.maxVisibleCharacters = 0;
        // �\�����x���Ԃ̏�����
        messageTime_ = 0.0f;
        // �A�C�R���̔�\��
        icon_.SetActive(false);
    }

    // 1������������\�����Ă���Ԃɍs������
    void Skip()
    {
        if (!skipFlg_)
        {
            return;
        }

        skipItv_ += Time.deltaTime;
        //Debug.Log(skipItv_);

        // 1�������\�������Ă����Ă�r���ō��N���b�N�����ƁA�S�Ẵe�L�X�g���\�������悤�ɂ���
        if (Input.GetMouseButtonUp(0) && message_.maxVisibleCharacters < message_.text.Length)
        {
            // �e�L�X�g���\�����n�߂�0.5f�ȏ�o�߂�����S�\�����\�ɂȂ�
            if (skipItv_ > 0.5f)
            {
                //Debug.Log("�����ɗ��܂���");

                skipFlg_ = false;
                skipItv_ = 0.0f;
                message_.maxVisibleCharacters = message_.text.Length;
            }
        }
    }
}
