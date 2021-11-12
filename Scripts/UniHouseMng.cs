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

        // バッグ関連
        GameObject.Find("Managers").GetComponent<Bag_Word>().Init();
        GameObject.Find("Managers").GetComponent<Bag_Magic>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Item>().DataLoad();
        GameObject.Find("Managers").GetComponent<Bag_Materia>().Init();

        // メインカメラを最初にアクティブにする
        var cameraMng_ = GameObject.Find("CameraController").GetComponent<CameraMng>();
        cameraMng_.SetChangeCamera(false);

        // 合成時のミニゲーム用Mng
        alchemyMng.gameObject.SetActive(false);
        magicCreateMng.gameObject.SetActive(false);
    }

    public void ClickSleepButton()
    {
        if(EventMng.GetChapterNum() < 7)    // 進行度が0〜6のとき
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
        if (0 < Bag_Materia.materiaState[Bag_Materia.emptyMateriaNum].haveCnt)
        {
            magicCreateMng.gameObject.SetActive(true);
            uniHouseCanvas.gameObject.SetActive(false);
            GameObject.Find("MagicCreateMng").GetComponent<MagicCreate>().Init();
        }
        Debug.Log("ワード合成ボタンが押下されました");
    }
}
