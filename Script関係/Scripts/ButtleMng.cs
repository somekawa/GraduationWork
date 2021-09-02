using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 戦闘全体について管理する

public class ButtleMng : MonoBehaviour
{
    public Canvas buttleUICanvas;           // 表示/非表示をこのクラスで管理される
    public Canvas FieldUICanvas;            // 表示/非表示をこのクラスで管理される

    private bool setCallOnce_ = false;      // 戦闘モードに切り替わった最初のタイミングだけ切り替わる

    private ImageRotate buttleCommandUI_;   // バトル中のコマンドUIを取得して、保存しておく変数

    private CharacterMng characterMng_;     // キャラクター管理クラスの情報

    void Start()
    {
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
        buttleCommandUI_ = buttleUICanvas.transform.Find("Image").GetComponent<ImageRotate>();
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
                buttleCommandUI_.ResetRotate();   // UIの回転を一番最初に戻す
            }
            buttleUICanvas.gameObject.SetActive(false);
            FieldUICanvas.gameObject.SetActive(true);
            return;
        }

        // 戦闘開始時に設定される項目
        if(!setCallOnce_)
        {
            setCallOnce_ = true;
            buttleUICanvas.gameObject.SetActive(true);
            FieldUICanvas.gameObject.SetActive(false);

            characterMng_.ButtleSetCallOnce();
        }

        characterMng_.Buttle();
    }
}
