using UnityEngine;

public class UniHouseMng : MonoBehaviour
{
    [SerializeField]
    private GameObject nightLights; // 街頭

    [SerializeField]
    private RectTransform alchemyMng;

    [SerializeField]
    private RectTransform magicCreateMng;

    [SerializeField]
    private Canvas uniHouseCanvas;

    private GameObject loadPrefab_;// タイトルシーンからの遷移かどうか
    private OnceLoad onceLoad_;// LoadPrefabにアタッチされてるScript
    void Start()
    {
        // 現在のシーンをUNIHOUSEとする
        SceneMng.SetNowScene(SceneMng.SCENE.UNIHOUSE);

        // 今が夜ならばライト点灯、それ以外ならライト消灯
        if (SceneMng.GetTimeGear() == SceneMng.TIMEGEAR.NIGHT)
        {
            nightLights.SetActive(true);
        }
        else
        {
            nightLights.SetActive(false);
        }

        // WarpTown.csの初期化関数を呼ぶ
        GameObject.Find("WarpInTown").GetComponent<WarpTown>().Init();

        // WarpField.csの初期化関数を先に呼ぶ
        GameObject.Find("WarpOut").GetComponent<WarpField>().Init();

        // オブジェクトがある＝タイトルシーンから遷移してきた
        loadPrefab_ = GameObject.Find("LoadPrefab");
        if (loadPrefab_ != null)
        {
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Item>().NewGameInit();
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>().NewGameInit();
            GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>().NewGameInit();
            onceLoad_ = GameObject.Find("LoadPrefab").GetComponent<OnceLoad>();
            onceLoad_.SetNewGameFlag(true);
        }
        // メインカメラを最初にアクティブにする
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);

        // 合成時のミニゲーム用Mng
        alchemyMng.gameObject.SetActive(false);
        magicCreateMng.gameObject.SetActive(false);

        // ステータスアップを消すか判定する
        if (!SceneMng.GetFinStatusUpTime())
        {
            for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].DeleteStatusUpByCook();
            }
        }
    }

    public void ClickSleepButton()
    {
        if(EventMng.GetChapterNum() < 7)    // 進行度が0～6のとき
        {
            Debug.Log("現在の進行度が7未満のため、休めません");
            return; // 休むボタンを押しても反応しないようにする
        }

        SceneMng.SetTimeGear(SceneMng.TIMEGEAR.MORNING);   // 時間経過
        Debug.Log("休むボタンが押下されました");
    }

    public void ClickAlchemyButton()
    {
        alchemyMng.gameObject.SetActive(true);
        uniHouseCanvas.gameObject.SetActive(false);
        Debug.Log("合成ボタンが押下されました");
    }

    public void ClickMagicCreateButton()
    {    
        // 空のマテリアを1つ以上持っていたらワード合成ができる
        //if (0 < Bag_Materia.materiaState[Bag_Materia.emptyMateriaNum].haveCnt)
        //{
            uniHouseCanvas.gameObject.SetActive(false);
            magicCreateMng.gameObject.SetActive(true);
            magicCreateMng.GetComponent<MagicCreate>().Init();
      //  }
        Debug.Log("魔法作成ボタンが押下されました");
    }
}
