using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 後々、QuestMngとかが制作されて、メインクエストが進むたびに、このスクリプトを呼び出して
// nowChapterNum_を加算し、新しくテキストを読み込むようにしていければいいかなと思う。

// テキストをスキップしたときにうまくいかない・・・

public class TextMng : MonoBehaviour
{
    public GameObject ConversationCanvas;
    public GameObject CharacterList;

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

    // キーがキャラ名,値が各キャラの顔を切り替えるクラスのマップ
    private Dictionary<string, UnityChan.FaceUpdate> charFacesMap_ = new Dictionary<string, UnityChan.FaceUpdate>();

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
        icon_ = ConversationCanvas.transform.Find("nextMessage_icon").gameObject;
        iconColor_ = icon_.GetComponent<Image>();
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
                if (Input.GetMouseButtonDown(0) && !skipFlg_)
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
                        Debug.Log("会話終了です");
                        // 越えたら話の終了の合図として使える
                    }
                }
            }
        }
    }

    // 名前,メッセージ,キャラの表情を設定する
    void TextAndFaceSetting()
    {
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
        name_.text = popChapter_.param[nowText_].name1;

        // 顔の表情を変更する
        charFacesMap_[popChapter_.param[nowText_].name2].OnCallChangeFace(popChapter_.param[nowText_].face);

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
}
