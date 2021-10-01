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
        // Uni‚Æ‰Æ‚Ì“–‚½‚è”»’è
        if(collision.transform.tag == "Player")
        {
            Debug.Log(this.gameObject.name + "‚ÆPlayer‚ªÚG");

            interiorMng_.SetActiveCanvas(true,this.gameObject.name);
        }
    }
}
