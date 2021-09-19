using UnityEngine;

public class FieldOutLEDs : MonoBehaviour
{
    private float intensity_ = 0.1f;
    private Renderer[] childrenRend_;

    void Start()
    {
        childrenRend_ = new Renderer[transform.childCount];
        for (int i=0;i<transform.childCount;i++)
        {
            childrenRend_[i]=transform.GetChild(i).GetComponent<Renderer>();
            //Debug.Log(childrenRend_[i].name+"LEDの明るさ" + intensity_);
        }
    }

    void Update()
    {
        intensity_ = Mathf.Sin(Time.time );// -1～1を取得
        intensity_=Mathf.Abs(intensity_);// -1は暗くなるだけのため絶対値で正の値に変更
        UpdateColor();
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
