using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldOutLEDs : MonoBehaviour
{
    private GameObject cameraCtl_;      // CameraMng.csがアタッチされてるオブジェクト
    private CameraMng cameraMng;        // メインカメラとサブカメラの切り替え

    private float intensity_ = 0.1f;
    private Renderer[] childrenRend_;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Town")
        {
            // オブジェクト経由でCameraMngScriptを取得
            cameraCtl_ = GameObject.Find("CameraController");
            cameraMng = cameraCtl_.transform.GetComponent<CameraMng>();
        }

        childrenRend_ = new Renderer[transform.childCount];
        for (int i=0;i<transform.childCount;i++)
        {
            childrenRend_[i]=transform.GetChild(i).GetComponent<Renderer>();
            //Debug.Log(childrenRend_[i].name+"LEDの明るさ" + intensity_);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Town")
        {
            intensity_ = Mathf.Sin(Time.time);// -1～1を取得
            intensity_ = Mathf.Abs(intensity_);// -1は暗くなるだけのため絶対値で正の値に変更
            UpdateColor();
        }
        else
        {
            if (cameraMng.subCamera.activeSelf == true)
            {
                intensity_ = Mathf.Sin(Time.time);// -1～1を取得
                intensity_ = Mathf.Abs(intensity_);// -1は暗くなるだけのため絶対値で正の値に変更
                UpdateColor();
            }
        }
    }

    private void UpdateColor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Material mat = childrenRend_[i].material;
            Color baseColor = mat.color;

            // Calculate the resulting color based on the intensity.
            Color finalColor = baseColor * Mathf.LinearToGammaSpace(intensity_);
            mat.SetColor("_EmissionColor", finalColor);
        }
    }
}
