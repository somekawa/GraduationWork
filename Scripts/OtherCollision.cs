using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCollision : MonoBehaviour
{
    private GameObject itemParent_;
    private ItemGet itemGet_;

    // Start is called before the first frame update
    void Start()
    {
        itemParent_ = GameObject.Find("ItemPoints");
        itemGet_ = itemParent_.transform.GetComponent<ItemGet>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("FrontColliderと接触したオブジェクト" + other.name);
            itemGet_.SetItemName(other.name,true);
        }
    }
}
