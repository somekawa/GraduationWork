using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMng : MonoBehaviour
{
    private HouseInteriorMng interiorMng_;

    private readonly string[] buildNameEng_ = { "MayorHouse", "BookStore", "ItemStore", "Guild", "Restaurant" };  // ������(�q�G�����L�[�Ɠ����p��)
    private readonly Vector3[] buildPos_ = { new Vector3(0.0f,0.0f,110.0f)};
    private Dictionary<string, Vector3> uniPosMap_ = new Dictionary<string, Vector3>();    // �L�[:�p�ꌚ����,�l:���j�����\�����W

    void Start()
    {
        // ���݂̃V�[����TOWN�Ƃ���
        SceneMng.SetNowScene(SceneMng.SCENE.TOWN);

        // �����ƍ��W����v������
        uniPosMap_.Add(buildNameEng_[0], buildPos_[0]);

        // SceneMng�����΂����������󂯂Ƃ�(��΂��Ȃ��Ă����Ƃ��͏������Ȃ��悤�ɒ���)
        string str = SceneMng.GetHouseName();

        if(str != "Mob")
        {
            // �L�����ɍ��W��������
            GameObject.Find("Uni").gameObject.transform.position = uniPosMap_[str];
        }

        // ��Ŏg��
        interiorMng_ = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
    }
}
