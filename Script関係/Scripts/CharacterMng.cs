using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 探索中/戦闘中問わず、キャラクターに関連するものを管理する

public class CharacterMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される

    // enumとキャラオブジェクトをセットにしたmapを制作するためのリスト
    // キャラオブジェクトを要素としてアタッチできるようにしておく
    public List<GameObject> charList;
    public GameObject buttleWarpPointPack;  // 戦闘時にフィールド上の戦闘ポイントにキャラをワープさせる

    //　通常攻撃弾のプレハブ
    [SerializeField]
    private GameObject uniAttackPrefab_;

    // キャラ識別用enum
    enum CharcterNum
    {
        UNI,    // 手前
        DEMO,   // 奥
        MAX
    }

    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)

    CharcterNum nowTurnChar_ = CharcterNum.MAX;     // 現在行動順が回ってきているキャラクター
    private bool selectFlg_ = false;

    private const int buttleCharMax_ = 2;           // バトル参加可能キャラ数の最大値(最終的には3にする)
    private Vector3[] buttleWarpPointsPos_ = new Vector3[buttleCharMax_];            // 戦闘時の配置位置を保存しておく変数
    private Quaternion[] buttleWarpPointsRotate_ = new Quaternion[buttleCharMax_];   // 戦闘時の回転角度を保存しておく変数(クォータニオン)

    // それぞれのキャラ用に、必要なものをstructでまとめる
    // AnimatorとかHPとか
    public struct CharcterSetting
    {
        public Animator animator;   // 各キャラについているAnimatorのコンポーネント
        public bool isMove;         // Waitモーション時はfalse
        public float animTime;      // 次のキャラの行動に遷移するまでの間
        public Vector3 buttlePos;   // 戦闘開始時に設定されるポジション(攻撃エフェクト等のInstance位置に利用)
    }

    // 上記の構造体を配列にしたもの
    private CharcterSetting[] charSetting = new CharcterSetting[(int)CharcterNum.MAX];

    // キーをキャラ識別enum,値を(キャラ識別に対応した)キャラオブジェクトで作ったmap
    private Dictionary<CharcterNum, GameObject> charMap_;

    private ImageRotate buttleCommandUI_;                         // バトル中のコマンドUIを取得して、保存しておく変数
    private EnemySelect buttleEnemySelect_;                       // バトル中の選択アイコン情報

    private int enemyNum_ = 0;                                    // バトル時の敵の数
    private Dictionary<int, List<Vector3>> enemyInstancePos_;     // 敵のインスタンス位置の全情報

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

        // ワープポイントの数ぶん、for文を回す
        for (int i = 0; i < buttleWarpPointPack.transform.childCount; i++)
        {
            // 個別にワープポイントを変数へ保存していく
            buttleWarpPointsPos_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.position;
            buttleWarpPointsRotate_[i] = buttleWarpPointPack.transform.GetChild(i).gameObject.transform.rotation;
        }

        buttleCommandUI_   = buttleUICanvas.transform.Find("Image").GetComponent<ImageRotate>();
        buttleEnemySelect_ = buttleUICanvas.transform.Find("EnemySelectObj").GetComponent<EnemySelect>();

        enemyInstancePos_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().GetEnemyPos();
    }

    void Update()
    {
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            nowTurnChar_ = CharcterNum.UNI;
        }
    }

    // ButtleMng.csから敵の数を受け取る
    public void SetEnemyNum(int enemyNum)
    {
        enemyNum_ = enemyNum;

        // 矢印アイコンが表示できるように座標を渡す
        // 一時変数に発生位置をコピーしてそれを代入することで、敵の発生位置の高さが書き換えるのを防ぐ
        List<Vector3> tmpInsPos = new List<Vector3>(enemyInstancePos_[enemyNum_]);
        buttleEnemySelect_.SetPosList(tmpInsPos);

        // NGな書き方
        // この書き方では、元の敵の発生位置座標を書き換える形で矢印アイコンが生成されて、2回目以降敵の発生位置が矢印アイコンの高さになってしまう
        //buttleEnemySelect_.SetPosList(enemyInstancePos_[enemyNum_]);
    }

    // 戦闘開始時に設定される項目(ButtleMng.csで参照)
    public void ButtleSetCallOnce()
    {
        // 戦闘用座標と回転角度を代入する
        // キャラの角度を変更は、ButtleWarpPointの箱の角度を回転させると可能。(1体1体向きを変えることもできる)
        foreach (KeyValuePair<CharcterNum, GameObject> character in charMap_)
        {
            character.Value.gameObject.transform.position = buttleWarpPointsPos_[(int)character.Key];
            character.Value.gameObject.transform.rotation = buttleWarpPointsRotate_[(int)character.Key];

            // ここで座標を保存しておくことで、メニュー画面での並び替えでも反映できるだろうし、
            // 攻撃エフェクトの発生位置の目安になる
            charSetting[(int)character.Key].buttlePos  = character.Value.gameObject.transform.position;
        }
    }

    // キャラの戦闘中に関する処理(ButtleMng.csで参照)
    public void Buttle()
    {
        // ATTACKで敵選択中に、特定のキー(今はTキー)を押下されたらコマンド選択に戻る
        if(selectFlg_ && !buttleEnemySelect_.ReturnSelectCommand())
        {
            selectFlg_ = false;
            buttleCommandUI_.SetRotaFlg(!selectFlg_);   // コマンド回転を有効化
        }

        // キャラ毎のモーションを呼ぶ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 選択されたコマンドに対する処理
            switch(buttleCommandUI_.GetNowCommand())
            {
                case ImageRotate.COMMAND.ATTACK:

                    if(!selectFlg_)
                    {
                        selectFlg_ = true;
                    }
                    else
                    {
                        Debug.Log("攻撃コマンドが有効コマンドです");
                        charSetting[(int)nowTurnChar_].animator.SetBool(key_isAttack, true);
                        // isMoveがfalseのときだけ攻撃エフェクトのInstanceとisMoveのtrue化処理を行うようにして、
                        // エフェクトがボタン連打で大量発生するのを防ぐ
                        if (!charSetting[(int)nowTurnChar_].isMove)
                        {
                            AttackStart((int)nowTurnChar_);
                            charSetting[(int)nowTurnChar_].isMove = true;
                            selectFlg_ = false;
                        }
                    }

                    buttleCommandUI_.SetRotaFlg(!selectFlg_);
                    buttleEnemySelect_.SetActive(selectFlg_);

                    break;
                case ImageRotate.COMMAND.MAGIC:
                    Debug.Log("魔法コマンドが有効コマンドです");
                    break;
                case ImageRotate.COMMAND.ITEM:
                    Debug.Log("アイテムコマンドが有効コマンドです");
                    break;
                case ImageRotate.COMMAND.ESCAPE:
                    Debug.Log("逃走コマンドが有効コマンドです");
                    break;
                default:
                    Debug.Log("無効なコマンドです");
                    break;
            }
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
                // animTime値が1.0を上回ったとき
                // animTime値の初期化と、モーションがWaitになった為isMoveをfalseへ戻す
                charSetting[(int)nowTurnChar_].animTime = 0.0f;
                charSetting[(int)nowTurnChar_].isMove = false;

                // 次のキャラが行動できるようにする
                // 最大まで加算されたら、初期値に戻す(前演算子重要)
                if (++nowTurnChar_ >= CharcterNum.MAX)
                {
                    nowTurnChar_ = CharcterNum.UNI;
                }
            }
        }

        //Debug.Log(nowTurnChar_ + "の攻撃");
    }

    void AttackStart(int charNum)
    {
        // 敵の位置を取得する
        var enePos = buttleEnemySelect_.GetSelectEnemyPos();
        enePos.y = 0.0f;        // ここで0.0fにしないと斜め上方向に飛んでしまう

        // 通常攻撃弾の方向の計算
        var dir = (enePos - charSetting[charNum].buttlePos).normalized;

        // 行動中のキャラが、攻撃対象の方向に体を向ける
        // charMap_の情報を直接変更する必要があるため、charMap_[nowTurnChar_]と記述している
        charMap_[nowTurnChar_].transform.localRotation = Quaternion.LookRotation(enePos - charSetting[charNum].buttlePos);

        // エフェクトの発生位置高さ調整
        var adjustPos = new Vector3(charSetting[charNum].buttlePos.x, charSetting[charNum].buttlePos.y + 0.5f, charSetting[charNum].buttlePos.z);

        //　通常攻撃弾プレハブをインスタンス化
        //var uniAttackInstance = Instantiate(uniAttackPrefab_, transform.position + transform.forward, Quaternion.identity);
        var uniAttackInstance = Instantiate(uniAttackPrefab_, adjustPos + transform.forward, Quaternion.identity);

        MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
        //　通常攻撃弾の飛んでいく方向を指定
        //magicMove.SetDirection(transform.forward);
        magicMove.SetDirection(dir);

        // 選択した敵の番号を渡す
        magicMove.SetTargetNum(buttleEnemySelect_.GetSelectNum() + 1);

        // 矢印位置のリセットを行う
        buttleEnemySelect_.ResetSelectPoint();
    }
}
