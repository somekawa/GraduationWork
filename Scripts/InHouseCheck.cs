using UnityEngine;

public class InHouseCheck : MonoBehaviour
{
    private HouseInteriorMng interiorMng_;

    void Start()
    {
        interiorMng_ = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Uniと家の当たり判定
        if(collision.transform.tag == "Player")
        {
            Debug.Log(this.gameObject.name + "とPlayerが接触");

            interiorMng_.SetActiveCanvas(true,this.gameObject.name);
        }
    }
}
