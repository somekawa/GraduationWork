using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WarpSelectScene : MonoBehaviour
{
    private GameObject[] warpObject_;   // マップ端のワープオブジェを保存


    [SerializeField]
    private GameObject sceneBtnPrefab_; // 行先ボタンを表示するためのプレハブ
    public static GameObject[] btnMng_; // 生成したものを保存するオブジェクト
    private Text[] sceneText_;          // 生成されたものの中からTextを保存
    private RectTransform btnParent_;   // ボタンの親にあたるオブジェクト

    public enum scene
    {
        NON = -1,
        KAIWA,  // 0
        TOWN,   // 1
        HOUSE,  // 2
        FIELD0, // 3
        FIELD1, // 4
        FIELD2, // 5
        FIELD3, // 6
        FIELD4, // 7
        CANCEL, // 8
        MAX
    }

    private string[] sceneName = new string[(int)scene.MAX] {
    "","town","house","field0","field1","field2","field3","field4","cancel"};

    private int nowScene = (int)scene.HOUSE;
    private int stpryProgress_ = (int)scene.FIELD0;// どの章まで進んでいるか

  ////  [SerializeField]
  //  private EventSystem eventSystem;
  //  private GameObject clickbtn_;    // 素材を拾ったときに生成されるプレハブ
    public void Init()
    {
        btnMng_ = new GameObject[(int)scene.MAX];
        sceneText_ = new Text[(int)scene.MAX];
        btnParent_ = GameObject.Find("Canvas/ScrollView/Viewport/Content").GetComponent<RectTransform>();
        for (int i = (int)scene.KAIWA; i < (int)scene.MAX; i++)
        {
            btnMng_[i] = Instantiate(sceneBtnPrefab_, new Vector2(0, 0),
                Quaternion.identity, btnParent_.transform);
            sceneText_[i] = btnMng_[i].transform.GetChild(0).GetComponent<Text>();
            sceneText_[i].text = sceneName[i];
            btnMng_[i].name = sceneName[i];
            if (nowScene == i)
            {
                btnMng_[i].SetActive(false);
            }
        }
        // 現在ストーリ以降のフィールドは非表示
        for (int i = stpryProgress_ + 1; i < (int)scene.CANCEL; i++)
        {
            btnMng_[i].SetActive(false);
        }
        btnMng_[0].SetActive(false);// 0番目はずっと非表示


                                    // マップ端にあるオブジェクトを検索（フィールドによって個数が違うため子の個数で見る）
        warpObject_ = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            warpObject_[i] = this.transform.GetChild(i).gameObject;
            //Debug.Log(warpObject_[i].name + "側のワープ座表" + warpObject_[i].transform.position);
        }
    }


}
