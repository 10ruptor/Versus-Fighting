using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] float horizontalMoveInputThreshold = 0.5f;
    [SerializeField] float downMoveInputThreshold = -0.5f;
    [SerializeField] float upMoveInputThreshold = 0.5f;
    const string PlayerActionMapName = "Player";
    
    //inputs
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;
    InputAction attackAction; 
    
    //values
    public bool jump;
    public bool fastFall;
    public bool attack;
    public float horizontalMoveInput;
    public float verticalMoveInput;
    
    public bool HasDownMoveInput => verticalMoveInput < downMoveInputThreshold;
    public bool HasUpMoveInput => verticalMoveInput > upMoveInputThreshold;
    public bool HasHorizontalMoveInput => Mathf.Abs(horizontalMoveInput) > horizontalMoveInputThreshold;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
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

    void HorizontalMoveInput(float newInput)
    {
        Debug.Log("Move input : "+ newInput);
        horizontalMoveInput = newInput;
    }

    void VerticalMoveInput(float newInput)
    {
        Debug.Log("Move input : "+ newInput);
        verticalMoveInput = newInput;
    }
    
    void AttackInput(bool newInput) { attack = newInput; }
    void FastFallInput(bool newInput) { fastFall = newInput; }
    void JumpInput(bool newInput) { jump = newInput; }
    public void ConsumeJumpRequest() { jump = false; }
    
    #region callbacks

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
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
        AttackInput(value.isPressed);
        Debug.Log("Attack Input: " + value.isPressed);
    }
    
    #endregion
    
}
