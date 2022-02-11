using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Endroll : MonoBehaviour
{
    public RectTransform scrollTextTrans;

    private Vector3 scrollTextPos_;
    private const float endPosY = 850.0f;       // �X�N���[���I�����W
    private float scrollMove_ = 0.0f;           // �ǂꂮ�炢�X�N���[������������
    private bool scrollFinFlg_ = false;         // �X�N���[���������I���������ǂ���
    private GameObject buttons_;

    // �摜�\�����z��
    private string[] pictureName_ =
    {
        "UniHouse",
        "MayorHouse",
        "Guild",
        "ItemStore",
        "BookStore",
        "Restaurant",
    };
    private float changePictureTime_ = 0.0f;  // �ǂꂮ�炢�X�N���[��������摜��؂�ւ��邩
    private int pictureNowNum_ = 0;             // ���݂̕\�����摜

    private Image objectImage_;                 // �X�N���[�����ɓo�ꂷ��摜
    private List<Texture2D> texture2dList = new List<Texture2D>();
    private List<Sprite> spriteList = new List<Sprite>();

    void Start()
    {
        scrollTextPos_ = scrollTextTrans.anchoredPosition;

        var canvas = GameObject.Find("Canvas").transform;
        objectImage_ = canvas.Find("Image").GetComponent<Image>();
        buttons_ = GameObject.Find("Buttons").gameObject;
    }

    void Update()
    {
        if(scrollFinFlg_)
        {
            return; // �X�N���[���������I�����Ă���Ȃ�return����
        }

        // �ڕW�l�ȉ��Ȃ�X�N���[�����X�V����
        if (scrollTextTrans.anchoredPosition.y < endPosY)
        {
            scrollTextPos_.y += 0.2f;
            scrollTextTrans.anchoredPosition = scrollTextPos_;
        }
        else
        {
            // �X�N���[���I����A�{�^����\����Ԃɐ؂�ւ���
            for(int i = 0; i < buttons_.transform.childCount; i++)
            {
                buttons_.transform.GetChild(i).gameObject.SetActive(true);
            }
            scrollFinFlg_ = true;
        }

        // �܂����̉摜������Ƃ�
        if (pictureName_.Length > pictureNowNum_)
        {
            // �}�C�i�X�l�ƃv���X�l�ŕ␳������Ȃ���scrollMove_�ɒl������
            if (scrollTextTrans.anchoredPosition.y < 0.0f)
            {
                scrollMove_ = endPosY - Mathf.Abs(scrollTextTrans.anchoredPosition.y) + 50;
            }
            else
            {
                scrollMove_ = scrollTextTrans.anchoredPosition.y + 900;
            }

            // �摜�؂�ւ��̃^�C�~���O�ɂȂ�����
            if (changePictureTime_ < scrollMove_)
            {
                // �摜��ؑւĖڕW�l�𑝂₷
                objectImage_.sprite = CreateSprite(pictureName_[pictureNowNum_]);
                pictureNowNum_++;
                changePictureTime_ += 350.0f;
                Debug.Log("�摜�ؑ�" + scrollMove_);
            }
        }
    }

    // �X�v���C�g�̐���
    private Sprite CreateSprite(string path)
    {
        // �t�@�C���p�X�쐬
        string str = Application.streamingAssetsPath + "/ChapterBack/" + path + ".png";
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

    // �Z�[�u���ă^�C�g���֖߂�
    public void TownBack()
    {
        DestroyTexture2D();
        SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);
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