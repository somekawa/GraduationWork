using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラのターンを管理する
// ButtleCanvasの表示/非表示もここで管理する
// ここで、戦闘時はUnitychanControllerを非アクティブにするってしたほうが良さそう

public class ButtleMng : MonoBehaviour
{
    // enumとキャラオブジェクトをセットにしたmapを制作するためのリスト
    // キャラオブジェクトを要素としてアタッチできるようにしておく
    public List<GameObject> charList;
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public GameObject buttleWarpPointPack;  // 戦闘時にフィールド上の戦闘ポイントにキャラをワープさせる

    // キャラ識別用enum
    enum CharcterNum
    {
        UNI,    // 手前
        DEMO,   // 奥
        MAX
    }

    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // 現在行動順が回ってきているキャラクター

    private const int buttleCharMax_ = 2;           // バトル参加可能キャラ数の最大値(最終的には3にする)
    private Vector3[] buttleWarpPointsPos_       = new Vector3[buttleCharMax_];      // 戦闘時の配置位置を保存しておく変数
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // 戦闘時の回転角度を保存しておく変数(クォータニオン)
    private bool setCallOnce_ = false;              // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

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

        nowTurnChar_ = CharcterNum.UNI;

        buttleUICanvas.gameObject.SetActive(false);

        // ワープポイントの数ぶん、for文を回す
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // 個別にワープポイントを変数へ保存していく
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }
    }

    void Update()
    {
        // FieldMngで遭遇タイミングを調整しているため、それを参照し、戦闘モード以外ならreturnする
        if (FieldMng.nowMode_ != FieldMng.MODE.BUTTLE)
        {
            setCallOnce_ = false;
            buttleUICanvas.gameObject.SetActive(false);
            return;
        }

        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);

            // 戦闘用座標と回転角度を代入する
            // キャラの角度を変更は、ButtleWarpPointの箱の角度を回転させると可能。(1体1体向きを変えることもできる)
            foreach (KeyValuePair<CharcterNum, GameObject> character in charMap_)
            {
                character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
                character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];
            }
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
