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

    [Header("Attack")] 
    [SerializeField] private Hitbox tiltHitbox;
    [SerializeField] private Hitbox aimHitbox;
    public Hitbox TiltHitbox => tiltHitbox;
    public Hitbox AimHitbox => aimHitbox;
    
    [Header("Stats")]
    [SerializeField] CharacterStatsSO characterStats;
    public CharacterStatsSO Stats => characterStats;
    
    [Header("Visuals")]
    [SerializeField] VisualOrientationController visualOrientationController;
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
    public PlayerStateMachine StateMachine { get; private set; }
    public Rigidbody Rigidbody => rb;
    public PlayerInputManager PlayerInputManager => playerInputManager;
    public AttackController AttackController => attackController;
    public bool IsGrounded => collisionController.IsGrounded;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputManager = GetComponent<PlayerInputManager>();
        StateMachine = new PlayerStateMachine(this);
       
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
        if (Mathf.Abs(PlayerInputManager.horizontalMoveInput.x) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputManager.horizontalMoveInput.x * Stats.moveSpeed;
        rb.linearVelocity = velocity;
    }
    public void ApplyHorizontalMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputManager.horizontalMoveInput.x * Stats.moveSpeed;
        rb.linearVelocity = velocity;
        characterAnimatorController.UpdateVelocityAnimation(velocity.x, Stats.moveSpeed);
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
        visualOrientationController.VisualOrientationUpdate(currentOrientation);
    }
}
