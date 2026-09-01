using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private CharacterStatData characterStatData;
    public CharacterStatData CharacterStatData => characterStatData;
    
    [Header("Visuals")]
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    [SerializeField] VFXManager vfxManager;
    public VFXManager VFXManager => vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    
    [Header("Attacks")]
    //[SerializeField] private List<AttackStatsSO> AttackStatList = new List<AttackStatsSO>();
    
    [SerializeField] private HitboxManager hitboxManager;
    [SerializeField] private HurtBoxManager hurtBoxManager;
    [SerializeField] private CharacterAttackLibrary attackLibrary;
        
    //public Dictionary<AttackTypes, AttackStatsSO> attackLookup = new Dictionary<AttackTypes, AttackStatsSO>();
    public CharacterAttackLibrary AttackLibrary => attackLibrary;
    public HitboxManager HitboxManager => hitboxManager;
    public HurtBoxManager HurtBoxManager => hurtBoxManager;
    
    private PlayerGameplay owner;
    
    private void Awake()
    {
        //attackLookup = AttackStatList.ToDictionary(x => x.AttackType);
    }

    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        hitboxManager.Initialize(owner);
        hurtBoxManager.Initialize(owner);
    }
}
