using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bag_Item : MonoBehaviour
{
    private enum EVENT_ITEM
    {
        NON=-1,
        FREE_MATERIA,
        BEGINNER_RECIPE,
        MAX
    }
    private string[] itemName= new string[30];

    [SerializeField]
    private GameObject itemUIPrefab;
    [SerializeField]
    private RectTransform itemParent_;    // �f�ނ��E�����Ƃ��ɐ��������v���n�u

    private static GameObject[] itemBox_ = new GameObject[30];
    private Image[] instanceImages_ = new Image[30];
    private Text[] instanceTexts_ = new Text[30];
    //private int instanceNum_ = 0;
    private static int itemNum_ = 0;// ���Ԗڂ̐����Ȃ̂�
    private static int[] itemCnt_ = new int[30];// 1�̑f�ނɑ΂���̏�����
    private static string[] saveItemName_ = new string[(int)EVENT_ITEM.MAX];    // �E�����f�ނ̖��O��ۑ�
    private static int[] saveItemNum_ = new int[30];    // ���Ԗڂ��E�������ۑ�
    private static int[] saveChapterNum_ = new int[(int)EVENT_ITEM.MAX];
    private static int chapterCnt_ = 0;
    // �\������摜��X���W
    private static float[] boxPosX_ = new float[5] {
        -285.0f,-95.0f,100.0f,290.0f,490.0f
    };
    private static float[] boxPosY_ = new float[2] {
        150.0f,-50.0f
    };
    private static int xCount_ = 0;// X���W�����炷���߂̃J�E���g
    private static int yCount_ = 0;// Y���W�����炷���߂̃J�E���g

    void Start()
    {
        //menuActive_ = menuActive;

        //  itemBox_ = transform.Find("ItemBox").GetComponent<Image>();
       // gameObject.SetActive(false);

        // StartCoroutine(ActiveItem(menuActive));
    }

    public IEnumerator ActiveItem(ItemBagMng itemBagMng)
    {
        gameObject.SetActive(true);
        Debug.Log("Item�\�����ł�");
        while (true)
        {
            yield return null;
            if (itemBagMng.GetStringNumber() != (int)ItemBagMng.topic.ITEM)
            {
                gameObject.SetActive(false);
                yield break;
            }
        }
    }

    // public void ItemGetCheck(int chapterNum)
    public void ItemGetCheck(string itemName)
    {
        if (saveItemName_[0] == null)
        {
            saveItemName_[0] = itemName;
            //saveItemNum_[0] = itemNum;
            itemCnt_[0]++;// �����������Z

            Debug.Log(saveItemName_[0] + "�����܂���");
            // �摜�𐶐��@(���ɂȂ�prefab�A���W�A��]�A�e)
            itemBox_[0] = Instantiate(itemUIPrefab,
                new Vector2(0, 0), Quaternion.identity,
                itemParent_);

            // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
            instanceImages_[0] = itemBox_[0].transform.Find("ItemIcon").GetComponent<Image>();
            instanceImages_[0].sprite =  ItemImageMng.spriteMap_[ItemImageMng.IMAGE.ITEM][0, 1];

            // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
            instanceTexts_[0] = itemBox_[0].transform.Find("ItemNum").GetComponent<Text>();
            instanceTexts_[0].text = itemCnt_[0].ToString();

            // ���O��ݒ�
            itemBox_[itemNum_].GetComponent<OwnedMateria>().SetMyName(itemName);
            // picture_.GetMateriakinds(fieldNum, itemNum);
        }



        //for (int chapterCnt_ = 0; chapterCnt_ < (int)ItemBagMng.topic.MAX; chapterCnt_++)
        //{
        //    if (saveChapterNum_[chapterCnt_] < EventMng.GetChapterNum())
        //    {
        //        itemName[chapterCnt_] = saveItemName_[chapterCnt_];
        //        //saveItemNum_[0] = itemNum;
        //        itemCnt_[chapterCnt_] = 5;
        //        // �摜�𐶐��@(���ɂȂ�prefab�A���W�A��]�A�e)
        //        itemBox_[chapterCnt_] = Instantiate(itemUIPrefab,
        //            new Vector2(0, 0), Quaternion.identity, this.transform.Find("Viewport/Content"));
        //        // �\���ʒu�����炷
        //        itemBox_[chapterCnt_].transform.localPosition = new Vector2(boxPosX_[chapterCnt_], boxPosY_[0]);

        //        // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        //        instanceImages_[chapterCnt_] = itemBox_[chapterCnt_].transform.Find("ItemIcon").GetComponent<Image>();
        //        instanceImages_[chapterCnt_].sprite = ItemImageMng.itemIcon_[1, 0];

        //        // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        //        instanceTexts_[chapterCnt_] = itemBox_[chapterCnt_].transform.Find("ItemNum").GetComponent<Text>();
        //        instanceTexts_[chapterCnt_].text = itemCnt_[chapterCnt_].ToString();
        //       // chapterCnt_++;
        //    }
        //}



        //if (saveChapterNum_[chapterCnt_] < EventMng.GetChapterNum())
        //{
        //    itemName[0] = saveItemName_[0];
        //    //saveItemNum_[0] = itemNum;
        //    itemCnt_[0] = 5;
        //    // �摜�𐶐��@(���ɂȂ�prefab�A���W�A��]�A�e)
        //    itemBox_[0] = Instantiate(itemUIPrefab,
        //        new Vector2(0, 0), Quaternion.identity, this.transform);
        //    // �\���ʒu�����炷
        //    itemBox_[0].transform.localPosition = new Vector2(boxPosX_[1], boxPosY_[0]);

        //    // ���������v���n�u�̎q�ɂȂ��Ă���Image��������
        //    instanceImages_[0] = itemBox_[0].transform.Find("ItemIcon").GetComponent<Image>();
        //    instanceImages_[0].sprite = ItemImageMng.materialIcon_[1, 0];

        //    // ���������v���n�u�̎q�ɂȂ��Ă���Text��������
        //    instanceTexts_[0] = itemBox_[0].transform.Find("ItemNum").GetComponent<Text>();
        //    instanceTexts_[0].text = itemCnt_[0].ToString();
        //}
        // chapterCnt_++;

    }

    public void SetItemKinds(ItemList list)
    {
        // �f�ޖ��̃��X�g���擾
        if (list != null)
        {
            for (int i = 0; i < (int)EVENT_ITEM.MAX; i++)
            {
                // ���݃t�B�[���h�̑f�ޖ���ۑ�
                saveItemName_[i] = list.param[i].ItemName;
                saveChapterNum_[i] = list.param[i].ChapterNumber;
            }
        }        
    }
}
