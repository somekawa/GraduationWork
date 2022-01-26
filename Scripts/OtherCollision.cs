using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス

    private string objName_;
    private int materiaNum_;
    private DropFieldMateria itemGet_;
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
        if (other.CompareTag("Drop"))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log(other.name);
                Debug.Log("スペースキーを押下しました");
                var nameCheck = other.name.Split('_');
                // オブジェクト名+番号
                objName_ = nameCheck[0];
                // オブジェクトの番号
                int objNum = int.Parse(Regex.Replace(objName_, @"[^0-9]", ""));
                // 素材の番号
                materiaNum_ = int.Parse(nameCheck[1]);
                for (int i = 0; i < (int)DropFieldMateria.MATERIA_NUMBER.MAX; i++)
                {
                    if (DropFieldMateria.objName[i] == other.name)
                    {
                        Debug.Log(i + "       FrontColliderと接触したオブジェクト" + other.name);
                        itemGet_.SetItemName(materiaNum_, objNum);
                        infoImage_.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drop"))
        {
            // 接触した瞬間
            if (infoImage_.gameObject.activeSelf == false)
            {
                infoImage_.gameObject.SetActive(true);
            }
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