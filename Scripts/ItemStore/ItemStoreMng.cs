using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreMng : MonoBehaviour
{
    [SerializeField]
    private GameObject itemStoreUI;    // ������X�p��UI

    public enum KIND
    {
        NON = -1,
        MATERIA,
        ITEM,
        MAX,
    }
    private Button[] kindBtn_=new Button[(int)KIND.MAX];// 0�@�f�ށ@1�A�C�e��
    private KIND selectKind_ = KIND.NON;

    public enum STORE
    {
        NON = -1,
        BUY,       // �����I��
        SELL,      // ����I��
        MAX
    }
    private STORE storeActive_ = STORE.NON;

    private RectTransform[] tradeMng_ = new RectTransform[(int)STORE.MAX];// �A�C�e�����蔃���p�̃L�����o�X

    private int saveItemsNum_;  // �I�����ꂽ�A�C�e���̔ԍ���ۑ�
   // private string saveName_;   // �I�����ꂽ�A�C�e���̖��O��ۑ�
    private int savePrice_;     // �I�����ꂽ�A�C�e���̗�����ۑ�
    private int kindNum_ = 0;

    private RectTransform processParent_;// �������������̂�\��������̂̐e
    private RectTransform cntParent_;
    private TMPro.TextMeshProUGUI itemInfo_;// �A�C�e��������
    private Image selectItemIcon_;// �I�������A�C�e���̉摜
    private Image exSymbol_;        // Ex�A�C�e���p�V���{��

    // �������A���鐔�֘A
    private RectTransform shoppingParent_;// �������
    private TMPro.TextMeshProUGUI shoppingText_;// ���������邩
    // ----�A�C�e�����֘A
    private TMPro.TextMeshProUGUI itemCountText_;// ��������������\��
    private TMPro.TextMeshProUGUI maxCountText_;// �����ł���ő����\��
    private TMPro.TextMeshProUGUI minCountText_;// �����ł���ŏ�����\��
    private int itemCnt_ = 1;
    private int minBuyCnt_ = 1;// �������̍ŏ��l
    private int minSellCnt_ = 0;// ����Ƃ��̍ŏ��l
    private int maxCnt_ = 99;// �ő��


    private Button[] cntBtn_ = new Button[2];// 0��(countDown)�@1�E(countUp)
    private Slider slider_;// ��������A�C�e���̌��ύX�p�X���C�_�[
    private bool pressFlag_ = false;

    // ----�����֘A
    private TMPro.TextMeshProUGUI priceText_;
    private int totalPrice_ = 0;

    // ������
    private TMPro.TextMeshProUGUI haveMoneyText_;
    private int haveMoney_ = 1001;// �f�o�b�O�p

    // ������
    private TMPro.TextMeshProUGUI haveCntText_;
    private int haveCnt_ = 0;

    // 0�����{�^���@1����{�^���@2�O�ɏo��{�^��
  //  private Button[] itemStoreBtn_ = new Button[3];
    private Canvas itemStoreCanvas_;

    private Bag_Materia bagMateria_;// �f�ޑI�����g�p
    private Bag_Item bagItem_;// �A�C�e���I�����g�p

    private InitPopList popItemList_;
    private int itemMaxCnt_ = 0;
    void Start()
    {
        itemStoreCanvas_ = GameObject.Find("ItemStoreCanvas").GetComponent<Canvas>();
        popItemList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();
        itemMaxCnt_ = popItemList_.SetMaxItemCount();

        var gameObject = DontDestroyMng.Instance;
        bagMateria_ = gameObject.transform.Find("Managers").GetComponent<Bag_Materia>();
        bagItem_ = gameObject.transform.Find("Managers").GetComponent<Bag_Item>();
        // �����p�̐e
        tradeMng_[(int)STORE.BUY] = itemStoreUI.transform.Find("BuyMng").GetComponent<RectTransform>();
        tradeMng_[(int)STORE.SELL] = itemStoreUI.transform.Find("SellMng").GetComponent<RectTransform>();
        // �f�ނƃA�C�e���ǂ����I�����Ă��邩
        kindBtn_[(int)KIND.MATERIA] = itemStoreUI.transform.Find("MateriaButton").GetComponent<Button>();
        kindBtn_[(int)KIND.ITEM] = itemStoreUI.transform.Find("ItemButton").GetComponent<Button>();
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;

        // �I�𒆂̂��̂�\������G���A
        processParent_ = itemStoreUI.transform.Find("CheckArea").GetComponent<RectTransform>();
        // �A�C�e���̐�����
        itemInfo_ = processParent_.transform.Find("InfoArea/InfoText").GetComponent<TMPro.TextMeshProUGUI>();
        itemInfo_.text = "";
        selectItemIcon_ = processParent_.transform.Find("InfoArea/ItemImage").GetComponent<Image>();
        exSymbol_ = processParent_.transform.Find("InfoArea/ExSymbol").GetComponent<Image>();
        exSymbol_.gameObject.SetActive(false);
        // ������
        var myselfPaent = processParent_.Find("MyselfData").GetComponent<RectTransform>();
        haveMoney_ = SceneMng.GetHaveMoney();
        haveMoneyText_ = myselfPaent.Find("Money/Count").GetComponent<TMPro.TextMeshProUGUI>();
        haveMoneyText_.text = haveMoney_.ToString() ;
        // �I�����Ă���A�C�e���̏�����
        haveCntText_ = myselfPaent.transform.Find("HaveItem/Count").GetComponent<TMPro.TextMeshProUGUI>();
        haveCntText_.text = haveCnt_.ToString()+"�R";

        shoppingParent_ = processParent_.Find("ShoppingData").GetComponent<RectTransform>();
        shoppingText_= shoppingParent_.Find("ShoppingButton/Text").GetComponent<TMPro.TextMeshProUGUI>();
        itemCountText_ = shoppingParent_.Find("TotalCount").GetComponent<TMPro.TextMeshProUGUI>();
        priceText_ = shoppingParent_.Find("PricePleate/TotalPrice").GetComponent<TMPro.TextMeshProUGUI>();
        itemCountText_.text = itemCnt_.ToString() + "�R";
        priceText_.text = totalPrice_.ToString();

        cntParent_ = shoppingParent_.Find("CountMng").GetComponent<RectTransform>();
        maxCountText_ = cntParent_.Find("Max/Count").GetComponent<TMPro.TextMeshProUGUI>();
        minCountText_ = cntParent_.Find("Min/Count").GetComponent<TMPro.TextMeshProUGUI>(); 
        // �A�C�e�������֘A
        cntBtn_[0] = cntParent_.Find("CountDown").GetComponent<Button>();
        cntBtn_[1] = cntParent_.Find("CountUp").GetComponent<Button>();
        cntBtn_[0].interactable = false;
        cntBtn_[1].interactable = true;
        slider_ = cntParent_.Find("Slider").GetComponent<Slider>();
        slider_.maxValue = maxCnt_;// �X���C�_�[�̍ő�l��ݒ�


        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);

        itemStoreUI.transform.gameObject.SetActive(false);
    }

    public void OnClickBuyBtn()
    {
        SceneMng.SetSE(0);
        CommonClickActive(STORE.BUY);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(true);
        shoppingText_.text = "�w��";
        transform.GetComponent<Trade_Buy>().Init((int)KIND.MATERIA, (int)KIND.ITEM);
    }

    public void OnClickSellBtn()
    {
        SceneMng.SetSE(0);
        CommonClickActive(STORE.SELL);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(true);
        shoppingText_.text = "���p";
        transform.GetComponent<Trade_Sell>().Init((int)KIND.MATERIA, (int)KIND.ITEM);
    }

    private void CommonClickActive(STORE store)
    {
        storeActive_ = store;
        selectKind_ = KIND.MATERIA;
        itemStoreUI.transform.gameObject.SetActive(true);
        kindBtn_[(int)KIND.MATERIA].interactable = false;// �f�ނ�\��
        kindBtn_[(int)KIND.ITEM].interactable = true;// �A�C�e�����\��
        itemStoreCanvas_.gameObject.SetActive(false);
    }

    public void SetSelectItemName(int num)
    {
        if (processParent_.gameObject.activeSelf == false)
        {
            // �{�^����X���C�_�[��\������
            processParent_.gameObject.SetActive(true);
        }
        // �Ăяo�������Ⴄ����1��1��if���Ń`�F�b�N����
        // �f�ނƃA�C�e���ǂ����I�����Ă��邩�i0:�f�ށA1:�A�C�e��
        itemInfo_.text = selectKind_ == KIND.ITEM ?  InitPopList.itemData[num].info: InitPopList.materiaData[num].info;
        // �w��A�C�e���̏�����
        haveCnt_ = selectKind_ == KIND.ITEM ? Bag_Item.itemState[num].haveCnt : Bag_Materia.materiaState[num].haveCnt;
        haveCntText_.text = haveCnt_.ToString() + "�R";
        // �w��A�C�e���̉摜
        var imageKinds = selectKind_ == KIND.ITEM ? ItemImageMng.IMAGE.ITEM: ItemImageMng.IMAGE.MATERIA;
        int imageNum = num;
        if (exSymbol_.gameObject.activeSelf == true)
        {
            // ��{�I��false
            exSymbol_.gameObject.SetActive(false);
        }
        if (imageKinds == ItemImageMng.IMAGE.ITEM)
        {
            // Ex�A�C�e���������ꂽ��true�ɂ���
            if(itemMaxCnt_ /2<= num)
            {
                imageNum -= itemMaxCnt_/2;
                exSymbol_.gameObject.SetActive(true);
            }
        }
        Debug.Log("�ԍ��F" + num + "    �����F" + itemInfo_.text);
        selectItemIcon_.sprite = ItemImageMng.spriteMap[imageKinds][imageNum];

        saveItemsNum_ = num;    // �ԍ���ۑ�
       // saveName_ = name;       // �A�C�e����ۑ�

        // �\�������Ă���
        shoppingParent_.gameObject.SetActive(true);

        if (storeActive_ == STORE.BUY)
        {
            savePrice_ = selectKind_ == KIND.ITEM ? InitPopList.itemData[num].buyPrice:InitPopList.materiaData[num].buyPrice ;
            Debug.Log("�I�������A�C�e���̒l�i" + savePrice_);
            if (SceneMng.GetHaveMoney() < savePrice_ || maxCnt_ <= haveCnt_)
            {               

                // �ő�܂ŏ������Ă��菊����������Ȃ�������w���ł��Ȃ�
                shoppingParent_.gameObject.SetActive(false);
                return;
            }

            int maxDiviCnt_ = SceneMng.GetHaveMoney() / savePrice_;// ���������猩���ő�l
            int maxSubCnt_ = maxCnt_ - haveCnt_;// ���������猩���ő�l
                                                // �������l��D�悵��maxValue�ɑ������
            slider_.maxValue = maxSubCnt_ < maxDiviCnt_ ? maxSubCnt_ : maxDiviCnt_;
            slider_.minValue = minBuyCnt_;// �ŏ�1
        }
        else if (storeActive_ == STORE.SELL)
        {
            if ( haveCnt_<=minSellCnt_)
            {
                // �������Ă��Ȃ�������\�����Ȃ�
                shoppingParent_.gameObject.SetActive(false);
                return;
            }
            savePrice_ = selectKind_ == KIND.ITEM ?  InitPopList.itemData[num].sellPrice: InitPopList.materiaData[num].sellPrice;
            slider_.maxValue = haveCnt_;
            slider_.minValue = minSellCnt_;
        }
        else
        {
            // �������Ȃ�
        }
        Debug.Log("�ԍ�"+ saveItemsNum_ + "     totalPrice_" + totalPrice_ + "        savePrice_   " + savePrice_);
        totalPrice_ = savePrice_;
        maxCountText_.text = slider_.maxValue.ToString();
        minCountText_.text = slider_.minValue.ToString();
        if (slider_.minValue == slider_.maxValue)
        {
            // �ŏ��ƍő傪�����l�Ȃ���ύX�n���\����
            cntParent_.gameObject.SetActive(false);
        }
        else
        {
            cntParent_.gameObject.SetActive(true);
            cntBtn_[1].interactable = true;
            slider_.value = slider_.minValue;
        }
        cntBtn_[0].interactable = false;// �ŏ��̒l�����牟���ł��Ȃ��悤�ɂ���
        // �I�����͍ŏ��l��
        itemCountText_.text = slider_.minValue.ToString() + "�R";
        // �w��A�C�e���̗���
        priceText_.text = totalPrice_.ToString() ;
    }

    public void OnClickCountDown()
    {
        itemCnt_--;
        if (itemCnt_ <= slider_.minValue)
        {
            cntBtn_[0].interactable = false;
        }
        Debug.Log("�I��" + itemCnt_ + "        �F�ŏ��l" + slider_.minValue);
        // �E��󂪉����ł��Ȃ��Ȃ牟���\��Ԃ�
        cntBtn_[1].interactable =  itemCnt_<slider_.maxValue  ? true : false;
        CommonUpDown();
        slider_.value = itemCnt_;
    }

    public void OnClickCountUp()
    {
        itemCnt_++;
        if (storeActive_ == STORE.BUY)                // �����ꍇ
        {
            // �A�C�e���𔃂��Ƃ�
            // �g�[�^���̗�����������+�I�𒆂̃A�C�e����1�̗���
            if (slider_.maxValue <= itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }
        else
        {
            // �A�C�e���𔄂�Ƃ�
            if (haveCnt_ <= itemCnt_)
            {
                cntBtn_[1].interactable = false;
            }
        }

        // ����󂪉����ł��Ȃ��Ȃ牟���\��Ԃ�
        cntBtn_[0].interactable = slider_.minValue < itemCnt_ ? true : false;

        CommonUpDown();
        slider_.value = itemCnt_;
    }

    private void CommonUpDown()
    {
        totalPrice_ = savePrice_ * itemCnt_;

        itemCountText_.text = itemCnt_.ToString() + "�R";
        // ��������A�C�e���̗���
        priceText_.text = totalPrice_.ToString();
        pressFlag_ = true;
    }

    public void OnNowValueCheck()
    {
        if (pressFlag_ == true)
        {
            // �{�^�������ł̃X���C�_�[�ړ��̏ꍇ�����ɓ���Ȃ��悤�ɂ���
            pressFlag_ = false;
            return;
        }

        // ��������l��\������
     //   Debug.Log("�I�𒆂̌�" + itemCnt_.ToString());
        itemCnt_ = (int)slider_.value;// �X���C�_�[�̈ʒu���A�C�e���̌�
        totalPrice_ = savePrice_ * itemCnt_;
        itemCountText_.text = itemCnt_.ToString() + "�R";
        // ��������A�C�e���̗���
        priceText_.text = totalPrice_.ToString();

        // �X���C�_�[�ő������Ƀ{�^���̏�Ԃ�ς���
        if (slider_.minValue < slider_.value && slider_.value < slider_.maxValue)
        {
            cntBtn_[0].interactable = true;
            cntBtn_[1].interactable = true;
        }
        else if (slider_.maxValue <= itemCnt_)
        {
            cntBtn_[1].interactable = false;
            cntBtn_[0].interactable = true;
        }
        else if (itemCnt_ <= slider_.minValue)
        {
            cntBtn_[0].interactable = false;
            cntBtn_[1].interactable = true;
        }
        else
        {
            // �������Ȃ�
        }
    }


    public void OnClickShopping()
    {
        SceneMng.SetSE(0);

        // �w���{�^������
       // Debug.Log(saveName_ + "���w�����܂���");

        // �����������Z����

        if (storeActive_ == STORE.BUY)
        {
            if (selectKind_ == KIND.MATERIA)
            {
                bagMateria_.MateriaGetCheck(saveItemsNum_,  itemCnt_);
            }
            else
            {
                bagItem_.ItemGetCheck(0, saveItemsNum_, itemCnt_);
            }
            // �����̌�������
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - totalPrice_);
           // haveMoney_ -= totalPrice_;  // �����������Z����
            haveCnt_ += itemCnt_;     // �������𑝂₷
        }
        else if (storeActive_ == STORE.SELL)
        {
            if (selectKind_ == KIND.MATERIA)
            {
                bagMateria_.MateriaGetCheck(saveItemsNum_,  -itemCnt_);
            }
            else
            {
                bagItem_.ItemGetCheck(0, saveItemsNum_, -itemCnt_);
            }
            // �����̉��Z����
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() + totalPrice_);
            //haveMoney_ += totalPrice_;  // �����������Z����
            haveCnt_ -= itemCnt_;     // �����������炷
            Debug.Log("�A�C�e��������" + haveCnt_);
            if (haveCnt_ < 1)
            {
                transform.GetComponent<Trade_Sell>().SetHaveCntCheck(kindNum_,saveItemsNum_);
                processParent_.gameObject.SetActive(false);
            }
        }
        else
        {
            // �������Ȃ�
        }

        // �\�����鏊�����̒l��ς���
        haveMoneyText_.text = SceneMng.GetHaveMoney().ToString();
        haveCntText_.text = haveCnt_.ToString() + "�R";// �\�����鏊������ύX
        slider_.value = 0;
        // �����������˂ČĂяo��
        SetSelectItemName(saveItemsNum_);
        //SetSelectItemName(saveItemsNum_, saveName_);
    }

    public void OnClickCancelBtn()
    {
        // ���������I��点�����Ƃ�
        storeActive_ = STORE.NON;
        selectKind_ = KIND.NON;
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;
        itemStoreUI.transform.gameObject.SetActive(false);
        tradeMng_[(int)STORE.BUY].gameObject.SetActive(false);
        tradeMng_[(int)STORE.SELL].gameObject.SetActive(false);
        processParent_.gameObject.SetActive(false);
        //itemStoreBtn_[0].gameObject.SetActive(true);
        //itemStoreBtn_[1].gameObject.SetActive(true);
        //itemStoreBtn_[2].gameObject.SetActive(true);
        itemStoreCanvas_.gameObject.SetActive(true);
    }




    public void OnClickItemButton()
    {
        selectKind_ = KIND.ITEM;
        if (storeActive_ == STORE.BUY)
        {
            transform.GetComponent<Trade_Buy>().Init((int)selectKind_, (int)KIND.MATERIA);
        }
        else
        {
            transform.GetComponent<Trade_Sell>().Init((int)selectKind_, (int)KIND.MATERIA);
        }
        processParent_.gameObject.SetActive(false);
        kindBtn_[(int)KIND.MATERIA].interactable = true;
        kindBtn_[(int)KIND.ITEM].interactable = false;
    }

    public void OnClickMateriaButton()
    {
        selectKind_ = KIND.MATERIA;
        if (storeActive_ == STORE.BUY)
        {
            transform.GetComponent<Trade_Buy>().Init((int)selectKind_, (int)KIND.ITEM);
        }
        else
        {
             transform.GetComponent<Trade_Sell>().Init((int)selectKind_, (int)KIND.ITEM);
        }
        processParent_.gameObject.SetActive(false);
        kindBtn_[(int)KIND.MATERIA].interactable = false;
        kindBtn_[(int)KIND.ITEM].interactable = true;
    }

    public void OnClickBackButton()
    {
        Debug.Log("�߂�{�^�����������܂���");
        itemStoreCanvas_.gameObject.SetActive(true);
        itemStoreUI.SetActive(false);

    }
}