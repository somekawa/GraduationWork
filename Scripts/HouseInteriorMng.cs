using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInteriorMng : MonoBehaviour
{
    private CameraMng cameraMng_;
    private UnitychanController playerController_;

    private GameObject inHouseInfoCanvas_;
    private bool inHouseFlg_ = true;        // 入室するか(true:入る , false:入らない)

    void Start()
    {
        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if (cameraMng_ == null)
        {
            Debug.Log("HouseInteriorMng.csで取得しているCameraMngがnullです");
        }

        playerController_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        if (playerController_ == null)
        {
            Debug.Log("HouseInteriorMng.csで取得しているplayerController_がnullです");
        }

        inHouseInfoCanvas_ = this.transform.Find("Canvas").gameObject;
    }

    public bool SetHouseVisible(string name)
    {
        if (!inHouseFlg_)
        {
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
        // キャラアニメーションを止める
        playerController_.StopUniRunAnim();

        // キャラ操作を止める
        playerController_.enabled = false;

        // キャンバスが表示中の間はコルーチン処理を行う
        while (flag)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                inHouseFlg_ = false;
                Debug.Log("選択肢「いいえ」");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
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
