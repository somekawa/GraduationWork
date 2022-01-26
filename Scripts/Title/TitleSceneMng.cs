using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneMng : MonoBehaviour
{
    // 見栄えをよくするため（ゲーム起動後は非表示
    private Image panel_;

    // カメラ変更タイミングでフェードアウト、インさせる
    private Fade fade_;
    private bool fadeFlag_=false;

    // 縦（Z）移動カメラ
    private Camera zMoveCamera_;
    private Vector3 zC_StartPos_ = new Vector3(2.0f, 31.5f, 8.5f);// スタート座標
    private Vector3 zC_MaxPos_ = new Vector3(2.0f, 31.5f, 120.0f);// マックス座標

    // 横（X)移動カメラ
    private Camera xMoveCamera_;
    private Vector3 xC_StartPos_ = new Vector3(-55.0f, 6.5f, 90.0f);// スタート座標
    private Vector3 xC_MaxPos_ = new Vector3(40.0f, 6.5f, 90.0f);// マックス座標

    // シーン遷移先の名前
    private string sceneName_ = "";

    private MenuActive menuActive_;
    void Start()
    {
        panel_ = GameObject.Find("Canvas/Panel").GetComponent<Image>();
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();
        menuActive_= gameObject.GetComponent<MenuActive>();

        zMoveCamera_ = GameObject.Find("MainCamera").GetComponent<Camera>();
        zMoveCamera_.transform.position = zC_StartPos_;
        zMoveCamera_.depth = 1;

        xMoveCamera_ = GameObject.Find("SubCamera").GetComponent<Camera>();
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
            // カメラ変更
            if (zMoveCamera_.depth == 1)
            {
                zMoveCamera_.depth = -1;// 動かない
                xMoveCamera_.depth = 1;// 動く
                // 初期位置に戻す
                zMoveCamera_.transform.position = zC_StartPos_;
            }
            else
            {
                zMoveCamera_.depth = 1;// 動く
                xMoveCamera_.depth = -1;// 動かない
                // 初期位置に戻す
                xMoveCamera_.transform.position = xC_StartPos_;
            }

            if (sceneName_ != "")
            {
                Debug.Log("シーン移動します");
                // シーン遷移するタイミングで入る
                // カメラの動きを止める
                StopCoroutine(MoveCamera());
                if(sceneName_ == "Town")
                {
                    menuActive_.DataLoad();
                }
                SceneManager.LoadScene(sceneName_);// 指定のシーンに移動
            }
            
        }
        fadeFlag_ = false;
        yield break; // ループを抜ける
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
                        // フェードアウト開始タイミング
                        StartCoroutine(FadeOutAndIn());
                    }
                }
                else
                {
                    if (xC_MaxPos_.x - 1.0f < xMoveCamera_.transform.position.x)
                    {
                        // フェードアウト開始タイミング
                        StartCoroutine(FadeOutAndIn());
                    }
                }
            }
            if (zMoveCamera_.depth == 1)
            {
                zMoveCamera_.transform.position += new Vector3(0.0f, 0.0f, 0.05f);// 移動
            }
            else
            {
                xMoveCamera_.transform.position += new Vector3(0.03f, 0.0f, 0.0f);// 移動
            }
        }
    }

    public void OnClickNewGame()
    {
        sceneName_ = "conversationdata";
        StartCoroutine(FadeOutAndIn());
    }

    public void OnClickLoadGame()
    {
        sceneName_ = "Town";
        StartCoroutine(FadeOutAndIn());
    }
}