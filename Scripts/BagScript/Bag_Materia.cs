using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Materia : MonoBehaviour
{
    [SerializeField]
    private GameObject materiaUIPrefab;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    //[SerializeField]
    //// Materia���擾�����ۂɐ��������prefab
    //private GameObject getMateria;
    //[SerializeField]
    //// Materia���擾�����ۂɐ��������prefab
    //private GameObject materiaCanvas;

    // �v���n�u���琶�����ꂽ�I�u�W�F�N�g����
    private static GameObject[] materiaBox_ = new GameObject[30];
    private static Image[] instanceImages_ = new Image[30];
    private static Text[] instanceTexts_ = new Text[30];

    public static int materiaNum_ = 0;// ���Ԗڂ̐����Ȃ̂�
    private static int[] materiaCnt_ = new int[30];// 1�̑f�ނɑ΂���̏�����
    public static string[] saveMateriaName_ = new string[30];    // �E�����f�ނ̖��O��ۑ�
    private static int[] saveMateriaNum_ = new int[30];    // ���Ԗڂ��E�������ۑ�

    //// �\������摜��X���W
    //private static float[] boxPosX_ = new float[5] {
    //    -285.0f,-95.0f,100.0f,290.0f,490.0f
    //};
    //private static float[] boxPosY_ = new float[2] {
    //    150.0f,-50.0f
    //};
    //private static int xCount_ = 0;// X���W�����炷���߂̃J�E���g
    //private static int yCount_ = 0;// Y���W�����炷���߂̃J�E���g

  //  private PictureBookCheck picture_;

    // void Start()
    public void Init()
    {
        //
        //menuActive_ = menuActive;
        // GameObject.Find("DontDestroyCanvas/OtherUI/PictureBookMng").GetComponent<PictureBookCheck>().Init();
       // picture_ = GameObject.Find("SceneMng").GetComponent<PictureBookCheck>();

        //gameObject.SetActive(false);

    }

    public IEnumerator ActiveMateria(ItemBagMng itemBagMng)
    {
        gameObject.SetActive(true);
        Debug.Log("Materia�\�����ł�");
        while (true)
        {
            yield return null;

            //// debug
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    ItemGetCheck(0, 0, "����΂��");
            //}
            //else if (Input.GetKeyDown(KeyCode.S))
            //{
            //    ItemGetCheck(0, 1, "�����΂��");
            //}
            //else if (Input.GetKeyDown(KeyCode.D))
            //{
            //    ItemGetCheck(0, 2, "�ɂ΂��");
            //}
            ////

            if (itemBagMng.GetStringNumber() != (int)ItemBagMng.topic.MATERIA)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    public void MateriaGetCheck(int fieldNum, int itemNum, string materiaName)
    {
        // �f�o�b�O�p
        saveMateriaName_[0] = materiaName;
        saveMateriaNum_[0] = itemNum;
        materiaCnt_[0] = 5;// �����������Z
        materiaBox_[0] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        instanceImages_[0] = materiaBox_[0].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[0].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 5];
        // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        instanceTexts_[0] = materiaBox_[0].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[0].text = materiaCnt_[0].ToString();
        // ���O��ݒ�
        materiaBox_[0].GetComponent<OwnedMateria>().SetMyName(materiaName);

        materiaNum_++;
        
        saveMateriaName_[materiaNum_] = "�|�̋k";
        saveMateriaNum_[materiaNum_] = itemNum;
        materiaCnt_[materiaNum_] = 1;// �����������Z
        materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[materiaNum_].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 0];
        // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();
        // ���O��ݒ�
        materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);

        materiaNum_++;

        saveMateriaName_[materiaNum_] = "�Ԃ̖�";
        saveMateriaNum_[materiaNum_] = itemNum;
        materiaCnt_[materiaNum_] = 1;// �����������Z
        materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
            new Vector2(0, 0), Quaternion.identity,
            GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        instanceImages_[materiaNum_].sprite = ItemImageMng.spriteMap_[ItemImageMng.IMAGE.MATERIA][0, 1];
        // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();
        // ���O��ݒ�
        materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);

        //-----------------------


        //if (saveMateriaName_[0] == null)
        //{

        //    saveMateriaName_[0] = materiaName;
        //    saveMateriaNum_[0] = itemNum;
        //    materiaCnt_[0]=5;// �����������Z

        //    Debug.Log(materiaNum_ + "�ڂ̑f�ނ��E���܂���");
        //    // �摜�𐶐��@(���ɂȂ�prefab�A���W�A��]�A�e)
        //    materiaBox_[0] = Instantiate(materiaUIPrefab,
        //        new Vector2(0, 0), Quaternion.identity, 
        //        GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);


        //    // �\���ʒu�����炷
        //   // materiaBox_[0].transform.localPosition = new Vector2(boxPosX_[0], boxPosY_[0]);

        //    // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        //    instanceImages_[0] = materiaBox_[0].transform.Find("MateriaIcon").GetComponent<Image>();
        //    // instanceImages_[0].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];
        //    instanceImages_[0].sprite = ItemImageMng.materialIcon_[0, 5];

        //    // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        //    instanceTexts_[0] = materiaBox_[0].transform.Find("MateriaNum").GetComponent<Text>();
        //    instanceTexts_[0].text = materiaCnt_[0].ToString();

        //    // ���O��ݒ�
        //    materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
        //   // picture_.GetMateriakinds(fieldNum, itemNum);
        //}
        //else
        //{
        //    if (materiaName == saveMateriaName_[0])
        //    {
        //        materiaCnt_[0]++;
        //        instanceTexts_[0].text = materiaCnt_[0].ToString();
        //    }
        //    else
        //    {
        //        // �擾�����f�ނ��O�ԈȊO��������
        //        bool flag = false;
        //        for (int i = 0; i < 25; i++)
        //        {
        //            if (saveMateriaName_[i] == materiaName)
        //            {
        //                // �������O�̑f�ނ��E�����珊���������Z����
        //                materiaCnt_[i]++;// ���������Z
        //                instanceTexts_[i].text = materiaCnt_[i].ToString();
        //                flag = true;
        //                break;
        //            }
        //        }

        //        if (flag == false)
        //        {
        //            materiaNum_++;  // �A�C�e���̏����ő��ނ��ӂ₷
        //            saveMateriaName_[materiaNum_] = materiaName;
        //            saveMateriaNum_[materiaNum_] = itemNum;

        //            //xCount_++;
        //            //if (xCount_ == 5)
        //            //{
        //            //    xCount_ = 0;
        //            //    yCount_++;
        //            //}
        //            Debug.Log(materiaNum_ + "�ڂ̑f�ނ��E���܂���");
        //            // �摜�𐶐��@(���ɂȂ�prefab�A���W�A��]�A�e)
        //            materiaBox_[materiaNum_] = Instantiate(materiaUIPrefab,
        //                new Vector2(0, 0), Quaternion.identity, 
        //                GameObject.Find("ItemBagMng/MateriaMng/Viewport/Content").transform);
        //            // �\���ʒu�����炷
        //           // materiaBox_[materiaNum_].transform.localPosition = new Vector2(boxPosX_[xCount_], boxPosY_[yCount_]);

        //            // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        //            instanceImages_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaIcon").GetComponent<Image>();
        //            instanceImages_[materiaNum_].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];

        //            // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        //            instanceTexts_[materiaNum_] = materiaBox_[materiaNum_].transform.Find("MateriaNum").GetComponent<Text>();
        //            materiaCnt_[materiaNum_] = 1;   // ������
        //            instanceTexts_[materiaNum_].text = materiaCnt_[materiaNum_].ToString();

        //            //  var prefab = Instantiate(materiaUIPrefab);
        //            // �N�G�X�g�ԍ��̐ݒ�
        //            materiaBox_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);
        //            //picture_.GetMateriakinds(fieldNum, itemNum);
        //        }
        //    }
        //}

        //  Debug.Log("�t�B�[���h�ԍ��F" + fieldNum + "     �A�C�e���ԍ��F" + itemNum + "     �A�C�e�����F" + materiaName);
    }
}