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
/// Identifie une hitbox a l'interieur d'une meme attaque. Un Animation Event ne peut
/// porter qu'un seul argument : le type d'attaque etant deja connu au runtime (l'animation
/// jouee EST l'attaque en cours), l'argument sert a designer laquelle des hitbox de cette
/// attaque on ouvre ou ferme. Un enum plutot qu'un index : la fenetre d'animation affiche
/// alors une liste deroulante lisible, et reordonner la liste de hitbox dans l'Inspecteur
/// ne casse pas les events deja poses.
/// A completer selon les besoins des personnages.
/// </summary>
public enum HitboxSlot
{
    Primary,
    Secondary,
    Tertiary,

    Head,
    Body,
    LeftHand,
    RightHand,
    LeftFoot,
    RightFoot,
    Tail,
    Weapon
}

[System.Serializable] public class Attack
{
    public string AnimationTrigger;
    public List<Hitbox> attackHitboxes = new  List<Hitbox>();
    public AttackTypes AttackType;
    public AttackDataSO attackData;

    /// <summary>
    /// Retourne la hitbox de cette attaque occupant le slot demande, ou null si aucune
    /// ne le porte. La recherche reste locale a l'attaque : deux attaques peuvent
    /// reutiliser le meme slot sans se marcher dessus.
    /// </summary>
    public Hitbox GetHitbox(HitboxSlot slot)
    {
        foreach (Hitbox hitbox in attackHitboxes)
        {
            if (hitbox != null && hitbox.Slot == slot)
                return hitbox;
        }

        return null;
    }
}
