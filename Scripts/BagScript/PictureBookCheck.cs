using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureBookCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject pictureUI;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    //private Text pageName_;
    //private string[] nameString_ = new string[(int)ItemGet.items.MAX]{
    //    "Field_0","Field_0","Field_0","Field_0","Field_0" };

    private GameObject[] activePicture_;
    public static Image[] instanceImages_ = new Image[30];
    private float[] posX_;

    void Start()
    {
        activePicture_ = new GameObject[(int)ItemGet.items.MAX];
        posX_ = new float[(int)ItemGet.items.MAX] {
        -400.0f,-200.0f,0.0f,200.0f,400.0f };
       //gameObject.SetActive(false);

    }

    public void GetMateriakinds(int fieldNum, int itemNum)
    {
        activePicture_[itemNum] = Instantiate(pictureUI,
                        new Vector2(0, 0), 
                        Quaternion.identity,
                        transform.Find("MateriaPicture").GetComponent<RectTransform>());
        // �\���ʒu�����炷
        activePicture_[itemNum].transform.localPosition = new Vector2(posX_[itemNum], 0.0f);
        Debug.Log("�t�B�[���h�ԍ��F" + fieldNum + "     �A�C�e���ԍ��F" + itemNum);
        // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        instanceImages_[itemNum] = activePicture_[itemNum].GetComponent<Image>();
        instanceImages_[itemNum].sprite = ItemImageMng.materialIcon_[fieldNum, itemNum];


        // var prefab = Instantiate(materiaUIPrefab);
        // �N�G�X�g�ԍ��̐ݒ�
        // activePicture_[materiaNum_].GetComponent<OwnedMateria>().SetMyName(materiaName);


    }
}