using UnityEngine;

/// <summary>
/// Resolution du cote d'ou vient un coup. Partagee par tous ceux qui doivent repousser
/// quelqu'un a partir d'un HitData : l'ejection du KnockbackController comme le recul du
/// ShieldController. Une seule regle, un seul endroit.
/// </summary>
public static class HitDirection
{
    // En dessous de ce seuil, le contact est considere comme centre sur la victime : le
    // cote n'est plus lisible geometriquement.
    const float SideResolutionThreshold = 0.001f;

    /// <summary>
    /// Retourne +1 (repousse vers la droite) ou -1 (vers la gauche). Le point de contact
    /// ne sert qu'a savoir de quel cote la victime a ete touchee : elle part a l'oppose.
    /// Aucune normale geometrique n'est derivee du contact, l'angle reste pilote par la
    /// data de l'attaque. Sur un contact centre, on retombe sur l'orientation de
    /// l'attaquant.
    /// </summary>
    public static float ResolveHorizontalSide(Vector3 victimPosition, HitData hitData)
    {
        float horizontalOffset = victimPosition.x - hitData.HitPosition.x;

        if (Mathf.Abs(horizontalOffset) >= SideResolutionThreshold)
            return Mathf.Sign(horizontalOffset);

        if (hitData.Attacker == null)
            return 1f;

        return hitData.Attacker.CurrentOrientation == PlayerGameplay.Orientation.Left ? -1f : 1f;
    }
}
