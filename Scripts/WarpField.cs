using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpField : MonoBehaviour
{
    enum rotate
    {
        UP,// 上
        DOWN,// 下
        LEFT,// 左
        RIGHT,// 右
        MAX
    }
    private rotate nowRotate_;// 上下左右度の方向を向いているか
    private float nowUniRotate_;// 向いてる方向の正規化を保存
    // up315~360 0~45 down135~225 left225~315 right15~135
    private int[] checkRot_ = new int[6] { 0, 45, 135, 225, 315, 360 };// ユニが向いてる方向出すための範囲
    private Vector3 saveUniRot_;// ユニが向いてる方向を保存

    // フィールド上でワープ先選択時
    enum kindsField
    {
        //  NON,
        TOWN,       // 町
        unitydata,  // フィールド1
        FIELD_2,    // フィールド2
        CANCEL,     // 選択キャンセル
        MAX
    }

    // 表示関連
    public Canvas locationSelCanvas;        // ワープ先を出すCanvas（親）

    private Transform[] canvasChild_;       // 選択できるフィールド(locationSelCanvasの子
    private Image[] selectFieldImage_ = new Image[(int)kindsField.MAX];      // 選択できるフィールドの背景（locationSelCanvasの孫
    private Text[] selectFieldText_ = new Text[(int)kindsField.MAX];        // 選択できるフィールドの文字（locationSelCanvasの孫
    private string[] fieldShowName_;            // 選択できるフィールドの名前(Textに代入
    private string[] sceneName_;            // 現在のscene名を確認

    private float changeSelectCnt_ = 0.0f;  // 連続でSpaceキーの処理に入らないようにするため
    private bool fieldEndHit = false;       // ワープオブジェクトに接触したかどうか

    private int saveNowField_ ;    // 現在いるフィールドを保存
    private int selectFieldNum_; // どのフィールドを選んでいるか（1スタート

    private Color nowImageColor_;               // 選択中の色（青）
    private Color resetColor_;                  // 選択外の色（白）

    // ワープオブジェクト関連
    private GameObject UniChan;         // ユニちゃん
    private GameObject[] warpObject_;   // ワープオブジェを保存
    private Vector3[] saveObjPos_;

    private int maxWarpObjNum_ = 0;     // ワープオブジェクトの最大個数
    private Vector3 uniNormalized_;     // （ユニ座標ーワープオブジェ）を正規化
    private Vector3 enterPos_;          // 当たり判定内に入った瞬間の座標
    private Vector3 addPos_ = new Vector3(0.0f, 0.0f, 0.0f);       // キャンセルを押した際に反対方向にはじく

    private bool nowTownFlag_ = false;
    private bool warpNowFlag_ = false;

    void Start()
    {
        // 座標と回転を変える可能性があるためユニを取得
        UniChan = GameObject.Find("Uni");

        nowImageColor_ = new Color(0.0f, 0.0f, 1.0f, 1.0f); // 青
        resetColor_ = new Color(1.0f, 1.0f, 1.0f, 1.0f);    // 白
        locationSelCanvas.enabled = false;// フィールド選択キャンバスを非表示

        sceneName_ = new string[(int)kindsField.MAX] {
            //"non",
            "towndata","unitydata","TestField","cansel"
        };
        fieldShowName_ = new string[(int)kindsField.MAX] {
            //"non",
            "Town","Field01","Field02","cansel"
        };

        // 表示する背景と文字の親を入れておく
        canvasChild_ = new Transform[2];// 0Image 1Text
        canvasChild_[0] = locationSelCanvas.transform.GetChild(0).GetComponent<Transform>();
        canvasChild_[1] = locationSelCanvas.transform.GetChild(1).GetComponent<Transform>();

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {

            selectFieldImage_[i] = canvasChild_[0].transform.GetChild(i).GetComponent<Image>();
            selectFieldText_[i] = canvasChild_[1].transform.GetChild(i).GetComponent<Text>();

            selectFieldText_[i].text = fieldShowName_[i];// Textに文字を入れる


            //Debug.Log("fieldName_：" + sceneName_[i]);
            //Debug.Log("フィールドの外移動先" + i + ";" + canvasChild_[0].transform.GetChild(i).GetComponent<Image>());

            if (SceneManager.GetActiveScene().name == sceneName_[i])
            {
                saveNowField_ = i;
            }
        }
        selectFieldImage_[saveNowField_].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        if(saveNowField_==(int)kindsField.TOWN)
        {
            selectFieldNum_ = (int)kindsField.unitydata;
        }
        else
        {
            selectFieldNum_ = (int)kindsField.TOWN;
        }


        //Debug.Log("SceneName：" + SceneManager.GetActiveScene().name);



        maxWarpObjNum_ = this.transform.childCount;
        warpObject_ = new GameObject[maxWarpObjNum_];
        saveObjPos_ = new Vector3[maxWarpObjNum_];
        for (int i = 0; i < maxWarpObjNum_; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            saveObjPos_[i] = warpObject_[i].transform.position;
            //Debug.Log(warpObject_[i].name + "側のワープ座表" + warpObject_[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(nowTownFlag_==true)
        {
            SelectGoToFiled();
            return;
        }

        //  Debug.Log("選択中のフィールド" + selectFieldNum_);
        if (fieldEndHit == true)
        {
            SelectGoToFiled();
        }
    }

    private void TownFieldSelect()
    {

    }

    private void SelectGoToFiled()
    {
        // Debug.Log(choiceFieldNum_);

        for (int i = (int)kindsField.TOWN; i < (int)kindsField.MAX; i++)
        {
            if (saveNowField_ == i)
            {
                continue;
            }

            if (selectFieldNum_ == i)
            {
                selectFieldImage_[selectFieldNum_].color = nowImageColor_;  // 選択中は青
            }
            else
            {
                selectFieldImage_[i].color = resetColor_;            // それ以外は白
            }
        }

        // warp→field順のためwarpの時の2倍
        if (changeSelectCnt_ < 0.5f)
        {
            // フィールド選択中にSpaceキー処理が入らないようにするため
            changeSelectCnt_ += Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selectFieldNum_ < (int)kindsField.CANCEL)
            {
                selectFieldNum_++;      // 下に移動
            }
            if (selectFieldNum_ == saveNowField_)
            {
                selectFieldNum_++;
            }

            Debug.Log("下に移動" + selectFieldNum_);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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

        if (Input.GetKey(KeyCode.Space))
        {
            // キャンセル選択時
            if (selectFieldNum_ == (int)kindsField.CANCEL)
            {
                // フィールド端に接触した際
                if (fieldEndHit == true)
                {
                    RotateCheck();
                    for (int i = 0; i < maxWarpObjNum_; i++)
                    {
                        uniNormalized_ = (enterPos_ - warpObject_[i].transform.position).normalized;
                        //Debug.Log(warpObject_[i].name + "との正規化" + uniNormalized_);
                        if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
                        {
                            nowUniRotate_ = uniNormalized_.z;
                            addPos_ = new Vector3(0.0f, 0.0f, 0.5f);
                        }
                        else
                        {
                            nowUniRotate_ = uniNormalized_.x;
                            addPos_ = new Vector3(-0.5f, 0.0f, 0.0f);
                        }
                    }
                    if (nowUniRotate_ < 0.0f)
                    {
                        // 出ようとした方向の反対側に加算
                        addPos_ = -addPos_;
                    }
                    UniChan.transform.rotation = Quaternion.Euler(0.0f, UniChan.transform.rotation.y*180, 0.0f);
                    UniChan.transform.position = new Vector3(
                                enterPos_.x + addPos_.x,
                                enterPos_.y,
                                enterPos_.z + addPos_.z);

                    fieldEndHit = false;
                }

                if (saveNowField_ == (int)kindsField.TOWN)
                {
                    selectFieldNum_ = (int)kindsField.unitydata; // フィールドの行き先をリセット
                }
                else
                {
                    selectFieldNum_ = (int)kindsField.TOWN; // フィールドの行き先をリセット
                }
                locationSelCanvas.enabled = false;    // フィールド選択キャンバス非表示
                changeSelectCnt_ = 0.0f;
            }
            else
            {
                Debug.Log(selectFieldNum_+ "を選択中。Sceneを移動します");
                //  SceneMng.SceneLoadUnLoad(selectFieldNum_, saveNowField_);
            }
            nowTownFlag_ = false;
            warpNowFlag_ = false;
        }
    }

    private void RotateCheck()
    {
        if (checkRot_[1] <= saveUniRot_.y && saveUniRot_.y < checkRot_[2])
        {
            // 右側
            nowRotate_ = rotate.RIGHT;
        }
        else if (checkRot_[2] <= saveUniRot_.y && saveUniRot_.y < checkRot_[3])
        {
            // 下側
            nowRotate_ = rotate.DOWN;
        }
        else if (checkRot_[3] <= saveUniRot_.y && saveUniRot_.y < checkRot_[4])
        {
            // 左側
            nowRotate_ = rotate.LEFT;
        }
        else if ((checkRot_[4] < saveUniRot_.y && saveUniRot_.y < checkRot_[5])
            || (checkRot_[0] <= saveUniRot_.y && saveUniRot_.y < checkRot_[1]))
        {
            // 上側
            nowRotate_ = rotate.UP;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fieldEndHit == false)
        {
            if (other.CompareTag("Player"))
            {
                locationSelCanvas.enabled = true;         // フィールドの移動先を表示
                fieldEndHit = true;
                enterPos_ = UniChan.transform.position;// 端と接触した時、ユニの座標を保存
                saveUniRot_ = UniChan.transform.localEulerAngles;


                warpNowFlag_ = true;
            }
        }
    }

    //public void SetWarpFieldFlag(bool flag)
    //{
    //    fieldEndHit = flag;
    //}

    //public bool GetWarpFieldFlag()
    //{
    //    return fieldEndHit;
    //}

    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }


    // 街中でフィールドにワープする時用
    public void SetNowTownFlag(bool flag)
    {
        nowTownFlag_ = flag;
    }


    public bool GetLocationSelActive()
    {
        return locationSelCanvas.enabled;
    }

    public void SetLocationSelActive(bool flag)
    {
        locationSelCanvas.enabled = flag;
    }
}