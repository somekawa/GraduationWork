using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�����N�^�[�X�e�[�^�X�̕ۑ��p�X�N���v�g
public class CharaData : MonoBehaviour
{
    public static CharaBase.CharacterSetting data;

    public static void SetCharaData(CharaBase.CharacterSetting tmpdata)
    {
        data = tmpdata;
    }

    public static CharaBase.CharacterSetting GetCharaData()
    {
        return data;
    }
}
