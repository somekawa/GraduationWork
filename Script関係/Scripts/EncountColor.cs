using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncountColor : MonoBehaviour
{
    // ���߂�ꂽ���ԓ��ɐԂ܂ŕK�����B����d�g��
    // redStartFlg_�́A���Ԓl��true�ɕω�����

    private FieldMng fieldMng_;    // FieldMng����[���ݒl / �G���J�E���g�����l]���v�Z�����l���󂯎��

    // int���ƃG���[���ł�
    private readonly float red   = 255.0f;
    private readonly float green = 255.0f;
    private readonly float blue  = 255.0f;

    private float r = 0.0f;
    private float g = 0.0f;
    private float b = 0.0f;

    private bool redStartFlg_ = false;  // �̉��Z�����l����������true�ɂ���

    private Image image_;

    void Start()
    {
        // ���ݎg�p����Ă���}�e���A�����擾
        image_ = this.GetComponent<Image>();

        fieldMng_ = GameObject.Find("FieldMng").GetComponent<FieldMng>();
    }

    void Update()
    {
        if(FieldMng.nowMode == FieldMng.MODE.BUTTLE)
        {
            // �퓬���[�h���Ȃ�v�Z���s��Ȃ�
            // �l�̏�����
            redStartFlg_ = false;
            r = 0.0f;
            g = 0.0f;
            b = 255.0f;
            return;
        }

        // ���t���[���擾����K�v������
        float time = fieldMng_.GetNowEncountTime();

        Debug.Log(time);

        if (time >= 0.6f)
        {
            redStartFlg_ = true;
        }

        if (redStartFlg_)
        {
            // red��0����255�֌����킹����
            r = red * time;
            if (r > 255.0f)
            {
                r = 255.0f;
            }

            if (time >= 0.65f)
            {
                // ���ݒl����0�֌����킹����
                g = 255.0f - (green * (time - 0.3f));
                if (g < 0.0f)
                {
                    g = 0.0f;
                }
            }
            else
            {
                // �F�̉��Z�𑱂���
                g = green * time;
                if (g > 255.0f)
                {
                    g = 255.0f;
                }
            }

            // redStartFlg_��true�ɂȂ�����A���Z�𑁂�����
            b = 255.0f - (blue * (time + 0.2f));
            if (b < 0.0f)
            {
                b = 0.0f;
            }
        }
        else
        {
            // green��0����255�֌������悤�ɂ���B
            g = green * time;
            if (g > 255.0f)
            {
                g = 255.0f;
            }

            // (time_ / toButtleTime_)�́A(���ݒl / �G���J�E���g��������)�Ȃ̂�0�`1�̒l�ɂł���
            // ��L�̂��blue����Z����ƁA0����255�֌������l�ɂȂ�B
            // blue�͖��邢��Ԃ���Â����Ă��������̂ŁA255- ��擪�ɂ��Ēl�𔽓]������(255����0�֌������l�ɂȂ�)
            b = 255.0f - (blue * time);
            if (b < 0.0f)
            {
                b = 0.0f;
            }
        }

        // �}�e���A���̐F�ݒ�ɗΐF��ݒ�
        image_.color= new Color(r/255.0f , g/255.0f , b/255.0f, 1.0f);
    }
}
