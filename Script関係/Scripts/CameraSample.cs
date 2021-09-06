using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSample : MonoBehaviour
{
    private GameObject player_;   // プレイヤー情報格納用
    private Vector3 offset_;      // 相対距離取得用

    void Start()
    {
        //unitychanの情報を取得
        this.player_ = GameObject.Find("Uni");

        // MainCamera(自分自身)とplayerとの相対距離を求める
        offset_ = transform.position - player_.transform.position;
    }

    void Update()
    {
        //新しいトランスフォームの値を代入する
        transform.position = player_.transform.position + offset_;
    }
}