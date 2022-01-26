using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharaBase;

public class EnemyInstanceMng : MonoBehaviour
{
    public GameObject enemyInstancePointPack;   // 敵の出現位置を設定したもの
    public GameObject enemyTest;                // テスト用の敵
    public GameObject enemyHPBar;               // 敵用のHPバー
    public EnemySelect enemySelectObj;          // 敵選択用アイコン
    public Canvas buttleUICanvas;               // バトル中のキャンバス

    [SerializeField]
    private GameObject enemyAttackPrefab_;     // ユニの通常攻撃と同じものでテストする
    [SerializeField]
    private GameObject soulEffect_;             // 敵死亡時の魂的な何か(エフェクト)

    // キーがint , valueがList Vector3の[各ワープポイントの数]のmapがいいかも
    private Dictionary<int, List<Vector3>> enemyPosSetMap_ = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, Vector3[]> enemyHPPos_         = new Dictionary<int, Vector3[]>();
    private Vector3 enemyHPPosOffset_;          // 高い位置の敵用にオフセットできるようにしておく
    private Dictionary<int, GameObject> enemyMap_ = new Dictionary<int, GameObject>();

    private GameObject DataPopPrefab_;
    private EnemyList enemyData_ = null;        // フィールド毎の敵情報を保存する

    public static List<(Enemy,HPMPBar)> enemyList_ = new List<(Enemy, HPMPBar)>();   // Enemy.csをキャラ毎にリスト化する

    private ButtleMng buttleMng_;
    private BadStatusMng badStatusMng_;
    private ANIMATION anim_ = ANIMATION.NON;
    private ANIMATION oldAnim_ = ANIMATION.NON;
    private int mapNum_ = 0;                    // マップ上に配置される敵の数
    private Vector3 enemyPos_;
    private Vector3 charaPos_;
    private int attackTarget_ = -1;             // 攻撃対象(敵からキャラの)

    // Item1:イベントにより強制戦闘する際の敵の種類,Item2:敵の数
    private (GameObject, int) eventEnemy_ = (null, -1);

    private BoxCollider changeEnableBoxCollider_;
    private Dictionary<CONDITION, int>[] enemyBstTurn_ = new Dictionary<CONDITION, int>[4];  // 敵毎のバッドステータス回復までのターン数(最大4体なのでここで確保しとく)

    private GameObject[] enemyBstIconImage_ = new GameObject[4];       // バステアイコン
    private GameObject[] enemyDebuffIconImage_ = new GameObject[4];    // デバフアイコン

    private GameObject buttleDamageIconsObj_;   // バトル中のダメージアイコンの親オブジェクト

    public void Init()
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

        // 敵データ
        // 数字がフィールドによって変更されるように、(現在シーン - FIELD0の番号)とする
        if (enemyData_ == null)
        {
            enemyData_ = DataPopPrefab_.GetComponent<PopList>().GetData<EnemyList>(PopList.ListData.ENEMY, (int)SceneMng.nowScene - (int)SceneMng.SCENE.FIELD0, name);
        }

        var buttlemng = GameObject.Find("ButtleMng");
        buttleMng_ = buttlemng.GetComponent<ButtleMng>();
        badStatusMng_ = buttlemng.GetComponent<BadStatusMng>();

        for(int i = 0; i < 4; i++)
        {
            // バッドステータス持続管理用
            enemyBstTurn_[i] = new Dictionary<CONDITION, int>();
        }

        buttleDamageIconsObj_ = buttleUICanvas.transform.Find("DamageIcons").gameObject;

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

