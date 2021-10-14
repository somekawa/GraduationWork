using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickEvent : MonoBehaviour
{
    private EventSystem eventSystem;// ボタンクリックのためのイベント処理

    private Canvas locationSelCanvas_;        // ワープ先を出すCanvas（親）
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数

    private WarpField warpField_;
   // private MenuActive menuActive_;

    void Start()
    {
        locationSelCanvas_ = GameObject.Find("LocationSelCanvas").GetComponent<Canvas>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        warpField_ = GameObject.Find("Mng").GetComponent<WarpField>();
       // menuActive_ = GameObject.Find("SceneMng").GetComponent<MenuActive>();
    }

    public void OnClickSceneBtn()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        if (clickbtn_.name == WarpField.btnMng_[(int)SceneMng.SCENE.CANCEL].name)
        {
            // キャンセルが選択されたらキャンバスを非表示にするだけ
            locationSelCanvas_.gameObject.SetActive(false);
            warpField_.CancelCheck();
            return;
        }

        for (int i = (int)SceneMng.SCENE.TOWN; i < (int)SceneMng.SCENE.CANCEL; i++)
        {
            if (clickbtn_.name == WarpField.btnMng_[i].name)
            {
                // クリックしたボタンの名前と一致していたら
                SceneMng.SceneLoad(i);   // そのシーンに移動する
            }
        }
    }



    //public void OnClickMenuBtn()
    //{
    //    clickbtn_ = eventSystem.currentSelectedGameObject;
    //    for (int i = (int)MenuActive.CANVAS.BAG; i < (int)MenuActive.CANVAS.MAX; i++)
    //    {
    //        if (clickbtn_.name == MenuActive.parentRectTrans_[i].name)
    //        {
    //            // クリックしたボタンの名前と一致していたら
    //            menuActive_.ActiveMng(i);
    //        }
    //    }

    //}
}
