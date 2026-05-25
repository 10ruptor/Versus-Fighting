using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }

    public override void FixedUpdate()
    {
        player.ApplyHorizontalMovement();

        if (player.JumpRequested && player.IsGrounded)
        {
            player.StateMachine.ChangeState(new PlayerJumpState(player));
            return;
        }

        if (!player.HasMoveInput && player.IsGrounded)
            player.StateMachine.ChangeState(new PlayerIdleState(player));
    }
}
