
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
            foreach (Hitbox hitbox in attack.attackHitboxes)
            {
                hitbox.Initialize(owner);
            }

        }
    }
    
    public void ActivateHitbox(AttackTypes attackType)
    {
        Attack attack = Attacks[attackType];
        foreach (Hitbox hitbox in attack.attackHitboxes)
        {
            hitbox.ReadAttack(attack);
            hitbox.enabled = true;
        }
        /*
        if (hitboxes.ContainsKey(attackType))
        {
            hitboxes[attackType].enabled = true;
        }
        else Debug.LogError("HitboxManager: No hitbox found for attack type " + attackType);*/
    }
    
    public void DeactivateHitbox(AttackTypes attackType)
    {
        Attack attack = Attacks[attackType];
        foreach (Hitbox hitbox in attack.attackHitboxes)
        {
            hitbox.enabled = false;
        }
        /*
        if (hitboxes.ContainsKey(attackType))
        {
            hitboxes[attackType].enabled = false;
        }
        else Debug.LogError("HitboxManager: No hitbox found for attack type " + attackType);*/
    }
    
}


