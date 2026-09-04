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

/// <summary>
/// Identifie une hitbox a l'interieur d'une meme attaque. 
/// </summary>
public enum HitboxSlot
{
    First,
    Secondary,
    Tertiary,
    Fourth,
    Fifth
}

[System.Serializable]
public class HitboxBinding
{
    public Hitbox Hitbox;
    [Tooltip("Identifie cette hitbox au sein de l'attaque. C'est la valeur choisie dans " +
             "l'Animation Event pour ouvrir ou fermer cette hitbox en particulier.")]
    public HitboxSlot Slot;
}

[System.Serializable] public class Attack
{
    public string AnimationTrigger;
    public List<HitboxBinding> attackHitboxes = new  List<HitboxBinding>();
    public AttackTypes AttackType;
    public AttackDataSO attackData;

    /// <summary>
    /// Retourne la hitbox de cette attaque occupant le slot demande, ou null si aucune
    /// ne le porte. La recherche reste locale a l'attaque : deux attaques peuvent
    /// reutiliser le meme slot sans se marcher dessus.
    /// </summary>
    public Hitbox GetHitbox(HitboxSlot slot)
    {
        foreach (HitboxBinding usedhitbox in attackHitboxes)
        {
            if (usedhitbox.Hitbox != null && usedhitbox.Slot == slot)
                return usedhitbox.Hitbox;
        }

        return null;
    }
}
