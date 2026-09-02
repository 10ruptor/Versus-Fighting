using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterAttackLibrary))]
public class HitboxManager : MonoBehaviour
{

    [SerializeField] private List<Hitbox> hitboxesList = new List<Hitbox>();
    private Dictionary<AttackTypes, Hitbox> hitboxes = new Dictionary<AttackTypes, Hitbox>();
    public Dictionary<AttackTypes, Hitbox> Hitboxes => hitboxes;

    private CharacterAttackLibrary characterAttackLibrary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private PlayerGameplay owner;
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        foreach (Hitbox hitbox in hitboxesList)
        {
            hitbox.Initialize(owner);
        }
    }

    void Awake()
    {
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>() )
        {
            hitboxesList.Add(hitbox);
            hitboxes.Add(hitbox.AttackType, hitbox);
        }

        characterAttackLibrary = GetComponent<CharacterAttackLibrary>();
    }

    public void ActivateHitbox(AttackTypes attackType)
    {
        Attack attack = characterAttackLibrary.Attacks[attackType];
        foreach (Hitbox hitboxe in attack.attackHitboxes)
        {
            hitboxe.enabled = true;
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
        if (hitboxes.ContainsKey(attackType))
        {
            hitboxes[attackType].enabled = false;
        }
        else Debug.LogError("HitboxManager: No hitbox found for attack type " + attackType);
    }
}
