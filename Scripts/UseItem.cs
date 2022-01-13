using UnityEngine;
using UnityEngine.UI;

public class UseItem : MonoBehaviour
{
    // �t�B�[���h�̃��j���[��ʂ���̂ݎg�p�\:0
    // �퓬���̂ݎg�p�\    :1
    // �ǂ���ł��g�p�\    :2
    private byte[] item_ = new byte[22]{
            2,//0:HP�|�[�V����(��)
            1,//1:�ŏ���
            1,//2:�U���A�C�e��(���S��)
            0,//3:�J�G���[��
            1,//4:�j�Q���[��
            1,//5:����/���@�U���̓A�b�v(�P��)
            1,//6:�h��̓A�b�v(�P��)
            1,//7:���x�A�b�v(�P��)
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
            1,//18:����/���@�U���̓A�b�v(�S��)
            1,//19:�h��̓A�b�v(�S��)
            1,//20:���x�A�b�v(�S��)
            1 //21:�h��(HP�S��)
    };

    private (int, int) hpmpNum_ = (0,0);
    private CharaBase.CONDITION condition_ = CharaBase.CONDITION.NON;
    private int itemNum_ = -1;
    private (int,int) buff_ = (-1,-1);
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
            case 4:    // �j�Q���[��(�����퓬�łȂ��ꍇ�͎g�p�\�Ƃ���)
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
            case 5:    // ����/���@�U���̓A�b�v(�P��)
                tmpFlg = true;
                buff_ = (1, 2);
                break;
            case 6:    // �h��̓A�b�v(�P��)
                tmpFlg = true;
                buff_ = (3, -1);
                break;
            case 7:    // ���x�A�b�v(�P��)
                tmpFlg = true;
                buff_ = (4, -1);
                break;
            case 8:    // HP�|�[�V����(��)
                hpmpNum_ = (50, 0);
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
                GameObject.Find("ButtleMng").GetComponent<ButtleMng>().ItemDamage(40);
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
        }

        itemNum_ = itemNum;
        return (true,tmpFlg);
    }

    public void OnClickCharaButton(int num)
    {
        // HPMP��
        SceneMng.charasList_[num].SetHP(SceneMng.charasList_[num].HP() + hpmpNum_.Item1);
        SceneMng.charasList_[num].SetMP(SceneMng.charasList_[num].MP() + hpmpNum_.Item2);

        // ��Ԉُ��(-1�����Ƃ���0���傫���Ƃ��͏�Ԉُ�񕜂���)
        if((int)condition_ - 1 > 0)
        {
            SceneMng.charasList_[num].ConditionReset(false, (int)condition_ - 1);
            condition_ = CharaBase.CONDITION.NON;
        }

        // �o�t����(�Œ�Œ��З�)
        // �o�t�̃A�C�R������
        System.Action<int, int,int> action = (int charaNum, int buffnum,int whatBuff) => {
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
            var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item1);
            action(num, tmp.Item1, buff_.Item1);
        }
        if (buff_.Item2 > 0)
        {
            var tmp = SceneMng.charasList_[num].SetBuff(1, buff_.Item2);
            action(num, tmp.Item1, buff_.Item2);
        }

        // �h������
        if(alive != "")
        {
            if(alive == "half")
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
            alive = ""; // �����񏉊���
        }

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

        //hpmpNum_ = (0, 0);  // ������

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

//<����>
//0:HP�|�[�V����(��)
//1:�ŏ���
//2:�U���A�C�e��(���S��)
//3:�J�G���[��
//4:�j�Q���[��
//5:����/���@�U���̓A�b�v(�P��)
//6:�h��̓A�b�v(�P��)
//7:���x�A�b�v(�P��)
//<����>
//8:HP�|�[�V����(��)
//9:MP�|�[�V����(��)
//10:�Èŏ���
//11:��჏���
//12:�U���A�C�e��(���S��)
//13:�G���J�E���g��(�ቺ)
//14:�h��(HP����Ԃ�)
//<�I��>
//15:HP�|�[�V����(��)
//16:MP�|�[�V����(��)
//17:�����g����
//18:�h��(HP�S��)