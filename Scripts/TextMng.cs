using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// 後々、QuestMngとかが制作されて、メインクエストが進むたびに、このスクリプトを呼び出して
// nowChapterNum_を加算し、新しくテキストを読み込むようにしていければいいかなと思う。

public class TextMng : MonoBehaviour
{
    public GameObject ConversationCanvas;
    public GameObject CharacterList;

    enum FADE
    {
        NON,
        IN,
        OUT,
        MAX
    }

    private GameObject DataPopPrefab_;

    private TMPro.TextMeshProUGUI name_;
    private TMPro.TextMeshProUGUI message_;
    private float messageTime_ = 0.0f;              // 表示速度時間
    private readonly float messageSpeed_ = 0.1f;    // 文字送り速度

    private GameObject icon_;        // 次のテキストへの合図をするアイコン
    private Image iconColor_;        // 点滅処理で使用する

    private ChapterList popChapter_; // 現在必要なテキスト部分だけ取得する
    private int nowChapterNum_ = 0;  // 現在のチャプター進行度(実際は0からスタート)
    private int nowText_ = 0;        // 現在のテキスト(チャプター進行度が切り替わったら0に初期化すること)

    private bool skipFlg_  = false;  // テキストが切り替わるタイミングでtrueになる
    private float skipItv_ = 0.0f;   // 文字の全表示に至るまでのインターバル

    private Image backImage_;        // 会話中の背景画像(Excelから画像名を読み込んで動的に差し替える)
    private Image objectImage_;      // 会話中に登場する画像(Excelから画像名を読み込んで動的に差し替える)

    private Fade fade_;              // トランジション関連(fadeoutとfadein関数を呼び出すために必要)
    private readonly float fadeTimeMax_ = 3.0f;
    private float nowFadeTime_ = 0.0f;

    // キーがキャラ名,値が各キャラの顔を切り替えるクラスのマップ
    private Dictionary<string, UnityChan.FaceUpdate> charFacesMap_ = new Dictionary<string, UnityChan.FaceUpdate>();

    private List<Texture2D> texture2dList = new List<Texture2D>();
    private List<Sprite> spriteList = new List<Sprite>();

    private SceneMng.SCENE sceneLoadNum_;   // Excelから読み込んだ次の切替予定シーンを保存する変数
    private bool clickLock_ = false;        // マウスの押下を許可するか(true:マウスクリック不可,false:マウスクリック可)

    void Start()
    {
        // CharacterListの子供を順番に取得していき、charFacesMap_に登録する
        Transform tmpt = CharacterList.gameObject.transform;
        for (int i = 0; i < tmpt.childCount; i++)
        {
            var childName = tmpt.GetChild(i).name;  // 子の名前を見る
            charFacesMap_.Add(childName, tmpt.Find(childName).GetComponent<UnityChan.FaceUpdate>());    // 登録
        }

        //popChapter_ = TextPopPrefab_.GetComponent<PopList>().GetChapterList(nowChapterNum_);
        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resourcesファイルから検索する
        popChapter_ = DataPopPrefab_.GetComponent<PopList>().GetData<ChapterList>(PopList.ListData.CHAPTER, nowChapterNum_);

        // Frame_textの子にあるMessageというテキストオブジェクトを探す
        message_ = ConversationCanvas.transform.Find("Frame_text/Message").GetComponent<TMPro.TextMeshProUGUI>();
        // Frame_nameの子にあるNameというテキストオブジェクトを探す
        name_ = ConversationCanvas.transform.Find("Frame_name/Name").GetComponent<TMPro.TextMeshProUGUI>();
        // nextMessage_iconというオブジェクトを探す
        icon_ = ConversationCanvas.transform.Find("NextMessage_icon").gameObject;
        iconColor_ = icon_.GetComponent<Image>();

        // 背景画像
        backImage_ = GameObject.Find("BackCanvas/BackImage").GetComponent<Image>();
        // 通常画像
        objectImage_ = ConversationCanvas.transform.Find("ObjectImage").GetComponent<Image>();

        // トランジション
        fade_ = GameObject.Find("FadeCanvas").GetComponent<Fade>();

        TextAndFaceSetting();
    }

