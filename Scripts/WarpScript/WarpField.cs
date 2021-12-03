using UnityEngine;
using UnityEngine.UI;

public class WarpField : MonoBehaviour
{
    // 表示関連
    private Canvas dontDestroyCanvas_;
    private RectTransform locationSelMng_;  // ワープ先を出すCanvas（親）
    public static Image[] btnMng_;          // 生成したものを保存するオブジェクト
    private Text[] sceneText_;              // 生成されたものの中からTextを保存
    private RectTransform btnParent_;       // ボタンの親にあたるオブジェクト

    private string[] sceneName = new string[(int)SceneMng.SCENE.MAX] {
    "","town","house","field0","field1","field2","field3","field4","cancel"};
    private int storyProgress_ = (int)SceneMng.SCENE.FIELD2;// どの章まで進んでいるか

    private bool fieldEndHit = false;   // ワープオブジェクトに接触したかどうか
    private bool nowTownFlag_ = false;  // 町ワープからのフィールドワープか
    private bool warpNowFlag_ = false;  // フィールドワープを選択中の時

    private GameObject[] warpObject_;   // マップ端のワープオブジェを保存

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

        btnMng_ = new Image[(int)SceneMng.SCENE.MAX];
        sceneText_ = new Text[(int)SceneMng.SCENE.MAX];

        dontDestroyCanvas_ = GameObject.Find("DontDestroyCanvas").GetComponent<Canvas>();
        GameObject game = DontDestroyMng.Instance;
        locationSelMng_ = dontDestroyCanvas_.transform.Find("LocationSelMng").GetComponent<RectTransform>();
       
       btnParent_ = locationSelMng_.transform.Find("Viewport/Content").GetComponent<RectTransform>();
        for (int i = (int)SceneMng.SCENE.CONVERSATION; i < (int)SceneMng.SCENE.MAX; i++)
        {
            btnMng_[i] = btnParent_.transform.GetChild(i).GetComponent<Image>();
            sceneText_[i] = btnMng_[i].transform.GetChild(0).GetComponent<Text>();
            sceneText_[i].text = sceneName[i];
            btnMng_[i].name = sceneName[i];
            // 現在ストーリ以降のフィールドと現在いるシーンは非表示
            if (((int)SceneMng.nowScene == i) || (storyProgress_ < i))
            {
                btnMng_[i].gameObject.SetActive(false);
            }
            else
            {
                btnMng_[i].gameObject.SetActive(true);
            }
        }
        btnMng_[(int)SceneMng.SCENE.CONVERSATION].gameObject.SetActive(false);// 0番目はずっと非表示
        btnMng_[(int)SceneMng.SCENE.CANCEL].gameObject.SetActive(true);// キャンセルはずっと必要はずっと非表示

        // マップ端にあるオブジェクトを検索（フィールドによって個数が違うため子の個数で見る）
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "側のワープ座表" + warpObject_[i].transform.position);
        }
        // フィールド選択キャンバスを非表示
        locationSelMng_.gameObject.SetActive(false);
    }

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

    public void CancelCheck()
    {
        if (fieldEndHit == true)
        {
            UniPushBack();
        }
        else
        {
            SetNowTownFlag(false);
        }
        UniChanController_.enabled = true;
        warpNowFlag_ = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーがフィールド端に接触したかどうか
        if (other.CompareTag("Player"))
        {
            // フィールドの移動先を表示
            locationSelMng_.gameObject.SetActive(true);
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
        if(flag==false)
        {
            return;
        }
        // ワープ選択中はユニちゃんのアニメーションを止めて、移動操作不可
        UniChanController_.StopUniRunAnim();
        UniChanController_.enabled = false;
    }

    public bool GetWarpNowFlag()
    {
        return warpNowFlag_;
    }

    // 街中でフィールドに行くためのキャンバスを表示するかどうか
    public bool GetLocationSelActive()
    {
        return locationSelMng_.gameObject.activeSelf;
    }

    public void SetNowTownFlag(bool flag)
    {
        // 街中でフィールドにワープする時
        // フィールド上でワープする時、Updataがないためここ経由で呼び出す必要がある
        nowTownFlag_ = flag;
        locationSelMng_.gameObject.SetActive(flag);
    }
}