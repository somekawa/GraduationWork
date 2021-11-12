using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Buy : MonoBehaviour
{
    private InitPopList popItemsList_;

    [SerializeField]
    private RectTransform buyParent;   // 表示位置の親

    private GameObject[] activeObj_;    //プレハブ生成時に使用
    private Text[] activePrice_;        // 表示する値段
    private Text[] activeText_;         // 表示する素材の名前

    private int maxCnt_ = 0;            // すべての素材数
    private int singleCnt_ = 0;         // 1つのシートに記載されてる最大個数

    private int nowFieldNum_ = 2;       // 現在のフィールド

    private void Start()
    {
        popItemsList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
     
        maxCnt_ = popItemsList_.SetMaxItemCount();
        activeObj_ = new GameObject[maxCnt_];
        activePrice_ = new Text[maxCnt_];
        activeText_ = new Text[maxCnt_];
        
        for (int i = 0; i < maxCnt_; i++)
        {
            //  Debug.Log("店で買えるもの" + PopMateriaList.activeObj_[i].name);
            activeObj_[i] = PopListInTown.materiaPleate[i];

            // 表示する名前を変更する
            activeText_[i] = activeObj_[i].transform.Find("Name").GetComponent<Text>();
            activeText_[i].text = activeObj_[i].name;

            // 料金を表示するText
            activePrice_[i] = activeObj_[i].transform.Find("Price").GetComponent<Text>();
        }
    }

    public void SetActiveBuy()
    {
        // 現在進行しているフィールドの素材しか表示しないようにする
        for (int i = 0; i < singleCnt_ * nowFieldNum_; i++)
        {
            // 親位置を変える
            activeObj_[i].transform.SetParent(buyParent.transform);
          
            // 表示する料金を買い値に変更
            activePrice_[i].text = InitPopList.materiaData[i].buyPrice.ToString() + "ビット";

            // すべてアクティブ状態にする
            activeObj_[i].SetActive(true);
        }
    }
}