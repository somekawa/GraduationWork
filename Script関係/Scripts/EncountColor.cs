using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncountColor : MonoBehaviour
{
    // ���߂�ꂽ���ԓ��ɐԂ܂ŕK�����B����d�g��
    // redStartFlg_�́A���Ԓl��true�ɕω�����

    // int���ƃG���[���ł�
    private readonly float red   = 255.0f;
    private readonly float green = 255.0f;
    private readonly float blue  = 255.0f;

    private float r = 0.0f;
    private float g = 0.0f;
    private float b = 0.0f;

    private bool redStartFlg_ = false;    // �̉��Z�����l����������true�ɂ���

    private FieldMng fieldMng_ = null;    // FieldMng�X�N���v�g�̏��
    private float encountTime_ = 0.0f;    // FieldMng����[���ݒl / �G���J�E���g�����l]���v�Z�����l���󂯎��
    private Image image_ = null;

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
        encountTime_ = fieldMng_.GetNowEncountTime();

        //Debug.Log(encountTime_);

        if (encountTime_ >= 0.6f)
        {
            redStartFlg_ = true;
        }

        if (redStartFlg_)
        {
            // red��0����255�֌����킹����
            r = ColorValueCalculation(red, true);

            if (encountTime_ >= 0.65f)
            {
                // ���ݒl����0�֌����킹����
                g = ColorValueCalculation(green, false, -0.3f);
            }
            else
            {
                // �F�̉��Z�𑱂���
                g = ColorValueCalculation(green, true);
            }

            // redStartFlg_��true�ɂȂ�����A���Z�𑁂�����
            b = ColorValueCalculation(blue, false, +0.2f);
        }
        else
        {
            // green��0����255�֌������悤�ɂ���B
            g = ColorValueCalculation(green, true);

            // (time_ / toButtleTime_)�́A(���ݒl / �G���J�E���g��������)�Ȃ̂�0�`1�̒l�ɂł���
            // ��L�̂��blue����Z����ƁA0����255�֌������l�ɂȂ�B
            // blue�͖��邢��Ԃ���Â����Ă��������̂ŁA255- ��擪�ɂ��Ēl�𔽓]������(255����0�֌������l�ɂȂ�)
            b = ColorValueCalculation(blue, false);
        }

        // �}�e���A���̐F�ݒ�ɗΐF��ݒ�
        image_.color= new Color(r/255.0f , g/255.0f , b/255.0f, 1.0f);
    }

    // �J���[�l�v�Z�����ĕԂ�
    // ����(color : �󂯎�����F,valueUpFlg : �l�̉��Z�Ȃ�true,���Z�Ȃ�false,timeAdjust : �F���������p�̎���(�L�������Ȃ�0.0f))
    private float ColorValueCalculation(float color, bool valueUpFlg, float timeAdjust = 0.0f)
    {
        if (valueUpFlg)
        {
            // �F�̉��Z
            float col = color * encountTime_;
            if (col > 255.0f)
            {
                col = 255.0f;
            }
            return col;
        }
        else
        {
            // �F�̌��Z
            float col = 255.0f - (color * (encountTime_ + timeAdjust));
            if (col < 0.0f)
            {
                col = 0.0f;
            }
            return col;
        }

        Debug.Log("EncountColor.cs�̊֐��ŃG���[");
        return 0.0f;
    }
}