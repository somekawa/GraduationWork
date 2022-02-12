using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropFieldMateria : MonoBehaviour
{
    [SerializeField]
    private GameObject dropMateriaImage;     // ドロップ時に表示する素材の画像

    public enum MATERIA_NUMBER
    {
        NON = -1,
        MATERIA_0,  // 
        MATERIA_1,  // 
        MATERIA_2,  // 
        MATERIA_3,  // 
        MATERIA_4,  // 
        MAX
    }

    // オブジェクトの名前
    public static string[] objName = new string[(int)MATERIA_NUMBER.MAX];

    // 素材オブジェクトを保存　ポジションチェックで使う
    private GameObject[] materiaPointChildren_;

    // ーーーーーーーーー画像関連
    private RectTransform parentCanvas_;    // アイテム関連を表示するキャンバス
    private RectTransform dropMng_;

    // 素材イラスト
    private GameObject[] materiaUIObj = new GameObject[5];        // 取得したアイテムを表示する画像
    private Image[] materiaImage_ = new Image[5];        // 取得したアイテムを表示する画像
    private Vector3 appearPos_;         // 画像出現位置
    private Vector2 worldObject_ScreenPosition_;

    private Vector2 randomPos_;// 複数ドロップの場合があるためランダムで座標をずらす

    // テロップ（素材名とその背景）
    private Image telopImage_;              // 素材名の背景画像
    private TMPro.TextMeshProUGUI telopText_;                // 素材名表示
    private float telopAlpha_ = 0.0f;       // 画像と文字のアルファ値
    private float maxScale_ = 1.5f;
    private float telopScale_ = 0.8f;       // 画像と文字のスケール
    private bool shootArrowFlag_ = false;

    // ーーーーーーーーーその他
    private UnitychanController uniCtl_;
    private Camera mainCamera;      // 座標空間変更時に使用

    // アイテムを取得した回数
    private Bag_Materia bagMateria_;
    private int itemNumberCheck_;
    private int fieldNumber_;

    private int dropCnt_ = 1;// 何個拾ったか

    private List<int> dropMateriaList_ = new List<int>();

    public void Init()
    {
        // nowSceneに値が入る前に呼ばれることがあるためFieldMng.csでここのInitを呼ぶ

        fieldNumber_ = (int)SceneMng.nowScene - (int)SceneMng.SCENE.FIELD0;
        if (fieldNumber_ < 0)
        {
            Debug.Log(fieldNumber_ + "番は、範囲外番号なのでreturnします");
            return;
        }

        Debug.Log(fieldNumber_ + "番のフィールドです");

        // nowScene=ForestFiledなら3番　0～6　5個配置
        Debug.Log(SceneMng.nowScene + "現在のフィールド" + fieldNumber_);

        uniCtl_ = GameObject.Find("Uni").GetComponent<UnitychanController>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();

        // parentCanvasは画像移動の計算で使うためDropMngではなくFieldUICanvasまでにしておく
        parentCanvas_ = GameObject.Find("FieldUICanvas").GetComponent<RectTransform>();
        dropMng_ = parentCanvas_.transform.Find("DropMng").GetComponent<RectTransform>();

        bagMateria_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Materia>();


        telopImage_ = dropMng_.Find("TelopBackImage").GetComponent<Image>();
        telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
        telopImage_.gameObject.SetActive(false);

        telopText_ = telopImage_.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        telopText_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);

        materiaPointChildren_ = new GameObject[transform.childCount];

        // フィールドに対する取得できる種類の個数
        int max = InitPopList.dropNum[(InitPopList.FIELD_NUM)fieldNumber_].Length;

        for (int i = 0; i < max; i++)
        {
            // Listに取得できる素材の番号を追加
            dropMateriaList_.Add(InitPopList.dropNum[(InitPopList.FIELD_NUM)fieldNumber_][i]);
            Debug.Log(i + "番目の素材：" + dropMateriaList_[i]);
        }

        // Random.Rangeはintの場合<=ではなく<になってしまうため要素を1つ追加
        dropMateriaList_.Add(InitPopList.dropNum[(InitPopList.FIELD_NUM)fieldNumber_][max - 1] + 1);

        for (int i = 0; i < 5; i++)
        {
            materiaPointChildren_[i] = transform.GetChild(i).gameObject;
            objName[i] = "Materia" + i + "_" + Random.Range(dropMateriaList_[0], dropMateriaList_[max]);
            materiaPointChildren_[i].name = objName[i];
            Debug.Log(materiaPointChildren_[i].name + "      " + objName[i]);
        }
    }

    public void SetItemName(int materiaNum, int objNum)
    {
        // 接触したObjの名前,whileに入ってよいかのフラグ
        NameAndPosCheck(materiaNum, objNum);
        Debug.Log(materiaNum);
    }

    private void NameAndPosCheck(int materiaNum, int objNum)
    {
        // ユニを動けなくする
        uniCtl_.enabled = false;

        // 値を保存
        itemNumberCheck_ = materiaNum;

        // 1～5個の素材を拾う
        dropCnt_ = Random.Range(1, 6);

        // 拾った素材の名前を表示
        telopText_.text = Bag_Materia.materiaState[materiaNum].name;
        telopImage_.gameObject.SetActive(true);

        // 拾った分をバッグに入れる
        bagMateria_.MateriaGetCheck(itemNumberCheck_, dropCnt_);

        // 素材を拾えるポイントのエフェクトを非表示にする
        materiaPointChildren_[objNum].SetActive(false);

        // 画像表示の初期位置計算
        // オブジェクトのワールド空間positionをビューポート空間に変換
        appearPos_ = mainCamera.WorldToViewportPoint(materiaPointChildren_[objNum].transform.position);
        Debug.Log(appearPos_);

        // ビューポートの原点は左下、Canvasは中央のためCanvasのRectTransformのサイズの1/2を引く
        worldObject_ScreenPosition_ = (appearPos_ * parentCanvas_.sizeDelta) - (parentCanvas_.sizeDelta * 0.5f);

        // アイテム名は素材アイコンよりY+70の位置に表示
        telopImage_.transform.localPosition = new Vector2(worldObject_ScreenPosition_.x,
            worldObject_ScreenPosition_.y + 100.0f);
        Debug.Log("WorldObject_ScreenPosition::" + worldObject_ScreenPosition_);

        // 拾った個数分画像を生成する　移動はMoveDropImage.cs
        StartCoroutine(InstanceMateriaUI(dropCnt_, materiaNum));
    }

    private IEnumerator InstanceMateriaUI(int count, int imageNum)
    {
        int instanceCnt_ = 0;// 何個生成するか
        while (true)
        {
            yield return null;

            if (FieldMng.nowMode == FieldMng.MODE.BUTTLE)
            {
                for (int i = 0; i < instanceCnt_; i++)
                {
                    // 画像移動中にバトルが始まったら破壊する
                    Destroy(materiaUIObj[instanceCnt_]);
                }
                // 値をリセット
                SetMoveFinish(false);
            }

            if (count <= instanceCnt_)
            {
                if (telopScale_ < maxScale_)
                {
                    // テロップの座標を上昇させる
                    telopImage_.transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
                    // スケールとアルファ値を大きくする
                    ChangeNums(0.8f, 0.8f);
                    Debug.Log("スケールとアルファ値を大きくする");
                }
                else
                {
                    uniCtl_.enabled = true;// ユニが動くようにする
                    shootArrowFlag_ = true;
                    //// 不透明度をなくしスケールが変化しないように
                    ChangeNums(1.0f, 1.0f);
                    Debug.Log("スケールとアルファ値を変化しないように");
                    yield break;
                }
            }
            else
            {
                // 生成座標をずらす
                randomPos_.x = Random.Range(-50.0f, 50.0f);
                randomPos_.y = Random.Range(-50.0f, 50.0f);
                // ドロップする個数分画像を生成する
                materiaUIObj[instanceCnt_] = Instantiate(dropMateriaImage,
                 new Vector2(0, 0), Quaternion.identity, dropMng_);
                // 名前を番号にして探しやすくする
                materiaUIObj[instanceCnt_].name = instanceCnt_.ToString();
                materiaImage_[instanceCnt_] = materiaUIObj[instanceCnt_].GetComponent<Image>();
                // 取得した素材の画像を入れる
                materiaImage_[instanceCnt_].sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][imageNum];
                Debug.Log("画像番号" + imageNum);
                // 表示画像のアンカーに座標を代入して座標を決定
                materiaImage_[instanceCnt_].transform.localPosition = worldObject_ScreenPosition_ + randomPos_;
                // 取得個数になるまで加算
                instanceCnt_++;
            }
        }
    }

    public bool GetShootArrowFlag()
    {
        // falseなら上昇　trueなら放物線移動
        return shootArrowFlag_;
    }

    public void SetMoveFinish(bool flag)
    {
        if (flag == true)
        {
            // 素材画像放物線描き中はテロップ画像上昇
            telopImage_.transform.localPosition += new Vector3(0.0f, 80.0f * Time.deltaTime, 0.0f);
            //   Debug.Log("     " + telopImage_.transform.localPosition.y);
            // スケールとアルファ値を小さくする
            ChangeNums(-0.8f, -0.8f);
            Debug.Log("スケールとアルファ値を小さくする");
        }
        else
        {
            shootArrowFlag_ = false;
            Debug.Log("スケールとアルファ値を初期化 ");
            // テロップのステータスを初期化する
            telopImage_.gameObject.SetActive(false);
            telopAlpha_ = 0.0f;
            telopScale_ = 0.8f;
            telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
            telopText_.color = new Color(1.0f, 0.0f, 1.0f, telopAlpha_);
            telopImage_.transform.localScale = new Vector3(telopScale_, telopScale_, telopScale_);
            telopImage_.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            Debug.Log("スケールとアルファ値     telopAlpha_" + telopAlpha_ + "     telopScale_" + telopScale_);
        }
    }

    private void ChangeNums(float alpha, float scale)
    {
        telopAlpha_ += alpha * Time.deltaTime;
        telopScale_ += scale * Time.deltaTime;
        telopImage_.color = new Color(1.0f, 1.0f, 1.0f, telopAlpha_);
        telopText_.color = new Color(1.0f, 0.0f, 1.0f, telopAlpha_);
        telopImage_.transform.localScale = new Vector3(telopScale_, telopScale_, telopScale_);
    }
}