using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    const string PlayerActionMapName = "Player";
    const string StageTag = "Stage";
    const float MoveInputThreshold = 0.01f;

    [SerializeField] CharacterStatsSO characterStats;
    [SerializeField] string currentStateName;

    
    public Animator animator;

    Rigidbody rb;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;

    int stageContactCount;
    bool jumpRequested;

    public CharacterStatsSO Stats => characterStats;
    public JumpController JumpController { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public Rigidbody Rigidbody => rb;
    public bool IsGrounded { get; private set; }
    public bool IsFastFallHeld => fastFallAction != null && fastFallAction.IsPressed();
    public bool JumpRequested => jumpRequested;
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPerformedThisFrame();
    public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    public bool HasMoveInput => Mathf.Abs(MoveInput.x) > MoveInputThreshold;
    
    
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        StateMachine = new PlayerStateMachine(this);

        if (characterStats == null)
            Debug.LogError("CharacterStatsSO is not assigned on Player.", this);

        JumpController = new JumpController(rb, transform, characterStats);
    }

    void Start()
    {
        if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != PlayerActionMapName)
            playerInput.SwitchCurrentActionMap(PlayerActionMapName);

        moveAction = playerInput.actions.FindAction("Move", true);
        jumpAction = playerInput.actions.FindAction("Jump", true);
        fastFallAction = playerInput.actions.FindAction("FastFall", true);

        StateMachine.Initialize(new PlayerIdleState(this));
    }

    void Update()
    {
        if (jumpAction != null && jumpAction.WasPerformedThisFrame())
            jumpRequested = true;

        StateMachine.CurrentState.Update();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
        jumpRequested = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsStageCollision(collision))
            SetGrounded(stageContactCount + 1);
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsStageCollision(collision))
            SetGrounded(Mathf.Max(stageContactCount, 1));
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsStageCollision(collision))
            SetGrounded(stageContactCount - 1);
    }

    public void ApplyHorizontalMovement()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = MoveInput.x * Stats.moveSpeed;
        rb.linearVelocity = velocity;
    }

    public void ApplyAirHorizontalMovement()
    {
        if (Mathf.Abs(MoveInput.x) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = MoveInput.x * Stats.moveSpeed;
        rb.linearVelocity = velocity;
    }

    public void SetCurrentStateName(string stateName)
    {
        currentStateName = stateName;
    }

    static bool IsStageCollision(Collision collision)
    {
        return collision.collider != null && collision.gameObject.CompareTag(StageTag);
    }

    void SetGrounded(int contactCount)
    {
        stageContactCount = Mathf.Max(0, contactCount);
        IsGrounded = stageContactCount > 0;

        if (IsGrounded)
            JumpController.ResetJumpCount();
    }
}
