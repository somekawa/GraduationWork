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

    private int mp_;
    private string magicPrefabNum_;    // 10�̌�->���ʔ͈�(�w�b�h),1�̌�->�G�������g
    private int headNum_;              // �w�b�h�ԍ�

    private ButtleMng buttleMng_;
    private EnemySelect enemySelect_;

    public void Init()
    {
        buttleMng_ = GameObject.Find("ButtleMng").GetComponent<ButtleMng>();
        enemySelect_ = GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>();
    }

    public void CheckUseMagic(Bag_Magic.MagicData magicData)
    {
        // ���X�g�̏�����
        list_.Clear();

        // �U���n���񕜌n���ŕ�����
        if (magicData.element == 0)
        {
            // �񕜌n
            enemySelect_.SetAllActive(false, false);

            //@ �s����������
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
            buttleMng_.SetDamageNum(magicData.power);
            // �U��������ButtleMng.cs�ɓn��
            buttleMng_.SetElement(magicData.element);
        }

        mp_ = magicData.rate;
        magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
        headNum_ = magicData.head;
    }

    public int MPdecrease()
    {
        return mp_; // ����MP���
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

        // �S�Ă̔����t���O��true�ɂȂ�����while�𔲂���悤�ɂ�����

        while(!tmpFlg)
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

                    // �e�̕����̌v�Z
                    var dir = (list_[t].Item2.enePos - list_[t].Item2.charaPos).normalized;
                    // �G�t�F�N�g�̔����ʒu��������
                    Vector3 adjustPos;
                    
                    // �З͂���̍U�����@�Ȃ�A�G�̓���ɃG�t�F�N�g���C���X�^���X������
                    if(int.Parse(magicPrefabNum_.Split('-')[1]) == 2)
                    {
                        adjustPos = new Vector3(list_[t].Item2.enePos.x, list_[t].Item2.enePos.y, list_[t].Item2.enePos.z);
                    }
                    else
                    {
                        adjustPos = new Vector3(list_[t].Item2.charaPos.x, list_[t].Item2.charaPos.y + 0.5f, list_[t].Item2.charaPos.z);
                    }

                    // ���@�v���n�u���C���X�^���X��(���݂͎w�肵�����@�̂�)
                    GameObject obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;

                    //var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity); // ��]���W���S��0�ɂȂ�ver.
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
                            weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(list_[t].Item2.targetNum + 1);

                            // �����������@�I�u�W�F�N�g����[�G�������g�ԍ�-���ʗʔԍ�(Clone)-���@�����ԍ�]�ɕύX����
                            weaponTagObj[i].name = magicPrefabNum_ + "(Clone)-" + list_[t].Item2.instanceNum;

                            // �G�q�b�g���̍폜�ΏۂƂ��Ė��O������
                            weaponTagObj[i].GetComponent<CheckAttackHit>().SetCharaMagicStr(weaponTagObj[i].name);
                        }
                    }
                }
            }
        }
    }
}
