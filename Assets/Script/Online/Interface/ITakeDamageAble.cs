using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnTakeDamageHadle(int Damage, Behaviour Sender);

public interface IOnTakeDamageAble
{
    event OnTakeDamageHadle OnTakeDamage;
}
