using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Sell : MonoBehaviour
{
    private PopListInTown popItemsList_; 

    [SerializeField]
    private RectTransform sellParent_;  // 表示位置の親

    private GameObject[] activeObj_;    //プレハブ生成時に使用
    private Text[] activePrice_;        // 表示する値段
    private Text[] activeText_;         // 表示する素材の名前

    private int maxCnt_ = 0;            // すべての素材数

    private void Start()
    {
        popItemsList_ = GameObject.Find("SceneMng").GetComponent<PopListInTown>();
       
        maxCnt_ = popItemsList_.SetMaxItemsCount();
        activeObj_ = new GameObject[maxCnt_];//プレハブ生成時に使用
        activePrice_ = new Text[maxCnt_];
        activeText_ = new Text[maxCnt_];

        for (int i = 0; i < maxCnt_; i++)
        {
            activeObj_[i] = PopListInTown.activeObj_[i];
            //  Debug.Log("バッグの中身" + PopMateriaList.activeObj_[i].name);
            // 表示する名前を変更する
            activeText_[i] = activeObj_[i].transform.Find("Name").GetComponent<Text>();
            activeText_[i].text = activeObj_[i].name;

            // 料金を表示するText
            activePrice_[i] = activeObj_[i].transform.Find("Price").GetComponent<Text>();
            activeObj_[i].SetActive(false);
        }
    }

    public void SetActiveSell()
    {
        // Debug.Log(objParent_.transform.childCount);
        for (int i = 0; i < maxCnt_; i++)
        {
            // 親位置を変える
            activeObj_[i].transform.SetParent(sellParent_.transform);

            // 表示する料金を売値に変更
            activePrice_[i].text = PopListInTown.mateiraSellPrice_[i].ToString() + "ビット";

            // 所持数が0以上でバッグの中身と同じものがあれば表示
            if (0 < Bag_Materia.materiaState[i].haveCnt)
            {
                activeObj_[i].SetActive(true);
            }
            else
            {
                activeObj_[i].SetActive(false);
            }
        }
    }

    public void SetHaveCntCheck(int materiaNum)
    {
        // 指定素材の所持数が0個になったら呼び出される
       activeObj_[materiaNum].SetActive(false);
    }
}

