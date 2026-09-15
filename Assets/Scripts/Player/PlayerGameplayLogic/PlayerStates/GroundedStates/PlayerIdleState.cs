using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    public override StateType State => StateType.Idle;
    protected override string StateAnimationName => "Idle";
    
    #region InputAccessors

    private bool playerHasWalkInput => playerGameplay.PlayerInputController.HasWalkInput;
    private bool playerHasDashInput => playerGameplay.PlayerInputController.HasDashInput;
    private bool playerHasDownMoveInput => playerGameplay.PlayerInputController.HasDownMoveInput;
    private bool playerHasAttackInput => playerGameplay.PlayerInputController.AttackBuffered;
    

    #endregion
    
    #region  Transitions

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => playerHasDownMoveInput && playerGameplay.IsGrounded, stateMachine.Crouch);
        AddTransition(() => playerHasAttackInput && playerGameplay.IsGrounded, stateMachine.Attack);
        AddTransition(() => playerHasDashInput && playerGameplay.IsGrounded, stateMachine.Dash);
        AddTransition(() => playerHasWalkInput && playerGameplay.IsGrounded, stateMachine.Move);
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
