using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected PlayerAirState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected bool IsLanding => playerGameplay.JumpController.CurrentPhase == JumpController.Phase.Descent;

    protected bool CanAirJump => !playerGameplay.IsGrounded
                                 && playerGameplay.JumpController.CanJump;
    
    public override void Exit()
    {
        base.Exit();
        playerGameplay.JumpController.End();
    }
    

    public override void FixedUpdate()
    {
        playerGameplay.ApplyAirHorizontalMovement();
        playerGameplay.JumpController.ApplyVerticalPhysics(playerGameplay.PlayerInputController.FastFall);
    }
}
