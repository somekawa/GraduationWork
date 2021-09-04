using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject EnemyInstancePointPack;   // 敵の出現位置を設定したもの
    public GameObject EnemyCube;                // テスト用の敵

    // キーがint , valueがList Vector3の[各ワープポイントの数]のmapがいいかも
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();

    void Start()
    {
        // EnemyInstancePointPackの子で回す
        foreach (Transform childTransform in EnemyInstancePointPack.gameObject.transform)
        {
            // リストに座標を一時保存できるようにする
            List<Vector3> posList = new List<Vector3>();

            // EnemyInstancePointPackの孫で回す
            foreach (Transform grandChildTransform in childTransform)
            {
                // リストに座標を保存していく
                posList.Add(grandChildTransform.gameObject.transform.position);
            }

            // 一時保存していた座標リストを、マップへ代入する
            enemyPosSetMap_[int.Parse(childTransform.name)] = posList;
        }
    }

    void Update()
    {

    }

    // 敵のインスタンス処理(配置ポジションをButtleMng.csで指定できるように引数を用意している)
    public void EnemyInstance(int mapNum)
    {
        int num = 1;
        // 指定されたマップのリストを取り出して、foreach文で回す
        foreach(Vector3 pos in enemyPosSetMap_[mapNum])
        {
            //　敵プレハブをインスタンス化
            //Instantiate(EnemyCube, pos, Quaternion.identity);
            GameObject enemy = Instantiate(EnemyCube, pos, Quaternion.identity) as GameObject;
            enemy.name = num.ToString();

            num++;
        }
    }

    // CharacterMng.csに座標情報を渡す
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }
}
