using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpField : MonoBehaviour
{
    // 表示関連
    public Canvas locationSelCanvas;        // ワープ先を出すCanvas（親）

    // フィールドにワープする時
    private enum kindsField
    {
        NON = -1,     // -1
        TOWN,       // 0 町
        unitydata,  // 1 フィールド1
        FIELD_2,    // 2 フィールド2
        CANCEL,     // 3 選択キャンセル
        MAX         // 4
    }
    private GameObject[] warpObject_;   // マップ端のワープオブジェを保存

    // Canvas-Image,Text。それに連なる孫0Image 1Text;  
    private Transform[] canvasChild_ = new Transform[2];     // 選択できるフィールド(locationSelCanvasの子
    private Image[] selectFieldImage_ = new Image[(int)kindsField.MAX];      // 選択できるフィールドの背景（locationSelCanvasの孫
    private Text[] selectFieldText_ = new Text[(int)kindsField.MAX];        // 選択できるフィールドの文字（locationSelCanvasの孫

    // [現在のscene名を確認、表示するフィールド名]
    private string[,] sceneName_ = new string[2, (int)kindsField.MAX]{
            { "Town","ForestField","TestField","cansel" },
            { "Town","Field01","Field02","cansel" }
    };

    private float changeSelectCnt_ = 0.0f;  // 連続でSpaceキーの処理に入らないようにするため

    private int saveNowField_;    // 現在いるフィールドを保存
    private int selectFieldNum_;  // どのフィールドを選んでいるか（1スタート

    private Color nowImageColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f);  // 選択中の色（青）
    private Color resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);     // 選択外の色（白）

    private bool fieldEndHit = false;   // ワープオブジェクトに接触したかどうか
    private bool nowTownFlag_ = false;  // 町ワープからのフィールドワープか
    private bool warpNowFlag_ = false;  // フィールドワープを選択中の時

    // マップ端からフィールド選択→キャンセルした場合関連
    private enum rotate
    {
        UP,     // 0 上 315<=360&&0<45
        RIGHT,  // 1 右 45<=135
        DOWN,   // 2 下 135<=225
        LEFT,   // 3 左 225<=315
        MAX
    }
    private rotate nowRotate_;// 上下左右どの方向を向いているか
    // ユニが向いてる方向出すための範囲
    private int[] checkRot_ = new int[6] { 0, 45, 135, 225, 315, 360 };

    private GameObject UniChan;         // ユニ
    private Vector3 saveUniRot_ = new Vector3(0.0f, 0.0f, 0.0f);    // ユニが向いてる方向を保存
    private Vector3 enterPos_ = new Vector3(0.0f, 0.0f, 0.0f);      // 接触した瞬間の座標を保存
    private Vector3 addPos_ = new Vector3(0.0f, 0.0f, 0.0f);        // キャンセル時に反対方向に弾く

    // （ユニ座標ーワープオブジェ）を正規化
    private Vector3 uniNormalized_ = new Vector3(0.0f, 0.0f, 0.0f);
    private float rotateNormalized_ = 0.0f;// 向いてる方向の正規化を保存


    void Start()
    {
        // 座標と回転を変える可能性があるためユニを取得
        UniChan = GameObject.Find("Uni");

        // フィールド選択キャンバスを非表示
        locationSelCanvas.gameObject.SetActive(false);

        // 表示する背景と文字の親を入れておく
        canvasChild_[0] = locationSelCanvas.transform.GetChild(0).GetComponent<Transform>();
        canvasChild_[1] = locationSelCanvas.transform.GetChild(1).GetComponent<Transform>();

        if(SceneManager.GetActiveScene().name=="InHouseAndUniHouse")
        {
            sceneName_[0, 0] = "InHouseAndUniHouse";
        }

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            selectFieldImage_[i] = canvasChild_[0].transform.GetChild(i).GetComponent<Image>();
            selectFieldText_[i] = canvasChild_[1].transform.GetChild(i).GetComponent<Text>();
            selectFieldText_[i].text = sceneName_[1, i];// Textに文字を入れる

            if (SceneManager.GetActiveScene().name == sceneName_[0, i])
            {
                saveNowField_ = i;// 現在のシーン名と合っているものを保存
            }
        }
        // 保存＝現在いるシーン　選べない色にする
        selectFieldImage_[saveNowField_].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

        // 選択中の初期場所を決める
        if (saveNowField_ == (int)kindsField.TOWN)
        {
            selectFieldNum_ = (int)kindsField.unitydata;
        }
        else
        {
            selectFieldNum_ = (int)kindsField.TOWN;
        }

        // マップ端にあるオブジェクトを検索（フィールドによって個数が違うため子の個数で見る）
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "側のワープ座表" + warpObject_[i].transform.position);
        }
    }

    // コルーチン  
    private IEnumerator Select()
    {
        // コルーチンの処理(返り値がtrueなら処理を続行する)  
        while (SelectGoToFiled())
        {
            yield return null;
        }
    }

    private bool SelectGoToFiled()
    {
        // 選択中の画像の色を変更
        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            if (saveNowField_ == i)
            {
                continue;  // 現在いるSceneの色はStartで指定した色。変化しない
            }

            if (selectFieldNum_ == i)
            {
                selectFieldImage_[selectFieldNum_].color = nowImageColor_;  // 現在選択中の時（青
            }
            else
            {
                selectFieldImage_[i].color = resetColor_; // 選択されてない時（白
            }
        }

        // フィールド選択中にSpaceキー処理が入らないようにするため
        if (changeSelectCnt_ < 0.5f)
        {
            changeSelectCnt_ += Time.deltaTime;
            return true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectFieldNum_ < (int)kindsField.CANCEL)
            {
                selectFieldNum_++;      // 下に移動
            }
            if (selectFieldNum_ == saveNowField_)
            {
                selectFieldNum_++;// 現在シーンの場合はもう一度加算
            }
            return true;
            //Debug.Log("下に移動" + selectFieldNum_);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (saveNowField_ != (int)kindsField.TOWN)
            {
                if ((int)kindsField.TOWN < selectFieldNum_)
                {
                    selectFieldNum_--;    // 上に移動
                }
                if (selectFieldNum_ == saveNowField_)
                {
                    selectFieldNum_--;
                }
            }
            else
            {
                // 町にいるときは町が一番上のため減算してほしくない
                if ((int)kindsField.TOWN + 1 < selectFieldNum_)
                {
                    selectFieldNum_--;    // 上に移動
                }
            }
            return true;
        }
        else
        {
            // 何も処理を行わない
        }

        // 行先決定
        if (Input.GetKey(KeyCode.Space))
        {
            // キャンセル以外の時＝シーン遷移をする
            if (selectFieldNum_ != (int)kindsField.CANCEL)
            {
                Debug.Log("コルーチンストップ");
                StopCoroutine(Select());                // コルーチンストップ
                //Debug.Log(selectFieldNum_+ "を選択中。Sceneを移動します");
                WarpTown.warpNum_ = 0;// フィールドからタウンに戻った時のために0に戻しておく
                SceneMng.SceneLoad(selectFieldNum_);
            }
            else
            {
                // フィールド端に接触した際はユニを回転させて押し返す必要がある
                if (fieldEndHit == true)
                {
                    UniPushBack();
                }

                // 選択中の位置を初期化
                if (saveNowField_ == (int)kindsField.TOWN)
                {
                    selectFieldNum_ = (int)kindsField.unitydata; // フィールドの行き先をリセット
                }
                else
                {
                    selectFieldNum_ = (int)kindsField.TOWN; // フィールドの行き先をリセット
                }

                // フィールド選択キャンバス非表示
                locationSelCanvas.gameObject.SetActive(false);
                changeSelectCnt_ = 0.0f;

                Debug.Log("コルーチンストップ");
                StopCoroutine(Select());                // コルーチンストップ
            }
            nowTownFlag_ = false;
            warpNowFlag_ = false;
        }
        return nowTownFlag_;
    }


    private void UniPushBack()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            uniNormalized_ = (enterPos_ - warpObject_[i].transform.position).normalized;
            //Debug.Log(warpObject_[i].name + "との正規化" + uniNormalized_);
            if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
            {
                rotateNormalized_ = uniNormalized_.z;
                addPos_ = new Vector3(0.0f, 0.0f, 0.5f);
                Debug.Log("上か下から接触");
            }
            else
            {
                rotateNormalized_ = uniNormalized_.x;
                addPos_ = new Vector3(-0.5f, 0.0f, 0.0f);
                Debug.Log("右か左から接触");
            }
        }

        if (rotateNormalized_ < 0.0f)
        {
            // 出ようとした方向の反対側に加算
            addPos_ = -addPos_;
        }
        // +180度で反対方向をむかせる
        UniChan.transform.rotation = Quaternion.Euler(0.0f, saveUniRot_.y + 180, 0.0f);
        UniChan.transform.position = enterPos_ + addPos_;
        fieldEndHit = false;

    }

    private void CheckUniTransfoem()
    {
        // ユニの座標と向いてる方向を保存
        enterPos_ = UniChan.transform.position;
        saveUniRot_ = UniChan.transform.localEulerAngles;

        if ((checkRot_[4] < saveUniRot_.y && saveUniRot_.y < checkRot_[5])
         || (checkRot_[0] <= saveUniRot_.y && saveUniRot_.y < checkRot_[1]))
        {
            nowRotate_ = rotate.UP;            // 上側
        }
        else
        {
            // 上側以外の方向の時
            for (int i = 1; i < (int)rotate.MAX; i++)
            {
                if (checkRot_[i] <= saveUniRot_.y && saveUniRot_.y < checkRot_[i + 1])
                {
                    nowRotate_ = (rotate)i;
                    Debug.Log(saveUniRot_.y + "   向いてる方向" + nowRotate_);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーがフィールド端に接触したかどうか
        if (other.CompareTag("Player"))
        {
            // フィールドの移動先を表示
            locationSelCanvas.gameObject.SetActive(true);
            fieldEndHit = true;
            warpNowFlag_ = true;
            CheckUniTransfoem();// ユニが向いてる方向を確定
            SetNowTownFlag(true);            // フィールド端に接触した時用のワープ
        }
    }

    // ワープ選択中かどうか
    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }

    // 街中でフィールドに行くためのキャンバスを表示するかどうか
    public bool GetLocationSelActive()
    {
        return locationSelCanvas.gameObject.activeSelf;
    }

    public void SetLocationSelActive(bool flag)
    {
        locationSelCanvas.gameObject.SetActive(flag);
    }

    public void SetNowTownFlag(bool flag)
    {
        // 街中でフィールドにワープする時
        // フィールド上でワープする時、Updataがないためここ経由で呼び出す必要がある
        nowTownFlag_ = flag;

        if (flag)
        {
            Debug.Log("コルーチンスタート");
            // コルーチンスタート
            StartCoroutine(Select());
        }
    }

}