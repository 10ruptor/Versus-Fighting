using System.Collections.Generic;
using UnityEngine;



public class CharacterAttackLibrary : MonoBehaviour
{
    private PlayerGameplay owner;
    [SerializeField] List<Attack> attackList = new List<Attack>();
    public Dictionary<AttackTypes, Attack> Attacks = new Dictionary<AttackTypes,Attack>();
    
    private void Awake()
    {
        foreach (Attack attack in attackList )
        {
            Attacks.Add(attack.AttackType, attack);
        }
    }
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        foreach (Attack attack in Attacks.Values)
        {
            foreach (UsedHitbox usedHitbox in attack.attackHitboxes)
            {
                usedHitbox.Hitbox.Initialize(owner);
            }

        }
    }

    /// <summary>Ouvre toutes les hitbox de l'attaque.</summary>
    public void ActivateHitbox(AttackTypes attackType)
    {
        if (!TryGetAttack(attackType, out Attack attack))
            return;

        foreach (UsedHitbox usedHitbox in attack.attackHitboxes)
        {
            usedHitbox.Hitbox.ReadAttack(attack);
            usedHitbox.Hitbox.enabled = true;
        }
    }

    /// <summary>Ouvre uniquement la hitbox de l'attaque occupant le slot demande.</summary>
    public void ActivateHitbox(AttackTypes attackType, HitboxSlot slot)
    {
        if (!TryGetHitbox(attackType, slot, out Attack attack, out Hitbox hitbox))
            return;

        hitbox.ReadAttack(attack);
        hitbox.enabled = true;
    }

    /// <summary>Ferme toutes les hitbox de l'attaque.</summary>
    public void DeactivateHitbox(AttackTypes attackType)
    {
        if (!TryGetAttack(attackType, out Attack attack))
            return;

        foreach (UsedHitbox usedHitbox in attack.attackHitboxes)
        {
            usedHitbox.Hitbox.enabled = false;
        }
    }

    /// <summary>Ferme uniquement la hitbox de l'attaque occupant le slot demande.</summary>
    public void DeactivateHitbox(AttackTypes attackType, HitboxSlot slot)
    {
        if (!TryGetHitbox(attackType, slot, out _, out Hitbox hitbox))
            return;

        hitbox.enabled = false;
    }

    private bool TryGetAttack(AttackTypes attackType, out Attack attack)
    {
        if (Attacks.TryGetValue(attackType, out attack))
            return true;

        Debug.LogError($"CharacterAttackLibrary: aucune attaque configuree pour {attackType} sur {name}.", this);
        return false;
    }

    /// <summary>
    /// Resout le couple attaque + slot en une hitbox. Le message d'erreur liste les slots
    /// disponibles : un Animation Event mal parametre se diagnostique sans ouvrir le prefab.
    /// </summary>
    private bool TryGetHitbox(AttackTypes attackType, HitboxSlot slot, out Attack attack, out Hitbox hitbox)
    {
        hitbox = null;

        if (!TryGetAttack(attackType, out attack))
            return false;

        hitbox = attack.GetHitbox(slot);
        if (hitbox != null)
            return true;

        Debug.LogError(
            $"CharacterAttackLibrary: l'attaque {attackType} de {name} n'a pas de hitbox sur le slot {slot}. " +
            $"Slots disponibles : {DescribeSlots(attack)}.", this);
        return false;
    }

    private static string DescribeSlots(Attack attack)
    {
        if (attack.attackHitboxes.Count == 0)
            return "aucun";

        List<string> slots = new List<string>(attack.attackHitboxes.Count);
        foreach (UsedHitbox usedHitbox in attack.attackHitboxes)
        {
            if (usedHitbox.Hitbox != null)
                slots.Add(usedHitbox.Slot.ToString());
        }

        return string.Join(", ", slots);
    }
}
