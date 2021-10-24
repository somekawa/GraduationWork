using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�����N�^�[�X�e�[�^�X�̕ۑ��p�X�N���v�g
public class CharaData : MonoBehaviour
{
    public static CharaBase.CharacterSetting[] data = new CharaBase.CharacterSetting[(int)SceneMng.CHARACTERNUM.MAX];

    public static void SetCharaData(int num,CharaBase.CharacterSetting tmpdata)
    {
        data[num] = tmpdata;
    }

    public static CharaBase.CharacterSetting GetCharaData(int num)
    {
        return data[num];
    }
}
