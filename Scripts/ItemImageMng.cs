using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ItemImageMng : MonoBehaviour
{
    public enum IMAGE
    {
        NON = -1,
        MATERIA,
        ALCHEMY_MATERIA,
        ITEM,
        BOOK,
        RECIPE,
        RESTAURANT,
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

        spriteMap[IMAGE.ITEM] = ImageMng(1, 7, "/Item/Items01");

        // �ǂݍ��݂����V�[�g���A�V�[�g���̑傫���A�ǂݍ��݂����摜
        spriteMap[IMAGE.MATERIA] = ImageMng(6, 7, "/Materia/Materia_Field");
        //spriteMap_[IMAGE.MATERIA] = ImageMng(5, 7, "/Materia/Materia_Field");
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