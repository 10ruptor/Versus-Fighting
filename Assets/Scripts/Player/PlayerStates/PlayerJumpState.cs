using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void Enter()
    {
        PlayerGameplay.PlayerInputController.ConsumeJumpRequest();
        PlayerGameplay.JumpController.ConsumeJump();
        PlayerGameplay.JumpController.Begin();
    }

    public override void Exit()
    {
        PlayerGameplay.JumpController.End();
    }

    public override void Update()
    {
        if (PlayerGameplay.PlayerInputController.JumpPressedThisFrame && PlayerGameplay.JumpController.CanJump && !PlayerGameplay.IsGrounded)
        {
            PlayerGameplay.PlayerInputController.ConsumeJumpRequest();
            PlayerGameplay.JumpController.ConsumeJump();
            PlayerGameplay.JumpController.Begin();
        }
    }

    public override void FixedUpdate()
    {
        PlayerGameplay.ApplyAirHorizontalMovement();
        PlayerGameplay.JumpController.ApplyVerticalPhysics(PlayerGameplay.PlayerInputController.IsFastFallHeld);

        if (!PlayerGameplay.IsGrounded || PlayerGameplay.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (PlayerGameplay.PlayerInputController.HasMoveInput)
            PlayerGameplay.StateMachine.ChangeState(new PlayerMoveState(PlayerGameplay));
        else
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
    }
}
