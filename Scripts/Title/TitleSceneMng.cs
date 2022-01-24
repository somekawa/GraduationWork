using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneMng : MonoBehaviour
{
    //プレイヤーを変数に格納
    public GameObject Player;

    //回転させるスピード
    public float rotateSpeed = 0.5f;

    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("カメラを動かせます");
       //// StartCoroutine(MoveCamera());
    }

    void Update()
    {
        camera.transform.RotateAround(Player.transform.position, new Vector3(0, 1, 0), 0.5f);
    }

    private IEnumerator MoveCamera()
    {
        while (true)
        {
            yield return null;
            //回転させる角度
            float angle = Input.GetAxis("Horizontal") * rotateSpeed;

            //プレイヤー位置情報
            Vector3 playerPos = Player.transform.position;

            //カメラを回転させる
            transform.RotateAround(playerPos, Vector3.up, angle);

            // 左ボタン押下かスペースキー押下でレベルアップ用の画像を表示
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("コルーチンを止めました");
                yield break;
            }

        }
    }

    public void OnClickNewGame()
    {
        SceneManager.LoadScene("conversationdata");
    }

    public void OnClickLoadGame()
    {
        SceneManager.LoadScene("Town");
    }
}
