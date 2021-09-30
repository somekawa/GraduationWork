using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseInteriorMng : MonoBehaviour
{
    public Canvas warpCanvas;       // ワープ先を示すキャンバス(表示/非表示を切り替えるために使用)
    public WarpTown warpTown;       // ワープ処理の切替(enableで使用)

    private CameraMng cameraMng_;   // カメラ関係
    private UnitychanController playerController_;  // キャラ操作の切替

    private GameObject inHouseInfoCanvas_;  // 建物に入るかの案内を出す
    private GameObject iconImage_;          // [はい][いいえ]の現在選択中の方に矢印アイコンを出す
    private TMPro.TextMeshProUGUI text_;    // 建物名を入れる

    private GameObject inHouseCanvas_;      // 室内の選択肢

    private bool inHouseFlg_ = true;        // 入室するか(true:入る , false:入らない)

    private string nowInHouseName = "";     // 今いる建物の名前を一時的に保存する
    private readonly string[] buildNameEng_ = { "UniHouse","MayorHouse","BookStore","ItemStore", "Guild" , "Restaurant" };  // 建物名(ヒエラルキーと同じ英語)
    private readonly string[] buildNameJpn_ = { "ユニの家", "町長の家" , "書店"    , "魔道具屋", "ギルド", "レストラン" };  // 建物名(表示用日本語)
    private Dictionary<string, string> buildNameMap_ = new Dictionary<string, string>();    // キー:英語建物名,値:日本語建物名

    private MayorHouse mayorHouse_;

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
        iconImage_ = inHouseInfoCanvas_.transform.Find("Icon").gameObject;
        text_ = inHouseInfoCanvas_.transform.Find("HouseInfo/Text").GetComponent<TMPro.TextMeshProUGUI>();

        inHouseCanvas_ = this.transform.Find("InHouseCanvas").gameObject;

        // 英語建物名と日本語建物名を組み合わせる
        for (int i = 0; i < buildNameEng_.Length; i++)
        {
            buildNameMap_.Add(buildNameEng_[i], buildNameJpn_[i]);
        }

        mayorHouse_ = this.transform.Find("MayorHouse").GetComponent<MayorHouse>();
    }

    public bool SetHouseVisible(string name)
    {
        if (!inHouseFlg_)
        {
            // 選択肢を「はい」に戻す
            inHouseFlg_ = true;
            // アイコン位置を「はい」に戻す
            iconImage_.transform.localPosition = new Vector3(-140.0f, -70.0f, 0.0f);

            // ワープ先キャンバスの表示
            warpCanvas.gameObject.SetActive(true);

            // ワープ処理を可能にするためにtrueに戻す
            warpTown.enabled = true;

            // 入室処理時に必ずこの関数は呼ばれるが、
            // inHouseFlg_がfalseなら必要な処理をしてreturnする
            SetActiveCanvas(false, "");

            // キャラ操作を再開する
            playerController_.enabled = true;
            return false;
        }

        // 建物オブジェクトの表示/非表示切り替え
        ChangeObjectActive(this.gameObject.transform.childCount, this.transform,name);

        return true;
    }

    // 入室確認用キャンバスの表示/非表示を切り替える
    public void SetActiveCanvas(bool flag, string name)
    {
        inHouseInfoCanvas_.SetActive(flag);

        if (name == "")
        {
            return;
        }

        // コルーチンスタート  
        StartCoroutine(SelectInHouse(flag,name));
    }

    // コルーチン  
    private IEnumerator SelectInHouse(bool flag,string name)
    {
        // ワープ先キャンバスの非表示
        warpCanvas.gameObject.SetActive(false);
        // ワープ処理のスペースキーを止めるためにfalse
        warpTown.enabled = false;

        // キャラアニメーションを止める
        playerController_.StopUniRunAnim();

        // キャラ操作を止める
        playerController_.enabled = false;

        text_.text = buildNameMap_[name] + "に入る？";

        // キャンバスが表示中の間はコルーチン処理を行う
        while (flag)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                iconImage_.transform.localPosition = new Vector3(40.0f, -70.0f, 0.0f);
                inHouseFlg_ = false;
                Debug.Log("選択肢「いいえ」");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                iconImage_.transform.localPosition = new Vector3(-140.0f, -70.0f, 0.0f);
                inHouseFlg_ = true;
                Debug.Log("選択肢「はい」");
            }
            else
            {
                // 何も処理を行わない
            }

            // スペースキーで選択肢を決定し、flagをfalseにすることでwhile文から抜けるようにする
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("選択肢の決定");
                flag = false;
            }
        }

        Debug.Log("コルーチンの終了");

        // 家のアクティブ/非アクティブの切り替え
        if (SetHouseVisible(name))
        {
            // 現在いる建物名を保存しておく
            nowInHouseName = name;

            //@ どのスクリプトでも対応できるように、mayorHouse_の部分を将来的にはmapにする
            if (!mayorHouse_.CheckEventMayorHouse())
            {
                cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
                cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
                cameraMng_.SetChangeCamera(true);

                // 室内キャンバスの表示
                inHouseCanvas_.SetActive(true);

                // 室内用キャンバスの表示/非表示切り替え
                ChangeObjectActive(inHouseCanvas_.gameObject.transform.childCount, inHouseCanvas_.transform, name);
            }
        }
    }

    // オブジェクトの表示/非表示の処理
    private void ChangeObjectActive(int childNum, Transform trans, string buildName)
    {
        // 子の数でfor文を回して、建物の名前一致は表示、名前不一致は非表示にする
        for (int i = 0; i < childNum; i++)
        {
            var child = trans.GetChild(i);
            if (child.name != buildName)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    // 建物からの出口処理テスト
    public void ExitButton()
    {
        Debug.Log("外へ出るボタンが押されました");

        // 室内用キャンバスの非表示
        inHouseCanvas_.SetActive(false);

        // ワープ先キャンバスの表示
        warpCanvas.gameObject.SetActive(true);

        // ワープ処理を可能にするためにtrueに戻す
        warpTown.enabled = true;

        // キャラ操作を再開する
        playerController_.enabled = true;

        // 建物の名前で処理を分ける
        if(nowInHouseName == "Guild" || nowInHouseName == "ItemStore")
        {
            // 右通路位置にカメラを戻す
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f)));
            cameraMng_.SetSubCameraPos(new Vector3(28.0f, 3.0f, 89.0f));
        }
        else
        {
            // メインカメラに戻す
            cameraMng_.SetChangeCamera(false);
        }

        // 現在の建物名を初期化
        nowInHouseName = "";
    }
}
