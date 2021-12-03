using System.Collections;
using UnityEngine;

public class CameraSample : MonoBehaviour
{
    private GameObject player_;   // プレイヤー情報格納用
    private Vector3 offset_;      // 相対距離取得用
    private IEnumerator rest_;    // コルーチンを保存する

    public void Init()
    {
        //unitychanの情報を取得
        this.player_ = GameObject.Find("Uni");

        if (SceneMng.nowScene == SceneMng.SCENE.FIELD3)
        {
            // 洞窟型のフィールドだけ見下ろし型カメラにする
            offset_ = new Vector3(0.0f, 4.0f, -2.0f);
        }
        else
        {
            offset_ = new Vector3(0.0f, 3.0f, -3.0f);
        }

        if(rest_ == null)
        {
            rest_ = CameraPosCoroutine();
        }
        else
        {
            StopCoroutine(rest_); //一時停止
            rest_ = null;         //リセット
            rest_ = CameraPosCoroutine(); //入れなおし
        }

        StartCoroutine(rest_);
    }

    // カメラを移動させるためのコルーチン
    private IEnumerator CameraPosCoroutine()
    {
        bool tmpFlg = false;
        while(!tmpFlg)
        {
            yield return null;
            //新しいトランスフォームの値を代入する
            transform.position = player_.transform.position + offset_;
        }
    }
}