using UnityEngine;

interface InterfaceButtle
{
    // Œp³‘¤‚ÅŠÖ”‚ğÀ‘•‚·‚é•K—v‚ª‚ ‚é
    bool Attack();
    void Damage();
    int HP();
    int MaxHP();
    (Vector3, bool) RunMove(float time,Vector3 myPos, Vector3 targetPos);
    (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos);
}