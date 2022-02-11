using System.Collections.Generic;
using UnityEngine;

//　抽象クラス(CharaBase)とインタフェース(InterfaceButtle)を継承してCharaクラスを作成
public class Chara : CharaBase,InterfaceButtle
{
    private int key_isAttack = Animator.StringToHash("isAttack");// 攻撃モーション(全キャラで名前を揃える必要がある)
    private int key_isRun = Animator.StringToHash("isRun");      // 移動モーション(全キャラで名前を揃える必要がある)
    private int key_isDamage = Animator.StringToHash("isDamage");// ダメージモーション(全キャラで名前を揃える必要がある)
    private int key_isDeath = Animator.StringToHash("isDeath");  // 死亡モーション(全キャラで名前を揃える必要がある)

    private CharacterSetting set_;                  // キャラ毎の情報   
    private int barrierNum_ = 0;                    // 防御時に値が入る
    private bool deathFlg_ = false;                 // 死亡状態か確認する変数

    //private int[] statusUp = new int[8];            // 一時アップの数値を保存する用
    private int[] saveKeep_ = new int[8];           // セーブ時に、一時アップの数字を避難させる用

    private readonly int[] statusMap_ = new int[4];
    private Dictionary<int, (int, int)> buffMap_
        = new Dictionary<int, (int, int)>();        // バフ後の値とターン数を管理する<ワード順,(効果値,バフターン数)>

    // 特殊バフ(基本上書き,発動後はNONに戻す)
    public enum SPECIALBUFF
    {
        NON = 1,    // 無し
        REF,        // 物理反射
        REF_M,      // 魔法反射
        ABS,        // 物理吸収
        ABS_M       // 魔法反射
    }

    private SPECIALBUFF spBuff_ = SPECIALBUFF.NON;

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Chara(string name, int objNum, Animator animator) : base(name,objNum,animator,null)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する

        // 数字の初期化
        //for (int i = 0; i < statusUp.Length; i++)
        //{
        //    saveKeep_[i] = 0;
        //    statusUp[i] = 0;
        //}

