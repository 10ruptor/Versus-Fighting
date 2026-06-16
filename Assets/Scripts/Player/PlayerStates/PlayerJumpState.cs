using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void Enter()
    {
        playerGameplay.PlayerInputManager.ConsumeJumpRequest();
        playerGameplay.JumpController.ConsumeJump();
        playerGameplay.JumpController.Begin();
    }

    public override void Exit()
    {
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

        if (!playerGameplay.IsGrounded || playerGameplay.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (playerGameplay.PlayerInputManager.HasWalkInput)
            playerGameplay.StateMachine.ChangeState(new PlayerMoveState(playerGameplay));
        else
            playerGameplay.StateMachine.ChangeState(new PlayerIdleState(playerGameplay));
    }
}
