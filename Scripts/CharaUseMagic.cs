using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラクターが使用する魔法の条件等を管理するクラス
public class CharaUseMagic : MonoBehaviour
{
    struct MagicAttackInfo
    {
        public Vector3 charaPos;    // キャラクター座標
        public Vector3 enePos;      // 敵座標
        public int targetNum;       // 魔法対象番号
        public int instanceNum;     // 魔法生成番号
        public float instanceTime;  // 魔法生成予定時間
    }

    // 魔法生成用リスト(bool->魔法生成したらtrueにする)
    private List<(bool,MagicAttackInfo)> list_ = new List<(bool,MagicAttackInfo)>();

    private string magicPrefabNum_;    
    private int headNum_;              
    private int elementNum_;
    private int tailNum_;
    private int sub1Num_;
    private int healPower_;            // 回復魔法の威力

    private ButtleMng buttleMng_;
    private EnemySelect enemySelect_;
    private CharacterMng characterMng_;

    public void Init()
    {
        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
        enemySelect_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>();
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
    }

    public void CheckUseMagic(Bag_Magic.MagicData magicData, int charaMagicPower)
    {
        // リストの初期化
        list_.Clear();

        // 最初に代入しておかないと、switch文のbreakで抜けて代入処理が追い付かなくなる
        headNum_ = magicData.head;
        elementNum_ = magicData.element;
        tailNum_ = magicData.tail;
        sub1Num_ = magicData.sub1;

        // 攻撃系か回復系かで分ける
        if (magicData.element == 0)
        {
            // 回復系
            enemySelect_.SetAllActive(false, false);
   
            // ヘッドワードの種類分け
            switch (magicData.head)
            {
                case 0:     // 単体
                    characterMng_.SetCharaArrowActive(false,false,magicData.sub2, magicData.sub3);  
                    break;
                case 1:     // 複数回
                    characterMng_.SetCharaArrowActive(true,true, magicData.sub2, magicData.sub3);
                    break;
                case 2:     // 全体
                    characterMng_.SetCharaArrowActive(true,false, magicData.sub2, magicData.sub3);
                    break;
                default:
                    Debug.Log("不明なヘッドワードです");
                    break;
            }

            // キャラの魔力依存の回復値を保存しておく
            healPower_ = magicData.power + (charaMagicPower / 2);

            if(magicData.sub2 == 0)
            {
                // [エレメント-威力-何を回復するか(sub2が0かそれ以外かで判断させたい)]
                magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString() + "-" + magicData.sub2.ToString();
            }
            else
            {
                magicPrefabNum_ = "0-" + magicData.tail.ToString() + "-9";
            }
        }
        else if(magicData.element == 1)
        {
            // 補助系

            // 敵へのデバフ
            if(magicData.sub1 == 1)
            {
                // ヘッドワードの種類分け
                switch (magicData.head)
                {
                    case 0:     // 単体
                        enemySelect_.SetActive(true);
                        break;
                    case 1:     // 複数回
                        enemySelect_.SetAllActive(true, true);
                        break;
                    case 2:     // 全体
                        enemySelect_.SetAllActive(true, false);
                        break;
                    default:
                        Debug.Log("不明なヘッドワードです");
                        break;
                }

                magicPrefabNum_ = magicData.tail + "-" + magicData.sub1.ToString() + "-" + magicData.sub2.ToString() +"-" + magicData.sub3.ToString();
            }
            else if(magicData.sub1 == 0)
            {
                // 味方へのバフ
                // ヘッドワードの種類分け
                switch (magicData.head)
                {
                    case 0:     // 単体
                        characterMng_.SetCharaArrowActive(false, false, magicData.sub2, magicData.sub3);
                        break;
                    case 1:     // 複数回
                        characterMng_.SetCharaArrowActive(true, true, magicData.sub2, magicData.sub3);
                        break;
                    case 2:     // 全体
                        characterMng_.SetCharaArrowActive(true, false, magicData.sub2, magicData.sub3);
                        break;
                    default:
                        Debug.Log("不明なヘッドワードです");
                        break;
                }

                magicPrefabNum_ = magicData.tail + "-" + magicData.sub1.ToString() + "-" + magicData.sub2.ToString() + "-" + magicData.sub3.ToString();
            }
            else
            {
                // 何も処理を行わない
            }
        }
        else
        {
            // 攻撃系
            // ヘッドワードの種類分け
            switch (magicData.head)
            {
                case 0:     // 単体
                            // 敵選択処理へ
                    enemySelect_.SetActive(true);
                    break;
                case 1:     // 複数回
                            // 全ての敵選択マークを表示へ(ただし実際の攻撃は誰に何回あたるか不明)
                    enemySelect_.SetAllActive(true,true);
                    break;
                case 2:     // 全体
                            // 全ての敵選択マークを表示へ
                    enemySelect_.SetAllActive(true, false);
                    break;
                default:
                    Debug.Log("不明なヘッドワードです");
                    break;
            }

            // 攻撃威力をButtleMng.csに渡す
            buttleMng_.SetDamageNum(magicData.power + charaMagicPower);
            // 攻撃属性をButtleMng.csに渡す
            buttleMng_.SetElement(magicData.element);
            // 状態異常をButtleMng.csに渡す
            buttleMng_.SetBadStatus(magicData.sub1, magicData.sub2);

            magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
        }
    }

