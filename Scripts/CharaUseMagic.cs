using UnityEngine;

// キャラクターが使用する魔法の条件等を管理するクラス
public class CharaUseMagic : MonoBehaviour
{
    private int mp_;
    private string magicPrefabNum_;    // 10の桁->効果範囲(ヘッド),1の桁->エレメント

    public void CheckUseMagic(Bag_Magic.MagicData magicData)
    {
        // 攻撃系か回復系かで分ける
        if(magicData.element == 0)
        {
            // 回復系
            GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
            GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(false,false);
            GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);

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
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetActive(true);
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                case 1:     // 複数回
                            // 全ての敵選択マークを表示へ(ただし実際の攻撃は誰に何回あたるか不明)
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(true,true);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                case 2:     // 全体
                            // 全ての敵選択マークを表示へ
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(true,false);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                default:
                    Debug.Log("不明なヘッドワードです");
                    break;
            }

            // 攻撃威力をButtleMng.csに渡す
            GameObject.Find("ButtleMng").GetComponent<ButtleMng>().SetDamageNum(magicData.power);
            // 攻撃属性をButtleMng.csに渡す
            GameObject.Find("ButtleMng").GetComponent<ButtleMng>().SetElement(magicData.element);
        }

        mp_ = magicData.rate;
        magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
    }

    public int MPdecrease()
    {
        return mp_; // 消費MP情報
    }

    public void MagicEffect(Vector3 charaPos,Vector3 enePos,int num)
    {
        // 通常攻撃弾の方向の計算
        var dir = (enePos - charaPos).normalized;
        // エフェクトの発生位置高さ調整
        var adjustPos = new Vector3(charaPos.x, charaPos.y + 0.5f, charaPos.z);

        // 通常攻撃弾プレハブをインスタンス化(現在は指定した魔法のみ)
        GameObject obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
        var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity);
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
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(num + 1);
            }
        }

    }
}
