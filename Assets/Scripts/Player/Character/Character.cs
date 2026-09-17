using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private CharacterStatData characterStatData;
    public CharacterStatData CharacterStatData => characterStatData;

    [Tooltip("Character dependant state parameter library (SO)")]
    [SerializeField] private StateParametersLibrarySO stateParametersLibrary;
    public StateParametersLibrarySO StateParametersLibrary => stateParametersLibrary;

    [Header("Visuals")] 
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    [SerializeField] VFXManager vfxManager;
    [SerializeField] MeshPositionComputer meshPositionComputer;
    [SerializeField] GameObject shield;
    
    public VFXManager VFXManager => vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    public MeshPositionComputer MeshPositionComputer => meshPositionComputer;
    public GameObject Shield => shield;
    
    [Header("Combat")]
    [SerializeField] private HurtBoxManager hurtBoxManager;
    [SerializeField] private CharacterAttackLibrary attackLibrary;
    
    //public Dictionary<AttackTypes, AttackStatsSO> attackLookup = new Dictionary<AttackTypes, AttackStatsSO>();
    public CharacterAttackLibrary AttackLibrary => attackLibrary;
    public HurtBoxManager HurtBoxManager => hurtBoxManager;
    private PlayerGameplay owner;
    
    
    public void Initialize(PlayerGameplay owner)
    {
        this.owner = owner;
        attackLibrary.Initialize(owner);
        hurtBoxManager.Initialize(owner);
        meshPositionComputer.Initialize(owner);
    }
}
