using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void Enter()
    {
        PlayerGameplay.PlayerInputManager.ConsumeJumpRequest();
        PlayerGameplay.JumpController.ConsumeJump();
        PlayerGameplay.JumpController.Begin();
    }

    public override void Exit()
    {
        PlayerGameplay.JumpController.End();
    }

    public override void Update()
    {
        if (PlayerGameplay.PlayerInputManager.jump && PlayerGameplay.JumpController.CanJump && !PlayerGameplay.IsGrounded)
        {
            PlayerGameplay.PlayerInputManager.ConsumeJumpRequest();
            PlayerGameplay.JumpController.ConsumeJump();
            PlayerGameplay.JumpController.Begin();
        }
    }

    public override void FixedUpdate()
    {
        PlayerGameplay.ApplyAirHorizontalMovement();
        PlayerGameplay.JumpController.ApplyVerticalPhysics(PlayerGameplay.PlayerInputManager.fastFall);

        if (!PlayerGameplay.IsGrounded || PlayerGameplay.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (PlayerGameplay.PlayerInputManager.HasMoveInput)
            PlayerGameplay.StateMachine.ChangeState(new PlayerMoveState(PlayerGameplay));
        else
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
    }
}
