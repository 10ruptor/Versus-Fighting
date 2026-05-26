using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerGameplay : MonoBehaviour
{
    const string PlayerActionMapName = "Player";
    const string StageTag = "Stage";
    const float MoveInputThreshold = 0.01f;

    [SerializeField] CharacterStatsSO characterStats;
    [SerializeField] string currentStateName;
    [SerializeField] VisualOrientationController visualOrientationController;
    
    public Animator animator;
    Rigidbody rb;
    PlayerInputController playerInputController;
    JumpController jumpController;

    int stageContactCount;


    public CharacterStatsSO Stats => characterStats;
    public JumpController JumpController => jumpController;
    public PlayerStateMachine StateMachine { get; private set; }
    public Rigidbody Rigidbody => rb;
    public PlayerInputController PlayerInputController => playerInputController;
    public bool IsGrounded { get; private set; }
    
    
    public enum Orientations { Left, Right }

    private Orientations currentOrientation;
    public Orientations CurrentOrientation => currentOrientation;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputController = GetComponent<PlayerInputController>();
        StateMachine = new PlayerStateMachine(this);
       
        if (characterStats == null)
            Debug.LogError("CharacterStatsSO is not assigned on Player.", this);

        jumpController = new JumpController(rb, transform, characterStats);
    }

    void Start()
    {
        StateMachine.Initialize(new PlayerIdleState(this));
    }

    void Update()
    {
        StateMachine.CurrentState.Update();
        OrientationCheck();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
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
        velocity.x = PlayerInputController.horizontalMoveInput.x * Stats.moveSpeed;
        rb.linearVelocity = velocity;
    }

    public void ApplyAirHorizontalMovement()
    {
        if (Mathf.Abs(PlayerInputController.horizontalMoveInput.x) <= MoveInputThreshold)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = PlayerInputController.horizontalMoveInput.x * Stats.moveSpeed;
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
