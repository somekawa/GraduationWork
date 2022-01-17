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
            // ���łɉ摜�����ς݂Ȃ�2�������Ȃ��悤�ɂ���
            return;
        }

        spriteMap[IMAGE.ITEM] = ImageMng(4, 6, "/Item/Items");
        spriteMap[IMAGE.MAGIC] = ImageMng(1, 7, "/MagicImage");
        spriteMap[IMAGE.BOOK] = ImageMng(1, 6, "/BookImage");
        // ��Ԉُ풆�̃A�C�R��
        spriteMap[IMAGE.BADSTATUSICON] = ImageMng(1, 3, "/BadStatusIcon");
        // �퓬���̃o�t�A�C�R��
        spriteMap[IMAGE.BUFFICON] = ImageMng(1, 6, "/BuffIcon");

        // �ǂݍ��݂����V�[�g���A�V�[�g���̑傫���A�ǂݍ��݂����摜
        spriteMap[IMAGE.MATERIA] = ImageMng(5, 7, "/Materia/Materia_Field");
        onceFlag_ = true;
    }

    public Sprite[] ImageMng(int pageMax, int spliteMax, string path)
    {
        var sprite = new Sprite[pageMax* spliteMax + spliteMax];
        if (pageMax <= 1)
        {
            // �ǂݍ��݂����摜��1�������̏ꍇ
            // Texture2D�Ƃ��ĉ摜��ǂݍ���
            string str = Application.streamingAssetsPath + path + ".png";
            byte[] bytes = File.ReadAllBytes(str);
            Texture2D texture = new Texture2D(64, 64);
            texture.LoadImage(bytes);

            for (int x = 0; x < spliteMax; x++)
            {
                // �擾�����摜���P��64*64�̃T�C�Y�ɕ���
                Rect rect = new Rect(x * 64, 0, 64, 64);
                sprite[x] = Sprite.Create(texture, rect,
                    new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
            }
        }
        else
        {
            for (int f = 0; f < pageMax; f++)
            {
                // Texture2D�Ƃ��ĉ摜��ǂݍ���
                string str = Application.streamingAssetsPath + path + f + ".png";
                byte[] bytes = File.ReadAllBytes(str);
                Texture2D texture = new Texture2D(64, 64);
                texture.LoadImage(bytes);

                for (int x = 0; x < spliteMax; x++)
                {
                    // �擾�����摜���P��64*64�̃T�C�Y�ɕ���
                    Rect rect = new Rect(x * 64, 0, 64, 64);
                    sprite[f* spliteMax + x] = Sprite.Create(texture, rect,
                        new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
                }
            }
        }
        return sprite;
    }
}