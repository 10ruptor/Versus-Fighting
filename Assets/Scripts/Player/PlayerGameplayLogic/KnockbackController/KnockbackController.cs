using UnityEngine;

/// <summary>
/// Domaine "encaisser un coup" : encaisse le %, calcule le vecteur d'ejection (angle + vitesse) a partir des stats de l'attaque et du % courant de la victime,
/// puis transmet le resultat a PlayerKnockedState.
/// </summary>
[RequireComponent(typeof(PlayerGameplay))]
public class KnockbackController : MonoBehaviour
{
    // En dessous de ce seuil, le contact est considere comme centre sur la victime : le cote d'ejection n'est plus lisible geometriquement.
    const float SideResolutionThreshold = 0.001f;

    PlayerGameplay playerGameplay;

<<<<<<< Updated upstream
=======
    /// <summary>
    /// Publie le resultat d'un coup encaisse : le HitData d'origine et le vecteur d'ejection effectivement applique. Point de sortie generique qui evite a un
    /// observateur (gizmos de debug, VFX, secousse de camera...) d'avoir a rejouer le calcul, sans que ce controleur ait a connaitre ses observateurs.
    /// </summary>
    public event Action<HitData, Vector3> KnockbackResolved;

>>>>>>> Stashed changes
    private void Awake()
    {
        playerGameplay = GetComponent<PlayerGameplay>();
    }

    public void Knockback(HitData hitData)
    {
        if (hitData.Attack == null)
        {
            Debug.LogError("KnockbackController: HitData recu sans AttackDataSO.", this);
            return;
        }

        // Le coup qui touche compte dans son propre scaling : on encaisse le % AVANT de calculer l'ejection. TotalDamage inclut la part elementaire quand l'attaque en porte une, sans que ce controleur ait a le savoir.
        playerGameplay.DamageController.AddDamage(hitData.Attack.TotalDamage);

        Vector3 launchVelocity = ComputeLaunchVelocity(hitData);
        float knockedDuration = playerGameplay.Character.CharacterStatData.knockedDuration;

        playerGameplay.PlayerKnockedState.Initialize(hitData, launchVelocity, knockedDuration);
        playerGameplay.StateMachine.ChangeState(playerGameplay.PlayerKnockedState);
    }

    /// <summary>
    /// Vitesse d'ejection = stats de l'attaque, mise a l'echelle par le % de la victime.
    /// Direction = launchAngle (degres, config designer) projete en 2D, mirrore selon le cote d'ou vient le coup.
    /// </summary>
    Vector3 ComputeLaunchVelocity(HitData hitData)
    {
        AttackDataSO attack = hitData.Attack;

        float speed = attack.baseKnockback + attack.knockbackScaling * playerGameplay.DamageController.CurrentPercent;
        speed *= attack.KnockbackMultiplier;
        float angleRadians = attack.launchAngle * Mathf.Deg2Rad;
        float side = ResolveLaunchSide(hitData);

        Vector3 direction = new Vector3(Mathf.Cos(angleRadians) * side, Mathf.Sin(angleRadians), 0f);
        return direction * speed;
    }

    /// <summary>
    /// Le point de contact ne sert qu'a determiner de quel cote la victime a ete
    /// touchee : on est ejecte a l'oppose. Aucune normale geometrique n'est derivee
    /// du contact, l'angle reste entierement pilote par la data de l'attaque.
    /// </summary>
    float ResolveLaunchSide(HitData hitData)
    {
        float horizontalOffset = transform.position.x - hitData.HitPosition.x;

        if (Mathf.Abs(horizontalOffset) >= SideResolutionThreshold)
            return Mathf.Sign(horizontalOffset);
        
        if (hitData.Attacker == null)
            return 1f;

        return hitData.Attacker.CurrentOrientation == PlayerGameplay.Orientation.Left ? -1f : 1f;
    }
}
