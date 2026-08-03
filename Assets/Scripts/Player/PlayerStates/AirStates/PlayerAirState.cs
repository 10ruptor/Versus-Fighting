using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected PlayerAirState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected bool IsLanding => playerGameplay.JumpController.CurrentPhase == JumpController.Phase.Descent;
    
    public override void Exit()
    {
        base.Exit();
        playerGameplay.JumpController.End();
    }

    public override void Update()
    {
        if (playerGameplay.PlayerInputManager.Jump && playerGameplay.JumpController.CanJump && !playerGameplay.IsGrounded)
        {
            playerGameplay.PlayerInputManager.ConsumeJumpRequest();
            playerGameplay.JumpController.ConsumeJump();
            playerGameplay.JumpController.Begin();
        }
    }

    public override void FixedUpdate()
    {
        playerGameplay.ApplyAirHorizontalMovement();
        playerGameplay.JumpController.ApplyVerticalPhysics(playerGameplay.PlayerInputManager.FastFall);
    }
}
