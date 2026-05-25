using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }

    public override void FixedUpdate()
    {
        player.ApplyHorizontalMovement();

        if (player.JumpRequested && player.IsGrounded && player.JumpController.CanJump)
        {
            player.StateMachine.ChangeState(new PlayerJumpState(player));
            return;
        }

        if (!player.HasMoveInput && player.IsGrounded)
            player.StateMachine.ChangeState(new PlayerIdleState(player));
    }
    public override void Enter()
    {
        base.Enter();
        player.animator.SetBool("Run",true);
    }

    public override void Exit()
    {
        player.animator.SetBool("Run",false);
    }
}
