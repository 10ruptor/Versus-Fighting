using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    
    protected override string StateAnimationName => "Idle";
    
    #region InputAccessors

    private bool playerHasWalkInput => stateMachine.PlayerGameplay.PlayerInputController.HasWalkInput;
    private bool playerHasDashInput => stateMachine.PlayerGameplay.PlayerInputController.HasDashInput;
    private bool playerHasDownMoveInput => stateMachine.PlayerGameplay.PlayerInputController.HasDownMoveInput;
    private bool playerHasAttackInput => stateMachine.PlayerGameplay.PlayerInputController.AttackBuffered;
    

    #endregion
    
    #region  Transitions

    public override void RegisterTransition()
    {
        base.RegisterTransition();
        AddTransition(() => playerHasDownMoveInput && stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Crouch]);
        AddTransition(() => playerHasAttackInput && stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Attack]);
        AddTransition(() => playerHasDashInput && stateMachine.PlayerGameplay.IsGrounded,stateMachine.stateLibrary[StateType.Dash] );
        AddTransition(() => playerHasWalkInput && stateMachine.PlayerGameplay.IsGrounded, stateMachine.stateLibrary[StateType.Move]);
    }
    
    #endregion
    
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckTransitions();
        Vector3 velocity = stateMachine.PlayerGameplay.Rigidbody.linearVelocity;
        velocity.x = 0f;
        stateMachine.PlayerGameplay.Rigidbody.linearVelocity = velocity;
    }
}
