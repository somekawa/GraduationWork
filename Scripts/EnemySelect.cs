using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// �G�̓���Ɍ������A�C�R���𑀍삷��X�N���v�g

public class EnemySelect : MonoBehaviour
{
    // Item1:���W���̃��X�g,Item2:�G�̐����ؑ֗p�̃��X�g(Destroy���ꂽ�G�̍��W���X�g��false�ɂ���)
    private System.Tuple<List<Vector3>, List<bool>> posList_;
    private List<GameObject> targetImageObjList_ = new List<GameObject>();   // �G�w��}�[�N�����X�g�ɕۑ�����

    private int selectNum_ = 0;
    private readonly float posOffset_Y = 1.5f;

    enum SelectKey
    {
        NON,
        UP,
        DOWN,
        MAX
    }

    private SelectKey selectKey_ = SelectKey.MAX;

    void Update()
    {
        if (posList_.Item1.Count <= 0 || selectKey_ == SelectKey.MAX)
        {
            return;
        }

        // ��ɉ�]�����Ă�����
        targetImageObjList_[selectNum_].transform.Rotate(0.0f, 0.0f, 120.0f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.J))
        {
            selectKey_ = SelectKey.UP;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            selectKey_ = SelectKey.DOWN;
        }
        else
        {
            selectKey_ = SelectKey.NON;
        }

