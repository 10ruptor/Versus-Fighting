using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(BufferedActionController))]
public class PlayerInputController : MonoBehaviour
{
    [Header("Horizontal movement")]
    [SerializeField] float walkThreshold = 0.2f;
    [SerializeField] float dashThreshold = 0.4f;
    
    [Header("Vertical movement")]
    [SerializeField] float downMoveInputThreshold = -0.5f;
    [SerializeField] float upMoveInputThreshold = 0.2f;
    const string PlayerActionMapName = "Player";
    
    BufferedActionController bufferedActionController;
    public BufferedActionController BufferedActionController =>  bufferedActionController;
    
    //inputs
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;
    InputAction attackAction; 
    InputAction shieldAction;
    //values
    public bool FastFall;
    public float HorizontalMoveInputValue;
    public float VerticalMoveInputValue;

    /// <summary>
    /// Etat maintenu du bouton bouclier. Les deux fronts sont bufferises (voir plus bas),
    /// mais l'etat maintenu reste necessaire : une intention bufferisee expire, alors
    /// qu'un bouton tenu doit garder le bouclier leve tant que le joueur appuie.
    /// </summary>
    public bool Shield;
    
    // Actions bufferisees : la FSM interroge le buffer plutot qu'un flag "presse cette frame".
    public bool JumpBuffered => bufferedActionController.HasAlive(BufferedAction.BufferedActionType.Jump);
    public bool AttackBuffered => bufferedActionController.HasAlive(BufferedAction.BufferedActionType.Attack);
    public bool ShieldPressedBuffered => bufferedActionController.HasAlive(BufferedAction.BufferedActionType.ShieldPressed);
    public bool ShieldReleasedBuffered => bufferedActionController.HasAlive(BufferedAction.BufferedActionType.ShieldReleased);
    public void ConsumeJumpBuffer() => bufferedActionController.Consume(BufferedAction.BufferedActionType.Jump);
    public void ConsumeAttackBuffer() => bufferedActionController.Consume(BufferedAction.BufferedActionType.Attack);
    public void ConsumeShieldPressedBuffer() => bufferedActionController.Consume(BufferedAction.BufferedActionType.ShieldPressed);
    public void ConsumeShieldReleasedBuffer() => bufferedActionController.Consume(BufferedAction.BufferedActionType.ShieldReleased);
    public void ClearBuffer() => bufferedActionController.Clear();

    /// <summary>
    /// Date du dernier appui bouclier, independante de la duree de buffer de l'action :
    /// c'est le ShieldController qui la confronte a SA propre fenetre de contre.
    /// </summary>
    public float LastShieldPressTime => bufferedActionController.LastPressTime(BufferedAction.BufferedActionType.ShieldPressed);

    public bool HasDownMoveInput => VerticalMoveInputValue < downMoveInputThreshold;
    public bool HasUpMoveInput => VerticalMoveInputValue > upMoveInputThreshold;
    public bool HasWalkInput => Mathf.Abs(HorizontalMoveInputValue) > walkThreshold;
    public bool HasDashInput => Mathf.Abs(HorizontalMoveInputValue) > dashThreshold;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        bufferedActionController = GetComponent<BufferedActionController>();
    }
    private void Start()
    {
        if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != PlayerActionMapName)
            playerInput.SwitchCurrentActionMap(PlayerActionMapName);
        
        moveAction = playerInput.actions.FindAction("Move", true);
        jumpAction = playerInput.actions.FindAction("Jump", true);
        fastFallAction = playerInput.actions.FindAction("FastFall", true);
        attackAction = playerInput.actions.FindAction("Attack", true);
        shieldAction = playerInput.actions.FindAction("Shield", true);
        
    }
    void HorizontalMoveInput(float newInput) { HorizontalMoveInputValue = newInput; }
    void VerticalMoveInput(float newInput) { VerticalMoveInputValue = newInput; }
    void FastFallInput(bool newInput) { FastFall = newInput; }
    void ShieldInput(bool newInput) { Shield = newInput; }
    #region callbacks

    public void OnJump(InputValue value)
    {
        bufferedActionController.AddBufferedAction(BufferedAction.BufferedActionType.Jump,Time.time);
    }

    public void OnFastFall(InputValue value)
    {
        FastFallInput(value.isPressed);
    }

    public void OnMove(InputValue value)
    {
        HorizontalMoveInput(value.Get<Vector2>().x);
        VerticalMoveInput(value.Get<Vector2>().y);
    }
    
    public void OnAttack(InputValue value)
    {
        bufferedActionController.AddBufferedAction(BufferedAction.BufferedActionType.Attack,Time.time);
    }

    /// <summary>
    /// L'action Shield est en Press(behavior=2) : ce callback est appele aux deux fronts.
    /// En plus de l'etat maintenu, chaque front est bufferise separement, avec sa propre
    /// duree dans le BufferInputSettingsSO.
    /// </summary>
    public void OnShield(InputValue value)
    {
        ShieldInput(value.isPressed);

        BufferedAction.BufferedActionType actionType = value.isPressed
            ? BufferedAction.BufferedActionType.ShieldPressed
            : BufferedAction.BufferedActionType.ShieldReleased;

        bufferedActionController.AddBufferedAction(actionType, Time.time);
    }

    #endregion
    
}
