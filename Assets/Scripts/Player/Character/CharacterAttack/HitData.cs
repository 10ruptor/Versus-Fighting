using System;
using UnityEngine;

public struct HitData
{
    public PlayerGameplay Attacker;
    public AttackDataSO Attack;
    public Vector3 HitPosition;
    public Hurtbox HurtedHurtbox;
}
