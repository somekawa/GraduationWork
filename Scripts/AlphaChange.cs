using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaChange : MonoBehaviour
{
    private float alpha_ = 0.0f;                // ���݂̃��l��ۑ�����
    private float alphaChangeSpeed_ = 0.01f;    // ���l�̕ω����x

    // true:Image,false:TMPro
    private (GameObject,bool)[] chiledObj_;     // Image��TMPro�̃I�u�W�F�N�g���ƃt���O��ۑ�����
    private List<int> notAlphaChangeList_ = new List<int>();    // ���l�̏������s��Ȃ��q���̔ԍ�

    // PopUp���A�N�e�B�u�ɂȂ�x�ɌĂяo�����
    void OnEnable()
    {
        alpha_ = 0.0f;
        chiledObj_ = new (GameObject, bool)[gameObject.transform.childCount];

        // �����̎q���̐���for������
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            // �ǂ̃R���|�[�l���g���A�^�b�`����Ă��邩���ׂ�
            if(gameObject.transform.GetChild(i).gameObject.GetComponent<Image>())
            {
                // �Q�[���I�u�W�F�N�g��true�Ŕz��ɕۑ����A���l��0.0f�ɂ���
                chiledObj_[i] = (gameObject.transform.GetChild(i).gameObject,true);
                chiledObj_[i].Item1.GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f, alpha_);
            }
            else if(gameObject.transform.GetChild(i).gameObject.GetComponent<TMPro.TextMeshProUGUI>())
            {
                // �Q�[���I�u�W�F�N�g��false�Ŕz��ɕۑ����A���l��0.0f�ɂ���
                chiledObj_[i] = (gameObject.transform.GetChild(i).gameObject,false);
                chiledObj_[i].Item1.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
            }
            else
            {
                // �R���[�`���ł̏������s��Ȃ��q���̔ԍ������X�g�ɕۑ�����
                notAlphaChangeList_.Add(i);
            }
        }

        StartCoroutine(PopUpAlpha());
    }

    private IEnumerator PopUpAlpha()
    {
        // ���l��1.0f��菬�����Ԃ�while������葱����
        while (alpha_ < 1.0f)
        {
            yield return null;

            bool tmpFlg = true;

            // ���l�����Z����
            alpha_ += alphaChangeSpeed_;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                // ���l���Z���������Ȃ��ԍ������ׂ�
                for(int k = 0; k < notAlphaChangeList_.Count; k++)
                {
                    // ���Z���Ăق����Ȃ��Ƃ���break�ŏ������΂�
                    if(notAlphaChangeList_[k] == i)
                    {
                        tmpFlg = false;
                        break;
                    }
                }

                // break�������牺�ɗ������ȊO�͒ʏ�̃��l����������
                if(tmpFlg)
                {
                    // �e�I�u�W�F�N�g�̃��l����������
                    if (chiledObj_[i].Item2)
                    {
                        // Image�̃��l��ύX����
                        chiledObj_[i].Item1.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
                    }
                    else
                    {
                        // TMPro�̃��l��ύX����
                        chiledObj_[i].Item1.GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, alpha_);
                    }
                }
            }
        }
    }

}
