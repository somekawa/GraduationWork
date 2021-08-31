using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラのターンを管理する

public class ButtleMng : MonoBehaviour
{
    // enumとキャラオブジェクトをセットにしたmapを制作するためのリスト
    // キャラオブジェクトを要素としてアタッチできるようにしておく
    public List<GameObject> charList;

    // キャラ識別用enum
    enum CharcterNum
    {
        UNI,    // 手前
        DEMO,   // 奥
        MAX
    }

    private GameObject buttleCamera;                // バトルカメラ格納用 
    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // 現在行動順が回ってきているキャラクター

    // それぞれのキャラ用に、必要なものをstructでまとめる
    // AnimatorとかHPとか
    public struct CharcterSetting
    {
        public Animator animator;   // 各キャラについているAnimatorのコンポーネント
        public bool isMove;         // Waitモーション時はfalse
        public float animTime;      // 次のキャラの行動に遷移するまでの間
    }

    // 上記の構造体を配列にしたもの
    private CharcterSetting[] charSetting = new CharcterSetting[(int)CharcterNum.MAX];

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    private Dictionary<CharcterNum, GameObject> charMap_;

    void Start()
    {
        // (何かに使えるかもしれないから、)キャラの情報はゲームオブジェクトとして最初に取得しておく
        charMap_ = new Dictionary<CharcterNum, GameObject>(){
            {CharcterNum.UNI,charList[(int)CharcterNum.UNI]},
            {CharcterNum.DEMO,charList[(int)CharcterNum.DEMO]},
        };

        // charMap_でforeachを回して、Animatorを取得する
        foreach (KeyValuePair<CharcterNum, GameObject> anim in charMap_) 
        {
            // 構造体のanimatorに、キャラ毎のAnimatorを代入する
            charSetting[(int)anim.Key].animator = anim.Value.GetComponent<Animator>();
            charSetting[(int)anim.Key].isMove = false;
            charSetting[(int)anim.Key].animTime = 0.0f;
        }

        buttleCamera = GameObject.Find("ButtleCamera");
        nowTurnChar_ = CharcterNum.UNI;
    }

    void Update()
    {
        // ButtoleCameraがOFFならここでreturnする
        if (!buttleCamera.activeSelf)
        {
            return;
        }

        Debug.Log(nowTurnChar_ + "の攻撃");

        // キャラ毎のモーションを呼ぶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
            charSetting[(int)nowTurnChar_].isMove = true;
        }
        else
        {
            charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, false);

            if (!charSetting[(int)nowTurnChar_].isMove)
            {
                return;
            }

            // ここから下は、isMoveがtrueの状態
            // isMoveがtrueなら、さっきまで攻撃モーションだったことがわかる
            // 再生時間用に間を空ける
            // キャラ毎に必要な間が違うかもしれないから、1.0fの所は外部データ読み込みで、charSetting[(int)nowTurnChar_].maxAnimTimeとかを用意したほうがいい
            if (charSetting[(int)nowTurnChar_].animTime < 1.0f)
            {
                charSetting[(int)nowTurnChar_].animTime += Time.deltaTime;
                return;
            }
            else
            {
                // animTime値の初期化と、モーションがWaitになった為isMoveをfalseへ戻す
                charSetting[(int)nowTurnChar_].animTime = 0.0f;
                charSetting[(int)nowTurnChar_].isMove = false;

                // animTimeが1.0f以上になったら次のキャラが行動できるようにする
                nowTurnChar_++;
                // 最大まで加算されたら、初期値に戻す
                if (nowTurnChar_ >= CharcterNum.MAX)
                {
                    nowTurnChar_ = CharcterNum.UNI;
                }
            }
        }

        // ↓元のキャラモーション操作
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
        //    //nowTurnChar_++; // ここで加算するとfalseに辿り着いてないのか変な動きになる
        //}
        //else
        //{
        //    charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, false);
        //}
    }
}
