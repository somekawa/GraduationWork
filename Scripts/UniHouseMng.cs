using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ���݂̃V�[����TOWN�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);
    }
}
