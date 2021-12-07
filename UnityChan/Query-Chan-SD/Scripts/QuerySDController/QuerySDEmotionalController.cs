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

        // �t���O��true�ŗ������̂̓R���[�`�����J�n����
        if (flag)
        {
            StartCoroutine(rest_);
        }
	}

    // �\����f�t�H���g�ɖ߂��Ƃ��Ɏg�p����R���[�`��
    private IEnumerator ReturnDefaultFace()
    {
        // 3�b�ҋ@
        float count = 0.0f;
        while(count <= 3.0f)
        {
            yield return null;
            count += Time.deltaTime;
        }

        Debug.Log("NPC���Ί炩��f�t�H���g�֖߂�");
        queryFace.GetComponent<Renderer>().material = emotionalMaterial[0];
    }
	
}
