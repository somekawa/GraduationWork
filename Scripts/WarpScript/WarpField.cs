using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpField : MonoBehaviour
{
    private GameObject[] warpObject_;   // マップ端のワープオブジェを保存

    // 表示関連
    private Canvas locationSelCanvas_;        // ワープ先を出すCanvas（親）
    [SerializeField]
    private GameObject sceneBtnPrefab_; // 行先ボタンを表示するためのプレハブ                                        
    // 生成したものを保存するオブジェクト
    public static GameObject[] btnMng_ = new GameObject[(int)SceneMng.SCENE.MAX];
    // 生成されたものの中からTextを保存
    private Text[] sceneText_ = new Text[(int)SceneMng.SCENE.MAX];
    // ボタンの親にあたるオブジェクト
    private RectTransform btnParent_; 

    private string[] sceneName = new string[(int)SceneMng.SCENE.MAX] {
    "","town","house","field0","field1","field2","field3","field4","cancel"};
    private int stpryProgress_ = (int)SceneMng.SCENE.FIELD1;// どの章まで進んでいるか


    private int saveNowField_ = -1;    // 現在いるフィールドを保存
    private int selectFieldNum_;  // どのフィールドを選んでいるか（1スタート


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

    private GameObject UniChan_;         // ユニ
    private UnitychanController UniChanController_;                 // ユニのコントローラー情報
    private Vector3[] uniPositions_ = new Vector3[3]{
        new Vector3(0.0f, 0.0f, 0.0f),    // ユニが向いてる方向を保存
        new Vector3(0.0f, 0.0f, 0.0f),     // 接触した瞬間の座標を保存
        new Vector3(0.0f, 0.0f, 0.0f)       // キャンセル時に反対方向に弾く
    };

    // （ユニ座標ーワープオブジェ）を正規化
    private Vector3 uniNormalized_ = new Vector3(0.0f, 0.0f, 0.0f);
    private float rotateNormalized_ = 0.0f;// 向いてる方向の正規化を保存

    public void Init()
    {
        // 座標と回転を変える可能性があるためユニを取得
        UniChan_ = GameObject.Find("Uni");
        UniChanController_ = UniChan_.GetComponent<UnitychanController>();

        btnMng_ = new GameObject[(int)SceneMng.SCENE.MAX];
        sceneText_ = new Text[(int)SceneMng.SCENE.MAX];
        locationSelCanvas_ = GameObject.Find("LocationSelCanvas").GetComponent<Canvas>();
        btnParent_ = locationSelCanvas_.transform.Find("ScrollView/Viewport/Content").GetComponent<RectTransform>();
        Debug.Log(btnParent_.name);
        for (int i = (int)SceneMng.SCENE.CONVERSATION; i < (int)SceneMng.SCENE.MAX; i++)
        {
            btnMng_[i] = Instantiate(sceneBtnPrefab_, new Vector2(0, 0),
                Quaternion.identity, btnParent_.transform);
            sceneText_[i] = btnMng_[i].transform.GetChild(0).GetComponent<Text>();
            sceneText_[i].text = sceneName[i];
            btnMng_[i].name = sceneName[i];
            if (((int)SceneMng.nowScene == i)|| (stpryProgress_ <i))
            {
                btnMng_[i].SetActive(false);
            }
        }
        //// 現在ストーリ以降のフィールドは非表示
        //for (int i = stpryProgress_ + 1; i < (int)SceneMng.SCENE.CANCEL; i++)
        //{
        //    btnMng_[i].SetActive(false);
        //}
        btnMng_[(int)SceneMng.SCENE.CONVERSATION].SetActive(false);// 0番目はずっと非表示
        btnMng_[(int)SceneMng.SCENE.CANCEL].SetActive(true);// 0番目はずっと非表示

        // マップ端にあるオブジェクトを検索（フィールドによって個数が違うため子の個数で見る）
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "側のワープ座表" + warpObject_[i].transform.position);
        }
        // フィールド選択キャンバスを非表示
        locationSelCanvas_.gameObject.SetActive(false);
    }

    // コルーチン  
    //private IEnumerator Select()
    //{
    //    // コルーチンの処理(返り値がtrueなら処理を続行する)  
    //    while (SelectGoToFiled())
    //    {
    //        yield return null;
    //    }
    //}

    //private bool SelectGoToFiled()
    //{


    //    if (Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        if (selectFieldNum_ < (int)SceneMng.SCENE.MAX - 1)
    //        {
    //            selectFieldNum_++;      // 下に移動
    //        }
    //        //if (selectFieldNum_ == saveNowField_)
    //        //{
    //        //    selectFieldNum_++;// 現在シーンの場合はもう一度加算
    //        //}
    //        return true;
    //        //Debug.Log("下に移動" + selectFieldNum_);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        if (saveNowField_ != (int)SceneMng.SCENE.TOWN)
    //        {
    //            if ((int)SceneMng.SCENE.TOWN < selectFieldNum_)
    //            {
    //                selectFieldNum_--;    // 上に移動
    //            }
    //            //if (selectFieldNum_ == saveNowField_)
    //            //{
    //            //    selectFieldNum_--;
    //            //}
    //        }
    //        else
    //        {
    //            // 町にいるときは町が一番上のため減算してほしくない
    //            if ((int)SceneMng.SCENE.TOWN + 1 < selectFieldNum_)
    //            {
    //                selectFieldNum_--;    // 上に移動
    //            }
    //        }
    //        return true;
    //    }
    //    else
    //    {
    //        // 何も処理を行わない
    //    }

    //    // 行先決定
    //    if (Input.GetKey(KeyCode.Space))
    //    {
    //        // キャンセル以外の時＝シーン遷移をする
    //        if (selectFieldNum_ != (int)SceneMng.SCENE.MAX - 1)
    //        {
    //            Debug.Log("コルーチンストップ");
    //            StopCoroutine(Select());                // コルーチンストップ
    //            //Debug.Log(selectFieldNum_+ "を選択中。Sceneを移動します");
    //            WarpTown.warpNum_ = 0;// フィールドからタウンに戻った時のために0に戻しておく
    //            SceneMng.SceneLoad(selectFieldNum_ + 1);
    //        }
    //        else
    //        {
    //            // フィールド端に接触した際はユニを回転させて押し返す必要がある
    //            if (fieldEndHit == true)
    //            {
    //                UniPushBack();
    //            }

    //            //// 選択中の位置を初期化
    //            //if (saveNowField_ == (int)SceneMng.SCENE.TOWN)
    //            //{
    //            //    selectFieldNum_ = (int)SceneMng.SCENE.FIELD; // フィールドの行き先をリセット
    //            //}
    //            //else
    //            //{
    //            //    selectFieldNum_ = (int)SceneMng.SCENE.TOWN; // フィールドの行き先をリセット
    //            //}

    //            // フィールド選択キャンバス非表示
    //            locationSelCanvas_.gameObject.SetActive(false);

    //            Debug.Log("コルーチンストップ");
    //            StopCoroutine(Select());                // コルーチンストップ
    //        }
    //        nowTownFlag_ = false;
    //        SetWarpNowFlag(false);
    //    }
    //    return nowTownFlag_;
    //}

    private void UniPushBack()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            uniNormalized_ = (uniPositions_[1] - warpObject_[i].transform.position).normalized;
            //Debug.Log(warpObject_[i].name + "との正規化" + uniNormalized_);
            if (nowRotate_ == rotate.UP || nowRotate_ == rotate.DOWN)
            {
                rotateNormalized_ = uniNormalized_.z;
                uniPositions_[2] = new Vector3(0.0f, 0.0f, 0.5f);
                Debug.Log("上か下から接触");
            }
            else
            {
                rotateNormalized_ = uniNormalized_.x;
                uniPositions_[2] = new Vector3(-0.5f, 0.0f, 0.0f);
                Debug.Log("右か左から接触");
            }
        }

        if (rotateNormalized_ < 0.0f)
        {
            // 出ようとした方向の反対側に加算
            uniPositions_[2] = -uniPositions_[2];
        }
        // +180度で反対方向をむかせる
        UniChan_.transform.rotation = Quaternion.Euler(0.0f, uniPositions_[0].y + 180, 0.0f);
        UniChan_.transform.position = uniPositions_[1] + uniPositions_[2];
        fieldEndHit = false;
    }

    private void CheckUniTransfoem()
    {
        // ユニの座標と向いてる方向を保存
        uniPositions_[1] = UniChan_.transform.position;
        uniPositions_[0] = UniChan_.transform.localEulerAngles;

        if ((checkRot_[4] < uniPositions_[0].y && uniPositions_[0].y < checkRot_[5])
         || (checkRot_[0] <= uniPositions_[0].y && uniPositions_[0].y < checkRot_[1]))
        {
            nowRotate_ = rotate.UP;            // 上側
        }
        else
        {
            // 上側以外の方向の時
            for (int i = 1; i < (int)rotate.MAX; i++)
            {
                if (checkRot_[i] <= uniPositions_[0].y && uniPositions_[0].y < checkRot_[i + 1])
                {
                    nowRotate_ = (rotate)i;
                    Debug.Log(uniPositions_[0].y + "   向いてる方向" + nowRotate_);
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
            locationSelCanvas_.gameObject.SetActive(true);
            fieldEndHit = true;
            SetWarpNowFlag(true);
            CheckUniTransfoem();// ユニが向いてる方向を確定
            SetNowTownFlag(true);            // フィールド端に接触した時用のワープ
        }
    }

    // ワープ選択中かどうか
    // ユニちゃんのアニメーションを止められるように、あえて同じスクリプト内でもここからフラグを変更するようにしている
    public void SetWarpNowFlag(bool flag)
    {
        warpNowFlag_ = flag;

        if (warpNowFlag_)
        {
            // ワープ選択中はユニちゃんのアニメーションを止めて、移動操作不可
            UniChanController_.StopUniRunAnim();
            UniChanController_.enabled = false;
        }
        else
        {
            // 移動操作可能
            UniChanController_.enabled = true;
        }
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }

    // 街中でフィールドに行くためのキャンバスを表示するかどうか
    public bool GetLocationSelActive()
    {
        return locationSelCanvas_.gameObject.activeSelf;
    }

    public void SetLocationSelActive(bool flag)
    {
        locationSelCanvas_.gameObject.SetActive(flag);
    }

    public void SetNowTownFlag(bool flag)
    {
        // 街中でフィールドにワープする時
        // フィールド上でワープする時、Updataがないためここ経由で呼び出す必要がある
        nowTownFlag_ = flag;

        //if (flag)
        //{
        //    Debug.Log("コルーチンスタート");
        //    // コルーチンスタート
        //    StartCoroutine(Select());
        //}
    }

    public void CancelCheck()
    {
        if (fieldEndHit == true)
        {
            UniPushBack();
        }
    }

}