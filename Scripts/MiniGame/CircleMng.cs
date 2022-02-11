using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CircleMng : MonoBehaviour
{
    [SerializeField]
    private RectTransform miniGameMng;    // カウントダウン、判定表示用

    [SerializeField] 
    private RectTransform magicCreateMng;    // 魔法合成
    private MagicCreate magicCreate_;

    [SerializeField] 
    private RectTransform AlchemyMng;    // アイテム合成
    private ItemCreateMng itemCreate_;

    [SerializeField]
    private Material[] elementMats;// マテリアル　7つ(6つ目はアイテム合成用)
    private Material[] elementMaterial = new Material[1];// マテリアルをセット先
    private MeshRenderer elementRenderer;// マテリアルをセットされる先のレンダー

    [SerializeField]
    private Material[] judgeMats;// 判定のマテリアル 4つ
    private Material[] judgeMaterial = new Material[1];// 判定をセットされる先
    MeshRenderer judgeMeshRender_;// 判定用マテリアルをセットするためのレンダー

    Texture2D tex;
    private Transform miniGameObj_;

    private Transform rotateCenter;// 回転地点
    private Transform needleTop;// 針の先


    public enum JUDGE
    {
        NON = -1,
        BAD,
        NORMAL,
        GOOD,
        MAX
    }
    private JUDGE judge_ = JUDGE.NON;
    private Image judgeBack_;
    private TMPro.TextMeshProUGUI judgeText_;
    // カウントダウン表示
    private Image countImage_;
    private TMPro.TextMeshProUGUI countText_;

    AssetBundle assetBundle_;


    public void Init(int elementNum, int judgeNum)
    {
        // アイテム合成か魔法合成かをアクティブ状態で判断
        if (magicCreateMng.gameObject.activeSelf == true)
        {
            magicCreate_ = magicCreateMng.GetComponent<MagicCreate>();
        }
        else
        {
            itemCreate_ = AlchemyMng.GetComponent<ItemCreateMng>();
        }

        if (elementRenderer == null)
        {
            miniGameObj_ = GameObject.Find("MiniGameObj").GetComponent<Transform>();
            judgeMeshRender_ = GetComponent<MeshRenderer>();
            // 魔法合成時の属性用マテリアルセットのため
            elementRenderer = GameObject.Find("ElementCircle").GetComponent<MeshRenderer>();

            // 針関連
            rotateCenter = GameObject.Find("RotateCenter").GetComponent<Transform>();
            needleTop = rotateCenter.Find("NeedleQuad/NeedleTop").GetComponent<Transform>();

            // カウントダウン
            countImage_ = miniGameMng.Find("CountImage").GetComponent<Image>();
            countText_ = countImage_.transform.Find("Count").GetComponent<TMPro.TextMeshProUGUI>();

            // 判定
            judgeBack_ = miniGameMng.transform.Find("JudgeBack").GetComponent<Image>();
            judgeText_ = judgeBack_.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            judgeMeshRender_.gameObject.SetActive(false);
            elementRenderer.gameObject.SetActive(false);
        }
        // 判定の初期化
        judge_ = JUDGE.NON;
        judgeText_.text = null;

        rotateCenter.gameObject.SetActive(false);

        elementMaterial[0] = elementMats[elementNum];
        elementRenderer.materials = elementMaterial;

        judgeMaterial[0] = judgeMats[judgeNum];
        judgeMeshRender_.materials = judgeMaterial;
        gameObject.transform.localEulerAngles += new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

        // マテリアルがセットされてない可能性があるためセット後アクティブ状態にする
        elementRenderer.gameObject.SetActive(true);
        judgeMeshRender_.gameObject.SetActive(true);

        miniGameMng.gameObject.SetActive(true);

        if (assetBundle_ == null)
        {
            assetBundle_ = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/create");
        }

        // カウントダウン関連
        countImage_.gameObject.SetActive(true);// カウントダウン用の画像を表示する
        StartCoroutine(CountDown());        // カウントダウンを開始する
    }

    public IEnumerator CountDown()
    {
        countText_.text = "3";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "2";
        yield return new WaitForSeconds(1.0f);
        countText_.text = "1";
        yield return new WaitForSeconds(1.0f);
        // カウントダウンの表示を消す
        countImage_.gameObject.SetActive(false);
        // 針を表示させる
        rotateCenter.gameObject.SetActive(true);
        // ミニゲームを開始する
        StartCoroutine(ResultMiniGame());
        yield break;

    }

    public IEnumerator ResultMiniGame()
    {
        bool flag = false;
        while (true)
        {
            yield return null;
            if (flag == false)
            {
                // 針を回転させる
                rotateCenter.transform.localEulerAngles = 
                    new Vector3(rotateCenter.transform.localEulerAngles.x, rotateCenter.transform.localEulerAngles.y, Mathf.Sin(Time.time) * -360.0f);

                // スペースキー押下で回転ストップ
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Vector3 pos = needleTop.transform.position;

                    // 斜め13.5度に変換
                    // 13.5度のとき、sin(0.2334),cos(0.9724)
                    Ray ray1 = new Ray(pos, new Vector3(0, -0.23f, 0.97f));

                    RaycastHit hit;
                    tex = judgeMeshRender_.material.mainTexture as Texture2D;

                    //  if (EventSystem.current.IsPointerOverGameObject()) return;
                    // 針の先端から魔法陣へ向かってレイを飛ばす
                    if (Physics.Raycast(pos, ray1.direction, out hit, Mathf.Infinity))
                    {
                        Vector2 uv = hit.textureCoord;
                        Color[] pix = tex.GetPixels(Mathf.FloorToInt(uv.x * tex.width), Mathf.FloorToInt(uv.y * tex.height), 1, 1);
                        Debug.Log(pix[0].ToString());
                        Debug.Log(pix[0]);

                        //text.text =pix[0].ToString();// new Color(pix[0].r, pix[0].g, pix[0].b, 1.0f).ToString();// 
                        //image.color =             pix[0];//new Color(pix[0].r, pix[0].g, pix[0].b, 1.0f);//    
                        //Debug.DrawRay(Vector3 start(rayを開始する位置), Vector3 dir(rayの方向と長さ), Color color(ラインの色), float duration(ラインの表示される時間), bool depthTest(ラインがカメラから近いオブジェクトによって隠された場合にラインを隠すかどうか));
                        Debug.DrawRay(pos, ray1.direction * 1000.0f, Color.red, 100.0f, false);
                        // 色がない部分だとアルファ値が0を取得できる
                        GameObject obj = null;
                        judgeBack_.gameObject.SetActive(true);
                        if (pix[0].a == 0.0f)
                        {
                            SceneMng.SetSE(1);
                            // アルファ値が0＝何も塗られてない箇所
                            judgeText_.text = "成功";
                            Debug.Log("成功しました");
                            judge_ = JUDGE.NORMAL;
                        }
                        else
                        {
                            if (0.8f < pix[0].r && pix[0].r <= 1.0f)
                            {
                                obj = assetBundle_.LoadAsset<GameObject>("CreateGood");
                                Instantiate(obj, this.gameObject.transform.position, obj.transform.rotation);

                                SceneMng.SetSE(2);
                                // 白
                                judgeText_.text = "大成功";
                                Debug.Log("大成功しました");
                                judge_ = JUDGE.GOOD;
                            }
                            else if (0.0f <= pix[0].r && pix[0].r <= 0.1f)
                            {
                                obj = assetBundle_.LoadAsset<GameObject>("CreateMiss");
                                Instantiate(obj, this.gameObject.transform.position, obj.transform.rotation);

                                SceneMng.SetSE(3);
                                // 黒
                                judgeText_.text = "失敗";
                                Debug.Log("失敗しました");
                                judge_ = JUDGE.BAD;
                            }
                        }

                        // ミニゲームリザルトに移る
                        flag = true;
                    }
                }
            }
            else
            {
                // ミニゲームのリザルト
                // アイテム合成か魔法作成化をアクティブ状態で判断
                if (magicCreateMng.gameObject.activeSelf == true)
                {
                   magicCreate_.ResultMagicCreate();
                }
                else
                {
                    itemCreate_.AlchemyRecipeSelect();                    
                }

                yield return new WaitForSeconds(2.0f);

                judgeBack_.gameObject.SetActive(false);
                miniGameMng.gameObject.SetActive(false);
                judgeText_.text = null;

                judgeMeshRender_.gameObject.SetActive(false);
                elementRenderer.gameObject.SetActive(false);
                // 見つけたマテリアルセット先は削除しておく
                Destroy(judgeMeshRender_.material);
                Destroy(elementRenderer.material);
                miniGameObj_.gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public JUDGE GetMiniGameJudge()
    {
        return judge_;
    }
}