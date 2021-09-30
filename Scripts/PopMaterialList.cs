using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopMaterialList : MonoBehaviour
{
    private MaterialList materialList_;

    private ItemGet materialGet_;
   // private TestApplication materialGet_;

    private int fieldNumber_;

    private enum fieldNum
    {
        NON = -1,
        FOREST,
        FIELD1,
        FIELD2,
        FIELD3,
        FIELD4,
        MAX
    }
    private string[] fieldNum_ = new string[(int)fieldNum.MAX] {
    "ForestField",    "TestField",    "Fidle2",    "Fidle3",    "Fidle4"
    };

    void Start()
    {
        materialGet_ = GameObject.Find("ItemPoints").GetComponent<ItemGet>();
       // materialGet_ = GameObject.Find("GameObject").GetComponent<TestApplication>();

        for (int i = 0; i < (int)fieldNum.MAX; i++)
        {
            if (SceneManager.GetActiveScene().name == fieldNum_[i])
            {
                fieldNumber_ = i;
                break;
            }
        }

         // materialList_ = Resources.Load("Field"+ itemNumber) as MaterialList;
        materialList_ = Resources.Load("MaterialList/Field" + fieldNumber_) as MaterialList;

        //for (int i = 0; i < materialList_.param.Count; i++)
        //{
        //    Debug.Log(i + "”Ô–Ú name=" + materialList_.param[i].ImageName);

        //}
        Debug.Log(materialList_.param[0].ImageName);
        materialGet_.SetMaterialKinds(fieldNumber_, materialList_);

    }
}
