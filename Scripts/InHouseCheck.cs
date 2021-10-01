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
        // Uni�ƉƂ̓����蔻��
        if(collision.transform.tag == "Player")
        {
            Debug.Log(this.gameObject.name + "��Player���ڐG");

            interiorMng_.SetActiveCanvas(true,this.gameObject.name);
        }
    }
}
