
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

[System.Serializable] public class Attack
{
    public string AnimationTrigger;
    public List<Hitbox> attackHitboxes = new  List<Hitbox>();
    public AttackTypes AttackType;
}



public class CharacterAttackLibrary : MonoBehaviour
{
    [SerializeField] List<Attack> attackList = new List<Attack>();
    public Dictionary<AttackTypes, Attack> Attacks = new Dictionary<AttackTypes,Attack>();
    
    private void Awake()
    {
        foreach (Attack attack in attackList )
        {
            Attacks.Add(attack.AttackType, attack);
        }
    }
}


