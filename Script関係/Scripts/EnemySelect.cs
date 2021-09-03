using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelect : MonoBehaviour
{
    private List<Vector3> posList_ = new List<Vector3>();
    private int selectNum_ = 0;
    private readonly float posOffset_Y = 1.5f;

    void Start()
    {
        
    }

    void Update()
    {
        if(posList_.Count <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            // 奥の敵に矢印が動く
            if (selectNum_ < posList_.Count - 1)
            {
                selectNum_++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            // 手前の敵に矢印が動く
            if (selectNum_ > 0)
            {
                selectNum_--;
            }
        }
        else
        {
            // 何も処理を行わない
        }

        Debug.Log("選択中の敵" + selectNum_);

        // 座標移動
        this.gameObject.transform.position = posList_[selectNum_];
    }

    // CharacterMng.csから戦闘時の敵の出現位置を受け取る
    public void SetPosList(List<Vector3> list)
    {
        posList_ = list;

        // 受け取った値のY座標にoffsetが必要
        for (int i = 0; i < posList_.Count; i++)
        {
            // そのままlistを書き換えようとするとエラーがでるので、下記のようにして変更する
            Vector3 tmpData = posList_[i];
            tmpData.y += posOffset_Y;
            posList_[i] = tmpData;
        }

        // 矢印の初期位置
        this.gameObject.transform.position = posList_[0];

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
        return selectNum_;
    }
}
