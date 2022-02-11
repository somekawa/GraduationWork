using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Endroll : MonoBehaviour
{
    public RectTransform scrollTextTrans;

    private Vector3 scrollTextPos_;
    private const float endPosY = 850.0f;       // スクロール終了座標
    private float scrollMove_ = 0.0f;           // どれぐらいスクロールが動いたか
    private bool scrollFinFlg_ = false;         // スクロール処理が終了したかどうか
    private GameObject buttons_;

    // 画像表示順配列
    private string[] pictureName_ =
    {
        "UniHouse",
        "MayorHouse",
        "Guild",
        "ItemStore",
        "BookStore",
        "Restaurant",
    };
    private float changePictureTime_ = 0.0f;  // どれぐらいスクロールしたら画像を切り替えるか
    private int pictureNowNum_ = 0;             // 現在の表示中画像

    private Image objectImage_;                 // スクロール中に登場する画像
    private List<Texture2D> texture2dList = new List<Texture2D>();
    private List<Sprite> spriteList = new List<Sprite>();

    void Start()
    {
        scrollTextPos_ = scrollTextTrans.anchoredPosition;

        var canvas = GameObject.Find("Canvas").transform;
        objectImage_ = canvas.Find("Image").GetComponent<Image>();
        buttons_ = GameObject.Find("Buttons").gameObject;
    }

    void Update()
    {
        if(scrollFinFlg_)
        {
            return; // スクロール処理が終了しているならreturnする
        }

        // 目標値以下ならスクロールを更新する
        if (scrollTextTrans.anchoredPosition.y < endPosY)
        {
            scrollTextPos_.y += 0.2f;
            scrollTextTrans.anchoredPosition = scrollTextPos_;
        }
        else
        {
            // スクロール終了後、ボタンを表示状態に切り替える
            for(int i = 0; i < buttons_.transform.childCount; i++)
            {
                buttons_.transform.GetChild(i).gameObject.SetActive(true);
            }
            scrollFinFlg_ = true;
        }

        // まだ次の画像があるとき
        if (pictureName_.Length > pictureNowNum_)
        {
            // マイナス値とプラス値で補正をいれながらscrollMove_に値を入れる
            if (scrollTextTrans.anchoredPosition.y < 0.0f)
            {
                scrollMove_ = endPosY - Mathf.Abs(scrollTextTrans.anchoredPosition.y) + 50;
            }
            else
            {
                scrollMove_ = scrollTextTrans.anchoredPosition.y + 900;
            }

            // 画像切り替えのタイミングになったら
            if (changePictureTime_ < scrollMove_)
            {
                // 画像を切替て目標値を増やす
                objectImage_.sprite = CreateSprite(pictureName_[pictureNowNum_]);
                pictureNowNum_++;
                changePictureTime_ += 350.0f;
                Debug.Log("画像切替" + scrollMove_);
            }
        }
    }

    // スプライトの生成
    private Sprite CreateSprite(string path)
    {
        // ファイルパス作成
        string str = Application.streamingAssetsPath + "/ChapterBack/" + path + ".png";
        // ファイルパス読み込み
        byte[] bytes = File.ReadAllBytes(str);
        // Texture2Dとして作成(Texture2D(2, 2)としているが、LoadImage実行後にサイズも更新されるので問題ない)
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        // Texture2DからSpriteへ変換
        Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
        // すぐには破棄が出来ないので、後から破棄できるようにリストに入れておく
        texture2dList.Add(texture);
        spriteList.Add(sprite);

        return sprite;
    }

    // セーブしてタイトルへ戻る
    public void TownBack()
    {
        DestroyTexture2D();
        SceneMng.SceneLoad((int)SceneMng.SCENE.TOWN);
    }

    // Texture2DとSpriteの画像破棄処理
    private void DestroyTexture2D()
    {
        foreach (var tex in texture2dList)
        {
            Destroy(tex);
        }

        foreach (var spr in spriteList)
        {
            Destroy(spr);
        }
    }
}