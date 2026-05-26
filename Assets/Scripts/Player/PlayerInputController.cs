using UnityEngine;
using System;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    const float MoveInputThreshold = 0.01f;
    const string PlayerActionMapName = "Player";
    
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction fastFallAction;
    
    bool jumpRequested;
    
    public bool IsGrounded { get; private set; }
    public bool IsFastFallHeld => fastFallAction != null && fastFallAction.IsPressed();
    public bool JumpRequested => jumpRequested; 
    public bool JumpPressedThisFrame => jumpAction != null && jumpAction.WasPerformedThisFrame();
    public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    public bool HasMoveInput => Mathf.Abs(MoveInput.x) > MoveInputThreshold;

    

    
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
    

    public void OnJump(InputValue value)
    {
        if (value.isPressed) jumpRequested = true;
    }

    public void ConsumeJumpRequest()
    {
        jumpRequested = false;
    }
}
