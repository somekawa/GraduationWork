using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface InterfaceButtle
{
    // 継承側で関数を実装する必要がある
    void Attack();
    void Damage();
    void HP();
}