        // AnimMaxの半分の値に到達したらtrueにする
        if (enemyList_[num].Item1.HalfAttackAnimTime())
        {
            if(changeEnableBoxCollider_ != null)
            {
                changeEnableBoxCollider_.enabled = true;
            }
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
        // すでに死亡フラグが立っていたらreturnする
        if (!enemyMap_[num + 1].activeSelf || enemyList_[num].Item1.GetDeathFlg())
        {
            // 死亡しているから非表示にされている
            anim_ = ANIMATION.IDLE;
            return;
        }

        // 初期設定では必ずfalseとする
        buttleMng_.SetIsAttackMagicFlg(false);

        // 自分の位置を取得する
        enemyPos_ = enemyPosSetMap_[mapNum_][num];

        // ランダムなキャラを取得する(ただし、死亡したキャラは除外する)
        //do
        //{
        //    attackTarget_ = Random.Range((int)SceneMng.CHARACTERNUM.UNI, (int)SceneMng.CHARACTERNUM.MAX);    // ユニ以上MAX未満で選択
        //} while (SceneMng.charasList_[attackTarget_].HP() <= 0);
        attackTarget_ = (int)SceneMng.CHARACTERNUM.JACK;

        // 行動前に発動するバッドステータスの処理
        var bst = badStatusMng_.BadStateMoveBefore(enemyList_[num].Item1.GetBS(), enemyList_[num].Item1, enemyList_[num].Item2, false);

        if(bst == (CONDITION.PARALYSIS,true))
        {
            // 麻痺で動けない
            anim_ = ANIMATION.IDLE;
            AfterAttack(num);        // これを呼ばないと、バステやデバフのターンが進まないかも
            return;
        }

        // ダメージと速度を渡す
        buttleMng_.SetDamageNum(enemyList_[num].Item1.Damage());
        buttleMng_.SetSpeedNum(enemyList_[num].Item1.Speed());
        buttleMng_.SetLuckNum(enemyList_[num].Item1.Luck());

        charaPos_ = SceneMng.charasList_[attackTarget_].GetButtlePos();

        Debug.Log("攻撃対象は" + (SceneMng.CHARACTERNUM)attackTarget_);

        // 行動中の敵が、攻撃対象の方向に体を向ける
        enemyMap_[num + 1].transform.localRotation = Quaternion.LookRotation(charaPos_ - enemyPos_);


        // 遠距離攻撃型の敵が使う魔法の弾生成
        if (enemyList_[num].Item1.MoveTime() < 0)
        {
            buttleMng_.SetIsAttackMagicFlg(true);      // 魔法攻撃なのでtrueにする
            GameObject obj = null;
            // enemyAttackPrefab_の子から、今回の敵が使用するエフェクトを名前で探し出す
            for (int m = 0; m < enemyAttackPrefab_.transform.childCount; m++)
            {
                // Excel側では(Clone)まで付けて名前保存してるから名前+(Clone)とする
                var name = enemyAttackPrefab_.transform.GetChild(m).gameObject.name + "(Clone)";
                if (name == enemyList_[num].Item1.WeaponTagObjName())
                {
                    // 今回使用するエフェクトが見つかったときは、そのエフェクトをobj変数に保存する
                    obj = enemyAttackPrefab_.transform.GetChild(m).gameObject;
                    break;
                }
            }

            // 通常攻撃弾の方向の計算
            var dir = (charaPos_ - enemyPos_).normalized;
            // エフェクトの発生位置高さ調整
            // キャラとエフェクトの発生方向が逆だから、forwardの減算に気を付ける事(Z軸はマイナスしないとだめ)
            var adjustPos = new Vector3(enemyPos_.x, enemyPos_.y + 0.3f, enemyPos_.z - transform.forward.z);

            // 通常攻撃弾プレハブをインスタンス化(保存していたobj変数のオブジェクトを使用する)
            var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity);
            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // 通常攻撃弾の飛んでいく方向を指定
            magicMove.SetDirection(dir);
        }

