using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private DropFieldMateria itemGet_;
    private RectTransform parentCanvas_;    // �A�C�e���֘A��\������L�����o�X
    private Image infoImage_;           // �ڐG�͈͂ɓ�������w�����o��

    void Start()
    {
        itemGet_ = GameObject.Find("MateriaPoints").transform.GetComponent<DropFieldMateria>();
        parentCanvas_ = GameObject.Find("FieldUICanvas/DropMng").GetComponent<RectTransform>();
        infoImage_ = parentCanvas_.gameObject.transform.Find("DropInfoBack").GetComponent<Image>();
        infoImage_.gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        // �ڐG��
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < (int)DropFieldMateria.MATERIA_NUMBER.MAX; i++)
            {
                if (DropFieldMateria.objName[i] == other.name)
                {
                 //   Debug.Log("FrontCollider�ƐڐG�����I�u�W�F�N�g" + other.name);
                    itemGet_.SetItemName(i, other.name, true);
                    infoImage_.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �ڐG�����u��
        if (infoImage_.gameObject.activeSelf == false)
        {
            infoImage_.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �͈͊O�ɏo���u��
        if (infoImage_.gameObject.activeSelf == true)
        {
            infoImage_.gameObject.SetActive(false);
        }
    }
}
