using System.Collections;
using UnityEngine;

// 右から差し込まれるようなUI

public class UIMove : MonoBehaviour
{
    // コルーチンの現在時間
    private float time_;
    // コルーチン終了時間
    private float totalTime_;
    // 目的座標
    private readonly Vector3 DestinationPos_ = new Vector3(0.0f, 0.0f, 0.0f);

    void OnEnable()
    {
        time_ = 0.0f;
        totalTime_ = 5.0f;

        float tmpX = 850.0f;
        float tmpY = 150.0f;
        // 画面外右にボタン位置を初期化する(2回目以降も右から差し込まれるようにするため)
        // 子のボタン数分for文を回す
        for (int i = 0; i < transform.childCount; i++)
        {
            // HouseInfoという名前のゲームオブジェクトは移動に含まれないようにする
            if(transform.GetChild(i).name == "HouseInfo")
            {
                var houseInfo = transform.GetChild(i);
                houseInfo.transform.localPosition = new Vector3(500.0f,320.0f,0.0f);

                // 更に、HouseInfoの子にHaveMoneyTextというゲームオブジェクトがあれば、そこに現在の所持金を代入する
                for(int k = 0; k < houseInfo.childCount; k++)
                {
                    if(houseInfo.GetChild(k).name == "HaveMoneyText")
                    {
                        houseInfo.GetChild(k).GetComponent<TMPro.TextMeshProUGUI>().text = SceneMng.GetHaveMoney().ToString();
                    }
                }

                continue;
            }

            transform.GetChild(i).transform.localPosition = new Vector3(tmpX, tmpY, 0.0f);
            tmpX += 50.0f;
            tmpY -= 100.0f;
        }

        StartCoroutine(Easing());
    }

    // コルーチン  
    private IEnumerator Easing()
    {
        // ボタン分のフラグの配列を用意する
        bool[] flag = new bool[transform.childCount];
        // 値の初期化
        for (int i = 0; i < transform.childCount; i++)
        {
            flag[i] = false;
        }

        // while文の終了用フラグ
        bool allFlag = false;   

        while(!allFlag)
        {
            time_ += Time.deltaTime;

            yield return null;

            // 子のボタン数分for文を回す
            for (int i = 0; i < transform.childCount; i++)
            {
                // 既にフラグがtrueなら飛ばして、次を回す
                if(flag[i])
                {
                    continue;
                }

                // HouseInfoならすぐにtrueにして飛ばす
                if (transform.GetChild(i).name == "HouseInfo")
                {
                    flag[i] = true;
                    continue;
                }

                var tmp = transform.GetChild(i).transform.localPosition;

                //目標座標より値が大きかったら座標を引いて更新する
                if (DestinationPos_.x < tmp.x)
                {
                    Vector2 pos = SineInOut(time_, totalTime_, tmp, DestinationPos_);
                    transform.GetChild(i).transform.localPosition = new Vector3(pos.x, tmp.y, tmp.z);
                }
                else
                {
                    transform.GetChild(i).transform.localPosition = new Vector3(DestinationPos_.x, tmp.y, tmp.z);
                    flag[i] = true;
                }
            }

            // すべてのフラグがtrueか確認する
            int num = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if(flag[i]) // trueの数だけnumを加算する
                {
                    num++;
                }
            }

            // すべてtrue(numの値と子の値が同じ)ならコルーチンを抜ける
            if(num >= transform.childCount)
            {
                allFlag = true;
            }
        }
    }


    // イージング関数(右から差し込まれるようなUI表現)
    private Vector2 SineInOut(float t, float totaltime, Vector2 min, Vector2 max)
    {
        max -= min;
        return -max / 2 * (Mathf.Cos(t * Mathf.PI / totaltime) - 1) + min;
    }
}
