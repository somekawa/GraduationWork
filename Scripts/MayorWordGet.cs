using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MayorWordGet : MonoBehaviour
{
    private Bag_Word bagWord_;
    private Button[] wordGetBtn_=new Button[2];
    private int clickCnt_ = 0;
    private GameObject popUp_;

    void Start()
    {

        if (bagWord_ == null)
        {
            bagWord_ = GameObject.Find("DontDestroyCanvas/Managers").GetComponent<Bag_Word>();
            wordGetBtn_[0] = GameObject.Find("MayorHouseCanvas/BigWindowBtn").GetComponent<Button>();
            wordGetBtn_[1] = GameObject.Find("MayorHouseCanvas/WindowBtn").GetComponent<Button>();
            if (Bag_Word.wordState[InitPopList.WORD.SUB3][2].getFlag == 1)
            {
                Debug.Log(Bag_Word.wordState[InitPopList.WORD.SUB3][2].name+"ÇéùÇ¡ÇƒÇ¢ÇÈÇΩÇﬂîjâÛÇµÇ‹Ç∑");
                Destroy(wordGetBtn_[0].gameObject);
                Destroy(wordGetBtn_[1].gameObject);
                Destroy(this);
            }
            Debug.Log(Bag_Word.wordState[InitPopList.WORD.SUB3][2].name);
            popUp_ = GameObject.Find("MayorHouseCanvas/PopUp").gameObject;
            popUp_.gameObject.SetActive(false);
        }
    }

    public void OnClickGetWord()
    {
        SceneMng.SetSE(1);
        clickCnt_++;
        Debug.Log("clickCnt_" + clickCnt_);
        if (5 <= clickCnt_)
        {
            StartCoroutine(GetPopUp());
        }
    }

    private IEnumerator GetPopUp()
    {
        bagWord_.WordGetCheck(InitPopList.WORD.SUB3, 2, 26);// îΩéÀ
        popUp_.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        popUp_.gameObject.SetActive(false);
        Destroy(wordGetBtn_[0].gameObject);
        Destroy(wordGetBtn_[1].gameObject);
        Destroy(this);
        yield break;
    }
}
