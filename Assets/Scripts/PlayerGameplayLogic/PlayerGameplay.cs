using System;
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
    
    [Header("Character")]
    [SerializeField] Character character;
    public Character Character => character;
    
    [Header("Visuals")]
    [SerializeField] CharacterAnimatorController characterAnimatorController;
    [SerializeField] VFXManager vfxManager;
    public CharacterAnimatorController CharacterAnimatorController => characterAnimatorController;
    public enum Orientation { Left, Right }
    private Orientation currentOrientation;
    public Orientation CurrentOrientation => currentOrientation;
    
    
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
    public VFXManager VFXManager => vfxManager;

    #region  StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerDashState playerDashState { get; private set; }
    public PlayerIdleState playerIdleState { get; private set; }
    public PlayerJumpingState PlayerJumpingState { get; private set; }
    public PlayerMoveState playerMoveState { get; private set; }
    public PlayerCrouchState playerCrouchState { get; private set; }
    public PlayerAttackState playerAttackState { get; private set; }
    public PlayerAirAttackState playerAirAttackState { get; private set; }
    
    public PlayerLandingState playerLandingState { get; private set; }
    
    void InitializeStateMachine()
    {
        StateMachine = new PlayerStateMachine(this);
        
        playerDashState = new PlayerDashState(this);
        playerIdleState = new PlayerIdleState(this);
        PlayerJumpingState = new PlayerJumpingState(this);
        playerMoveState = new PlayerMoveState(this);
        playerCrouchState = new PlayerCrouchState(this);
        playerAttackState = new PlayerAttackState(this);
        playerAirAttackState = new PlayerAirAttackState(this);
        playerLandingState = new PlayerLandingState(this);
        
        playerDashState.RegisterTransition();
        playerIdleState.RegisterTransition();
        PlayerJumpingState.RegisterTransition();
        playerMoveState.RegisterTransition();
        playerCrouchState.RegisterTransition();
        playerAttackState.RegisterTransition();
        playerAirAttackState.RegisterTransition();
        playerLandingState.RegisterTransition();
        
    }
    #endregion
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collisionController = GetComponent<CharacterCollisionController>();
        attackController = GetComponent<AttackController>();
        collisionController = GetComponent<CharacterCollisionController>();
        playerInputManager = GetComponent<PlayerInputManager>();
        jumpController = GetComponent<JumpController>();
        
        InitializeStateMachine();
        
        if (!character)
            Debug.LogError("No Character assigned to player.", this);
        
    }

    void Start()
    {
        StateMachine.Initialize(playerIdleState);
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
        if (Mathf.Abs(PlayerInputManager.HorizontalMoveInputValue) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputManager.HorizontalMoveInputValue * Character.CharacterStatData.moveSpeed;
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
            currentOrientation = Orientation.Right;
            
        }
        else if (rb.linearVelocity.x < 0)
        {
            currentOrientation  = Orientation.Left;
        }
        characterAnimatorController.VisualOrientationUpdate(currentOrientation);
    }
}
