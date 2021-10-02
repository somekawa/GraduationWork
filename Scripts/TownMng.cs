using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMng : MonoBehaviour
{
    [SerializeField]
    private Canvas inHouseCanvas;

    private readonly string[] buildNameEng_ = { "MayorHouse", "BookStore", "ItemStore", "Guild", "Restaurant" };  // 建物名(ヒエラルキーと同じ英語)
    private readonly Vector3[] buildPos_ = { new Vector3(0.0f, 0.0f, 110.0f),   
                                             new Vector3(0.0f, 0.0f, 110.0f),
                                             new Vector3(0.0f, 0.0f, 110.0f),
                                             new Vector3(23.0f, 0.0f, 96.0f),
                                             new Vector3(0.0f, 0.0f, 110.0f) };
    private Dictionary<string, Vector3> uniPosMap_ = new Dictionary<string, Vector3>();    // キー:英語建物名,値:ユニちゃん表示座標

    void Start()
    {
        // 現在のシーンをTOWNとする
        SceneMng.SetNowScene(SceneMng.SCENE.TOWN);

        // 建物と座標を一致させる
        for(int i = 0; i < buildNameEng_.Length; i++)
        {
            uniPosMap_.Add(buildNameEng_[i], buildPos_[i]);
        }

        // SceneMngから飛ばす建物名を受けとる(飛ばさなくていいときは処理しないように注意)
        string str = SceneMng.GetHouseName();

        // WarpTown.csの初期化関数を先に呼ぶ
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();

        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();

        if (str != "Mob")
        {
            // キャラに座標を代入する
            // WarpTown.csのStart関数が後から呼ばれてキャラの座標を書き換えてしまうから正しく設定できない
            // WarpTown.csのStart関数を先に呼ぶようにしよう!
            GameObject.Find("Uni").gameObject.transform.position = uniPosMap_[str];

            var temp = GameObject.Find("HouseInterior").GetComponent<HouseInteriorMng>();
            temp.SetHouseVisible(str);

            // ここでサブカメラに切り替えたいのにうまくいかない
            // →後からCameraMng.csのStart関数でサブカメラがfalseにされているせいだった
            // CameraMng.csのStart関数をなくして、カメラの切替処理はすべてSetChangeCamera関数で対応する
            cameraMng_.SetSubCameraPos(new Vector3(100.0f, 0.3f, 0.0f));
            cameraMng_.SetSubCameraRota(Quaternion.Euler(new Vector3(13.5f, 0.0f, 0.0f)));
            cameraMng_.SetChangeCamera(true);

            // 室内キャンバスの表示(非表示の状態から取得が必要になる為、SerializeFieldを使って外部アタッチしている)
            inHouseCanvas.gameObject.SetActive(true);
            temp.ChangeObjectActive(inHouseCanvas.gameObject.transform.childCount, inHouseCanvas.transform, str);

            temp.SetWarpCanvasAndCharaController(false);

            // 現在の建物名を保存(カメラ位置調整に必要)
            temp.SetInHouseName(str);
        }
        else
        {
            // メインカメラをアクティブにする
            cameraMng_.SetChangeCamera(false);
        }
    }
}