        // [Weapon]のタグがついているオブジェクトを全て検索する
        var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
        int tryParseNum = -1;
        for (int i = 0; i < weaponTagObj.Length; i++)
        {
            // 見つけたオブジェクトの名前を比較して、今回攻撃に扱う武器についているCheckAttackHit関数の設定を行う
            if (weaponTagObj[i].name == enemyList_[num].Item1.WeaponTagObjName())
            {
                // int型に変換できたか確認する
                bool checkChangeInt = int.TryParse(weaponTagObj[i].transform.root.gameObject.name, out tryParseNum);

                if(checkChangeInt)
                {
                    // 変換ができた(近距離攻撃型)
                    // 見つけたものが自分であるか、親の番号を見て判定する
                    if (tryParseNum == (num + 1))
                    {
                        changeEnableBoxCollider_ = weaponTagObj[i].GetComponent<BoxCollider>();

                        // 攻撃対象のキャラの番号を渡す
                        weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_,num);
                    }
                }
                else
                {
                    // 変換が出来なかった(遠距離攻撃型)
                    // 攻撃対象のキャラの番号を渡す
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(attackTarget_,num);
                }
            }
        }

        // 攻撃対象に向かって走る処理
        StartCoroutine(MoveToPlayerPos(num));
    }

    // 攻撃への移動コルーチン  
    private IEnumerator MoveToPlayerPos(int num)
    {
        bool flag = false;

        // 遠距離攻撃型の敵は移動が必要ないから
        if (enemyList_[num].Item1.MoveTime() < 0)
        {
            flag = true;
        }

        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / enemyList_[num].Item1.MoveTime();  // deltaTimeだけだと移動が速すぎるため、任意の値で割る

            var tmp = enemyList_[num].Item1.RunMove(time, enemyMap_[num + 1].transform.localPosition, charaPos_);
            flag = tmp.Item2;   // while文を抜けるかフラグを代入する
            enemyMap_[num + 1].transform.localPosition = tmp.Item1;     // 座標を代入する
        }

        anim_ = ANIMATION.ATTACK;    // 攻撃モーション移行確認切替

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

        // 生成した敵の番号保存用
        int cnt = 0;
        int[] saveNum = new int[mapNum_];

        // 指定されたマップのリストを取り出して、foreach文で回す
        foreach (Vector3 pos in enemyPosSetMap_[mapNum_])
        {
            // 敵プレハブをインスタンス
            GameObject enemy = null;
            enemyHPPosOffset_ = Vector3.zero;

            // 番号でどの敵をインスタンスするか決める
            int enemyNum = Random.Range(0, enemyTest.transform.childCount);
            enemyNum = 3;   // ハチ固定

            if (eventEnemy_.Item1 == null)
            {
                // フィールド上の敵をランダムで出す
                // 子の順とエクセル番号も合わせる必要がある
                enemy = Instantiate(enemyTest.transform.GetChild(enemyNum).gameObject,
                                    pos, Quaternion.identity) as GameObject;
                // 敵の体の向きを変える
                enemy.transform.Rotate(0, 180, 0);

                // 敵がEagleかStoneMonsterかゴーレムときは、HPバーの高さを上のほうに調整しないといけない
                if(enemyTest.transform.GetChild(enemyNum).gameObject.name == "Enemy_Eagle" ||
                   enemyTest.transform.GetChild(enemyNum).gameObject.name == "Enemy_StoneMonster" ||
                   enemyTest.transform.GetChild(enemyNum).gameObject.name == "Enemy_Golem" ||
                   enemyTest.transform.GetChild(enemyNum).gameObject.name == "Enemy_Beholder")
                {
                    enemyHPPosOffset_ = new Vector3(0.0f, 60.0f,0.0f);
                }

            }
            else
            {
                // イベント用の敵を出す
                enemy = Instantiate(eventEnemy_.Item1, pos, Quaternion.identity) as GameObject;
                // イベント用の敵の番号部分をenemyNumとして適用する(エクセル番号も合わせる必要がある)
                enemyNum = int.Parse(eventEnemy_.Item1.name.Split('_')[1]);

                // 敵の体の向きを変える
                enemy.transform.Rotate(0, 180, 0);

                // 敵がボスゴーレムときは、HPバーの高さを上のほうに調整しないといけない
                if (eventEnemy_.Item1.name == "BossGolem_5")
                {
                    enemyHPPosOffset_ = new Vector3(0.0f, 60.0f, 0.0f);
                }
            }

            // 生成したエネミーの番号を保存
            saveNum[cnt] = enemyNum;
            cnt++;

            enemy.name = num.ToString();

            // 敵HPをインスタンス
            GameObject hpBar = Instantiate(enemyHPBar, enemyHPPos_[mapNum_][num - 1] + enemyHPPosOffset_, Quaternion.identity, parentCanvas.transform) as GameObject;
            hpBar.name = "HPBar_"+num.ToString();

            // param[x]のxは出現させる敵の行番号
            // Animatorがアタッチされているか確認(カボスはないからnullをいれる)
            if(enemy.GetComponent<Animator>() == null)
            {
                enemyList_.Add((new Enemy(num.ToString(), 1, null, enemyData_.param[enemyNum]), hpBar.GetComponent<HPMPBar>()));
            }
            else
            {
                enemyList_.Add((new Enemy(num.ToString(), 1, enemy.GetComponent<Animator>(), enemyData_.param[enemyNum]), hpBar.GetComponent<HPMPBar>()));
            }

            // 敵HPの設定
            enemyList_[num - 1].Item2.SetHPMPBar(enemyList_[num - 1].Item1.HP(), enemyList_[num - 1].Item1.MaxHP());

            // 敵オブジェクトを変数に入れる
            enemyMap_.Add(num, enemy);

            // 敵HPの親を、EnemySelectObjにする
            hpBar.transform.SetParent(enemySelectObj.transform);

            // 状態異常用のアイコン(1体の敵に対して3つ)
            enemyBstIconImage_[num - 1] = hpBar.transform.Find("BadStateImages").gameObject;
            // アイコンの透過
            for(int icon = 0; icon < enemyBstIconImage_[num - 1].transform.childCount; icon++)
            {
                enemyBstIconImage_[num - 1].transform.GetChild(icon).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            // デバフ用のアイコン(1体の敵に対して4つ)
            enemyDebuffIconImage_[num - 1] = hpBar.transform.Find("BuffImages").gameObject;
            // アイコンの透過
            for (int icon = 0; icon < enemyDebuffIconImage_[num - 1].transform.childCount; icon++)
            {
                enemyDebuffIconImage_[num - 1].transform.GetChild(icon).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }

            num++;
        }

        // 番号を渡す
        buttleMng_.SetEnemyNum(saveNum);
        
        // 1度読み込んだらeventEnemy_をnullにする
        if (eventEnemy_.Item1)
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
                // 有効にしていたダメージ用コライダーをfalseに戻す
                if(changeEnableBoxCollider_ != null)
                {
                    changeEnableBoxCollider_.enabled = false;
                }

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
                AfterAttack(num);        // 攻撃終了後 
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
        // バステの付与を無しに戻す
        buttleMng_.SetBadStatus(-1, -1);

        enemyList_[num].Item1.Attack();

        // その敵が発動できる状態異常を入れる
        buttleMng_.SetBadStatus(enemyList_[num].Item1.Bst(),-1);
    }

    void AfterAttack(int num)
    {
        // 元いた位置に戻る処理
        StartCoroutine(MoveToInitPos(num));

        // 行動終了後のバッドステータス発動処理
        badStatusMng_.BadStateMoveAfter(enemyList_[num].Item1.GetBS(), enemyList_[num].Item1, enemyList_[num].Item2,false);

        // バステの持続ターン数を-1する
        for (int i = 0; i < (int)CONDITION.DEATH; i++)
        {
            // キーが存在しなければとばす
            if (!enemyBstTurn_[num].ContainsKey((CONDITION)i))
            {
                continue;
            }
            // -1ターンする
            enemyBstTurn_[num][(CONDITION)i]--;
            // ターン数が0以下になったら、マップから削除する
            if (enemyBstTurn_[num][(CONDITION)i] <= 0)
            {
                enemyList_[num].Item1.ConditionReset(false, i);    // 0以下になったものだけ回復
                enemyBstTurn_[num].Remove((CONDITION)i);
                badStatusMng_.SetBstIconImage(num, -1, enemyBstIconImage_, enemyList_[num].Item1.GetBS(),true);
            }
        }

        // デバフを1ターン減少させる
        if (!enemyList_[num].Item1.CheckBuffTurn())
        {
            // falseの状態(=何かのデバフがきれたら)
            var buffMap = enemyList_[num].Item1.GetBuff();
            for (int i = 0; i < buffMap.Count; i++)
            {
                if (buffMap[i + 1].Item2 > 0)
                {
                    continue;
                }

                // 効果が切れた(=ターンが0以下)
                var child = enemyDebuffIconImage_[num].transform.GetChild(i);
                if (child.GetComponent<Image>().sprite != null)
                {
                    // アイコンをnullにして、低下矢印も非表示にする
                    child.GetComponent<Image>().sprite = null;
                    child.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    for (int m = 0; m < child.childCount; m++)
                    {
                        child.GetChild(m).gameObject.SetActive(false);
                    }
                }
            }
        }

        // 全ての状態異常が治ったとき
        if (enemyBstTurn_[num].Count <= 0)
        {
            // CONDITIONをNONに戻す
            enemyList_[num].Item1.ConditionReset(true);
            badStatusMng_.SetBstIconImage(num, -1, enemyBstIconImage_, enemyList_[num].Item1.GetBS(), true);
            Debug.Log("敵状態異常が全て治った");
        }
    }

    // 攻撃から戻ってくるコルーチン  
    private IEnumerator MoveToInitPos(int num)
    {
        bool flag = false;

        // 遠距離攻撃型の敵は移動が必要ないから または、攻撃後自爆してHPが0の敵は移動が必要ない
        if (enemyList_[num].Item1.MoveTime() < 0 || enemyList_[num].Item1.HP() <= 0)
        {
            flag = true;
        }

        // y軸を0.0fにしておかないと空中へ向かって戻ることになる
        var tmpEnemyPos_ = new Vector3(enemyPosSetMap_[mapNum_][num].x, 0.0f, enemyPosSetMap_[mapNum_][num].z);

        float time = 0.0f;
        while (!flag)
        {
            yield return null;

            time += Time.deltaTime / (enemyList_[num].Item1.MoveTime() / 2.0f);  // deltaTimeだけだと移動が速すぎるため、任意の値で割る

            var tmp = enemyList_[num].Item1.BackMove(time, enemyMap_[num + 1].transform.localPosition, tmpEnemyPos_);
            flag = tmp.Item2;   // while文を抜けるかフラグを代入する
            enemyMap_[num + 1].transform.localPosition = tmp.Item1;     // 座標を代入する
        }

        anim_ = ANIMATION.IDLE;

    }

    // CharacterMng.csに座標情報を渡す
    public Dictionary<int, List<Vector3>> GetEnemyPos()
    {
        return enemyPosSetMap_;
    }

    public void HPdecrease(int num,bool refFlg = false)
    {
        // すでにHPが0を下回っていたら処理を抜ける(魔法攻撃連続ヒット用のガード処理)
        if (enemyList_[num].Item1.HP() <= 0)
        {
            return;
        }

        int hitProbabilityOffset = 0; 
        var damage = 0;
        bool weakFlg = false;
        bool criticalFlg = false;

        if(refFlg || buttleMng_.GetAutoHit())
        {
            // 防御貫通
            // 反射時は必中
            damage = buttleMng_.GetDamageNum();

            if(buttleMng_.GetAutoHit())
            {
                // 攻撃側の属性と自分の弱点属性が一致してたらダメージ量を2倍にする
                if (enemyList_[num].Item1.Weak() == buttleMng_.GetElement())
                {
                    damage *= 2;
                    Debug.Log("敵の弱点属性！ ダメージ量2倍で" + damage);
                    weakFlg = true;
                }
                buttleMng_.SetAutoHit(false);
            }
        }
        else
        {
            // クリティカルの計算をする(基礎値と幸運値で上限を狭める)
            int criticalRand = Random.Range(0, 100 - (10 + buttleMng_.GetLuckNum()));
            if (criticalRand <= 10 + buttleMng_.GetLuckNum())
            {
                // クリティカル発生(必中+ダメージ2倍)10はクリティカルの基礎値
                Debug.Log(criticalRand + "<=" + (10 + buttleMng_.GetLuckNum()) + "なので、キャラの攻撃がクリティカル！");
                // クリティカルダメージ
                damage = (buttleMng_.GetDamageNum() * 2) - enemyList_[num].Item1.Defence(true);

                hitProbabilityOffset = 200; // 100以上の数字が必要になる(バステ付与時に幸運値+ランダムが100を越える可能性があるから)
                criticalFlg = true;
            }
            else
            {
                // クリティカルじゃないとき
                Debug.Log(criticalRand + ">" + (10 + buttleMng_.GetLuckNum()) + "なので、キャラの攻撃はクリティカルではない");

                // 命中計算をする
                // ①攻撃する側のSpeed / 攻撃される側のSpeed * 100 = ％の出力
                var hitProbability = (int)((float)buttleMng_.GetSpeedNum() / (float)enemyList_[num].Item1.Speed() * 100.0f);
                // ②キャラも敵も+10％の補正値を入れてキャラ側だけに(自分のLuck * 5)％をプラスする。
                hitProbabilityOffset = hitProbability + 10 + (buttleMng_.GetLuckNum() * 5);
                // ③hitProbabilityOffsetが100以上なら自動命中で、それ以下ならランダム値を取る。
                if (hitProbabilityOffset < 100)
                {
                    int rand = Random.Range(0, 100);
                    Debug.Log("命中率" + hitProbabilityOffset + "ランダム値" + rand);

                    if (rand <= hitProbabilityOffset)
                    {
                        // 命中
                        Debug.Log(rand + "<=" + hitProbabilityOffset + "なので、命中");
                        damage = buttleMng_.GetDamageNum() - enemyList_[num].Item1.Defence(true);
                    }
                    else
                    {
                        // 回避
                        Debug.Log(rand + ">" + hitProbabilityOffset + "なので、回避");
                        DamageIcon(num, "ミス",false,false);
                        return;
                    }
                }
                else
                {
                    Debug.Log("命中率" + hitProbabilityOffset + "が100以上ならので、自動命中");
                    damage = buttleMng_.GetDamageNum() - enemyList_[num].Item1.Defence(true);
                }
            }

            // 攻撃側の属性と自分の弱点属性が一致してたらダメージ量を2倍にする
            if (enemyList_[num].Item1.Weak() == buttleMng_.GetElement())
            {
                damage *= 2;
                Debug.Log("敵の弱点属性！ ダメージ量2倍で" + damage);
                weakFlg = true;
            }


            // ダメージ値の算出
            if (damage <= 0)
            {
                Debug.Log("キャラの攻撃力より敵の防御力が上回ったのでダメージが1になりました");
                damage = 1;
            }
        }

        // ダメージアイコン
        DamageIcon(num, damage.ToString(),weakFlg,criticalFlg);

        // バッドステータスが付与されるか判定
        enemyList_[num].Item1.SetBS(buttleMng_.GetBadStatus(), hitProbabilityOffset);

        var getBs = enemyList_[num].Item1.GetBS();
        // バステの効果持続ターンを設定する
        for (int i = 0; i < (int)CONDITION.DEATH; i++)
        {
            // 健康状態以外のコンディションのフラグがtrueになっていたら
            if (getBs[i].Item2 && getBs[i].Item1 != CONDITION.NON)
            {
                if (!enemyBstTurn_[num].ContainsKey(getBs[i].Item1))
                {
                    enemyBstTurn_[num].Add(getBs[i].Item1, Random.Range(1, 5));// 1以上5未満
                    badStatusMng_.SetBstIconImage(num, (int)enemyList_[num].Item1.GetBS()[i].Item1, enemyBstIconImage_, enemyList_[num].Item1.GetBS());
                }
            }
        }

        StartCoroutine(enemyList_[num].Item2.MoveSlideBar(enemyList_[num].Item1.HP() - damage, enemyList_[num].Item1.HP()));
        // 内部数値の変更を行う
        enemyList_[num].Item1.SetHP(enemyList_[num].Item1.HP() - damage);

        // 即死用に処理呼び出し
        var bst = badStatusMng_.BadStateMoveBefore(getBs, enemyList_[num].Item1, enemyList_[num].Item2, false);
        if(bst == (CONDITION.DEATH,true))   // 即死処理
        {
            StartCoroutine(enemyList_[num].Item2.MoveSlideBar(enemyList_[num].Item1.HP() - 999, enemyList_[num].Item1.HP()));
            // 内部数値の変更を行う
            enemyList_[num].Item1.SetHP(enemyList_[num].Item1.HP() - 999);
        }

        // もしHPが0になったら、オブジェクトを削除する
        if (enemyList_[num].Item1.HP() <= 0)
        {
            // ここで矢印処理を呼び出す(HP減少処理→矢印処理としないと、HPが0の場所に矢印が出てしまうから)
            enemySelectObj.ResetSelectPoint();

            // 敵オブジェクトを削除する(タグ検索)
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (int.Parse(obj.name) == num + 1)
                {
                    // エフェクトの生成
                    //Instantiate(soulEffect_, enemyMap_[num + 1].transform.position, Quaternion.identity);
                    //Destroy(obj);   // 敵の削除

                    // 死亡用のフラグを立てる
                    enemyList_[num].Item1.SetDeathFlg(true);
                }
            }

            GameObject.Find(enemyList_[num].Item2.name).SetActive(false);   // HPバーの非表示
            //Destroy(GameObject.Find(enemyList_[num].Item2.name));   // HPバーの削除(削除したらエラーになるから、あとから削除する)
        }
    }

    // num:敵番号,tail:威力,debuff:減少効果内容
    public void Debuff(int num,int tail,int debuff)
    {
        // バフのアイコン処理
        System.Action<int, int> action = (int charaNum, int buffnum) => {
            var bufftra = enemyDebuffIconImage_[charaNum].transform;
            for (int i = 0; i < bufftra.childCount; i++)
            {
                if (bufftra.GetChild(i).GetComponent<Image>().sprite == null)
                {
                    // アイコンをいれる
                    bufftra.GetChild(i).GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][debuff - 1];
                    bufftra.GetChild(i).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                    // 矢印でアップ倍率をいれる
                    // ▲*1 = バフが1% ~30%,▲*2 = バフが31% ~70%,▲*3 = バフが71%~100%
                    for (int m = 0; m < bufftra.GetChild(i).childCount; m++)
                    {
                        if (m <= buffnum)    // buffnumの数字以下ならtrueにして良い
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(true);
                        }
                        else
                        {
                            bufftra.GetChild(i).GetChild(m).gameObject.SetActive(false);
                        }
                    }

                    break;
                }
            }
        };

        var debuffnum = enemyList_[num].Item1.SetBuff(tail, debuff);

        if (!debuffnum.Item2)
        {
            action(num, debuffnum.Item1);
        }
    }

    public void SetEnemySpawn(GameObject obj,int num)
    {
        eventEnemy_ = (obj, num);
    }

    public void NotMyTurn(int refNum)
    {
        if(refNum >= 0) // 攻撃がキャラによって反射されたとき
        {
            // 攻撃をしかけた敵のHPが減るようにする
            HPdecrease(refNum,true);
            buttleMng_.SetRefEnemyNum(-1);
        }

        // ダメージを受けたときのモーションを規定時間で終了させる
        for (int i = 0; i < enemyMap_.Count; i++)
        {
            enemyList_[i].Item1.DamageAnim();
        }

        // 敵の死亡処理
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            // すでに非表示にされたものは飛ばす
            if(obj.activeSelf)
            {
                if (enemyList_[int.Parse(obj.name) - 1].Item1.GetDeathFlg() && enemyList_[int.Parse(obj.name) - 1].Item1.DeathAnim())
                {
                    // エフェクトの生成
                    Instantiate(soulEffect_, enemyMap_[int.Parse(obj.name)].transform.position, Quaternion.identity);

                    // 直ぐに削除せずに、非表示切替にする
                    obj.SetActive(false);
                }
            }
        }
    }

    public void DeleteEnemy()
    {
        // 敵とそのHPバーの削除処理
        for(int i = 0; i < enemyMap_.Count; i++)
        {
            Debug.Log("敵" + i + "を削除しました");
            Destroy(enemyMap_[i + 1]);
            Destroy(GameObject.Find(enemyList_[i].Item2.name));
        }

        // 毎回使用前に初期化する
        enemyList_.Clear();
        enemyMap_.Clear();
    }

    public bool AllAnimationFin()
    {
        Debug.Log("AllAnimationFin処理");
        for (int i = 0; i < enemyMap_.Count; i++)
        {
            // まだ1体でも死亡アニメーション中ならfalseで返す
            if(enemyMap_[i + 1].activeSelf)
            {
                return false;
            }
        }

        // Finishタグがついているものを検索して、あるときはfalseで返す
        if (GameObject.FindGameObjectsWithTag("Finish").Length > 0)
        {
            return false;
        }


        // 全ての敵が死亡アニメーションまで終了したらtrueで返す
        return true;
    }

    // キャラからのアイテムによる固定ダメージ
    public void ItemDamage()
    {
        // 対象は全てのエネミー
        for(int i = 0; i < enemyList_.Count; i++)
        {
            HPdecrease(i,true);
            enemyList_[i].Item1.DamageAnim();
        }
    }

    // 行動ターン数字を代入する
    public void SetMoveSpeedNum(int num, string enemyNum)
    {
        if(enemySelectObj.transform.Find("HPBar_" + enemyNum + "/MoveSpeed"))
        {
            var tmp = enemySelectObj.transform.Find("HPBar_" + enemyNum + "/MoveSpeed").GetComponent<TMPro.TextMeshProUGUI>();
            tmp.text = num.ToString();
            if(SceneMng.nowScene == SceneMng.SCENE.FIELD3)
            {
                // 文字の色を白にする
                tmp.color = new Color(1.0f, 1.0f, 1.0f);
            }
            else
            {
                // 文字の色を黒にする
                tmp.color = new Color(0.0f, 0.0f, 0.0f);
            }
        }
    }

    private void DamageIcon(int num,string str, bool weakFlg, bool criticallFlg)
    {
        // ダメージアイコン
        for (int i = 0; i < buttleDamageIconsObj_.transform.childCount; i++)
        {
            var tmp = buttleDamageIconsObj_.transform.GetChild(i).gameObject;
            if (tmp.activeSelf)
            {
                continue;
            }

            // まだ非表示状態の使われていないアイコンを見つけたとき
            // 座標を被ダメージキャラの頭上にする
            buttleDamageIconsObj_.transform.GetChild(i).transform.position = enemyHPPos_[mapNum_][num];
            // ダメージ数値を入れる
            tmp.transform.GetChild(0).GetComponent<Text>().text = str;
            // 表示状態にする
            tmp.SetActive(true);

            tmp.transform.GetChild(0).Find("Weak").gameObject.SetActive(weakFlg);
            tmp.transform.GetChild(0).Find("Critical").gameObject.SetActive(criticallFlg);

            break;
        }
    }
}
