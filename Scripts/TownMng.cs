using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMng : MonoBehaviour
{
    private HouseInteriorMng interiorMng_;

    private readonly string[] buildNameEng_ = { "MayorHouse", "BookStore", "ItemStore", "Guild", "Restaurant" };  // 建物名(ヒエラルキーと同じ英語)
    private readonly Vector3[] buildPos_ = { new Vector3(0.0f,0.0f,110.0f)};
    private Dictionary<string, Vector3> uniPosMap_ = new Dictionary<string, Vector3>();    // キー:英語建物名,値:ユニちゃん表示座標

    void Start()
    {
        // 現在のシーンをTOWNとする
        SceneMng.SetNowScene(SceneMng.SCENE.TOWN);

        // 建物と座標を一致させる
        uniPosMap_.Add(buildNameEng_[0], buildPos_[0]);

        // SceneMngから飛ばす建物名を受けとる(飛ばさなくていいときは処理しないように注意)
        string str = SceneMng.GetHouseName();

        if(str != "Mob")
        {
            // キャラに座標を代入する
            GameObject.Find("Uni").gameObject.transform.position = uniPosMap_[str];
        }

        // 後で使う
        interiorMng_ = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
    }
}
