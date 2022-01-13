using UnityEngine;

public class DamageIconCtl : MonoBehaviour
{
    private System.Collections.IEnumerator enumerator_;
    private UnityEngine.UI.Image image_;
    private float alpha_;
    private const float moveNum = 0.5f;  

    // 表示時
    private void OnEnable()
    {
        image_ = GetComponent<UnityEngine.UI.Image>();
        alpha_ = 1.0f;
        image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
        enumerator_ = MovePosAndAlpha();
        StartCoroutine(enumerator_);
    }

    // 非表示時
    private void OnDisable()
    {
        StopCoroutine(enumerator_);
        enumerator_ = null;
        alpha_ = 0.0f;
        image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
    }

    private System.Collections.IEnumerator MovePosAndAlpha()
    {
        // alpha値が0.0f以上の間は、while文を回し続ける
        while(alpha_ > 0.0f)
        {
            // 上へ上昇
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + moveNum, transform.localPosition.z);
            // α値減少
            alpha_ -= Time.deltaTime * (moveNum * 2.0f);
            image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
            yield return null;
        }

        // while文を抜けた後は、非表示へ切り替える
        gameObject.SetActive(false);
    }
}
