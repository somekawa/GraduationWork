using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePoint : MonoBehaviour
{
    public enum JUDGE
    {
        NON = -1,
        BAD,
        NORMAL,
        GOOD,
        MAX
    }
    private JUDGE judge_ = JUDGE.NON;


    // カウントダウン表示
    private Image countImage_;
    private TMPro.TextMeshProUGUI countText_;

    private RectTransform gameGauge_;
    private float gaugeOffsetMin_;
    private float gaugeOffsetMax_;
    private RectTransform specialGauge_;
    private float[] sizeDeltaX_ = new float[4]{
        30.0f,50.0f,70.0f,100.0f
    };
    private float[] specialPosX_ = new float[5]{
            -200.0f,-100.0f,70.0f,100.0f,200.0f
        };

    private float specialMin_;
    private float specialMax_;

    private Image pointImage_;
    private Vector2 savePointPos_;

    public void Init()
    {
        // カウントダウン関連
        countImage_ = transform.Find("CountImage").GetComponent<Image>();
        countText_ = countImage_.transform.Find("Count").GetComponent<TMPro.TextMeshProUGUI>();
        countImage_.gameObject.SetActive(false);
        // ミニゲーム関連
        gameGauge_ = this.transform.Find("BackImage").GetComponent<RectTransform>();
        gaugeOffsetMin_ = gameGauge_.anchorMin.x;
        gaugeOffsetMax_ = gameGauge_.anchorMax.x;
        Debug.Log("黄色ゲージの最小" + gaugeOffsetMin_ + "最大" + gaugeOffsetMax_);
        // スペシャルゲージ
        specialGauge_ = gameGauge_.transform.Find("SpecialImage").GetComponent<RectTransform>();

        // ポイント
        pointImage_ = gameGauge_.transform.Find("Point").GetComponent<Image>();
        pointImage_.gameObject.SetActive(false);
    }

    public IEnumerator CountDown()
    {
        if (specialGauge_ == null)
        {
            Init();
        }
        // 大成功位置と幅を決める
        specialGauge_.sizeDelta = new Vector2(sizeDeltaX_[(Random.Range(0, 3))], 0.0f);
        specialGauge_.transform.localPosition = new Vector2(specialPosX_[(Random.Range(0, 4))], 0.0f);
        specialMin_ = specialGauge_.transform.localPosition.x - 50.0f;
        specialMax_ = specialGauge_.transform.localPosition.x + 50.0f;
        Debug.Log("スペシャルゲージの最小" + specialMin_ + "最大" + specialMax_);
        countImage_.gameObject.SetActive(true);

        countText_.text = "3";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "2";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "1";
        yield return new WaitForSeconds(1.0f);
        countImage_.gameObject.SetActive(false);
        pointImage_.gameObject.SetActive(true);

        while (true)
        {
            yield return null;
            // pos_=speed_*time
            pointImage_.transform.localPosition = new Vector3(Mathf.Sin(Time.time) * 200.0f, 0, 0);
            // Debug.Log("ポイントを移動" + pointImage_.transform.localPosition);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                savePointPos_ = pointImage_.transform.localPosition;
                Debug.Log("座標を保存" + savePointPos_);
                // createMng_.GetJudgeCheck(JUDGE.NORMAL);
                judge_ = JUDGE.NORMAL;
                // 大成功の場合
                if (specialMin_ < savePointPos_.x && savePointPos_.x < specialMax_)
                {
                    //createMng_.GetJudgeCheck(JUDGE.GOOD);
                    judge_ = JUDGE.GOOD;
                }
                // finishFlag_ = true;
                yield break;
            }
        }
    }

    public JUDGE GetMiniGameJudge()
    {
        return judge_;
    }

    public void SetMiniGameJudge(JUDGE judge)
    {
        judge_ = judge;
    }
}