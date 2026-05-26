using UnityEngine;
using UnityEditor.Animations;
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerGameplay playerGameplay) : base(playerGameplay) { }

    public override void FixedUpdate()
    {
        PlayerGameplay.ApplyHorizontalMovement();

        if (PlayerGameplay.PlayerInputController.JumpRequested && PlayerGameplay.IsGrounded && PlayerGameplay.JumpController.CanJump)
        {
            PlayerGameplay.StateMachine.ChangeState(new PlayerJumpState(PlayerGameplay));
            return;
        }

        if (!PlayerGameplay.PlayerInputController.HasMoveInput && PlayerGameplay.IsGrounded)
            PlayerGameplay.StateMachine.ChangeState(new PlayerIdleState(PlayerGameplay));
    }
    public override void Enter()
    {
        base.Enter();
        PlayerGameplay.animator.SetBool("Run",true);
    }

    public override void Exit()
    {
        PlayerGameplay.animator.SetBool("Run",false);
    }
}
