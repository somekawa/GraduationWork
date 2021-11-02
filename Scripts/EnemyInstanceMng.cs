using System.Collections.Generic;
using UnityEngine;
using static CharaBase;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject enemyInstancePointPack;   // 敵の出現位置を設定したもの
    public GameObject enemyTest;                // テスト用の敵
    public GameObject enemyHPBar;               // 敵用のHPバー
    public EnemySelect enemySelectObj;          // 敵選択用アイコン
                                                // 通常攻撃弾のプレハブ
    [SerializeField]
    private GameObject kabosuAttackPrefab_;     // ユニの通常攻撃と同じものでテストする
    [SerializeField]
    private GameObject soulEffect_;             // 敵死亡時の魂的な何か(エフェクト)

    // キーがint , valueがList Vector3の[各ワープポイントの数]のmapがいいかも
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();
    private Dictionary<int, GameObject> enemyMap_ = new Dictionary<int, GameObject>();

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;        // フィールド毎の敵情報を保存する

    public static List<(Enemy,HPBar)> enemyList_ = new List<(Enemy, HPBar)>();   // Enemy.csをキャラ毎にリスト化する

    private ButtleMng buttleMng_;
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;
    private int mapNum_ = 0;                    // マップ上に配置される敵の数
    private Vector3 enemyPos_;
    private Vector3 charaPos_;
    private int attackTarget_ = -1;             // 攻撃対象(敵からキャラの)

    // Item1:イベントにより強制戦闘する際の敵の種類,Item2:敵の数
    private (GameObject, int) eventEnemy_ = (null, -1);     

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

        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();

        // 乱数の値の元になる値(=シード値)を現在の時間をつかって初期化する
        // →シード値を変更しなければ規則的に同じ順番で同じ番号が生成されてしまうから
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    // ButtleMng.csでUpdate関数のように使用する
    public void Buttle(int num)
    {
        if(anim_ == ANIMATION.NON)
        {
            anim_ = ANIMATION.BEFORE;
        }

        if (anim_ == ANIMATION.ATTACK && enemyList_[num].Item1.ChangeNextChara())
        {
            anim_ = ANIMATION.AFTER;
        }

        if (oldAnim_ != anim_)
        {
            oldAnim_ = anim_;
            AnimationChange(num);
        }
    }

    private void BeforeAttack(int num)
    {
        if(enemyMap_[num + 1] == null)
        {
            // 死亡しているからDestroyされている
            anim_ = ANIMATION.IDLE;
            return;
        }

        // 自分の位置を取得する
        enemyPos_ = enemyPosSetMap_[mapNum_][num];

        // ランダムなキャラを取得する(ただし、死亡したキャラは除外する)
        do
        {
            attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);    // ユニ以上MAX未満で選択
        } while (SceneMng.charasList_[attackTarget_].HP() <= 0);
        //attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);

        charaPos_ = SceneMng.charasList_[attackTarget_].GetButtlePos();

        Debug.Log("攻撃対象は" + (SceneMng.CHARACTERNUM)attackTarget_);

        // 行動中の敵が、攻撃対象の方向に体を向ける
        enemyMap_[num + 1].transform.localRotation = Quaternion.LookRotation(charaPos_ - enemyPos_);

        anim_ = ANIMATION.ATTACK;    
    }

    // 敵のインスタンス処理(配置ポジションをButtleMng.csで指定できるように引数を用意している)
    public int EnemyInstance(int mapNum,Canvas parentCanvas)
    {
        // イベント用の変数が1以上の値だった場合、イベント用数値を優先する
        if (eventEnemy_.Item2 >= 1)
        {
            mapNum_ = eventEnemy_.Item2;
        }
        else
        {
            mapNum_ = mapNum;
        }

        // 毎回使用前に初期化する
        enemyList_.Clear(); 
        enemyMap_.Clear();

        int num = 1;
        // 指定されたマップのリストを取り出して、foreach文で回す
        foreach(Vector3 pos in enemyPosSetMap_[mapNum_])
        {
            // 敵プレハブをインスタンス
            GameObject enemy = null;

            if(eventEnemy_.Item1 == null)
            {
                // フィールド上の敵をランダムで出す
                enemy = Instantiate(enemyTest, pos, Quaternion.identity) as GameObject;
            }
            else
            {
                // イベント用の敵を出す
                enemy = Instantiate(eventEnemy_.Item1, pos, Quaternion.identity) as GameObject;
            }

            enemy.name = num.ToString();

            // 敵HPをインスタンス
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum_][num - 1], Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]のxは出現させる敵の行番号(いまはカボス固定)
            enemyList_.Add((new Enemy(num.ToString(), 1,null,enemyData_.param[0]), hpBar.GetComponent<HPBar>()));

            // 敵HPの設定
            enemyList_[num - 1].Item2.SetHPBar(enemyList_[num - 1].Item1.HP(), enemyList_[num - 1].Item1.MaxHP());

            // 敵オブジェクトを変数に入れる
            enemyMap_.Add(num, enemy);

            num++;
        }

        // 1度読み込んだらeventEnemy_をnullにする
        if(eventEnemy_.Item1)
        {
            eventEnemy_ = (null, -1);
        }

        return mapNum_;
    }

    void AnimationChange(int num)
    {
        switch (anim_)
        {
            case ANIMATION.IDLE:
                // 行動が終わったからターンを移す
                buttleMng_.SetMoveTurn();
                anim_ = ANIMATION.NON;
                break;
            case ANIMATION.BEFORE:
                BeforeAttack(num);    // 攻撃準備
                break;
            case ANIMATION.ATTACK:    // 実際の攻撃処理
                Attack(num);
                break;
            case ANIMATION.AFTER:
                AfterAttack();        // 攻撃終了後 
                break;
            //case ANIMATION.DEATH:
            //    break;
            default:
                break;
        }
    }

    public (int,string) EnemyTurnSpeed(int num)
    {
        return (enemyList_[num].Item1.Speed(), enemyList_[num].Item1.Name());
    }

    private void Attack(int num)
    {
        enemyList_[num].Item1.Attack();

        AttackStart(num);
    }

    private void AttackStart(int num)
    {
        // ダメージを渡す
        buttleMng_.SetDamageNum(enemyList_[num].Item1.Damage());

        string str = "";

        // 通常攻撃弾の方向の計算
        var dir = (charaPos_ - enemyPos_).normalized;
        // エフェクトの発生位置高さ調整
        // キャラとエフェクトの発生方向が逆だから、forwardの減算に気を付ける事(Z軸はマイナスしないとだめ)
        var adjustPos = new Vector3(enemyPos_.x, enemyPos_.y + 0.3f, enemyPos_.z - transform.forward.z);

        // 通常攻撃弾プレハブをインスタンス化
        var uniAttackInstance = Instantiate(kabosuAttackPrefab_, adjustPos, Quaternion.identity);
        MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
        // 通常攻撃弾の飛んでいく方向を指定
        magicMove.SetDirection(dir);

        // 名前の設定
        str = "KabosuAttack(Clone)";

        if (str == "")
        {
            Debug.Log("エラー：文字が入っていません");
            return; // 文字が入っていない場合はreturnする
        }

        // [Weapon]のタグがついているオブジェクトを全て検索する
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // 見つけたオブジェクトの名前を比較して、今回攻撃に扱う武器についているCheckAttackHit関数の設定を行う
            if (weaponTagObj[i].name == str)
            {
                // 攻撃対象のキャラの番号を渡す
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_);
            }
        }
    }

    void AfterAttack()
    {
        anim_ = ANIMATION.IDLE;
    }

    // CharacterMng.csに座標情報を渡す
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }

    public void HPdecrease(int num)
    {
        // ダメージ値の算出
        var damage = buttleMng_.GetDamageNum() - enemyList_[num].Item1.Defence();
        if(damage <= 0)
        {
            Debug.Log("キャラの攻撃力より敵の防御力が上回ったのでダメージが0になりました");
            damage = 0;
        }

        StartCoroutine(enemyList_[num].Item2.MoveSlideBar(enemyList_[num].Item1.HP() - damage));
        // 内部数値の変更を行う
        enemyList_[num].Item1.sethp(enemyList_[num].Item1.HP() - damage);

        // もしHPが0になったら、オブジェクトを削除する
        if(enemyList_[num].Item1.HP() <= 0)
        {
            // ここで矢印処理を呼び出す(HP減少処理→矢印処理としないと、HPが0の場所に矢印が出てしまうから)
            enemySelectObj.ResetSelectPoint();

            // 敵オブジェクトを削除する(タグ検索)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (int.Parse(obj.name) == num + 1)
                {
                    // エフェクトの生成
                    Instantiate(soulEffect_, enemyMap_[num + 1].transform.position, Quaternion.identity);

                    Destroy(obj);   // 敵の削除
                }
            }

            Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HPバーの削除
        }
    }

    public void SetEnemySpawn(GameObject obj,int num)
    {
        eventEnemy_ = (obj, num);
    }
}
