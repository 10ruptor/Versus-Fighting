using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void FixedUpdate()
    {
        Vector3 velocity = player.Rigidbody.linearVelocity;
        velocity.x = 0f;
        player.Rigidbody.linearVelocity = velocity;

        if (player.JumpRequested && player.IsGrounded && player.CanJump)
        {
            player.StateMachine.ChangeState(new PlayerJumpState(player));
            return;
        }

        if (player.HasMoveInput && player.IsGrounded)
            player.StateMachine.ChangeState(new PlayerMoveState(player));
    }
}
