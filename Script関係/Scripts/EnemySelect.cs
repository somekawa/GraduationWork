using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 敵の頭上に現れる矢印アイコンを操作するスクリプト

public class EnemySelect : MonoBehaviour
{
    // Item1:座標情報のリスト,Item2:敵の生存切替用のリスト(Destroyされた敵の座標リストをfalseにする)
    private System.Tuple<List<Vector3>, List<bool>> posList_;
    //private List<Vector3> posList_ = new List<Vector3>();

    private int selectNum_ = 0;
    private readonly float posOffset_Y = 1.5f;

    enum SelectKey
    {
        NON,
        UP,
        DOWN
    }

    private SelectKey selectKey_ = SelectKey.NON;

    void Start()
    {
        
    }

    void Update()
    {
        if (posList_.Item1.Count <= 0)
        {
            return;
        }

        // falseになっているposListがあったら飛ばしたい
        // while文でやったほうがよさそう

        if (Input.GetKeyDown(KeyCode.J))
        {
            selectKey_ = SelectKey.UP;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            selectKey_ = SelectKey.DOWN;
        }
        else
        {
            selectKey_ = SelectKey.NON;
        }

        MoveSelectKey(selectKey_);

        Debug.Log("選択中の敵" + selectNum_);

        // 座標移動
        this.gameObject.transform.position = posList_.Item1[selectNum_];

        // タプル変更前
        //if(posList_.Count <= 0)
        //{
        //    return;
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    // 奥の敵に矢印が動く
        //    if (selectNum_ < posList_.Count - 1)
        //    {
        //        selectNum_++;
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.H))
        //{
        //    // 手前の敵に矢印が動く
        //    if (selectNum_ > 0)
        //    {
        //        selectNum_--;
        //    }
        //}
        //else
        //{
        //    // 何も処理を行わない
        //}
        //Debug.Log("選択中の敵" + selectNum_);
        //// 座標移動
        //this.gameObject.transform.position = posList_[selectNum_];
    }

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
    }

    // CharacterMng.csから戦闘時の敵の出現位置を受け取る
    public void SetPosList(List<Vector3> list)
    {
        // 受け取ったlistの数分、trueにしたboolのリストを用意する
        var test = new List<bool>();
        for(int i = 0; i < list.Count(); i++)
        {
            test.Add(true);
        }

        // 代入する
        posList_ = new System.Tuple<List<Vector3>, List<bool>>(list, test);

        // 受け取った値のY座標にoffsetが必要
        for (int i = 0; i < posList_.Item1.Count; i++)
        {
            // そのままlistを書き換えようとするとエラーがでるので、下記のようにして変更する
            Vector3 tmpData = posList_.Item1[i];
            tmpData.y += posOffset_Y;
            posList_.Item1[i] = tmpData;
        }

        // 矢印の初期位置
        this.gameObject.transform.position = posList_.Item1[0];

        // タプル変更前
        //posList_ = list;
        //// 受け取った値のY座標にoffsetが必要
        //for (int i = 0; i < posList_.Count; i++)
        //{
        //    // そのままlistを書き換えようとするとエラーがでるので、下記のようにして変更する
        //    Vector3 tmpData = posList_[i];
        //    tmpData.y += posOffset_Y;
        //    posList_[i] = tmpData;
        //}
        // 矢印の初期位置
        //this.gameObject.transform.position = posList_[0];

        // 戦闘画面に遷移時、すぐにアイコンが表示されたら困るため非表示にする
        this.gameObject.SetActive(false);
    }

    // CharacterMng.cs側で表示/非表示を変更できるようにする
    public void SetActive(bool flag)
    {
        this.gameObject.SetActive(flag);
    }

    // CharacterMng.cs側に目標座標を渡す
    public Vector3 GetSelectEnemyPos()
    {
        // offset値を元に戻してから渡す
        Vector3 tmppos = this.gameObject.transform.position;
        tmppos.y -= posOffset_Y;
        return tmppos;
    }

    // CharacterMng.cs側に選択された番号を渡す
    public int GetSelectNum()
    {
        // returnされるselectNum_はMagicMove.csでDestroyされるので、該当するlistをfalseにする
        posList_.Item2[selectNum_] = false;
        return selectNum_;
    }

    // CharacterMng.cs側からGetSelectNum関数の後に呼び出してもらい、矢印位置を再設定する
    // この処理がないと、次のキャラの行動時にDestroyした敵の頭上から矢印がスタートしてしまう。
    public void ResetSelectPoint()
    {
        // 0から順に調べて、trueのところで止まる
        int num = 0;
        foreach (bool flag in posList_.Item2)
        {
            if (flag)    // 0から順番に調べていって、trueになっているところで止まる
            {
                // trueの敵で矢印の移動がストップするように、値を代入する必要がある
                selectNum_ = num;
                break;
            }
            num++;
        }
    }
}
