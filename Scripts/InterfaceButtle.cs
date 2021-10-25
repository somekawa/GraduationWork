using UnityEngine;

interface InterfaceButtle
{
    // Œp³‘¤‚ÅŠÖ”‚ğÀ‘•‚·‚é•K—v‚ª‚ ‚é
    string Name();
    bool Attack();
    int Damage();
    int Speed();
    int HP();
    int MaxHP();
    (Vector3, bool) RunMove(float time,Vector3 myPos, Vector3 targetPos);
    (Vector3, bool) BackMove(float time, Vector3 myPos, Vector3 targetPos);
}