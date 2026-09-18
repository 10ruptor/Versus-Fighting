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
    
    public VFXManager VFXManager => vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    public MeshPositionComputer MeshPositionComputer => meshPositionComputer;

    [Header("Shield")]
    [Tooltip("Bouclier du personnage : son visuel et son volume. Le composant est porte par le GameObject Shield.")]
    [SerializeField] CharacterShield shield;

    [Tooltip("Stats du bouclier de ce personnage : durabilite, taille, recul au blocage, fenetre de contre.")]
    [SerializeField] ShieldDataSO shieldData;

    public CharacterShield Shield => shield;
    public ShieldDataSO ShieldData => shieldData;
    
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

        if (shield != null)
            shield.Initialize(owner);
    }
}
