using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInteriorMng : MonoBehaviour
{
    public void SetHouseVisible(string name)
    {
        // 一致したオブジェクト以外を非アクティブ
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if(child.name != name)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
