using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatusMagicMng : MonoBehaviour
{
    private Image magicSetBack_;// 魔法全体背景
    private Image magicSetCheckMask_;// 魔法一覧用のMask

    public void Init()
    {
        if (gameObject.activeSelf == true)
        {
            magicSetBack_ = GameObject.Find("MagicSetBack").GetComponent<Image>();
            magicSetCheckMask_ = GameObject.Find("MagicCheckMask").GetComponent<Image>();
        }
        if (magicSetBack_ != null)
        {
            magicSetBack_.fillAmount = 0.5f;
            magicSetCheckMask_.fillAmount = 0.0f;
        }
    }

    private IEnumerator MagicMaskFillAmount()
    {
        Debug.Log("FillAmount変更処理が呼ばれました");
        while (true)
        {
            yield return null;
            if (magicSetBack_.fillAmount < 1.0f)
            {
                magicSetBack_.fillAmount += 0.05f;
                magicSetCheckMask_.fillAmount += 0.1f;
            }
            else
            {
                magicSetBack_.fillAmount = 1.0f;
                magicSetCheckMask_.fillAmount = 1.0f;
                yield break;
            }
        }
    }

    private IEnumerator MagicMaskClose()
    {
        Debug.Log("魔法一覧の表示を閉じます");
        while (true)
        {
            yield return null;
            if (0.5f < magicSetBack_.fillAmount)
            {
                magicSetBack_.fillAmount -= 0.05f;
                magicSetCheckMask_.fillAmount -= 0.1f;
            }
            else
            {
                magicSetBack_.fillAmount = 0.5f;
                magicSetCheckMask_.fillAmount = 0.0f;
                yield break;
            }
        }
    }

    //public void SetBackColor(SceneMng.CHARACTERNUM chara)
    //{
    //    magicSetBack_.color = chara == SceneMng.CHARACTERNUM.UNI ? uniColor_ : jackColor_;
    //}

    public void SetCoroutineFlag()
    {
        if (magicSetBack_.fillAmount != 1.0f)
        {
            StartCoroutine(MagicMaskFillAmount());
        }
    }

    public void SetCloseFlag()
    {
        if (magicSetBack_.fillAmount != 0.5)
        {
            StartCoroutine(MagicMaskClose());
        }
    }
}