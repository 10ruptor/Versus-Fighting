using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        player.JumpController.ConsumeJump();
        player.JumpController.Begin();
    }

    public override void Exit()
    {
        player.JumpController.End();
    }

    public override void Update()
    {
        if (player.JumpPressedThisFrame && player.JumpController.CanJump && !player.IsGrounded)
        {
            player.JumpController.ConsumeJump();
            player.JumpController.Begin();
        }
    }

    public override void FixedUpdate()
    {
        player.ApplyAirHorizontalMovement();
        player.JumpController.ApplyVerticalPhysics(player.IsFastFallHeld);

        if (!player.IsGrounded || player.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (player.HasMoveInput)
            player.StateMachine.ChangeState(new PlayerMoveState(player));
        else
            player.StateMachine.ChangeState(new PlayerIdleState(player));
    }
}
