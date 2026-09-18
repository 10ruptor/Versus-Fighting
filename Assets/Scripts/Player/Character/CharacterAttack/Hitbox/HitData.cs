using System;
using UnityEngine;

public struct HitData
{
    public PlayerGameplay Attacker;
    public AttackDataSO AttackData;
    public Vector3 HitPosition;

    /// <summary>Hurtbox touchee, null quand le contact a eu lieu sur le bouclier.</summary>
    public Hurtbox HurtedHurtbox;

    /// <summary>
    /// Construit le coup a partir de la hitbox qui l'a porte. Fabrique partagee : hurtbox
    /// et bouclier constatent le meme contact et doivent en tirer le meme HitData.
    /// Retourne false (et ne construit rien) si la hitbox ne porte aucune attaque
    /// exploitable, plutot que de laisser filer un coup vide dans le pipeline.
    /// </summary>
    public static bool TryCreate(Hitbox hitbox, Vector3 hitPosition, Hurtbox hurtedHurtbox, out HitData hitData)
    {
        hitData = default;

        if (hitbox == null)
            return false;

        Attack attack = hitbox.CurrentAttack;

        if (attack == null)
        {
            Debug.LogError($"HitData : hitbox {hitbox.name} active sans attaque en cours.", hitbox);
            return false;
        }

        if (attack.attackData == null)
        {
            Debug.LogError("AttackDataSO not found on hitbox.", hitbox);
            return false;
        }

        hitData = new HitData
        {
            Attacker = hitbox.Owner,
            AttackData = attack.attackData,
            HitPosition = hitPosition,
            HurtedHurtbox = hurtedHurtbox
        };

        return true;
    }
}
