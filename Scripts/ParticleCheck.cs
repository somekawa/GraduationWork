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
  
    private Camera mainCamera_;      // 座標空間変更時に使用
    private Vector3 lb; // 左下の座標
    private Vector3 rt; // 右上の座標
    
    
    
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

                    Debug.Log("ユニから一番近い距離のは" + obj_[i].name);
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
        // 可視範囲外
        enabled = false;
        Debug.Log("可視範囲外");


    }

    void OnBecameVisible()
    {
        // 可視範囲
        enabled = true;
        Debug.Log("可視範囲");

    }
}