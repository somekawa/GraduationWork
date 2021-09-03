using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelect : MonoBehaviour
{
    private List<Vector3> posList_ = new List<Vector3>();
    private int selectNum_ = 0;
    private readonly float posOffset_Y = 1.5f;

    void Start()
    {
        
    }

    void Update()
    {
        if(posList_.Count <= 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            // ���̓G�ɖ�󂪓���
            if (selectNum_ < posList_.Count - 1)
            {
                selectNum_++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            // ��O�̓G�ɖ�󂪓���
            if (selectNum_ > 0)
            {
                selectNum_--;
            }
        }
        else
        {
            // �����������s��Ȃ�
        }

        Debug.Log("�I�𒆂̓G" + selectNum_);

        // ���W�ړ�
        this.gameObject.transform.position = posList_[selectNum_];
    }

    // CharacterMng.cs����퓬���̓G�̏o���ʒu���󂯎��
    public void SetPosList(List<Vector3> list)
    {
        posList_ = list;

        // �󂯎�����l��Y���W��offset���K�v
        for (int i = 0; i < posList_.Count; i++)
        {
            // ���̂܂�list�����������悤�Ƃ���ƃG���[���ł�̂ŁA���L�̂悤�ɂ��ĕύX����
            Vector3 tmpData = posList_[i];
            tmpData.y += posOffset_Y;
            posList_[i] = tmpData;
        }

        // ���̏����ʒu
        this.gameObject.transform.position = posList_[0];

        // �퓬��ʂɑJ�ڎ��A�����ɃA�C�R�����\�����ꂽ�獢�邽�ߔ�\���ɂ���
        this.gameObject.SetActive(false);
    }

    // CharacterMng.cs���ŕ\��/��\����ύX�ł���悤�ɂ���
    public void SetActive(bool flag)
    {
        this.gameObject.SetActive(flag);
    }

    // CharacterMng.cs���ɖڕW���W��n��
    public Vector3 GetSelectEnemyPos()
    {
        // offset�l�����ɖ߂��Ă���n��
        Vector3 tmppos = this.gameObject.transform.position;
        tmppos.y -= posOffset_Y;
        return tmppos;
    }

    // CharacterMng.cs���ɑI�����ꂽ�ԍ���n��
    public int GetSelectNum()
    {
        return selectNum_;
    }
}
