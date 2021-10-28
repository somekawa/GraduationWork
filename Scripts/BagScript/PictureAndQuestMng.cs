using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PictureAndQuestMng : MonoBehaviour
{
    private enum LOOK
    {
        NON = -1,
        QUEST,
        PICTUER,
        MAX
    }

    private RectTransform[] lookCheck = new RectTransform[(int)LOOK.MAX];
    private Image backImage;

    private EventSystem eventSystem;// ボタンクリックのためのイベント処理
    private GameObject clickbtn_;    // どのボタンをクリックしたか代入する変数
    private string[] lookName = new string[(int)LOOK.MAX]{
    "Quest","Picture"};

    public void Init()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        lookCheck[(int)LOOK.QUEST] = transform.Find("QuestMng").GetComponent<RectTransform>();
        lookCheck[(int)LOOK.PICTUER] = transform.Find("PictureBookMng").GetComponent<RectTransform>();
        lookCheck[(int)LOOK.QUEST].gameObject.SetActive(true);
        lookCheck[(int)LOOK.PICTUER].gameObject.SetActive(false);

        backImage = transform.Find("Back").GetComponent<Image>();

        // クエスト履歴が呼ばれた状態でスタート
        backImage.color = new Color(0.5f, 0.5f, 1.0f, 1.0f);

    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void OnClickOtherFrame()
    {
        clickbtn_ = eventSystem.currentSelectedGameObject;
        if (clickbtn_.name == lookName[(int)LOOK.QUEST])
        {
            // クエスト履歴が呼ばれたら
            lookCheck[(int)LOOK.QUEST].gameObject.SetActive(true);
            lookCheck[(int)LOOK.PICTUER].gameObject.SetActive(false);

            backImage.color = new Color(0.5f, 0.5f,1.0f,  1.0f);
        }
        else
        {
            // 図鑑が呼ばれたら
            lookCheck[(int)LOOK.PICTUER].gameObject.SetActive(true);
            lookCheck[(int)LOOK.QUEST].gameObject.SetActive(false);
            backImage.color = new Color(1.0f,0.5f,0.5f,1.0f);
        }
    }
}