    void Update()
    {
        Skip();

        // 全て文字が表示されたら、自動的にfalseにする(これがないと、途中でテキストが進まなくなる)
        if (message_.maxVisibleCharacters >= message_.text.Length)
        {
            skipFlg_ = false;
        }

        if (messageTime_ < messageSpeed_)    
        {
            // 時間を満たしていなければ加算する
            messageTime_ += Time.deltaTime;
            return;
        }
        else
        {
            // 文字の最大値と比較する
            if (message_.maxVisibleCharacters < message_.text.Length)
            {
                // 表示する文字数を増やしていくことで、左から順に文字がでているように見せる
                message_.maxVisibleCharacters++;
                messageTime_ = 0.0f;
            }
            else
            {
                // アイコンの表示
                icon_.SetActive(true);
                // アイコンの点滅処理(Time.time * 5.0の[5.0]は点滅速度調整用の数値です)
                iconColor_.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Sin(Time.time * 5.0f) / 2 + 0.5f);

                // マウスの左クリック押下時
                if (Input.GetMouseButtonDown(0) && !skipFlg_ && !clickLock_)
                {
                    // Excelの列の最大数を越えないようにする(Countに-1をつけないとエラーになる)
                    if (nowText_ < popChapter_.param.Count - 1)
                    {
                        Debug.Log("次のテキスト");
                        skipFlg_ = true;
                        skipItv_ = 0.0f;

                        nowText_++;
                        TextAndFaceSetting();
                    }
                    else
                    {
                        // 越えたら話の終了の合図として使える
                        Debug.Log("会話終了です");

                        // そのチャプターで使った画像をまとめて破棄する
                        // シーン遷移する直前が正しいが、テストとしてここに書いています
                        DestroyTexture2D();
                        SceneMng.SceneLoad((int)sceneLoadNum_);
                    }
                }
            }
        }
    }

    // 背景画像の読み込みと差し替え
    void ChangeBackImage()
    {
        while (popChapter_.param[nowText_].name1 == "Image")
        {
            if(popChapter_.param[nowText_].name2 == "Back")
            {
                // 画像差し替え
                backImage_.sprite = CreateSprite(popChapter_.param[nowText_].name2);
            }
            else if(popChapter_.param[nowText_].name2 == "Object")
            {
                if (popChapter_.param[nowText_].message == "")
                {
                    objectImage_.sprite = null;
                }
                else
                {
                    // 手紙画像
                    objectImage_.sprite = CreateSprite(popChapter_.param[nowText_].name2);
                    // 描画する画像があるときは表示にする
                    objectImage_.gameObject.SetActive(true);
                }
            }
            else
            {
                // 何も処理を行わない
            }

            if (nowText_ < popChapter_.param.Count - 1)
            {
                // Excelの行を進める
                nowText_++;
            }
            else
            {
                // while文で永久ループにならないように、最後の行ならbreakする
                break;
            }
        }

        // 描画する画像がないときは非表示にする
        if (objectImage_.sprite == null)
        {
            objectImage_.gameObject.SetActive(false);
        }
    }

    // トランジション処理があるか確認する
    private void CheckTransition()
    {
        if (popChapter_.param[nowText_].name1 == "Fade")
        {
            // フェード時には、テキストを出す場所を非表示にする
            ConversationCanvas.SetActive(false);

            if (popChapter_.param[nowText_].name2 == "Out")
            {
                // フェードアウト処理
                fade_.FadeOut(fadeTimeMax_);
                nowFadeTime_ = fadeTimeMax_;
                // ここでコルーチンを呼ぶ
                StartCoroutine(Transition(FADE.OUT));
            }
            else if(popChapter_.param[nowText_].name2 == "In")
            {
                // フェードイン処理
                fade_.FadeIn(fadeTimeMax_);
                nowFadeTime_ = fadeTimeMax_;
                // ここでコルーチンを呼ぶ
                StartCoroutine(Transition(FADE.IN));
            }
            else
            {
                // 何も処理を行わない
            }
        }
        else
        {
            // フェード時以外はキャンバスを表示にする
            ConversationCanvas.SetActive(true);
        }
    }

    // トランジションのコルーチン  
    private IEnumerator Transition(FADE fade)
    {
        clickLock_ = true;

        while (fade != FADE.NON)
        {
            yield return null;

            if (nowFadeTime_ > 0.0f)
            {
                nowFadeTime_ -= Time.deltaTime;
            }
            else
            {
                if(fade != FADE.MAX)
                {
                    nowFadeTime_ = fadeTimeMax_;

                    // 最終行では無いときは行を進める
                    if (nowText_ < popChapter_.param.Count - 1)
                    {
                        // Excelの行を進める
                        nowText_++;
                        // 次の表示をするために、関数を呼び出す
                        TextAndFaceSetting();
                    }
                }

                if (fade == FADE.IN)
                {
                    fade_.FadeOut(fadeTimeMax_);    // 次のフェード処理
                    fade = FADE.MAX;
                }
                else if (fade == FADE.OUT)
                {
                    fade_.FadeIn(fadeTimeMax_);     // 次のフェード処理
                    fade = FADE.MAX;
                }
                else
                {
                    // フェード時以外はキャンバスを表示にする
                    ConversationCanvas.SetActive(true);
                    fade = FADE.NON;
                }
            }
        }

        clickLock_ = false;
    }

    // シーン切り替え準備
    private void ChangeScene()
    {
        // シーンを読み込んだ場合は、変数に一時保存しておく
        if (popChapter_.param[nowText_].name1 == "Scene")
        {
            // 文字列をenumに変換する処理
            sceneLoadNum_ = (SceneMng.SCENE)System.Enum.Parse(typeof(SceneMng.SCENE), popChapter_.param[nowText_].name2);
        }
    }

    // 名前,メッセージ,キャラの表情を設定する
    private void TextAndFaceSetting()
    {
        ChangeBackImage();
        CheckTransition();
        ChangeScene();

        // Excel内に改行文字[\n]があったら、改行して表示する
        // メッセージ更新
        if (popChapter_.param[nowText_].message.Contains("\\n"))
        {
            message_.text = popChapter_.param[nowText_].message.Replace("\\n", System.Environment.NewLine);
        }
        else
        {
            message_.text = popChapter_.param[nowText_].message;
        }

        // 名前更新
        if(popChapter_.param[nowText_].name1 != "Fade" && popChapter_.param[nowText_].name1 != "Scene")
        {
            name_.text = popChapter_.param[nowText_].name1;
        }

        // キャラ以外をExcel側で"Mob"と登録しているため、"Mob"なら顔の表情を変更しないようにする
        if (popChapter_.param[nowText_].face != "Mob")
        {
            // 顔の表情を変更する
            charFacesMap_[popChapter_.param[nowText_].name2].OnCallChangeFace(popChapter_.param[nowText_].face);
        }

        // 表示文字数の初期化
        // maxVisibleCharacters：最大表示文字数
        message_.maxVisibleCharacters = 0;
        // 表示速度時間の初期化
        messageTime_ = 0.0f;
        // アイコンの非表示
        icon_.SetActive(false);
    }

    // 1文字ずつ文字を表示している間に行う処理
    void Skip()
    {
        if (!skipFlg_)
        {
            return;
        }

        skipItv_ += Time.deltaTime;
        //Debug.Log(skipItv_);

        // 1文字ずつ表示させていってる途中で左クリックされると、全てのテキストが表示されるようにする
        if (Input.GetMouseButtonUp(0) && message_.maxVisibleCharacters < message_.text.Length)
        {
            // テキストが表示し始めて0.5f以上経過したら全表示が可能になる
            if (skipItv_ > 0.5f)
            {
                //Debug.Log("処理に来ました");

                skipFlg_ = false;
                skipItv_ = 0.0f;
                message_.maxVisibleCharacters = message_.text.Length;
            }
        }
    }

    // スプライトの生成
    private Sprite CreateSprite(string path)
    {
        // ファイルパス作成
        string str = Application.streamingAssetsPath + "/Chapter" + path + "/"+ popChapter_.param[nowText_].message + ".png";
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
