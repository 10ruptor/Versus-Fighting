using UnityEngine;
using System.Collections.Generic;
public class HurtBoxManager : MonoBehaviour
{

    [SerializeField] List<Hurtbox> characterHurtboxes = new List<Hurtbox>() ;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private PlayerGameplay owner;
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        foreach (Hurtbox hurtbox in characterHurtboxes)
        {
            hurtbox.Initialize(owner);
        }
    }

    void Awake()
    {
        characterHurtboxes.AddRange(GetComponentsInChildren<Hurtbox>(includeInactive: true));
    }
}
