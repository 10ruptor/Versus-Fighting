using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    

    public PlayerAirState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Jump";
    protected bool IsLanding => playerGameplay.JumpController.CurrentPhase == JumpController.Phase.Descent;
    public override void RegisterTransition()
    {
       
    }

    public override void Enter()
    {
        base.Enter();
    }

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
