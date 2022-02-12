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
                Debug.Log(kind + "番目の回転数" + magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1));
                for (int i = 0; i < magicCreate_.GetSelectKindMaxWord((int)InitPopList.WORD.SUB1); i++)
                {

                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name == "味方")
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
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "麻痺"
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "暗闇"
                    || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name == "毒")
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
                    // 敵を選択していた場合
                    if (magicCreate_.GetSelectWord((int)Bag_Word.WORD_MNG.SUB1) == "敵")
                    {
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name == "低下")
                        {
                            add_++;
                            if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).getFlag == false)
                            {
                                getFalseCnt_++;
                            }
                        }
                        // CommonButtonCheck(true, "低下", Bag_Word.WORD_MNG.SUB3, i);
                    }
                    else
                    {

                        //// 味方を選択していた場合
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name != "必中"
                            || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name != "低下")
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
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name != "必中"
                        || magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).name != "吸収")
                    {
                        add_++;
                        Debug.Log("全体個数" + add_);
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB1, i).getFlag == false)
                        {
                            getFalseCnt_++;
                            Debug.Log("持ってない個数" + getFalseCnt_);
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
                    // 吸収以外かつsub1で選んだの以外
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).name != "吸収")
                    {
                        add_++;
                        if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB2, i).getFlag == false)
                        {
                            getFalseCnt_++;
                        }
                    }
                }
                Debug.Log("回転数"+ add_+"         持ってない個数" + getFalseCnt_);
                return ActiveCheck(add_, getFalseCnt_);

            case Bag_Word.WORD_MNG.SUB3:
                for (int i = 0; i < magicCreate_.GetMngMaxCnt((int)Bag_Word.WORD_MNG.SUB3); i++)
                //                    for (int i = 0; i < mngMaxCnt[(int)Bag_Word.WORD_MNG.SUB3]; i++)
                {
                    if (magicCreate_.GetCreateData(Bag_Word.WORD_MNG.SUB3, i).name == "必中")
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