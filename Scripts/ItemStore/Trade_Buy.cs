using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trade_Buy : MonoBehaviour
{
    [SerializeField]
    private GameObject merchandisePrefab;    // 素材を拾ったときに生成されるプレハブ

    public static GameObject[,] merchandise_ = new GameObject[5, 30];
    public static Text[,] merchandiseName_ = new Text[5, 30];
    private MateriaList materiaList_;
    private int nowFieldNum_ = 1;// どのフィールドまで進んでいるか。

    public void Init()
    {
        for (int f = 0; f <= nowFieldNum_; f++)
        {
            materiaList_ = Resources.Load("MaterialList/Field" + f) as MateriaList;
            for (int m = 0; m < materiaList_.param.Count; m++)
            {
                merchandise_[f, m] = Instantiate(merchandisePrefab,
                new Vector2(0, 0), Quaternion.identity,
                GameObject.Find("TradeCanvas/BuyMng/Viewport/Content").transform);
                merchandiseName_[f, m] = merchandise_[f, m].transform.Find("Name").GetComponent<Text>();
                merchandiseName_[f, m].text = materiaList_.param[m].MateriaName;
                merchandise_[f, m].name = materiaList_.param[m].MateriaName;
            }
        }

    }

}
