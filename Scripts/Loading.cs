using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    //　非同期動作で使用するAsyncOperation
    private AsyncOperation async;
    //　シーンロード中に表示するUI画面
    private GameObject backGround;
    //　読み込み率を表示するスライダー
    private Slider slider;

    private Image uniImage_;        // ユニの2D画像
    private int uniAnimNum_ = 0;    // ユニの画像番号
    private int nowTimeCnt_ = 0;    // 現在の時間
    private float sceneTime_ = 0.0f;    // ロード後に数秒待つ時間の設定
    private bool sceneTimeFlg_ = false; // ロードが一定以上おわったらtrueにするフラグ

    private int buildNum_;          // ビルド番号

    public void NextScene(int buildNum)
    {
        uniAnimNum_ = 0;
        gameObject.GetComponent<AudioListener>().enabled = true;
        backGround.SetActive(true);
        buildNum_ = buildNum;
        sceneTimeFlg_ = false;

        // シーン読み込み
        async = SceneManager.LoadSceneAsync(buildNum_);
        // 読み込みが終了しても、すぐに遷移しないようにfalse
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

        // 2Dユニちゃんのアニメーション処理
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

        // ローディング状態を示すスライドバー
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
