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
    //values
    public bool Jump;
    public bool FastFall;
    public bool Attack;
    public float HorizontalMoveInputValue;
    public float VerticalMoveInputValue;
    
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
        
    }
    void HorizontalMoveInput(float newInput) { HorizontalMoveInputValue = newInput; }
    void VerticalMoveInput(float newInput) { VerticalMoveInputValue = newInput; }
    void AttackInput(bool newInput) { Attack = newInput; }
    void FastFallInput(bool newInput) { FastFall = newInput; }
    
    void JumpInput() { Jump = true; }
    
    public void ConsumeBufferedJumpRequest() { Jump = false; }
    
    #region callbacks

    public void OnJump(InputValue value)
    {
        //JumpInput(value.isPressed);
        bufferedActionController.AddBufferedAction(new BufferedAction( BufferedAction.BufferedActionType.Jump , Time.time ));
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
        //AttackInput(value.isPressed);
        bufferedActionController.AddBufferedAction(new BufferedAction( BufferedAction.BufferedActionType.Attack , Time.time ));
    }
    
    #endregion
    
}
