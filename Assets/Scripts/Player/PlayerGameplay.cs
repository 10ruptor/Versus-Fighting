using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerGameplay : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    
    [Header("FSM")]
    [SerializeField] string currentStateName;
    
    [Header("Stats")]
    [SerializeField] CharacterStatData characterStats;
    public CharacterStatData Stats => characterStats;
    
    [Header("Visuals")]
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    public enum Orientations { Left, Right }
    private Orientations currentOrientation;
    public Orientations CurrentOrientation => currentOrientation;
    
    Rigidbody rb;
    PlayerInputManager playerInputManager;
    JumpController jumpController;
    CharacterCollisionController collisionController;
    AttackController attackController;
    
    public JumpController JumpController => jumpController;
    public CharacterCollisionController CollisionController => collisionController;
    public Rigidbody Rigidbody => rb;
    public PlayerInputManager PlayerInputManager => playerInputManager;
    public AttackController AttackController => attackController;
    public bool IsGrounded => collisionController.IsGrounded;

    #region  StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerDashState playerDashState { get; private set; }
    public PlayerIdleState playerIdleState { get; private set; }
    public PlayerJumpState playerJumpState { get; private set; }
    public PlayerMoveState playerMoveState { get; private set; }
    public PlayerCrouchState playerCrouchState { get; private set; }
    public PlayerAttackState playerAttackState { get; private set; }
    void InitializeStateMachine()
    {
        StateMachine = new PlayerStateMachine(this);
        playerDashState = new PlayerDashState(this);
        playerIdleState = new PlayerIdleState(this);
        playerJumpState = new PlayerJumpState(this);
        playerMoveState = new PlayerMoveState(this);
        playerCrouchState = new PlayerCrouchState(this);
        playerAttackState = new PlayerAttackState(this);
    }
    #endregion
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputManager = GetComponent<PlayerInputManager>();
        InitializeStateMachine();
        if (characterStats == null)
            Debug.LogError("CharacterStatsSO is not assigned on Player.", this);

        jumpController = new JumpController(rb, transform, characterStats);
        collisionController = GetComponent<CharacterCollisionController>();
        attackController = GetComponent<AttackController>();
    }

    void Start()
    {
        StateMachine.Initialize(new PlayerIdleState(this));
    }

    void Update()
    {
        StateMachine.CurrentState.Update();
        GroundCheck();
        OrientationCheck();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
    }
    

    public void ApplyAirHorizontalMovement()
    {
        if (Mathf.Abs(PlayerInputManager.horizontalMoveInput) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputManager.horizontalMoveInput * Stats.moveSpeed;
        rb.linearVelocity = velocity;
    }
    public void SetCurrentStateName(string stateName)
    {
        currentStateName = stateName;
    }

    void GroundCheck()
    {
        if (IsGrounded) JumpController.ResetJumpCount();
    }

    void OrientationCheck()
    {
        if (rb.linearVelocity.x == 0)
        {
            return;
        }
        else if (rb.linearVelocity.x > 0)
        {
            currentOrientation = Orientations.Right;
            
        }
        else if (rb.linearVelocity.x < 0)
        {
            currentOrientation  = Orientations.Left;
        }
        characterAnimatorController.VisualOrientationUpdate(currentOrientation);
    }
}
