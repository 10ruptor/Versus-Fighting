using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected bool IsLanding => stateMachine.PlayerGameplay.JumpController.CurrentPhase == JumpController.Phase.Descent;

    protected bool CanAirJump => !stateMachine.PlayerGameplay.IsGrounded
                                 && stateMachine.PlayerGameplay.JumpController.CanJump;
    
    public override void OnDisable()
    {
        base.OnDisable();
        stateMachine.PlayerGameplay.JumpController.End();
    }
    

    public override void FixedUpdate()
    {
        stateMachine.PlayerGameplay.ApplyAirHorizontalMovement();
        stateMachine.PlayerGameplay.JumpController.ApplyVerticalPhysics(stateMachine.PlayerGameplay.PlayerInputController.FastFall);
    }
}
