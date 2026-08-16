using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerGameplay : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    
    [Header("FSM")]
    [SerializeField] string currentStateName;

    [Header("Character")] 
    [SerializeField] private GameObject characterPrefab;
    Character character;
    public Character Character => character;
    
    public enum Orientation { Left, Right }
    private Orientation currentOrientation;
    public Orientation CurrentOrientation => currentOrientation;
    
    private int playerIndex;
    public int PlayerIndex => playerIndex;
    
    Rigidbody rb;
    PlayerInputController _playerInputController;
    JumpController jumpController;
    CharacterCollisionController collisionController;
    AttackController attackController;
    
    public JumpController JumpController => jumpController;
    public CharacterCollisionController CollisionController => collisionController;
    public Rigidbody Rigidbody => rb;
    public PlayerInputController PlayerInputController => _playerInputController;
    public AttackController AttackController => attackController;
    public bool IsGrounded => collisionController.IsGrounded;

    public void Initialize(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    #region  StateMachine
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerDashState PlayerDashState { get; private set; }
    public PlayerIdleState PlayerIdleState { get; private set; }
    public PlayerJumpingState PlayerJumpingState { get; private set; }
    public PlayerMoveState PlayerMoveState { get; private set; }
    public PlayerCrouchState PlayerCrouchState { get; private set; }
    public PlayerAttackState PlayerAttackState { get; private set; }
    public PlayerAirAttackState PlayerAirAttackState { get; private set; }
    
    public PlayerLandingState PlayerLandingState { get; private set; }
    
    void InitializeStateMachine()
    {
        StateMachine = new PlayerStateMachine(this);
        
        PlayerDashState = new PlayerDashState(this);
        PlayerIdleState = new PlayerIdleState(this);
        PlayerJumpingState = new PlayerJumpingState(this);
        PlayerMoveState = new PlayerMoveState(this);
        PlayerCrouchState = new PlayerCrouchState(this);
        PlayerAttackState = new PlayerAttackState(this);
        PlayerAirAttackState = new PlayerAirAttackState(this);
        PlayerLandingState = new PlayerLandingState(this);
        
        PlayerDashState.RegisterTransition();
        PlayerIdleState.RegisterTransition();
        PlayerJumpingState.RegisterTransition();
        PlayerMoveState.RegisterTransition();
        PlayerCrouchState.RegisterTransition();
        PlayerAttackState.RegisterTransition();
        PlayerAirAttackState.RegisterTransition();
        PlayerLandingState.RegisterTransition();
        
    }
    #endregion
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collisionController = GetComponent<CharacterCollisionController>();
        attackController = GetComponent<AttackController>();
        collisionController = GetComponent<CharacterCollisionController>();
        _playerInputController = GetComponent<PlayerInputController>();
        jumpController = GetComponent<JumpController>();
        
        if (!characterPrefab)
        {
            Debug.LogError("No Character assigned to player.", this);
        }
        else
        {
            InitializeCharacter();
        }
    
        InitializeStateMachine();
    }  

    void Start()
    {
        StateMachine.Initialize(PlayerIdleState);
    }
    void Update()
    {
        StateMachine.CurrentState.Update();
        GroundCheck();
        if(IsGrounded) OrientationCheck();
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
        Character.CharacterAnimatorController.VisualOrientationUpdate(currentOrientation);
    }

    void InitializeCharacter()
    {
        var characterInstance = Instantiate(characterPrefab, transform);
        character = characterInstance.GetComponent<Character>();
        character.Initialize(this);
    }
}
