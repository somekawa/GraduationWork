using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    // フィールドのメニュー画面からのみ使用可能:0
    // 戦闘中のみ使用可能    :1
    // どちらでも使用可能    :2
    private byte[] item_ = new byte[38]{
            2,//0:HPポーション(小)
            1,//1:毒消し
            1,//2:攻撃アイテム(小全体)
            0,//3:カエレール
            1,//4:防御力アップ(単体)
            1,//5:物理/魔法攻撃力アップ(単体)
            1,//6:速度アップ(単体)
            1,//7:ニゲレール
            2,//8:HPポーション(中)
            2,//9:MPポーション(小)
            1,//10:暗闇消し
            1,//11:麻痺消し
            1,//12:攻撃アイテム(中全体)
            0,//13:エンカウント率(低下)
            1,//14:蘇生(HP小状態で)
            2,//15:HPポーション(大)
            2,//16:MPポーション(中)
            1,//17:即死身代わり
            1,//18:蘇生(HP全快)
            // ここから下は大成功処理---------------
            2,//19:HPポーション(小)
            1,//20:毒消し
            1,//21:攻撃アイテム(小全体)
            0,//22:カエレール
            1,//23:防御力アップ(単体)
            1,//24:物理/魔法攻撃力アップ(単体)
            1,//25:速度アップ(単体)
            1,//26:ニゲレール
            2,//27:HPポーション(中)
            2,//28:MPポーション(小)
            1,//29:暗闇消し
            1,//30:麻痺消し
            1,//31:攻撃アイテム(中全体)
            0,//32:エンカウント率(低下)
            1,//33:蘇生(HP小状態で)
            2,//34:HPポーション(大)
            2,//35:MPポーション(中)
            1,//36:即死身代わり
            1 //37:蘇生(HP全快)
    };

    private (int, int) hpmpNum_ = (0,0);
    private CharaBase.CONDITION condition_ = CharaBase.CONDITION.NON;
    private int itemNum_ = -1;
    private (int,int) buff_ = (-1,-1);
    private bool buffExItemFlg_ = false;    // 大成功アイテムならバフ量を増やす
    private GameObject charasText_;
    private string alive = "";

    // 画面を開いた時に1回と、回復毎に1回呼ぶ
    public void TextInit(Text[] text)
    {
        // キャラのステータス値を表示させたい
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            var data = SceneMng.GetCharasSettings(i);

            text[i].text = data.name + "\n" +
                     "HP " + data.HP.ToString() + "/" + data.maxHP.ToString() + "\n" +
                     "MP " + data.MP.ToString() + "/" + data.maxMP.ToString();
        }
    }

    // 返り値について…(bool->今使えるかどうか,bool->使う際に対象の指定が必要かどうか)
    // 2つ目がtrueになるタイミングは回復系のみ
    public (bool,bool) Use(int itemNum)
    {
        Debug.Log(itemNum + "番のアイテムを使用します");

        // 現在フィールドのメニュー画面ならば
        // item_変数が「１」のアイテムは使用不可
        if(FieldMng.nowMode  == FieldMng.MODE.MENU && item_[itemNum] == 1)
        {
            Debug.Log("戦闘中にしか使えないアイテムを使用しようとしました");
            return (false,false);
        }

        // 現在戦闘中もしくは強制戦闘中ならば
        // item_変数が「０」のアイテムは使用不可
        if ((FieldMng.nowMode == FieldMng.MODE.BUTTLE || FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE) &&
           item_[itemNum] == 0)
        {
            Debug.Log("探索中にしか使えないアイテムを使用しようとしました");
            return (false, false);
        }

        bool tmpFlg = false;
        switch (itemNum)
        {
            case 0:    // HPポーション(小)
                hpmpNum_ = (10, 0);
                tmpFlg = true;
                break;
            case 1:    // 毒消し
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 2:    // 全体小ダメージ
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(15);
                break;
            case 3:    // カエレール(フィールドからユニハウスへ)
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 4:    // 防御力アップ(単体)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 5:    // 物理/魔法攻撃力アップ(単体)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 6:    // 速度アップ(単体)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 7:    // ニゲレール(強制戦闘でない場合は使用可能とする)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    var tmp = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
                    tmp.CallDeleteEnemy();
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    SceneMng.charMap_[SceneMng.CHARACTERNUM.UNI].gameObject.transform.position = tmp.GetFieldPos();
                    // アイテム画面を閉じる
                    GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
                    Debug.Log("Uniは逃げ出した");
                }
                else
                {
                    return (false, tmpFlg);
                }
                break;
            case 8:    // HPポーション(中)
                hpmpNum_ = (40, 0);
                tmpFlg = true;
                break;
            case 9:    // MPポーション(小)
                hpmpNum_ = (0, 10);
                tmpFlg = true;
                break;
            case 10:    // 暗闇消し
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 11:    // 麻痺消し
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 12:    // 全体中ダメージ
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(30);
                break;
            case 13:    // エンカウント率低下
                //@ 現在のエンカウント色のまま、一定時間エンカウントしなくなるようにする
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 14:    // 蘇生(最大HPの半分)
                alive = "half";
                tmpFlg = true;
                break;
            case 15:    // HPポーション(大)
                hpmpNum_ = (100, 0);
                tmpFlg = true;
                break;
            case 16:    // MPポーション(中)
                hpmpNum_ = (0, 50);
                tmpFlg = true;
                break;
            case 17:    // 即死身代わり
                // 持っているだけで効果がでるから、ここに処理は書かない
                break;
            case 18:    // 蘇生(HP全快)
                alive = "all";
                tmpFlg = true;
                break;
                // ここから下は大成功処理--------------------------------------------------
            case 19:    // HPポーション(小)
                hpmpNum_ = (35, 0);
                tmpFlg = true;
                break;
            case 20:    // 毒消し
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 21:    // 全体小ダメージ
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(20);
                break;
            case 22:    // カエレール(フィールドからユニハウスへ)
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 23:    // 防御力アップ(単体)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 24:    // 物理/魔法攻撃力アップ(単体)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 25:    // 速度アップ(単体)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 26:    // ニゲレール(強制戦闘でない場合は使用可能とする)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    var tmp = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
                    tmp.CallDeleteEnemy();
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    SceneMng.charMap_[SceneMng.CHARACTERNUM.UNI].gameObject.transform.position = tmp.GetFieldPos();
                    // アイテム画面を閉じる
                    GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
                    Debug.Log("Uniは逃げ出した");
                }
                else
                {
                    return (false, tmpFlg);
                }
                break;
            case 27:    // HPポーション(中)
                hpmpNum_ = (65, 0);
                tmpFlg = true;
                break;
            case 28:    // MPポーション(小)
                hpmpNum_ = (0, 35);
                tmpFlg = true;
                break;
            case 29:    // 暗闇消し
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 30:    // 麻痺消し
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 31:    // 全体中ダメージ
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(45);
                break;
            case 32:    // エンカウント率低下
                //@ 現在のエンカウント色のまま、一定時間エンカウントしなくなるようにする
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 33:    // 蘇生(最大HPの半分)
                alive = "half";
                tmpFlg = true;
                break;
            case 34:    // HPポーション(大)
                hpmpNum_ = (130, 0);
                tmpFlg = true;
                break;
            case 35:    // MPポーション(中)
                hpmpNum_ = (0, 65);
                tmpFlg = true;
                break;
            case 36:    // 即死身代わり
                // 持っているだけで効果がでるから、ここに処理は書かない
                break;
            case 37:    // 蘇生(HP全快)
                alive = "all";
                tmpFlg = true;
                break;
        }

        itemNum_ = itemNum;
        return (true,tmpFlg);
    }

    public void OnClickCharaButton(int num)
    {
        if(!SceneMng.charasList_[num].GetDeathFlg())
        {
            // HPMP回復
            SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + hpmpNum_.Item1);
            SceneMng.charasList_[num].SetMP(SceneMng.charasList_[num].MP() + hpmpNum_.Item2);

            // 状態異常回復(-1したときに0より大きいときは状態異常回復する)
            if ((int)condition_ - 1 > 0)
            {
                SceneMng.charasList_[num].ConditionReset(false, (int)condition_ - 1);
                condition_ = CharaBase.CONDITION.NON;
            }

            // バフ処理(固定で中威力)
            // バフのアイコン処理
            System.Action<int, int, int> action = (int charaNum, int buffnum, int whatBuff) => {
                var bufftra = GameObject.Find("ButtleUICanvas/" + SceneMng.charasList_[charaNum].Name() + "CharaData/BuffImages").transform;
                for (int i = 0; i < bufftra.childCount; i++)
                {
                    if (bufftra.GetChild(i).GetComponent<Image>().sprite == null)
                    {
                        // アイコンをいれる
                        bufftra.GetChild(i).GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff - 1];
                        // 矢印でアップ倍率をいれる
                        // ▲*1 = バフが1% ~30%,▲*2 = バフが31% ~70%,▲*3 = バフが71%~100%
                        for (int m = 0; m < bufftra.GetChild(i).childCount; m++)
                        {
                            if (m <= buffnum)    // buffnumの数字以下ならtrueにして良い
                            {
                                bufftra.GetChild(i).GetChild(m).gameObject.SetActive(true);
                            }
                            else
                            {
                                bufftra.GetChild(i).GetChild(m).gameObject.SetActive(false);
                            }
                        }

                        break;
                    }
                }
            };

            // 物理攻撃と魔法攻撃の威力が同時に上がる事を考えて、pairにしている
            if (buff_.Item1 > 0)
            {
                if (buffExItemFlg_)
                {
                    // 大成功アイテム使用で効果量増加
                    var tmp = SceneMng.charasList_[num].SetBuff(2, buff_.Item1);
                    action(num, tmp.Item1, buff_.Item1);
                }
                else
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item1);
                    action(num, tmp.Item1, buff_.Item1);
                }
            }
            if (buff_.Item2 > 0)
            {
                if (buffExItemFlg_)
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(2, buff_.Item2);
                    action(num, tmp.Item1, buff_.Item2);
                }
                else
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item2);
                    action(num, tmp.Item1, buff_.Item2);
                }
            }
        }
        else
        {
            // 蘇生処理
            if (alive != "")
            {
                if (alive == "half")
                {
                    // 半回復状態で立ち上がらせる
                    SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + (SceneMng.charasList_[num].MaxHP() / 2));
                    SceneMng.charasList_[num].SetDeathFlg(false);   // 死亡状態を解除
                }
                else
                {
                    // 全回復状態で立ち上がらせる
                    SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + SceneMng.charasList_[num].MaxHP());
                    SceneMng.charasList_[num].SetDeathFlg(false);   // 死亡状態を解除
                }
            }
        }

        alive = ""; // 文字列初期化

        if (charasText_ == null)
        {
            charasText_ = GameObject.Find("ItemBagMng/CharasText").gameObject;
        }

        Text[] text = new Text[2];
        for (int i = 0; i < charasText_.transform.childCount; i++)
        {
            text[i] = charasText_.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }
        TextInit(text);

        Bag_Item.itemState[itemNum_].haveCnt--;
        Bag_Item.itemUseFlg = true;
        // 表示中の所持数を更新
        Bag_Item.itemState[itemNum_].cntText.text = Bag_Item.itemState[itemNum_].haveCnt.ToString();
        if (Bag_Item.itemState[itemNum_].haveCnt < 1)
        {
            // 所持数が0になったら非表示にする
            Bag_Item.itemState[itemNum_].box.SetActive(false);
        }
    }
}