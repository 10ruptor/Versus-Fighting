using System.Collections.Generic;
using UnityEngine;

public enum AttackTypes
{
    UpTilt,
    SideTilt,
    DownTilt,
    NeutralTilt,
        
    Nair,
    Fair,
    Bair,
    Dair,
    Uair
}

[System.Serializable] public class Attack
{
    public string AnimationTrigger;
    public List<Hitbox> attackHitboxes = new  List<Hitbox>();
    public AttackTypes AttackType;
    public AttackDataSO attackData;
}