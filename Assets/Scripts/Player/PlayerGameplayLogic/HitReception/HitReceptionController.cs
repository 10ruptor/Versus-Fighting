using System;
using UnityEngine;

/// <summary>
/// Point d'entree unique d'un coup recu. Hurtbox et bouclier ne decident plus de ce qui
/// arrive au joueur : ils constatent le contact et transmettent ici.
///
/// Ce controleur n'implemente aucune reaction, il ordonne les defenses avant de laisser le
/// coup suivre son cours : le bouclier a son mot a dire en premier, et c'est seulement
/// s'il ne l'arrete pas que le KnockbackController ejecte.
///
/// C'est l'endroit ou viendront se brancher les defenses a venir (super armure,
/// invincibilite de roulade, reaction elementaire...) sans toucher ni aux hurtbox ni au
/// KnockbackController.
/// </summary>
[RequireComponent(typeof(PlayerGameplay))]
public class HitReceptionController : MonoBehaviour
{
    PlayerGameplay playerGameplay;

    /// <summary>Publie ce qu'il est advenu du coup une fois toutes les defenses consultees.</summary>
    public event Action<HitData, ShieldHitOutcome> HitReceived;

    void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    /// <param name="contactOnShield">
    /// True quand c'est le volume du bouclier qui a constate le contact : inutile alors de
    /// verifier qu'il couvre le point d'impact, il EST le point d'impact.
    /// </param>
    public void ReceiveHit(HitData hitData, bool contactOnShield = false)
    {
        if (hitData.AttackData == null)
        {
            Debug.LogError("HitReceptionController : HitData recu sans AttackDataSO.", this);
            return;
        }

        ShieldHitOutcome outcome = playerGameplay.ShieldController.ResolveHit(hitData, contactOnShield);

        if (outcome == ShieldHitOutcome.NotShielded)
            playerGameplay.KnockbackController.Knockback(hitData);

        HitReceived?.Invoke(hitData, outcome);
    }
}
