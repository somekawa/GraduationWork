using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//　抽象クラス(CharaBase)とインタフェース(InterfaceButtle)を継承してCharaクラスを作成
public class Chara : CharaBase,InterfaceButtle
{
    private const string key_isAttack = "isAttack"; // 攻撃モーション(全キャラで名前を揃える必要がある)
    private CharacterSetting set_;                  // キャラ毎の情報            

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Chara(string name, CharacterMng.CharcterNum num, Animator animator) : base(name,num,animator)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する

        // データ書き出しテスト
        //StreamWriter swLEyeLog;
        //FileInfo fiLEyeLog;
        //fiLEyeLog = new FileInfo(Application.dataPath + "/test.csv");
        //swLEyeLog = fiLEyeLog.AppendText();
        //swLEyeLog.Write(set_.HP);   // 書込
        //swLEyeLog.Write(", ");
        //swLEyeLog.Flush();
        //swLEyeLog.Close();
    }

    public bool Attack()
    {
        Debug.Log(GetName() + "の攻撃！");
        set_.animator.SetBool(key_isAttack, true);
        // isMoveがfalseのときだけ攻撃エフェクトのInstanceとisMoveのtrue化処理を行うようにして、
        // エフェクトがボタン連打で大量発生するのを防ぐ
        if (!set_.isMove)
        {
            //AttackStart((int)nowTurnChar_); // characterMng.cs側でやる
            set_.isMove = true;
            //selectFlg_ = false; // characterMng.cs側でやる
            return true;
        }
        return false;
    }

    public void Damage()
    {
        Debug.Log("ダメージ受けた！");
    }

    public void HP()
    {
        Debug.Log("HP！");
    }

    // CharaBaseクラスの抽象メソッドを実装する
    public override void LevelUp()
    {
        Debug.Log("レベルアップ！");
    }

    public override void Weapon()
    {
        Debug.Log("武器切替！");
    }
    public override void Defence()
    {
        Debug.Log("防御！");
    }
    public override void Magic()
    {
        Debug.Log("魔法！");
    }
    public override void Item()
    {
        Debug.Log("アイテム！");
    }

    public override bool ChangeNextChara()
    {
        set_.animator.SetBool(key_isAttack, false);

        if (!set_.isMove)
        {
            return false;
        }

        // ここから下は、isMoveがtrueの状態
        // isMoveがtrueなら、さっきまで攻撃モーションだったことがわかる
        // 再生時間用に間を空ける
        // キャラ毎に必要な間が違うかもしれないから、1.0fの所は外部データ読み込みで、charSetting[(int)nowTurnChar_].maxAnimTimeとかを用意したほうがいい
        if (set_.animTime < 1.0f)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            // animTime値が1.0を上回ったとき
            // animTime値の初期化と、モーションがWaitになった為isMoveをfalseへ戻す
            set_.animTime = 0.0f;
            set_.isMove = false;

            return true;

            // CharacterMng.cs側でやる
            // 次のキャラが行動できるようにする
            // 最大まで加算されたら、初期値に戻す(前演算子重要)
            //if (++nowTurnChar_ >= CharcterNum.MAX)
            //{
            //    nowTurnChar_ = CharcterNum.UNI;
            //}
        }

        return false;
    }

    public Vector3 GetButtlePos()
    {
        return set_.buttlePos;
    }

    public void SetButtlePos(Vector3 pos)
    {
        set_.buttlePos = pos;
    }

}
