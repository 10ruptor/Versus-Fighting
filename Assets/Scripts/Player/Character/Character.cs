using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private CharacterStatData characterStatData;
    public CharacterStatData CharacterStatData => characterStatData;

    [Header("Visuals")] 
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    [SerializeField] VFXManager vfxManager;
    [SerializeField] MeshPositionComputer meshPositionComputer;
    
    public VFXManager VFXManager => vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    public MeshPositionComputer MeshPositionComputer => meshPositionComputer;
    
    [Header("Combat")]
    [SerializeField] private HurtBoxManager hurtBoxManager;
    [SerializeField] private CharacterAttackLibrary attackLibrary;
    
    
    //public Dictionary<AttackTypes, AttackStatsSO> attackLookup = new Dictionary<AttackTypes, AttackStatsSO>();
    public CharacterAttackLibrary AttackLibrary => attackLibrary;
    
    public HurtBoxManager HurtBoxManager => hurtBoxManager;
    
    private PlayerGameplay owner;
    
    
    private void Awake()
    {
        //attackLookup = AttackStatList.ToDictionary(x => x.AttackType);
    }

    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        attackLibrary.Initialize(owner);
        hurtBoxManager.Initialize(owner);
        meshPositionComputer.Initialize(owner);
    }
}
