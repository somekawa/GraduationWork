using UnityEngine;

public class PopList : MonoBehaviour
{
    public enum ListData
    {
        CHAPTER,        // �X�g�[���[
        CHARACTER,      // �L�����N�^�[�������(���ƂŁA���ׂ�CSV�`������̓ǂݍ��ݏ����o���ɕς��邩��)
        ENEMY,          // �G���
        QUESTINFO,      // �N�G�X�g���
        CHEST,          // �󔠏��
        MATERIA,
        ITEM,
        WORD,
        BOOK_STORE,
        MAX
    }

    private T GetList<T>(string dataName)
    {
        T list = (T)(object)Resources.Load(dataName);

        if (list == null)
        {
            Debug.Log("PopMob.cs��list���null�ł�");
        }

        return list;
    }

    public T GetData<T>(ListData data, int num = 0, string str = "")
    {
        string tmpStr;

        switch (data)
        {
            case ListData.CHAPTER:
                tmpStr = "Chapter/Chapter" + num;
                //object(�C�ӂ�T�ɃL���X�g�ł���)�ɃL���X�g���A��������ʂ̌^�ɃL���X�g����K�v������
                return (T)(object)GetList<ChapterList>(tmpStr);

            case ListData.CHARACTER:
                tmpStr = "Chara" + str;
                return (T)(object)GetList<CharacterList>(tmpStr);

            case ListData.ENEMY:
                tmpStr = "EnemyData/Field" + num;
                return (T)(object)GetList<EnemyList>(tmpStr);

            case ListData.QUESTINFO:
                tmpStr = "Quest" + num;
                return (T)(object)GetList<QuestInfo>(tmpStr);

            case ListData.CHEST:
                tmpStr = "Chest";
                return (T)(object)GetList<ChestList>(tmpStr);

            case ListData.MATERIA:
                tmpStr = "MateriaList/M_Field"+num;
                return (T)(object)GetList<MateriaList>(tmpStr);

            case ListData.ITEM:
                tmpStr = "ItemList/ItemList";
                return (T)(object)GetList<ItemList>(tmpStr);

            case ListData.WORD:
                tmpStr = "WordList/WordList0";
                return (T)(object)GetList<WordList>(tmpStr);

            case ListData.BOOK_STORE:
                tmpStr = "BookList/BookList" + num;
                return (T)(object)GetList<BookList>(tmpStr);


            default:
                return default(T);
        }
    }
}
