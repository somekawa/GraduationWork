using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// ��X�AQuestMng�Ƃ������삳��āA���C���N�G�X�g���i�ނ��тɁA���̃X�N���v�g���Ăяo����
// nowChapterNum_�����Z���A�V�����e�L�X�g��ǂݍ��ނ悤�ɂ��Ă�����΂������ȂƎv���B

public class TextMng : MonoBehaviour
{
    public GameObject ConversationCanvas;
    public GameObject CharacterList;

    enum FADE
    {
        NON,
        IN,
        OUT,
        MAX
    }

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

    private Image backImage_;        // ��b���̔w�i�摜(Excel����摜����ǂݍ���œ��I�ɍ����ւ���)
    private Image objectImage_;      // ��b���ɓo�ꂷ��摜(Excel����摜����ǂݍ���œ��I�ɍ����ւ���)

    private Fade fade_;              // �g�����W�V�����֘A(fadeout��fadein�֐����Ăяo�����߂ɕK�v)
    private readonly float fadeTimeMax_ = 3.0f;
    private float nowFadeTime_ = 0.0f;

    // �L�[���L������,�l���e�L�����̊��؂�ւ���N���X�̃}�b�v
    private Dictionary<string, UnityChan.FaceUpdate> charFacesMap_ = new Dictionary<string, UnityChan.FaceUpdate>();

    private List<Texture2D> texture2dList = new List<Texture2D>();
    private List<Sprite> spriteList = new List<Sprite>();

    private SceneMng.SCENE sceneLoadNum_;   // Excel����ǂݍ��񂾎��̐ؑ֗\��V�[����ۑ�����ϐ�
    private bool clickLock_ = false;        // �}�E�X�̉����������邩(true:�}�E�X�N���b�N�s��,false:�}�E�X�N���b�N��)

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
        icon_ = ConversationCanvas.transform.Find("NextMessage_icon").gameObject;
        iconColor_ = icon_.GetComponent<Image>();

        // �w�i�摜
        backImage_ = GameObject.Find("BackCanvas/BackImage").GetComponent<Image>();
        // �ʏ�摜
        objectImage_ = ConversationCanvas.transform.Find("ObjectImage").GetComponent<Image>();

        // �g�����W�V����
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();

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
                if (Input.GetMouseButtonDown(0) && !skipFlg_ && !clickLock_)
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
                        // �z������b�̏I���̍��}�Ƃ��Ďg����
                        Debug.Log("��b�I���ł�");

