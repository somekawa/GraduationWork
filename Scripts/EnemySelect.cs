using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 敵の頭上に現れる矢印アイコンを操作するスクリプト

public class EnemySelect : MonoBehaviour
{
    // Item1:座標情報のリスト,Item2:敵の生存切替用のリスト(Destroyされた敵の座標リストをfalseにする)
    private System.Tuple<List<Vector3>, List<bool>> posList_;
    private List<GameObject> targetImageObjList_ = new List<GameObject>();   // 敵指定マークをリストに保存する

    private bool allSelectFlg_; // true:複数or全体攻撃,false:単体攻撃
    private bool randFlg_;      // true:複数,false:全体攻撃
    private int selectNum_ = 0;
    private readonly float posOffset_Y = 1.5f;

    enum SelectKey
    {
        NON,
        UP,
        DOWN,
        MAX
    }

    private SelectKey selectKey_ = SelectKey.MAX;

    void Update()
    {
        if (posList_.Item1.Count <= 0 || selectKey_ == SelectKey.MAX)
        {
            return;
        }

        if(allSelectFlg_)
        {
            // 攻撃対象が複数or全体
            // 常に回転させていたい
            for(int i = 0; i < targetImageObjList_.Count; i++)
            {
                targetImageObjList_[i].transform.Rotate(0.0f, 0.0f, 120.0f * Time.deltaTime);
            }
        }
        else
        {
            // 攻撃対象が単体
            // 常に回転させていたい
            targetImageObjList_[selectNum_].transform.Rotate(0.0f, 0.0f, 120.0f * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.J))
            {
                SceneMng.SetSE(0);
                selectKey_ = SelectKey.UP;
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                SceneMng.SetSE(0);
                selectKey_ = SelectKey.DOWN;
            }
            else
            {
                selectKey_ = SelectKey.NON;
            }

            MoveSelectKey(selectKey_);
        }
    }

    // falseになっているposListがあったらアイコン設置位置を飛ばす処理
    void MoveSelectKey(SelectKey key)
    {
        if(key == SelectKey.NON)
        {
            return;
        }

        if(key == SelectKey.UP)
        {
            // breakで抜けたときはtrueになる
            // while文を抜ける条件で抜けた時はfalseのまま
            bool tmpFlg = false;

            // 座標上限より値が小さければwhile文を続行する
            while (selectNum_ < posList_.Item1.Count - 1)
            {
                selectNum_++;

                // 敵が存在すればwhile文を抜ける
                if (posList_.Item2[selectNum_])
                {
                    tmpFlg = true;
                    break;
                }
            }

            // 手前の敵がfalseになっている時に必要な処理
            if (!tmpFlg)
            {
                int num = posList_.Item1.Count - 1;
                // 中身を逆にする(0〜4なら、4〜0になる)
                posList_.Item2.Reverse();

                foreach (bool flag in posList_.Item2)
                {
                    if (flag)    // 最大値から順番に調べていって、trueになっているところで止まる
                    {
                        // trueの敵で矢印の移動がストップするように、値を代入する必要がある
                        selectNum_ = num;
                        break;
                    }
                    num--;
                }

                // foreachの為に逆順にしていたのを元に戻す
                posList_.Item2.Reverse();
            }

        }
        else if(key == SelectKey.DOWN)
        {
            // breakで抜けたときはtrueになる
            // while文を抜ける条件で抜けた時はfalseのまま
            bool tmpFlg = false;

            // 0より値が大きければwhile文を続行する
            while (selectNum_ > 0)
            {
                selectNum_--;

                // 敵が存在すればwhile文を抜ける
                if (posList_.Item2[selectNum_])
                {
                    tmpFlg = true;
                    break;
                }
            }

            // 奥の敵がfalseになっている時に必要な処理
            if(!tmpFlg)
            {
                ResetSelectPoint();
            }
        }
        else
        {
            // 何も処理を行わない
        }

        // TargetImageを全てfalseにする
        for (int i = 0; i < this.transform.childCount; i++)
        {
            targetImageObjList_[i].SetActive(false);
        }
        // 選択中の敵のTargetImageをtrueにする
        targetImageObjList_[selectNum_].SetActive(true);
    }

    // CharacterMng.csから戦闘時の敵の出現位置を受け取る
    public void SetPosList(List<Vector3> list)
    {
        // 受け取ったlistの数分、trueにしたboolのリストを用意する
        var tmpList = new List<bool>();
        for(int i = 0; i < list.Count(); i++)
        {
            tmpList.Add(true);
        }

        // 代入する
        posList_ = new System.Tuple<List<Vector3>, List<bool>>(list, tmpList);

        // 受け取った値のY座標にoffsetが必要
        for (int i = 0; i < posList_.Item1.Count; i++)
        {
            // そのままlistを書き換えようとするとエラーがでるので、下記のようにして変更する
            Vector3 tmpData = posList_.Item1[i];
            tmpData.y += posOffset_Y;
            posList_.Item1[i] = tmpData;
        }

        // 矢印の初期位置
        selectNum_ = 0;

        // 中身を初期化する
        targetImageObjList_.Clear();

        // TargetImageを全てfalseにする
        // リストに登録する
        for (int i = 0; i < this.transform.childCount; i++)
        {
            targetImageObjList_.Add(this.transform.GetChild(i).transform.Find("TargetImage").gameObject);
            targetImageObjList_[i].SetActive(false);
        }
    }

    // CharacterMng.cs側で表示/非表示を変更できるようにする
    public void SetActive(bool flag)
    {
        allSelectFlg_ = false;
        if (!flag)
        {
            selectKey_ = SelectKey.MAX;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                targetImageObjList_[i].SetActive(flag);
            }
        }
        else
        {
            selectKey_ = SelectKey.NON;
            targetImageObjList_[selectNum_].SetActive(flag);
        }
    }

    // 全ての敵選択マークを表示/非表示を切り替えられるようにする
    public void SetAllActive(bool flag,bool randFlg)
    {
        selectKey_ = SelectKey.NON;
        allSelectFlg_ = true;
        randFlg_ = randFlg;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            targetImageObjList_[i].SetActive(flag);
        }
    }

    // CharacterMng.cs側に目標座標を渡す
    public Vector3 GetSelectEnemyPos(int num)
    {
        return posList_.Item1[num];
    }

    public int[] GetSelectNum()
    {
        // 初期数値を-1にしておく
        int[] tmpArray = { -1, -1, -1, -1 };

        if(!allSelectFlg_)
        {
            // selectNum_と3つの-1
            tmpArray[0] = selectNum_;
        }
        else
        {
            // 先に生きてる敵だけまとめたほうが早いかも
            List<int> tmpList = new List<int>();
            for (int i = 0; i < posList_.Item1.Count; i++)
            {
                if(!EnemyInstanceMng.enemyList_[i].Item1.GetDeathFlg())
                {
                    tmpList.Add(i);
                }
            }

            // 複数か全体で処理を分ける
            if (randFlg_)
            {
                // 攻撃回数をランダムにする(2〜4回とする)
                int randAttackNum = Random.Range(2, 5); // 2以上5未満の値がでる
                Debug.Log("複数回魔法攻撃の攻撃回数は" + randAttackNum + "回に決定しました");

                // 生きてる敵の中から攻撃対象をランダムで決定する
                for (int i = 0; i < randAttackNum; i++)
                {
                    // ランダムなキャラを取得する(ただし、死亡した敵は除外する)
                    do
                    {
                        tmpArray[i] = Random.Range(0, posList_.Item1.Count);

                        if(Input.GetKeyDown(KeyCode.A))
                        {
                            break;
                        }

                    } while (EnemyInstanceMng.enemyList_[tmpArray[i]].Item1.GetDeathFlg());
                }
            }
            else
            {
                // 全体
                // 生きてる敵の数値を入れる
                for (int i = 0; i < posList_.Item1.Count; i++)
                {
                    if (!EnemyInstanceMng.enemyList_[i].Item1.GetDeathFlg())
                    {
                        tmpArray[i] = i;
                    }
                }
            }
        }

        return tmpArray;
    }

    // CharacterMng.cs側からGetSelectNum関数の後に呼び出してもらい、矢印位置を再設定する
    // この処理がないと、次のキャラの行動時にDestroyした敵の頭上から矢印がスタートしてしまう。
    public bool ResetSelectPoint()
    {
        // HPが0の敵はfalseにする
        var tmp = EnemyInstanceMng.enemyList_;
        for (int i = 0; i < tmp.Count(); i++)
        {
            if (tmp[i].Item1.HP() <= 0 && posList_.Item2[i])
            {
                posList_.Item2[i] = false;
            }
        }

        // 0から順に調べて、trueのところで止まる
        int num = 0;
        foreach (bool flag in posList_.Item2)
        {
            if (flag)    // 0から順番に調べていって、trueになっているところで止まる
            {
                // trueの敵で矢印の移動がストップするように、値を代入する必要がある
                selectNum_ = num;
                return true;
            }
            num++;
        }

        // ここまで到達したら(=breakで引っかからなかったら、戦闘モード終了の合図に出来そう)
        return false;
    }

    // CharacterMng.cs側から呼び出して、Tキーが押されていたらコマンド選択に戻れるようにする
    public bool ReturnSelectCommand()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SceneMng.SetSE(0);
            SetActive(false);
            return false;
        }
        return true;
    }
}
