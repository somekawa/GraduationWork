using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Warp : MonoBehaviour
{
    private enum warp
    {
        NON,
        HOUSE,      // ユニの家
        FOUNTAIN,   // 噴水前
        CROSS,      // 十字路
        FIELD,      // フィールド
        MAX
    }

    public CameraMng cameraMng;     // メインカメラとサブカメラの切り替え
    public GameObject UniChan;      // ユニちゃん
    public Image WarpActive;        // ワープ選択中か（白：選択中
    public Canvas fieldCanvas;      // フィールドに出たいときのキャンバス

    private float[] needleRotate;       // 長針の回転先を保存
    private bool moveNeedle_ = false;   // 長針が回転しても良い状態かチェック 
    private int warpNum = (int)warp.HOUSE; // 長針がどこを指しているか（1スタート

    private float changeCnt_ = 0.0f;    // 連続でSpaceキーの処理に入らないようにするため

    private Vector3 starMainCameraPos_; // ユニティちゃんとカメラの座標差を出すため初期座標を保存
    private UnitychanController UniCtl; // ユニちゃんの移動系処理のScript
    private GameObject[] warpChildren_; // 街中のワープ先
    private Image needleImage;               // 長針画像

    // フィールド選択時
    private enum field
    {
        NON,
        FIELD1, // フィールド1
        FIELD2, // フィールド2
        CANCEL, // キャンセル
        MAX
    }

    private int choiceNum_ = (int)field.FIELD1; // どのフィールドを選んでいるか（1スタート
    private Color choiceColor_;                 // 選択中の色（青）
    private Color resetColor_;                  // 選択外の色（白）
    private Image[] choiceField_;               // 選択できるフィールドの画像

    void Start()
    {
        // 長針画像取得
        needleImage = WarpActive.transform.Find("Needle").GetComponent<Image>();

        // UnitychanControllerScript取得
        UniCtl = UniChan.GetComponent<UnitychanController>();

        // メインカメラの初期画像を取得
        starMainCameraPos_ = cameraMng.mainCamera.transform.position;

        choiceColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f);// 青
        resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);// 白
        WarpActive.color = choiceColor_;// ワープしないとき（青

        // 長針の回転先
        needleRotate = new float[(int)warp.MAX] {
            0.0f,  32.0f, 12.0f,  -12.0f, -32.0f
        };

        // 長針の初期位置
        needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[(int)warp.HOUSE]);

        // フィールドに出たいときのキャンバスは非表示
        fieldCanvas.enabled = false;

        // 街中のワープ先を保存 フィールドを含まない
        warpChildren_ = new GameObject[(int)warp.MAX - 1];
        for (int i = (int)warp.NON; i < (int)warp.MAX - 1; i++)
        {
            warpChildren_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log("Warp先" + i + ";" + this.transform.GetChild(i).gameObject);
        }

        // 街の外の移動先を保存
        choiceField_ = new Image[(int)field.MAX];
        for (int i = (int)field.NON; i < (int)field.MAX; i++)
        {
            choiceField_[i] = fieldCanvas.transform.GetChild(i).GetComponent<Image>();
            //Debug.Log("町の外移動先" + i + ";" + fieldCanvas.transform.GetChild(i).GetComponent<Image>());
        }
    }

    void Update()
    {
        // フィールドキャンバスがアクティブの時
        if (fieldCanvas.enabled == true)
        {
            ChoiceFiledLocation();
            //Debug.Log("フィールド移動先表示中");
            return;
        }

        // 長針が動かない＝ワープ先を選べない
        if (moveNeedle_ == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // ワープ準備
                UniCtl.enabled = false;
                moveNeedle_ = true;
            }
        }
        else  
        {
            if (changeCnt_ < 0.5f)
            {
                // ワープ処理に入る時のSpaceキー処理が入らないようにするため
                changeCnt_ += Time.deltaTime;
                return;
            }
            // ワープ先選択処理
            ChoiceWarpLocation();
        }
    }

    private void ChoiceWarpLocation()
    {
        WarpActive.color = resetColor_;     // ワープ先を選べる（白

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (warpNum < (int)warp.MAX - 1)
            {
                warpNum++;            // 長針右に移動
            }
            //Debug.Log("右移動カウント" + warpNum);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if ((int)warp.NON + 1 < warpNum)
            {
                warpNum--;            // 長針左に移動
            }
            //Debug.Log("左移動カウント" + warpNum);
        }

        for (int i = (int)warp.HOUSE; i < (int)warp.MAX; i++)
        {
            if (warpNum == i)
            {
                // 長針の角度
                needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[i]);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (warpNum == (int)warp.FIELD)
            {
                fieldCanvas.enabled = true;         // フィールドの移動先を表示
                WarpActive.color = choiceColor_;    // ワープしないとき（青
                return;
            }
            else
            {
                // 街中ワープ後のユニちゃんの座標
                UniChan.transform.position = warpChildren_[warpNum].transform.position;

                // サブカメラに切り替わっていたら
                if (cameraMng.mainCamera.activeSelf == false)
                {
                    cameraMng.SetChangeCamera(false);// メインカメラに戻す
                    
                    // 初期座標の差でカメラが追従しているため
                    cameraMng.mainCamera.transform.position =
                         UniChan.transform.position + starMainCameraPos_;
                }
            }
            CommonWarpCansel();    // ワープ処理リセット
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CommonWarpCansel();            // ワープキャンセルの場合
        }
    }

    private void ChoiceFiledLocation()
    {
        //Debug.Log(choiceNum_);
        moveNeedle_ = false;// 長針を動かないようにする

        for (int i = (int)field.NON + 1; i < (int)field.MAX; i++)
        {
            if (choiceNum_ == i)
            {
                choiceField_[choiceNum_].color = choiceColor_;  // 選択中は青
            }
            else
            {
                choiceField_[i].color = resetColor_;            // それ以外は白
            }
        }

        // warp→field順のためwarpの時の2倍
        if (changeCnt_ < 1.0f)
        {
            // フィールド選択中にSpaceキー処理が入らないようにするため
            changeCnt_ += Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (choiceNum_ < (int)field.MAX - 1)
            {
                choiceNum_++;      // 下に移動
            }
            //Debug.Log("下に移動" + choiceNum_);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if ((int)field.NON + 1 < choiceNum_)
            {
                choiceNum_--;    // 上に移動
            }
            //Debug.Log("上に移動" + choiceNum_);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (choiceNum_ == (int)field.CANCEL)
            {
                CommonWarpCansel();             // ワープをキャンセル
                choiceNum_ = (int)field.FIELD1; // フィールドの行き先をリセット
                fieldCanvas.enabled = false;    // フィールド選択キャンバス非表示
            }
            else
            {
                SceneManager.LoadScene("unitydata");   // unitydataSceneへ移動
            }
        }
    }

    private void CommonWarpCansel()
    {
        // 移動Cancelの場合
        UniCtl.enabled = true;  // ユニちゃんを動かせるように
        moveNeedle_ = false;    // 長針が動かないように
        changeCnt_ = 0.0f;      // ワープに入るためのSpaceキー処理が入るように
        WarpActive.color = choiceColor_;    // ワープ先を選べない状態（青
    }
}