        //SetStatusUpByCook(GameObject.Find("SceneMng").GetComponent<SaveLoadCSV>().StatusNum());
    }

    public bool Attack()
    {
        Debug.Log(set_.name + "の攻撃！");
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

    public int Damage()
    {
        return buffMap_[1].Item1;
        //return set_.Attack;
    }

    public int HP()
    {
        return set_.HP;
    }

    public int MaxHP()
    {
        return set_.maxHP;
    }

    public int MP()
    {
        return set_.MP;
    }

    public int MaxMP()
    {
        return set_.maxMP;
    }

    public (Vector3,bool) RunMove(float time,Vector3 myPos, Vector3 targetPos)
    {
        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 1.5f)   // 目標地点0.0fにすると敵と衝突してお互い吹っ飛んでいくから1.5fにしている
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("ジャック目的敵到着");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, time), true);
        }
        else
        {
            // キャラの座標移動
            return (Vector3.Lerp(myPos, targetPos, time), false);
        }
    }

    public (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos)
    {
        if (!set_.animator.GetBool(key_isRun))
        {
            set_.animator.SetBool(key_isRun, true);
        }

        float distance = Vector3.Distance(myPos, targetPos);

        if (distance <= 0.5f)  // 目標地点0.0fにすると誤差で辿り着けなくなるから0.5fにしている
        {
            // 指定箇所まできたらtrueを返す
            Debug.Log("ジャック目的敵到着");
            set_.animator.SetBool(key_isRun, false);
            return (Vector3.Lerp(myPos, targetPos, 1.0f),true);
        }
        else
        {
            // キャラの座標移動
            return (Vector3.Lerp(myPos, targetPos, time),false);
        }
    }


    public void SetHP(int hp)
    {
        set_.HP = hp;

        // 最小値は0
        if(set_.HP < 0)
        {
            set_.HP = 0;
        }

        // 最大値はHPのMAX値
        if(set_.HP > set_.maxHP)
        {
            set_.HP = set_.maxHP;
        }
    }

    public void SetMP(int mp)
    {
        set_.MP = mp;
    }

    public int CharacterMaxExp()
    {
        return set_.CharacterMaxExp;
    }
    public int CharacterExp()
    {
        return set_.CharacterExp;
    }

    // CharaBaseクラスの抽象メソッドを実装する
    public override void LevelUp(int[] num)
    {
        set_.Attack += num[0];
        set_.MagicAttack += num[1];
        set_.Defence += num[2];
        set_.Speed += num[3];
        set_.Luck += num[4];
        set_.maxHP += num[5];
        set_.HP += num[5];
        set_.maxMP += num[6];
        set_.MP += num[6];
        set_.Level += num[7];
        if (num[7] != 0)
        {
            // レベルが上がった時しかMax値を代入しない
            set_.CharacterMaxExp = num[9];
        }
        // exp本購入は基本的にレベルが上がる1つ前まで上げる
        set_.CharacterExp = num[7] == 0 ? set_.CharacterMaxExp - 1 : num[8];
        Debug.Log(set_.CharacterExp + "           " + set_.CharacterMaxExp);
        Debug.Log("レベルアップ！");
    }

    public override void Weapon()
    {
        Debug.Log("武器切替！");
    }

    public override int Defence(bool flag)
    {
        // 被ダメージアニメーションを開始
        set_.animator.SetBool(key_isDamage, flag);
        return buffMap_[3].Item1 + barrierNum_;
        //return set_.Defence + barrierNum_;
    }

    public override int MagicPower()
    {
        return buffMap_[2].Item1;
        //return set_.MagicAttack;
    }
    public override void Item()
    {
        Debug.Log("アイテム！");
    }

    // 名称正しくないかも。攻撃Motion終了確認に使いたい
    public override bool ChangeNextChara()
    {
        if (!set_.isMove)
        {
            return false;
        }

        // ここから下は、isMoveがtrueの状態
        // isMoveがtrueなら、さっきまで攻撃モーションだったことがわかる
        // 再生時間用に間を空ける
        // キャラ毎に必要な間が違うかもしれないから、1.0fの所は外部データ読み込みで、charSetting[(int)nowTurnChar_].maxAnimTimeとかを用意したほうがいい
        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
            return false;
        }
        else
        {
            // animTime値が1.0を上回ったとき
            // animTime値の初期化と、攻撃モーションの終了処理をしてisMoveをfalseへ戻す
            set_.animTime = 0.0f;
            set_.isMove = false;
            set_.animator.SetBool(key_isAttack, false);

            return true;

            // CharacterMng.cs側でやる
            // 次のキャラが行動できるようにする
            // 最大まで加算されたら、初期値に戻す(前演算子重要)
            //if (++nowTurnChar_ >= CharcterNum.MAX)
            //{
            //    nowTurnChar_ = CharcterNum.UNI;
            //}
        }
    }

    public Vector3 GetButtlePos()
    {
        return set_.buttlePos;
    }

    public void SetButtlePos(Vector3 pos)
    {
        set_.buttlePos = pos;
    }

    // 戦闘の始めに初期化する
    public void SetTurnInit()
    {
        set_.isMove = false;
        set_.animTime = 0.0f;

        statusMap_[0] = set_.Attack;
        statusMap_[1] = set_.MagicAttack;
        statusMap_[2] = set_.Defence;
        statusMap_[3] = set_.Speed;

        buffMap_.Clear();
        for (int i = 0; i < statusMap_.Length; i++)
        {
            buffMap_.Add(i + 1, (statusMap_[i], -1));  // ワードの順番と揃える
        }
    }

    public bool GetIsMove()
    {
        return set_.isMove;
    }

    // SceneMng.csから呼び出す
    public CharacterSetting GetCharaSetting()
    {
        return set_;
    }

    // SceneMng.csから呼び出す
    public void SetCharaSetting(CharacterSetting set)
    {
        set_.name = set.name;
        set_.Level = set.Level;
        set_.HP = set.HP;
        set_.MP = set.MP;
        set_.maxHP = set.maxHP;
        set_.maxMP = set.maxMP;
        set_.Attack = set.Attack;
        set_.MagicAttack = set.MagicAttack;
        set_.Defence = set.Defence;
        set_.Speed = set.Speed;
        set_.Luck = set.Luck;
        set_.AnimMax = set.AnimMax;
        set_.Magic = set.Magic;
        set_.CharacterExp = set.CharacterExp;
        set_.CharacterMaxExp = set.CharacterMaxExp;
        set_.statusUp = set.statusUp;

        SetTurnInit();
        // アニメーターはセットしてはいけない
        //setting_.animator = animator;
    }

    // RestaurantMng.csで呼び出す(あくまでも一時的なUpだから、それぞれに加算した値を別の変数に入れておく)
    public void SetStatusUpByCook(int[] num)
    {
        set_.Attack += num[0];
        set_.MagicAttack += num[1];
        set_.Defence += num[2];
        set_.Speed += num[3];
        set_.Luck += num[4];
        set_.maxHP += num[5];
        set_.maxMP += num[6];
        set_.Exp += num[7];
        set_.HP += num[5];
        set_.MP += num[6];


        // 一時アップの数字保存
        for (int i = 0; i < set_.statusUp.Length; i++)
        {
            set_.statusUp[i] = num[i];
        }
    }

    public int[] GetStatusUpByCook(bool flag)
    {
        if(flag)
        {
            return set_.statusUp;
        }
        else
        {
            return saveKeep_;
        }
    }

    public void DeleteStatusUpByCook(bool loadFlag = false)
    {
        // 一時アップ分、各ステータスからマイナスする
        set_.Attack -= set_.statusUp[0];
        set_.MagicAttack -= set_.statusUp[1];
        set_.Defence -= set_.statusUp[2];
        set_.Speed -= set_.statusUp[3];
        set_.Luck -= set_.statusUp[4];
        set_.maxHP -= set_.statusUp[5];
        set_.maxMP -= set_.statusUp[6];
        set_.Exp -= set_.statusUp[7];
        set_.HP -= set_.statusUp[5];
        set_.MP -= set_.statusUp[6];

        if (loadFlag)
        {
            // 保存を別にうつして、一時アップの数字を初期化する
            for (int i = 0; i < set_.statusUp.Length; i++)
            {
                saveKeep_[i] = set_.statusUp[i];

                if(set_.name == "Jack")
                {
                    set_.statusUp[i] = 0;
                }
            }
        }
        else
        {
            // 一時アップの数字を初期化する
            for (int i = 0; i < set_.statusUp.Length; i++)
            {
                if (set_.name == "Jack")
                {
                    set_.statusUp[i] = 0;
                }
            }
        }
    }

    public int Speed()
    {
        return buffMap_[4].Item1;
        //return set_.Speed;
    }

    public int Luck()
    {
        return set_.Luck;
    }

    public string Name()
    {
        return set_.name;
    }

    public void SetBarrierNum(int num = 0)
    {
        barrierNum_ = num;
    }

    public void SetMagicNum(int arrayNum,int num = 0)
    {
        set_.Magic[arrayNum] = num;
    }



    public Bag_Magic.MagicData GetMagicNum(int arrayNum)
    {
        return Bag_Magic.data[set_.Magic[arrayNum]];
    }

    public Sprite GetMagicImage(int num)
    {
        // 魔法装備無し
        if(set_.Magic[num] == 0)
        {
            return null;
        }

        return ItemImageMng.spriteMap[ItemImageMng.IMAGE.MAGIC][Bag_Magic.data[set_.Magic[num]].element];
    }

    public bool CheckMagicNum(int num)
    {
        if(num < 0 || num >= 4)
        {
            return false;
        }

        return true;
    }

    public bool GetDeathFlg()
    {
        return deathFlg_;
    }

    public void SetDeathFlg(bool flag)
    {
        // アニメーション切り替え
        set_.animator.SetBool(key_isDeath, flag);
        deathFlg_ = flag;
    }

    public void ButtleInit(bool flag = true)
    {
        if(flag)
        {
            // 最初のモーション設定
            set_.animator.Play("Standing@loop");
        }

        // バステを全て消す
        ConditionReset(true);

        // バフを全て消す
        for (int i = 1; i <= buffMap_.Count; i++)
        {
            // もしバフが継続していたら
            if (buffMap_[i].Item2 > 0)
            {
                buffMap_[i] = (statusMap_[i - 1], -1);
            }
        }
    }

    public override void DamageAnim()
    {
        // そもそもダメージを受けてないときは処理を抜ける
        if (!set_.animator.GetBool(key_isDamage))
        {
            return;
        }

        // 時間でモーションを終了させるか判定する
        if (set_.animTime < set_.AnimMax)
        {
            set_.animTime += Time.deltaTime;
        }
        else
        {
            // 被ダメージアニメーションを終了
            set_.animator.SetBool(key_isDamage, false);
            set_.animTime = 0.0f;
        }
    }

    public override void SetBS((int, int) num,int hitNum)
    {
        // 何らかのバステ効果がある魔法があたる、かつ、敵の命中率がキャラの幸運値+ランダム値より高いとき
        if (num.Item1 > 0 &&  hitNum > set_.Luck + Random.Range(0, 100))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item1 - 1].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("キャラは状態異常にかかった");
        }

        if (num.Item2 > 0 && hitNum > set_.Luck + Random.Range(0, 100))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item2 - 4].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("キャラは状態異常にかかった");
        }

        Debug.Log("キャラは状態異常にかからなかった");
    }

    public override (CONDITION, bool)[] GetBS()
    {
        return set_.condition;
    }

    public override void SetSpeed(int num)
    {
        set_.Speed = num;
    }

    public override void ConditionReset(bool allReset, int targetReset = -1)
    {
        if(allReset)
        {
            set_.condition[(int)CONDITION.NON - 1].Item2 = true;
            set_.condition[(int)CONDITION.POISON - 1].Item2 = false;
            set_.condition[(int)CONDITION.DARK - 1].Item2 = false;
            set_.condition[(int)CONDITION.PARALYSIS - 1].Item2 = false;
            set_.condition[(int)CONDITION.DEATH - 1].Item2 = false;
        }
        else
        {
            // 特定の状態異常だけ回復する
            if (targetReset > -1)
            {
                set_.condition[targetReset].Item2 = false;
            }
        }
    }

    public override (int, bool) SetBuff(int tail, int buff)
    {
        bool flag = false;
        // バフ/デバフ内容を反映させる
        // まずは効果量の設定として、威力数字をつくる
        // 小(0)->2割,中(1)->4割,大(2)->6割とするには、tail+1*2にすればいい
        float alphaNum = (((float)buffMap_[buff].Item1 - (float)statusMap_[buff - 1]) / (float)statusMap_[buff - 1]) * 10.0f;   // バフの重ね掛け用
        float buffNum = ((tail + 1) * 2 + alphaNum) * 0.1f;
        float num = statusMap_[buff - 1] * buffNum;
        if (num <= 1.0f)
        {
            num = 1.0f;
        }

        // バフの重ね掛けかを残りの効果ターン数をみて判断する
        if(buffMap_[buff].Item2 > 0)
        {
            // 各項目に応じた上昇処理(固定で+1ターンの延長としておく)
            buffMap_[buff] = (statusMap_[buff - 1] + (int)num, buffMap_[buff].Item2 + 1);
            flag = true;
        }
        else
        {
            // 各項目に応じた上昇処理(固定で4ターン後回復としておく)
            buffMap_[buff] = (statusMap_[buff - 1] + (int)num, 4);
        }

        // (現在値 - 元値) / 元値 = 倍率 * 100%
        float tmp = (((float)buffMap_[buff].Item1 - (float)statusMap_[buff - 1]) / (float)statusMap_[buff - 1]) * 100.0f;

        // どの％領域にいるかで返す数字を変更する
        if(tmp > 0 && tmp <= 30)
        {
            tmp = 0;
        }
        else if(tmp > 30 && tmp <= 70)
        {
            tmp = 1;
        }
        else if(tmp > 70 && tmp <= 100)
        {
            tmp = 2;
        }
        else
        {
            tmp = -1;
        }

        return ((int)tmp,flag);
    }

    public override bool CheckBuffTurn()
    {
        bool flag = true;
        for (int i = 1; i <= buffMap_.Count; i++)
        {
            // もしバフが継続していたら
            if (buffMap_[i].Item2 > 0)
            {
                // -1ターンする
                buffMap_[i] = (buffMap_[i].Item1, buffMap_[i].Item2 - 1);

                if (buffMap_[i].Item2 <= 0)    // 先ほどの-1で0以下になったならば
                {
                    Debug.Log("味方のバフが解除されました");
                    buffMap_[i] = (statusMap_[i - 1], -1);
                    flag = false;
                }
            }
        }
        return flag;
    }

    // 反射か吸収のバフ処理(基本は効果上書き)
    public void SetReflectionOrAbsorption(int sub2,int num)
    {
        spBuff_ = (SPECIALBUFF)(sub2 + num);
    }

    // ダメージ処理時によびだされる
    public SPECIALBUFF GetReflectionOrAbsorption()
    {
        return spBuff_;
    }

    public override Dictionary<int, (int, int)> GetBuff()
    {
        return buffMap_;
    }

    public override int Level()
    {
        return set_.Level;
    }
}
