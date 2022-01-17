using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class ItemImageMng : MonoBehaviour
{
    public enum IMAGE
    {
        NON = -1,
        MATERIA,
        ALCHEMY_MATERIA,
        ITEM,
        BOOK,
        MAGIC,
        RESTAURANT,
        BADSTATUSICON,
        BUFFICON,
        MAX
    }
    public static Dictionary<IMAGE, Sprite[]> spriteMap = new Dictionary<IMAGE, Sprite[]>();
    private static bool onceFlag_ = false;

    void Awake()
    {
        if (onceFlag_ == true)
        {
            // すでに画像生成済みなら2回も入らないようにする
            return;
        }

        spriteMap[IMAGE.ITEM] = ImageMng(4, 6, "/Item/Items");
        spriteMap[IMAGE.MAGIC] = ImageMng(1, 7, "/MagicImage");
        spriteMap[IMAGE.BOOK] = ImageMng(1, 6, "/BookImage");
        // 状態異常中のアイコン
        spriteMap[IMAGE.BADSTATUSICON] = ImageMng(1, 3, "/BadStatusIcon");
        // 戦闘中のバフアイコン
        spriteMap[IMAGE.BUFFICON] = ImageMng(1, 6, "/BuffIcon");

        // 読み込みたいシート数、シート枚の大きさ、読み込みたい画像
        spriteMap[IMAGE.MATERIA] = ImageMng(5, 7, "/Materia/Materia_Field");
        onceFlag_ = true;
    }

    public Sprite[] ImageMng(int pageMax, int spliteMax, string path)
    {
        var sprite = new Sprite[pageMax* spliteMax + spliteMax];
        if (pageMax <= 1)
        {
            // 読み込みたい画像が1枚だけの場合
            // Texture2Dとして画像を読み込む
            string str = Application.streamingAssetsPath + path + ".png";
            byte[] bytes = File.ReadAllBytes(str);
            Texture2D texture = new Texture2D(64, 64);
            texture.LoadImage(bytes);

            for (int x = 0; x < spliteMax; x++)
            {
                // 取得した画像を１つ64*64のサイズに分割
                Rect rect = new Rect(x * 64, 0, 64, 64);
                sprite[x] = Sprite.Create(texture, rect,
                    new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
            }
        }
        else
        {
            for (int f = 0; f < pageMax; f++)
            {
                // Texture2Dとして画像を読み込む
                string str = Application.streamingAssetsPath + path + f + ".png";
                byte[] bytes = File.ReadAllBytes(str);
                Texture2D texture = new Texture2D(64, 64);
                texture.LoadImage(bytes);

                for (int x = 0; x < spliteMax; x++)
                {
                    // 取得した画像を１つ64*64のサイズに分割
                    Rect rect = new Rect(x * 64, 0, 64, 64);
                    sprite[f* spliteMax + x] = Sprite.Create(texture, rect,
                        new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
                }
            }
        }
        return sprite;
    }
}