                        // ���̃`���v�^�[�Ŏg�����摜���܂Ƃ߂Ĕj������
                        // �V�[���J�ڂ��钼�O�����������A�e�X�g�Ƃ��Ă����ɏ����Ă��܂�
                        DestroyTexture2D();
                        SceneMng.SceneLoad((int)sceneLoadNum_);
                    }
                }
            }
        }
    }

    // �w�i�摜�̓ǂݍ��݂ƍ����ւ�
    void ChangeBackImage()
    {
        while (popChapter_.param[nowText_].name1 == "Image")
        {
            if(popChapter_.param[nowText_].name2 == "Back")
            {
                // �摜�����ւ�
                backImage_.sprite = CreateSprite(popChapter_.param[nowText_].name2);
            }
            else if(popChapter_.param[nowText_].name2 == "Object")
            {
                if (popChapter_.param[nowText_].message == "")
                {
                    objectImage_.sprite = null;
                }
                else
                {
                    // �莆�摜
                    objectImage_.sprite = CreateSprite(popChapter_.param[nowText_].name2);
                    // �`�悷��摜������Ƃ��͕\���ɂ���
                    objectImage_.gameObject.SetActive(true);
                }
            }
            else
            {
                // �����������s��Ȃ�
            }

            if (nowText_ < popChapter_.param.Count - 1)
            {
                // Excel�̍s��i�߂�
                nowText_++;
            }
            else
            {
                // while���ŉi�v���[�v�ɂȂ�Ȃ��悤�ɁA�Ō�̍s�Ȃ�break����
                break;
            }
        }

        // �`�悷��摜���Ȃ��Ƃ��͔�\���ɂ���
        if (objectImage_.sprite == null)
        {
            objectImage_.gameObject.SetActive(false);
        }
    }

    // �g�����W�V�������������邩�m�F����
    private void CheckTransition()
    {
        if (popChapter_.param[nowText_].name1 == "Fade")
        {
            // �t�F�[�h���ɂ́A�e�L�X�g���o���ꏊ���\���ɂ���
            ConversationCanvas.SetActive(false);

            if (popChapter_.param[nowText_].name2 == "Out")
            {
                // �t�F�[�h�A�E�g����
                fade_.FadeOut(fadeTimeMax_);
                nowFadeTime_ = fadeTimeMax_;
                // �����ŃR���[�`�����Ă�
                StartCoroutine(Transition(FADE.OUT));
            }
            else if(popChapter_.param[nowText_].name2 == "In")
            {
                // �t�F�[�h�C������
                fade_.FadeIn(fadeTimeMax_);
                nowFadeTime_ = fadeTimeMax_;
                // �����ŃR���[�`�����Ă�
                StartCoroutine(Transition(FADE.IN));
            }
            else
            {
                // �����������s��Ȃ�
            }
        }
        else
        {
            // �t�F�[�h���ȊO�̓L�����o�X��\���ɂ���
            ConversationCanvas.SetActive(true);
        }
    }

    // �g�����W�V�����̃R���[�`��  
    private IEnumerator Transition(FADE fade)
    {
        clickLock_ = true;

        while (fade != FADE.NON)
        {
            yield return null;

            if (nowFadeTime_ > 0.0f)
            {
                nowFadeTime_ -= Time.deltaTime;
            }
            else
            {
                if(fade != FADE.MAX)
                {
                    nowFadeTime_ = fadeTimeMax_;

                    // �ŏI�s�ł͖����Ƃ��͍s��i�߂�
                    if (nowText_ < popChapter_.param.Count - 1)
                    {
                        // Excel�̍s��i�߂�
                        nowText_++;
                        // ���̕\�������邽�߂ɁA�֐����Ăяo��
                        TextAndFaceSetting();
                    }
                }

                if (fade == FADE.IN)
                {
                    fade_.FadeOut(fadeTimeMax_);    // ���̃t�F�[�h����
                    fade = FADE.MAX;
                }
                else if (fade == FADE.OUT)
                {
                    fade_.FadeIn(fadeTimeMax_);     // ���̃t�F�[�h����
                    fade = FADE.MAX;
                }
                else
                {
                    // �t�F�[�h���ȊO�̓L�����o�X��\���ɂ���
                    ConversationCanvas.SetActive(true);
                    fade = FADE.NON;
                }
            }
        }

        clickLock_ = false;
    }

    // �V�[���؂�ւ�����
    private void ChangeScene()
    {
        // �V�[����ǂݍ��񂾏ꍇ�́A�ϐ��Ɉꎞ�ۑ����Ă���
        if (popChapter_.param[nowText_].name1 == "Scene")
        {
            // �������enum�ɕϊ����鏈��
            sceneLoadNum_ = (SceneMng.SCENE)System.Enum.Parse(typeof(SceneMng.SCENE), popChapter_.param[nowText_].name2);
        }
    }

    // ���O,���b�Z�[�W,�L�����̕\���ݒ肷��
    private void TextAndFaceSetting()
    {
        ChangeBackImage();
        CheckTransition();
        ChangeScene();

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
        if(popChapter_.param[nowText_].name1 != "Fade" && popChapter_.param[nowText_].name1 != "Scene")
        {
            name_.text = popChapter_.param[nowText_].name1;
        }

        // �L�����ȊO��Excel����"Mob"�Ɠo�^���Ă��邽�߁A"Mob"�Ȃ��̕\���ύX���Ȃ��悤�ɂ���
        if (popChapter_.param[nowText_].face != "Mob")
        {
            // ��̕\���ύX����
            charFacesMap_[popChapter_.param[nowText_].name2].OnCallChangeFace(popChapter_.param[nowText_].face);
        }

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

    // �X�v���C�g�̐���
    private Sprite CreateSprite(string path)
    {
        // �t�@�C���p�X�쐬
        string str = Application.streamingAssetsPath + "/Chapter" + path + "/"+ popChapter_.param[nowText_].message + ".png";
        // �t�@�C���p�X�ǂݍ���
        byte[] bytes = File.ReadAllBytes(str);

        // Texture2D�Ƃ��č쐬(Texture2D(2, 2)�Ƃ��Ă��邪�ALoadImage���s��ɃT�C�Y���X�V�����̂Ŗ��Ȃ�)
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        // Texture2D����Sprite�֕ϊ�
        Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        // �����ɂ͔j�����o���Ȃ��̂ŁA�ォ��j���ł���悤�Ƀ��X�g�ɓ���Ă���
        texture2dList.Add(texture);
        spriteList.Add(sprite);

        return sprite;
    }

    // Texture2D��Sprite�̉摜�j������
    private void DestroyTexture2D()
    {
        foreach (var tex in texture2dList)
        {
            Destroy(tex);
        }

        foreach (var spr in spriteList)
        {
            Destroy(spr);
        }
    }
}
