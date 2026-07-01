using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(PlayerGameplay playerGameplay) : base(playerGameplay) { }
    protected override string StateAnimationName => "Jump";
    public override void RegisterTransition()
    {
        AddTransition(() => playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerMoveState);
        AddTransition(() => playerGameplay.IsGrounded && !playerGameplay.PlayerInputManager.HasWalkInput,playerGameplay.playerIdleState);
    }

    public override void Enter()
    {
        base.Enter();
        playerGameplay.PlayerInputManager.ConsumeJumpRequest();
        playerGameplay.JumpController.ConsumeJump();
        playerGameplay.JumpController.PrepareJump();
    }

    public override void Exit()
    {
        base.Exit();
        playerGameplay.JumpController.End();
    }

    public override void Update()
    {
        if (playerGameplay.PlayerInputManager.jump && playerGameplay.JumpController.CanJump && !playerGameplay.IsGrounded)
        {
            playerGameplay.PlayerInputManager.ConsumeJumpRequest();
            playerGameplay.JumpController.ConsumeJump();
            playerGameplay.JumpController.Begin();
        }
    }

    public override void FixedUpdate()
    {
        playerGameplay.ApplyAirHorizontalMovement();
        playerGameplay.JumpController.ApplyVerticalPhysics(playerGameplay.PlayerInputManager.fastFall);

        if (playerGameplay.JumpController.CurrentPhase == JumpController.Phase.Start || !playerGameplay.IsGrounded || playerGameplay.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        CheckTransitions();
    }
}
