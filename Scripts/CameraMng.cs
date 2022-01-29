using UnityEngine;

// 街の中の定点カメラや戦闘時のバトルカメラはサブカメラとして管理している
// メインカメラは、街でもフィールドでもキャラに後ろから追従するようにしている

public class CameraMng : MonoBehaviour
{
    public GameObject mainCamera;      // メインカメラ格納用
    public GameObject subCamera;       // フィールドならバトルカメラ格納,街なら定点カメラ格納

    // 街や戦闘時のサブカメラ位置を変更するときに呼ばれる
    public void SetSubCameraPos(Vector3 pos)
    {
        subCamera.transform.position = pos;
    }

    public void SetSubCameraRota(Quaternion rota)
    {
        subCamera.transform.rotation = rota;
    }

    // 外部からカメラ状態の切替を行えるようにする
    public void SetChangeCamera(bool flag, bool allfalseFlag = false)
    {
        if(allfalseFlag)
        {
            mainCamera.GetComponent<AudioListener>().enabled = false;
            mainCamera.SetActive(false);

            subCamera.GetComponent<AudioListener>().enabled = false;
            subCamera.SetActive(false);
        }
        else
        {
            mainCamera.GetComponent<AudioListener>().enabled = !flag;
            mainCamera.SetActive(!flag);

            subCamera.GetComponent<AudioListener>().enabled = flag;
            subCamera.SetActive(flag);
        }

        // ユニハウスのSceneでは呼ばれないようにする
        if (mainCamera.activeSelf && SceneMng.nowScene != SceneMng.SCENE.UNIHOUSE)
        {
            mainCamera.GetComponent<CameraSample>().Init();
        }
    }

    // FieldMng.csの現在シーン確定後に呼び出すもの
    public void MainCameraPosInit()
    {
        // 設定を呼び出す
        mainCamera.GetComponent<CameraSample>().Init();
    }
}
