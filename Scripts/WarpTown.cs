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
    public Canvas warpCanvas;       // 街中のワープ先を表示するキャンバス

    private GameObject warpInTown_;         // 街中ワープ先を持つ親オブジェクト
    private GameObject[] warpChildren_;     // 街中のワープ先
    private float changeCnt_ = 0.0f;        // 連続でSpaceキーの処理に入らないようにするため
    private bool decisionFlag_ = false;     // ワープ先が決まったかどうか

    // ワープ選択画像背景
    private Image warpActive;   // warpCanvasの子の背景画像を取得
    private Color selectColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f);     // 選択中の色（青）
    private Color resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);      // 選択外の色（白）

    // 長針関連
    private Image needleImage;              // warpActive(背景の子)から長針画像を取得
    private float[] needleRotate = new float[(int)warp.MAX] {
            32.0f, 12.0f,  -12.0f, -32.0f
        };
    // 長針の回転先を保存
    private bool moveNeedle_ = false;       // 長針が回転しても良い状態かチェック 
    private int warpNum_ = (int)warp.HOUSE; // 長針がどこを指しているか（1スタート

    // 町から出るとき関連
    private GameObject warpOutObj_;     // warpField.csがアタッチされてるオブジェクト
    private WarpField warpFieldScript;  // warpField.cs

    // カメラ関連　座標関連
    private GameObject UniChan;         // ユニちゃん
    private GameObject cameraCtl_;      // CameraMng.csがアタッチされてるオブジェクト
    private CameraMng cameraMng;        // メインカメラとサブカメラの切り替え
    private Vector3 starMainCameraPos_; // ユニティちゃんとカメラの座標差を出すため初期座標を保存

    // フェードアウト関連
    private Image fadePanel;         // warpCanvasの子のフェードアウト用パネルを取得
    private float alphaCnt_ = 0.0f;  // panelのアルファ値


    void Start()
    {
        // ワープする座標を入れるため
        UniChan = GameObject.Find("Uni");

        // オブジェクト経由でWarpFieldScriptを取得
        warpOutObj_ = GameObject.Find("WarpOut");
        warpFieldScript = warpOutObj_.transform.GetComponent<WarpField>();

        // 0 ワープ先が子についている空のオブジェクト
        warpActive = warpCanvas.transform.GetChild(0).GetComponent<Image>();
        warpActive.color = selectColor_;                    // ワープしないとき（青                                                          
        // 1 フェードアウト用のImageのみ
        fadePanel = warpCanvas.transform.GetChild(1).GetComponent<Image>();

        // 長針画像取得
        needleImage = warpActive.transform.Find("Needle").GetComponent<Image>();
        // 長針の初期位置
        needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[(int)warp.HOUSE]);

        if (SceneManager.GetActiveScene().name == "towndata")
        {
            // オブジェクト経由でCameraMngScriptを取得
            cameraCtl_ = GameObject.Find("CameraController");
            cameraMng = cameraCtl_.transform.GetComponent<CameraMng>();
            // メインカメラの初期画像を取得
            starMainCameraPos_ = cameraMng.mainCamera.transform.position;
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
    }

    void Update()
    {
        if (decisionFlag_ == true)
        {
            FadeOutAndIn();
        }

        // フィールドキャンバスがアクティブの時
        if (warpFieldScript.GetLocationSelActive() == true)
        {
            return;   // 町中のワープは選択できないように
        }
        else
        {
            // 長針が動かない＝ワープ先を選べない
            if (moveNeedle_ == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // ワープ準備
                    moveNeedle_ = true;
                    warpFieldScript.SetWarpNowFlag(true);
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
        warpActive.color = resetColor_;     // ワープ先を選べる（白

        // 選択先を変更
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CommonWarpCansel();            // ワープキャンセルの場合
            warpFieldScript.SetWarpNowFlag(false);
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
                needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[i]);
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
                warpFieldScript.SetWarpNowFlag(false);
            }
            else
            {                // フィールド選択時
                warpFieldScript.SetLocationSelActive(true);         // フィールドの移動先を表示
                warpActive.color = selectColor_;    // フィールド選択中に選べないように
                warpFieldScript.SetNowTownFlag(true);// 町からのワープだとFlagをtrueにする
            }
            CommonWarpCansel();    // ワープ処理リセット
        }
    }

    private void FadeOutAndIn()
    {
        // 町中をワープする場合
        if (alphaCnt_ < 1.0f)
        {
            UniChan.SetActive(false);
            alphaCnt_ += Time.deltaTime * 2;
        }
        else if (1.0f < alphaCnt_)
        {
            UniChan.SetActive(true);
            alphaCnt_ = 0.0f;
            decisionFlag_ = false;

            // 街中ワープ後のユニちゃんの座標
            UniChan.transform.position = warpChildren_[warpNum_].transform.position;

            if (SceneManager.GetActiveScene().name == "towndata")
            {
                // サブカメラに切り替わっていたら
                if (cameraMng.mainCamera.activeSelf == false)
                {
                    cameraMng.SetChangeCamera(false);// メインカメラに戻す

                    // 初期座標の差でカメラが追従しているため
                    cameraMng.mainCamera.transform.position =
                         UniChan.transform.position + starMainCameraPos_;
                }
            }
        }
        fadePanel.color = new Color(1.0f, 1.0f, 1.0f, alphaCnt_);
        //  Debug.Log(alphaCnt_);
    }

    private void CommonWarpCansel()
    {
        // 移動Cancelの場合
        moveNeedle_ = false;             // 長針が動かないように
        changeCnt_ = 0.0f;               // ワープに入るためのSpaceキー処理が入るように
        warpActive.color = selectColor_; // ワープ先を選べない状態（青
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ユニの家の前のコライダーに接触");
            // ユニの家の中にワープ
            UniChan.transform.position = new Vector3(0.0f, 0.0f, -7.0f);
            UniChan.transform.rotation = Quaternion.Euler(0.0f, 180, 0.0f);
        }
    }
}