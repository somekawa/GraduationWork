using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private DropFieldMateria itemGet_;
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス
    private Image infoImage_;           // 接触範囲に入ったら指示を出す

    void Start()
    {
        itemGet_ = GameObject.Find("MateriaPoints").transform.GetComponent<DropFieldMateria>();
        parentCanvas_ = GameObject.Find("FieldUICanvas/DropMng").GetComponent<RectTransform>();
        infoImage_ = parentCanvas_.gameObject.transform.Find("DropInfoBack").GetComponent<Image>();
        infoImage_.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        // 接触中
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < (int)DropFieldMateria.MATERIA_NUMBER.MAX; i++)
            {
                if (DropFieldMateria.objName[i] == other.name)
                {
                 //   Debug.Log("FrontColliderと接触したオブジェクト" + other.name);
                    itemGet_.SetItemName(i, other.name, true);
                    infoImage_.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 接触した瞬間
        if (infoImage_.gameObject.activeSelf == false)
        {
            infoImage_.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 範囲外に出た瞬間
        if (infoImage_.gameObject.activeSelf == true)
        {
            infoImage_.gameObject.SetActive(false);
        }
    }
}
