using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseInteriorMng : MonoBehaviour
{
    public Canvas warpCanvas;       //  ワープ先を示すキャンバス(表示/非表示を切り替えるために使用)
    public WarpTown warpTown;

    private CameraMng cameraMng_;
    private UnitychanController playerController_;

    private GameObject inHouseInfoCanvas_;
    private GameObject iconImage_;
    private TMPro.TextMeshProUGUI text_;

    private bool inHouseFlg_ = true;        // 入室するか(true:入る , false:入らない)

    private readonly string[] buildNameEng_ = { "UniHouse","MayorHouse","BookStore","ItemStore", "Guild" , "Restaurant" };  // 建物名(ヒエラルキーと同じ英語)
    private readonly string[] buildNameJpn_ = { "ユニの家", "町長の家" , "書店"    , "魔道具屋", "ギルド", "レストラン" };  // 建物名(表示用日本語)
    private Dictionary<string, string> buildNameMap_ = new Dictionary<string, string>();    // キー:英語建物名,値:日本語建物名

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
        iconImage_ = inHouseInfoCanvas_.transform.Find("Icon").gameObject;
        text_ = inHouseInfoCanvas_.transform.Find("HouseInfo/Text").GetComponent<TMPro.TextMeshProUGUI>();

        // 英語建物名と日本語建物名を組み合わせる
        for (int i = 0; i < buildNameEng_.Length; i++)
        {
            buildNameMap_.Add(buildNameEng_[i], buildNameJpn_[i]);
        }
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

        // 一致したオブジェクト以外を非アクティブ
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if (child.name != name)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }

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

        // 家のアクティブ/非アクティブの切替
        if (SetHouseVisible(name))
        {
            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
            cameraMng_.SetChangeCamera(true);
        }
    }
}
