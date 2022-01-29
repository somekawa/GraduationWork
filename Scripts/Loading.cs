using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //�@�񓯊�����Ŏg�p����AsyncOperation
    private AsyncOperation async;
    //�@�V�[�����[�h���ɕ\������UI���
    private GameObject backGround;
    //�@�ǂݍ��ݗ���\������X���C�_�[
    private Slider slider;

    private Image uniImage_;        // ���j��2D�摜
    private int uniAnimNum_ = 0;    // ���j�̉摜�ԍ�
    private int nowTimeCnt_ = 0;    // ���݂̎���
    private float sceneTime_ = 0.0f;    // ���[�h��ɐ��b�҂��Ԃ̐ݒ�
    private bool sceneTimeFlg_ = false; // ���[�h�����ȏエ�������true�ɂ���t���O

    private int buildNum_;          // �r���h�ԍ�

    public void NextScene(int buildNum)
    {
        uniAnimNum_ = 0;
        gameObject.GetComponent<AudioListener>().enabled = true;
        backGround.SetActive(true);
        buildNum_ = buildNum;
        sceneTimeFlg_ = false;

        // �V�[���ǂݍ���
        async = SceneManager.LoadSceneAsync(buildNum_);
        // �ǂݍ��݂��I�����Ă��A�����ɑJ�ڂ��Ȃ��悤��false
        async.allowSceneActivation = false;
    }

    void Update()
    {
        if (backGround == null)
        {
            backGround = GameObject.Find("LoadingCanvas/Background").gameObject;
            slider = backGround.transform.Find("Slider").GetComponent<Slider>();
            uniImage_ = backGround.transform.Find("Image").GetComponent<Image>();
            uniImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.LOADING_UNI][0];
            backGround.SetActive(false);
        }

        if (!backGround.activeSelf)
        {
            if (gameObject.GetComponent<AudioListener>().isActiveAndEnabled)
            {
                gameObject.GetComponent<AudioListener>().enabled = false;
            }
            return;
        }

        // 2D���j�����̃A�j���[�V��������
        if(nowTimeCnt_ % 20 == 0)
        {
            nowTimeCnt_ = 0;
            uniImage_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.LOADING_UNI][uniAnimNum_];
            if (++uniAnimNum_ >= ItemImageMng.spriteMap[ItemImageMng.IMAGE.LOADING_UNI].Length)
            {
                uniAnimNum_ = 0;
            }
        }
        nowTimeCnt_++;

        // ���[�f�B���O��Ԃ������X���C�h�o�[
        slider.value = Mathf.Clamp01(async.progress / 0.93f);

        if(async.progress >= 0.9f && !sceneTimeFlg_)
        {
            sceneTime_ = 2.0f;
            sceneTimeFlg_ = true;
        }

        if(sceneTimeFlg_)
        {
            sceneTime_ -= Time.deltaTime;

            if(sceneTime_ <= 0.0f)
            {
                slider.value = 1.0f;
                async.allowSceneActivation = true;
            }
        }
    }
}
