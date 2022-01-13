using UnityEngine;

public class DamageIconCtl : MonoBehaviour
{
    private System.Collections.IEnumerator enumerator_;
    private UnityEngine.UI.Image image_;
    private float alpha_;
    private const float moveNum = 0.5f;  

    // �\����
    private void OnEnable()
    {
        image_ = GetComponent<UnityEngine.UI.Image>();
        alpha_ = 1.0f;
        image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
        enumerator_ = MovePosAndAlpha();
        StartCoroutine(enumerator_);
    }

    // ��\����
    private void OnDisable()
    {
        StopCoroutine(enumerator_);
        enumerator_ = null;
        alpha_ = 0.0f;
        image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
    }

    private System.Collections.IEnumerator MovePosAndAlpha()
    {
        // alpha�l��0.0f�ȏ�̊Ԃ́Awhile�����񂵑�����
        while(alpha_ > 0.0f)
        {
            // ��֏㏸
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + moveNum, transform.localPosition.z);
            // ���l����
            alpha_ -= Time.deltaTime * (moveNum * 2.0f);
            image_.color = new Color(1.0f, 1.0f, 1.0f, alpha_);
            yield return null;
        }

        // while���𔲂�����́A��\���֐؂�ւ���
        gameObject.SetActive(false);
    }
}
