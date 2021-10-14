using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private ItemGet itemGet_;
    private string saveObjName_;

    private RectTransform parentCanvas_;    // �A�C�e���֘A��\������L�����o�X
    private Image itemNearImage_;
    private Vector3 activePos_;
    private Camera mainCamera_;      // ���W��ԕύX���Ɏg�p
    private GameObject uniChan_;      // ���W��ԕύX���Ɏg�p

    void Start()
    {
        parentCanvas_ = GameObject.Find("ItemCanvas").GetComponent<RectTransform>();
        itemNearImage_ = parentCanvas_.gameObject.transform.Find("ActionImage").GetComponent<Image>();
        itemNearImage_.gameObject.SetActive(false);
        itemGet_ = GameObject.Find("ItemPoints").transform.GetComponent<ItemGet>();
        mainCamera_ = GameObject.Find("MainCamera").GetComponent<Camera>();
        uniChan_ = GameObject.Find("Uni");

        saveObjName_ = "";

    }

    private void OnTriggerStay(Collider other)
    {
        //if (saveObjName_ == other.name)
        //{
        //    // �������O�̃I�u�W�F�N�g�̏ꍇ�擾�ł��Ȃ��悤�ɂ���
        //    return;
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < (int)ItemGet.items.MAX; i++)
            {
                if (ItemGet.objName[i] == other.name)
                {
                 //   Debug.Log("FrontCollider�ƐڐG�����I�u�W�F�N�g" + other.name);
                    itemGet_.SetItemName(i, other.name, true);
                    saveObjName_ = other.name;
                    itemNearImage_.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ActionActivePos();
    }

    private void OnTriggerExit(Collider other)
    {
        if (itemNearImage_.gameObject.activeSelf == true)
        {
            itemNearImage_.gameObject.SetActive(false);
        }
    }

    private void ActionActivePos()
    {
        if(itemNearImage_.gameObject.activeSelf==false)
        {
            itemNearImage_.gameObject.SetActive(true);

        }

        // �I�u�W�F�N�g�̃��[���h���position���r���[�|�[�g��Ԃɕϊ�
        activePos_ = mainCamera_.WorldToViewportPoint(uniChan_.transform.position);
      //  Debug.Log(activePos_);

        // �r���[�|�[�g�̌��_�͍����ACanvas�͒����̂���Canvas��RectTransform�̃T�C�Y��1/2������
        var WorldObject_ScreenPosition = (activePos_ * parentCanvas_.sizeDelta) - (parentCanvas_.sizeDelta * 0.5f);
        // �\���摜�̃A���J�[�ɍ��W�������č��W������
        //itemNearImage_.transform.localPosition = new Vector2(WorldObject_ScreenPosition.x,
        //    WorldObject_ScreenPosition.y + 100);
        itemNearImage_.transform.localPosition = new Vector2(
            WorldObject_ScreenPosition.x+45.0f,
            WorldObject_ScreenPosition.y+100.0f);
        //Debug.Log(itemNearImage_.transform.localPosition+"          �r���[�|�[�g"+WorldObject_ScreenPosition);
    }
}
