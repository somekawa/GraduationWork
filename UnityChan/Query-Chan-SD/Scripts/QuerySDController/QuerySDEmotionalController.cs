using UnityEngine;
using System.Collections;

public class QuerySDEmotionalController : MonoBehaviour {

	[SerializeField]
	Material[] emotionalMaterial;

	[SerializeField]
	GameObject queryFace;

    private IEnumerator rest_;

    public enum QueryChanSDEmotionalType
	{
		// Normal emotion
		NORMAL_DEFAULT = 0,
		NORMAL_ANGER = 1,
		NORMAL_BLINK = 2,
		NORMAL_GURUGURU = 3,
		NORMAL_SAD = 4,
		NORMAL_SMILE = 5,
		NORMAL_SURPRISE = 6

	}


	public void ChangeEmotion (QueryChanSDEmotionalType faceNumber,bool flag = false)
	{
		queryFace.GetComponent<Renderer>().material = emotionalMaterial[(int)faceNumber];

        if(rest_ != null)
        {
            StopCoroutine(rest_);
            rest_ = null;
        }
        rest_ = ReturnDefaultFace();

        // フラグがtrueで来たものはコルーチンを開始する
        if (flag)
        {
            StartCoroutine(rest_);
        }
	}

    // 表情をデフォルトに戻すときに使用するコルーチン
    private IEnumerator ReturnDefaultFace()
    {
        // 3秒待機
        float count = 0.0f;
        while(count <= 3.0f)
        {
            yield return null;
            count += Time.deltaTime;
        }

        Debug.Log("NPCを笑顔からデフォルトへ戻す");
        queryFace.GetComponent<Renderer>().material = emotionalMaterial[0];
    }
	
}