    public int MPdecrease(Bag_Magic.MagicData magicData)
    {
        return magicData.rate; // 消費MP情報
    }

    public int GetHealPower()
    {
        return healPower_;
    }

    public int GetTailNum()
    {
        return tailNum_;
    }

    public (int,int) GetElementAndSub1Num()
    {
        return (elementNum_,sub1Num_);
    }

    public void InstanceMagicInfo(Vector3 charaPos,Vector3 enePos,int targetNum,int instanceNum)
    {
        // 情報の設定
        MagicAttackInfo tmpInfo = new MagicAttackInfo();
        tmpInfo.charaPos = charaPos;
        tmpInfo.enePos = enePos;
        tmpInfo.targetNum = targetNum;
        tmpInfo.instanceNum = instanceNum;

        if(headNum_ == 1)   // 複数回攻撃の場合
        {
            tmpInfo.instanceTime = instanceNum * 0.2f;  // 0.2秒ずつ発動時間をずらす
        }
        else
        {
            tmpInfo.instanceTime = 0.0f;    // 同時に発動する
        }

        list_.Add((false,tmpInfo));
    }

    // 引数の内容をstructにして発動時間まで保存させたあと、
    // 発動時間になったらそのstructの情報を使って生成
    public IEnumerator InstanceMagicCoroutine()
    {
        float time = 0.0f;
        bool tmpFlg = false;
        var split = magicPrefabNum_.Split('-');

        // 全ての発動フラグがtrueになったらwhileを抜けるようにしたい

        while (!tmpFlg)
        {
            yield return null;

            // フラグの状態は、先にtrueにしておいてfor文で1つでも未発動の魔法があればfalseになるようにする
            tmpFlg = true;
            for(int k = 0; k < list_.Count; k++)
            {
                if(!list_[k].Item1)
                {
                    // 未発動の魔法あり
                    tmpFlg = false;
                    break;
                }
            }

            time += Time.deltaTime;
            //Debug.Log("時間計測" + time);

            for (int t = 0; t < list_.Count; t++)
            {
                if(!list_[t].Item1 && list_[t].Item2.instanceTime <= time)  // 発動時間になっている
                {
                    // 発動確認フラグをtrueにする(一時変数に入れて更新する)
                    (bool, MagicAttackInfo) tmp = (true, list_[t].Item2);
                    list_[t] = tmp;

                    // エフェクトの発生位置高さ調整
                    Vector3 adjustPos;

                    // magicPrefabNum_変数をハイフンで分割したときに2つに分けれたら(=攻撃魔法)
                    if (split.Length == 2)
                    {
                        // 威力が大以上の攻撃魔法なら(極大含む)、敵の頭上にエフェクトをインスタンスさせる
                        if (int.Parse(split[1]) >= 2)
                        {
                            adjustPos = new Vector3(list_[t].Item2.enePos.x, list_[t].Item2.enePos.y, list_[t].Item2.enePos.z);
                        }
                        else
                        {
                            adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y + 0.5f, list_[t].Item2.charaPos.z);
                        }

                        Common(t, adjustPos,false);
                    }
                    else if(split.Length == 3)
                    {
                        //  3つに分かれているから回復魔法
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y, list_[t].Item2.charaPos.z);
                        Common(t, adjustPos, true);
                    }
                    else
                    {
                        // 4つに分かれているから補助魔法
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y, list_[t].Item2.charaPos.z);

                        // 味方用か敵用かsub1で見分ける
                        Common(t, adjustPos, split[1] == 0.ToString() ? true : false);
                    }
                }
            }
        }
    }

    private void Common(int t,Vector3 adjustPos,bool flag)
    {
        GameObject obj = null;
        if (flag)
        {
            var tmpStr = magicPrefabNum_;
            var split = tmpStr.Split('-');
            // 反射と吸収はエフェクトの数字設定
            if (split[3] == 2.ToString() && split.Length == 4)
            {
                // 反射魔法の時(同じエフェクトを使うから固定番号)
                obj = Resources.Load("MagicPrefabs/0-0-1-2") as GameObject;
            }
            else if (split[3] == 3.ToString() && split.Length == 4)
            {
                // 吸収魔法の時(同じエフェクトを使うから固定番号)
                obj = Resources.Load("MagicPrefabs/0-0-1-3") as GameObject;
            }
            else
            {
                // 魔法プレハブをインスタンス化
                obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
            }

            // プレハブの高さも含めた位置に生成する
            Instantiate(obj, adjustPos + obj.transform.position, obj.transform.rotation);
        }
        else
        {
            // 弾の方向の計算
            var dir = (list_[t].Item2.enePos - list_[t].Item2.charaPos).normalized;

            // 魔法プレハブをインスタンス化
            var tmpStr = magicPrefabNum_;
            var split = tmpStr.Split('-');

            if (split[1] == 1.ToString() && split.Length == 4)
            {
                // 敵へのデバフ魔法の時(同じエフェクトを使うから固定番号)
                obj = Resources.Load("MagicPrefabs/0-1-1-1") as GameObject;
            }
            else
            {
                obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
            }

            // プレハブの高さも含めた位置に生成する
            var uniAttackInstance = Instantiate(obj, adjustPos + obj.transform.position, obj.transform.rotation);

            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // 通常攻撃弾の飛んでいく方向を指定
            magicMove.SetDirection(dir);

            // [Weapon]のタグがついているオブジェクトを全て検索する
            var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
            for (int i = 0; i < weaponTagObj.Length; i++)
            {
                // 見つけたオブジェクトの名前を比較して、今回攻撃に扱う武器についているCheckAttackHit関数の設定を行う
                if (weaponTagObj[i].name == (magicPrefabNum_ + "(Clone)"))
                {
                    // 選択した敵の番号を渡す
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(list_[t].Item2.targetNum + 1,-1);

                    // 生成した魔法オブジェクト名を[エレメント番号-効果量番号(Clone)-魔法生成番号]に変更する
                    weaponTagObj[i].name = magicPrefabNum_ + "(Clone)-" + list_[t].Item2.instanceNum;

                    // 敵ヒット時の削除対象として名前を入れる
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetCharaMagicStr(weaponTagObj[i].name);
                }
            }
        }
    }
}
