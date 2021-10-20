using System.Collections.Generic;
using UnityEngine;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject enemyInstancePointPack;   // 敵の出現位置を設定したもの
    public GameObject enemyCube;                // テスト用の敵
    public GameObject enemyHPBar;               // 敵用のHPバー

    // キーがint , valueがList Vector3の[各ワープポイントの数]のmapがいいかも
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;    // フィールド毎の敵情報を保存する

    public static List<(Enemy,HPBar)> enemyList_ = new List<(Enemy, HPBar)>();   // Enemy.csをキャラ毎にリスト化する

    void Start()
    {
        // HPバー表示位置の設定
        // 1体
        Vector3[] tmp1 = new Vector3[1];
        tmp1[0] = new Vector3(1000.0f, 530.0f);
        enemyHPPos_[1] = tmp1;
        // 2体
        Vector3[] tmp2 = new Vector3[2];
        tmp2[0] = new Vector3(870.0f, 560.0f);
        tmp2[1] = new Vector3(1030.0f, 530.0f);
        enemyHPPos_[2] = tmp2;
        // 3体
        Vector3[] tmp3 = new Vector3[3];
        tmp3[0] = new Vector3(780.0f, 570.0f);
        tmp3[1] = new Vector3(940.0f, 540.0f);
        tmp3[2] = new Vector3(1150.0f, 510.0f);
        enemyHPPos_[3] = tmp3;
        // 4体
        Vector3[] tmp4 = new Vector3[4];
        tmp4[0] = new Vector3(760.0f,  580.0f);
        tmp4[1] = new Vector3(830.0f,  540.0f);
        tmp4[2] = new Vector3(1000.0f, 520.0f);
        tmp4[3] = new Vector3(1170.0f, 470.0f);
        enemyHPPos_[4] = tmp4;

        // EnemyInstancePointPackの子で回す
        foreach (Transform childTransform in enemyInstancePointPack.gameObject.transform)
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


        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する

        // 敵(数字がフィールドによって書き換えられるようにしとかないといけない)
        if (enemyData_ == null)
        {
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, 0, name);
        }
    }

    // 敵のインスタンス処理(配置ポジションをButtleMng.csで指定できるように引数を用意している)
    public void EnemyInstance(int mapNum,Canvas parentCanvas)
    {
        enemyList_.Clear(); // 毎回使用前に初期化する

        int num = 1;
        // 指定されたマップのリストを取り出して、foreach文で回す
        foreach(Vector3 pos in enemyPosSetMap_[mapNum])
        {
            // 敵プレハブをインスタンス
            GameObject enemy = Instantiate(enemyCube, pos, Quaternion.identity) as GameObject;
            enemy.name = num.ToString();

            // 敵HPをインスタンス
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum][num - 1], Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]のxは出現させる敵の行番号(いまはカボス固定)
            enemyList_.Add((new Enemy(num.ToString(), 1,null,enemyData_.param[0]), hpBar.GetComponent<HPBar>()));

            // 敵HPの設定
            enemyList_[num - 1].Item2.SetHPBar(enemyList_[num - 1].Item1.HP(), enemyList_[num - 1].Item1.MaxHP());

            num++;
        }
    }

    // CharacterMng.csに座標情報を渡す
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }

    public void HPdecrease(int num)
    {
        StartCoroutine(enemyList_[num].Item2.MoveSlideBar(enemyList_[num].Item1.HP() - 5));
        // 内部数値の変更を行う
        enemyList_[num].Item1.sethp(enemyList_[num].Item1.HP() - 5);

        // もしHPが0になったら、オブジェクトを削除する
        if(enemyList_[num].Item1.HP() <= 0)
        {
            // 敵オブジェクトを削除する(タグ検索)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if(int.Parse(obj.name) == num + 1)
                {
                    Destroy(obj);   // 敵の削除
                }
            }

            Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HPバーの削除
        }
    }
}
