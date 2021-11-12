using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    private Bag_Materia materia_;

    public enum STORE
    {
        NON = -1,
        BUY,       // �����I��
        SELL,      // ����I��
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private Canvas tradeCanvas_;// �A�C�e�����蔃���p�̃L�����o�X
    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// �A�C�e�����蔃���p�̃L�����o�X
    private Canvas selectBtnParent_;

    private int saveItemsNum_;  // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
    private string saveName_;   // �I�����ꂽ�A�C�e���̖��O��ۑ�
    private int savePrice_;     // �I�����ꂽ�A�C�e���̗�����ۑ�

    private RectTransform processParent_;// �������������̂�\��������̂̐e
    private Text itemInfo_;// �A�C�e��������
    private Image selectItemIcon_;// �I�������A�C�e���̉摜

    // �������A���鐔�֘A
    private RectTransform countParent_;
    // ----�A�C�e�����֘A
    private Text countText_;
    private int itemCount_ = 1;
    private int maxCnt_ = 99;

    // ----�����֘A
    private Text priceText_;
    private int totalPrice_ = 0;

    // ������
    private Text haveMoneyText_;
    private int haveMoney_ = 9990;// �f�o�b�O�p

    // �����Ă���A�C�e���̐�
    private Text haveMateriaText_;
    private int haveCnt_ = 0;
    void Start()
    {
        var gameObject = DontDestroyMng.Instance;
        materia_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();

        // �����p�̐e
        tradeCanvas_ = GameObject.Find("ItemShoppingMng").GetComponent<Canvas>();
        tradeMng_[(int)STORE.BUY] = tradeCanvas_.transform.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = tradeCanvas_.transform.Find("SellMng").GetComponent<RectTransform>();

        // �I�𒆂̂��̂�\������G���A
        processParent_ = tradeCanvas_.transform.Find("CheckArea").GetComponent<RectTransform>();

        // ������
        haveMoneyText_ = processParent_.transform.Find("Money/Count").GetComponent<Text>();
        haveMoneyText_.text = haveMoney_.ToString() + "�r�b�g";

        // �I�����Ă���A�C�e���̏�����
        haveMateriaText_ = processParent_.transform.Find("HaveItem/Count").GetComponent<Text>();
        haveMateriaText_.text = haveCnt_.ToString();

        // �A�C�e���̐�����
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<Text>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();

        selectBtnParent_ = GameObject.Find("InHouseCanvas").GetComponent<Canvas>();

        countParent_ = processParent_.transform.Find("CountMng").GetComponent<RectTransform>();
        countText_ = countParent_.transform.Find("BuyCount").GetComponent<Text>();
        countText_.text = itemCount_.ToString();
        priceText_ = countParent_.transform.Find("TotalPrice").GetComponent<Text>();
        priceText_.text = totalPrice_.ToString() + "�r�b�g";

        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        tradeCanvas_.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        storeActive_ = STORE.BUY;
        Debug.Log(tradeCanvas_.name + "           " + selectBtnParent_.name);
        tradeCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
        transform.GetComponent<Trade_Buy>().SetActiveBuy();
    }

    public void OnClickSellBtn()
    {
        storeActive_ = STORE.SELL;
        tradeCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        selectBtnParent_.gameObject.SetActive(false);
        transform.GetComponent<Trade_Sell>().SetActiveSell();
    }

    public void SetSelectItemName(string name)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            processParent_.gameObject.SetActive(true);
        }

        // ����������
        itemCount_ = 1;
        countText_.text = itemCount_.ToString();

        for (int i = 0; i < materia_.GetMaxHaveMateriaCnt(); i++)
        {
            // name��m�Ԗڂ�MateriaName�Ɠ����Ȃ�
            if (name == InitPopList.materiaData[i].name)
            {
                // ������
                itemInfo_.text = InitPopList.materiaData[i].info;

                // �摜
                selectItemIcon_.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];
                saveItemsNum_ = i;
                saveName_ = name;
                Debug.Log(saveName_ + "��I�т܂���");

                // �w��A�C�e���̏�����
                haveCnt_ = Bag_Materia.materiaState[saveItemsNum_].haveCnt;
                haveMateriaText_.text = haveCnt_.ToString();

                if (storeActive_ == STORE.BUY)
                {
                    savePrice_ = InitPopList.materiaData[i].buyPrice;
                }
                else if (storeActive_ == STORE.SELL)
                {
                    savePrice_ = InitPopList.materiaData[i].sellPrice;
                }
                else
                {
                    // �������Ȃ�
                }

                // �w��A�C�e���̗���
                priceText_.text = savePrice_.ToString() + "�r�b�g";
                totalPrice_ = savePrice_;

                return;
            }
        }
    }

    public void OnClickCountDown()
    {
        // �����������A���肽���������炷
        if (itemCount_ <= 1)
        {
            // 1�ȉ��ɂȂ�Ȃ��悤�ɂ���
            itemCount_ -= 0;
            totalPrice_ -= 0;
        }
        else
        {
            itemCount_--;
            totalPrice_ -= savePrice_;
        }

        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "�r�b�g";
    }

    public void OnClickCountUp()
    {
        // �����������A���肽�����𑝂₷
        if (storeActive_ == STORE.BUY)
        {
            if (haveMoney_-savePrice_ < totalPrice_)
            {
                itemCount_ += 0;
                totalPrice_ += 0;
            }
            else
            {
                CommonCountUp(maxCnt_);
            }
        }
        else if (storeActive_ == STORE.SELL)
        {
            // �w��̑f�ނ̏�������葽�����낤�Ƃ��Ă�����
            CommonCountUp(Bag_Materia.materiaState[saveItemsNum_].haveCnt);
        }
        else
        {
            // �������Ȃ�
        }

        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "�r�b�g";
    }

    private void CommonCountUp(int max)
    {
        if (max <= itemCount_)
        {
            // �������ŉ��Z���~�߂�
            itemCount_ = max;
        }
        else
        {
            itemCount_++;
            totalPrice_ += savePrice_;
        }
    }

    public void OnClickShopping()
    {
        // �w���{�^������
        if (itemCount_ < 1)
        {
            return;
        }
        Debug.Log(saveName_ + "���w�����܂���");

        // �����������Z����

        if (storeActive_ == STORE.BUY)
        {
            materia_.MateriaGetCheck(saveItemsNum_, saveName_, itemCount_);
            haveMoney_ -= totalPrice_;  // �����������Z����
            haveCnt_ += itemCount_;     // �������𑝂₷
        }
        else if (storeActive_ == STORE.SELL)
        {
            materia_.MateriaGetCheck(saveItemsNum_, saveName_, -itemCount_);
            haveMoney_ += totalPrice_;  // �����������Z����
            haveCnt_ -= itemCount_;     // �����������炷
            if (haveCnt_ < 1)
            {
                transform.GetComponent<Trade_Sell>().SetHaveCntCheck(saveItemsNum_);
            }
        }

        // �\�����鏊�����̒l��ς���
        haveMoneyText_.text = haveMoney_.ToString() + "�r�b�g";
        haveMateriaText_.text = haveCnt_.ToString();// �\�����鏊������ύX

        // �l��������
        totalPrice_ = 0;
        itemCount_ = 0;
        countText_.text = itemCount_.ToString();
        priceText_.text = totalPrice_ + "�r�b�g";
    }

    public void OnClickCancelBtn()
    {
        // ���������I��点�����Ƃ�
        storeActive_ = STORE.NON;
        tradeCanvas_.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        selectBtnParent_.gameObject.SetActive(true);
    }
}