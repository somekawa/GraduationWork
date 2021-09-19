using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpTown : MonoBehaviour
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
    public Canvas LocationSelCanvas;      // フィールドに出たいときのキャンバス
    public Canvas warpCanvas;

    private bool decisionFlag_ = false;// ワープ先が決まったかどうか
    private float alphaCnt_ = 0.0f;// panelのアルファ値
    private bool warpCheck_ = false;// ワープを選んでいる状態かどうか
    private float[] needleRotate;       // 長針の回転先を保存
    private bool moveNeedle_ = false;   // 長針が回転しても良い状態かチェック 
    private int warpNum_ = (int)warp.HOUSE; // 長針がどこを指しているか（1スタート

    private float changeCnt_ = 0.0f;    // 連続でSpaceキーの処理に入らないようにするため

    private float saveUniRotateY_ = 0.0f;

    private Vector3 starMainCameraPos_; // ユニティちゃんとカメラの座標差を出すため初期座標を保存
    private GameObject[] warpChildren_; // 街中のワープ先
    private Image needleImage;               // 長針画像
    private Image warpActive;        // ワープ選択中か（白：選択中
    private Image fadePanel;


    // フィールド選択時
    private enum field
    {
        NON,
        FIELD1, // フィールド1
        FIELD2, // フィールド2
        CANCEL, // キャンセル
        MAX
    }

    private bool warpBoxHit_ = false;// 町外れでフィールドに出ようとしてるかどうか
    private int choiceNum_ = (int)field.FIELD1; // どのフィールドを選んでいるか（1スタート
    private Color choiceColor_;                 // 選択中の色（青）
    private Color resetColor_;                  // 選択外の色（白）
    private Image[] choiceField_;               // 選択できるフィールドの画像


    // 押下状態を確認したいキーをまとめたもの
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    void Start()
    {       
        warpActive = warpCanvas.transform.GetChild(0).GetComponent<Image>();

        fadePanel = warpCanvas.transform.GetChild(1).GetComponent<Image>();

        // 長針画像取得
        needleImage = warpActive.transform.Find("Needle").GetComponent<Image>();

        // メインカメラの初期画像を取得
        starMainCameraPos_ = cameraMng.mainCamera.transform.position;

        choiceColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f);// 青
        resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);// 白
        warpActive.color = choiceColor_;// ワープしないとき（青

        // 長針の回転先
        needleRotate = new float[(int)warp.MAX] {
            0.0f,  32.0f, 12.0f,  -12.0f, -32.0f
        };

        // 長針の初期位置
        needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[(int)warp.HOUSE]);

        // フィールドに出たいときのキャンバスは非表示
        LocationSelCanvas.enabled = false;

        // 街中のワープ先を保存 フィールドを含まない
        warpChildren_ = new GameObject[(int)warp.MAX - 1];
        for (int i = (int)warp.NON; i < (int)warp.MAX - 1; i++)
        {
            warpChildren_[i] = this.transform.GetChild(i).gameObject;
            Debug.Log("Warp先" + i + ";" + this.transform.GetChild(i).gameObject);
        }

        // 街の外の移動先を保存
        choiceField_ = new Image[(int)field.MAX];
        for (int i = (int)field.NON; i < (int)field.MAX; i++)
        {
            choiceField_[i] = LocationSelCanvas.transform.GetChild(i).GetComponent<Image>();
            Debug.Log("町の外移動先" + i + ";" + LocationSelCanvas.transform.GetChild(i).GetComponent<Image>());
        }
    }

    void Update()
    {
        Debug.Log("選択しているワープ先"+warpNum_);
        if (decisionFlag_ == true)
        {
            FadeOutAndIn();
        }

        // フィールドキャンバスがアクティブの時
        if (LocationSelCanvas.enabled == true)
        {
            ChoiceFiledLocation();
            Debug.Log("フィールド移動先表示中");
            return;
        }

        // 長針が動かない＝ワープ先を選べない
        if (moveNeedle_ == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // ワープ準備
                moveNeedle_ = true;
                warpCheck_ = true;
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
            ChoiceWarpLocation();            // ワープ先選択処理
        }
    }

    private void ChoiceWarpLocation()
    {
        warpActive.color = resetColor_;     // ワープ先を選べる（白

         CommonSelMove(warpNum_, 3, 2, (int)warp.FIELD, (int)warp.HOUSE);

        for (int i = (int)warp.HOUSE; i < (int)warp.MAX; i++)
        {
            if (warpNum_ == i)
            {
                // 長針の角度
                needleImage.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate[i]);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (warpNum_ == (int)warp.FIELD)
            {
                LocationSelCanvas.enabled = true;         // フィールドの移動先を表示
                warpActive.color = choiceColor_;    // ワープしないとき（青
                return;
            }
            else
            {
                decisionFlag_ = true;
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
        Debug.Log(choiceNum_);
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

        CommonSelMove(choiceNum_, 1, 0, (int)field.CANCEL, (int)field.FIELD1);

        if (Input.GetKey(KeyCode.Space))
        {
            // キャンセル選択時
            if (choiceNum_ == (int)field.CANCEL)
            {
                if (warpBoxHit_ == true)
                {
                    if (UniChan.transform.position.x < -1.0f)
                    {
                        // 左から出ようとしていたら
                        UniChan.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                        UniChan.transform.position = new Vector3(
                             UniChan.transform.position.x + 0.5f,
                             UniChan.transform.position.y,
                             UniChan.transform.position.z);
                    }
                    else
                    {
                        // 右から出ようとしていたら
                        UniChan.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                        UniChan.transform.position = new Vector3(
                                UniChan.transform.position.x - 0.5f,
                                UniChan.transform.position.y,
                                UniChan.transform.position.z);
                    }
                    warpBoxHit_ = false;
                    warpCheck_ = false;
                }
                CommonWarpCansel();             // ワープをキャンセル
                choiceNum_ = (int)field.FIELD1; // フィールドの行き先をリセット
                LocationSelCanvas.enabled = false;    // フィールド選択キャンバス非表示
            }
            else
            {
                decisionFlag_ = true;
            }
        }
    }


    public void CommonSelMove(int changeNum, int numUpKey, int numDownKey,int maxNum,int minNum)
    {
        if (Input.GetKeyDown(keyArray_[numUpKey]))
        {
            if (changeNum < maxNum)
            {
                changeNum++;      // 下に移動
            }
            Debug.Log("移動" + changeNum);
        }
        else if (Input.GetKeyDown(keyArray_[numDownKey]))
        {
            if (minNum < changeNum)
            {
                changeNum--;    // 上に移動
            }
            Debug.Log("移動" + changeNum);
        }

        if(moveNeedle_==true)
        {
            // ここないと加算減算されない……
            warpNum_ = changeNum;
        }
        else
        {
            choiceNum_ = changeNum;
        }
    }

    public bool GetWarpFlag()
    {
        return warpCheck_;
    }

    private void FadeOutAndIn()
    {
        if (LocationSelCanvas.enabled == true)
        {
            // 町から出る場合
            if (alphaCnt_ <= 0.5f)
            {
                alphaCnt_ += Time.deltaTime * 2;
            }
            else
            {
                alphaCnt_ = 0.0f;
                decisionFlag_ = false;
                SceneMng.SceneLoad((int)SceneMng.SCENE.FIELD);
            }
        }
        else
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
        Debug.Log(alphaCnt_);
    }

    private void CommonWarpCansel()
    {
        // 移動Cancelの場合
        moveNeedle_ = false;    // 長針が動かないように
        warpCheck_ = false;
        changeCnt_ = 0.0f;      // ワープに入るためのSpaceキー処理が入るように
        warpActive.color = choiceColor_;    // ワープ先を選べない状態（青
    }


    private void OnTriggerStay(Collider other)
    {
        if (warpBoxHit_ == false)
        {
            if (other.tag == "Player")
            {
                //  UniCtl.enabled = false;// ユニの動きを止める
                LocationSelCanvas.enabled = true;         // フィールドの移動先を表示
                warpBoxHit_ = true;
                warpCheck_ = true;
                saveUniRotateY_ = UniChan.transform.rotation.y;
                Debug.Log("WarpBoxHit");
                ChoiceFiledLocation();
            }
        }
    }
}