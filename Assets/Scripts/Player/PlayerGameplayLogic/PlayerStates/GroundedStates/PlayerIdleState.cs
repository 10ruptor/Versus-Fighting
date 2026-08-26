using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Idle";
    
    #region InputAccessors

    private bool playerHasWalkInput => playerGameplay.PlayerInputController.HasWalkInput;
    private bool playerHasDashInput => playerGameplay.PlayerInputController.HasDashInput;
    private bool playerHasDownMoveInput => playerGameplay.PlayerInputController.HasDownMoveInput;
    private bool playerHasAttackInput => playerGameplay.PlayerInputController.Attack;
    

    #endregion
    
    #region  Transitions

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => playerHasDownMoveInput && playerGameplay.IsGrounded, playerGameplay.PlayerCrouchState);
        AddTransition(() => playerHasAttackInput && playerGameplay.IsGrounded, playerGameplay.PlayerAttackState);
        AddTransition(() => playerHasDashInput && playerGameplay.IsGrounded, playerGameplay.PlayerDashState);
        AddTransition(() => playerHasWalkInput && playerGameplay.IsGrounded, playerGameplay.PlayerMoveState);
    }
    
    #endregion
    
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
        Vector3 velocity = playerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        playerGameplay.Rigidbody.linearVelocity = velocity;
    }
}
