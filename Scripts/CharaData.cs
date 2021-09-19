using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラクターステータスの保存用スクリプト
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
