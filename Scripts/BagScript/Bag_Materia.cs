using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    private InitPopList popMateriaList_;

    [SerializeField]
    private RectTransform materiaParent;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    public struct materia
    {
        public GameObject box;  // �f�ނ̃I�u�W�F�N�g
        public Image image;     // �f�ނ̉摜
        public Text cntText;    // �����Ă���f�ނ̌���\��
        public int haveCnt;     // �w��f�ނ̎����Ă��
        public string name;     // �f�ނ̖��O
        public bool getFlag;    // 1�ȏ㎝���Ă��邩
    }
    public static materia[] materiaState;

    // ��Script�Ŏw�肷�郏�[�h�͔ԍ����擾���Ă���
    public static int emptyMateriaNum;// ��̃}�e���A�̔ԍ�

    // �v���n�u���琶�����ꂽ�I�u�W�F�N�g����
    private int maxCnt_ = 0;            // ���ׂĂ̑f�ސ�

    private int maxHaveCnt_ = 99;// �w��f�ނ̏��������

    public void Init()
    {
        popMateriaList_ = GameObject.Find("SceneMng").GetComponent<InitPopList>();

        if (maxCnt_ == 0)
        {
            maxCnt_ = popMateriaList_.SetMaxMateriaCount();
            materiaState = new materia[maxCnt_];
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i] = new materia
                {
                    box = InitPopList.materiaData[i].box,
                    getFlag=false,
                    haveCnt = 0,
                };
                materiaState[i].name = InitPopList.materiaData[i].name;

                materiaState[i].box.transform.SetParent(materiaParent.transform);
                materiaState[i].box.name = materiaState[i].name;

                // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
                materiaState[i].image= materiaState[i].box.transform.Find("MateriaIcon").GetComponent<Image>();
                materiaState[i].image.sprite = ItemImageMng.spriteMap[ItemImageMng.IMAGE.MATERIA][i];

                // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
                materiaState[i].cntText = materiaState[i].box.transform.Find("MateriaNum").GetComponent<Text>();
                materiaState[i].cntText.text = materiaState[i].name;

                if(materiaState[i].name == "��̃}�e���A")
                {
                    emptyMateriaNum = i;
                }

                materiaState[i].box.SetActive(false);// ���ׂĔ�\���ɂ���
            }
        }
        if (materiaState[0].box.transform.parent != materiaParent.transform)
        {
            for (int i = 0; i < maxCnt_; i++)
            {
                materiaState[i].box.transform.SetParent(materiaParent.transform);
            }
        }


        // �f�o�b�O�p �S���̑f�ނ�5�擾������ԂŎn�܂�
        for (int i = 0; i < maxCnt_; i++)
        {
            MateriaGetCheck(i, materiaState[i].name, 5);
           // Debug.Log(i + "�Ԗڂ̑f��" + materiaBox_[i].name);
        }

    }

    public void MateriaGetCheck(int itemNum, string materiaName, int getCnt)
    {
        materiaState[itemNum].getFlag = true;

        // Debug.Log("     �A�C�e���ԍ��F" + itemNum + "     �A�C�e�����F" + materiaName);
        if (maxHaveCnt_ <= materiaState[itemNum].haveCnt)
        {
            // �ő及������99��
            materiaState[itemNum].haveCnt = maxHaveCnt_;
        }
        else
        {
            // �Ă΂ꂽ�f�ޔԍ��̑f�ނ̏����������Z
            materiaState[itemNum].haveCnt += getCnt;
        }

        if (materiaState[itemNum].haveCnt < 1)
        {
            // ��������1�ȉ��Ȃ��\����
            materiaState[itemNum].box.SetActive(false);
        }
        else
        {
            // 1�ł������Ă�����\������
            materiaState[itemNum].box.SetActive(true);
        }
        materiaState[itemNum].cntText.text = materiaState[itemNum].haveCnt.ToString();
    }

    public int GetMaxHaveMateriaCnt()
    {
        return materiaParent.transform.childCount;
    }
}