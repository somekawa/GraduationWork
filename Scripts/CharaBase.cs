using UnityEngine;

public abstract class CharaBase : object
{
    private GameObject DataPopPrefab_;

    public struct CharacterSetting
    {
        public Animator animator;   // 各キャラについているAnimatorのコンポーネント
        public bool isMove;         // Waitモーション時はfalse
        public float animTime;      // 次のキャラの行動に遷移するまでの間
        public Vector3 buttlePos;   // 戦闘開始時に設定されるポジション(攻撃エフェクト等のInstance位置に利用)

        public string name;
        public int maxHP;           // 最大HP
        public int maxMP;           // 最大MP

        // Excel読み込みで得るデータ(最終的にはセーブ/ロード用に構造体分けたほうがいいかも)
        // 構造体分けたら、構造体(CharacterSetting)の中に構造体(新規)を入れる感じにする
        public int Level;
        public int HP;
        public int MP;
        public int Attack;          // この値にポイントを振り分けると攻撃力が上がる
        public int MagicAttack;
        public int Defence;         // この値にポイントを振り分けると防御力が上がる
        public int Speed;           // この値にポイントを振り分けると素早さが上がる
        public int Luck;            // この値にポイントを振り分けると幸運が上がる
        public float AnimMax;       // 攻撃モーションのフレームを時間に直した値が入っている(モーション切り替えで使用する)
        public int[] Magic;
        public int CharacterExp;// 取得経験値
        public int CharacterMaxExp;// 取得経験値
        public int CharacterSumExp;// 取得経験値
        public (CONDITION,bool)[] condition; // 各キャラと各敵の状態異常が確認できるようにする

        // 敵用の情報
        public int Exp;             // この敵を倒した際にキャラが得られる経験値
        public string Drop;         // この敵を倒した際にキャラが得られるドロップ素材
        public float MoveTime;      // 移動時にdeltaTimeを割る値
        public float MoveDistance;  // キャラとの距離許容範囲
        public string WeaponTagObjName;  // CheckAttackHit.csがアタッチされているオブジェクトの名前
        public int Weak;            // この敵の弱点属性
        public int Bst;             // この敵が発動できる状態異常効果
    }

    public enum ANIMATION
    {
        NON,
        IDLE,
        BEFORE,
        ATTACK,
        AFTER,
        //DEATH
    };

    // 状態異常系(magicData.csvと合わせる為にNONを1からスタートしている)
    public enum CONDITION
    {
        NON = 1,    // 健康
        POISON,     // 毒
        DARK,       // 暗闇
        PARALYSIS,  // 麻痺
        DEATH,      // 即死
    }

    private CharacterSetting setting_;

    public CharaBase(string name,int objNum, Animator animator,EnemyList.Param param)
    {
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        if(objNum == 0)
        {
            // キャラクター
            var popCharacter = DataPopPrefab_.GetComponent<PopList>().GetData<CharacterList>(PopList.ListData.CHARACTER, 0, name);
            setting_.name = name;
            setting_.Level = popCharacter.param[0].Level;
            setting_.HP = popCharacter.param[0].HP;
            setting_.maxHP = popCharacter.param[0].MaxHP;
            setting_.MP = popCharacter.param[0].MP;
            setting_.maxMP = popCharacter.param[0].MaxMP;
            setting_.Attack = popCharacter.param[0].Attack;
            setting_.MagicAttack = popCharacter.param[0].MagicAttack;
            setting_.Defence = popCharacter.param[0].Defence;
            setting_.Speed = popCharacter.param[0].Speed;
            setting_.Luck = popCharacter.param[0].Luck;
            setting_.AnimMax = popCharacter.param[0].AnimMax;
            setting_.animator = animator;
            setting_.isMove = false;
            setting_.animTime = 0.0f;
            //setting.buttlePos = set.buttlePos;    // 設定のタイミングが異なる

            setting_.CharacterExp = popCharacter.param[0].Exp;
            setting_.CharacterMaxExp = popCharacter.param[0].MaxExp;
            setting_.CharacterSumExp = popCharacter.param[0].SumExp;
            

            // 最大4つまでつけられるのでここで初期化しておく
            setting_.Magic = new int[4];
        }
        else
        {
            // 敵情報
            setting_.name = param.Name + "_" +name;
            setting_.Level = param.Level;
            setting_.HP = param.HP;
            setting_.MP = param.MP;
            setting_.Attack = param.Attack;
            setting_.MagicAttack = param.MagicAttack;
            setting_.Defence = param.Defence;
            setting_.Speed = param.Speed;
            setting_.Luck = param.Luck;
            setting_.AnimMax = param.AnimMax;

            setting_.Exp = param.Exp;
            setting_.Drop = param.Drop;
            setting_.MoveTime = param.MoveTime;
            setting_.MoveDistance = param.MoveDistance;
            setting_.WeaponTagObjName = param.WeaponTagObjName;
            setting_.Weak = param.Weak;
            setting_.Bst = param.Bst;

            setting_.animator = animator;
            setting_.isMove = false;
            setting_.animTime = 0.0f;
            //setting.buttlePos = set.buttlePos;    // 設定のタイミングが異なる

            setting_.maxHP = setting_.HP;
            setting_.maxMP = setting_.MP;
        }

        // 最初は敵もキャラも共通で健康
        setting_.condition = new (CONDITION, bool)[(int)CONDITION.DEATH]
        {
            (CONDITION.NON,true),
            (CONDITION.POISON,false),
            (CONDITION.DARK,false),
            (CONDITION.PARALYSIS,false),
            (CONDITION.DEATH,false),
        };
    }

    public CharacterSetting GetSetting()
    {
        return setting_;
    }

    // 抽象関数(中身は継承先で実装する必要がある）
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract int Defence(bool flag);
    public abstract int MagicPower();
    public abstract void Item();
    public abstract bool ChangeNextChara(); // 次のキャラの操作に切り替える為の準備処理
    public abstract void DamageAnim();      // 攻撃を受けたときのモーション処理
    public abstract void SetBS((int, int) num, int hitNum);    // 状態異常の設定
    public abstract void ConditionReset(bool allReset,int targetReset = -1);  // 状態異常をリセットする
    public abstract (CONDITION, bool)[] GetBS();   // 自分の状態異常を取得
    public abstract void SetSpeed(int num);
    public abstract (int,bool) SetBuff(int tail,int buff);
    public abstract System.Collections.Generic.Dictionary<int, (int, int)> GetBuff();
    public abstract bool CheckBuffTurn();
    public abstract int Level();

    //// override出来る関数
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("これから話します");
    //}

    //// 抽象関数(中身は継承先で実装する必要がある）
    //public abstract void Walk();
}