        MoveSelectKey(selectKey_);
    }

    // false�ɂȂ��Ă���posList����������A�C�R���ݒu�ʒu���΂�����
    void MoveSelectKey(SelectKey key)
    {
        if(key == SelectKey.NON)
        {
            return;
        }

        if(key == SelectKey.UP)
        {
            // break�Ŕ������Ƃ���true�ɂȂ�
            // while���𔲂�������Ŕ���������false�̂܂�
            bool tmpFlg = false;

            // ���W������l�����������while���𑱍s����
            while (selectNum_ < posList_.Item1.Count - 1)
            {
                selectNum_++;

                // �G�����݂����while���𔲂���
                if (posList_.Item2[selectNum_])
                {
                    tmpFlg = true;
                    break;
                }
            }

            // ��O�̓G��false�ɂȂ��Ă��鎞�ɕK�v�ȏ���
            if (!tmpFlg)
            {
                int num = posList_.Item1.Count - 1;
                // ���g���t�ɂ���(0�`4�Ȃ�A4�`0�ɂȂ�)
                posList_.Item2.Reverse();

                foreach (bool flag in posList_.Item2)
                {
                    if (flag)    // �ő�l���珇�Ԃɒ��ׂĂ����āAtrue�ɂȂ��Ă���Ƃ���Ŏ~�܂�
                    {
                        // true�̓G�Ŗ��̈ړ����X�g�b�v����悤�ɁA�l��������K�v������
                        selectNum_ = num;
                        break;
                    }
                    num--;
                }

                // foreach�ׂ̈ɋt���ɂ��Ă����̂����ɖ߂�
                posList_.Item2.Reverse();
            }

        }
        else if(key == SelectKey.DOWN)
        {
            // break�Ŕ������Ƃ���true�ɂȂ�
            // while���𔲂�������Ŕ���������false�̂܂�
            bool tmpFlg = false;

            // 0���l���傫�����while���𑱍s����
            while (selectNum_ > 0)
            {
                selectNum_--;

                // �G�����݂����while���𔲂���
                if (posList_.Item2[selectNum_])
                {
                    tmpFlg = true;
                    break;
                }
            }

            // ���̓G��false�ɂȂ��Ă��鎞�ɕK�v�ȏ���
            if(!tmpFlg)
            {
                ResetSelectPoint();
            }
        }
        else
        {
            // �����������s��Ȃ�
        }

        // TargetImage��S��false�ɂ���
        for (int i = 0; i < this.transform.childCount; i++)
        {
            targetImageObjList_[i].SetActive(false);
        }
        // �I�𒆂̓G��TargetImage��true�ɂ���
        targetImageObjList_[selectNum_].SetActive(true);
    }

    // CharacterMng.cs����퓬���̓G�̏o���ʒu���󂯎��
    public void SetPosList(List<Vector3> list)
    {
        // �󂯎����list�̐����Atrue�ɂ���bool�̃��X�g��p�ӂ���
        var tmpList = new List<bool>();
        for(int i = 0; i < list.Count(); i++)
        {
            tmpList.Add(true);
        }

        // �������
        posList_ = new System.Tuple<List<Vector3>, List<bool>>(list, tmpList);

        // �󂯎�����l��Y���W��offset���K�v
        for (int i = 0; i < posList_.Item1.Count; i++)
        {
            // ���̂܂�list�����������悤�Ƃ���ƃG���[���ł�̂ŁA���L�̂悤�ɂ��ĕύX����
            Vector3 tmpData = posList_.Item1[i];
            tmpData.y += posOffset_Y;
            posList_.Item1[i] = tmpData;
        }

        // ���̏����ʒu
        selectNum_ = 0;
        //this.gameObject.transform.position = posList_.Item1[selectNum_];

        // �퓬��ʂɑJ�ڎ��A�����ɃA�C�R�����\�����ꂽ�獢�邽�ߔ�\���ɂ���
        //this.gameObject.SetActive(false);

        // ���g������������
        targetImageObjList_.Clear();

        // TargetImage��S��false�ɂ���
        // ���X�g�ɓo�^����
        for (int i = 0; i < this.transform.childCount; i++)
        {
            targetImageObjList_.Add(this.transform.GetChild(i).transform.Find("TargetImage").gameObject);
            targetImageObjList_[i].SetActive(false);
        }
    }

    // CharacterMng.cs���ŕ\��/��\����ύX�ł���悤�ɂ���
    public void SetActive(bool flag)
    {
        if(!flag)
        {
            selectKey_ = SelectKey.MAX;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                targetImageObjList_[i].SetActive(flag);
            }
        }
        else
        {
            selectKey_ = SelectKey.NON;
            targetImageObjList_[selectNum_].SetActive(flag);
        }
    }

    // CharacterMng.cs���ɖڕW���W��n��
    public Vector3 GetSelectEnemyPos()
    {
        // offset�l�����ɖ߂��Ă���n��
        //Vector3 tmppos = this.gameObject.transform.position;
        //tmppos.y -= posOffset_Y;
        //return tmppos;

        return posList_.Item1[selectNum_];
    }

    // CharacterMng.cs���ɑI�����ꂽ�ԍ���n��
    public int GetSelectNum()
    {
        // return�����selectNum_��MagicMove.cs��Destroy�����̂ŁA�Y������list��false�ɂ���
        //posList_.Item2[selectNum_] = false;
        return selectNum_;
    }

    // CharacterMng.cs������GetSelectNum�֐��̌�ɌĂяo���Ă��炢�A���ʒu���Đݒ肷��
    // ���̏������Ȃ��ƁA���̃L�����̍s������Destroy�����G�̓��ォ���󂪃X�^�[�g���Ă��܂��B
    public bool ResetSelectPoint()
    {
        // HP��0�̓G��false�ɂ���
        var tmp = EnemyInstanceMng.enemyList_;
        for (int i = 0; i < tmp.Count(); i++)
        {
            if (tmp[i].Item1.HP() <= 0 && posList_.Item2[i])
            {
                posList_.Item2[i] = false;
            }
        }

        // 0���珇�ɒ��ׂāAtrue�̂Ƃ���Ŏ~�܂�
        int num = 0;
        foreach (bool flag in posList_.Item2)
        {
            if (flag)    // 0���珇�Ԃɒ��ׂĂ����āAtrue�ɂȂ��Ă���Ƃ���Ŏ~�܂�
            {
                // true�̓G�Ŗ��̈ړ����X�g�b�v����悤�ɁA�l��������K�v������
                selectNum_ = num;
                return true;
            }
            num++;
        }

        // �����܂œ��B������(=break�ň���������Ȃ�������A�퓬���[�h�I���̍��}�ɏo������)
        return false;
    }

    // CharacterMng.cs������Ăяo���āAT�L�[��������Ă�����R�}���h�I���ɖ߂��悤�ɂ���
    public bool ReturnSelectCommand()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SetActive(false);
            return false;
        }
        return true;
    }
}
