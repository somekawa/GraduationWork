using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantMng : MonoBehaviour
{
    [SerializeField]
    // ���X�g�����̃��j���[��\������UI
    private GameObject restaurantMenuUI;

    [SerializeField]
    //�@���̗�������\������v���n�u
    private Transform cookPrefab;

    private GameObject restaurantCanvas_;      // �����𒍕����邩�A�O�ɏo�邩�̃L�����o�X
    private Transform content_;                // UI�̃R���e���c

    private Text cookInfoText_;                // �������̑薼�e�L�X�g
    private Text statusUpInfoText_;            // �X�e�[�^�X�A�b�v�̑薼�e�L�X�g
    private Text numStatusUpInfoText_;         // �A�b�v���鐔���̃e�L�X�g
    private Text needFoodText_;                // �K�v�ȑf�ނ̃e�L�X�g
    private Text haveFoodText_;                // �K�v�ȑf�ނ̌��ݎ������ƁA�K�v���̐����e�L�X�g
    private Text moneyText_;                   // �K�v�Ȃ����̃e�L�X�g

    private (string, int)[] statusUp_ = new (string, int)[5];    // �A�b�v�X�e�[�^�X�̖��O�ƃA�b�v�l���y�A�ɂ����ϐ�
    private readonly string[] statusUpStr_ = { "Attack", "MagicAttack", "Defence", "Speed", "Luck" };   // �X�e�[�^�X���ɕ��ׂ�������

    private int num_ = -1;

    private List<Transform> cookUIInstanceList_;    // UI�̃C���X�^���X������
    private QuestButton[] button_;                  // �����̃{�^������

    // Excel����̃f�[�^�ǂݍ���
    private GameObject DataPopPrefab_;
    private Cook0 popCookInfo_;

    void Start()
    {
        restaurantCanvas_ = GameObject.Find("RestaurantCanvas");

        //�@�����\���pUI�̃R���e���c
        content_ = restaurantMenuUI.transform.Find("Background/Scroll View/Viewport/Content");
        cookInfoText_ = restaurantMenuUI.transform.Find("CookInfoText").GetComponent<Text>();
        statusUpInfoText_ = restaurantMenuUI.transform.Find("StatusUpInfoText").GetComponent<Text>();
        numStatusUpInfoText_ = restaurantMenuUI.transform.Find("StatusUpInfoText/StatusUpInfoNumText").GetComponent<Text>();
        needFoodText_ = restaurantMenuUI.transform.Find("NeedFoodText").GetComponent<Text>();
        haveFoodText_ = restaurantMenuUI.transform.Find("NeedFoodText/NeedFoodNumText").GetComponent<Text>();
        moneyText_ = restaurantMenuUI.transform.Find("MoneyText").GetComponent<Text>();

        DataPopPrefab_ = Resources.Load("DataPop") as GameObject;   // Resources�t�@�C�����猟������
        popCookInfo_ = DataPopPrefab_.GetComponent<PopList>().GetData<Cook0>(PopList.ListData.RESTAURANT);

        // Excel�̍s������N�G�X�g�̍��v����ݒ肷��
        var totalNum_ = popCookInfo_.param.Count;
        // ���݂̐i�s�x�ԍ��ƃN�G�X�g�̐i�s�x�ԍ����r���āA���݂̐i�s�x�ԍ��ȉ��̃N�G�X�g�̌��𐔂���
        int tmpCookNum = 0;
        for (int i = 0; i < totalNum_; i++)
        {
            if (EventMng.GetChapterNum() >= popCookInfo_.param[i].eventNum)
            {
                tmpCookNum++;
            }
        }

        cookUIInstanceList_ = new List<Transform>();
        button_ = new QuestButton[tmpCookNum];

        for (var i = 0; i < tmpCookNum; i++)
        {
            // �����p��UI�v���n�u�𐶐�����
            var uIInstance = Instantiate(cookPrefab, content_);
            cookUIInstanceList_.Add(uIInstance);

            // �ԍ���ݒ肷��
            button_[i] = uIInstance.GetComponent<QuestButton>();
            button_[i].SetQuestNum(i);

            // �����̌��o���ɗ��������o��
            cookUIInstanceList_[i].Find("TitlePanel/Button/Text").GetComponent<TMPro.TextMeshProUGUI>().text =
                popCookInfo_.param[i].name;
        }

        // ������
        for(int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }
    }

    // �u�����𒍕�����v�̃{�^������
    public void OnClickOrderButton()
    {
        Debug.Log("�����𒍕�����{�^�����������܂���");
        restaurantCanvas_.SetActive(false);
        restaurantMenuUI.SetActive(true);
    }

    // �e���j���[���J�������̉E���́u��������v�̃{�^������
    public void OnClickMenuOrderButton()
    {
        Debug.Log("�����{�^�����������܂���");

        // �t���O��true�Ȃ�H�ׂ�Ȃ��悤�ɂ���
        if(SceneMng.GetFinStatusUpTime())
        {
            Debug.Log("�����������ς��ŐH�ׂ��Ȃ����I�I");
            return;
        }

        // �J��Ԃ����p����̂ŁA�ꎞ�ϐ��ɕۑ����Ďg��
        var tmppop = popCookInfo_.param[num_];

        // ������������������邩�m�F����
        if(tmppop.needMoney > SceneMng.GetHaveMoney())
        {
            Debug.Log("��������̂ɂ���������܂���");
            return;
        }

        // �f�ނ��v������闿�����ǂ����ŏ����𕪂���
        if (tmppop.needFood <= -1)
        {
            // �K�v�f�ނ��Ȃ��Ƃ�

            // �����̌�������
            SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - tmppop.needMoney);

            // �L�����̃X�e�[�^�X�A�b�v����
            int[] tmp = { statusUp_[0].Item2, statusUp_[1].Item2, statusUp_[2].Item2, statusUp_[3].Item2, statusUp_[4].Item2 };
            for(int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
            {
                SceneMng.charasList_[i].SetStatusUpByCook(tmp);
            }
        }
        else
        {
            // �K�v�f�ނ�����Ƃ�
            // �f�ނ�����Ă��邩�m�F����
            if(Bag_Materia.materiaState[tmppop.needFood].haveCnt >= tmppop.needNum)
            {
                // �f�ނ������
                Debug.Log("�f�ނ������");

                // �����̌�������
                SceneMng.SetHaveMoney(SceneMng.GetHaveMoney() - tmppop.needMoney);

                // �L�����̃X�e�[�^�X�A�b�v����
                int[] tmp = { statusUp_[0].Item2, statusUp_[1].Item2, statusUp_[2].Item2, statusUp_[3].Item2, statusUp_[4].Item2 };
                for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
                {
                    SceneMng.charasList_[i].SetStatusUpByCook(tmp);
                }

                // �f�ނ����炷
                Bag_Materia.materiaState[tmppop.needFood].haveCnt -= tmppop.needNum;
            }
            else
            {
                // �f�ނ�����Ȃ�
                Debug.Log("�f�ނ�����Ȃ�");
                return;
            }
        }

        BackMenu();

        // ���ʂ��؂�鎞����ۑ�����
        SceneMng.SetFinStatusUpTime();

    }

    public void SetSelectOrder(int num)
    {
        Debug.Log("RestaurantMng��" + num + "���󂯎��܂���");

        // �J��Ԃ����p����̂ŁA�ꎞ�ϐ��ɕۑ����Ďg��
        var tmppop = popCookInfo_.param[num];

        // �������Ɛ������o��
        cookInfoText_.text = tmppop.name + "\n\n" + tmppop.info;

        // �J���}��؂�
        var tmp = tmppop.statusUp.Split(',');

        List<string> tmpList = new List<string>();
        List<int> tmpListNum = new List<int>();

        // �J���}��؂�ɂȂ������̂�����ɃA���_�[�o�[�ŋ�؂�
        for (int i = 0; i < tmp.Length; i++)
        {
            var underbarSplit = tmp[i].Split('_');
            // �A���_�[�o�[�ŋ�؂������̂��A�ʁX�̃��X�g�֓����
            tmpList.Add(underbarSplit[0]);
            tmpListNum.Add(int.Parse(underbarSplit[1]));
        }

        // �����̏�����
        statusUpInfoText_.text = "Status Up";
        numStatusUpInfoText_.text = "";
        needFoodText_.text = "Need Food";
        haveFoodText_.text = "Have";

        // �X�e�[�^�X�A�b�v�������o��
        for (int i = 0; i < tmpList.Count; i++)
        {
            statusUpInfoText_.text += "\n�E" + tmpList[i];
            numStatusUpInfoText_.text += "\n+" + tmpListNum[i];

            // �Y������ꏊ�ɐ���������
            for(int k = 0; k < statusUp_.Length; k++)
            {
                // ���O��v
                if(statusUp_[k].Item1 == tmpList[i])
                {
                    statusUp_[k].Item2 = tmpListNum[i];
                    break;  // 1�x��v����Ƃ�����������̂�break�Ŕ�����
                }
            }
        }

        // �K�v�f�ނ̖��̂��o��(��������f�ޖ������)
        if(tmppop.needFood <= -1)
        {
            // needFood�̒l��-1�̂Ƃ��͕K�v�f�ނ�����������A�\���͂Ȃ��ɂ���
            needFoodText_.text += "\n�Ȃ�";
            haveFoodText_.text += "\n0/0";
        }
        else
        {
            // �K�v�f�ނƐ��ɂ��ĕ\������
            needFoodText_.text += "\n" + Bag_Materia.materiaState[tmppop.needFood].name;
            // ������ / �K�v��
            haveFoodText_.text += "\n" + Bag_Materia.materiaState[tmppop.needFood].haveCnt + "/" + tmppop.needNum;
        }

        // �K�v�Ȃ������o��
        moneyText_.text = tmppop.needMoney + "�r�b�g";

        // �I�𒆂̃��j���[��ۑ�����
        num_ = num;
    }

    public void OnClickBackButton()
    {
        Debug.Log("�߂�{�^�����������܂���");
        BackMenu();
    }

    // ��ʂ�߂��ۂɕK�v�ȏ���
    private void BackMenu()
    {
        restaurantCanvas_.SetActive(true);
        restaurantMenuUI.SetActive(false);

        // �l�̏�����
        for (int i = 0; i < statusUp_.Length; i++)
        {
            statusUp_[i] = (statusUpStr_[i], 0);
        }
    }
}
