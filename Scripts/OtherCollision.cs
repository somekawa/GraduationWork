using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherCollision : MonoBehaviour
{
    private ItemGet itemGet_;
    private string saveObjName_;

    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス
    private Image itemNearImage_;
    private Vector3 activePos_;
    private Camera mainCamera_;      // 座標空間変更時に使用
    private GameObject uniChan_;      // 座標空間変更時に使用

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
        //    // 同じ名前のオブジェクトの場合取得できないようにする
        //    return;
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            for (int i = 0; i < (int)ItemGet.items.MAX; i++)
            {
                if (ItemGet.objName[i] == other.name)
                {
                 //   Debug.Log("FrontColliderと接触したオブジェクト" + other.name);
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

        // オブジェクトのワールド空間positionをビューポート空間に変換
        activePos_ = mainCamera_.WorldToViewportPoint(uniChan_.transform.position);
      //  Debug.Log(activePos_);

        // ビューポートの原点は左下、Canvasは中央のためCanvasのRectTransformのサイズの1/2を引く
        var WorldObject_ScreenPosition = (activePos_ * parentCanvas_.sizeDelta) - (parentCanvas_.sizeDelta * 0.5f);
        // 表示画像のアンカーに座標を代入して座標を決定
        //itemNearImage_.transform.localPosition = new Vector2(WorldObject_ScreenPosition.x,
        //    WorldObject_ScreenPosition.y + 100);
        itemNearImage_.transform.localPosition = new Vector2(
            WorldObject_ScreenPosition.x+45.0f,
            WorldObject_ScreenPosition.y+100.0f);
        //Debug.Log(itemNearImage_.transform.localPosition+"          ビューポート"+WorldObject_ScreenPosition);
    }
}
