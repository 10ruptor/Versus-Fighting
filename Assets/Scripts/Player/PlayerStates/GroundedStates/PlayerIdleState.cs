using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Idle";
    
    #region InputAccessors

    private bool playerHasWalkInput => playerGameplay.PlayerInputManager.HasWalkInput;
    private bool playerHasDashInput => playerGameplay.PlayerInputManager.HasDashInput;
    private bool playerHasDownMoveInput => playerGameplay.PlayerInputManager.HasDownMoveInput;
    private bool playerHasUpMoveInput => playerGameplay.PlayerInputManager.HasUpMoveInput;
    private bool playerHasAttackInput => playerGameplay.PlayerInputManager.attack;
    private bool playerHasJumpInput => playerGameplay.PlayerInputManager.jump;

    #endregion
    
    #region  Transitions
    
    public override void RegisterTransition()
    {
        AddTransition(() => playerHasDownMoveInput && playerGameplay.IsGrounded, playerGameplay.playerCrouchState);
        AddTransition(() => playerHasJumpInput && playerGameplay.IsGrounded && playerGameplay.JumpController.CanJump, playerGameplay.playerJumpState);
        AddTransition(() => playerHasAttackInput && playerGameplay.IsGrounded, playerGameplay.playerAttackState);
        AddTransition(() => playerHasDashInput && playerGameplay.IsGrounded, playerGameplay.playerDashState);
        AddTransition(() => playerHasWalkInput && playerGameplay.IsGrounded, playerGameplay.playerMoveState);
    }
    
    #endregion
    
    
    public override void FixedUpdate()
    {
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        playerGameplay.Rigidbody.linearVelocity = velocity;
        CheckTransitions();
    }
}
