using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 現在のシーンをTOWNとする
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);

        // メインカメラを最初にアクティブにする
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);
    }
}
