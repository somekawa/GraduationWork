using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    private Bag_Materia bagMateria_;

    public enum STORE
    {
        NON = -1,
        BUY,       // �����I��
        SELL,      // ����I��
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private RectTransform tradeParentCanvas_;// �A�C�e�����蔃���p�̃L�����o�X
    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// �A�C�e�����蔃���p�̃L�����o�X
                                                                          // private Canvas selectBtnParent_;

    private int saveItemsNum_;  // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
    private string saveName_;   // �I�����ꂽ�A�C�e���̖��O��ۑ�
    private int savePrice_;     // �I�����ꂽ�A�C�e���̗�����ۑ�

    private RectTransform processParent_;// �������������̂�\��������̂̐e
    private Text itemInfo_;// �A�C�e��������
    private Image selectItemIcon_;// �I�������A�C�e���̉摜

    // �������A���鐔�֘A
    private RectTransform countParent_;
    // ----�A�C�e�����֘A
    private Text itemCountText_;
    private Text maxCountText_;// �����ł���ő��
    private int itemCnt_ = 1;
    private int oldItemCnt_ = 1;
    private int minCnt_ = 1;
    private int maxCnt_ = 99;


    private Button[] cntBtn_ = new Button[2];// 0��(countDown)�@1�E(countUp)
    private Slider slider_;// ��������A�C�e���̌��ύX�p�X���C�_�[
    private bool pressFlag_ = false;

    // ----�����֘A
    private Text priceText_;
    private int totalPrice_ = 0;

    // ������
    private Text haveMoneyText_;
    private int haveMoney_ = 1001;// �f�o�b�O�p

    // �����Ă���A�C�e���̐�
    private Text haveMateriaText_;
    private int haveCnt_ = 0;

    // ItemShoppingMng�ȊO�̕\����
    private Image storeName_;// ������X�ƕ\������Text�̔w�i
    private Vector3 startStoreNamePos_;// ���X���̍��W
    private Vector3 newStoreNamePos_ = new Vector3(0.0f, 320.0f, 0.0f);// �����I�����̈ړ���
    private Button[] itemStoreBtn_ = new Button[3];

    void Start()
    {


        storeName_ = GameObject.Find("HouseInfo").GetComponent<Image>();
        startStoreNamePos_ = storeName_.gameObject.transform.localPosition;
        itemStoreBtn_[0] = GameObject.Find("BuyButton").GetComponent<Button>();
        itemStoreBtn_[1] = GameObject.Find("SellButton").GetComponent<Button>();
        itemStoreBtn_[2] = GameObject.Find("ExitButton").GetComponent<Button>();

        var gameObject = DontDestroyMng.Instance;
        bagMateria_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();

        // �����p�̐e
        tradeParentCanvas_ = GameObject.Find("ItemShoppingMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.BUY] = tradeParentCanvas_.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = tradeParentCanvas_.Find("SellMng").GetComponent<RectTransform>();

        // �I�𒆂̂��̂�\������G���A
        processParent_ = tradeParentCanvas_.Find("CheckArea").GetComponent<RectTransform>();
        RectTransform countParent = processParent_.Find("CountMng").GetComponent<RectTransform>();

        itemCountText_ = countParent.Find("TotalCount").GetComponent<Text>();
        itemCountText_.text = itemCnt_.ToString();
        oldItemCnt_ = itemCnt_;
        maxCountText_ = countParent.Find("Max/Count").GetComponent<Text>();
        // �A�C�e�������֘A
        cntBtn_[0] = countParent.Find("CountDown").GetComponent<Button>();
        cntBtn_[1] = countParent.Find("CountUp").GetComponent<Button>();
        slider_ = countParent.Find("Slider").GetComponent<Slider>();
        slider_.maxValue = maxCnt_;

        priceText_ = countParent.Find("TotalPrice").GetComponent<Text>();
        priceText_.text = totalPrice_.ToString() + "�r�b�g";

        // ������
        haveMoneyText_ = processParent_.Find("Money/Count").GetComponent<Text>();
        haveMoneyText_.text = haveMoney_.ToString() + "�r�b�g";

        // �I�����Ă���A�C�e���̏�����
        haveMateriaText_ = processParent_.transform.Find("HaveItem/Count").GetComponent<Text>();
        haveMateriaText_.text = haveCnt_.ToString();

        // �A�C�e���̐�����
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<Text>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();


        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        tradeParentCanvas_.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        storeActive_ = STORE.BUY;
        // Debug.Log(tradeParentCanvas_.name + "           " + selectBtnParent_.name);
        tradeParentCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        itemStoreBtn_[0].gameObject.SetActive(false);
        itemStoreBtn_[1].gameObject.SetActive(false);
        itemStoreBtn_[2].gameObject.SetActive(false);
        //  selectBtnParent_.gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = newStoreNamePos_;
        transform.GetComponent<Trade_Buy>().SetActiveBuy();
    }

    public void OnClickSellBtn()
    {
        storeActive_ = STORE.SELL;
        tradeParentCanvas_.gameObject.SetActive(true);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        //   selectBtnParent_.gameObject.SetActive(false);
        itemStoreBtn_[0].gameObject.SetActive(false);
        itemStoreBtn_[1].gameObject.SetActive(false);
        itemStoreBtn_[2].gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = newStoreNamePos_;
        transform.GetComponent<Trade_Sell>().SetActiveSell();
    }

    public void SetSelectItemName(string name)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            processParent_.gameObject.SetActive(true);
        }
        cntBtn_[0].interactable = false;// �ŏ��̒l�����牟���ł��Ȃ��悤�ɂ���

        for (int i = 0; i < bagMateria_.GetMaxHaveMateriaCnt(); i++)
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
                    int maxDiviCnt_ = haveMoney_ / savePrice_;// ���������猩���ő�l
                    int maxSubCnt_ = maxCnt_ - Bag_Item.itemState[i].haveCnt;// ���������猩���ő�l
                    // �������l��D�悵��maxValue�ɑ������
                    slider_.maxValue = maxSubCnt_ < maxDiviCnt_ ? maxSubCnt_ : maxDiviCnt_;
                }
                else if (storeActive_ == STORE.SELL)
                {
                    savePrice_ = InitPopList.materiaData[i].sellPrice;
                    slider_.maxValue = Bag_Item.itemState[i].haveCnt;
                }
                else
                {
                    // �������Ȃ�
                }

                maxCountText_.text = slider_.maxValue.ToString();
                
                // �w��A�C�e���̗���
                priceText_.text = savePrice_.ToString() + "�r�b�g";
                totalPrice_ = savePrice_;

                return;
            }
        }
    }

    public void OnClickCountDown()
    {
        if (storeActive_ == STORE.BUY)
        {
            //// �����ꍇ
            if (itemCnt_ <= minCnt_)
            {
                //cntBtn_[0].interactable = false;
                itemCnt_ = minCnt_;
            }
            else
            {
                itemCnt_--;
            }
        }
        else
        {
            if (Bag_Item.itemState[saveItemsNum_].haveCnt <= minCnt_)
            {
                cntBtn_[0].interactable = false;
                itemCnt_ = minCnt_;
            }
            else
            {
                itemCnt_--;
            }
        }
        if (cntBtn_[1].interactable == false)
        {
            // �E��󂪉����ł��Ȃ��Ȃ牟���\��Ԃ�
            cntBtn_[1].interactable = true;
        }
        CommonUpDown();
        slider_.value = itemCnt_;
        // totalPrice_ -= savePrice_;
        // OnNowValueCheck(-1);
    }

    public void OnClickCountUp()
    {
        itemCnt_++;
        if (storeActive_ == STORE.BUY)                // �����ꍇ
        {
            // �A�C�e���𔃂��Ƃ�
            // �g�[�^���̗�����������+�I�𒆂̃A�C�e����1�̗���
            if (totalPrice_ <  haveMoney_ -  savePrice_)
              //  || itemCnt_ <= maxCnt_ - Bag_Item.itemState[saveItemsNum_].haveCnt)
            {
                cntBtn_[1].interactable = false;
            }
        }
        else
        {
            // �A�C�e���𔄂�Ƃ�
            if (Bag_Item.itemState[saveItemsNum_].haveCnt < itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }
        if (cntBtn_[0].interactable == false)
        {
            // ����󂪉����ł��Ȃ��Ȃ牟���\��Ԃ�
            cntBtn_[0].interactable = true;
        }
        CommonUpDown();
        slider_.value = itemCnt_;
        //totalPrice_ += savePrice_;
        // totalPrice_ += savePrice_;
        // OnNowValueCheck(1);
    }

    public void OnNowValueCheck()
    {
        if (pressFlag_ == true)
        {
            pressFlag_ = false;
            return;
        }

        // �{�^���ŃX���C�_�[���������邽�߂��������ʍ��ɂȂ�
        // ��������l��\������
        Debug.Log("�I�𒆂̌�"+itemCnt_.ToString());
        totalPrice_ = savePrice_ * itemCnt_;
        itemCountText_.text = itemCnt_.ToString();
        // ��������A�C�e���̗���
        priceText_.text = totalPrice_ + "�r�b�g";

        // �X���C�_�[�ő������Ƀ{�^���̏�Ԃ�ς���
        if (itemCnt_== slider_.maxValue)
        {
            cntBtn_[1].interactable = false;
            cntBtn_[0].interactable = true;
        }
        else if(itemCnt_==minCnt_)
        {
            cntBtn_[0].interactable = false;
            cntBtn_[1].interactable = true;
        }
        else
        {
            // �������Ȃ�
        }

        oldItemCnt_ = itemCnt_;
        if (itemCnt_ < 1)
        {
            cntBtn_[0].interactable = false;
            itemCnt_ = 1;
        }
        if (storeActive_ == STORE.BUY)                // �����ꍇ
        {           
            // �g�[�^���̗�����������+�I�𒆂̃A�C�e����1�̗���
            if (haveMoney_ - savePrice_< totalPrice_)
            //  || itemCnt_ <= maxCnt_ - Bag_Item.itemState[saveItemsNum_].haveCnt)
            {
                cntBtn_[1].interactable = false;
                // if���ɓ���O�ɉ��Z������������
                itemCnt_--;
            }
        }
        //number = 0;
    }

    private void CommonUpDown()
    {
        totalPrice_ = savePrice_ * itemCnt_;

        itemCountText_.text = itemCnt_.ToString();
        // ��������A�C�e���̗���
        priceText_.text = totalPrice_ + "�r�b�g";
        pressFlag_ = true;

    }



    public void OnClickShopping()
    {
        // �w���{�^������
        //if (itemCount_ < 1)
        //{
        //    return;
        //}
        Debug.Log(saveName_ + "���w�����܂���");

        // �����������Z����

        if (storeActive_ == STORE.BUY)
        {
            //bagMateria_.MateriaGetCheck(saveItemsNum_, saveName_, itemCount_);
            //haveMoney_ -= totalPrice_;  // �����������Z����
            //haveCnt_ += itemCount_;     // �������𑝂₷
        }
        else if (storeActive_ == STORE.SELL)
        {
            //bagMateria_.MateriaGetCheck(saveItemsNum_, saveName_, -itemCount_);
            //haveMoney_ += totalPrice_;  // �����������Z����
            //haveCnt_ -= itemCount_;     // �����������炷
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
        itemCnt_ = 1;
        itemCountText_.text = itemCnt_.ToString();
        priceText_.text = totalPrice_ + "�r�b�g";
    }

    public void OnClickCancelBtn()
    {
        // ���������I��点�����Ƃ�
        storeActive_ = STORE.NON;
        tradeParentCanvas_.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        storeName_.gameObject.transform.localPosition = startStoreNamePos_;
        itemStoreBtn_[0].gameObject.SetActive(true);
        itemStoreBtn_[1].gameObject.SetActive(true);
        itemStoreBtn_[2].gameObject.SetActive(true);
        // selectBtnParent_.gameObject.SetActive(true);
    }
}