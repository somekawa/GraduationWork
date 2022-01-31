using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    // �t�B�[���h�̃��j���[��ʂ���̂ݎg�p�\:0
    // �퓬���̂ݎg�p�\    :1
    // �ǂ���ł��g�p�\    :2
    private byte[] item_ = new byte[38]{
            2,//0:HP�|�[�V����(��)
            1,//1:�ŏ���
            1,//2:�U���A�C�e��(���S��)
            0,//3:�J�G���[��
            1,//4:�h��̓A�b�v(�P��)
            1,//5:����/���@�U���̓A�b�v(�P��)
            1,//6:���x�A�b�v(�P��)
            1,//7:�j�Q���[��
            2,//8:HP�|�[�V����(��)
            2,//9:MP�|�[�V����(��)
            1,//10:�Èŏ���
            1,//11:��჏���
            1,//12:�U���A�C�e��(���S��)
            0,//13:�G���J�E���g��(�ቺ)
            1,//14:�h��(HP����Ԃ�)
            2,//15:HP�|�[�V����(��)
            2,//16:MP�|�[�V����(��)
            1,//17:�����g����
            1,//18:�h��(HP�S��)
            // �������牺�͑听������---------------
            2,//19:HP�|�[�V����(��)
            1,//20:�ŏ���
            1,//21:�U���A�C�e��(���S��)
            0,//22:�J�G���[��
            1,//23:�h��̓A�b�v(�P��)
            1,//24:����/���@�U���̓A�b�v(�P��)
            1,//25:���x�A�b�v(�P��)
            1,//26:�j�Q���[��
            2,//27:HP�|�[�V����(��)
            2,//28:MP�|�[�V����(��)
            1,//29:�Èŏ���
            1,//30:��჏���
            1,//31:�U���A�C�e��(���S��)
            0,//32:�G���J�E���g��(�ቺ)
            1,//33:�h��(HP����Ԃ�)
            2,//34:HP�|�[�V����(��)
            2,//35:MP�|�[�V����(��)
            1,//36:�����g����
            1 //37:�h��(HP�S��)
    };

    private (int, int) hpmpNum_ = (0,0);
    private CharaBase.CONDITION condition_ = CharaBase.CONDITION.NON;
    private int itemNum_ = -1;
    private (int,int) buff_ = (-1,-1);
    private bool buffExItemFlg_ = false;    // �听���A�C�e���Ȃ�o�t�ʂ𑝂₷
    private GameObject charasText_;
    private string alive = "";

    // ��ʂ��J��������1��ƁA�񕜖���1��Ă�
    public void TextInit(Text[] text)
    {
        // �L�����̃X�e�[�^�X�l��\����������
        for (int i = 0; i < (int)SceneMng.CHARACTERNUM.MAX; i++)
        {
            var data = SceneMng.GetCharasSettings(i);

            text[i].text = data.name + "\n" +
                     "HP " + data.HP.ToString() + "/" + data.maxHP.ToString() + "\n" +
                     "MP " + data.MP.ToString() + "/" + data.maxMP.ToString();
        }
    }

    // �Ԃ�l�ɂ��āc(bool->���g���邩�ǂ���,bool->�g���ۂɑΏۂ̎w�肪�K�v���ǂ���)
    // 2�ڂ�true�ɂȂ�^�C�~���O�͉񕜌n�̂�
    public (bool,bool) Use(int itemNum)
    {
        Debug.Log(itemNum + "�Ԃ̃A�C�e�����g�p���܂�");

        // ���݃t�B�[���h�̃��j���[��ʂȂ��
        // item_�ϐ����u�P�v�̃A�C�e���͎g�p�s��
        if(FieldMng.nowMode  == FieldMng.MODE.MENU && item_[itemNum] == 1)
        {
            Debug.Log("�퓬���ɂ����g���Ȃ��A�C�e�����g�p���悤�Ƃ��܂���");
            return (false,false);
        }

        // ���ݐ퓬���������͋����퓬���Ȃ��
        // item_�ϐ����u�O�v�̃A�C�e���͎g�p�s��
        if ((FieldMng.nowMode == FieldMng.MODE.BUTTLE || FieldMng.nowMode == FieldMng.MODE.FORCEDBUTTLE) &&
           item_[itemNum] == 0)
        {
            Debug.Log("�T�����ɂ����g���Ȃ��A�C�e�����g�p���悤�Ƃ��܂���");
            return (false, false);
        }

        bool tmpFlg = false;
        switch (itemNum)
        {
            case 0:    // HP�|�[�V����(��)
                hpmpNum_ = (10, 0);
                tmpFlg = true;
                break;
            case 1:    // �ŏ���
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 2:    // �S�̏��_���[�W
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(15);
                break;
            case 3:    // �J�G���[��(�t�B�[���h���烆�j�n�E�X��)
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 4:    // �h��̓A�b�v(�P��)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 5:    // ����/���@�U���̓A�b�v(�P��)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 6:    // ���x�A�b�v(�P��)
                buffExItemFlg_ = false;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 7:    // �j�Q���[��(�����퓬�łȂ��ꍇ�͎g�p�\�Ƃ���)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    var tmp = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
                    tmp.CallDeleteEnemy();
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    SceneMng.charMap_[SceneMng.CHARACTERNUM.UNI].gameObject.transform.position = tmp.GetFieldPos();
                    // �A�C�e����ʂ����
                    GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
                    Debug.Log("Uni�͓����o����");
                }
                else
                {
                    return (false, tmpFlg);
                }
                break;
            case 8:    // HP�|�[�V����(��)
                hpmpNum_ = (40, 0);
                tmpFlg = true;
                break;
            case 9:    // MP�|�[�V����(��)
                hpmpNum_ = (0, 10);
                tmpFlg = true;
                break;
            case 10:    // �Èŏ���
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 11:    // ��჏���
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 12:    // �S�̒��_���[�W
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(30);
                break;
            case 13:    // �G���J�E���g���ቺ
                //@ ���݂̃G���J�E���g�F�̂܂܁A��莞�ԃG���J�E���g���Ȃ��Ȃ�悤�ɂ���
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 14:    // �h��(�ő�HP�̔���)
                alive = "half";
                tmpFlg = true;
                break;
            case 15:    // HP�|�[�V����(��)
                hpmpNum_ = (100, 0);
                tmpFlg = true;
                break;
            case 16:    // MP�|�[�V����(��)
                hpmpNum_ = (0, 50);
                tmpFlg = true;
                break;
            case 17:    // �����g����
                // �����Ă��邾���Ō��ʂ��ł邩��A�����ɏ����͏����Ȃ�
                break;
            case 18:    // �h��(HP�S��)
                alive = "all";
                tmpFlg = true;
                break;
                // �������牺�͑听������--------------------------------------------------
            case 19:    // HP�|�[�V����(��)
                hpmpNum_ = (35, 0);
                tmpFlg = true;
                break;
            case 20:    // �ŏ���
                condition_ = CharaBase.CONDITION.POISON;
                tmpFlg = true;
                break;
            case 21:    // �S�̏��_���[�W
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(20);
                break;
            case 22:    // �J�G���[��(�t�B�[���h���烆�j�n�E�X��)
                FieldMng.nowMode = FieldMng.MODE.SEARCH;
                FieldMng.oldMode = FieldMng.MODE.NON;
                SceneMng.SceneLoad((int)SceneMng.SCENE.UNIHOUSE);
                break;
            case 23:    // �h��̓A�b�v(�P��)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 24:    // ����/���@�U���̓A�b�v(�P��)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 25:    // ���x�A�b�v(�P��)
                buffExItemFlg_ = true;
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 26:    // �j�Q���[��(�����퓬�łȂ��ꍇ�͎g�p�\�Ƃ���)
                if (FieldMng.nowMode != FieldMng.MODE.FORCEDBUTTLE)
                {
                    var tmp = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
                    tmp.CallDeleteEnemy();
                    FieldMng.nowMode = FieldMng.MODE.SEARCH;
                    SceneMng.charMap_[SceneMng.CHARACTERNUM.UNI].gameObject.transform.position = tmp.GetFieldPos();
                    // �A�C�e����ʂ����
                    GameObject.Find("SceneMng").GetComponent<MenuActive>().IsOpenItemMng(false);
                    Debug.Log("Uni�͓����o����");
                }
                else
                {
                    return (false, tmpFlg);
                }
                break;
            case 27:    // HP�|�[�V����(��)
                hpmpNum_ = (65, 0);
                tmpFlg = true;
                break;
            case 28:    // MP�|�[�V����(��)
                hpmpNum_ = (0, 35);
                tmpFlg = true;
                break;
            case 29:    // �Èŏ���
                condition_ = CharaBase.CONDITION.DARK;
                tmpFlg = true;
                break;
            case 30:    // ��჏���
                condition_ = CharaBase.CONDITION.PARALYSIS;
                tmpFlg = true;
                break;
            case 31:    // �S�̒��_���[�W
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(45);
                break;
            case 32:    // �G���J�E���g���ቺ
                //@ ���݂̃G���J�E���g�F�̂܂܁A��莞�ԃG���J�E���g���Ȃ��Ȃ�悤�ɂ���
                FieldMng.stopEncountTimeFlg = true;
                break;
            case 33:    // �h��(�ő�HP�̔���)
                alive = "half";
                tmpFlg = true;
                break;
            case 34:    // HP�|�[�V����(��)
                hpmpNum_ = (130, 0);
                tmpFlg = true;
                break;
            case 35:    // MP�|�[�V����(��)
                hpmpNum_ = (0, 65);
                tmpFlg = true;
                break;
            case 36:    // �����g����
                // �����Ă��邾���Ō��ʂ��ł邩��A�����ɏ����͏����Ȃ�
                break;
            case 37:    // �h��(HP�S��)
                alive = "all";
                tmpFlg = true;
                break;
        }

        itemNum_ = itemNum;
        return (true,tmpFlg);
    }

    public void OnClickCharaButton(int num)
    {
        if(!SceneMng.charasList_[num].GetDeathFlg())
        {
            // HPMP��
            SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + hpmpNum_.Item1);
            SceneMng.charasList_[num].SetMP(SceneMng.charasList_[num].MP() + hpmpNum_.Item2);

            // ��Ԉُ��(-1�����Ƃ���0���傫���Ƃ��͏�Ԉُ�񕜂���)
            if ((int)condition_ - 1 > 0)
            {
                SceneMng.charasList_[num].ConditionReset(false, (int)condition_ - 1);
                condition_ = CharaBase.CONDITION.NON;
            }

            // �o�t����(�Œ�Œ��З�)
            // �o�t�̃A�C�R������
            System.Action<int, int, int> action = (int charaNum, int buffnum, int whatBuff) => {
                var bufftra = GameObject.Find("ButtleUICanvas/" + SceneMng.charasList_[charaNum].Name() + "CharaData/BuffImages").transform;
                for (int i = 0; i < bufftra.childCount; i++)
                {
                    if (bufftra.GetChild(i).GetComponent<Image>().sprite == null)
                    {
                        // �A�C�R���������
                        bufftra.GetChild(i).GetComponent<Image>().sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.BUFFICON][whatBuff - 1];
                        // ���ŃA�b�v�{���������
                        // ��*1 = �o�t��1% ~30%,��*2 = �o�t��31% ~70%,��*3 = �o�t��71%~100%
                        for (int m = 0; m < bufftra.GetChild(i).childCount; m++)
                        {
                            if (m <= buffnum)    // buffnum�̐����ȉ��Ȃ�true�ɂ��ėǂ�
                            {
                                bufftra.GetChild(i).GetChild(m).gameObject.SetActive(true);
                            }
                            else
                            {
                                bufftra.GetChild(i).GetChild(m).gameObject.SetActive(false);
                            }
                        }

                        break;
                    }
                }
            };

            // �����U���Ɩ��@�U���̈З͂������ɏオ�鎖���l���āApair�ɂ��Ă���
            if (buff_.Item1 > 0)
            {
                if (buffExItemFlg_)
                {
                    // �听���A�C�e���g�p�Ō��ʗʑ���
                    var tmp = SceneMng.charasList_[num].SetBuff(2, buff_.Item1);
                    action(num, tmp.Item1, buff_.Item1);
                }
                else
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item1);
                    action(num, tmp.Item1, buff_.Item1);
                }
            }
            if (buff_.Item2 > 0)
            {
                if (buffExItemFlg_)
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(2, buff_.Item2);
                    action(num, tmp.Item1, buff_.Item2);
                }
                else
                {
                    var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item2);
                    action(num, tmp.Item1, buff_.Item2);
                }
            }
        }
        else
        {
            // �h������
            if (alive != "")
            {
                if (alive == "half")
                {
                    // ���񕜏�Ԃŗ����オ�点��
                    SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + (SceneMng.charasList_[num].MaxHP() / 2));
                    SceneMng.charasList_[num].SetDeathFlg(false);   // ���S��Ԃ�����
                }
                else
                {
                    // �S�񕜏�Ԃŗ����オ�点��
                    SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + SceneMng.charasList_[num].MaxHP());
                    SceneMng.charasList_[num].SetDeathFlg(false);   // ���S��Ԃ�����
                }
            }
        }

        alive = ""; // �����񏉊���

        if (charasText_ == null)
        {
            charasText_ = GameObject.Find("ItemBagMng/CharasText").gameObject;
        }

        Text[] text = new Text[2];
        for (int i = 0; i < charasText_.transform.childCount; i++)
        {
            text[i] = charasText_.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }
        TextInit(text);

        Bag_Item.itemState[itemNum_].haveCnt--;
        Bag_Item.itemUseFlg = true;
        // �\�����̏��������X�V
        Bag_Item.itemState[itemNum_].cntText.text = Bag_Item.itemState[itemNum_].haveCnt.ToString();
        if (Bag_Item.itemState[itemNum_].haveCnt < 1)
        {
            // ��������0�ɂȂ������\���ɂ���
            Bag_Item.itemState[itemNum_].box.SetActive(false);
        }
    }
}