using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwnedMateria : MonoBehaviour
{
    void Awake()
    {
        // シーンを跨いでも消えないオブジェクトにする
        // このオブジェクトが消えてしまうと、QuestClearCheck.csの受注中のクエストを保存しているリストがnullになってしまうから
        DontDestroyOnLoad(this);
    }

    public void SetMyName(string num)
    {
        // 自分のオブジェクト名を、クエスト番号に変換
        // 他スクリプトがヒエラルキーからこのオブジェクトを検索するときに番号+タグ(Quest)で判別できるようにした
        this.gameObject.name = num.ToString();
    }
}
