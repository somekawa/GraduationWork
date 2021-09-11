using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラが通過したかを確認して、CameraMng.csにカメラ切り替えの指示を出す

public class GoThroughCheck : MonoBehaviour
{
    private CameraMng cameraMng_;
    private GameObject player_;

    private Vector3 EnterPos_;  // 当たり判定内に入った瞬間の座標
    private Vector3 ExitPos_;   // 当たり判定内を出た瞬間の座標

    void Start()
    {
        //unitychanの情報を取得
        player_ = GameObject.Find("Uni");
        if(player_ == null)
        {
            Debug.Log("GoThroughCheck.csで取得しているPlayer情報がnullです");
        }

        cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        if(cameraMng_ == null)
        {
            Debug.Log("GoThroughCheck.csで取得しているCameraMngがnullです");
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player")) //col.tag == "Player"と書くより、処理が速い
        {
            Debug.Log("カメラ切替");

            // 操作キャラクターが、画面に映らない状態でカメラ切り替えが発生するのを防ぐ
            if(player_.transform.position.z <= 94.0f)
            {
                player_.transform.position = new Vector3(player_.transform.position.x, player_.transform.position.y, 95.0f);
            }

            EnterPos_ = player_.transform.position;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player")) //col.tag == "Player"と書くより、処理が速い
        {
            ExitPos_ = player_.transform.position;

            if (this.gameObject.name == "GoThroughRight")
            {
                // 右通路の時(ギルドと魔道具屋方面)
                if ((ExitPos_ - EnterPos_).normalized.x >= 0.0f)
                {
                    // 1.0の時は右への通過の為true(サブカメラアクティブ)
                    cameraMng_.SetChangeCamera(true);
                    // カメラ位置調整
                    cameraMng_.SetSubCameraPos(new Vector3(24.0f, 3.0f, 89.0f));
                }
                else
                {
                    // -1.0の時は左への通過の為false(メインカメラアクティブ)
                    cameraMng_.SetChangeCamera(false);
                }
            }
            else
            {
                // 左通路の時(住宅街)
                if ((ExitPos_ - EnterPos_).normalized.x >= 0.0f)
                {
                    // 1.0の時は左への通過の為false(メインカメラアクティブ)
                    cameraMng_.SetChangeCamera(false);
                }
                else
                {
                    // -1.0の時は右への通過の為true(サブカメラアクティブ)
                    cameraMng_.SetChangeCamera(true);
                    // カメラ位置調整
                    cameraMng_.SetSubCameraPos(new Vector3(-24.0f, 3.0f, 89.0f));
                }
            }
        }
    }
}
