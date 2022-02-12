using UnityEngine;

public class WordGetCheck : MonoBehaviour
{
    private MagicCreate magicCreate_;

    void Start()
    {
        magicCreate_ = GetComponent<MagicCreate>();
    }

    public bool HealSubWordCheck(Bag_Word.WORD_MNG kind)
    {
        int getFalseCnt_ = 0;
        int add_ = 0;
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                Debug.Log(kind + "î‘ñ⁄ÇÃâÒì]êî" + magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1));
                for (int i = 0; i < magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1); i++)
                {

                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name == "ñ°ï˚")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB2); i++)
                {
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "HP"
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "ñÉ·É"
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "à√à≈"
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "ì≈")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            default:
                return ActiveCheck(add_, getFalseCnt_);
        }
    }

    public bool AssistSubWordCheck(Bag_Word.WORD_MNG kind)
    {
        int getFalseCnt_ = 0;
        int add_ = 0;
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = 0; i < magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1); i++)
                {
                    add_++;
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).getFlag == false)
                    {
                        getFalseCnt_++;
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = 0; i < magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB2); i++)
                //       for (int i = 0; i < selectKindMaxCnt_[(int)InitPopList.WORD.SUB2]; i++)
                {

                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "HP")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }

                    //  CommonButtonCheck(false, "HP", Bag_Word.WORD_MNG.SUB2, i);
                }
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB3); i++)
                {
                    // ìGÇëIëÇµÇƒÇ¢ÇΩèÍçá
                    if (magicCreate_.GetSelectWord((int)Bag_Word.WORD_MNG.SUB1) == "ìG")
                    {
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name == "í·â∫")
                        {
                            add_++;
                            if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).getFlag == false)
                            {
                                getFalseCnt_++;
                            }
                        }
                        // CommonButtonCheck(true, "í·â∫", Bag_Word.WORD_MNG.SUB3, i);
                    }
                    else
                    {

                        //// ñ°ï˚ÇëIëÇµÇƒÇ¢ÇΩèÍçá
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name != "ïKíÜ"
                            || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name != "í·â∫")
                        {
                            add_++;
                            if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).getFlag == false)
                            {
                                getFalseCnt_++;
                            }
                        }
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            default:
                return ActiveCheck(add_, getFalseCnt_);
        }
    }

    public bool AttackSubWordCheck(Bag_Word.WORD_MNG kind)
    {
        int getFalseCnt_ = 0;
        int add_ = 0;
        switch (kind)
        {
            case Bag_Word.WORD_MNG.SUB1:
                for (int i = magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1); i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB1); i++)
                {
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name != "ïKíÜ"
                        || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name != "ãzé˚")
                    {
                        add_++;
                        Debug.Log("ëSëÃå¬êî" + add_);
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).getFlag == false)
                        {
                            getFalseCnt_++;
                            Debug.Log("éùÇ¡ÇƒÇ»Ç¢å¬êî" + getFalseCnt_);
                        }
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB2:
                for (int i = magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB2); i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB2); i++)
                {
                    if (magicCreate_.GetSelectWord((int)Bag_Word.WORD_MNG.SUB1) == magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name)
                    {
                        continue;
                    }
                    // ãzé˚à»äOÇ©Ç¬sub1Ç≈ëIÇÒÇæÇÃà»äO
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name != "ãzé˚")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }
                }
                Debug.Log("âÒì]êî"+ add_+"         éùÇ¡ÇƒÇ»Ç¢å¬êî" + getFalseCnt_);
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB3); i++)
                //                    for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name == "ïKíÜ")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }
                }
                return ActiveCheck(add_, getFalseCnt_);

            default:
                return ActiveCheck(add_, getFalseCnt_);
        }
    }

    private bool ActiveCheck(int add, int cnt)
    {
        if (add == cnt)
        {
            return false;
        }
        return true;
    }
}