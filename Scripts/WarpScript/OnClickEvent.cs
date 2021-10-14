using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickEvent : MonoBehaviour
{
    private RectTransform locationSelMng_;        // ワープ先を出すCanvas（親）
    private WarpField warpField_;

    void Start()
    {
        locationSelMng_ = GameObject.Find("DontDestroyCanvas/LocationSelMng").GetComponent<RectTransform>();
        warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();
    }

    public void OnClickSceneBtn()
    {
        Debug.Log("クリックしたボタン"+this.gameObject.name);
        if (warpField_ == null)
        {
            warpField_ = GameObject.Find("WarpOut").GetComponent<WarpField>();
        }

        if (this.gameObject.name == WarpField.btnMng_[(int)SceneMng.SCENE.CANCEL].name)
        {
            // キャンセルが選択されたらキャンバスを非表示にするだけ
            locationSelMng_.gameObject.SetActive(false);
            warpField_.CancelCheck();
            return;
        }

        for (int i = (int)SceneMng.SCENE.TOWN; i < (int)SceneMng.SCENE.CANCEL; i++)
        {
            if (this.gameObject.name == WarpField.btnMng_[i].name)
            {
                // クリックしたボタンの名前と一致していたら
                SceneMng.SceneLoad(i);   // そのシーンに移動する
            }
        }
    }
}
