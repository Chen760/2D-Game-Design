using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharaterEvents
{
    //攻击和攻击数值
    public static UnityAction<GameObject,int> characterDamaged;
    //回复和回复数值
    public static UnityAction<GameObject,int> characterHealed;
}
