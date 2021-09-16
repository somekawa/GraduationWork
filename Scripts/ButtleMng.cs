using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 戦闘全体について管理する

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public Canvas fieldUICanvas;            // 表示/非表示をこのクラスで管理される

    public int debugEnemyNum = 1;           // インスペクターから敵の生成数を変えれるように 

    private bool setCallOnce_ = false;      // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

    private ImageRotate buttleCommandRotate_;   // バトル中のコマンドUIを取得して、保存しておく変数

    private CharacterMng characterMng_;         // キャラクター管理クラスの情報
    private EnemyInstanceMng enemyInstanceMng_; // 敵インスタンス管理クラスの情報

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        enemyInstanceMng_ = GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>();

        buttleCommandRotate_ = buttleUICanvas.transform.Find("Command").transform.Find("Image").GetComponent<ImageRotate>();
        buttleUICanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // FieldMngで遭遇タイミングを調整しているため、それを参照し、戦闘モード以外ならreturnする
        if (FieldMng.nowMode != FieldMng.MODE.BUTTLE)
        {
            setCallOnce_ = false;

            if(buttleUICanvas.gameObject.activeSelf)
            {
                buttleCommandRotate_.ResetRotate();   // UIの回転を一番最初に戻す
            }
            buttleUICanvas.gameObject.SetActive(false);
            fieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // 戦闘開始時に設定される項目
        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            fieldUICanvas.gameObject.SetActive(false);

            characterMng_.ButtleSetCallOnce();

            // 敵のインスタンス(1～4)
            enemyInstanceMng_.EnemyInstance(debugEnemyNum);

            // Character管理クラスに敵の出現数を渡す
            characterMng_.SetEnemyNum(debugEnemyNum);
        }

        characterMng_.Buttle();

        // キャラクターの攻撃対象が最後の敵だった時
        if(characterMng_.GetLastEnemyToAttackFlg())
        {
            // Enemyタグの数を見て該当する物がない(= 0)なら、MODEを探索に切り替える
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
            }
        }
    }
}
