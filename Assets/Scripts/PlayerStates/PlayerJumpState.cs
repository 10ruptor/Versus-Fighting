using UnityEngine;

public class PlayerJumpState : PlayerState
{
    const float LandingVelocityThreshold = 0.05f;

    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        Vector3 velocity = player.Rigidbody.linearVelocity;
        velocity.y = player.JumpForce;
        player.Rigidbody.linearVelocity = velocity;
    }

    public override void FixedUpdate()
    {
        player.ApplyHorizontalMovement();
        ApplyFastFall();

        if (!player.IsGrounded || player.Rigidbody.linearVelocity.y > LandingVelocityThreshold)
            return;

        if (player.HasMoveInput)
            player.StateMachine.ChangeState(new PlayerMoveState(player));
        else
            player.StateMachine.ChangeState(new PlayerIdleState(player));
    }

    void ApplyFastFall()
    {
        if (!player.IsFastFallHeld)
            return;

        Vector3 velocity = player.Rigidbody.linearVelocity;
        if (velocity.y >= 0f)
            return;

        velocity.y = Mathf.Min(velocity.y, -player.FastFallSpeed);
        player.Rigidbody.linearVelocity = velocity;
    }
}
