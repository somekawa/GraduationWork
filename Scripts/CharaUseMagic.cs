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

    private int mp_;
    private string magicPrefabNum_;    // 10の桁->効果範囲(ヘッド),1の桁->エレメント
    private int headNum_;              // ヘッド番号

    private ButtleMng buttleMng_;
    private EnemySelect enemySelect_;

    public void Init()
    {
        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
        enemySelect_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>();
    }

    public void CheckUseMagic(Bag_Magic.MagicData magicData)
    {
        // リストの初期化
        list_.Clear();

        // 攻撃系か回復系かで分ける
        if (magicData.element == 0)
        {
            // 回復系
            enemySelect_.SetAllActive(false, false);

            //@ 不足処理あり
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
            buttleMng_.SetDamageNum(magicData.power);
            // 攻撃属性をButtleMng.csに渡す
            buttleMng_.SetElement(magicData.element);
        }

        mp_ = magicData.rate;
        magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
        headNum_ = magicData.head;
    }

    public int MPdecrease()
    {
        return mp_; // 消費MP情報
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

        // 全ての発動フラグがtrueになったらwhileを抜けるようにしたい

        while(!tmpFlg)
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

                    // 弾の方向の計算
                    var dir = (list_[t].Item2.enePos - list_[t].Item2.charaPos).normalized;
                    // エフェクトの発生位置高さ調整
                    Vector3 adjustPos;
                    
                    // 威力が大の攻撃魔法なら、敵の頭上にエフェクトをインスタンスさせる
                    if(int.Parse(magicPrefabNum_.Split('-')[1]) == 2)
                    {
                        adjustPos = new Vector3(list_[t].Item2.enePos.x, list_[t].Item2.enePos.y, list_[t].Item2.enePos.z);
                    }
                    else
                    {
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y + 0.5f, list_[t].Item2.charaPos.z);
                    }

                    // 魔法プレハブをインスタンス化(現在は指定した魔法のみ)
                    GameObject obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;

                    //var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity); // 回転座標が全て0になるver.
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
                            weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(list_[t].Item2.targetNum + 1);

                            // 生成した魔法オブジェクト名を[エレメント番号-効果量番号(Clone)-魔法生成番号]に変更する
                            weaponTagObj[i].name = magicPrefabNum_ + "(Clone)-" + list_[t].Item2.instanceNum;

                            // 敵ヒット時の削除対象として名前を入れる
                            weaponTagObj[i].GetComponent<CheckAttackHit>().SetCharaMagicStr(weaponTagObj[i].name);
                        }
                    }
                }
            }
        }
    }
}
