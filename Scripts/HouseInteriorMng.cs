using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInteriorMng : MonoBehaviour
{
    public void SetHouseVisible(string name)
    {
        // ��v�����I�u�W�F�N�g�ȊO���A�N�e�B�u
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
