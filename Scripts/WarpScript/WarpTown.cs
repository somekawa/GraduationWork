using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private GameObject[] warpChildren_;     // 街中のワープ先

    // ワープ選択関連
    private Image warpActive_;          // Mask画像。子に移動先と針がある
    private Image frameImage_;          // フレーム画像
    private Vector2[] framPositions_ = new Vector2[2];// 0初期値　1最大値
    private float[] speeds_ = new float[2] { 8.0f, 1.0f };// 0移動速度　1回転速度

    // 長針関連
    private Image needleImage_;              // warpActive(背景の子)から長針画像を取得
    private float[] needleRotate_ = new float[(int)warp.MAX]
                    {  // 長針の回転先を保存
                        32.0f, 12.0f,  -12.0f, -32.0f
                    };
    public static int warpNum_ = (int)warp.HOUSE; // 長針がどこを指しているか（0スタート

    // フェードアウト関連
    private Fade fade_;

    // 町から出るとき関連
    private WarpField warpFieldScript_;  // warpField.cs

    // カメラ関連　座標関連
    private GameObject uniChan_;         // ユニちゃん
    private CameraMng cameraMng_;        // メインカメラとサブカメラの切り替え

    // ユニの家の前にいる場合
    private Collider houseWarpColl_;// ユニの家に行く、出るための当たり判定
    private int count_ = 0;

    private static bool tutorialFlag_ = true;

    private MenuActive menuActive_;
   // public Canvas inHouseInfoCanvas_;

    // Start関数から名称を変更し、TownMng.csのStart関数で呼び出されるように修正
    public void Init()
    {
           //if(tutorialFlag_==true)
           //{
           //    return;
           //}
           menuActive_ = GameObject.Find("SceneMng").transform.GetComponent<MenuActive>();
        // ワープする座標を入れるため
        uniChan_ = GameObject.Find("Uni");

        // オブジェクト経由でWarpFieldScriptを取得
        warpFieldScript_ = GameObject.Find("WarpOut").transform.GetComponent<WarpField>();

        // Mask画像を取得
        warpActive_ = GameObject.Find("WarpCanvas").transform.Find("WarpImageMask").GetComponent<Image>();
        // ワープイメージのフレーム
        frameImage_ = GameObject.Find("WarpCanvas").transform.Find("FrameImage").GetComponent<Image>();
        framPositions_[0] = new Vector2(frameImage_.transform.localPosition.x, frameImage_.transform.localPosition.y);
        framPositions_[1] = new Vector2(framPositions_[0].x - 200, framPositions_[0].y + 200);
        //Debug.Log("初期値" + framPositions_[0] + "  Max座標" + framPositions_[1]);

        // 長針画像取得
        needleImage_ = warpActive_.transform.Find("Needle").GetComponent<Image>();
        // 長針の初期位置
        needleImage_.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate_[(int)warp.HOUSE]);

        // トランジション
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        // ユニの家から出る、入るための当たり判定を取得
        houseWarpColl_ = this.transform.Find("UniHouseCollider").GetComponent<Collider>();
        houseWarpColl_.isTrigger = false;

        // 街中のワープ先を保存 フィールドを含まない
        warpChildren_ = new GameObject[(int)warp.MAX - 1];
        for (int i = (int)warp.HOUSE; i < (int)warp.MAX - 1; i++)
        {
            // 親からワープ先の子どもを順に代入
            warpChildren_[i] = GameObject.Find("WarpInTown").transform.GetChild(i).gameObject;
        }

        // サブカメラ時にメインカメラに切り替えさせるために取得しておく
        if (SceneMng.nowScene == SceneMng.SCENE.TOWN)
        {
            cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        }

        // InHousesから戻ってきたときに針の場所を保存するため
        if (warpNum_ != (int)warp.HOUSE)
        {
            Debug.Log("別シーンからワープで戻ってきました");
            needleImage_.transform.rotation = Quaternion.Euler(0.0f, 0.0f, needleRotate_[warpNum_]);

            if (SceneMng.nowScene == SceneMng.SCENE.TOWN)
            {
                if (warpNum_ != (int)warp.FIELD)
                {
                    // 選択されているワープ座標に移動
                    uniChan_.transform.position = warpChildren_[warpNum_].transform.position;
                }
                else
                {
                    uniChan_.transform.position = warpChildren_[(int)warp.HOUSE].transform.position;
                }
            }
        }
    }

    void Update()
    {
        //if (tutorialFlag_ == true)
        //{
        //    return;
        //}

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

        if (menuActive_.GetActiveFlag() == true)
         //   || inHouseInfoCanvas_.gameObject.activeSelf==true)
        {
            return;            // バッグ使用中はワープできないようにする
        }

        frameImage_.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, speeds_[1]);
        // フィールドキャンバスがアクティブの時
        if (warpFieldScript_.GetLocationSelActive() == true)
        {
            speeds_[1] += 10.0f * Time.deltaTime;            // 右回転
            return;   // 町中のワープは選択できないように
        }
        else
        {
            //// 長針が動かない＝ワープ先を選べない
            if (warpFieldScript_.GetWarpNowFlag() == false)
            {
            //Debug.Log("warpNowFlag_がfalseの時");
                speeds_[1] += 10.0f * Time.deltaTime;            // 右回転
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // ワープ準備
                    warpFieldScript_.SetWarpNowFlag(true);
                    StartCoroutine(NeedleCheck(true));            // ワープ先選択処理
                }
            }
            else
            {
                speeds_[1] += 30.0f * Time.deltaTime;// 回転スピードを上げる
                if (framPositions_[1].x < frameImage_.transform.localPosition.x
                      && frameImage_.transform.localPosition.y < framPositions_[1].y)
                {
                    speeds_[0] += 1.0f * Time.deltaTime;
                    frameImage_.transform.localPosition -= new Vector3(speeds_[0], -speeds_[0], 0.0f);
                    warpActive_.transform.localPosition = frameImage_.transform.localPosition;
                    //Debug.Log(maxHandlePos_ + "    <   " + frameImage_.transform.localPosition.x);
                }
            }
        }
    }

    private IEnumerator NeedleCheck(bool flag)
    {
        // 0.5秒待ってからワープ選択処理に入る
        yield return new WaitForSeconds(0.5f);
        while (flag)
        {
            yield return null;
            // 選択先を変更
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(WarpCansel());            // ワープキャンセルの場合
                warpFieldScript_.CancelCheck();
                yield break; // ループを抜ける
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
                if (warpNum_ == (int)warp.FIELD)
                {
                    // フィールド選択時
                    warpFieldScript_.SetNowTownFlag(true);// 町からのワープだとFlagをtrueにする
                }
                else
                {
                    warpFieldScript_.CancelCheck();
                    if (warpNum_ == (int)warp.HOUSE)
                    {
                        // Scene名がTownならユニの家のシーンに飛ぶ
                        CommonWarp(SceneMng.SCENE.TOWN, SceneMng.SCENE.UNIHOUSE);
                    }
                    else
                    {
                        // Scene名がInHouseAndUniHouseならTwonに飛ぶ
                        CommonWarp(SceneMng.SCENE.UNIHOUSE, SceneMng.SCENE.TOWN);
                    }
                }
                warpFieldScript_.SetWarpNowFlag(false);
                StartCoroutine(WarpCansel());            // ワープ処理リセット
                yield break;
            }
        }
    }

    private void CommonWarp(SceneMng.SCENE scene, SceneMng.SCENE nextScene)
    {
        // 現在シーンと同じ名前ならシーン遷移する
        if (SceneMng.nowScene == scene)
        {
            SceneMng.SceneLoad((int)nextScene);
        }
        else
        {
            StartCoroutine(FadeOutAndIn()); // 以外ならフェードアウトさせるだけ
        }
    }

    private IEnumerator FadeOutAndIn()
    {
        fade_.FadeIn(1);
        uniChan_.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        uniChan_.SetActive(true);
        if (SceneMng.nowScene == SceneMng.SCENE.TOWN)
        {
            // 街中ワープ後のユニちゃんの座標
            uniChan_.transform.position = warpChildren_[warpNum_].transform.position;
            // サブカメラに切り替わっていたら
            if (cameraMng_.mainCamera.activeSelf == false)
            {
                cameraMng_.SetChangeCamera(false);// メインカメラに戻す
            }
        }
        fade_.FadeOut(1);
        yield break; // ループを抜ける
    }

    private IEnumerator WarpCansel()
    {
        StopCoroutine(NeedleCheck(false));
        // 移動Cancelの場合
        while (true)
        {
            yield return null;
            if (frameImage_.transform.localPosition.x < framPositions_[0].x
                  && framPositions_[0].y < frameImage_.transform.localPosition.y)
            {
                speeds_[0] += 1.0f * Time.deltaTime;
                frameImage_.transform.localPosition += new Vector3(speeds_[0], -speeds_[0], 0.0f);
                warpActive_.transform.localPosition = frameImage_.transform.localPosition;
                //Debug.Log(maxHandlePos_ + "    <   " + frameImage_.transform.localPosition.x);
            }
            else
            {
                // スピードが変わるため初期化
                speeds_[0] = 8.0f;
                speeds_[1] = 0.0f;
                frameImage_.transform.localPosition = framPositions_[0];
                warpActive_.transform.localPosition = framPositions_[0];
                yield break; // ループを抜ける
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneMng.nowScene == SceneMng.SCENE.TOWN)
            {
                Debug.Log("ユニの家の前のコライダーに接触");
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
            }
            else
            {
                // ユニの家から街に出る
                warpNum_ = (int)warp.HOUSE;
                Debug.Log("ユニの家からタウンに出現" + uniChan_.transform.position);
                SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);
            }
        }
    }
}