using System;
using UnityEngine;

public struct HitData
{
    public PlayerGameplay Attacker;
    public AttackDataSO Attack;
    public Vector3 HitPosition;

    // Quelle hurtbox a encaisse le coup. Un personnage en porte plusieurs, reparties sur
    // le squelette, alors qu'il ne possede qu'un seul KnockbackController : sans cette
    // information, un observateur du resultat ne peut pas savoir quelle partie du corps a
    // ete touchee.
    public Hurtbox TargetHurtbox;
}
