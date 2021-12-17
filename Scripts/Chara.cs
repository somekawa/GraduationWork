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

    private int[] statusUp = new int[5];            // 一時アップの数値を保存する用

    // name,num,animatorは親クラスのコンストラクタを呼び出して設定
    // numは、CharacterNumのenumの取得で使えそうかもだから用意してみた。使わなかったら削除する。
    public Chara(string name, int objNum, Animator animator) : base(name,objNum,animator,null)
    {
        set_ = GetSetting();  // CharaBase.csからGet関数で初期設定する

        // 数字の初期化
        for(int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = 0;
        }
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
        return set_.Attack;
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

        if (distance <= 0.15f)  // 目標地点0.0fにすると誤差で辿り着けなくなるから0.15fにしている
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

    // CharaBaseクラスの抽象メソッドを実装する
    public override void LevelUp()
    {
        set_.Level += 1;
        set_.HP += 10;
        set_.MP += 5;
        set_.Attack += 3;
        set_.MagicAttack += 2;
        set_.Defence += 3;
        set_.Speed += 2;
        set_.Luck += 1;

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
        return set_.Defence + barrierNum_;
    }

    public override int MagicPower()
    {
        return set_.MagicAttack;
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
        set_.Attack = set.Attack;
        set_.MagicAttack = set.MagicAttack;
        set_.Defence = set.Defence;
        set_.Speed = set.Speed;
        set_.Luck = set.Luck;
        set_.AnimMax = set.AnimMax;
        set_.Magic = set.Magic;

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

        // 一時アップの数字保存
        for (int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = num[i];
        }
    }

    public void DeleteStatusUpByCook()
    {
        // 一時アップ分、各ステータスからマイナスする
        set_.Attack -= statusUp[0];
        set_.MagicAttack -= statusUp[1];
        set_.Defence -= statusUp[2];
        set_.Speed -= statusUp[3];
        set_.Luck -= statusUp[4];

        // 一時アップの数字を初期化する
        for (int i = 0; i < statusUp.Length; i++)
        {
            statusUp[i] = 0;
        }
    }

    public int Speed()
    {
        return set_.Speed;
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
        // 死亡アニメーションに切り替える
        set_.animator.SetBool(key_isDeath, true);
        deathFlg_ = flag;
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
        if (num.Item1 >= 0 &&  hitNum > set_.Luck + Random.Range(0, 100))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item1 - 1].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("キャラは状態異常にかかった");
        }

        if (num.Item2 >= 0 && hitNum > set_.Luck + Random.Range(0, 100))
        {
            // 該当するバッドステータスをtrueにする
            set_.condition[num.Item2 - 1].Item2 = true;
            // NONをfalseにする
            set_.condition[(int)CONDITION.NON - 1].Item2 = false;
            Debug.Log("キャラは状態異常にかかった");
        }
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
}
