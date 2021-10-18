using UnityEngine;

interface InterfaceButtle
{
    // �p�����Ŋ֐�����������K�v������
    bool Attack();
    void Damage();
    int HP();
    int MaxHP();
    (Vector3, bool) RunMove(float time,Vector3 myPos, Vector3 targetPos);
    (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos);
}