using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VisualOrientationController))]
[RequireComponent(typeof(ShieldController))]
[RequireComponent(typeof(HitReceptionController))]
public class PlayerGameplay : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    
    [Header("UI")]
    [SerializeField] GameObject playerUIPrefab;

    public GameObject PlayerUIPrefab => playerUIPrefab;

    [Header("FSM")]
    [SerializeField] string currentStateName;

    [Header("Character")] 
    [SerializeField] private GameObject characterPrefab;
    Character character;
    public Character Character => character;
    
    [Header("Input")]
    [SerializeField] PlayerInputController playerInputController;
    
    public enum Orientation { Left, Right }
    public Orientation CurrentOrientation => visualOrientationController.CurrentOrientation;
    
    private int playerIndex;
    public int PlayerIndex => playerIndex;
    
    Rigidbody rb;
    JumpController jumpController;
    CharacterCollisionController collisionController;
    AttackController attackController;
    KnockbackController knockbackController;
    DamageController damageController;
    VisualOrientationController visualOrientationController;
    ShieldController shieldController;
    HitReceptionController hitReceptionController;
    GameObject uiParent;
    
    
    public JumpController JumpController => jumpController;
    public CharacterCollisionController CollisionController => collisionController;
    public Rigidbody Rigidbody => rb;
    public PlayerInputController PlayerInputController => playerInputController;
    public AttackController AttackController => attackController;
    public KnockbackController KnockbackController => knockbackController;
    public DamageController DamageController => damageController;
    public VisualOrientationController VisualOrientationController => visualOrientationController;
    public ShieldController ShieldController => shieldController;
    public HitReceptionController HitReceptionController => hitReceptionController;

    // PlayerGameplay ne connait plus les etats un par un : la machine en est proprietaire
    // et les expose par propriete typee. Ajouter un etat ne touche donc que PlayerStateMachine.
    public PlayerStateMachine StateMachine { get; private set; }
    
    public bool IsGrounded => collisionController.IsGrounded;

    public void Initialize(int playerIndex, GameObject uiParent)
    {
        this.playerIndex = playerIndex;
        this.uiParent = uiParent;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        attackController = GetComponent<AttackController>();
        collisionController = GetComponent<CharacterCollisionController>();
        jumpController = GetComponent<JumpController>();
        knockbackController = GetComponent<KnockbackController>();
        damageController = GetComponent<DamageController>();
        visualOrientationController = GetComponent<VisualOrientationController>();
        shieldController = GetComponent<ShieldController>();
        hitReceptionController = GetComponent<HitReceptionController>();
        
        if (!characterPrefab)
        {
            Debug.LogError("No Character assigned to player.", this);
        }
        else
        {
            InitializeCharacter();
        }
    
        StateMachine = new PlayerStateMachine(this);

        // Apres la machine ET le Character : les parametres sont une donnee du personnage,
        // les etats en sont les porteurs. A rappeler si le personnage change en cours de partie.
        if (character != null)
            StateMachine.BindStateParameters(character.StateParametersLibrary);
    }  

    void Start()
    {
        StateMachine.Initialize(StateMachine.Idle);
        InitializePlayerUI();
    }
    void Update()
    {
        StateMachine.CurrentState.Update();
        GroundCheck();
        if(IsGrounded) visualOrientationController.UpdateOrientation();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
    }

    public void ApplyAirHorizontalMovement()
    {
        if (Mathf.Abs(PlayerInputController.HorizontalMoveInputValue) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputController.HorizontalMoveInputValue * Character.CharacterStatData.moveSpeed;
        rb.linearVelocity = velocity;
    }
    public void SetCurrentStateName(string stateName)
    {
        this.currentStateName = stateName;
    }
    
    void GroundCheck()
    {
        if (IsGrounded) JumpController.ResetJumpCount();
    }

    void InitializeCharacter()
    {
        var characterInstance = Instantiate(characterPrefab, transform);
        character = characterInstance.GetComponent<Character>();
        character.Initialize(this);
    }

    void InitializePlayerUI()
    {
        TextMeshProUGUI percentText = Instantiate(this.PlayerUIPrefab, uiParent.transform).GetComponent<PlayerUIArea>().PercentText;
        this.DamageController.Initialize(percentText);
    }

    public void PlayerReswpan()
    {
        damageController.ResetPercent();
        transform.position = new Vector3(0, 3, 0);
    }
    
    
}
