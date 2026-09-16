using System;
using UnityEngine;

public struct HitData
{
    public PlayerGameplay Attacker;
    public AttackDataSO AttackData;
    public Vector3 HitPosition;
    public Hurtbox HurtedHurtbox;
}
