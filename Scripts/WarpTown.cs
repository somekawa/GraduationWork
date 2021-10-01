using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpTown : MonoBehaviour
{
    private enum warp
    {
        NON = -1,     // -1
        HOUSE,      // 0 ユニの家
        FOUNTAIN,   // 1 噴水前
        CROSS,      // 2 十字路
        FIELD,      // 3 フィールド
        MAX         // 
    }

    // 街中ワープ関連
    private Canvas warpCanvas_;       // 街中のワープ先を表示するキャンバス

    private GameObject warpInTown_;         // 街中ワープ先を持つ親オブジェクト
    private GameObject[] warpChildren_;     // 街中のワープ先
    private float changeCnt_ = 0.0f;        // 連続でSpaceキーの処理に入らないようにするため
    private bool decisionFlag_ = false;     // ワープ先が決まったかどうか

    // ワープ選択画像背景
    private Image frameImage_;
    private Vector2 saveHandlePos_;
    private Vector2 maxHandlePos_;
    private float handleSpeed_ = 8.0f;
    private float handleRotate_ = 1.0f;
    private Image warpActive_;   // warpCanvasの子の背景画像を取得

    // 長針関連
    private Image needleImage_;              // warpActive(背景の子)から長針画像を取得
    private float[] needleRotate_ = new float[(int)warp.MAX] {
            32.0f, 12.0f,  -12.0f, -32.0f
        };
    // 長針の回転先を保存
    private bool moveNeedle_ = false;       // 長針が回転しても良い状態かチェック 
    public static int warpNum_ = (int)warp.HOUSE; // 長針がどこを指しているか（1スタート

    // 町から出るとき関連
    private WarpField warpFieldScript_;  // warpField.cs

    // カメラ関連　座標関連
    private GameObject uniChan_;         // ユニちゃん
    private GameObject cameraCtl_;      // CameraMng.csがアタッチされてるオブジェクト
    private CameraMng cameraMng_;        // メインカメラとサブカメラの切り替え
    private Vector3 starMainCameraPos_; // ユニティちゃんとカメラの座標差を出すため初期座標を保存

    // フェードアウト関連
    private Image fadePanel;         // warpCanvasの子のフェードアウト用パネルを取得
    private float alphaCnt_ = 0.0f;  // panelのアルファ値

    // 家の前にいる場合
    private Vector3 houseFrontPos_ = new Vector3(20.0f, 0.0f, 10.0f);
    private Collider houseWarpColl_;
    private int count_ = 0;

    // Start関数から名称を変更し、TownMng.csのStart関数で呼び出されるように修正
    public void Init()
    {
        // ワープする座標を入れるため
        uniChan_ = GameObject.Find("Uni");

        // オブジェクト経由でWarpFieldScriptを取得
        warpFieldScript_ = GameObject.Find("WarpOut").transform.GetComponent<WarpField>();

        warpCanvas_ = GameObject.Find("WarpCanvas").GetComponent<Canvas>();
        warpActive_ = warpCanvas_.transform.Find("WarpImageMask").GetComponent<Image>();
        // フェードアウトするためのパネル
        fadePanel = warpCanvas_.transform.Find("FadePanel").GetComponent<Image>();

        // ワープイメージのフレーム
        frameImage_ = warpCanvas_.transform.Find("FrameImage").GetComponent<Image>();
        saveHandlePos_ = new Vector2(frameImage_.transform.localPosition.x, frameImage_.transform.localPosition.y);
        maxHandlePos_ = new Vector2(saveHandlePos_.x - 200, saveHandlePos_.y + 200);
        Debug.Log("Max座標" + maxHandlePos_ + "  初期値" + saveHandlePos_);


        // 長針画像取得
        needleImage_ = warpActive_.transform.Find("Needle").GetComponent<Image>();
        // 長針の初期位置
        needleImage_.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate_[(int)warp.HOUSE]);

        if (SceneManager.GetActiveScene().name == "Town")
        {
            // オブジェクト経由でCameraMngScriptを取得
            cameraCtl_ = GameObject.Find("CameraController");
            cameraMng_ = cameraCtl_.transform.GetComponent<CameraMng>();
            // メインカメラの初期画像を取得
            starMainCameraPos_ = cameraMng_.mainCamera.transform.position;
        }

        // ワープ先のオブジェクトの親を取得
        warpInTown_ = GameObject.Find("WarpInTown");

        // 街中のワープ先を保存 フィールドを含まない
        warpChildren_ = new GameObject[(int)warp.MAX - 1];
        for (int i = (int)warp.HOUSE; i < (int)warp.MAX - 1; i++)
        {
            // 親からワープ先の子どもを順に代入
            warpChildren_[i] = warpInTown_.transform.GetChild(i).gameObject;
        }
        houseWarpColl_ = this.gameObject.transform.Find("UniHouseCollider").GetComponent<Collider>();
        houseWarpColl_.isTrigger = false;

        if (warpNum_ != (int)warp.HOUSE)
        {
            needleImage_.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate_[warpNum_]);

            if (SceneManager.GetActiveScene().name == "Town")
            {
                uniChan_.transform.position = warpChildren_[warpNum_].transform.position;
                // 初期座標の差でカメラが追従しているため
                cameraMng_.mainCamera.transform.position =
                 uniChan_.transform.position + starMainCameraPos_;
            }
            else
            {
                uniChan_.transform.position = houseFrontPos_;
            }
        }
    }

    void Update()
    {
        if (houseWarpColl_.isTrigger == false)
        {
            if (count_ < 60)
            {
                count_++;
            }
            else
            {
                houseWarpColl_.isTrigger = true;
                Debug.Log("istriggerをtrueにしました");
            }
        }

        if (decisionFlag_ == true)
        {
            FadeOutAndIn();
        }


        // フィールドキャンバスがアクティブの時
        if (warpFieldScript_.GetLocationSelActive() == true)
        {
            // 常に回転させる
            handleRotate_ += 30.0f * Time.deltaTime;
            frameImage_.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, handleRotate_);
            return;   // 町中のワープは選択できないように
        }
        else
        {
            // 長針が動かない＝ワープ先を選べない
            if (moveNeedle_ == false)
            {
                // 常に回転させる
                handleRotate_ += 30.0f * Time.deltaTime;
                frameImage_.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, handleRotate_);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // ワープ準備
                    moveNeedle_ = true;
                    warpFieldScript_.SetWarpNowFlag(true);
                }
            }
            else
            {
                // moveNeedle_=trueになった瞬間にワープ選択処理に入らないようにする
                if (changeCnt_ < 0.5f)
                {
                    // ワープ処理に入る時のSpaceキー処理が入らないようにするため
                    changeCnt_ += Time.deltaTime;
                    return;
                }
                ChoiceWarpLocation();            // ワープ先選択処理
            }
        }
    }

    private void ChoiceWarpLocation()
    {
        if (maxHandlePos_.x < frameImage_.transform.localPosition.x
            && frameImage_.transform.localPosition.y < maxHandlePos_.y)
        {
            handleSpeed_ += 1.0f * Time.deltaTime;
            frameImage_.transform.localPosition -= new Vector3(handleSpeed_, -handleSpeed_, 0.0f);
            warpActive_.transform.localPosition -= new Vector3(handleSpeed_, -handleSpeed_, 0.0f);
            //Debug.Log(maxHandlePos_ + "    <   " + frameImage_.transform.localPosition.x);
        }
        else
        {
            // スピードが変わるため初期化
            handleSpeed_ = 8.0f;
        }
        // ワープ先選択中は反対周りにする
        handleRotate_ -= 30.0f * Time.deltaTime;
        frameImage_.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, handleRotate_);

        // 選択先を変更
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(WarpCansel(true));            // ワープキャンセルの場合
            warpFieldScript_.SetWarpNowFlag(false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (warpNum_ < (int)warp.FIELD)
            {
                warpNum_++;      // 右に移動
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if ((int)warp.HOUSE < warpNum_)
            {
                warpNum_--;    // 左に移動
            }
        }
        else
        {
            // 何もしない
        }

        for (int i = (int)warp.HOUSE; i < (int)warp.MAX; i++)
        {
            if (warpNum_ == i)
            {
                // 長針の角度
                needleImage_.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate_[i]);
                break;
            }
        }

        // 行先決定時
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (warpNum_ != (int)warp.FIELD)
            {
                // フィールド以外のワープ先確定
                decisionFlag_ = true;
                warpFieldScript_.SetWarpNowFlag(false);
            }
            else
            {                // フィールド選択時
                warpFieldScript_.SetLocationSelActive(true);         // フィールドの移動先を表示
                warpFieldScript_.SetNowTownFlag(true);// 町からのワープだとFlagをtrueにする
            }
            StartCoroutine(WarpCansel(true));            // ワープ処理リセット
        }
    }

    private void FadeOutAndIn()
    {
        // 町中をワープする場合
        if (alphaCnt_ < 1.0f)
        {
            uniChan_.SetActive(false);
            alphaCnt_ += Time.deltaTime * 2;
        }
        else if (1.0f < alphaCnt_)
        {
            uniChan_.SetActive(true);
            alphaCnt_ = 0.0f;
            decisionFlag_ = false;

            if (SceneManager.GetActiveScene().name != "Town")
            {
                SceneMng.SceneLoad(0);
            }
            else
            {
                // 街中ワープ後のユニちゃんの座標
                uniChan_.transform.position = warpChildren_[warpNum_].transform.position;
                // サブカメラに切り替わっていたら
                if (cameraMng_.mainCamera.activeSelf == false)
                {
                    cameraMng_.SetChangeCamera(false);// メインカメラに戻す

                    // 初期座標の差でカメラが追従しているため
                    cameraMng_.mainCamera.transform.position =
                         uniChan_.transform.position + starMainCameraPos_;
                }
            }
        }
        else
        {
            // 何もしない
        }
        fadePanel.color = new Color(1.0f, 1.0f, 1.0f, alphaCnt_);
    }

    private IEnumerator WarpCansel(bool flag)
    {
        // 移動Cancelの場合
        moveNeedle_ = false;             // 長針が動かないように
        changeCnt_ = 0.0f;               // ワープに入るためのSpaceキー処理が入るように
        while (flag)
        {
            if (frameImage_.transform.localPosition.x < saveHandlePos_.x
                  && saveHandlePos_.y < frameImage_.transform.localPosition.y)
            {
                handleSpeed_ += 1.0f * Time.deltaTime;
                frameImage_.transform.localPosition += new Vector3(handleSpeed_, -handleSpeed_, 0.0f);
                warpActive_.transform.localPosition += new Vector3(handleSpeed_, -handleSpeed_, 0.0f);
                //Debug.Log(maxHandlePos_ + "    <   " + frameImage_.transform.localPosition.x);
            }
            else
            {
                // スピードが変わるため初期化
                handleSpeed_ = 8.0f;
                handleRotate_ = 0.0f;
                frameImage_.transform.localPosition = saveHandlePos_;
                warpActive_.transform.localPosition = saveHandlePos_;
                flag = false;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name == "Town")
            {
                Debug.Log("ユニの家の前のコライダーに接触");
                SceneMng.SceneLoad(3);
                // ユニの家の中にワープ
            }
            else
            {
                // ユニの家から街に出る
                warpNum_ = (int)warp.HOUSE;
                //  UniChan.transform.position = warpChildren_[(int)warp.HOUSE].transform.localPosition;
                Debug.Log("ユニの家からタウンに出現" + uniChan_.transform.position);
                SceneMng.SceneLoad(0);
            }
        }
    }
}