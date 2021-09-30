using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleCheck : MonoBehaviour
{
    private GameObject uniChan_;
    private Vector3 viewUniPos_;
    private ParticleSystem testPS_;
    public GameObject itemPoints_;
    private GameObject[] obj_;
  
    private Camera mainCamera_;      // ���W��ԕύX���Ɏg�p
    private Vector3 lb; // �����̍��W
    private Vector3 rt; // �E��̍��W
    
    
    
    private float distance_ = 0.0f;
    private float targetDistance = 5.0f;
    void Start()
    {
        uniChan_ = GameObject.Find("Uni");

        mainCamera_ = GameObject.Find("MainCamera").GetComponent<Camera>();

        viewUniPos_ = mainCamera_.WorldToViewportPoint(uniChan_.transform.position);


        itemPoints_ = GameObject.Find("ItemPoints");
        obj_ = new GameObject[itemPoints_.transform.childCount];
        for (int i = 0; i < itemPoints_.transform.childCount; i++)
        {
            obj_[i] = itemPoints_.transform.GetChild(i).gameObject;
            testPS_ = obj_[i].transform.GetChild(0).GetComponent<ParticleSystem>();
        }
        //  testPS_.gameObject.SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {


        for (int i = 0; i < itemPoints_.transform.childCount; i++)
        {
            distance_ = (uniChan_.transform.position - obj_[i].transform.position).sqrMagnitude;
            if (distance_ < targetDistance * targetDistance)
            {
                //if (uniChan_.transform.position.x < (obj_[i].transform.position.x)
                //             || (uniChan_.transform.position.z < obj_[i].transform.position.z))
                //{

                    testPS_ = obj_[i].transform.GetChild(0).GetComponent<ParticleSystem>();
                    testPS_.Play();

                    Debug.Log("���j�����ԋ߂������̂�" + obj_[i].name);
              //  }
            }
        }

        Ray ray = mainCamera_.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            for (int i = 0; i < itemPoints_.transform.childCount; i++)
            {
                if (hit.transform.name == obj_[i].name)
                {
                    //    if (hit.transform.name == "Item2")
                    //    {
                    print("I'm looking at " + hit.transform.name);
                    print("I'm looking at " + hit.transform.position);

                    //}
                }
            }
        }
    }
    


    void OnBecameInvisible()
    {
        // ���͈͊O
        enabled = false;
        Debug.Log("���͈͊O");


    }

    void OnBecameVisible()
    {
        // ���͈�
        enabled = true;
        Debug.Log("���͈�");

    }
}