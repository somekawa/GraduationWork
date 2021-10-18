using UnityEngine;

interface InterfaceButtle
{
    // 継承側で関数を実装する必要がある
    bool Attack();
    void Damage();
    int HP();
    int MaxHP();
    (Vector3, bool) RunMove(float time,Vector3 myPos, Vector3 targetPos);
    (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos);
}