using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickEvent : MonoBehaviour
{
    private EventSystem eventSystem;    // ボタンクリックのためのイベント処理
    private GameObject clickbtn_;       // どのボタンをクリックしたか代入する変数

    private RectTransform locationSelMng_;        // ワープ先を出すCanvas（親）
    private WarpField warpField_;

    void Start()
    {
        locationSelMng_ = GameObject.Find("DontDestroyCanvas/LocationSelMng").GetComponent<RectTransform>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();
    }

    public void OnClickSceneBtn()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        if (clickbtn_.name == WarpField.btnMng_[(int)SceneMng.SCENE.CANCEL].name)
        {
            // キャンセルが選択されたらキャンバスを非表示にするだけ
            locationSelMng_.gameObject.SetActive(false);
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
}
