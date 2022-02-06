using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class MayorWordGet : MonoBehaviour
{

    private Bag_Word bagWord_;
    private Button[] wordGetBtn_=new Button[2];
    private int clickCnt_ = 0;

    void Start()
    {
        if (bagWord_ == null)
        {
            bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();
            wordGetBtn_[0] = GameObject.Find("MayorHouseCanvas/BigWindowBtn").GetComponent<Button>();
            wordGetBtn_[1] = GameObject.Find("MayorHouseCanvas/WindowBtn").GetComponent<Button>();
        }
    }

    public void OnClickGetWord()
    {
        clickCnt_++;
        Debug.Log("clickCnt_" + clickCnt_);
        if (5 <= clickCnt_)
        {
            bagWord_.WordGetCheck(InitPopList.WORD.SUB3, 2, 26);// ”½ŽË
            Destroy(wordGetBtn_[0].gameObject);
            Destroy(wordGetBtn_[1].gameObject);
            Destroy(this);

        }
    }
}
