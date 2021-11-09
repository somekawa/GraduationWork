using System.Collections;
using UnityEngine;

public class ForcedButtle : MonoBehaviour
{
    public GameObject eventEnemy;                // 強制戦闘時の敵をアタッチする
    public int eventEnemyNum;                    // 強制戦闘時の敵の数を外部から指定する

    private GameObject uniChan_;                 // ユニ
    private UnitychanController controller_;     // ユニの操作状態
    private FieldMng fieldMng_;

    void Start()
    {
        // 座標移動に使用する
        uniChan_ = GameObject.Find("Uni");
        // ユニのコントローラーを取得する
        controller_ = GameObject.Find("Uni").GetComponent<UnitychanController>();

        fieldMng_ = GameObject.Find("FieldMng").GetComponent<FieldMng>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            StartCoroutine(SelectForcedButtle());
        }
    }

    // コルーチン  
    private IEnumerator SelectForcedButtle()
    {
        // アクティブにする
        fieldMng_.ChangeFieldUICanvasPopUpActive(-1, -1, false);

        bool tmpFlg = true;    // true:はい,false:いいえ
        fieldMng_.MoveArrowIcon(tmpFlg);

        // ユニのアニメーションを止める
        controller_.StopUniRunAnim();
        // 選択肢を選ぶ間は、コントローラー操作できなくしておく
        controller_.enabled = false;

        while (!controller_.enabled)
        {
            yield return null;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("選択肢「いいえ」");
                tmpFlg = false;
                fieldMng_.MoveArrowIcon(tmpFlg);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("選択肢「はい」");
                tmpFlg = true;
                fieldMng_.MoveArrowIcon(tmpFlg);
            }
            else
            {
                // 何も処理を行わない
            }

            // スペースキーで選択肢を決定し、enabledをtrueにすることでwhile文から抜けるようにする
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("選択肢の決定");
                // 操作可能状態にする
                controller_.enabled = true;
                // 非アクティブにするために再度呼び出す
                fieldMng_.ChangeFieldUICanvasPopUpActive(-1, -1, false);

                if (tmpFlg)
                {
                    // はい
                    // 敵の種類と数を指定する
                    GameObject.Find("EnemyInstanceMng").GetComponent<EnemyInstanceMng>().SetEnemySpawn(eventEnemy, eventEnemyNum);

                    // 強制戦闘が発生する
                    FieldMng.nowMode = FieldMng.MODE.BUTTLE;
                    Debug.Log("ユニが強制戦闘用の壁を通過しました");

                    // オブジェクトを非アクティブにする(非アクティブにしないと、連続で戦闘が発生する)
                    this.gameObject.SetActive(false);
                }
                else
                {
                    // いいえ
                    // -transform.forwardが後ろ
                    Vector3 velocity = (-gameObject.transform.forward) * 3.0f;
                    uniChan_.transform.position += velocity;
                }
            }
        }
    }

}
