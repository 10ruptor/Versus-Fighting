using UnityEngine;
using System.Collections.Generic;
public class HurtBoxManager : MonoBehaviour
{

    [SerializeField] Hurtbox characterHurtbox ;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private PlayerGameplay owner;
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        characterHurtbox.Initialize(owner);
    }

    void Awake()
    {
        characterHurtbox = GetComponentInChildren<Hurtbox>();
    }

    public void ActivateHurtbox(AttackTypes attackType)
    {
        characterHurtbox.enabled = true;
    }
    
    public void DeactivateHurtbox(AttackTypes attackType)
    {
        
        characterHurtbox.enabled = false;
    }
}
