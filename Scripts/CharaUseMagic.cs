using UnityEngine;

// �L�����N�^�[���g�p���閂�@�̏��������Ǘ�����N���X
public class CharaUseMagic : MonoBehaviour
{
    private int mp_;
    private string magicPrefabNum_;    // 10�̌�->���ʔ͈�(�w�b�h),1�̌�->�G�������g

    public void CheckUseMagic(Bag_Magic.MagicData magicData)
    {
        // �U���n���񕜌n���ŕ�����
        if(magicData.element == 0)
        {
            // �񕜌n
            GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
            GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(false,false);
            GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);

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
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetActive(true);
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                case 1:     // ������
                            // �S�Ă̓G�I���}�[�N��\����(���������ۂ̍U���͒N�ɉ��񂠂��邩�s��)
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(true,true);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                case 2:     // �S��
                            // �S�Ă̓G�I���}�[�N��\����
                    GameObject.Find("ButtleUICanvas/Command/Image").GetComponent<ImageRotate>().SetRotaFlg(false);
                    GameObject.Find("ButtleUICanvas/EnemySelectObj").GetComponent<EnemySelect>().SetAllActive(true,false);
                    GameObject.Find("ButtleUICanvas/SetMagicObj").gameObject.SetActive(false);
                    break;
                default:
                    Debug.Log("�s���ȃw�b�h���[�h�ł�");
                    break;
            }

            // �U���З͂�ButtleMng.cs�ɓn��
            GameObject.Find("ButtleMng").GetComponent<ButtleMng>().SetDamageNum(magicData.power);
            // �U��������ButtleMng.cs�ɓn��
            GameObject.Find("ButtleMng").GetComponent<ButtleMng>().SetElement(magicData.element);
        }

        mp_ = magicData.rate;
        magicPrefabNum_ = magicData.element.ToString() + "-" + magicData.tail.ToString();
    }

    public int MPdecrease()
    {
        return mp_; // ����MP���
    }

    public void MagicEffect(Vector3 charaPos,Vector3 enePos,int num)
    {
        // �ʏ�U���e�̕����̌v�Z
        var dir = (enePos - charaPos).normalized;
        // �G�t�F�N�g�̔����ʒu��������
        var adjustPos = new Vector3(charaPos.x, charaPos.y + 0.5f, charaPos.z);

        // �ʏ�U���e�v���n�u���C���X�^���X��(���݂͎w�肵�����@�̂�)
        GameObject obj = Resources.Load("MagicPrefabs/" + magicPrefabNum_) as GameObject;
        var uniAttackInstance = Instantiate(obj, adjustPos, Quaternion.identity);
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
                weaponTagObj[i].GetComponent<CheckAttackHit>().SetTargetNum(num + 1);
            }
        }

    }
}
