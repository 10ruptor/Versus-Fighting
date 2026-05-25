using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        player.ConsumeJump();
        player.BeginJump();
    }

    public override void Exit()
    {
        player.EndJumpPhysics();
    }

    public override void Update()
    {
        if (player.JumpPressedThisFrame && player.CanJump && !player.IsGrounded)
        {
            player.ConsumeJump();
            player.BeginJump();
        }
    }

    public override void FixedUpdate()
    {
        player.ApplyAirHorizontalMovement();
        player.ApplyJumpVerticalPhysics();

        if (!player.IsGrounded || player.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (player.HasMoveInput)
            player.StateMachine.ChangeState(new PlayerMoveState(player));
        else
            player.StateMachine.ChangeState(new PlayerIdleState(player));
    }
}
