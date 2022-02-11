using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L�����N�^�[���g�p���閂�@�̏��������Ǘ�����N���X
public class CharaUseMagic : MonoBehaviour
{
    struct MagicAttackInfo
    {
        public Vector3 charaPos;    // �L�����N�^�[���W
        public Vector3 enePos;      // �G���W
        public int targetNum;       // ���@�Ώ۔ԍ�
        public int instanceNum;     // ���@�����ԍ�
        public float instanceTime;  // ���@�����\�莞��
    }

    // ���@�����p���X�g(bool->���@����������true�ɂ���)
    private List<(bool,MagicAttackInfo)> list_ = new List<(bool,MagicAttackInfo)>();

    private string magicPrefabNum_;    
    private int headNum_;              
    private int elementNum_;
    private int tailNum_;
    private int sub1Num_;
    private int healPower_;            // �񕜖��@�̈З�

    private ButtleMng buttleMng_;
    private EnemySelect enemySelect_;
    private CharacterMng characterMng_;

    public void Init()
    {
        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
        enemySelect_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>();
        characterMng_ = GameObject.Find("CharacterMng").GetComponent<CharacterMng>();
    }

    public void CheckUseMagic(Bag_Magic.MagicData magicData, int charaMagicPower)
    {
        // ���X�g�̏�����
        list_.Clear();

        // �ŏ��ɑ�����Ă����Ȃ��ƁAswitch����break�Ŕ����đ���������ǂ��t���Ȃ��Ȃ�
        headNum_ = magicData.head;
        elementNum_ = magicData.element;
        tailNum_ = magicData.tail;
        sub1Num_ = magicData.sub1;

        // �U���n���񕜌n���ŕ�����
        if (magicData.element == 0)
        {
            // �񕜌n
            enemySelect_.SetAllActive(false, false);
   
            // �w�b�h���[�h�̎�ޕ���
            switch (magicData.head)
            {
                case 0:     // �P��
                    characterMng_.SetCharaArrowActive(false,false,magicData.sub2, magicData.sub3);  
                    break;
                case 1:     // ������
                    characterMng_.SetCharaArrowActive(true,true, magicData.sub2, magicData.sub3);
                    break;
                case 2:     // �S��
                    characterMng_.SetCharaArrowActive(true,false, magicData.sub2, magicData.sub3);
                    break;
                default:
                    Debug.Log("�s���ȃw�b�h���[�h�ł�");
                    break;
            }

            // �L�����̖��͈ˑ��̉񕜒l��ۑ����Ă���
            healPower_ = magicData.power + (charaMagicPower / 2);

            if(magicData.sub2 == 0)
            {
                // [�G�������g-�З�-�����񕜂��邩(sub2��0������ȊO���Ŕ��f��������)]
                magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString() + "-" + magicData.sub2.ToString();
            }
            else
            {
                magicPrefabNum_ = "0-" + magicData.tail.ToString() + "-9";
            }
        }
        else if(magicData.element == 1)
        {
            // �⏕�n

            // �G�ւ̃f�o�t
            if(magicData.sub1 == 1)
            {
                // �w�b�h���[�h�̎�ޕ���
                switch (magicData.head)
                {
                    case 0:     // �P��
                        enemySelect_.SetActive(true);
                        break;
                    case 1:     // ������
                        enemySelect_.SetAllActive(true, true);
                        break;
                    case 2:     // �S��
                        enemySelect_.SetAllActive(true, false);
                        break;
                    default:
                        Debug.Log("�s���ȃw�b�h���[�h�ł�");
                        break;
                }

                magicPrefabNum_ = magicData.tail + "-" + magicData.sub1.ToString() + "-" + magicData.sub2.ToString() +"-" + magicData.sub3.ToString();
            }
            else if(magicData.sub1 == 0)
            {
                // �����ւ̃o�t
                // �w�b�h���[�h�̎�ޕ���
                switch (magicData.head)
                {
                    case 0:     // �P��
                        characterMng_.SetCharaArrowActive(false, false, magicData.sub2, magicData.sub3);
                        break;
                    case 1:     // ������
                        characterMng_.SetCharaArrowActive(true, true, magicData.sub2, magicData.sub3);
                        break;
                    case 2:     // �S��
                        characterMng_.SetCharaArrowActive(true, false, magicData.sub2, magicData.sub3);
                        break;
                    default:
                        Debug.Log("�s���ȃw�b�h���[�h�ł�");
                        break;
                }

                magicPrefabNum_ = magicData.tail + "-" + magicData.sub1.ToString() + "-" + magicData.sub2.ToString() + "-" + magicData.sub3.ToString();
            }
            else
            {
                // �����������s��Ȃ�
            }
        }
        else
        {
            // �U���n
            // �w�b�h���[�h�̎�ޕ���
            switch (magicData.head)
            {
                case 0:     // �P��
                            // �G�I��������
                    enemySelect_.SetActive(true);
                    break;
                case 1:     // ������
                            // �S�Ă̓G�I���}�[�N��\����(���������ۂ̍U���͒N�ɉ��񂠂��邩�s��)
                    enemySelect_.SetAllActive(true,true);
                    break;
                case 2:     // �S��
                            // �S�Ă̓G�I���}�[�N��\����
                    enemySelect_.SetAllActive(true, false);
                    break;
                default:
                    Debug.Log("�s���ȃw�b�h���[�h�ł�");
                    break;
            }

            // �U���З͂�ButtleMng.cs�ɓn��
            buttleMng_.SetDamageNum(magicData.power + charaMagicPower);
            // �U��������ButtleMng.cs�ɓn��
            buttleMng_.SetElement(magicData.element);
            // ��Ԉُ��ButtleMng.cs�ɓn��
            buttleMng_.SetBadStatus(magicData.sub1, magicData.sub2);

            if(magicData.sub3 == 4 || magicData.sub2 == 9)
            {
                // �K�����ʂ���
                buttleMng_.SetAutoHit(true);
            }

            magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
        }
    }

    public int MPdecrease(Bag_Magic.MagicData magicData)
    {
        return magicData.mp; // ����MP���
    }

    public int GetHealPower()
    {
        return healPower_;
    }

    public int GetTailNum()
    {
        return tailNum_;
    }

    public (int,int) GetElementAndSub1Num()
    {
        return (elementNum_,sub1Num_);
    }

    public void InstanceMagicInfo(Vector3 charaPos,Vector3 enePos,int targetNum,int instanceNum)
    {
        // ���̐ݒ�
        MagicAttackInfo tmpInfo = new MagicAttackInfo();
        tmpInfo.charaPos = charaPos;
        tmpInfo.enePos = enePos;
        tmpInfo.targetNum = targetNum;
        tmpInfo.instanceNum = instanceNum;

        if(headNum_ == 1)   // ������U���̏ꍇ
        {
            tmpInfo.instanceTime = instanceNum * 0.2f;  // 0.2�b���������Ԃ����炷
        }
        else
        {
            tmpInfo.instanceTime = 0.0f;    // �����ɔ�������
        }

        list_.Add((false,tmpInfo));
    }

    // �����̓��e��struct�ɂ��Ĕ������Ԃ܂ŕۑ����������ƁA
    // �������ԂɂȂ����炻��struct�̏����g���Đ���
    public IEnumerator InstanceMagicCoroutine()
    {
        float time = 0.0f;
        bool tmpFlg = false;
        var split = magicPrefabNum_.Split('-');

        // �S�Ă̔����t���O��true�ɂȂ�����while�𔲂���悤�ɂ�����

        while (!tmpFlg)
        {
            yield return null;

            // �t���O�̏�Ԃ́A���true�ɂ��Ă�����for����1�ł��������̖��@�������false�ɂȂ�悤�ɂ���
            tmpFlg = true;
            for(int k = 0; k < list_.Count; k++)
            {
                if(!list_[k].Item1)
                {
                    // �������̖��@����
                    tmpFlg = false;
                    break;
                }
            }

            time += Time.deltaTime;
            //Debug.Log("���Ԍv��" + time);

            for (int t = 0; t < list_.Count; t++)
            {
                if(!list_[t].Item1 && list_[t].Item2.instanceTime <= time)  // �������ԂɂȂ��Ă���
                {
                    // �����m�F�t���O��true�ɂ���(�ꎞ�ϐ��ɓ���čX�V����)
                    (bool, MagicAttackInfo) tmp = (true, list_[t].Item2);
                    list_[t] = tmp;

                    // �G�t�F�N�g�̔����ʒu��������
                    Vector3 adjustPos;

                    // magicPrefabNum_�ϐ����n�C�t���ŕ��������Ƃ���2�ɕ����ꂽ��(=�U�����@)
                    if (split.Length == 2)
                    {
                        // �З͂���ȏ�̍U�����@�Ȃ�(�ɑ�܂�)�A�G�̓���ɃG�t�F�N�g���C���X�^���X������
                        if (int.Parse(split[1]) >= 2)
                        {
                            adjustPos = new Vector3(list_[t].Item2.enePos.x, list_[t].Item2.enePos.y, list_[t].Item2.enePos.z);
                        }
                        else
                        {
                            adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y + 0.5f, list_[t].Item2.charaPos.z);
                        }

                        Common(t, adjustPos,false);
                    }
                    else if(split.Length == 3)
                    {
                        //  3�ɕ�����Ă��邩��񕜖��@
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y, list_[t].Item2.charaPos.z);
                        Common(t, adjustPos, true);
                    }
                    else
                    {
                        // 4�ɕ�����Ă��邩��⏕���@
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y, list_[t].Item2.charaPos.z);

                        // �����p���G�p��sub1�Ō�������
                        Common(t, adjustPos, split[1] == 0.ToString() ? true : false);
                    }
                }
            }
        }
    }

    private void Common(int t,Vector3 adjustPos,bool flag)
    {
        GameObject obj = null;

        var assetBundle_ = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/AssetBundles/StandaloneWindows/magic");

        if (flag)
        {
            var tmpStr = magicPrefabNum_;
            var split = tmpStr.Split('-');
            // ���˂Ƌz���̓G�t�F�N�g�̐����ݒ�
            if (split.Length == 4)
            {
                if (split[3] == 2.ToString())
                {
                    // ���˖��@�̎�(�����G�t�F�N�g���g������Œ�ԍ�)
                    obj = assetBundle_.LoadAsset<GameObject>("0-0-1-2");
                    SceneMng.SetSE(11);
                    //obj = Resources.Load("MagicPrefabs/0-0-1-2") as GameObject;
                }
                else if(split[3] == 3.ToString())
                {
                    // �z�����@�̎�(�����G�t�F�N�g���g������Œ�ԍ�)
                    obj = assetBundle_.LoadAsset<GameObject>("0-0-1-3");
                    SceneMng.SetSE(12);
                    //obj = Resources.Load("MagicPrefabs/0-0-1-3") as GameObject;
                }
                else
                {
                    // ���@�v���n�u���C���X�^���X��
                    obj = assetBundle_.LoadAsset<GameObject>(magicPrefabNum_);
                    //obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
                }
            }
            else
            {
                // ���@�v���n�u���C���X�^���X��
                obj = assetBundle_.LoadAsset<GameObject>(magicPrefabNum_);
                //obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
            }

            // �v���n�u�̍������܂߂��ʒu�ɐ�������
            Instantiate(obj, adjustPos + obj.transform.position, obj.transform.rotation);
        }
        else
        {
            // �e�̕����̌v�Z
            var dir = (list_[t].Item2.enePos - list_[t].Item2.charaPos).normalized;

            // ���@�v���n�u���C���X�^���X��
            var tmpStr = magicPrefabNum_;
            var split = tmpStr.Split('-');

            if (split[1] == 1.ToString() && split.Length == 4)
            {
                // �G�ւ̃f�o�t���@�̎�(�����G�t�F�N�g���g������Œ�ԍ�)
                SceneMng.SetSE(4);
                obj = assetBundle_.LoadAsset<GameObject>("0-1-1-1");
                //obj = Resources.Load("MagicPrefabs/0-1-1-1") as GameObject;
            }
            else
            {
                switch(elementNum_)
                {
                    case 2:
                        SceneMng.SetSE(5);
                        break;
                    case 3:
                        SceneMng.SetSE(6);
                        break;
                    case 4:
                        SceneMng.SetSE(8);
                        break;
                    case 5:
                        SceneMng.SetSE(7);
                        break;
                    default:
                        break;
                }
                obj = assetBundle_.LoadAsset<GameObject>(magicPrefabNum_);
                //obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
            }

            assetBundle_.Unload(false);

            // �v���n�u�̍������܂߂��ʒu�ɐ�������
            var uniAttackInstance = Instantiate(obj, adjustPos + obj.transform.position, obj.transform.rotation);

            MagicMove magicMove = uniAttackInstance.GetComponent<MagicMove>();
            // �ʏ�U���e�̔��ł����������w��
            magicMove.SetDirection(dir);

            // [Weapon]�̃^�O�����Ă���I�u�W�F�N�g��S�Č�������
            var weaponTagObj = GameObject.FindGameObjectsWithTag("Weapon");
            for (int i = 0; i < weaponTagObj.Length; i++)
            {
                // �������I�u�W�F�N�g�̖��O���r���āA����U���Ɉ�������ɂ��Ă���CheckAttackHit�֐��̐ݒ���s��
                if (weaponTagObj[i].name == (magicPrefabNum_ + "(Clone)"))
                {
                    // �I�������G�̔ԍ���n��
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(list_[t].Item2.targetNum + 1,-1);

                    // �����������@�I�u�W�F�N�g����[�G�������g�ԍ�-���ʗʔԍ�(Clone)-���@�����ԍ�]�ɕύX����
                    weaponTagObj[i].name = magicPrefabNum_ + "(Clone)-" + list_[t].Item2.instanceNum;

                    // �G�q�b�g���̍폜�ΏۂƂ��Ė��O������
                    weaponTagObj[i].GetComponent<CheckAttackHit>().SetCharaMagicStr(weaponTagObj[i].name);
                }
            }
        }
    }

    public string MagicInfoMake(Bag_Magic.MagicData magicData)
    {
        string info = "";
        string info2 = "";
        string info3 = "";
        string info4 = "";

        // �U���n���񕜌n���ŕ�����
        if (magicData.element == 0)
        {
            // �񕜌n
            info = "����";
            info3 = "��";
            switch (magicData.sub2)
            {
                case 0:     // HP
                    info2 = "HP";
                    break;
                case 5:     // ��
                    info4 += "(��)";
                    break;
                case 6:     // �È�
                    info4 += "(�È�)";
                    break;
                case 7:     // ���
                    info4 += "(���)";
                    break;
                case 8:     // ����
                    info4 += "(����)";
                    break;
                default:
                    Debug.Log("�s����sub2���[�h�ł�");
                    break;
            }
        }
        else if (magicData.element == 1)
        {
            // �⏕�n
            info3 = "�t�^";

            switch (magicData.sub2)
            {
                case 1:     // �����U����
                    info2 = "�����U����";
                    break;
                case 2:     // ���@�U����
                    info2 = "���@�U����";
                    break;
                case 3:     // �h���
                    info2 = "�h���";
                    break;
                case 4:     // ����/����
                    info2 = "����/����";
                    break;
                default:
                    Debug.Log("�s����sub2���[�h�ł�");
                    break;
            }

            if (magicData.sub1 == 1)
            {
                info = "�G";
                info2 += "�ቺ";
            }
            else if (magicData.sub1 == 0)
            {
                info = "����";
                switch (magicData.sub3)
                {
                    case 0:     // �㏸
                        info2 += "�㏸";
                        break;
                    case 2:     // ����
                        info2 += "����";
                        break;
                    case 3:     // �z��
                        info2 += "�z��";
                        break;
                    default:
                        Debug.Log("�s����sub3���[�h�ł�");
                        break;
                }
            }
            else
            {
                // �����������s��Ȃ�
            }
        }
        else
        {
            // �U���n
            info = "�G";
            info3 = "�U��";

            switch (magicData.element)
            {
                case 2:     // ��
                    info2 = "������";
                    break;
                case 3:     // ��
                    info2 = "������";
                    break;
                case 4:     // �y
                    info2 = "�y����";
                    break;
                case 5:     // ��
                    info2 = "������";
                    break;
                default:
                    Debug.Log("�s����element���[�h�ł�");
                    break;
            }

            switch (magicData.sub1)
            {
                case 2:     // ��
                    info4 = "(��)";
                    break;
                case 3:     // �È�
                    info4 = "(�È�)";
                    break;
                case 4:     // ���
                    info4 = "(���)";
                    break;
                case 5:     // ����
                    info4 = "(����)";
                    break;
                default:
                    Debug.Log("�s����sub1���[�h�ł�");
                    break;
            }

            switch (magicData.sub2)
            {
                case 5:     // ��
                    info4 += "(��)";
                    break;
                case 6:     // �È�
                    info4 += "(�È�)";
                    break;
                case 7:     // ���
                    info4 += "(���)";
                    break;
                case 8:     // ����
                    info4 += "(����)";
                    break;
                default:
                    Debug.Log("�s����sub2���[�h�ł�");
                    break;
            }

            if(magicData.sub3 == 4 || magicData.sub2 == 9)
            {
                info4 += "(�K��)";
            }
        }

        // �w�b�h���[�h�̎�ޕ���
        switch (magicData.head)
        {
            case 0:     // �P��
                info += "�P�̂�";
                break;
            case 1:     // ������
                info += "�����_���������";
                break;
            case 2:     // �S��
                info += "�S�̂�";
                break;
            default:
                Debug.Log("�s���ȃw�b�h���[�h�ł�");
                break;
        }

        // �e�C�����[�h�̎�ޕ���
        switch (magicData.tail)
        {
            case 0:     // ��
                info += "���З͂�";
                break;
            case 1:     // ��
                info += "���З͂�";
                break;
            case 2:     // ��
                info += "��З͂�";
                break;
            case 3:     // �ɑ�
                info += "�ɑ�З͂�";
                break;
            default:
                Debug.Log("�s���ȃw�b�h���[�h�ł�");
                break;
        }

        return info + info2 + info3 + info4;
    }
}
