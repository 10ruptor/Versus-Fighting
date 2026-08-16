using UnityEngine;
using System.Collections.Generic;
public class HitboxManager : MonoBehaviour
{

    [SerializeField] private List<Hitbox> hitboxesList = new List<Hitbox>();
    private Dictionary<AttackTypes, Hitbox> hitboxes = new Dictionary<AttackTypes, Hitbox>();
    public Dictionary<AttackTypes, Hitbox> Hitboxes => hitboxes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (Hitbox hitbox in GetComponentsInChildren<Hitbox>() )
        {
            hitboxesList.Add(hitbox);
            hitboxes.Add(hitbox.attack, hitbox);
        }
    }

    public Hitbox selectHitbox(AttackTypes attackType)
    {
        if (hitboxes.ContainsKey(attackType))
        {
            return hitboxes[attackType];
        }
        else
        {
            Debug.Log ("No hit box found for : " + attackType);
            return null;
        }
    }
}
