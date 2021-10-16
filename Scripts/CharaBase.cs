using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CharaBase : object
{
    private GameObject DataPopPrefab_;

    // 番号
    private SceneMng.CHARACTERNUM charNum_;

    public struct CharacterSetting
    {
        public Animator animator;   // 各キャラについているAnimatorのコンポーネント
        public bool isMove;         // Waitモーション時はfalse
        public float animTime;      // 次のキャラの行動に遷移するまでの間
        public Vector3 buttlePos;   // 戦闘開始時に設定されるポジション(攻撃エフェクト等のInstance位置に利用)

        public string name;

        // Excel読み込みで得るデータ(最終的にはセーブ/ロード用に構造体分けたほうがいいかも)
        // 構造体分けたら、構造体(CharacterSetting)の中に構造体(新規)を入れる感じにする
        public int Level;
        public int HP;
        public int MP;
        public int Attack;          // この値にポイントを振り分けると攻撃力が上がる
        public int Defence;         // この値にポイントを振り分けると防御力が上がる
        public int Speed;           // この値にポイントを振り分けると素早さが上がる
        public int Luck;            // この値にポイントを振り分けると幸運が上がる
        public float AnimMax;       // 攻撃モーションのフレームを時間に直した値が入っている(モーション切り替えで使用する)
    }

    private CharacterSetting setting_;

    public CharaBase(string name, SceneMng.CHARACTERNUM num, Animator animator)
    {
        setting_.name = name;

        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        var popCharacter = DataPopPrefab_.GetComponent<PopList>().GetData<CharacterList>(PopList.ListData.CHARACTER, 0, name);
        setting_.Level = popCharacter.param[0].Level;
        setting_.HP = popCharacter.param[0].HP;
        setting_.MP = popCharacter.param[0].MP;
        setting_.Attack  = popCharacter.param[0].Attack;
        setting_.Defence = popCharacter.param[0].Defence;
        setting_.Speed   = popCharacter.param[0].Speed;
        setting_.Luck    = popCharacter.param[0].Luck;
        setting_.AnimMax = popCharacter.param[0].AnimMax;

        charNum_ = num;

        setting_.animator  = animator;
        setting_.isMove    = false;
        setting_.animTime  = 0.0f;
        //setting.buttlePos = set.buttlePos;    // 設定のタイミングが異なる
    }

    public SceneMng.CHARACTERNUM GetNum()
    {
        return charNum_;
    }

    public CharacterSetting GetSetting()
    {
        return setting_;
    }

    // 抽象関数(中身は継承先で実装する必要がある）
    public abstract void LevelUp();
    public abstract void Weapon();
    public abstract void Defence();
    public abstract void Magic();
    public abstract void Item();
    public abstract bool ChangeNextChara(); // 次のキャラの操作に切り替える為の準備処理

    //// override出来る関数
    //public virtual void Talk(string talk)
    //{
    //    Debug.Log("これから話します");
    //}

    //// 抽象関数(中身は継承先で実装する必要がある）
    //public abstract void Walk();
}