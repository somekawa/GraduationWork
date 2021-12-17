using UnityEngine;

// �o�b�h�X�e�[�^�X�̌��ʂ𔭓������邽�߂̃N���X

public class BadStatusMng : MonoBehaviour
{
    // EnemyInstanceMng.cs��CharacterMng.cs�ŌĂяo��(�U����)
    public void BadStateMoveAfter<T>((CharaBase.CONDITION, bool)[] bs,T obj,HPMPBar hpmpBar,bool isCharacterFlg)
    {
        // for����CONDITION���񂵂�true�ɂȂ��Ă���ꏊ��T��
        // true�̏ꏊ���Ɍ��ʂ𔭓�������

        for(int i = 0; i < (int)CharaBase.CONDITION.DEATH; i++)
        {
            if(bs[i].Item2)
            {
                switch(bs[i].Item1)
                {
                    case CharaBase.CONDITION.NON:
                        break;
                    case CharaBase.CONDITION.POISON:
                        if (!isCharacterFlg) // �G
                        {
                            var enemy = (Enemy)(object)obj;
                            int damage = (int)(enemy.MaxHP() * 0.2f);
                            StartCoroutine(hpmpBar.MoveSlideBar(enemy.HP() - damage));
                            // �������l�̕ύX���s��
                            enemy.SetHP(enemy.HP() - damage);

                            Debug.Log(enemy.Name() + "�́A�ł�HP��" + damage + "���炵�܂�");

                        }
                        else // �L����
                        {
                            var chara = (Chara)(object)obj;
                            int damage = (int)(chara.MaxHP() * 0.2f);
                            StartCoroutine(hpmpBar.MoveSlideBar(chara.HP() - damage));
                            // �������l�̕ύX���s��
                            chara.SetHP(chara.HP() - damage);

                            Debug.Log(chara.Name() + "�́A�ł�HP��" + damage + "���炵�܂�");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // EnemyInstanceMng.cs��CharacterMng.cs�ŌĂяo��(�U���O)
    public (CharaBase.CONDITION,bool) BadStateMoveBefore<T>((CharaBase.CONDITION, bool)[] bs, T obj, HPMPBar hpmpBar, bool isCharacterFlg)
    {
        (CharaBase.CONDITION, bool) tmpConditionCheck = (CharaBase.CONDITION.NON, false);

        for (int i = 0; i < (int)CharaBase.CONDITION.DEATH; i++)
        {
            if (bs[i].Item2)
            {
                switch (bs[i].Item1)
                {
                    case CharaBase.CONDITION.NON:
                        break;
                    case CharaBase.CONDITION.DARK:
                        if (!isCharacterFlg) // �G
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "�́A�Èŏ�ԂȂ̂Ŗ���/����̒l�����炵�܂�");
                            // speed�̒l�����݂̔����ɂ���
                            // �s�����x�ɂ͉e���Ȃ��͂�
                            enemy.SetSpeed(enemy.Speed() / 2);
                        }
                        else // �L����
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "�́A�Èŏ�ԂȂ̂Ŗ���/����̒l�����炵�܂�");
                            chara.SetSpeed(chara.Speed() / 2);
                        }
                        break;
                    case CharaBase.CONDITION.PARALYSIS:
                        int rand = UnityEngine.Random.Range(0, 100);

                        if (!isCharacterFlg) // �G
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "�́A��჏�ԂȂ̂œ����邩�����Ȃ����̔�������܂�");

                            // ��Ⴢœ����Ȃ��Ƃ���(CharaBase.CONDITION.PARALYSIS, true),������Ƃ��͌���bool��false�ɂ���
                            if (30 + enemy.Luck() < rand)   // 3����̊m���Ɏ��g�̍K�^�l�𑫂��Čv�Z
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, true);  // �����Ȃ�
                                Debug.Log(30 + enemy.Luck() + "<" + rand + "�Ȃ̂Ŗ�Ⴢ̌��ʂœ����Ȃ�");
                            }
                            else
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, false);  // ������
                                Debug.Log(30 + enemy.Luck() + ">=" + rand + "�Ȃ̂Ŗ�Ⴢ̌��ʒ������A������");
                            }
                        }
                        else // �L����
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "�́A��჏�ԂȂ̂œ����邩�����Ȃ����̔�������܂�");

                            // ��Ⴢœ����Ȃ��Ƃ���(CharaBase.CONDITION.PARALYSIS, true),������Ƃ��͌���bool��false�ɂ���
                            if (30 + chara.Luck() < rand)   // 3����̊m���Ɏ��g�̍K�^�l�𑫂��Čv�Z
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, true);  // �����Ȃ�
                                Debug.Log(30 + chara.Luck() + "<" + rand + "�Ȃ̂Ŗ�Ⴢ̌��ʂœ����Ȃ�");
                            }
                            else
                            {
                                tmpConditionCheck = (CharaBase.CONDITION.PARALYSIS, false);  // ������
                                Debug.Log(30 + chara.Luck() + ">=" + rand + "�Ȃ̂Ŗ�Ⴢ̌��ʒ������A������");
                            }
                        }
                        break;
                    case CharaBase.CONDITION.DEATH:
                        if (!isCharacterFlg) // �G
                        {
                            var enemy = (Enemy)(object)obj;
                            Debug.Log(enemy.Name() + "�́A������ԂȂ̂Ŏ��S��������܂�");
                            tmpConditionCheck = (CharaBase.CONDITION.DEATH, true); 
                        }
                        else // �L����
                        {
                            var chara = (Chara)(object)obj;
                            Debug.Log(chara.Name() + "�́A������ԂȂ̂Ŏ��S��������܂�");
                            tmpConditionCheck = (CharaBase.CONDITION.DEATH, true);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        return tmpConditionCheck;
    }
}
