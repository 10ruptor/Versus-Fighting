using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    const string PlayerActionMapName = "Player";
    
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;
    
    
    //new 
    public bool jump;
    public bool fastFall;
    public Vector2 horizontalMoveInput;
    
    //public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    public bool HasMoveInput => Mathf.Abs(horizontalMoveInput.x) > MoveInputThreshold;
    
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
        
    }

    
    void HorizontalMoveInput(Vector2 newInput) { horizontalMoveInput = newInput; }
    void JumpInput(bool newInput) { jump = newInput; }
    void FastFallInput(bool newInput) { fastFall = newInput; }
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
        HorizontalMoveInput(value.Get<Vector2>());
        Debug.Log("Move input: " + horizontalMoveInput);
    }
    
    #endregion
    
}
