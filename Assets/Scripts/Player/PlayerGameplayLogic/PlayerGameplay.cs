using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VisualOrientationController))]
public class PlayerGameplay : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    
    [Header("UI")]
    [SerializeField] GameObject playerUIPrefab;

    public GameObject PlayerUIPrefab => playerUIPrefab;

    [Header("FSM")]
    [SerializeField] string currentStateName;
    [SerializeField] PlayerStateMachine stateMachine;

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
    GameObject uiParent;
    
    
    public JumpController JumpController => jumpController;
    public CharacterCollisionController CollisionController => collisionController;
    public Rigidbody Rigidbody => rb;
    public PlayerInputController PlayerInputController => playerInputController;
    public AttackController AttackController => attackController;
    public KnockbackController KnockbackController => knockbackController;
    public DamageController DamageController => damageController;
    public VisualOrientationController VisualOrientationController => visualOrientationController;
    public PlayerStateMachine StateMachine => stateMachine;
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
        
        if (!characterPrefab)
        {
            Debug.LogError("No Character assigned to player.", this);
        }
        else
        {
            InitializeCharacter();
        }
    }  

    void Start()
    {
        InitializePlayerUI();
        stateMachine.Initialize(this);
    }
    void Update()
    {
        GroundCheck();
        if(IsGrounded) visualOrientationController.UpdateOrientation();
    }

    void FixedUpdate()
    {
        //StateMachine.CurrentState.FixedUpdate();
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
