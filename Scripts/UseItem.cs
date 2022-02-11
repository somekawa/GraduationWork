using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    // フィールドのメニュー画面からのみ使用可能:0
    // 戦闘中のみ使用可能    :1
    // どちらでも使用可能    :2
    // どちらでも使用不可    :3(即死身代わり)
    private byte[] item_ = new byte[38]{
            2,//0:HPポーション(小)
            1,//1:毒消し
            0,//2:カエレール
            1,//3:防御力アップ(単体)
            1,//4:攻撃アイテム(小全体)
            1,//5:物理/魔法攻撃力アップ(単体)
            1,//6:ニゲレール
            1,//7:暗闇消し
            2,//8:MPポーション(小)
            2,//9:HPポーション(中)
            1,//10:麻痺消し
            0,//11:エンカウント率(低下)
            1,//12:速度アップ(単体)
            3,//13:即死身代わり
            1,//14:蘇生(HP小状態で)
            2,//15:HPポーション(大)
            2,//16:MPポーション(中)
            1,//17:攻撃アイテム(中全体)
            1,//18:蘇生(HP全快)
            // ここから下は大成功処理---------------
            2,//19:HPポーション(小)
            1,//20:毒消し
            0,//21:カエレール
            1,//22:防御力アップ(単体)
            1,//23:攻撃アイテム(小全体)
            1,//24:物理/魔法攻撃力アップ(単体)
            1,//25:ニゲレール
            1,//26:暗闇消し
            2,//27:MPポーション(小)
            2,//28:HPポーション(中)
            1,//29:麻痺消し
            0,//30:エンカウント率(低下)
            1,//31:速度アップ(単体)
            3,//32:即死身代わり
            1,//33:蘇生(HP小状態で)
            2,//34:HPポーション(大)
            2,//35:MPポーション(中)
            1,//36:攻撃アイテム(中全体)
            1,//37:蘇生(HP全快)
    };

    private (int, int) hpmpNum_ = (0,0);
    private CharaBase.CONDITION condition_ = CharaBase.CONDITION.NON;
    private int itemNum_ = -1;
    private (int,int) buff_ = (-1,-1);
    private bool buffExItemFlg_ = false;    // 大成功アイテムならバフ量を増やす
    private GameObject charasText_;
    private string alive = "";
    private int seNum_ = 0;

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

    // 返り値について…bool->今使えるかどうか
    public bool  CanUseItem(int itemNum)
    {
        if (item_[itemNum] == 3)
        {
            Debug.Log("身代わりアイテムなので、手動使用はできません");
            return false;
        }

        // 現在フィールドのメニュー画面ならば
        // item_変数が「１」のアイテムは使用不可
        if (FieldMng.nowMode == FieldMng.MODE.MENU && item_[itemNum] == 1)
        {
            Debug.Log("戦闘中にしか使えないアイテムを使用しようとしました");
            return false;
        }

        // 現在戦闘中もしくは強制戦闘中ならば
        // item_変数が「０」のアイテムは使用不可
        if ((FieldMng.nowMode == FieldMng.MODE.BUTTLE || FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE) &&
           item_[itemNum] == 0)
        {
            Debug.Log("探索中にしか使えないアイテムを使用しようとしました");
            return false;
        }

        if((SceneMng.nowScene == SceneMng.SCENE.TOWN || SceneMng.nowScene == SceneMng.SCENE.UNIHOUSE) &&
            item_[itemNum] == 0)
        {
            Debug.Log("街もしくはユニハウスで、フィールドでしか使えないアイテムを使用しようとしました");
            return false;
        }

        return true;
    }


    // 返り値について…bool->使う際に対象の指定が必要かどうか
    // 2つ目がtrueになるタイミングは回復系のみ
    public bool Use(int itemNum)
    {
        Debug.Log(itemNum + "番のアイテムを使用します");


        bool tmpFlg = false;
        switch (itemNum)
        {
            case 0:    // HPポーション(小)
                seNum_ = 9;
                hpmpNum_ = (10, 0);
                tmpFlg = true;
                break;
            case 1:    // 毒消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 2:    // カエレール(フィールドからユニハウスへ)
                seNum_ = 0;
                SceneMng.SetSE(seNum_);
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 3:    // 防御力アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 4:    // 全体小ダメージ
                seNum_ = 13;
                SceneMng.SetSE(seNum_);
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(15);
                break;
            case 5:    // 物理/魔法攻撃力アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 6:    // ニゲレール(強制戦闘でない場合は使用可能とする)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    seNum_ = 0;
                    SceneMng.SetSE(seNum_);
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
                    return tmpFlg;
                }
                break;
            case 7:    // 暗闇消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 8:    // MPポーション(小)
                seNum_ = 9;
                hpmpNum_ = (0, 10);
                tmpFlg = true;
                break;
            case 9:    // HPポーション(中)
                seNum_ = 9;
                hpmpNum_ = (40, 0);
                tmpFlg = true;
                break;
            case 10:    // 麻痺消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 11:    // エンカウント率低下
                        // 現在のエンカウント色のまま、一定時間エンカウントしなくなるようにする
                seNum_ = 0;
                SceneMng.SetSE(seNum_);
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 12:    // 速度アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 13:    // 即死身代わり
                // 持っているだけで効果がでるから、ここに処理は書かない
                break;
            case 14:    // 蘇生(最大HPの半分)
                seNum_ = 9;
                alive = "half";
                tmpFlg = true;
                break;
            case 15:    // HPポーション(大)
                seNum_ = 9;
                hpmpNum_ = (100, 0);
                tmpFlg = true;
                break;
            case 16:    // MPポーション(中)
                seNum_ = 9;
                hpmpNum_ = (0, 50);
                tmpFlg = true;
                break;
            case 17:    // 全体中ダメージ
                seNum_ = 13;
                SceneMng.SetSE(seNum_);
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(30);
                break;
            case 18:    // 蘇生(HP全快)
                seNum_ = 9;
                alive = "all";
                tmpFlg = true;
                break;
                // ここから下は大成功処理--------------------------------------------------
            case 19:    // HPポーション(小)
                seNum_ = 9;
                hpmpNum_ = (35, 0);
                tmpFlg = true;
                break;
            case 20:    // 毒消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 21:    // カエレール(フィールドからユニハウスへ)
                seNum_ = 0;
                SceneMng.SetSE(seNum_);
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 22:    // 防御力アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 23:    // 全体小ダメージ
                seNum_ = 13;
                SceneMng.SetSE(seNum_);
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(20);
                break;
            case 24:    // 物理/魔法攻撃力アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 25:    // ニゲレール(強制戦闘でない場合は使用可能とする)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    seNum_ = 0;
                    SceneMng.SetSE(seNum_);
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
                    return tmpFlg;
                }
                break;
            case 26:    // 暗闇消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 27:    // MPポーション(小)
                seNum_ = 9;
                hpmpNum_ = (0, 35);
                tmpFlg = true;
                break;
            case 28:    // HPポーション(中)
                seNum_ = 9;
                hpmpNum_ = (65, 0);
                tmpFlg = true;
                break;
            case 29:    // 麻痺消し
                seNum_ = 10;
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 30:    // エンカウント率低下
                // 現在のエンカウント色のまま、一定時間エンカウントしなくなるようにする
                seNum_ = 0;
                SceneMng.SetSE(seNum_);
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 31:    // 速度アップ(単体)
                seNum_ = 2;
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 32:    // 即死身代わり
                // 持っているだけで効果がでるから、ここに処理は書かない
                break;
            case 33:    // 蘇生(最大HPの半分)
                seNum_ = 9;
                alive = "half";
                tmpFlg = true;
                break;
            case 34:    // HPポーション(大)
                seNum_ = 9;
                hpmpNum_ = (130, 0);
                tmpFlg = true;
                break;
            case 35:    // MPポーション(中)
                seNum_ = 9;
                hpmpNum_ = (0, 65);
                tmpFlg = true;
                break;
            case 36:    // 全体中ダメージ
                seNum_ = 13;
                SceneMng.SetSE(seNum_);
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(45);
                break;
            case 37:    // 蘇生(HP全快)
                seNum_ = 9;
                alive = "all";
                tmpFlg = true;
                break;
        }

        itemNum_ = itemNum;
        return tmpFlg;
    }

    public void OnClickCharaButton(int num)
    {
        // ここに音の処理書かないとだめ。
        SceneMng.SetSE(seNum_);
        // あと会話後にアイテムもらっても、メニューを開かないで戦闘で始めて開いたらおかしくなってた気がする

        if (!SceneMng.charasList_[num].GetDeathFlg())
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
                        bufftra.GetChild(i).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

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