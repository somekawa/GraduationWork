using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemImageMng : MonoBehaviour
{
    //public int[,] materiaImages_ = new int[5, 5];
    public static Sprite[,] materialIcon_ = new Sprite[5,5];// 取得した素材のイラスト
    private static bool onceFlag_ = false;

    void Start()
    {
        if(onceFlag_==true)
        {
            // すでに画像生成済みなら2回も入らないようにする
            return;
        }

        for (int f = 0; f < 5; f++)
        {
            // Texture2Dとして画像を読み込む
            string str = Application.streamingAssetsPath + "/Materia/Materia_Field" + f+".png";
            byte[] bytes = File.ReadAllBytes(str);
            Texture2D texture = new Texture2D(64, 64);
            texture.LoadImage(bytes);

            for (int x = 0; x < 5; x++)
            {
                // 取得した画像を１つ128*128のサイズに分割
                Rect rect = new Rect(x * 64, 0, 64, 64);
                materialIcon_[f, x] = Sprite.Create(texture, rect,
                    new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
               // Debug.Log(f + "" + x);
            }
        }
        onceFlag_ = true;
    }
}
