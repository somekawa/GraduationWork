using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ItemImageMng : MonoBehaviour
{
    //public int[,] materiaImages_ = new int[5, 5];
    public static Sprite[,] materialIcon_ = new Sprite[5,5];// �擾�����f�ނ̃C���X�g
    private static bool onceFlag_ = false;

    void Start()
    {
        if(onceFlag_==true)
        {
            // ���łɉ摜�����ς݂Ȃ�2�������Ȃ��悤�ɂ���
            return;
        }

        for (int f = 0; f < 5; f++)
        {
            // Texture2D�Ƃ��ĉ摜��ǂݍ���
            string str = Application.streamingAssetsPath + "/Materia/Materia_Field" + f+".png";
            byte[] bytes = File.ReadAllBytes(str);
            Texture2D texture = new Texture2D(64, 64);
            texture.LoadImage(bytes);

            for (int x = 0; x < 5; x++)
            {
                // �擾�����摜���P��128*128�̃T�C�Y�ɕ���
                Rect rect = new Rect(x * 64, 0, 64, 64);
                materialIcon_[f, x] = Sprite.Create(texture, rect,
                    new Vector2(0.5f, 0.5f), 1.0f, 0, SpriteMeshType.FullRect);
               // Debug.Log(f + "" + x);
            }
        }
        onceFlag_ = true;
    }
}
