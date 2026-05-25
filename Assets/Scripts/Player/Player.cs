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
    [SerializeField] int jumpCount;
    [SerializeField] string currentStateName;

    Rigidbody rb;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;

    int stageContactCount;

    public enum JumpPhase { Ascent, Descent }

    float jumpAscentTimer;
    float jumpStartHeight;
    JumpPhase currentJumpPhase;

    public CharacterStatsSO Stats => characterStats;
    public JumpPhase CurrentJumpPhase => currentJumpPhase;
    public PlayerStateMachine StateMachine { get; private set; }
    public Rigidbody Rigidbody => rb;
    public int JumpCount => jumpCount;
    public bool CanJump => jumpCount < Stats.maxAddJumpCount;
    public bool IsGrounded { get; private set; }
    public bool IsFastFallHeld => fastFallAction != null && fastFallAction.IsPressed();
    public bool JumpRequested { get; private set; }
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPerformedThisFrame();
    public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    public bool HasMoveInput => Mathf.Abs(MoveInput.x) > MoveInputThreshold;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        StateMachine = new PlayerStateMachine(this);

        if (characterStats == null)
            Debug.LogError("CharacterStatsSO is not assigned on Player.", this);
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
            JumpRequested = true;

        StateMachine.CurrentState.Update();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            JumpRequested = true;
    }

    void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
        JumpRequested = false;
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

    public void BeginJump()
    {
        currentJumpPhase = JumpPhase.Ascent;
        jumpAscentTimer = 0f;
        jumpStartHeight = transform.position.y;
        rb.useGravity = false;
    }

    public void EndJumpPhysics()
    {
        rb.useGravity = true;
    }

    public void ApplyJumpVerticalPhysics()
    {
        float ascentDuration = Mathf.Max(Stats.jumpAscentDuration, 0.01f);
        Vector3 velocity = rb.linearVelocity;

        if (currentJumpPhase == JumpPhase.Ascent)
        {
            jumpAscentTimer += Time.fixedDeltaTime;
            float ascentProgress = Mathf.Clamp01(jumpAscentTimer / ascentDuration);
            float easeOut = 1f - ascentProgress;
            velocity.y = (3f * Stats.jumpHeight / ascentDuration) * easeOut * easeOut;

            bool reachedHeight = transform.position.y >= jumpStartHeight + Stats.jumpHeight;
            bool reachedDuration = ascentProgress >= 1f;

            if (reachedHeight || reachedDuration)
            {
                currentJumpPhase = JumpPhase.Descent;
                velocity.y = 0f;
            }
        }
        else
        {
            float fallAccelerationMultiplier = Stats.weight;

            if (IsFastFallHeld && velocity.y < 0f)
                fallAccelerationMultiplier *= Stats.fastFallAccelerationMultiplier;

            velocity.y += Physics.gravity.y * fallAccelerationMultiplier * Time.fixedDeltaTime;
        }

        Vector3 current = rb.linearVelocity;
        current.y = velocity.y;
        rb.linearVelocity = current;
    }

    public void ConsumeJump()
    {
        jumpCount++;
    }

    public void ResetJumpCount()
    {
        jumpCount = 0;
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
            ResetJumpCount();
    }
}
