using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private RectTransform parentCanvas_;    // �A�C�e���֘A��\������L�����o�X

    private string objName_;
    private int materiaNum_;
    private DropFieldMateria itemGet_;
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
        if (other.CompareTag("Drop"))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log(other.name);
                Debug.Log("�X�y�[�X�L�[���������܂���");
                var nameCheck = other.name.Split('_');
                // �I�u�W�F�N�g��+�ԍ�
                objName_ = nameCheck[0];
                // �I�u�W�F�N�g�̔ԍ�
                int objNum = int.Parse(Regex.Replace(objName_, @"[^0-9]", ""));
                // �f�ނ̔ԍ�
                materiaNum_ = int.Parse(nameCheck[1]);
                for (int i = 0; i < (int)DropFieldMateria.MATERIA_NUMBER.MAX; i++)
                {
                    if (DropFieldMateria.objName[i] == other.name)
                    {
                        Debug.Log(i + "       FrontCollider�ƐڐG�����I�u�W�F�N�g" + other.name);
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
            // �ڐG�����u��
            if (infoImage_.gameObject.activeSelf == false)
            {
                infoImage_.gameObject.SetActive(true);
            }
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